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
        password="lwy2004926",
        database=database if database else "mysql"  # 默认连接到 mysql 数据库
    )

#测试连接

def main():
    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute("SELECT DATABASE();")
        result = cursor.fetchone()
        print("Connected to database:", result[0])
    except mysql.connector.Error as err:
        print("Error:", err)
    finally:
        if conn.is_connected():
            cursor.close()
            conn.close()

if __name__ == "__main__":
    main()