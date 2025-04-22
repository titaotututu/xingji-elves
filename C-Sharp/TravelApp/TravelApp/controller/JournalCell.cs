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
using Newtonsoft.Json;
using TravelApp.models;

namespace TravelApp.controller
{
    public partial class JournalCell : UserControl
    {
        public long JournalId;
        public Refresh Refresh;
        public ChangePanel ChangePanel;
        public JournalCell(long journalId, ChangePanel changePanel, Refresh refresh)
        {
            InitializeComponent();
            this.JournalId = journalId;
            this.ChangePanel = changePanel;
            this.Refresh = refresh;
        }

        private async Task<Journal> GetJournalById(long journalId)
        {
            string url = "http://localhost:5199/api/Journal/get?journalId=" + journalId;
            Client client = new Client();
            Journal journal = null;
            try
            {
                HttpResponseMessage result = await client.Get(url);
                if (result.IsSuccessStatusCode)
                {
                    string jsonContent = await result.Content.ReadAsStringAsync();
                    journal = JsonConvert.DeserializeObject<Journal>(jsonContent);
                }
                // Optionally handle failure to get local journal
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting journal {journalId} in JournalCell: {ex.Message}");
            }
            return journal;
        }

        private async void pbDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("是否确认删除此日志？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.OK)
            {
                long localJournalId = this.JournalId;
                Client client = new Client();
                bool localDeleteAttempted = false;
                bool refreshNeeded = false;

                // 1. 获取UserId，为删除Python端做准备
                Journal journalInfo = await GetJournalById(localJournalId);
                if (journalInfo != null)
                {
                    string pythonJournalId = journalInfo.UserId + "" + localJournalId;
                    string pythonCheckUrl = "http://localhost:5199/api/python/journal/" + pythonJournalId;
                    try
                    {
                        // 检查Python端是否存在
                        HttpResponseMessage checkResult = await client.Get(pythonCheckUrl);
                        if (checkResult.IsSuccessStatusCode)
                        {
                            // 存在，则尝试删除Python端
                            string pythonDeleteUrl = "http://localhost:5199/api/python/journal/" + pythonJournalId;
                            HttpResponseMessage pythonDeleteResult = await client.Delete(pythonDeleteUrl);
                            if (!pythonDeleteResult.IsSuccessStatusCode)
                            {
                                // Python端删除失败，可以选择提示用户或记录日志，但仍然继续删除本地
                                Console.WriteLine($"Python journal {pythonJournalId} deletion failed: {pythonDeleteResult.StatusCode}");
                                // MessageBox.Show("服务器端日志删除失败，但将继续删除本地日志。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                refreshNeeded = true; // Python端删除成功，需要刷新
                            }
                        }
                        // 如果检查不存在，则无需删除Python端，继续删除本地
                    }
                    catch (Exception ex)
                    {
                        // 检查或删除Python端时出错，仍然继续删除本地
                         Console.WriteLine($"Error checking/deleting python journal {pythonJournalId}: {ex.Message}");
                         // MessageBox.Show("检查或删除服务器端日志时出错，将继续删除本地日志。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                     // 无法获取UserId，无法尝试删除Python端，直接删除本地
                     Console.WriteLine($"Could not get UserId for journal {localJournalId} to attempt Python deletion.");
                }

                // 2. 删除本地日志
                string localDeleteUrl = "http://localhost:5199/api/Journal/delete?journalid=" + localJournalId;
                try
                {
                    HttpResponseMessage localResult = await client.Delete(localDeleteUrl);
                    localDeleteAttempted = true;
                    if (localResult.IsSuccessStatusCode)
                    {
                        refreshNeeded = true; // 本地删除成功，需要刷新
                    }
                    else
                    {
                        MessageBox.Show("本地日志删除未成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n本地日志删除失败", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // 3. 如果需要刷新 (本地或Python端至少有一个删除成功或尝试删除)
                if (refreshNeeded || localDeleteAttempted) // 只要尝试了本地删除或Python删除成功就刷新
                {
                    this.Refresh();
                }
            }
        }
        private void JournalCell_Click(object sender, EventArgs e)
        {
            long id = this.JournalId;
            JournalDetail journalDetail = new JournalDetail(id, ChangePanel);
            this.ChangePanel(journalDetail);
        }

        private void pictureBoxGoose_Click(object sender, EventArgs e)
        {
            JournalCell_Click(sender, e);
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {
            JournalCell_Click(sender, e);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            JournalCell_Click(sender, e);
        }

        private void lblWeather_Click(object sender, EventArgs e)
        {
            JournalCell_Click(sender, e);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            JournalCell_Click(sender, e);
        }

        private void lblTime_Click(object sender, EventArgs e)
        {
            JournalCell_Click(sender, e);
        }
    }
}
