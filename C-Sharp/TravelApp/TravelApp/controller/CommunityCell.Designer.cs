namespace TravelApp.controller
{
    partial class CommunityCell
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public System.Windows.Forms.PictureBox image;
        public System.Windows.Forms.Label title;
        public System.Windows.Forms.Label userName;
        public System.Windows.Forms.Label time;


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
            this.userName = new System.Windows.Forms.Label();
            this.time = new System.Windows.Forms.Label();
            this.title = new System.Windows.Forms.Label();
            this.image = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.image)).BeginInit();
            this.SuspendLayout();
            // 
            // userName
            // 
            this.userName.AutoSize = true;
            this.userName.Font = new System.Drawing.Font("幼圆", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.userName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.userName.Location = new System.Drawing.Point(55, 57);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(102, 28);
            this.userName.TabIndex = 1;
            this.userName.Text = "label1";
            this.userName.Click += new System.EventHandler(this.userName_Click);
            // 
            // time
            // 
            this.time.AutoSize = true;
            this.time.Font = new System.Drawing.Font("幼圆", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.time.Location = new System.Drawing.Point(55, 85);
            this.time.Name = "time";
            this.time.Size = new System.Drawing.Size(102, 28);
            this.time.TabIndex = 2;
            this.time.Text = "label1";
            this.time.Click += new System.EventHandler(this.time_Click);
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("幼圆", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.title.Location = new System.Drawing.Point(55, 18);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(109, 30);
            this.title.TabIndex = 3;
            this.title.Text = "label1";
            this.title.Click += new System.EventHandler(this.title_Click);
            // 
            // image
            // 
            this.image.Location = new System.Drawing.Point(614, 3);
            this.image.Name = "image";
            this.image.Size = new System.Drawing.Size(164, 117);
            this.image.TabIndex = 0;
            this.image.TabStop = false;
            this.image.Click += new System.EventHandler(this.image_Click);
            // 
            // CommunityCell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Controls.Add(this.title);
            this.Controls.Add(this.time);
            this.Controls.Add(this.userName);
            this.Controls.Add(this.image);
            this.Name = "CommunityCell";
            this.Size = new System.Drawing.Size(818, 136);
            ((System.ComponentModel.ISupportInitialize)(this.image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //private System.Windows.Forms.Label title;

        #endregion

        //private System.Windows.Forms.PictureBox image;
        //private System.Windows.Forms.Label userName;
        //private System.Windows.Forms.Label time;
    }
}
