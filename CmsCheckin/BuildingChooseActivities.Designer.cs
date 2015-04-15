namespace CmsCheckin
{
	partial class BuildingChooseActivities
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
			this.list = new System.Windows.Forms.CheckedListBox();
			this.ok = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// list
			// 
			this.list.CheckOnClick = true;
			this.list.Dock = System.Windows.Forms.DockStyle.Top;
			this.list.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.list.FormattingEnabled = true;
			this.list.Location = new System.Drawing.Point(0, 0);
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size(578, 277);
			this.list.TabIndex = 0;
			// 
			// ok
			// 
			this.ok.BackColor = System.Drawing.Color.DodgerBlue;
			this.ok.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ok.ForeColor = System.Drawing.Color.White;
			this.ok.Location = new System.Drawing.Point(484, 296);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(82, 49);
			this.ok.TabIndex = 1;
			this.ok.Text = "OK";
			this.ok.UseVisualStyleBackColor = false;
			// 
			// cancel
			// 
			this.cancel.BackColor = System.Drawing.Color.Red;
			this.cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cancel.ForeColor = System.Drawing.Color.White;
			this.cancel.Location = new System.Drawing.Point(396, 296);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(82, 49);
			this.cancel.TabIndex = 2;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = false;
			// 
			// ChooseActivities
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(578, 376);
			this.ControlBox = false;
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.ok);
			this.Controls.Add(this.list);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ChooseActivities";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "ChooseActivities";
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.Button ok;
		public System.Windows.Forms.Button cancel;
		public System.Windows.Forms.CheckedListBox list;
	}
}