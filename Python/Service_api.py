from fastapi import FastAPI, HTTPException, UploadFile, File, Form
from pydantic import BaseModel
import mysql.connector
from typing import Optional, List
import os
from datetime import datetime
from pathlib import Path
import shutil


# 连接 MySQL 数据库（不指定 database）
def get_db_connection(database=None):
    return mysql.connector.connect(
        host="localhost",
        user="root",
        password="123123",
        database=database if database else "mysql"  # 默认连接到 mysql 数据库
    )


# 确保数据库和表存在
def setup_database():
    # 第一步：创建数据库
    conn = get_db_connection(database=None)
    cursor = conn.cursor()
    cursor.execute("CREATE DATABASE IF NOT EXISTS server_db;")
    cursor.close()
    conn.close()

    # 第二步：连接到新创建的数据库并创建表
    conn = get_db_connection(database="server_db")
    cursor = conn.cursor()
    
    # 创建用户表
    cursor.execute("""
        CREATE TABLE IF NOT EXISTS users (
            UserId BIGINT PRIMARY KEY,
            UserName VARCHAR(255) UNIQUE NOT NULL,
            password BIGINT NOT NULL
        );
    """)
    
    # 创建日志表
    cursor.execute("""
        CREATE TABLE IF NOT EXISTS journals (
            JournalId BIGINT PRIMARY KEY,
            Time DATETIME NOT NULL,
            Title VARCHAR(255),
            Weather VARCHAR(50),
            Emotion VARCHAR(50),
            Description TEXT,
            Picture VARCHAR(255),
            UserId BIGINT NOT NULL,
            FOREIGN KEY (UserId) REFERENCES users(UserId)
        );
    """)
    
    # 创建反馈表
    cursor.execute("""
        CREATE TABLE IF NOT EXISTS feedbacks (
            FeedbackId BIGINT PRIMARY KEY AUTO_INCREMENT,
            UserId BIGINT NOT NULL,
            Content TEXT NOT NULL,
            Time DATETIME NOT NULL,
            Status VARCHAR(20) DEFAULT '未处理',
            FOREIGN KEY (UserId) REFERENCES users(UserId)
        );
    """)
    
    conn.commit()
    cursor.close()
    conn.close()


# 初始化数据库
setup_database()

# FastAPI 服务器
app = FastAPI()


class DataModel(BaseModel):
    key_name: str
    value: str


@app.post("/store_data")
def store_data(data: DataModel):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            INSERT INTO data_exchange (key_name, value) 
            VALUES (%s, %s) 
            ON DUPLICATE KEY UPDATE value = VALUES(value);
        """, (data.key_name, data.value))
        conn.commit()
        cursor.close()
        conn.close()
        return {"message": "Data stored successfully"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


@app.get("/get_data")
def get_data(key_name: str):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("SELECT value FROM data_exchange WHERE key_name = %s", (key_name,))
        result = cursor.fetchone()
        cursor.close()
        conn.close()
        if result:
            return {"key_name": key_name, "value": result[0]}
        else:
            raise HTTPException(status_code=404, detail="Key not found")
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


# 新增用户模型
class UserModel(BaseModel):
    UserId: int
    UserName: str
    password: int

class UserResponse(BaseModel):
    UserId: int
    UserName: str
    password: int

# 修改登录模型
class LoginModel(BaseModel):
    UserId: int
    password: int

# 新增用户相关API端点
@app.post("/user/register", response_model=UserResponse)
def register_user(user: UserModel):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            INSERT INTO users (UserId, UserName, password) 
            VALUES (%s, %s, %s)
        """, (user.UserId, user.UserName, user.password))
        conn.commit()
        cursor.close()
        conn.close()
        return {"UserId": user.UserId, "UserName": user.UserName, "password": user.password}
    except mysql.connector.Error as e:
        if e.errno == 1062:
            raise HTTPException(status_code=400, detail="用户ID或用户名已存在")
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/user/login", response_model=UserResponse)
def login_user(login: LoginModel):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            SELECT UserId, UserName, password FROM users 
            WHERE UserId = %s AND password = %s
        """, (login.UserId, login.password))
        result = cursor.fetchone()
        cursor.close()
        conn.close()
        
        if result:
            return {"UserId": result[0], "UserName": result[1], "password": result[2]}
        raise HTTPException(status_code=401, detail="用户ID或密码错误")
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/user/{user_id}", response_model=UserResponse)
def get_user(user_id: int):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM users WHERE UserId = %s", (user_id,))
        result = cursor.fetchone()
        cursor.close()
        conn.close()
        
        if result:
            return {"UserId": result[0], "UserName": result[1], "password": result[2]}
        raise HTTPException(status_code=404, detail="用户不存在")
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.put("/user/{user_id}")
def update_user(user_id: int, user: UserModel):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            UPDATE users 
            SET UserName = %s, password = %s 
            WHERE UserId = %s
        """, (user.UserName, user.password, user_id))
        conn.commit()
        cursor.close()
        conn.close()
        
        if cursor.rowcount == 0:
            raise HTTPException(status_code=404, detail="用户不存在")
        return {"message": "用户信息更新成功"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.delete("/user/{user_id}")
def delete_user(user_id: int):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("DELETE FROM users WHERE UserId = %s", (user_id,))
        conn.commit()
        cursor.close()
        conn.close()
        
        if cursor.rowcount == 0:
            raise HTTPException(status_code=404, detail="用户不存在")
        return {"message": "用户删除成功"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/users", response_model=List[UserResponse])
def get_all_users():
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM users")
        results = cursor.fetchall()
        cursor.close()
        conn.close()
        
        return [
            {"UserId": row[0], "UserName": row[1], "password": row[2]}
            for row in results
        ]
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))





# 在 UserModel 后添加日志模型
class JournalModel(BaseModel):
    Time: datetime
    Title: Optional[str] = None
    Weather: Optional[str] = None
    Emotion: Optional[str] = None
    Description: Optional[str] = None
    Picture: Optional[str] = None
    UserId: int
    JournalId: Optional[int] = None  # 让API自动生成JournalId

class JournalResponse(BaseModel):
    JournalId: int
    Time: datetime
    Title: Optional[str] = None
    Weather: Optional[str] = None
    Emotion: Optional[str] = None
    Description: Optional[str] = None
    Picture: Optional[str] = None
    UserId: int

# 修改创建日志的API
@app.post("/journal/create", response_model=JournalResponse)
async def create_journal(
    journal: JournalModel,  # 直接从请求体接收JSON数据
):
    try:
        # 生成JournalId
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        
        current_date = datetime.now().strftime("%Y%m%d")
        cursor.execute("""
            SELECT JournalId 
            FROM journals 
            WHERE JournalId >= %s AND JournalId < %s
            ORDER BY JournalId DESC 
            LIMIT 1
        """, (int(current_date + "0000"), int(current_date + "9999")))
        
        result = cursor.fetchone()
        if result:
            journal_id = result[0] + 1
        else:
            journal_id = int(current_date + "0000")

        # 插入日志
        cursor.execute("""
            INSERT INTO journals (JournalId, Time, Title, Weather, Emotion, Description, Picture, UserId)
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s)
        """, (
            journal_id,
            journal.Time,
            journal.Title,
            journal.Weather,
            journal.Emotion,
            journal.Description,
            journal.Picture,
            journal.UserId
        ))
        conn.commit()
        cursor.close()
        conn.close()
        
        return {
            "JournalId": journal_id,
            "Time": journal.Time,
            "Title": journal.Title,
            "Weather": journal.Weather,
            "Emotion": journal.Emotion,
            "Description": journal.Description,
            "Picture": journal.Picture,
            "UserId": journal.UserId
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

# 如果需要上传图片，创建单独的API
@app.post("/journal/upload_image/{journal_id}")
async def upload_journal_image(
    journal_id: int,
    picture_file: UploadFile = File(...)
):
    try:
        uploads_dir = Path("uploads")
        uploads_dir.mkdir(exist_ok=True)
        
        file_extension = Path(picture_file.filename).suffix
        picture_filename = f"{journal_id}_{datetime.now().strftime('%Y%m%d%H%M%S')}{file_extension}"
        picture_path = f"uploads/{picture_filename}"
        
        with open(picture_path, "wb") as buffer:
            shutil.copyfileobj(picture_file.file, buffer)
        
        # 更新日志的图片路径
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            UPDATE journals 
            SET Picture = %s 
            WHERE JournalId = %s
        """, (picture_path, journal_id))
        conn.commit()
        cursor.close()
        conn.close()
        
        return {"message": "图片上传成功", "picture_path": picture_path}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/journal/{journal_id}", response_model=JournalResponse)
def get_journal(journal_id: int):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            SELECT JournalId, Time, Title, Weather, Emotion, Description, Picture, UserId 
            FROM journals WHERE JournalId = %s
        """, (journal_id,))
        result = cursor.fetchone()
        cursor.close()
        conn.close()
        
        if result:
            return {
                "JournalId": result[0],
                "Time": result[1],
                "Title": result[2],
                "Weather": result[3],
                "Emotion": result[4],
                "Description": result[5],
                "Picture": result[6],
                "UserId": result[7]
            }
        raise HTTPException(status_code=404, detail="日志不存在")
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/journals/user/{user_id}", response_model=List[JournalResponse])
def get_user_journals(user_id: int):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            SELECT JournalId, Time, Title, Weather, Emotion, Description, Picture, UserId 
            FROM journals WHERE UserId = %s
        """, (user_id,))
        results = cursor.fetchall()
        cursor.close()
        conn.close()
        
        return [
            {
                "JournalId": row[0],
                "Time": row[1],
                "Title": row[2],
                "Weather": row[3],
                "Emotion": row[4],
                "Description": row[5],
                "Picture": row[6],
                "UserId": row[7]
            }
            for row in results
        ]
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.put("/journal/{journal_id}")
async def update_journal(
    journal_id: int,
    journal: JournalModel
):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            UPDATE journals 
            SET Time = %s, Title = %s, Weather = %s, Emotion = %s, Description = %s, 
                Picture = %s, UserId = %s 
            WHERE JournalId = %s
        """, (
            journal.Time,
            journal.Title,
            journal.Weather,
            journal.Emotion,
            journal.Description,
            journal.Picture,
            journal.UserId,
            journal_id
        ))
        conn.commit()
        
        if cursor.rowcount == 0:
            cursor.close()
            conn.close()
            raise HTTPException(status_code=404, detail="日志不存在")
            
        cursor.close()
        conn.close()
        
        return {
            "message": "日志更新成功",
            "journal": {
                "JournalId": journal_id,
                "Time": journal.Time,
                "Title": journal.Title,
                "Weather": journal.Weather,
                "Emotion": journal.Emotion,
                "Description": journal.Description,
                "Picture": journal.Picture,
                "UserId": journal.UserId
            }
        }
    except HTTPException as e:
        raise e
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.delete("/journal/{journal_id}")
def delete_journal(journal_id: int):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        
        # 先获取图片路径
        cursor.execute("SELECT Picture FROM journals WHERE JournalId = %s", (journal_id,))
        result = cursor.fetchone()
        if result and result[0]:
            # 删除图片文件
            picture_path = Path(result[0])
            if picture_path.exists():
                picture_path.unlink()
        
        # 删除数据库记录
        cursor.execute("DELETE FROM journals WHERE JournalId = %s", (journal_id,))
        conn.commit()
        cursor.close()
        conn.close()
        
        if cursor.rowcount == 0:
            raise HTTPException(status_code=404, detail="日志不存在")
        return {"message": "日志删除成功"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

# 添加获取所有日记的API
@app.get("/journals", response_model=List[JournalResponse])
def get_all_journals():
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            SELECT JournalId, Time, Title, Weather, Emotion, Description, Picture, UserId 
            FROM journals
            ORDER BY Time DESC
        """)
        results = cursor.fetchall()
        cursor.close()
        conn.close()
        
        return [
            {
                "JournalId": row[0],
                "Time": row[1],
                "Title": row[2],
                "Weather": row[3],
                "Emotion": row[4],
                "Description": row[5],
                "Picture": row[6],
                "UserId": row[7]
            }
            for row in results
        ]
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

# 在 UserModel 后添加日志模型
class FeedbackModel(BaseModel):
    UserId: int
    Content: str
    Time: Optional[datetime] = None
    Status: Optional[str] = "未处理"

class FeedbackResponse(BaseModel):
    FeedbackId: int
    UserId: int
    Content: str
    Time: datetime
    Status: str

# 添加反馈相关API端点
@app.post("/feedback/create", response_model=FeedbackResponse)
async def create_feedback(feedback: FeedbackModel):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        
        # 设置当前时间
        current_time = datetime.now()
        
        cursor.execute("""
            INSERT INTO feedbacks (UserId, Content, Time, Status)
            VALUES (%s, %s, %s, %s)
        """, (
            feedback.UserId,
            feedback.Content,
            current_time,
            feedback.Status
        ))
        
        feedback_id = cursor.lastrowid
        conn.commit()
        cursor.close()
        conn.close()
        
        return {
            "FeedbackId": feedback_id,
            "UserId": feedback.UserId,
            "Content": feedback.Content,
            "Time": current_time,
            "Status": feedback.Status
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/feedback/user/{user_id}", response_model=List[FeedbackResponse])
def get_user_feedbacks(user_id: int):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            SELECT FeedbackId, UserId, Content, Time, Status 
            FROM feedbacks WHERE UserId = %s
            ORDER BY Time DESC
        """, (user_id,))
        results = cursor.fetchall()
        cursor.close()
        conn.close()
        
        return [
            {
                "FeedbackId": row[0],
                "UserId": row[1],
                "Content": row[2],
                "Time": row[3],
                "Status": row[4]
            }
            for row in results
        ]
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/feedbacks", response_model=List[FeedbackResponse])
def get_all_feedbacks():
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            SELECT FeedbackId, UserId, Content, Time, Status 
            FROM feedbacks
            ORDER BY Time DESC
        """)
        results = cursor.fetchall()
        cursor.close()
        conn.close()
        
        return [
            {
                "FeedbackId": row[0],
                "UserId": row[1],
                "Content": row[2],
                "Time": row[3],
                "Status": row[4]
            }
            for row in results
        ]
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

# 添加反馈状态更新模型
class FeedbackStatusUpdate(BaseModel):
    status: str

@app.put("/feedback/{feedback_id}")
def update_feedback_status(feedback_id: int, status_update: FeedbackStatusUpdate):
    try:
        conn = get_db_connection("server_db")
        cursor = conn.cursor()
        cursor.execute("""
            UPDATE feedbacks 
            SET Status = %s 
            WHERE FeedbackId = %s
        """, (status_update.status, feedback_id))
        conn.commit()
        cursor.close()
        conn.close()
        
        if cursor.rowcount == 0:
            raise HTTPException(status_code=404, detail="反馈不存在")
        return {"message": "反馈状态更新成功"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="0.0.0.0", port=8000)
