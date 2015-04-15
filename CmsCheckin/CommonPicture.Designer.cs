namespace CmsCheckin
{
    partial class CommonPicture
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.TakePic = new System.Windows.Forms.Button();
            this.Return = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(2, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(419, 442);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // TakePic
            // 
            this.TakePic.BackColor = System.Drawing.Color.Khaki;
            this.TakePic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TakePic.Font = new System.Drawing.Font("Verdana", 19.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TakePic.Location = new System.Drawing.Point(2, 448);
            this.TakePic.Name = "TakePic";
            this.TakePic.Size = new System.Drawing.Size(242, 51);
            this.TakePic.TabIndex = 1;
            this.TakePic.Text = "Take Picture";
            this.TakePic.UseVisualStyleBackColor = false;
            this.TakePic.Click += new System.EventHandler(this.TakePic_Click);
            // 
            // Return
            // 
            this.Return.BackColor = System.Drawing.Color.LightGreen;
            this.Return.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Return.Font = new System.Drawing.Font("Verdana", 19.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Return.Location = new System.Drawing.Point(250, 448);
            this.Return.Name = "Return";
            this.Return.Size = new System.Drawing.Size(159, 51);
            this.Return.TabIndex = 2;
            this.Return.Text = "Return";
            this.Return.UseVisualStyleBackColor = false;
            this.Return.Click += new System.EventHandler(this.Return_Click);
            // 
            // Picture
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(423, 511);
            this.ControlBox = false;
            this.Controls.Add(this.Return);
            this.Controls.Add(this.TakePic);
            this.Controls.Add(this.pictureBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Picture";
            this.Text = "Take Picture";
            this.Load += new System.EventHandler(this.Picture_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button TakePic;
        private System.Windows.Forms.Button Return;


    }
}