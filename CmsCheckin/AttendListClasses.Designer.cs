namespace CmsCheckin
{
    partial class AttendListClasses
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
            this.GoBackButton = new System.Windows.Forms.Button();
            this.pgup = new System.Windows.Forms.Button();
            this.pgdn = new System.Windows.Forms.Button();
            this.allclasses = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GoBackButton
            // 
            this.GoBackButton.AutoSize = true;
            this.GoBackButton.BackColor = System.Drawing.Color.LightGreen;
            this.GoBackButton.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
            this.GoBackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GoBackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GoBackButton.ForeColor = System.Drawing.Color.Black;
            this.GoBackButton.Location = new System.Drawing.Point(751, 687);
            this.GoBackButton.Margin = new System.Windows.Forms.Padding(4);
            this.GoBackButton.Name = "GoBackButton";
            this.GoBackButton.Size = new System.Drawing.Size(269, 66);
            this.GoBackButton.TabIndex = 10;
            this.GoBackButton.Text = "Cancel";
            this.GoBackButton.UseVisualStyleBackColor = false;
            this.GoBackButton.Click += new System.EventHandler(this.GoBack_Click);
            // 
            // pgup
            // 
            this.pgup.AutoSize = true;
            this.pgup.BackColor = System.Drawing.Color.LightGray;
            this.pgup.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
            this.pgup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pgup.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pgup.ForeColor = System.Drawing.Color.Black;
            this.pgup.Location = new System.Drawing.Point(906, 7);
            this.pgup.Margin = new System.Windows.Forms.Padding(4);
            this.pgup.Name = "pgup";
            this.pgup.Size = new System.Drawing.Size(118, 53);
            this.pgup.TabIndex = 11;
            this.pgup.Text = "PgUp";
            this.pgup.UseVisualStyleBackColor = false;
            this.pgup.Click += new System.EventHandler(this.pgup_Click);
            // 
            // pgdn
            // 
            this.pgdn.AutoSize = true;
            this.pgdn.BackColor = System.Drawing.Color.LightGray;
            this.pgdn.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
            this.pgdn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pgdn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pgdn.ForeColor = System.Drawing.Color.Black;
            this.pgdn.Location = new System.Drawing.Point(904, 615);
            this.pgdn.Margin = new System.Windows.Forms.Padding(4);
            this.pgdn.Name = "pgdn";
            this.pgdn.Size = new System.Drawing.Size(120, 53);
            this.pgdn.TabIndex = 12;
            this.pgdn.Text = "PgDn";
            this.pgdn.UseVisualStyleBackColor = false;
            this.pgdn.Click += new System.EventHandler(this.pgdn_Click);
            // 
            // allclasses
            // 
            this.allclasses.FlatAppearance.BorderSize = 0;
            this.allclasses.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.allclasses.Font = new System.Drawing.Font("Verdana", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.allclasses.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.allclasses.Location = new System.Drawing.Point(3, 677);
            this.allclasses.Name = "allclasses";
            this.allclasses.Size = new System.Drawing.Size(81, 88);
            this.allclasses.TabIndex = 14;
            this.allclasses.Text = ".";
            this.allclasses.UseVisualStyleBackColor = true;
            this.allclasses.Click += new System.EventHandler(this.allclasses_Click);
            // 
            // ListClasses
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.allclasses);
            this.Controls.Add(this.pgdn);
            this.Controls.Add(this.pgup);
            this.Controls.Add(this.GoBackButton);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ListClasses";
            this.Size = new System.Drawing.Size(1024, 768);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button GoBackButton;
        public System.Windows.Forms.Button pgup;
        public System.Windows.Forms.Button pgdn;
        private System.Windows.Forms.Button allclasses;

    }
}
