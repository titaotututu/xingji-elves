using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravelApp.models;

namespace TravelApp.controller
{
    public partial class CommunityPage : UserControl
    {
        public long UserId;
        public ChangePanel ChangePanel;

        public CommunityPage(long userId, ChangePanel changePanel)
        {
            InitializeComponent();
            this.UserId = userId;
            this.ChangePanel = changePanel;
            Init(); // 初始化页面
        }

        // 初始化页面，加载日志数据
        public async void Init()
        {
            string url = "http://localhost:5199/api/python/journals"; // API endpoint to fetch all journals

            Client client = new Client();
            try
            {
                Console.WriteLine("开始请求日志数据..."); // 调试语句
                HttpResponseMessage result = await client.Get(url);
                Console.WriteLine($"HTTP 请求状态码: {result.StatusCode}"); // 打印 HTTP 状态码

                if (result.IsSuccessStatusCode)
                {
                    string jsonContent = await result.Content.ReadAsStringAsync();
                    Console.WriteLine($"返回的 JSON 数据: {jsonContent}"); // 打印返回的 JSON 数据

                    List<Journal> journals = JsonConvert.DeserializeObject<List<Journal>>(jsonContent);
                    Console.WriteLine($"解析出的日志数量: {journals.Count}"); // 打印日志数量

                    flowLayoutPanel1.Controls.Clear(); // 清空之前的内容

                    foreach (Journal journal in journals)
                    {
                        try
                        {
                            // 创建 CommunityCell 并设置内容
                            CommunityCell cell = new CommunityCell();
                            cell.userName.Text = $"用户ID: {journal.UserId}";
                            cell.time.Text = journal.Time.ToString();

                            // 设置默认图片
                            Console.WriteLine($"设置默认图片: {journal.Picture}"); // 调试语句
                            cell.image.Image = Properties.Resources.default_image; // 使用默认图片

                            flowLayoutPanel1.Controls.Add(cell); // 添加到 FlowLayoutPanel
                            Console.WriteLine($"成功添加日志: {journal.Title}"); // 打印日志标题
                        }
                        catch (Exception cellEx)
                        {
                            Console.WriteLine($"创建或添加 CommunityCell 时出错: {cellEx.Message}"); // 打印错误信息
                        }
                    }

                    // 添加底部标志
                    Label lblBottom = new Label
                    {
                        Text = "无更多内容",
                        Anchor = AnchorStyles.None,
                        AutoSize = true
                    };
                    flowLayoutPanel1.Controls.Add(lblBottom);
                    Console.WriteLine("日志加载完成"); // 调试语句
                }
                else
                {
                    string errorContent = await result.Content.ReadAsStringAsync();
                    Console.WriteLine($"HTTP 请求失败: {result.StatusCode}, 错误信息: {errorContent}"); // 打印错误信息
                    MessageBox.Show("无法加载日志数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载日志失败: {ex.Message}"); // 打印异常信息
                MessageBox.Show($"加载日志失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

            // 刷新页面
            public void RefreshPage()
            {
                this.Init();
            }

            private void CommunityPage_Load(object sender, EventArgs e)
            {
                this.Init(); // 初始化页面
            }
    }
}
