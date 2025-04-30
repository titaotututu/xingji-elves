namespace TravelApp.controller
{
    partial class Feedback
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
            this.flpHead = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.labelIcon = new System.Windows.Forms.Label();
            this.submit = new System.Windows.Forms.Button();
            this.notice = new System.Windows.Forms.Label();
            this.rtbDescription = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.flpHead.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.Controls.Add(this.flpHead);
            this.panel1.Controls.Add(this.submit);
            this.panel1.Controls.Add(this.notice);
            this.panel1.Controls.Add(this.rtbDescription);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(-9, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(969, 666);
            this.panel1.TabIndex = 1;
            // 
            // flpHead
            // 
            this.flpHead.BackColor = System.Drawing.Color.White;
            this.flpHead.Controls.Add(this.pictureBoxIcon);
            this.flpHead.Controls.Add(this.labelIcon);
            this.flpHead.Location = new System.Drawing.Point(9, 0);
            this.flpHead.Name = "flpHead";
            this.flpHead.Size = new System.Drawing.Size(960, 88);
            this.flpHead.TabIndex = 26;
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Image = global::TravelApp.Properties.Resources.反馈__1_;
            this.pictureBoxIcon.Location = new System.Drawing.Point(30, 20);
            this.pictureBoxIcon.Margin = new System.Windows.Forms.Padding(30, 20, 3, 3);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(58, 57);
            this.pictureBoxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxIcon.TabIndex = 0;
            this.pictureBoxIcon.TabStop = false;
            // 
            // labelIcon
            // 
            this.labelIcon.AutoSize = true;
            this.labelIcon.Font = new System.Drawing.Font("幼圆", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelIcon.ForeColor = System.Drawing.Color.DarkGreen;
            this.labelIcon.Location = new System.Drawing.Point(101, 35);
            this.labelIcon.Margin = new System.Windows.Forms.Padding(10, 35, 3, 0);
            this.labelIcon.Name = "labelIcon";
            this.labelIcon.Size = new System.Drawing.Size(81, 33);
            this.labelIcon.TabIndex = 1;
            this.labelIcon.Text = "反馈";
            // 
            // submit
            // 
            this.submit.Font = new System.Drawing.Font("幼圆", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.submit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.submit.Location = new System.Drawing.Point(749, 562);
            this.submit.Name = "submit";
            this.submit.Size = new System.Drawing.Size(167, 57);
            this.submit.TabIndex = 25;
            this.submit.Text = "提交反馈";
            this.submit.UseVisualStyleBackColor = true;
            // 
            // notice
            // 
            this.notice.AutoSize = true;
            this.notice.Font = new System.Drawing.Font("幼圆", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.notice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.notice.Location = new System.Drawing.Point(76, 117);
            this.notice.Name = "notice";
            this.notice.Size = new System.Drawing.Size(607, 36);
            this.notice.TabIndex = 24;
            this.notice.Text = "请在此处键入您对行迹精灵的建议：";
            // 
            // rtbDescription
            // 
            this.rtbDescription.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtbDescription.Location = new System.Drawing.Point(66, 200);
            this.rtbDescription.Name = "rtbDescription";
            this.rtbDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtbDescription.Size = new System.Drawing.Size(850, 341);
            this.rtbDescription.TabIndex = 23;
            this.rtbDescription.Text = "hhhhh";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("幼圆", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label3.Location = new System.Drawing.Point(70, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 36);
            this.label3.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label2.Location = new System.Drawing.Point(6, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1072, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "————————————————————————————————————————————————————————";
            // 
            // Feedback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "Feedback";
            this.Size = new System.Drawing.Size(961, 666);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flpHead.ResumeLayout(false);
            this.flpHead.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox rtbDescription;
        private System.Windows.Forms.Label notice;
        private System.Windows.Forms.Button submit;
        private System.Windows.Forms.FlowLayoutPanel flpHead;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label labelIcon;
    }
}
