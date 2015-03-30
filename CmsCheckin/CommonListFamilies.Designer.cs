namespace CmsCheckin
{
    partial class CommonListFamilies
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
            this.Return = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pgdn = new System.Windows.Forms.Button();
            this.pgup = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Return
            // 
            this.Return.AutoSize = true;
            this.Return.BackColor = System.Drawing.Color.LightGreen;
            this.Return.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
            this.Return.FlatAppearance.BorderSize = 2;
            this.Return.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Return.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Return.ForeColor = System.Drawing.Color.Black;
            this.Return.Location = new System.Drawing.Point(422, 674);
            this.Return.Margin = new System.Windows.Forms.Padding(4);
            this.Return.Name = "Return";
            this.Return.Size = new System.Drawing.Size(583, 66);
            this.Return.TabIndex = 11;
            this.Return.Text = "Return to Phone Number";
            this.Return.UseVisualStyleBackColor = false;
            this.Return.Click += new System.EventHandler(this.GoBack_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(273, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(257, 37);
            this.label1.TabIndex = 12;
            this.label1.Text = "Choose a Family";
            // 
            // pgdn
            // 
            this.pgdn.AutoSize = true;
            this.pgdn.BackColor = System.Drawing.Color.LightGray;
            this.pgdn.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
            this.pgdn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pgdn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pgdn.ForeColor = System.Drawing.Color.Black;
            this.pgdn.Location = new System.Drawing.Point(900, 612);
            this.pgdn.Margin = new System.Windows.Forms.Padding(4);
            this.pgdn.Name = "pgdn";
            this.pgdn.Size = new System.Drawing.Size(120, 53);
            this.pgdn.TabIndex = 14;
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
            this.pgup.Location = new System.Drawing.Point(902, 4);
            this.pgup.Margin = new System.Windows.Forms.Padding(4);
            this.pgup.Name = "pgup";
            this.pgup.Size = new System.Drawing.Size(118, 53);
            this.pgup.TabIndex = 13;
            this.pgup.Text = "PgUp";
            this.pgup.UseVisualStyleBackColor = false;
            this.pgup.Click += new System.EventHandler(this.pgup_Click);
            // 
            // ListFamilies
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pgdn);
            this.Controls.Add(this.pgup);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Return);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ListFamilies";
            this.Size = new System.Drawing.Size(1024, 768);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Return;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button pgdn;
        public System.Windows.Forms.Button pgup;
    }
}
