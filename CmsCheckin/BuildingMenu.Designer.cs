namespace CmsCheckin
{
    partial class BuildingMenu
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
			this.Cancel = new System.Windows.Forms.Button();
			this.Add = new System.Windows.Forms.Button();
			this.Edit = new System.Windows.Forms.Button();
			this.Print = new System.Windows.Forms.Button();
			this.addidcard = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Cancel
			// 
			this.Cancel.BackColor = System.Drawing.Color.Ivory;
			this.Cancel.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
			this.Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Cancel.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Cancel.ForeColor = System.Drawing.Color.Black;
			this.Cancel.Location = new System.Drawing.Point(98, 111);
			this.Cancel.Margin = new System.Windows.Forms.Padding(4);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(179, 45);
			this.Cancel.TabIndex = 22;
			this.Cancel.Text = "Cancel";
			this.Cancel.UseVisualStyleBackColor = false;
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// Add
			// 
			this.Add.BackColor = System.Drawing.Color.IndianRed;
			this.Add.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
			this.Add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Add.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Add.ForeColor = System.Drawing.Color.Black;
			this.Add.Location = new System.Drawing.Point(191, 4);
			this.Add.Margin = new System.Windows.Forms.Padding(4);
			this.Add.Name = "Add";
			this.Add.Size = new System.Drawing.Size(179, 45);
			this.Add.TabIndex = 19;
			this.Add.Text = "Add to Family";
			this.Add.UseVisualStyleBackColor = false;
			this.Add.Click += new System.EventHandler(this.Add_Click);
			// 
			// Edit
			// 
			this.Edit.BackColor = System.Drawing.Color.Coral;
			this.Edit.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
			this.Edit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Edit.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Edit.ForeColor = System.Drawing.Color.Black;
			this.Edit.Location = new System.Drawing.Point(4, 4);
			this.Edit.Margin = new System.Windows.Forms.Padding(4);
			this.Edit.Name = "Edit";
			this.Edit.Size = new System.Drawing.Size(179, 45);
			this.Edit.TabIndex = 18;
			this.Edit.Text = "Edit Record";
			this.Edit.UseVisualStyleBackColor = false;
			this.Edit.Click += new System.EventHandler(this.Edit_Click);
			// 
			// Print
			// 
			this.Print.BackColor = System.Drawing.Color.Yellow;
			this.Print.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
			this.Print.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Print.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Print.ForeColor = System.Drawing.Color.Black;
			this.Print.Location = new System.Drawing.Point(191, 57);
			this.Print.Margin = new System.Windows.Forms.Padding(4);
			this.Print.Name = "Print";
			this.Print.Size = new System.Drawing.Size(179, 45);
			this.Print.TabIndex = 17;
			this.Print.Text = "Print Label";
			this.Print.UseVisualStyleBackColor = false;
			this.Print.Click += new System.EventHandler(this.Print_Click);
			// 
			// addidcard
			// 
			this.addidcard.BackColor = System.Drawing.Color.Cyan;
			this.addidcard.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
			this.addidcard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.addidcard.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.addidcard.ForeColor = System.Drawing.Color.Black;
			this.addidcard.Location = new System.Drawing.Point(4, 57);
			this.addidcard.Margin = new System.Windows.Forms.Padding(4);
			this.addidcard.Name = "addidcard";
			this.addidcard.Size = new System.Drawing.Size(179, 45);
			this.addidcard.TabIndex = 23;
			this.addidcard.Text = "Add ID Card";
			this.addidcard.UseVisualStyleBackColor = false;
			this.addidcard.Click += new System.EventHandler(this.addidcard_Click);
			// 
			// Menu2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.addidcard);
			this.Controls.Add(this.Cancel);
			this.Controls.Add(this.Add);
			this.Controls.Add(this.Edit);
			this.Controls.Add(this.Print);
			this.Name = "Menu2";
			this.Size = new System.Drawing.Size(376, 164);
			this.ResumeLayout(false);

        }

        #endregion

		public System.Windows.Forms.Button Cancel;
        public System.Windows.Forms.Button Add;
        public System.Windows.Forms.Button Edit;
		public System.Windows.Forms.Button Print;
		public System.Windows.Forms.Button addidcard;
    }
}
