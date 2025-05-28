
import torch
import time
import torch.nn.functional as F
import torch.nn as nn
import math
from compressor.compressor import PastKey, PastValue, Compressor
# perform qk calculation and get indices
# this version will not update in inference mode
def split_heads_softmax(attn_weights, dim=-1, dtype=torch.float32):
    """
    分 attention heads 逐个计算 softmax，并合并结果。
    Args:
        attn_weights: 输入张量，形状 [batch_size, num_heads, seq_len_q, seq_len_k]
        dim: 进行 softmax 的维度，通常是 -1
        dtype: 输出的目标数据类型
    Returns:
        合并后的结果，形状 [batch_size, num_heads, seq_len_q, seq_len_k]
    """
    batch_size, num_heads, seq_len_q, seq_len_k = attn_weights.shape
    # 用来存储每个 head 的 softmax 结果
    head_softmax_results = []

    for head_idx in range(num_heads):
        # 提取每个 head 的权重，形状 [batch_size, seq_len_q, seq_len_k]
        single_head_weights = attn_weights[:, head_idx, :, :]
        # 对当前 head 进行 softmax
        single_head_softmax = nn.functional.softmax(single_head_weights, dim=dim, dtype=dtype)
        head_softmax_results.append(single_head_softmax)
        # head_softmax_results.append(single_head_softmax.to(device='cuda:1'))
    # del attn_weights
    # torch.cuda.empty_cache()
    # 合并所有 heads，形状 [batch_size, num_heads, seq_len_q, seq_len_k]
    attn_weights_softmax = torch.stack(head_softmax_results, dim=1)
    return attn_weights_softmax
# def split_heads_softmax(attn_weights, dim=-1, dtype=torch.float32):
#     """
#     分 attention heads 逐个计算 softmax，并合并结果。
#     Args:
#         attn_weights: 输入张量，形状 [batch_size, num_heads, seq_len_q, seq_len_k]
#         dim: 进行 softmax 的维度，通常是 -1
#         dtype: 输出的目标数据类型
#     Returns:
#         合并后的结果，形状 [batch_size, num_heads, seq_len_q, seq_len_k]
#     """
#     batch_size, num_heads, seq_len_q, seq_len_k = attn_weights.shape
#     # 初始化一个空的张量来存储拼接结果
#     attn_weights_softmax = None

#     for head_idx in range(num_heads):
#         # 提取每个 head 的权重，形状 [batch_size, seq_len_q, seq_len_k]
#         single_head_weights = attn_weights[:, head_idx, :, :]
#         # 对当前 head 进行 softmax
#         single_head_softmax = nn.functional.softmax(single_head_weights, dim=dim, dtype=dtype)
#         single_head_softmax = single_head_softmax.to(device='cuda:1')
        
#         # 如果 attn_weights_softmax 为空，则初始化它
#         if attn_weights_softmax is None:
#             attn_weights_softmax = single_head_softmax.unsqueeze(1)
#         else:
#             # 否则，将新的结果拼接到 attn_weights_softmax
#             attn_weights_softmax = torch.cat((attn_weights_softmax, single_head_softmax.unsqueeze(1)), dim=1)
        
#         # 删除已计算的部分以释放显存
#         del single_head_softmax
#         torch.cuda.empty_cache()
#     del attn_weights
#     torch.cuda.empty_cache()

#     return attn_weights_softmax
# Copied from transformers.models.llama.modeling_llama.repeat_kv
def repeat_kv(hidden_states: torch.Tensor, n_rep: int) -> torch.Tensor:
    """
    This is the equivalent of torch.repeat_interleave(x, dim=1, repeats=n_rep). The hidden states go from (batch,
    num_key_value_heads, seqlen, head_dim) to (batch, num_attention_heads, seqlen, head_dim)
    """
    batch, num_key_value_heads, slen, head_dim = hidden_states.shape
    if n_rep == 1:
        return hidden_states
    hidden_states = hidden_states[:, :, None, :, :].expand(batch, num_key_value_heads, n_rep, slen, head_dim)
    return hidden_states.reshape(batch, num_key_value_heads * n_rep, slen, head_dim)


class SpindleKVCluster():
    def __init__(self, num_hidden_layers = 32, window_size = 64, max_capacity_prompt = 256 + 64, kernel_size = 5, pooling = 'avgpool', beta = 20, num_layers = 80, layer_idx=None):
        
        self.layer_idx = layer_idx
        self.num_hidden_layers = num_hidden_layers
        
        self.steps = -1
        self.beta = beta
        
        self.window_size = window_size
        self.max_capacity_prompt = max_capacity_prompt
        if self.max_capacity_prompt - self.window_size < 0:
            self.window_size = self.max_capacity_prompt-4
            assert self.window_size > 0, f"window_size must be greater than 0, but got {self.window_size} in layer {layer_idx}, max_capacity_prompt {max_capacity_prompt}"
        # assert self.max_capacity_prompt - self.window_size > 0
        self.kernel_size = kernel_size
        self.pooling = pooling

    def reset(self, window_size = 64, max_capacity_prompt = 256 + 64, kernel_size = 5, pooling = 'avgpool'):
        self.window_size = window_size
        self.max_capacity_prompt = max_capacity_prompt
        assert self.max_capacity_prompt - self.window_size > 0
        self.kernel_size = kernel_size
        self.pooling = pooling

    def update_kv(self, unposition_key_states, unrepeated_key_states,unrepeated_value_states,key_states, query_states, value_states, attention_mask, num_key_value_groups,idx=None):
        
        # check if prefix phase
        assert key_states.shape[-2] == query_states.shape[-2]
        bsz, num_heads, q_len, head_dim = query_states.shape
        
        # TODO
        # window_sizes = 32
        min_num = (self.max_capacity_prompt - self.window_size) // self.beta
        max_num = (self.max_capacity_prompt - self.window_size) * 2 - min_num
        
            
        if max_num >= q_len - self.window_size:
            max_num = q_len - self.window_size
            min_num = (self.max_capacity_prompt - self.window_size) * 2 - max_num
    
       
        steps = (max_num - min_num) // (self.num_hidden_layers - 1)
        max_capacity_prompt = max_num - self.layer_idx * steps
        if not self.compressor.docompress or not self.layer_idx in self.compressor.prlayers:
            max_capacity_prompt = 1000000000
        min_context_length = 64
        distance_weight = 1.2
        streamingllm_sink_len = 4
        recent_len = int(self.compressor.image_count * self.compressor.pr_protect_ratio)+self.compressor.text_count
        
        if self.compressor.pr_strategy == "linear":
            schedule_prefill_decay_ratio = (1.0 - self.compressor.pr_decay_ratio) * (idx / self.compressor.layer_num) + self.compressor.pr_decay_ratio
        elif self.compressor.pr_strategy == "cosine":
            # print(f"idx: {idx}",)
            schedule_prefill_decay_ratio = (1.0 - self.compressor.pr_decay_ratio) * (math.cos(math.pi * idx / self.compressor.layer_num) + 1) / 2 + self.compressor.pr_decay_ratio
        else:
            schedule_prefill_decay_ratio = self.compressor.pr_decay_ratio
        # print(f'debug in SpindleKVCluster,layer is:{self.layer_idx},max_capacity_prompt is {max_capacity_prompt}, recent_len is {recent_len}, schedule_prefill_decay_ratio is {schedule_prefill_decay_ratio}')
        self.compressor.recent_len = recent_len
        if self.compressor.prmethod == 'pyramidinfer2':
            if not self.compressor.docompress:
                past_key = PastKey(None,unrepeated_key_states,None)
                past_value = PastValue(None,unrepeated_value_states,None)
                return past_key,past_value
            if q_len > recent_len :
                attn_weights = torch.matmul(query_states[..., -(recent_len+1):, :], key_states[...,:-(1 + recent_len),:].transpose(2, 3)) / math.sqrt(head_dim)
                if attention_mask is not None:
                    attn_weights = attn_weights + attention_mask
                attn_weights = split_heads_softmax(attn_weights, dim=-1, dtype=query_states.dtype)
            else :
                attn_weights = torch.matmul(query_states, key_states[...,:-(1 + recent_len),:].transpose(2, 3)) / math.sqrt(self.head_dim)
                if attention_mask is not None:
                    attn_weights = attn_weights + attention_mask
                # print(f'q_len小，attn_weights:{attn_weights.shape}')
                attn_weights = nn.functional.softmax(attn_weights, dim=-1, dtype=torch.float32).to(query_states.dtype)
                # attn_weights = split_heads_softmax(attn_weights, dim=-1, dtype=query_states.dtype)
            # attn_weights = attn_weights.mean(dim = 1).to(device=query_states.device) # [batch_size, seq_len_q, seq_len_k]
            # 这里我不希望直接将所有的attention head都平均到一起，而是希望同一个group中的attention head平均到一起
            # [batch_size, num_heads, seq_len_q, seq_len_k]
            head_num = self.compressor.head_num  #28 for qwen2
            kv_head_num = self.compressor.kv_head_num  #4 for qwen2
            group_size = head_num // kv_head_num # 7 for qwen2
            attn_weights = attn_weights.view(bsz, kv_head_num, group_size, attn_weights.shape[-2], attn_weights.shape[-1])
            attn_weights = attn_weights.mean(dim=2) # [batch_size, kv_head_num, seq_len_q, seq_len_k]
            
            attn_weights *= torch.linspace(1.0, distance_weight, attn_weights.shape[2], device=attn_weights.device)[None, None, :, None] # weight the attention weights by distance
            attn_weights = attn_weights.mean(dim=-2) # [batch_size, kv_head_num, seq_len_k]
            attn_weights[:, :, :streamingllm_sink_len] = torch.finfo(attn_weights.dtype).max # always keep the sink tokens
            if self.compressor.pyramidinfer2_last_context_length != 0 :
                context_length = self.compressor.pyramidinfer2_last_context_length
            else:
                context_length = attn_weights.shape[-1]  
              
            indices = None
            selected_position_ids = None
            if context_length > min_context_length and schedule_prefill_decay_ratio < 1.0 and idx in self.compressor.prlayers: 
                topk = int(context_length * schedule_prefill_decay_ratio) if int(context_length * schedule_prefill_decay_ratio) > min_context_length else context_length
                self.compressor.pyramidinfer2_last_context_length = topk
                indices = torch.topk(attn_weights, topk, dim=-1, largest=True, sorted=False).indices.sort(dim=-1).values # shape [batch_size, kv_head_num, topk]
            else :
                self.compressor.pyramidinfer2_last_context_length = context_length
            if indices is not None:
                if unposition_key_states is not None:
                    selected_position_ids = torch.arange(q_len, device=key_states.device).unsqueeze(0).unsqueeze(0).expand(bsz, query_states.shape[-3],  -1)
                    selected_position_ids = torch.gather(selected_position_ids, dim=-1, index=indices)
                    indices = indices.unsqueeze(-1).expand(-1, -1, -1, head_dim) # [batch_size, kv_head_num, topk, head_dim]
                    k_past_compress = unposition_key_states[:, :, :-(1+recent_len), :].gather(dim = 2, index = indices) # [batch_size, kv_head_num, topk, head_dim]
                else :
                    indices = indices.unsqueeze(-1).expand(-1, -1, -1, head_dim) # [batch_size, kv_head_num, topk, head_dim]
                    k_past_compress = unrepeated_key_states[:, :, :-(1+recent_len), :].gather(dim = 2, index = indices) # [batch_size, kv_head_num, topk, head_dim]
                    selected_position_ids = None
                v_past_compress = unrepeated_value_states[:, :, :-(recent_len+1), :].gather(dim = 2, index = indices) # [batch_size, kv_head_num, topk, head_dim]
            else :
                if unposition_key_states is not None:
                    selected_position_ids = torch.arange(q_len, device=key_states.device).unsqueeze(0).unsqueeze(0).expand(bsz, query_states.shape[-3],  -1)
                    k_past_compress = unposition_key_states[:, :, :-(1+recent_len), :]
                else :
                    k_past_compress = unrepeated_key_states[:, :, :-(1+recent_len), :]
                    selected_position_ids = None
                v_past_compress = unrepeated_value_states[:, :, :-(1+recent_len), :]
            k_cur = unrepeated_key_states[:, :, -(1+recent_len):, :] # [batch_size, kv_head_num, recent_len, head_dim]
            v_cur = unrepeated_value_states[:, :, -(1+recent_len):, :] # [batch_size, kv_head_num, recent_len, head_dim]
            compressedkey,compressedvalue,biaskey,biasvalue = self.compressor.compress(k_past_compress,v_past_compress,self.layer_idx,self.compressRatio,recent_len*k_cur.shape[-3])
            if unposition_key_states is None:
                key_states = torch.cat([compressedkey, k_cur], dim = 2)
                value_states = torch.cat([compressedvalue, v_cur], dim = 2)
                past_key = PastKey(None,key_states,None)
                past_value = PastValue(None,value_states,None)
            else:
                past_key = PastKey(compressedkey, k_cur, biaskey)
                past_value = PastValue(compressedvalue, v_cur, biasvalue)
                self.compressor.selected_position_idss.append(selected_position_ids)
            self.compressor.recent2context_topk_indices=None
            self.compressor.recent_length=None
            return past_key,past_value
        elif self.compressor.prmethod == 'pyramidinfer':
            if not self.compressor.docompress:
                past_key = PastKey(None,unrepeated_key_states,None)
                past_value = PastValue(None,unrepeated_value_states,None)
                return past_key,past_value
            if q_len > recent_len :
                # print(f'debug query_states:{query_states.shape}，recent_len:{recent_len}')
                # print(f'debug query_states_to_matmul:{query_states_to_matmul.shape},key_states:{key_states.shape}')
                attn_weights = torch.matmul(query_states[..., -(recent_len+1):, :], key_states[...,:-(1 + recent_len),:].transpose(2, 3)) / math.sqrt(head_dim)
                # batch_size, num_heads, seq_len, head_dim = query_states.shape

                # # 初始化一个列表来存储每个 attention head 的结果
                # attn_weights_list = []

                # # 对每个 attention head 进行计算
                # for head in range(num_heads):
                #     attn_weights_head = torch.matmul(
                #         query_states[:, head, -(recent_len + 1):, :],
                #         key_states[:, head, :-(1 + recent_len), :].transpose(-1, -2)  # 使用正确的维度索引
                #     ) / math.sqrt(head_dim)
                #     attn_weights_list.append(attn_weights_head.to(device='cuda:2'))

                # attn_weights = torch.stack(attn_weights_list, dim=1)
                # del attn_weights_list
                # torch.cuda.empty_cache()
                
                # if attention_mask is not None:
                #     print(f'attn_weights:{attn_weights.shape},attention_mask:{attention_mask.shape},recent_len:{recent_len}')
                #     attn_weights = attn_weights + attention_mask[...,-(recent_len+1):,:]
                # print(f'q_len大，attn_weights:{attn_weights.shape}')
                # attn_weights = nn.functional.softmax(attn_weights, dim=-1, dtype=torch.float32).to(query_states.dtype)
                attn_weights = split_heads_softmax(attn_weights, dim=-1, dtype=query_states.dtype)
            else :
                attn_weights = torch.matmul(query_states, key_states[...,:-(1 + recent_len),:].transpose(2, 3)) / math.sqrt(self.head_dim)
                if attention_mask is not None:
                    attn_weights = attn_weights + attention_mask
                # print(f'q_len小，attn_weights:{attn_weights.shape}')
                attn_weights = nn.functional.softmax(attn_weights, dim=-1, dtype=torch.float32).to(query_states.dtype)
                # attn_weights = split_heads_softmax(attn_weights, dim=-1, dtype=query_states.dtype)
                
            attn_weights = attn_weights.mean(dim = 1).to(device=query_states.device)
            
            # recent2context_attn_weights = attn_weights[:, :, :-(1 + recent_len) ]
            recent2context_attn_weights = attn_weights
            recent2context_attn_weights *= torch.linspace(1.0, distance_weight, recent2context_attn_weights.shape[1], device=recent2context_attn_weights.device)[None, :, None] # weight the recent2context attention weights by distance
            recent2context_attn_weights = recent2context_attn_weights.mean(dim=-2) 
            recent2context_attn_weights[:, :streamingllm_sink_len] = torch.finfo(recent2context_attn_weights.dtype).max # always keep the sink tokens
            context_length = recent2context_attn_weights.shape[-1]  
              
            recent2context_topk_indices = None
            new_selected_position_ids = None
            if idx == 0 :
                selected_position_ids = torch.arange(self.compressor.kv_seq_len, device=recent2context_attn_weights.device).unsqueeze(0).expand(query_states.shape[0], -1)
                self.compressor.selected_position_idss.append(selected_position_ids)
            if context_length > min_context_length and schedule_prefill_decay_ratio < 1.0 and idx in self.compressor.prlayers: 
                topk = int(context_length * schedule_prefill_decay_ratio) if int(context_length * schedule_prefill_decay_ratio) > min_context_length else context_length
                recent2context_topk_indices = torch.topk(recent2context_attn_weights, topk, dim=-1, largest=True, sorted=False).indices.sort(dim=-1).values

            if unposition_key_states is not None:
                # print(f'debug in eviction,unposition_key_states:{unposition_key_states.shape}')
                k_past_compress = unposition_key_states[:, :, :-(1+recent_len), :]
            else :
                # print(f'debug in eviction,position_key_states:{key_states.shape}')
                if unrepeated_value_states is not None:
                    k_past_compress = unrepeated_key_states[:, :, :-(1+recent_len), :]
                else :
                    k_past_compress = key_states[:, :, :-(1+recent_len), :]
            if unrepeated_value_states is not None:
                v_past_compress = unrepeated_value_states[:, :, :-(1+recent_len), :]
            else :
                v_past_compress = value_states[:, :, :-(1+recent_len), :]
            # print(f'debug in eviction,k_past_compress:{k_past_compress.shape}')
            # print(f'debug in eviction,v_past_compress:{v_past_compress.shape}')
            if unrepeated_key_states is not None:
                k_cur = unrepeated_key_states[:, :, -(1+recent_len):, :]
                v_cur = unrepeated_value_states[:, :, -(1+recent_len):, :]
            else:
                k_cur = key_states[:, :, -(1+recent_len):, :]
                v_cur = value_states[:, :,-(1+recent_len):, :]
            compressedkey,compressedvalue,biaskey,biasvalue = self.compressor.compress(k_past_compress,v_past_compress,self.layer_idx,self.compressRatio,(recent_len+1)*k_cur.shape[-3])
            if unposition_key_states is None:
                key_states = torch.cat([compressedkey, k_cur], dim = 2)
                value_states = torch.cat([compressedvalue, v_cur], dim = 2)
                past_key = PastKey(None,key_states,None)
                past_value = PastValue(None,value_states,None)
            else:
                past_key = PastKey(compressedkey,k_cur,biaskey)
                past_value = PastValue(compressedvalue,v_cur,biasvalue)
            selected_position_ids =  self.compressor.selected_position_idss[-1]
            if recent2context_topk_indices is not None :
                new_selected_position_ids = torch.cat([
                                    torch.gather(selected_position_ids[:, :-(1 + recent_len)], dim=-1, index=recent2context_topk_indices.to(selected_position_ids.device)),
                                    selected_position_ids[:, -(1 + recent_len):],
                                ], dim=-1)
            self.compressor.selected_position_idss.append(new_selected_position_ids if new_selected_position_ids is not None else self.compressor.selected_position_idss[-1])
            self.compressor.recent2context_topk_indices=recent2context_topk_indices
            self.compressor.recent_length=recent_len
            return past_key,past_value
        elif self.compressor.prmethod == "pyramidKV":
            # print(f"PyramidKV max_capacity_prompt {max_capacity_prompt}")
            if q_len < max_capacity_prompt:
                self.compressor.recent2context_topk_indices=None
                self.compressor.recent_length=None
                past_key = PastKey(None,key_states,None)
                past_value = PastValue(None,value_states,None)
                return past_key,past_value
            elif q_len < (max_capacity_prompt - self.window_size) * 2:
                attn_weights = torch.matmul(query_states[..., -self.window_size:, :], key_states.transpose(2, 3)) / math.sqrt(head_dim)
                mask = torch.full((self.window_size, self.window_size), torch.finfo(attn_weights.dtype).min, device=attn_weights.device)
                mask_cond = torch.arange(mask.size(-1), device=attn_weights.device)
                mask.masked_fill_(mask_cond < (mask_cond + 1).view(mask.size(-1), 1), 0)
                mask = mask.to(attn_weights.device)
                attention_mask = mask[None, None, :, :]

                attn_weights[:, :, -self.window_size:, -self.window_size:] += attention_mask

                attn_weights = nn.functional.softmax(attn_weights, dim=-1, dtype=torch.float32).to(query_states.dtype)
                attn_weights_sum = attn_weights[:, :, -self.window_size:, : -self.window_size].sum(dim = -2)
                if self.pooling == 'avgpool':
                    attn_cache = F.avg_pool1d(attn_weights_sum, kernel_size = self.kernel_size, padding=self.kernel_size//2, stride=1)
                elif self.pooling == 'maxpool':
                    attn_cache = F.max_pool1d(attn_weights_sum, kernel_size = self.kernel_size, padding=self.kernel_size//2, stride=1)
                else:
                    raise ValueError('Pooling method not supported')
                indices = attn_cache.topk(max_capacity_prompt - self.window_size, dim=-1).indices
                if unposition_key_states is not None:
                    # print(f'debug in eviction,indices:{indices.shape}')
                    selected_position_ids = torch.arange(q_len, device=key_states.device).unsqueeze(0).unsqueeze(0).expand(bsz, query_states.shape[-3],  -1)
                    # print(f'debug in eviction,selected_position_ids:{selected_position_ids.shape}')
                    selected_position_ids = torch.gather(selected_position_ids, dim=-1, index=indices)
                    # print(f'debug in eviction,selected_position_ids:{selected_position_ids.shape}')
                    indices = indices.unsqueeze(-1).expand(-1, -1, -1, head_dim)
                    # print(f'debug in eviction,indices:{indices.shape}')
                    k_past_compress = unposition_key_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
                else :
                    indices = indices.unsqueeze(-1).expand(-1, -1, -1, head_dim)
                    k_past_compress = key_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
                    selected_position_ids = None
                v_past_compress = value_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
                # print(f'debug in eviction,k_past_compress:{k_past_compress.shape}')
                # print(f'debug in eviction,v_past_compress:{v_past_compress.shape}')
                # k_past_compress = repeat_kv(k_past_compress[:,:8], 4)
                # print(f'debug in eviction,k_past_compress:{k_past_compress.shape}')
                # k_past_compress = k_past_compress[:,:8]
                # print(k_past_compress.shape)
                # # key = repeat_kv(key,4)
                # k_past_compress = k_past_compress.repeat(1,4,1,1)
                # print(k_past_compress.shape)
                k_cur = key_states[:, :, -self.window_size:, :]
                v_cur = value_states[:, :, -self.window_size:, :]
                compressedkey,compressedvalue,biaskey,biasvalue = self.compressor.compress(k_past_compress,v_past_compress,self.layer_idx,self.compressRatio,self.window_size*k_cur.shape[-3])
                # compressedkey,compressedvalue,biaskey,biasvalue = self.compressor.compress(k_past_compress,v_past_compress,self.layer_idx,self.compressRatio,self.window_size*self.compressor.kv_head_num)
                if unposition_key_states is None:
                    key_states = torch.cat([compressedkey, k_cur], dim = 2)
                    value_states = torch.cat([compressedvalue, v_cur], dim = 2)
                    past_key = PastKey(None,key_states,None)
                    past_value = PastValue(None,value_states,None)
                else:
                    past_key = PastKey(compressedkey,k_cur,biaskey)
                    past_value = PastValue(compressedvalue,v_cur,biasvalue)
                    self.compressor.selected_position_idss.append(selected_position_ids)
                self.compressor.recent2context_topk_indices=None
                self.compressor.recent_length=None
                return past_key,past_value
            else:
                if not self.compressor.docompress:
                    past_key = PastKey(None,key_states,None)
                    past_value = PastValue(None,value_states,None)
                    self.compressor.recent2context_topk_indices=None
                    self.compressor.recent_length=None
                    return past_key,past_value                           
                attn_weights = torch.matmul(query_states[..., -self.window_size:, :], key_states.transpose(2, 3)) / math.sqrt(head_dim)
                mask = torch.full((self.window_size, self.window_size), torch.finfo(attn_weights.dtype).min, device=attn_weights.device)
                mask_cond = torch.arange(mask.size(-1), device=attn_weights.device)
                mask.masked_fill_(mask_cond < (mask_cond + 1).view(mask.size(-1), 1), 0)
                mask = mask.to(attn_weights.device)
                attention_mask = mask[None, None, :, :]

                attn_weights[:, :, -self.window_size:, -self.window_size:] += attention_mask

                attn_weights = nn.functional.softmax(attn_weights, dim=-1, dtype=torch.float32).to(query_states.dtype)
                attn_weights_sum = attn_weights[:, :, -self.window_size:, : -self.window_size].sum(dim = -2)
                if self.pooling == 'avgpool':
                    attn_cache = F.avg_pool1d(attn_weights_sum, kernel_size = self.kernel_size, padding=self.kernel_size//2, stride=1)
                elif self.pooling == 'maxpool':
                    attn_cache = F.max_pool1d(attn_weights_sum, kernel_size = self.kernel_size, padding=self.kernel_size//2, stride=1)
                else:
                    raise ValueError('Pooling method not supported')
                indices = attn_cache.topk(max_capacity_prompt, dim=-1).indices
                if unposition_key_states is not None:
                    # print(f'debug in eviction,indices:{indices.shape}')
                    selected_position_ids = torch.arange(q_len, device=key_states.device).unsqueeze(0).unsqueeze(0).expand(bsz, query_states.shape[-3],  -1)
                    # print(f'debug in eviction,selected_position_ids:{selected_position_ids.shape}')
                    selected_position_ids = torch.gather(selected_position_ids, dim=-1, index=indices)
                    # print(f'debug in eviction,selected_position_ids:{selected_position_ids.shape}')
                    indices = indices.unsqueeze(-1).expand(-1, -1, -1, head_dim)
                    # print(f'debug in eviction,indices:{indices.shape}')
                    k_past_compress = unposition_key_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
                else :
                    indices = indices.unsqueeze(-1).expand(-1, -1, -1, head_dim)
                    k_past_compress = key_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
                    selected_position_ids = None
                v_past_compress = value_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
                # print(f'debug in eviction,k_past_compress:{k_past_compress.shape}')
                # print(f'debug in eviction,v_past_compress:{v_past_compress.shape}')
                k_cur = key_states[:, :, -self.window_size:, :]
                v_cur = value_states[:, :, -self.window_size:, :]
                compressedkey,compressedvalue,biaskey,biasvalue = self.compressor.compress(k_past_compress,v_past_compress,self.layer_idx,self.compressRatio,self.window_size*k_cur.shape[-3])
                # compressedkey,compressedvalue,biaskey,biasvalue = self.compressor.compress(k_past_compress,v_past_compress,self.layer_idx,self.compressRatio,self.window_size*self.compressor.kv_head_num)
                if unposition_key_states is None:
                    key_states = torch.cat([compressedkey, k_cur], dim = 2)
                    value_states = torch.cat([compressedvalue, v_cur], dim = 2)
                    past_key = PastKey(None,key_states,None)
                    past_value = PastValue(None,value_states,None)
                else:
                    past_key = PastKey(compressedkey,k_cur,biaskey)
                    past_value = PastValue(compressedvalue,v_cur,biasvalue)
                    self.compressor.selected_position_idss.append(selected_position_ids)
                self.compressor.recent2context_topk_indices=None
                self.compressor.recent_length=None
                return past_key,past_value
                # position_key_states = torch.cat([k_past_compress, k_cur], dim = 2)
                # value_states = torch.cat([v_past_compress, v_cur], dim = 2)
                # return position_key_states, value_states

class SnapKVCluster():
    def __init__(self, window_size = 64, max_capacity_prompt = 256 + 64, kernel_size = 5, pooling = 'avgpool'):
        self.window_size = window_size
        self.max_capacity_prompt = max_capacity_prompt
        assert self.max_capacity_prompt - self.window_size > 0
        self.kernel_size = kernel_size
        self.pooling = pooling

    def reset(self, window_size = 64, max_capacity_prompt = 256 + 64, kernel_size = 5, pooling = 'avgpool'):
        self.window_size = window_size
        self.max_capacity_prompt = max_capacity_prompt
        assert self.max_capacity_prompt - self.window_size > 0
        self.kernel_size = kernel_size
        self.pooling = pooling

    def update_kv(self, key_states, query_states, value_states, attention_mask, num_key_value_groups):
        
        # check if prefix phase
        assert key_states.shape[-2] == query_states.shape[-2]
        bsz, num_heads, q_len, head_dim = query_states.shape
        
        print(f"SnapKV max_capacity_prompt {self.max_capacity_prompt}")
        
        if q_len < self.max_capacity_prompt:
            return key_states, value_states
        else:
            attn_weights = torch.matmul(query_states[..., -self.window_size:, :], key_states.transpose(2, 3)) / math.sqrt(head_dim)
            mask = torch.full((self.window_size, self.window_size), torch.finfo(attn_weights.dtype).min, device=attn_weights.device)
            mask_cond = torch.arange(mask.size(-1), device=attn_weights.device)
            mask.masked_fill_(mask_cond < (mask_cond + 1).view(mask.size(-1), 1), 0)
            mask = mask.to(attn_weights.device)
            attention_mask = mask[None, None, :, :]

            attn_weights[:, :, -self.window_size:, -self.window_size:] += attention_mask

            attn_weights = nn.functional.softmax(attn_weights, dim=-1, dtype=torch.float32).to(query_states.dtype)
            attn_weights_sum = attn_weights[:, :, -self.window_size:, : -self.window_size].sum(dim = -2)
            if self.pooling == 'avgpool':
                attn_cache = F.avg_pool1d(attn_weights_sum, kernel_size = self.kernel_size, padding=self.kernel_size//2, stride=1)
            elif self.pooling == 'maxpool':
                attn_cache = F.max_pool1d(attn_weights_sum, kernel_size = self.kernel_size, padding=self.kernel_size//2, stride=1)
            else:
                raise ValueError('Pooling method not supported')
            indices = attn_cache.topk(self.max_capacity_prompt - self.window_size, dim=-1).indices
            indices = indices.unsqueeze(-1).expand(-1, -1, -1, head_dim)
            k_past_compress = key_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
            v_past_compress = value_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
            k_cur = key_states[:, :, -self.window_size:, :]
            v_cur = value_states[:, :, -self.window_size:, :]
            key_states = torch.cat([k_past_compress, k_cur], dim = 2)
            value_states = torch.cat([v_past_compress, v_cur], dim = 2)
            return key_states, value_states


class H2OKVCluster():
    def __init__(self, window_size = 64, max_capacity_prompt = 256 + 64, kernel_size = 5, pooling = 'avgpool'):
        self.window_size = window_size
        self.max_capacity_prompt = max_capacity_prompt
        assert self.max_capacity_prompt - self.window_size > 0
        self.kernel_size = kernel_size
        self.pooling = pooling

    def reset(self, window_size = 64, max_capacity_prompt = 256 + 64, kernel_size = 5, pooling = 'avgpool'):
        self.window_size = window_size
        self.max_capacity_prompt = max_capacity_prompt
        assert self.max_capacity_prompt - self.window_size > 0
        self.kernel_size = kernel_size
        self.pooling = pooling

    def update_kv(self, key_states, query_states, value_states, attention_mask, num_key_value_groups):
        
        # check if prefix phase
        assert key_states.shape[-2] == query_states.shape[-2]
        bsz, num_heads, q_len, head_dim = query_states.shape
        
        print(f"H2O max_capacity_prompt {self.max_capacity_prompt}")
        
        if q_len < self.max_capacity_prompt:
            return key_states, value_states
        else:
            attn_weights = torch.matmul(query_states, key_states.transpose(2, 3)) / math.sqrt(head_dim)
            mask = torch.full((self.window_size, self.window_size), torch.finfo(attn_weights.dtype).min, device=attn_weights.device)
            mask_cond = torch.arange(mask.size(-1), device=attn_weights.device)
            mask.masked_fill_(mask_cond < (mask_cond + 1).view(mask.size(-1), 1), 0)
            mask = mask.to(attn_weights.device)
            attention_mask = mask[None, None, :, :]

            attn_weights[:, :, -self.window_size:, -self.window_size:] += attention_mask

            attn_weights = nn.functional.softmax(attn_weights, dim=-1, dtype=torch.float32).to(query_states.dtype)
            attn_weights_sum = attn_weights[:, :, :, : -self.window_size].sum(dim = -2)
            # if self.pooling == 'avgpool':
            #     attn_cache = F.avg_pool1d(attn_weights_sum, kernel_size = self.kernel_size, padding=self.kernel_size//2, stride=1)
            # elif self.pooling == 'maxpool':
            #     attn_cache = F.max_pool1d(attn_weights_sum, kernel_size = self.kernel_size, padding=self.kernel_size//2, stride=1)
            # else:
            #     raise ValueError('Pooling method not supported')
            attn_cache = attn_weights_sum
            indices = attn_cache.topk(self.max_capacity_prompt - self.window_size, dim=-1).indices
            indices = indices.unsqueeze(-1).expand(-1, -1, -1, head_dim)
            k_past_compress = key_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
            v_past_compress = value_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
            k_cur = key_states[:, :, -self.window_size:, :]
            v_cur = value_states[:, :, -self.window_size:, :]
            key_states = torch.cat([k_past_compress, k_cur], dim = 2)
            value_states = torch.cat([v_past_compress, v_cur], dim = 2)
            return key_states, value_states


class StreamingLLMKVCluster():
    def __init__(self, window_size = 64, max_capacity_prompt = 256 + 64, kernel_size = 5, pooling = 'avgpool'):
        self.window_size = window_size
        self.max_capacity_prompt = max_capacity_prompt
        assert self.max_capacity_prompt - self.window_size > 0
        self.kernel_size = kernel_size
        self.pooling = pooling

    def reset(self, window_size = 64, max_capacity_prompt = 256 + 64, kernel_size = 5, pooling = 'avgpool'):
        self.window_size = window_size
        self.max_capacity_prompt = max_capacity_prompt
        assert self.max_capacity_prompt - self.window_size > 0
        self.kernel_size = kernel_size
        self.pooling = pooling

    def update_kv(self, key_states, query_states, value_states, attention_mask, num_key_value_groups):
        
        # check if prefix phase
        assert key_states.shape[-2] == query_states.shape[-2]
        bsz, num_heads, q_len, head_dim = query_states.shape
        
        print(f"StreamingLLM max_capacity_prompt {self.max_capacity_prompt}")
        
        if q_len < self.max_capacity_prompt:
            return key_states, value_states
        else:
            # attn_weights = torch.matmul(query_states[..., -self.window_size:, :], key_states.transpose(2, 3)) / math.sqrt(head_dim)
            # mask = torch.full((self.window_size, self.window_size), torch.finfo(attn_weights.dtype).min, device=attn_weights.device)
            # mask_cond = torch.arange(mask.size(-1), device=attn_weights.device)
            # mask.masked_fill_(mask_cond < (mask_cond + 1).view(mask.size(-1), 1), 0)
            # mask = mask.to(attn_weights.device)
            # attention_mask = mask[None, None, :, :]

            # attn_weights[:, :, -self.window_size:, -self.window_size:] += attention_mask

            # attn_weights = nn.functional.softmax(attn_weights, dim=-1, dtype=torch.float32).to(query_states.dtype)
            # attn_weights_sum = attn_weights[:, :, :, : -self.window_size].sum(dim = -2)
            # if self.pooling == 'avgpool':
            #     attn_cache = F.avg_pool1d(attn_weights_sum, kernel_size = self.kernel_size, padding=self.kernel_size//2, stride=1)
            # elif self.pooling == 'maxpool':
            #     attn_cache = F.max_pool1d(attn_weights_sum, kernel_size = self.kernel_size, padding=self.kernel_size//2, stride=1)
            # else:
            #     raise ValueError('Pooling method not supported')
            # attn_cache = attn_weights_sum
            # indices = attn_cache.topk(self.max_capacity_prompt - self.window_size, dim=-1).indices
            
            
            indices = torch.tensor(range(self.max_capacity_prompt - self.window_size), dtype=torch.int64).to(key_states.device)
            indices = indices.unsqueeze(0).unsqueeze(0).unsqueeze(-1).repeat(bsz, num_heads, 1, head_dim)

            k_past_compress = key_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
            v_past_compress = value_states[:, :, :-self.window_size, :].gather(dim = 2, index = indices)
            k_cur = key_states[:, :, -self.window_size:, :]
            v_cur = value_states[:, :, -self.window_size:, :]
            key_states = torch.cat([k_past_compress, k_cur], dim = 2)
            value_states = torch.cat([v_past_compress, v_cur], dim = 2)
            return key_states, value_states


def init_spindlekv(self, num_hidden_layers):
    if not hasattr(self, "kv_cluster"):
        # print(f'{self.config}')
        if not hasattr(self.config, 'window_size'):
            self.config.window_size = 32
        if not hasattr(self.config, 'max_capacity_prompt'):
            self.config.max_capacity_prompt = 2048
        if not hasattr(self.config, 'kernel_size'):
            self.config.kernel_size = 5
        if not hasattr(self.config, 'pooling'):
            self.config.pooling = 'avgpool'
    
    self.kv_cluster = SpindleKVCluster( 
        num_hidden_layers = num_hidden_layers,
        layer_idx = self.layer_idx,
        window_size = self.config.window_size, 
        max_capacity_prompt = self.config.max_capacity_prompt, 
        kernel_size = self.config.kernel_size,
        pooling = self.config.pooling
        )
    self.kv_cluster.compressor = self.compressor
    self.kv_cluster.compressRatio = self.compressRatio
 
def init_snapkv(self):
    if not hasattr(self, "kv_cluster"):
        if not hasattr(self.config, 'window_size'):
            self.config.window_size = 32
        if not hasattr(self.config, 'max_capacity_prompt'):
            self.config.max_capacity_prompt = 4096
        if not hasattr(self.config, 'kernel_size'):
            self.config.kernel_size = 5
        if not hasattr(self.config, 'pooling'):
            self.config.pooling = 'avgpool'
    
    
    self.kv_cluster = SnapKVCluster( 
        window_size = self.config.window_size, 
        max_capacity_prompt = self.config.max_capacity_prompt, 
        kernel_size = self.config.kernel_size,
        pooling = self.config.pooling
        )

def init_H2O(self):
    if not hasattr(self, "kv_cluster"):
        if not hasattr(self.config, 'window_size'):
            self.config.window_size = 32
        if not hasattr(self.config, 'max_capacity_prompt'):
            self.config.max_capacity_prompt = 2048
        if not hasattr(self.config, 'kernel_size'):
            self.config.kernel_size = 5
        if not hasattr(self.config, 'pooling'):
            self.config.pooling = 'avgpool'
    
    
    self.kv_cluster = H2OKVCluster(
        window_size = self.config.window_size, 
        max_capacity_prompt = self.config.max_capacity_prompt, 
        kernel_size = self.config.kernel_size,
        pooling = self.config.pooling
        )

def init_StreamingLLM(self):
    if not hasattr(self, "kv_cluster"):
        if not hasattr(self.config, 'window_size'):
            self.config.window_size = 32
        if not hasattr(self.config, 'max_capacity_prompt'):
            self.config.max_capacity_prompt = 2048
        if not hasattr(self.config, 'kernel_size'):
            self.config.kernel_size = 5
        if not hasattr(self.config, 'pooling'):
            self.config.pooling = 'avgpool'
    
    
    self.kv_cluster = StreamingLLMKVCluster(
        window_size = self.config.window_size, 
        max_capacity_prompt = self.config.max_capacity_prompt, 
        kernel_size = self.config.kernel_size,
        pooling = self.config.pooling
        )
