namespace TravelApp.controller
{
    partial class WeatherController
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
            this.label1 = new System.Windows.Forms.Label();
            this.cityTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.fetchWeatherButton = new System.Windows.Forms.Button();
            this.resultTextBox = new System.Windows.Forms.RichTextBox();
            this.forecastTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("方正粗黑宋简体", 22.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(24, 32);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(270, 72);
            this.label1.TabIndex = 0;
            this.label1.Text = "天气查询";
            // 
            // cityTextBox
            // 
            this.cityTextBox.Location = new System.Drawing.Point(309, 142);
            this.cityTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cityTextBox.Name = "cityTextBox";
            this.cityTextBox.Size = new System.Drawing.Size(206, 44);
            this.cityTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 145);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(271, 33);
            this.label2.TabIndex = 2;
            this.label2.Text = "请输入查询地点：";
            // 
            // fetchWeatherButton
            // 
            this.fetchWeatherButton.Location = new System.Drawing.Point(685, 134);
            this.fetchWeatherButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fetchWeatherButton.Name = "fetchWeatherButton";
            this.fetchWeatherButton.Size = new System.Drawing.Size(230, 54);
            this.fetchWeatherButton.TabIndex = 3;
            this.fetchWeatherButton.Text = "点击查询";
            this.fetchWeatherButton.UseVisualStyleBackColor = true;
            this.fetchWeatherButton.Click += new System.EventHandler(this.fetchWeatherButton_Click);
            // 
            // resultTextBox
            // 
            this.resultTextBox.Location = new System.Drawing.Point(36, 228);
            this.resultTextBox.Name = "resultTextBox";
            this.resultTextBox.Size = new System.Drawing.Size(479, 467);
            this.resultTextBox.TabIndex = 4;
            this.resultTextBox.Text = "";
            // 
            // forecastTextBox
            // 
            this.forecastTextBox.Location = new System.Drawing.Point(566, 228);
            this.forecastTextBox.Name = "forecastTextBox";
            this.forecastTextBox.Size = new System.Drawing.Size(479, 467);
            this.forecastTextBox.TabIndex = 5;
            this.forecastTextBox.Text = "";
            // 
            // WeatherController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 33F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.forecastTextBox);
            this.Controls.Add(this.resultTextBox);
            this.Controls.Add(this.fetchWeatherButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cityTextBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "WeatherController";
            this.Size = new System.Drawing.Size(1175, 727);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox cityTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button fetchWeatherButton;
        private System.Windows.Forms.RichTextBox resultTextBox;
        private System.Windows.Forms.RichTextBox forecastTextBox;
    }
}
