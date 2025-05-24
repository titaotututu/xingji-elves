namespace TravelApp.controller
{
    partial class ChatPage
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonSend = new System.Windows.Forms.Button();
            this.flpHead = new System.Windows.Forms.FlowLayoutPanel();
            this.labelIcon = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.listBoxChat = new System.Windows.Forms.ListBox();
            this.listBoxSessions = new System.Windows.Forms.ListBox();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.buttonNewSession = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.flpHead.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonNewSession)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.buttonSend);
            this.panel1.Controls.Add(this.flpHead);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.buttonNewSession);
            this.panel1.Controls.Add(this.textBoxInput);
            this.panel1.Controls.Add(this.listBoxChat);
            this.panel1.Controls.Add(this.listBoxSessions);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1284, 895);
            this.panel1.TabIndex = 0;
            // 
            // buttonSend
            // 
            this.buttonSend.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.buttonSend.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonSend.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonSend.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonSend.Location = new System.Drawing.Point(1056, 791);
            this.buttonSend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(189, 79);
            this.buttonSend.TabIndex = 18;
            this.buttonSend.Text = "发送";
            this.buttonSend.UseVisualStyleBackColor = false;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // flpHead
            // 
            this.flpHead.BackColor = System.Drawing.Color.White;
            this.flpHead.Controls.Add(this.pictureBoxIcon);
            this.flpHead.Controls.Add(this.labelIcon);
            this.flpHead.Location = new System.Drawing.Point(1, 0);
            this.flpHead.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flpHead.Name = "flpHead";
            this.flpHead.Size = new System.Drawing.Size(1092, 117);
            this.flpHead.TabIndex = 15;
            // 
            // labelIcon
            // 
            this.labelIcon.AutoSize = true;
            this.labelIcon.Font = new System.Drawing.Font("幼圆", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelIcon.ForeColor = System.Drawing.Color.DarkGreen;
            this.labelIcon.Location = new System.Drawing.Point(154, 47);
            this.labelIcon.Margin = new System.Windows.Forms.Padding(13, 47, 4, 0);
            this.labelIcon.Name = "labelIcon";
            this.labelIcon.Size = new System.Drawing.Size(181, 40);
            this.labelIcon.TabIndex = 1;
            this.labelIcon.Text = "智能助手";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("幼圆", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.DarkGreen;
            this.label1.Location = new System.Drawing.Point(291, 733);
            this.label1.Margin = new System.Windows.Forms.Padding(13, 47, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 40);
            this.label1.TabIndex = 2;
            this.label1.Text = "输入：";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.DarkGray;
            this.label3.Location = new System.Drawing.Point(-3, 121);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(1333, 3);
            this.label3.TabIndex = 16;
            this.label3.Text = "label3";
            // 
            // textBoxInput
            // 
            this.textBoxInput.BackColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxInput.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxInput.Location = new System.Drawing.Point(440, 737);
            this.textBoxInput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(805, 36);
            this.textBoxInput.TabIndex = 2;
            // 
            // listBoxChat
            // 
            this.listBoxChat.AllowDrop = true;
            this.listBoxChat.BackColor = System.Drawing.Color.WhiteSmoke;
            this.listBoxChat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxChat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBoxChat.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxChat.FormattingEnabled = true;
            this.listBoxChat.ItemHeight = 30;
            this.listBoxChat.Location = new System.Drawing.Point(295, 147);
            this.listBoxChat.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listBoxChat.Name = "listBoxChat";
            this.listBoxChat.Size = new System.Drawing.Size(951, 565);
            this.listBoxChat.TabIndex = 1;
            // 
            // listBoxSessions
            // 
            this.listBoxSessions.AllowDrop = true;
            this.listBoxSessions.BackColor = System.Drawing.Color.WhiteSmoke;
            this.listBoxSessions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxSessions.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxSessions.FormattingEnabled = true;
            this.listBoxSessions.ItemHeight = 28;
            this.listBoxSessions.Location = new System.Drawing.Point(0, 147);
            this.listBoxSessions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listBoxSessions.Name = "listBoxSessions";
            this.listBoxSessions.Size = new System.Drawing.Size(273, 700);
            this.listBoxSessions.TabIndex = 0;
            this.listBoxSessions.SelectedIndexChanged += new System.EventHandler(this.listBoxSessions_SelectedIndexChanged);
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Image = global::TravelApp.Properties.Resources.智能助手__1_;
            this.pictureBoxIcon.Location = new System.Drawing.Point(4, 4);
            this.pictureBoxIcon.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(133, 92);
            this.pictureBoxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxIcon.TabIndex = 0;
            this.pictureBoxIcon.TabStop = false;
            // 
            // buttonNewSession
            // 
            this.buttonNewSession.BackColor = System.Drawing.Color.Transparent;
            this.buttonNewSession.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonNewSession.Image = global::TravelApp.Properties.Resources.add;
            this.buttonNewSession.Location = new System.Drawing.Point(1151, 27);
            this.buttonNewSession.Margin = new System.Windows.Forms.Padding(40, 27, 4, 4);
            this.buttonNewSession.Name = "buttonNewSession";
            this.buttonNewSession.Size = new System.Drawing.Size(77, 80);
            this.buttonNewSession.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.buttonNewSession.TabIndex = 17;
            this.buttonNewSession.TabStop = false;
            this.buttonNewSession.Click += new System.EventHandler(this.buttonNewSession_Click);
            // 
            // ChatPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ChatPage";
            this.Size = new System.Drawing.Size(1284, 899);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flpHead.ResumeLayout(false);
            this.flpHead.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonNewSession)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBoxChat;
        private System.Windows.Forms.ListBox listBoxSessions;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.FlowLayoutPanel flpHead;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label labelIcon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox buttonNewSession;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Label label1;
    }
}
