using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace TravelApp.controller
{
    public partial class ChatPage : UserControl
    {
        private long UserId;
        private List<long> sessionIdList = new List<long>();
        private long currentSessionId = -1;
        public ChangePanel ChangePanel;

        public ChatPage(long userId, ChangePanel changePanel)
        {
            InitializeComponent();

            listBoxChat.DrawMode = DrawMode.OwnerDrawVariable;
            listBoxChat.MeasureItem += listBoxChat_MeasureItem;
            listBoxChat.DrawItem += listBoxChat_DrawItem;

            this.UserId = userId;
            this.ChangePanel = changePanel;
            // 确保页面加载事件绑定
            this.Load += ChatPage_Load;
        }

        // 自动测量每一项高度
        private void listBoxChat_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0) return;
            string text = listBoxChat.Items[e.Index].ToString();
            SizeF size = e.Graphics.MeasureString(text, listBoxChat.Font, listBoxChat.Width);
            e.ItemHeight = (int)size.Height + 4; // 适当加点padding
        }

        // 自动绘制每一项内容并换行
        private void listBoxChat_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();
            string text = listBoxChat.Items[e.Index].ToString();
            using (Brush brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(text, listBoxChat.Font, brush, e.Bounds);
            }
            e.DrawFocusRectangle();
        }


        // 页面加载时，加载所有历史会话
        private async void ChatPage_Load(object sender, EventArgs e)
        {
            await LoadAllSessions();
        }

        // 加载所有历史会话
        private async Task LoadAllSessions(long? selectSessionId = null)
        {
            try
            {
                var url = $"http://localhost:8001/chat/user/{UserId}";
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("用户历史会话接口返回：" + result);

                    var obj = JObject.Parse(result);

                    listBoxSessions.Items.Clear();
                    sessionIdList.Clear();

                    var history = obj["history"] as JObject;
                    if (history != null)
                    {
                        foreach (var prop in history.Properties())
                        {
                            string sessionIdStr = prop.Name;
                            long sessionId = long.Parse(sessionIdStr);
                            var messages = prop.Value as JArray;
                            string preview = "";
                            if (messages != null && messages.Count > 0)
                            {
                                var lastMsg = messages[messages.Count - 1];
                                preview = lastMsg["content"]?.ToString();
                            }
                            listBoxSessions.Items.Add($"会话 {sessionId}：{preview}");
                            sessionIdList.Add(sessionId);
                        }
                    }

                    // 自动选中指定会话
                    if (selectSessionId != null)
                    {
                        int idx = sessionIdList.IndexOf(selectSessionId.Value);
                        if (idx >= 0)
                        {
                            listBoxSessions.SelectedIndex = idx;
                        }
                    }
                    else if (listBoxSessions.Items.Count > 0 && listBoxSessions.SelectedIndex == -1)
                    {
                        listBoxSessions.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载历史会话失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonNewSession_Click(object sender, EventArgs e)
        {
            try
            {
                var url = "http://localhost:8001/chat/session";
                var data = new { user_id = UserId };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, content);
                    var result = await response.Content.ReadAsStringAsync();
                    dynamic obj = JsonConvert.DeserializeObject(result);
                    long newSessionId = obj.SessionId;
                    await LoadAllSessions(newSessionId); // 自动选中新建会话
                    currentSessionId = newSessionId;
                    listBoxChat.Items.Clear(); // 聊天区清空
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("新建会话失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            string input = textBoxInput.Text.Trim();
            if (string.IsNullOrEmpty(input) || currentSessionId == -1)
            {
                MessageBox.Show("请输入内容或请先选择/新建会话！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var url = "http://localhost:8001/chat";
                var data = new { session_id = currentSessionId, input = input };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, content);
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("接口返回内容：" + result);

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("新建会话接口请求失败，状态码：" + response.StatusCode + "\n内容：" + result, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    await LoadSessionHistory(currentSessionId); // 只刷新聊天区
                }
                textBoxInput.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送消息失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 选中会话时加载聊天内容
        private async void listBoxSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxSessions.SelectedIndex >= 0 && listBoxSessions.SelectedIndex < sessionIdList.Count)
            {
                currentSessionId = sessionIdList[listBoxSessions.SelectedIndex];
                await LoadSessionHistory(currentSessionId);
            }
            else
            {
                currentSessionId = -1;
                listBoxChat.Items.Clear();
            }
        }

        // 加载指定会话的聊天历史
        private async Task LoadSessionHistory(long sessionId)
        {
            try
            {
                var url = $"http://localhost:8001/chat/session/{sessionId}";
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    var result = await response.Content.ReadAsStringAsync();
                    dynamic obj = JsonConvert.DeserializeObject(result);
                    listBoxChat.Items.Clear();
                    foreach (var msg in obj.history)
                    {
                        string sender = msg.sender == "user" ? "我" : "AI";
                        string content = msg.content.ToString();
                        string time = msg.timestamp.ToString();
                        listBoxChat.Items.Add($"{sender} [{time}]: {content}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载聊天历史失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}