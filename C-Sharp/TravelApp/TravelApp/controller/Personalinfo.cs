﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravelApp.models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace TravelApp.controller

{
    public partial class Personalinfo : UserControl
    {
        public Delegate_init init;
        ChangePanel changePanel;
        private readonly string baseUrl = "http://localhost:5199/api/User";
        private readonly long Uid;
        User user = new User();
        public Personalinfo(long uid, ChangePanel changePanel)
        {
            InitializeComponent();
            this.Uid = uid;
            InitInfo();
            this.changePanel = changePanel;
            new_pwd.PasswordChar = '*';
        }

        private async void InitInfo()
        {
            //初始化相关信息
            string url = baseUrl +"/"+ this.Uid;
            //
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(User));
            //
           // User user = new User();
            Client client = new Client();
            try
            {
                HttpResponseMessage result = await client.Get(url);
                if (result.IsSuccessStatusCode)
                {
                    //
                    //User user = (User)xmlSerializer.Deserialize(await result.Content.ReadAsStreamAsync());
                    //
                    string jsonContent = await result.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<User>(jsonContent);

                    new_id.Text = user.UserId.ToString();
                    new_name.Text = user.UserName;
                    //cbGender.Text = user.Sex;
                    //密码显示与不显示
                    long pwd = SimpleEncryptionHelper.DecryptLong(user.password);
                    //if (checkBox1.Checked)
                    //{
                    //    new_pwd.Text = Convert.ToString(pwd);
                        //tBoxPassword.PasswordChar = '\0';   //显示输入
                    //}
                    //else
                    //{
                    //    int length = (int)(Math.Log10(pwd) + 1);
                    //    new_pwd.Text = new string('*', length);

                        //new_pwd.PasswordChar = '*';   //显示*
                    //}
                    new_pwd.Text = Convert.ToString(pwd);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            new_name.Enabled = false;
            new_pwd.Enabled = false;
            new_pwd.PasswordChar = '*';

            string newUsername = new_name.Text;
            string Userpwd = new_pwd.Text;
            long newUserpwd = long.Parse(Userpwd);
            newUserpwd = SimpleEncryptionHelper.EncryptLong(newUserpwd);

            User newuser = new User();
            newuser.UserName = newUsername;
            newuser.password = newUserpwd;
            newuser.UserId = user.UserId;

            Client client = new Client();
            try
            {
                string jsonData = JsonConvert.SerializeObject(newuser);

                // 1. 更新本地数据库
                string localUrl = baseUrl + "/" + user.UserId;
                HttpResponseMessage localResult = await client.Put(localUrl, jsonData);
                
                if (!localResult.IsSuccessStatusCode)
                {
                    string errorMessage = await localResult.Content.ReadAsStringAsync();
                    MessageBox.Show($"本地数据库更新失败: {errorMessage}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 2. 更新Python数据库
                string pythonUrl = "http://localhost:5199/api/python/user/" + user.UserId;
                HttpResponseMessage pythonResult = await client.Put(pythonUrl, jsonData);
                
                if (!pythonResult.IsSuccessStatusCode)
                {
                    string errorMessage = await pythonResult.Content.ReadAsStringAsync();
                    MessageBox.Show($"Python数据库更新失败: {errorMessage}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 更新成功，更新本地user对象
                user.UserName = newUsername;
                user.password = newUserpwd;
                MessageBox.Show("修改成功!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        private void button2_Click(object sender, EventArgs e)
        {
            new_name.Enabled = true;
            new_pwd.Enabled = true;
            new_pwd.PasswordChar = '\0';
            button1.Enabled = true;
            button2.Enabled = false;
        }

        //private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        //{
        //    long pwd = SimpleEncryptionHelper.DecryptLong(user.password);
        //    if (checkBox1.Checked)
        //    {
        //        new_pwd.Text = Convert.ToString(pwd);
        //tBoxPassword.PasswordChar = '\0';   //显示输入
        //    }
        //    else
        //   {
        //        int length = (int)(Math.Log10(pwd) + 1);
        //        new_pwd.Text = new string('*', length);

        //new_pwd.PasswordChar = '*';   //显示*
        //    }
        //}
    }
}
