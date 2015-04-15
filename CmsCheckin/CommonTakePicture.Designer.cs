namespace CmsCheckin
{
    partial class CommonTakePicture
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
            this.Return = new System.Windows.Forms.Button();
            this.imageResizer1 = new CmsCheckin.CommonImageResizer();
            this.SuspendLayout();
            // 
            // Return
            // 
            this.Return.BackColor = System.Drawing.Color.LightGreen;
            this.Return.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Return.Font = new System.Drawing.Font("Verdana", 16.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Return.Location = new System.Drawing.Point(665, 406);
            this.Return.Name = "Return";
            this.Return.Size = new System.Drawing.Size(120, 69);
            this.Return.TabIndex = 1;
            this.Return.Text = "Return";
            this.Return.UseVisualStyleBackColor = false;
            this.Return.Click += new System.EventHandler(this.Return_Click);
            // 
            // imageResizer1
            // 
            this.imageResizer1.Location = new System.Drawing.Point(12, 11);
            this.imageResizer1.Name = "imageResizer1";
            this.imageResizer1.OffsetX = 0;
            this.imageResizer1.OffsetY = 0;
            this.imageResizer1.RequiredHeight = 0;
            this.imageResizer1.RequiredWidth = 0;
            this.imageResizer1.Size = new System.Drawing.Size(773, 485);
            this.imageResizer1.TabIndex = 2;
            // 
            // TakePicture
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(798, 512);
            this.ControlBox = false;
            this.Controls.Add(this.Return);
            this.Controls.Add(this.imageResizer1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TakePicture";
            this.Text = "Take Picture";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TakePicture_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Return;
        private CommonImageResizer imageResizer1;

    }
}