﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using TravelApp.models;
using System.Security.Cryptography;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace TravelApp
{
    public partial class Register : Form
    {
        private string baseUrl = "http://localhost:5199/api/User";
        //private static int idnum = 0;
        public Register()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (reg_name.Text.Length == 0 || reg_name.Text == "单行输入")
            {
                MessageBox.Show("请输入用户姓名");
            }
            else if (reg_pwd.Text.Length == 0 || reg_pwd.Text == "单行输入")
            {
                MessageBox.Show("请输入密码");
            }
            else
            {
                User user = new User();
                Client client = new Client();

                try
                {
                    // 设置用户信息
                    user.UserName = reg_name.Text;
                    user.password = SimpleEncryptionHelper.EncryptLong(long.Parse(reg_pwd.Text));
                    long ticks = DateTime.Now.Ticks;
                    long randomNum = ticks % 100000;
                    user.UserId = randomNum;

                    // 1. 先调用本地数据库API
                    string localUrl = baseUrl;
                    string data = JsonConvert.SerializeObject(user);
                    HttpResponseMessage localResult = await client.Post(localUrl, data);
                    
                    if (!localResult.IsSuccessStatusCode)
                    {
                        string errorMessage = await localResult.Content.ReadAsStringAsync();
                        MessageBox.Show($"本地数据库注册失败: {errorMessage}");
                        return;
                    }

                    // 2. 调用Python数据库API
                    string pythonUrl = "http://localhost:5199/api/python/user";
                    HttpResponseMessage pythonResult = await client.Post(pythonUrl, data);
                    
                    if (!pythonResult.IsSuccessStatusCode)
                    {
                        string errorMessage = await pythonResult.Content.ReadAsStringAsync();
                        MessageBox.Show($"Python数据库注册失败: {errorMessage}");
                        return;
                    }

                    MessageBox.Show("注册成功!您的ID是" + user.UserId);
                    using (MainFormFinal mff = new MainFormFinal(user.UserId))
                    {
                        mff.changePanel = mff.AddControlsToPanel;
                        this.Hide();
                        mff.ShowDialog();
                        this.Dispose();
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }
        }

        public static class SimpleEncryptionHelper
        {
            private static readonly long key = 0x2A3B4C5D6E7F8090; // Replace with your key

            public static long EncryptLong(long value)
            {
                return value ^ key;
            }

            public static long DecryptLong(long encryptedValue)
            {
                return encryptedValue ^ key;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                reg_pwd.PasswordChar = '\0';   //显示输入
            }
            else
            {
                reg_pwd.PasswordChar = '*';   //显示*
            }
        }
    }
}
