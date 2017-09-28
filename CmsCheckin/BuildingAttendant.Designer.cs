namespace CmsCheckin
{
	partial class BuildingAttendant
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
            this.ShowCheckin = new System.Windows.Forms.Button();
            this.NameDisplay = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.notes = new System.Windows.Forms.TextBox();
            this.save = new System.Windows.Forms.Button();
            this.history = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ShowCheckin
            // 
            this.ShowCheckin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShowCheckin.Location = new System.Drawing.Point(12, 12);
            this.ShowCheckin.Name = "ShowCheckin";
            this.ShowCheckin.Size = new System.Drawing.Size(110, 65);
            this.ShowCheckin.TabIndex = 0;
            this.ShowCheckin.Text = "Show Checkin";
            this.ShowCheckin.UseVisualStyleBackColor = true;
            this.ShowCheckin.Click += new System.EventHandler(this.ShowCheckin_Click);
            // 
            // NameDisplay
            // 
            this.NameDisplay.AutoSize = true;
            this.NameDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameDisplay.Location = new System.Drawing.Point(552, 12);
            this.NameDisplay.Name = "NameDisplay";
            this.NameDisplay.Size = new System.Drawing.Size(61, 24);
            this.NameDisplay.TabIndex = 1;
            this.NameDisplay.Text = "Name";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(553, 41);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(419, 442);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "History";
            // 
            // notes
            // 
            this.notes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notes.Location = new System.Drawing.Point(553, 531);
            this.notes.Multiline = true;
            this.notes.Name = "notes";
            this.notes.Size = new System.Drawing.Size(419, 119);
            this.notes.TabIndex = 8;
            this.notes.Text = "notes";
            this.notes.TextChanged += new System.EventHandler(this.onNotesTextChange);
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(887, 502);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(85, 23);
            this.save.TabIndex = 10;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // history
            // 
            this.history.FormattingEnabled = true;
            this.history.Location = new System.Drawing.Point(16, 112);
            this.history.Name = "history";
            this.history.Size = new System.Drawing.Size(512, 537);
            this.history.TabIndex = 11;
            this.history.SelectedIndexChanged += new System.EventHandler(this.history_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(553, 511);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Notes";
            // 
            // BuildingAttendant
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 662);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.history);
            this.Controls.Add(this.save);
            this.Controls.Add(this.notes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.NameDisplay);
            this.Controls.Add(this.ShowCheckin);
            this.Name = "BuildingAttendant";
            this.Text = "Attendant";
            this.LocationChanged += new System.EventHandler(this.Attendant_LocationChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button ShowCheckin;
		public System.Windows.Forms.Label NameDisplay;
		public System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.TextBox notes;
		private System.Windows.Forms.Button save;
		public System.Windows.Forms.ListBox history;
		private System.Windows.Forms.Label label1;
	}
}