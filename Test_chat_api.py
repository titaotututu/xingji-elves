import requests
import json

BASE_URL = "http://127.0.0.1:8000"

def create_chat_session(user_id):
    url = f"{BASE_URL}/chat/session"
    payload = {"user_id": user_id}
    headers = {"Content-Type": "application/json"}
    response = requests.post(url, data=json.dumps(payload), headers=headers)
    if response.status_code == 200:
        print("创建聊天会话成功:", response.json())
        return response.json()["SessionId"]
    else:
        print("创建聊天会话失败:", response.text)
        return None

def send_chat_message(session_id, input_text):
    url = f"{BASE_URL}/chat"
    payload = {"session_id": session_id, "input": input_text}
    headers = {"Content-Type": "application/json"}
    response = requests.post(url, data=json.dumps(payload), headers=headers)
    if response.status_code == 200:
        print("发送聊天消息成功:", response.json())
    else:
        print("发送聊天消息失败:", response.text)

def get_chat_history(session_id):
    url = f"{BASE_URL}/chat/session/{session_id}"
    response = requests.get(url)
    if response.status_code == 200:
        print("获取聊天历史记录成功:", response.json())
    else:
        print("获取聊天历史记录失败:", response.text)

def get_user_chat_history(user_id):
    url = f"{BASE_URL}/chat/user/{user_id}"
    response = requests.get(url)
    if response.status_code == 200:
        print("获取用户聊天历史记录成功:", response.json())
    else:
        print("获取用户聊天历史记录失败:", response.text)
        
if __name__ == "__main__":
    # Step 1: 创建两个聊天会话
    user_id = 1  # 假设用户 ID 为 1
    session_id_1 = create_chat_session(user_id)
    session_id_2 = create_chat_session(user_id)

    if session_id_1 and session_id_2:
        # Step 2: 在第一个会话中发送消息
        send_chat_message(session_id_1, "你好")
        send_chat_message(session_id_1, "最近有什么旅游计划吗？")

        # Step 3: 在第二个会话中发送消息
        send_chat_message(session_id_2, "你好")
        send_chat_message(session_id_2, "武汉有没有什么好玩的地方？")

        # Step 4: 查询第一个会话的聊天历史记录
        get_chat_history(session_id_1)

        # Step 5: 查询第二个会话的聊天历史记录
        get_chat_history(session_id_2)

        # Step 6: 查询用户的所有聊天历史记录
        get_user_chat_history(user_id)