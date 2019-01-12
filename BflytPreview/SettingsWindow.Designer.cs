namespace BflytPreview
{
    partial class SettingsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.pickColor = new System.Windows.Forms.Button();
            this.selectedColor = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.outlineColor = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.loadBackgroundImage = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.showImg = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pane Color";
            // 
            // pickColor
            // 
            this.pickColor.Location = new System.Drawing.Point(96, 8);
            this.pickColor.Name = "pickColor";
            this.pickColor.Size = new System.Drawing.Size(75, 23);
            this.pickColor.TabIndex = 1;
            this.pickColor.UseVisualStyleBackColor = true;
            this.pickColor.Click += new System.EventHandler(this.pickColor_Click);
            // 
            // selectedColor
            // 
            this.selectedColor.Location = new System.Drawing.Point(96, 37);
            this.selectedColor.Name = "selectedColor";
            this.selectedColor.Size = new System.Drawing.Size(75, 23);
            this.selectedColor.TabIndex = 2;
            this.selectedColor.UseVisualStyleBackColor = true;
            this.selectedColor.Click += new System.EventHandler(this.selectedColor_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Selected Color";
            // 
            // outlineColor
            // 
            this.outlineColor.Location = new System.Drawing.Point(96, 67);
            this.outlineColor.Name = "outlineColor";
            this.outlineColor.Size = new System.Drawing.Size(75, 23);
            this.outlineColor.TabIndex = 4;
            this.outlineColor.UseVisualStyleBackColor = true;
            this.outlineColor.Click += new System.EventHandler(this.outlineColor_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Outline Color";
            // 
            // loadBackgroundImage
            // 
            this.loadBackgroundImage.Location = new System.Drawing.Point(13, 102);
            this.loadBackgroundImage.Name = "loadBackgroundImage";
            this.loadBackgroundImage.Size = new System.Drawing.Size(158, 23);
            this.loadBackgroundImage.TabIndex = 6;
            this.loadBackgroundImage.Text = "Set Background Image";
            this.loadBackgroundImage.UseVisualStyleBackColor = true;
            this.loadBackgroundImage.Click += new System.EventHandler(this.loadBackgroundImage_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(177, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(206, 117);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // showImg
            // 
            this.showImg.Location = new System.Drawing.Point(13, 132);
            this.showImg.Name = "showImg";
            this.showImg.Size = new System.Drawing.Size(158, 23);
            this.showImg.TabIndex = 8;
            this.showImg.Text = "Show Image";
            this.showImg.UseVisualStyleBackColor = true;
            this.showImg.Click += new System.EventHandler(this.showImg_Click);
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 173);
            this.Controls.Add(this.showImg);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.loadBackgroundImage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.outlineColor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.selectedColor);
            this.Controls.Add(this.pickColor);
            this.Controls.Add(this.label1);
            this.Name = "SettingsWindow";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button pickColor;
        private System.Windows.Forms.Button selectedColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button outlineColor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button loadBackgroundImage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button showImg;
    }
}