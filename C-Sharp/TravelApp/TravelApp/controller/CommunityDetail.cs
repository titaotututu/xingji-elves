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
        public long UserId;
        public string baseUrl = "http://localhost:5199/api/python/Journals";
        public ChangePanel ChangePanel;
        public Refresh Refresh;
        public bool IsFromToDo;
        public Travel travel;


        public CommunityDetail(long userId,long journalId, ChangePanel changePanel)
        {
            InitializeComponent();
            this.UserId = userId;
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

                        // 解析图片路径并加载图片
                        if (!string.IsNullOrEmpty(journal.Picture))
                        {
                            // 打印原始图片路径
                            Console.WriteLine($"原始图片路径: {journal.Picture}");

                            // 分割图片路径并去重
                            string[] picturePaths = journal.Picture.Split(',').Distinct().ToArray();

                            // 打印去重后的图片路径
                            Console.WriteLine("去重后的图片路径:");
                            foreach (string path in picturePaths)
                            {
                                Console.WriteLine(path);
                            }

                            foreach (string picturePath in picturePaths)
                            {
                                await LoadImg(this.JournalId);
                            }
                        }
                        else
                        {
                            Console.WriteLine("日志中未包含图片路径");
                            AddDefaultImage();
                        }
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

        private async Task LoadImg(long journalId)
        {
            flpImage.Controls.Clear(); // 清空之前的图片

            string imageApiUrl = $"http://localhost:5199/api/python/journal/image/{journalId}";
            Client client = new Client();

            try
            {
                HttpResponseMessage response = await client.Get(imageApiUrl);
                string jsonContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("imageAPI 返回的 JSON 数据:");
                Console.WriteLine(jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var imageResult = JsonConvert.DeserializeObject<ImageApiResponse>(jsonContent);

                    // Debugging: Print the deserialized object to verify correctness
                    Console.WriteLine("Deserialized ImageApiResponse:");
                    Console.WriteLine(JsonConvert.SerializeObject(imageResult, Formatting.Indented));

                    if (imageResult?.Images != null && imageResult.Images.Count > 0)
                    {
                        foreach (var imageData in imageResult.Images)
                        {
                            if (string.IsNullOrEmpty(imageData?.Data))
                            {
                                Console.WriteLine("图片数据为空，添加默认图片。");
                                AddDefaultImage();
                                continue;
                            }

                            try
                            {
                                Console.WriteLine($"图片文件名: {imageData.Filename}");
                                Console.WriteLine($"Base64 数据长度: {imageData.Data.Length}");
                                Console.WriteLine($"Base64 数据前50字符: {imageData.Data.Substring(0, Math.Min(50, imageData.Data.Length))}");

                                byte[] imageBytes = Convert.FromBase64String(imageData.Data);
                                using (var ms = new MemoryStream(imageBytes))
                                {
                                    Image image = Image.FromStream(ms);

                                    System.Windows.Forms.PictureBox pb = new System.Windows.Forms.PictureBox
                                    {
                                        Image = ResizeImage(image, new Size(280, 160)),
                                        SizeMode = PictureBoxSizeMode.StretchImage,
                                        Margin = new Padding(5),
                                        Anchor = AnchorStyles.None,
                                        Width = 280,
                                        Height = 160
                                    };

                                    flpImage.Controls.Add(pb);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"单张图片处理失败: {ex.Message}");
                                AddDefaultImage();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("返回结果无图片，添加默认图片。");
                        AddDefaultImage();
                    }
                }
                else
                {
                    Console.WriteLine($"请求图片接口失败，状态码: {response.StatusCode}");
                    AddDefaultImage();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"请求或解析图片失败: {ex.Message}");
                AddDefaultImage();
            }

            flpImage.Refresh(); // 最后刷新界面
        }


        private Image ResizeImage(Image imgToResize, Size containerSize)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercentW = (float)containerSize.Width / sourceWidth;
            float nPercentH = (float)containerSize.Height / sourceHeight;
            float nPercent = Math.Min(nPercentW, nPercentH); // Use Min to fit the image within the container without cropping

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(containerSize.Width, containerSize.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
            g.Clear(Color.White); // Optional: Set background color
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // Calculate position to center the image
            int x = (containerSize.Width - destWidth) / 2;
            int y = (containerSize.Height - destHeight) / 2;

            g.DrawImage(imgToResize, x, y, destWidth, destHeight);
            }

            return b;
        }

        private void AddDefaultImage()
        {
            System.Windows.Forms.PictureBox pb = new System.Windows.Forms.PictureBox
            {
                Image = Properties.Resources.default_image,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Margin = new Padding(5),
                Anchor = AnchorStyles.None,
                Width = 280,
                Height = 160
            };

            flpImage.Controls.Add(pb); // 添加到 FlowLayoutPanel
        }

        private void pbBack_Click(object sender, EventArgs e)
        {
            // 创建 CommunityPage 实例并切换回主页面
            CommunityPage communityPage = new CommunityPage(this.JournalId, this.ChangePanel);
            this.ChangePanel(communityPage);
        }
    }


}

