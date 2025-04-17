using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApi.Models;
using TravelApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // 获取用户的所有聊天记录
        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<ChatRecord>> GetUserChatRecords(long userId)
        {
            return Ok(_chatService.GetUserChatRecords(userId));
        }

        // 获取特定聊天记录及其所有消息
        [HttpGet("record/{recordId}")]
        public ActionResult<ChatRecord> GetChatRecord(int recordId)
        {
            var record = _chatService.GetChatRecord(recordId);
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }

        // 获取特定聊天记录的所有消息
        [HttpGet("record/{recordId}/messages")]
        public ActionResult<IEnumerable<ChatMessage>> GetChatMessages(int recordId)
        {
            return Ok(_chatService.GetChatMessages(recordId));
        }

        // 创建新的聊天记录
        [HttpPost("record")]
        public ActionResult<ChatRecord> CreateChatRecord(ChatRecord chatRecord)
        {
            _chatService.AddChatRecord(chatRecord);
            return CreatedAtAction(nameof(GetChatRecord), new { recordId = chatRecord.Id }, chatRecord);
        }

        // 添加新的聊天消息
        [HttpPost("record/{recordId}/message")]
        public ActionResult<ChatMessage> AddChatMessage(int recordId, ChatMessage message)
        {
            _chatService.AddChatMessage(recordId, message);
            return Ok(message);
        }

        // 删除聊天记录及其所有消息
        [HttpDelete("record/{recordId}")]
        public IActionResult DeleteChatRecord(int recordId)
        {
            _chatService.DeleteChatRecord(recordId);
            return NoContent();
        }
    }
} 