using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Http;
using Newtonsoft.Json;

namespace TravelApp.controller
{
    public partial class Feedback : UserControl
    {
        public long UserId;
        public ChangePanel ChangePanel;

        public Feedback(long userId, ChangePanel changePanel)
        {
            InitializeComponent();
            this.UserId = userId;
            this.ChangePanel = changePanel;
            //Init(); // 初始化页面
            // 绑定提交按钮的点击事件
            submit.Click += Submit_Click;
        }
        //public Feedback()
        //{
        //    InitializeComponent();
        //}

        private async void Submit_Click(object sender, EventArgs e)
        {
            // 获取用户输入的反馈内容
            string feedbackContent = rtbDescription.Text.Trim();

            // 检查输入是否为空
            if (string.IsNullOrEmpty(feedbackContent))
            {
                MessageBox.Show("反馈内容不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 构造反馈数据
            var feedbackData = new
            {
                feedbackId = 0, // 让后端自动生成 ID
                userId = this.UserId,
                content = feedbackContent,
                time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                status = "未处理" // 默认状态
            };

            // 将数据序列化为 JSON
            string jsonData = JsonConvert.SerializeObject(feedbackData);

            // 发送 POST 请求到后端 API
            string url = "http://localhost:5199/api/python/feedback";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("反馈提交成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        rtbDescription.Clear(); // 清空文本框
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"反馈提交失败：{response.StatusCode}\n{errorMessage}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"提交反馈时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
