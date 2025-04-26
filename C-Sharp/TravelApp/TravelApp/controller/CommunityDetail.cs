using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravelApp.models;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using TravelApp.models;

namespace TravelApp.controller
{
    public partial class CommunityDetail : UserControl
    {
        public long JournalId;
        public string baseUrl = "http://localhost:5199/api/python/Journals";
        public ChangePanel ChangePanel;
        public Refresh Refresh;
        public bool IsFromToDo;
        public Travel travel;


        public CommunityDetail(long journalId, ChangePanel changePanel)
        {
            InitializeComponent();
            this.JournalId = journalId;
            this.ChangePanel = changePanel;

            LoadJournalDetails(); // 加载日志详情
        }

        private async void LoadJournalDetails()
        {
            string url = "http://localhost:5199/api/python/journals"; // API URL
            Client client = new Client();

            try
            {
                HttpResponseMessage response = await client.Get(url);
                if (response.IsSuccessStatusCode)
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();

                    // 打印返回的 JSON 数据到终端
                    Console.WriteLine("API 返回的 JSON 数据:");
                    Console.WriteLine(jsonContent);

                    List<Journal> journals = JsonConvert.DeserializeObject<List<Journal>>(jsonContent);

                    // 根据 JournalId 查找对应的日志
                    Journal journal = journals.FirstOrDefault(j => j.JournalId == this.JournalId);
                    if (journal != null)
                    {
                        // 设置页面控件的值
                        tbTitle.Text = journal.Title;
                        lblTime.Text = journal.Time.ToString("yyyy-MM-dd HH:mm:ss");
                        tbWeather.Text = journal.Weather;
                        tbEmotion.Text = journal.Emotion;
                        rtbDescription.Text = journal.Description;

                        // 拼接图片的完整路径
                        string imagePath = $@"C:\Users\32188\Desktop\SE\final2.0\Python\{journal.Picture}";

                        // 检查图片是否存在
                        if (!string.IsNullOrEmpty(journal.Picture) && System.IO.File.Exists(imagePath))
                        {
                            // 从本地加载图片
                            flpImage.BackgroundImage = Image.FromFile(imagePath);
                            Console.WriteLine($"成功加载图片: {imagePath}"); // 调试语句
                        }
                        else
                        {
                            // 使用默认图片
                            flpImage.BackgroundImage = Properties.Resources.default_image;
                            Console.WriteLine($"图片不存在，使用默认图片: {journal.Picture}"); // 调试语句
                        }

                        // 设置图片自适应
                        flpImage.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else
                    {
                        Console.WriteLine($"未找到 JournalId 为 {this.JournalId} 的日志");
                        MessageBox.Show("未找到对应的日志详情", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();

                    // 打印错误状态码和内容
                    Console.WriteLine($"HTTP 请求失败: {response.StatusCode}");
                    Console.WriteLine($"错误内容: {errorContent}");

                    MessageBox.Show("无法加载日志详情", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载日志详情失败: {ex.Message}");
                MessageBox.Show($"加载日志详情失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pbBack_Click(object sender, EventArgs e)
        {
            // 创建 CommunityPage 实例并切换回主页面
            CommunityPage communityPage = new CommunityPage(this.JournalId, this.ChangePanel);
            this.ChangePanel(communityPage);
        }
    }
}
