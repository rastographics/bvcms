using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CmsCheckin
{
	public partial class BuildingListFamily : UserControl
	{
		private const int ExtraPixelsName = 15;
		private const string STR_CheckMark = "ü";
		public BuildingListFamily()
		{
			InitializeComponent();
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			const int WM_KEYDOWN = 0x100;
			const int WM_SYSKEYDOWN = 0x104;

			if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN)) {
				switch (keyData) {
					case Keys.PageUp:
						if (pgup.Visible)
							ShowPage(page - 1);
						return true;
					case Keys.PageDown:
						if (pgdn.Visible)
							ShowPage(page + 1);
						return true;
					case Keys.Escape:
						Program.TimerStop();
						this.GoHome(string.Empty);
						return true;
					case Keys.Return:
						Program.TimerStop();
						//DoPrinting(null, null);
						this.GoHome(string.Empty);
						return true;
					case Keys.S | Keys.Alt:
						Program.TimerReset();
						Program.CursorShow();
						foreach (var c in sucontrols) {
							c.Enabled = true;
							c.BackColor = Color.Coral;
						}
						return true;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		public static List<PersonInfo> list;
		int page = 1;
		List<Control> controls = new List<Control>();
		List<Control> sucontrols = new List<Control>();
		XDocument xdoc;
		const string Verdana = "Verdana";
		Font pfont = new Font(Verdana, 14f, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

		public void ShowFamily(int fid)
		{
			//page = 1;
			//xdoc = this.GetDocument("Checkin2/SingleFamily/" + fid + "?Building=" + Program.Building);

			Program.FamilyId = fid;

			var url = "Checkin2/SingleFamily/" + fid;

			var post = new NameValueCollection();
			post.Add("building", Program.settings.building);
			post.Add("querybit", Program.BuildingInfo.querybit);

			var x = this.PostDocument(url, post);

			ShowFamily(x);
		}

		public void ShowFamily(XDocument x)
		{
			xdoc = x;
			this.Focus();
			Program.FamilyId = x.Root.Attribute("familyid").Value.ToInt();

			list = new List<PersonInfo>();
			if (x.Descendants("member").Count() == 0) {
				ClearControls();
				var lab = new Label();
				lab.Font = pfont;
				lab.Location = new Point(15, 200);
				lab.AutoSize = true;
				pgup.Visible = false;
				pgdn.Visible = false;
				lab.Text = "Not Found, try another phone number, or 411?";
				this.Controls.Add(lab);
				Return.Text = "Try Again";
				controls.Add(lab);
				return;
			} else {
				Return.Text = "Return";
			}

			foreach (var e in x.Descendants("member")) {
				var a = new PersonInfo {
					pid = e.Attribute("id").Value.ToInt(),
					first = e.Attribute("first").Value,
					last = e.Attribute("last").Value,
					dob = e.Attribute("dob").Value,

					goesby = e.Attribute("goesby").Value,
					email = e.Attribute("email").Value,
					addr = e.Attribute("addr").Value,
					zip = e.Attribute("zip").Value,
					home = e.Attribute("home").Value.FmtFone(),
					cell = e.Attribute("cell").Value.FmtFone(),
					gender = e.Attribute("gender").Value.ToInt(),
					marital = e.Attribute("marital").Value.ToInt(),
					Row = list.Count,
					HasPicture = bool.Parse(e.Attribute("haspicture").Value),
					MemberStatus = e.Attribute("memberstatus").Value,
					access = e.Attribute("access").Value,
					notes = e.Value,
					visitcount = e.Attribute("visitcount").Value.ToInt(),
				};
				list.Add(a);
			}
			ShowPage(1);
		}
		public void ShowPage(int page)
		{
			ClearControls();
			this.page = page;

			const int sep = 10;
			const int rowheight = 50;
			int top = 50;
			const int bsize = 45;
			const int bwid = 65;
			const int mwid = 80;

			var points = 14F;
			var g = this.CreateGraphics();

			Font font;
			Font labfont;
			string Present = "Attend";
			string Labels = "Labels";
			//Return.Text = "Print Labels, Return";

			var cols = new int[6];

			const int PageSize = 10;

			int srow = (page - 1) * PageSize;
			int erow = srow + PageSize;
			if (erow > list.Count)
				erow = list.Count;
			pgdn.Visible = list.Count > erow;
			pgup.Visible = srow > 0;

			int maxheight;
			int twidab, widname, widorg, twidlb;
			int totalwid;

			while (true) {
				twidab = widname = widorg = twidlb = maxheight = 0;
				font = new Font(Verdana, points, FontStyle.Regular,
					GraphicsUnit.Point, ((byte)(0)));
				labfont = new Font(Verdana, points,
					((FontStyle)((FontStyle.Italic | FontStyle.Underline))),
					GraphicsUnit.Point, ((byte)(0)));
				maxheight = 0;
				foreach (var c in list) {
					var size = g.MeasureString(Present, labfont);
					twidab = Math.Max(twidab, (int)Math.Ceiling(size.Width));
					twidab = Math.Max(twidab, bwid);

					size = g.MeasureString(c.name, font);
					widname = Math.Max(widname, (int)Math.Ceiling(size.Width) + ExtraPixelsName);

					size = g.MeasureString(Labels, labfont);
					twidlb = Math.Max(twidlb, (int)Math.Ceiling(size.Width));
					twidlb = Math.Max(twidlb, mwid);

					size = g.MeasureString("Choose activities to show here, this needs to be really long", font);
					widorg = Math.Max(widorg, (int)Math.Ceiling(size.Width));

					size = g.MeasureString("|", labfont);
					maxheight = Math.Max(maxheight, (int)Math.Ceiling(size.Height));
				}

				totalwid = sep + twidab + sep + widname + sep + widorg
									+ sep + twidlb + sep;
				if (totalwid > 1024) {
					points -= 1F;
					continue;
				}
				break;
			}
			var labtop = top - rowheight;
			var LeftEdge = (1024 - totalwid) / 2;

			var head = new Label();
			LeftEdge += sep;
			head.Location = new Point(LeftEdge, labtop);
			head.Size = new Size(twidab + 5, maxheight);
			head.Font = labfont;
			head.Text = Present;
			this.Controls.Add(head);
			controls.Add(head);

			head = new Label();
			LeftEdge += twidab + sep;
			head.Location = new Point(LeftEdge, labtop);
			head.Size = new Size(widname + 5, maxheight);
			head.Font = labfont;
			head.Text = "Name";
			this.Controls.Add(head);
			controls.Add(head);

			head = new Label();
			LeftEdge += mwid + sep + widname + sep;
			head.Location = new Point(LeftEdge, labtop);
			head.Size = new Size(widorg + 5, maxheight);
			head.Font = labfont;
			head.Text = "Activity";
			this.Controls.Add(head);
			controls.Add(head);

			for (var r = srow; r < erow; r++) {
				var c = list[r];

				LeftEdge = (1024 - totalwid) / 2;
				top += rowheight;

				var ab = new Button();
				LeftEdge += sep;
				ab.Location = new Point(LeftEdge, top - 5);
				ab.Size = new Size(bwid, bsize);

				ab.FlatStyle = FlatStyle.Flat;
				ab.FlatAppearance.BorderSize = 1;

				ab.BackColor = Color.CornflowerBlue;
				ab.FlatAppearance.BorderColor = Color.Black;


				ab.ForeColor = Color.White;
				ab.Font = new Font("Wingdings", 24, FontStyle.Bold,
					GraphicsUnit.Point, ((byte)(2)));
				ab.Name = "attend" + c.Row;
				ab.TextAlign = ContentAlignment.TopCenter;
				ab.UseVisualStyleBackColor = false;
				this.Controls.Add(ab);
				ab.KeyDown += AttendButton_KeyDown;
				ab.Click += Attend_Click;
				ab.Tag = c.Row;
				controls.Add(ab);

				var nam = new Button();
				LeftEdge += twidab + sep;
				nam.UseVisualStyleBackColor = false;
				nam.FlatStyle = FlatStyle.Flat;
				nam.FlatAppearance.BorderSize = 1;
				nam.FlatAppearance.BorderColor = Color.Black;
				if (c.HasPicture)
					nam.BackColor = Color.FromArgb(0xFF, 0xCC, 0x99);
				else
					nam.BackColor = Color.White;

				nam.Location = new Point(LeftEdge, top - 5);
				nam.Size = new Size(widname, bsize);

				nam.Font = font;
				nam.UseMnemonic = false;
				nam.Text = c.name;
				nam.Name = "name" + c.Row;
				nam.TextAlign = ContentAlignment.MiddleLeft;

				nam.Click += new EventHandler(ShowPic_Click);
				nam.Enabled = false;
				nam.Tag = c.Row;
				this.Controls.Add(nam);
				controls.Add(nam);
				sucontrols.Add(nam);

				var menu = new Button();
				LeftEdge += widname + 5 + sep;
				menu.Location = new Point(LeftEdge, top - 5);
				menu.Size = new Size(mwid, bsize);
				menu.Text = "menu";
				menu.BackColor = SystemColors.Control;
				menu.Enabled = false;
				menu.Font = pfont;
				menu.Name = "menu" + c.Row;
				menu.Tag = c.Row;
				menu.TextAlign = ContentAlignment.TopCenter;
				menu.UseVisualStyleBackColor = false;
				this.Controls.Add(menu);
				menu.Click += new EventHandler(Menu_Click);
				controls.Add(menu);
				sucontrols.Add(menu);

				var org = new Label();
				LeftEdge += mwid + 5 + sep;
				org.Location = new Point(LeftEdge, top);
				org.Size = new Size(widorg + 5, maxheight);
				org.Font = font;
				org.UseMnemonic = false;
				org.Text = "Choose Activities";
				org.TextAlign = ContentAlignment.MiddleLeft;
				org.Name = "org" + c.Row;
				this.Controls.Add(org);
				controls.Add(org);
			}
			Program.TimerStart(timer1_Tick);
			ComputeLabels();
		}

		void timer1_Tick(object sender, EventArgs e)
		{
			Program.TimerStop();
			Util.UnLockFamily();
			Program.ClearFields();
			this.GoHome("");
		}

		void AttendButton_KeyDown(object sender, KeyEventArgs e)
		{
			Program.TimerReset();
			if (e.KeyValue == 27) {
				Program.TimerStop();
				Util.UnLockFamily();
				this.GoHome(string.Empty);
			}
		}

		void Attend_Click(object sender, EventArgs e)
		{
			Attend_Click((Button)sender);
		}

		public void Attend_Click(Button ab)
		{
			Program.TimerReset();
			var c = list[(int)ab.Tag];
			if (c.lastpress.HasValue && DateTime.Now.Subtract(c.lastpress.Value).TotalSeconds < 1)
				return;

			if (c.access == "restricted") {
				Program.attendant.AddHistoryString("-- " + DateTime.Now.ToString("MM-dd-yy hh:mm tt") + " Check-in rejected for " + c.name + " because they are not permitted to use the facility.");
				MessageBox.Show("An access error has occurred. Please check with the attendant.", "Access Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			} else if (c.access == "timer") {
				Program.attendant.AddHistoryString("-- " + DateTime.Now.ToString("MM-dd-yy hh:mm tt") + " Check-in rejected for " + c.name + " because they have logged in recently.");
				MessageBox.Show("An access error has occurred. Please check with the attendant.", "Access Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			} else if (c.access == "true") {
				c.accesstype = 1;
			} else {
				if (c.visitcount < Program.BuildingInfo.maxvisits && Program.addguests == null) {
					MessageBox.Show("Welcome " + c.name + "! This is visit number " + (c.visitcount + 1) + " of " + Program.BuildingInfo.maxvisits + ".", "Guest Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
					c.accesstype = 2;
				} else {
					if (Program.addguests == null) {
						Program.attendant.AddHistoryString("-- " + DateTime.Now.ToString("MM-dd-yy hh:mm tt") + " Check-in rejected for " + c.name + " because they are not a member.");
						MessageBox.Show("Only members may check-in. You will need to be a guest of a member to check-in.", "Members Only", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}

					if (Program.BuildingInfo.maxguests > -1) {
						var p = Program.GuestOf();

						if (p != null && Util.GetGuestCount(p.pid) >= Program.BuildingInfo.maxguests) {
							Program.attendant.AddHistoryString("-- " + DateTime.Now.ToString("MM-dd-yy hh:mm tt") + " Check-in rejected for " + c.name + " because " + p.name + " reached the guest limit.");
							MessageBox.Show("No additional guests are permitted for today.", "Guest Limit Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}

						c.accesstype = 3;
					}
				}
			}

			var pb = Program.attendant.pictureBox1;
			pb.SetPropertyThreadSafe(() => pb.Image, Util.GetImage(c.pid));
			var na = Program.attendant.NameDisplay;
			na.SetPropertyThreadSafe(() => na.Text, c.name);
			var notes = Program.attendant.notes;
			notes.SetPropertyThreadSafe(() => notes.Text, c.notes);

			activities = new BuildingChooseActivities();
			activities.ControlBox = false;
			activities.Tag = ab.Tag;
			activities.StartPosition = FormStartPosition.Manual;
			activities.Location = new Point(Program.baseform.Location.X + 100, Program.baseform.Location.Y + 100);
			activities.Text = "Choose Activities for " + c.name;

			activities.list.Items.Clear();
			foreach (var i in Program.BuildingInfo.Activities)
				activities.list.Items.Add(i);
			activities.ok.Tag = ab;
			activities.cancel.Tag = ab;
			activities.AcceptButton = activities.ok;
			activities.CancelButton = activities.cancel;
			activities.ok.Click += ok_Click;
			activities.cancel.Click += cancel_Click;

			mask = new Label();
			mask.BackColor = BackColor;
			mask.Size = Size;
			//mask.Location = this.Location;
			mask.Parent = this;
			mask.BringToFront();
			var nam = this.Controls[this.Controls.IndexOfKey("name" + ab.Tag.ToString())] as Button;
			var org = this.Controls[this.Controls.IndexOfKey("org" + ab.Tag.ToString())] as Label;
			nam.BringToFront();
			org.BringToFront();

			mask.Show();
			nam.Enabled = false;
			activities.ShowDialog();
			activities.BringToFront();
		}

		void ok_Click(object sender, EventArgs e)
		{
			var ok = (Button)sender;
			var ab = ok.Tag as Button;
			Cursor.Current = Cursors.WaitCursor;
			Program.CursorShow();
			var c = list[(int)ab.Tag];
			ab.Text = STR_CheckMark;
			c.lastpress = DateTime.Now;
			var bw = new BackgroundWorker();
			bw.DoWork += CheckUnCheckDoWork;
			bw.RunWorkerCompleted += CheckUncheckCompleted;
			bw.RunWorkerAsync(ok);
		}
		void cancel_Click(object sender, EventArgs e)
		{
			var cancel = (Button)sender;
			var ab = cancel.Tag as Button;
			Cursor.Current = Cursors.WaitCursor;
			Program.CursorShow();
			var c = list[(int)ab.Tag];
			ab.Text = "";
			var org = this.Controls[this.Controls.IndexOfKey("org" + activities.Tag.ToString())] as Label;
			org.Text = "Choose Activities";
			c.lastpress = DateTime.Now;
			if (c.CheckinId.HasValue)
				Util.BuildingUnCheckin(c.CheckinId.Value);
			RemoveActivities();
			c.CheckinId = null;
		}
		private void CheckUnCheckDoWork(object sender, DoWorkEventArgs e)
		{
			var ok = e.Argument as Button;
			var ab = ok.Tag as Button;
			var c = list[(int)ab.Tag];
			var f = ok.Parent as BuildingChooseActivities;
			var items = f.list.CheckedItems.OfType<Activity>().ToList();
			if (items.Count == 0)
				items.Add(new Activity { name = "Other" });
			c.CheckinId = Util.BuildingCheckin(c.pid, items, c.accesstype);
			c.ischecked = true;
			c.Items = items;
		}
		private void CheckUncheckCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Cursor.Current = Cursors.Default;
			var org = this.Controls[this.Controls.IndexOfKey("org" + activities.Tag.ToString())] as Label;
			var ab = this.Controls[this.Controls.IndexOfKey("attend" + activities.Tag.ToString())] as Button;
			var c = list[(int)ab.Tag];
			org.Text = c.ItemsDisplay();
			Program.attendant.AddHistory(c);
			RemoveActivities();
			Program.CursorHide();
		}

		void ShowPic_Click(object sender, EventArgs e)
		{
			Program.TimerReset();
			var eb = sender as Button;
			var ab = this.Controls[this.Controls.IndexOfKey("attend" + eb.Tag.ToString())] as Button;
			var c = list[(int)ab.Tag];
			Program.PeopleId = c.pid;
			var f = new CommonPicture();
			f.ShowDialog();
		}

		public List<ClassInfo> classlist = new List<ClassInfo>();
		private Label mask;
		private BuildingMenu menu;
		private BuildingChooseActivities activities;

		void Menu_Click(object sender, EventArgs e)
		{
			Program.TimerReset();
			var MenuButton = sender as Button;
			menu = new BuildingMenu();
			menu.Tag = MenuButton.Tag;
			menu.Parent = this;
			menu.Location = new Point(MenuButton.Location.X - 100, MenuButton.Location.Y + MenuButton.Height);
			mask = new Label();
			mask.BackColor = this.BackColor;
			mask.Size = this.Size;
			//mask.Location = this.Location;
			mask.Parent = this;
			mask.BringToFront();
			var nam = this.Controls[this.Controls.IndexOfKey("name" + menu.Tag.ToString())] as Button;
			var org = this.Controls[this.Controls.IndexOfKey("org" + menu.Tag.ToString())] as Label;
			nam.BringToFront();
			org.BringToFront();

			mask.Show();
			var c = list[(int)menu.Tag];
			nam.Enabled = false;
			menu.EditRecord += EditRecord_Click;
			menu.AddFamily += AddToFamily_Click;
			menu.CancelMenu += CancelMenu_Click;
			menu.Show();
			menu.BringToFront();
		}

		void CancelMenu_Click(object sender, EventArgs e)
		{
			RemoveMenu();
		}
		private void ComputeLabels()
		{
			//Return.Text = "Print Labels, Return";
		}

		void EditRecord_Click(object sender, EventArgs e)
		{
			var c = list[(int)menu.Tag];

			var home = Program.buildingHome;
			Program.PeopleId = c.pid;
			home.SetFields(c.last, c.email, c.addr, c.zip, c.home);
			home.first.textBox1.Text = c.first;
			home.goesby.textBox1.Text = c.goesby;
			home.dob.textBox1.Text = c.dob;
			home.cellphone.textBox1.Text = c.cell.FmtFone();
			home.gendermarital.Marital = c.marital;
			home.gendermarital.Gender = c.gender;
			Util.UnLockFamily();

			Program.editing = true;
			RemoveMenu();
			this.Swap(home.first);
		}
		private void RemoveMenu()
		{
			var nam = this.Controls[this.Controls.IndexOfKey("name" + menu.Tag.ToString())] as Button;
			nam.Enabled = true;
			this.Controls.Remove(menu);
			this.Controls.Remove(mask);
			menu.Dispose();
			mask.Dispose();
		}
		private void RemoveActivities()
		{
			var nam = this.Controls[this.Controls.IndexOfKey("name" + activities.Tag.ToString())] as Button;
			nam.Enabled = true;
			this.Controls.Remove(activities);
			this.Controls.Remove(mask);
			activities.Dispose();
			activities.Dispose();
		}

		PleaseWait PleaseWaitForm = null;
		private void ClearControls()
		{
			foreach (var c in controls) {
				this.Controls.Remove(c);
				c.Dispose();
			}
			controls.Clear();
			sucontrols.Clear();
		}

		private void MagicButton_Click(object sender, EventArgs e)
		{
			if (!Program.CheckAdminPIN()) return;

			Program.TimerStop();
			if (list.Count == 0) {
				if (Program.baseform.textbox.Parent is AttendHome)
					this.Swap(Program.attendHome.namesearch);
				else if (Program.baseform.textbox.Parent is BuildingHome)
					this.Swap(Program.buildingHome.namesearch);
				return;
			}
			foreach (var c in sucontrols) {
				c.Enabled = true;
				if (c.BackColor == SystemColors.Control)
					c.BackColor = Color.Coral;
			}
		}

		private void pgup_Click(object sender, EventArgs e)
		{
			ShowPage(page - 1);
		}

		private void pgdn_Click(object sender, EventArgs e)
		{
			ShowPage(page + 1);
		}

		private void AddToFamily_Click(object sender, EventArgs e)
		{
			var c = list[(int)menu.Tag];
			Program.editing = false;
			Util.UnLockFamily();
			RemoveMenu();

			Program.buildingHome.SetFields(c.last, c.email, c.addr, c.zip, c.home);
			this.Swap(Program.buildingHome.first);
		}

		private void Return_Click(object sender, EventArgs e)
		{
			Program.TimerStop();

			if (Return.Text.Contains("Try Again")) {
				this.GoHome(string.Empty);
				return;
			}

			if (Program.addguests == null && list.Where(i => i.ischecked && i.access == "true").Any()) {
				Program.addguests = new BuildingAddGuests();
				Program.addguests.StartPosition = FormStartPosition.Manual;
				Program.addguests.Location = new Point(Program.baseform.Location.X + 100, Program.baseform.Location.Y + 100);
				var i = 1;
				foreach (var p in list.Where(pp => pp.ischecked && pp.access == "true")) {
					var rb = Program.addguests.Controls.Find("rb" + i, true)[0] as RadioButton;
					rb.Text = p.name;
					rb.Visible = true;
					rb.Tag = p;
					if (i == 1)
						rb.Checked = true;
					i++;
				}
			}

			if (Program.addguests != null) {
				var ret = Program.addguests.ShowDialog();
				if (ret == DialogResult.No) {
					Program.addguests.Dispose();
					Program.addguests = null;
				}
			}

			PleaseWaitForm = new PleaseWait();
			PleaseWaitForm.Show();

			var bw = new BackgroundWorker();
			bw.DoWork += DoPrinting;
			bw.RunWorkerCompleted += PrintingCompleted;
			bw.RunWorkerAsync();
		}
		private void DoPrinting(object sender, DoWorkEventArgs e)
		{

		}
		private void PrintingCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			PleaseWaitForm.Hide();
			PleaseWaitForm.Dispose();
			PleaseWaitForm = null;
			Program.FamilyId = 0;
			classlist = new List<ClassInfo>();
			this.GoHome(string.Empty);
		}

		// all of these come from the attribtues on the attendee element
		// attributes have the same name unless noted otherwise
		[Serializable]
		public class LabelInfo
		{
			public int n { get; set; } // numlabels attribute
			public string location { get; set; } // loc attribute
			public string allergies { get; set; }
			public string org { get; set; } // orgname attribute
			public DateTime? hour { get; set; }
			public int pid { get; set; } // id attribute
			public string mv { get; set; }
			public string first { get; set; }
			public string last { get; set; }
			public bool transport { get; set; }
			public bool custody { get; set; }
			public bool requiressecuritylabel { get; set; }
		}
	}

	/*
	[Serializable]
	public class Activity
	{
		[XmlAttribute]
		public string name { get; set; }
		[XmlAttribute]
		public int org { get; set; }
		[XmlText]
		public string display { get; set; }
		public override string ToString()
		{
			return display;
		}
	}
	*/

	public class PersonInfo
	{
		public DateTime? lastpress { get; set; }
		public bool ischecked { get; set; }
		public int? CheckinId { get; set; }
		public List<Activity> Items { get; set; }
		public int pid { get; set; }
		public string first { get; set; }
		public string last { get; set; }
		public string dob { get; set; }

		public string goesby { get; set; }
		public string email { get; set; }
		public string addr { get; set; }
		public string zip { get; set; }
		public string home { get; set; }
		public string cell { get; set; }

		public int gender { get; set; }
		public int marital { get; set; }

		public string activities { get; set; }
		public int Row { get; set; }
		public bool HasPicture { get; set; }
		public string MemberStatus { get; set; }
		public string notes { get; set; }
		public string access { get; set; }
		public int visitcount { get; set; }
		public int accesstype { get; set; }

		public string name
		{
			get { return first + " " + last; }
		}
		public string ItemsDisplay()
		{
			var s = string.Join(",", Items.Select(ii => ii.display));
			if (!s.HasValue())
				s = "Other";
			return s;
		}

		public override string ToString()
		{
			return String.Format("{0:MM-dd-yy hh:mm tt} {1} ({2})", lastpress, name, ItemsDisplay());
		}
	}

}
