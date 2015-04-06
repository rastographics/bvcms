using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CmsCheckin
{
    public partial class CommonListNames : UserControl
    {
		UserControl nextcontrol;
		public CommonListNames(UserControl next)
        {
            InitializeComponent();
			this.nextcontrol = next;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                switch (keyData)
                {
                    case Keys.PageUp:
                        if (pgup.Visible)
                            ShowResults(name, prev.Value);
                        return true;
                    case Keys.PageDown:
                        if (pgdn.Visible)
                            ShowResults(name, next.Value);
                        return true;
                    case Keys.Escape:
                        Program.TimerStop();
                        if (Program.settings.hideCursor)
                            Cursor.Hide();
                        this.GoHome(string.Empty);
                        return true;
                    case Keys.S | Keys.Alt:
                        Program.TimerReset();
                        Program.CursorShow();
                        foreach (var c in sucontrols)
                        {
                            c.Enabled = true;
                            c.BackColor = Color.Coral;
                        }
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        int? next, prev;
        string name;
        List<Control> controls = new List<Control>();
        List<Control> sucontrols = new List<Control>();

        public void ShowResults(string match, int page)
        {
            ClearControls();

            name = match;
            var x = this.GetDocument("Checkin2/NameSearch/" + name + "?page=" + page);

            var points = 14F;

            string Verdana = "Verdana";
            Font labfont;
            var g = this.CreateGraphics();

            name = x.Root.Attribute("name").Value;
            next = x.Root.Attribute("next").Value.ToInt2();
            prev = x.Root.Attribute("prev").Value.ToInt2();
            pgdn.Visible = next.HasValue;
            pgup.Visible = prev.HasValue;

            if (x.Descendants("person").Count() == 0)
            {
                var lab = new Label();
                lab.Font = new Font(Verdana, points, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                lab.Location = new Point(15, 200);
                lab.AutoSize = true;
                lab.Text = "Not Found, try another name?";
                this.Controls.Add(lab);
                controls.Add(lab);
                GoBackButton.Text = "Try again";
                return;
            }

            const int WidName = 890;
            while (true)
            {
                var wid = 0;
                labfont = new Font(Verdana, points, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                foreach (var e in x.Descendants("person"))
                {
                    var s = e.Attribute("display").Value;
                    var size = g.MeasureString(s, labfont);
                    wid = Math.Max(wid, (int)size.Width);
                }
                if (wid > WidName)
                {
                    points -= 1F;
                    continue;
                }
                break;
            }
            var row = 0;
            foreach (var e in x.Descendants("person"))
            {
                var ab = new Button();
                ab.BackColor = SystemColors.ControlLight;
                ab.Font = labfont;
                const int Gutter = 10;
                ab.Location = new Point(Gutter, 100 + (row * 50));
                ab.Tag = e.Attribute("fid").Value.ToInt();
                var homephone = e.Attribute("home").Value;
                var cellphone = e.Attribute("cell").Value;
                var ph = homephone.HasValue() ? homephone : cellphone.HasValue() ? cellphone : "";
                if (!ph.HasValue())
                    ab.BackColor = Color.LightPink;

                ab.Size = new Size(WidName, 45);
                ab.TextAlign = ContentAlignment.MiddleLeft;
                ab.UseVisualStyleBackColor = false;
                ab.Text = e.Attribute("display").Value;
                this.Controls.Add(ab);
                ab.Click += new EventHandler(ab_Click);
                controls.Add(ab);

                row++;
            }
            Program.TimerStart(timer1_Tick);
            this.Focus();
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            Program.TimerStop();
            Program.ClearFields();
            this.GoHome("");
        }

        void ab_Click(object sender, EventArgs e)
        {
            Program.TimerStop();
            var ab = sender as Button;
            this.Swap(nextcontrol);
			if (nextcontrol is AttendListFamily)
				((AttendListFamily)nextcontrol).ShowFamily((int)ab.Tag);
			else if (nextcontrol is BuildingListFamily)
				((BuildingListFamily)nextcontrol).ShowFamily((int)ab.Tag);
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
			if (Program.baseform.textbox.Parent is AttendHome)
			{
				Program.home.textBox1.Text = name;
				this.Swap(Program.home.namesearch);
			}
			else if (Program.baseform.textbox.Parent is BuildingHome)
			{
				Program.home2.namesearch.textBox1.Text = name;
				this.Swap(Program.home2.namesearch);
			}
        }

        private void AddNewFamily_Click(object sender, EventArgs e)
        {
            Program.FamilyId = 0;
            Program.editing = false;
			if (Program.baseform.textbox.Parent is AttendHome)
				this.Swap(Program.home.first);
			else if (Program.baseform.textbox.Parent is BuildingHome)
				this.Swap(Program.home2.first);
        }

        private void ClearControls()
        {
            foreach (var c in controls)
            {
                this.Controls.Remove(c);
                c.Dispose();
            }
            controls.Clear();
            sucontrols.Clear();
            sucontrols.Add(bAddNewFamily);
            bAddNewFamily.BackColor = SystemColors.Control;
            bAddNewFamily.Enabled = false;
        }
        private void pgdn_Click(object sender, EventArgs e)
        {
            ShowResults(name, next.Value);
        }

        private void pgup_Click(object sender, EventArgs e)
        {
            ShowResults(name, prev.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.TimerReset();
            foreach (var c in sucontrols)
            {
                c.Enabled = true;
                c.BackColor = Color.Coral;
            }
        }
    }
}
