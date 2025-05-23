from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import pymysql
from typing import List
from datetime import datetime
from fastapi.responses import JSONResponse
import requests
from zhipuai import ZhipuAI

OPENAI_API_KEY = "aafe39764e0046ea9666bfeb969bdb8e.lUjC0Dcy7hocpl3v"
BASE_URL = "https://open.bigmodel.cn/api/paas/v4/chat/completions"
MODEL = "GLM-4-Flash"
client = ZhipuAI(api_key=OPENAI_API_KEY) 
# 示例
# response = client.chat.completions.create(
#     model="glm-4-plus",  # 请填写您要调用的模型名称
#     messages=[
#         {"role": "user", "content": "作为一名营销专家，请为我的产品创作一个吸引人的口号"},
#         {"role": "assistant", "content": "当然，要创作一个吸引人的口号，请告诉我一些关于您产品的信息"},
#         {"role": "user", "content": "智谱AI开放平台"},
#         {"role": "assistant", "content": "点燃未来，智谱AI绘制无限，让创新触手可及！"},
#         {"role": "user", "content": "创作一个更精准且吸引人的口号"}
#     ],
# )
# print(response)
# Completion(model='glm-4-plus', created=1747971865, choices=[CompletionChoice(index=0, finish_reason='stop', message=CompletionMessage(content='"智启未来，谱绘可能 —— 智谱AI，让每一行代码赋能世界！"\n\n这个口号旨在强调智谱AI开放平一行代码赋能世界！"\n\n这个口号旨在强调智谱AI开放平台的核心价值：智能化、创新性和赋能性。它既突出了平台对未来科技的引领作用，又表达了通过代码实现无限可能的理念，吸引开发者和企业关注并使用该平台。', role='assistant', tool_calls=None))], request_id='20250523114423337af8c492594978', id='20250523114423337af8c492594978', usage=CompletionUsage(prompt_tokens=73, completion_tokens=73, total_tokens=146))


# 连接 MySQL 数据库
def get_db_connection(database=None):
    try:
        conn = pymysql.connect(
            host="localhost",
            user="root",
            password="tyxqc",
            database=database if database else "mysql",  # 默认连接到 mysql 数据库
            port=3306,
            charset="utf8mb4"
        )
        print("Database connection successful")
        return conn
    except pymysql.MySQLError as err:
        print(f"Error: {err}")
        raise HTTPException(status_code=500, detail=f"Database connection failed: {err}")

# 确保数据库和表存在
def setup_database():
    try:
        # 创建数据库
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute("CREATE DATABASE IF NOT EXISTS server_db;")
        conn.close()

        # 创建表
        conn = get_db_connection(database="server_db")
        cursor = conn.cursor()

        # 用户表
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS users (
                UserId BIGINT PRIMARY KEY AUTO_INCREMENT,
                UserName VARCHAR(50) NOT NULL,
                Password VARCHAR(50) NOT NULL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
            );
        """)

        # 聊天会话表
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS chat_sessions (
                SessionId BIGINT PRIMARY KEY AUTO_INCREMENT,
                UserId BIGINT NOT NULL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (UserId) REFERENCES users(UserId)
            );
        """)

        # 聊天消息表
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS chat_messages (
                MessageId BIGINT PRIMARY KEY AUTO_INCREMENT,
                SessionId BIGINT NOT NULL,
                Sender VARCHAR(10) NOT NULL, -- "user" 或 "model"
                Content TEXT NOT NULL,
                Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (SessionId) REFERENCES chat_sessions(SessionId)
            );
        """)

        conn.commit()
        cursor.close()
        conn.close()
        print("Database setup completed")
    except Exception as e:
        print(f"Error during database setup: {e}")
        raise HTTPException(status_code=500, detail=f"Database setup failed: {e}")

# 初始化数据库
setup_database()

# FastAPI 服务器
app = FastAPI()

# 创建聊天会话
class CreateSessionRequest(BaseModel):
    user_id: int

@app.post("/chat/session")
def create_chat_session(request: CreateSessionRequest):
    try:
        conn = pymysql.connect(
            host="localhost",
            user="root",
            password="tyxqc",
            database="server_db",
            port=3306,
            charset="utf8mb4"
        )
        cursor = conn.cursor()
        cursor.execute("""
            INSERT INTO chat_sessions (UserId) 
            VALUES (%s)
        """, (request.user_id,))
        session_id = cursor.lastrowid
        
        welcome_msg = "您好，我是智能助手，请问有什么可以帮助您的？"
        cursor.execute("""
                       INSERT INTO chat_messages (SessionId, Sender, Content)
                       VALUES (%s, %s, %s)
                       """, (session_id, "model", welcome_msg))
        
        conn.commit()
        cursor.close()
        conn.close()
        return JSONResponse(
            content={"SessionId": session_id, "message": "新的聊天会话已创建"},
            media_type="application/json; charset=utf-8"
        )
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Failed to create chat session: {e}")

# 请求和响应模型
class ChatRequest(BaseModel):
    session_id: int
    input: str

class ChatResponse(BaseModel):
    response: str

# 模拟聊天模型
# def chat(input_text: str) -> str:
#     print(f"用户输入: {input_text}")
#     return f"模型回复: 这是一个模拟的回复，您输入的是: {input_text}"
def chat(input_text: str,history) -> str:
    try:
        # 将用户输入添加到历史记录中
        for message in history:
            if message["role"] == "model":
                message["role"] = "assistant"
        
        history.append({"role": "user", "content": input_text})

        # 调用 ZhipuAI 的 chat.completions 接口
        response = client.chat.completions.create(
            model=MODEL,
            messages=history
        )

        # 提取模型的回复内容
        reply = response.choices[0].message.content
        return reply
    except Exception as e:
        print(f"调用 ZhipuAI 失败: {e}")
        return "抱歉，我暂时无法处理您的请求。"
# 聊天接口
@app.post("/chat", response_model=ChatResponse)
async def chat_with_model(request: ChatRequest):
    try:
        # 获取聊天会话的历史记录
        conn = get_db_connection(database="server_db")
        cursor = conn.cursor()
        cursor.execute("""
            SELECT Sender, Content
            FROM chat_messages
            WHERE SessionId = %s
            ORDER BY Timestamp ASC
        """, (request.session_id,))
        results = cursor.fetchall()
        cursor.close()
        conn.close()
        # 格式化历史记录
        history = []
        for row in results:
            history.append({"role": row[0], "content": row[1]})
            
        # 调用 chat 函数生成回复
        response = chat(request.input,history)

        # 存储用户输入和模型回复到数据库
        conn = get_db_connection(database="server_db")
        cursor = conn.cursor()

        # 存储用户输入
        cursor.execute("""
            INSERT INTO chat_messages (SessionId, Sender, Content) 
            VALUES (%s, %s, %s)
        """, (request.session_id, "user", request.input))

        # 存储模型回复
        cursor.execute("""
            INSERT INTO chat_messages (SessionId, Sender, Content) 
            VALUES (%s, %s, %s)
        """, (request.session_id, "model", response))

        conn.commit()
        cursor.close()
        conn.close()

        return JSONResponse(
            content={"response": response},
            media_type="application/json; charset=utf-8"
        )
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Failed to process chat: {e}")

# 根据用户ID获取聊天历史记录
@app.get("/chat/user/{user_id}")
def get_user_chat_history(user_id: int):
    try:
        conn = get_db_connection(database="server_db")
        cursor = conn.cursor()
        cursor.execute("""
            SELECT cs.SessionId, cm.Sender, cm.Content, cm.Timestamp
            FROM chat_sessions cs
            JOIN chat_messages cm ON cs.SessionId = cm.SessionId
            WHERE cs.UserId = %s
            ORDER BY cm.Timestamp ASC
        """, (user_id,))
        results = cursor.fetchall()
        cursor.close()
        conn.close()

        # 格式化返回结果
        history = {}
        for row in results:
            session_id = row[0]
            if session_id not in history:
                history[session_id] = []
            history[session_id].append({
                "sender": row[1],
                "content": row[2],
                "timestamp": row[3].strftime("%Y-%m-%d %H:%M:%S")
            })

        return {"user_id": user_id, "history": history}
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Failed to fetch user chat history: {e}")


# 根据session_id获取聊天历史记录
@app.get("/chat/session/{session_id}")
def get_chat_history(session_id: int):
    try:
        conn = get_db_connection(database="server_db")
        cursor = conn.cursor()
        cursor.execute("""
            SELECT Sender, Content, Timestamp 
            FROM chat_messages 
            WHERE SessionId = %s
            ORDER BY Timestamp ASC
        """, (session_id,))
        results = cursor.fetchall()
        cursor.close()
        conn.close()

        # 格式化返回结果
        history = [
            {"sender": row[0], "content": row[1], "timestamp": row[2].strftime("%Y-%m-%d %H:%M:%S")}
            for row in results
        ]
        return {"session_id": session_id, "history": history}
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Failed to fetch chat history: {e}")

# 启动服务
if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)