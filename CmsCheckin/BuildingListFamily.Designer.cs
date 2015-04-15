namespace CmsCheckin
{
    partial class BuildingListFamily
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.pgdn = new System.Windows.Forms.Button();
			this.pgup = new System.Windows.Forms.Button();
			this.Return = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Blue;
			this.label1.Location = new System.Drawing.Point(90, 698);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(309, 37);
			this.label1.TabIndex = 11;
			this.label1.Text = "Touch blue buttons";
			// 
			// button1
			// 
			this.button1.FlatAppearance.BorderSize = 0;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Font = new System.Drawing.Font("Verdana", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button1.ForeColor = System.Drawing.SystemColors.ButtonShadow;
			this.button1.Location = new System.Drawing.Point(3, 674);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(81, 88);
			this.button1.TabIndex = 13;
			this.button1.Text = ".";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.MagicButton_Click);
			// 
			// pgdn
			// 
			this.pgdn.AutoSize = true;
			this.pgdn.BackColor = System.Drawing.Color.LightGray;
			this.pgdn.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
			this.pgdn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.pgdn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pgdn.ForeColor = System.Drawing.Color.Black;
			this.pgdn.Location = new System.Drawing.Point(900, 618);
			this.pgdn.Margin = new System.Windows.Forms.Padding(4);
			this.pgdn.Name = "pgdn";
			this.pgdn.Size = new System.Drawing.Size(120, 53);
			this.pgdn.TabIndex = 15;
			this.pgdn.Text = "PgDn";
			this.pgdn.UseVisualStyleBackColor = false;
			this.pgdn.Click += new System.EventHandler(this.pgdn_Click);
			// 
			// pgup
			// 
			this.pgup.AutoSize = true;
			this.pgup.BackColor = System.Drawing.Color.LightGray;
			this.pgup.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
			this.pgup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.pgup.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pgup.ForeColor = System.Drawing.Color.Black;
			this.pgup.Location = new System.Drawing.Point(906, 5);
			this.pgup.Margin = new System.Windows.Forms.Padding(4);
			this.pgup.Name = "pgup";
			this.pgup.Size = new System.Drawing.Size(118, 53);
			this.pgup.TabIndex = 14;
			this.pgup.Text = "PgUp";
			this.pgup.UseVisualStyleBackColor = false;
			this.pgup.Click += new System.EventHandler(this.pgup_Click);
			// 
			// Return
			// 
			this.Return.BackColor = System.Drawing.Color.LimeGreen;
			this.Return.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
			this.Return.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Return.Font = new System.Drawing.Font("Verdana", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Return.ForeColor = System.Drawing.Color.Black;
			this.Return.Location = new System.Drawing.Point(439, 686);
			this.Return.Margin = new System.Windows.Forms.Padding(4);
			this.Return.Name = "Return";
			this.Return.Size = new System.Drawing.Size(381, 66);
			this.Return.TabIndex = 18;
			this.Return.Text = "Return";
			this.Return.UseVisualStyleBackColor = false;
			this.Return.Click += new System.EventHandler(this.Return_Click);
			// 
			// ListFamily2
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.Return);
			this.Controls.Add(this.pgdn);
			this.Controls.Add(this.pgup);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "ListFamily2";
			this.Size = new System.Drawing.Size(1024, 768);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button pgdn;
		public System.Windows.Forms.Button pgup;
		public System.Windows.Forms.Button Return;

    }
}
