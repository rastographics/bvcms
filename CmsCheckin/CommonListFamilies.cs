using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CmsCheckin
{
    public partial class CommonListFamilies : UserControl
    {
		UserControl nextcontrol;
        public CommonListFamilies(UserControl next)
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
                            ShowFamilies(prev);
                        return true;
                    case Keys.PageDown:
                        if (pgdn.Visible)
                            ShowFamilies(next);
                        return true;
                    case Keys.Escape:
                        Program.TimerStop();
                        this.GoHome(string.Empty);
                        return true;
                    case Keys.S | Keys.Alt:
                        Program.TimerReset();
                        Program.CursorShow();
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private List<Control> controls = new List<Control>();
        private string phone;
        int? next, prev;

        private void ShowFamilies(int? page)
        {
            var x = this.GetDocument("Checkin2/Match/" + phone.GetDigits() 
                + Program.QueryString + "&page=" + page);
            ShowFamilies(x);
        }
        public void ShowFamilies(XDocument x)
        {
            ClearControls();
            this.Focus();

            phone = x.Root.Attribute("phone").Value;
            next = x.Root.Attribute("next").Value.ToInt2();
            prev = x.Root.Attribute("prev").Value.ToInt2();
            pgdn.Visible = next.HasValue;
            pgup.Visible = prev.HasValue;

            var font = new Font("Verdana", 14F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            var g = this.CreateGraphics();
            var row = 0;

            var labfont = new Font("Verdana", 15F, ((FontStyle)((FontStyle.Italic | FontStyle.Underline))), GraphicsUnit.Point, ((byte)(0)));

            foreach (var e in x.Descendants("family"))
            {

                var ab = new Button();
                var locked = bool.Parse(e.Attribute("waslocked").Value);
                ab.Enabled = !locked;
                ab.BackColor = SystemColors.ControlLight;
                ab.Font = labfont;
                ab.Location = new Point(230, 100 + (row * 50));
                ab.Tag = int.Parse(e.Attribute("id").Value);
                ab.Size = new Size(400, 45);
                ab.TextAlign = ContentAlignment.TopCenter;
                ab.UseVisualStyleBackColor = false;
                ab.Text = e.Attribute("name").Value;
                this.Controls.Add(ab);
                ab.Click += new EventHandler(ab_Click);
                controls.Add(ab);

                row++;
            }
            Program.TimerStart(timer1_Tick);
        }
        void timer1_Tick(object sender, EventArgs e)
        {
            Program.TimerStop();
            Program.ClearFields();
            this.GoHome("");
        }

        void ab_Click(object sender, EventArgs e)
        {
            var ab = sender as Button;
            this.Swap(nextcontrol);
			if (nextcontrol is AttendListFamily)
				((AttendListFamily)nextcontrol).ShowFamily((int)ab.Tag);
			else if (nextcontrol is BuildingListFamily)
				((BuildingListFamily)nextcontrol).ShowFamily((int)ab.Tag);
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            Program.TimerStop();
            this.GoHome(string.Empty);
        }
        private void ClearControls()
        {
            foreach (var c in controls)
            {
                this.Controls.Remove(c);
                c.Dispose();
            }
            controls.Clear();
        }

        private void pgup_Click(object sender, EventArgs e)
        {
            ShowFamilies(prev);
        }

        private void pgdn_Click(object sender, EventArgs e)
        {
            ShowFamilies(next);
        }
    }
}
