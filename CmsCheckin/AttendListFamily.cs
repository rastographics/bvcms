using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using CmsCheckin.Classes;

namespace CmsCheckin
{
    public partial class AttendListFamily : UserControl
    {
        private const int ExtraPixelsName = 15;
        private const string STR_CheckMark = "ü";
        public AttendListFamily()
        {
            InitializeComponent();
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
                    DoPrinting(null, null);
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

        List<AttendLabel> list;
        int page = 1;
        List<Control> controls = new List<Control>();
        List<Control> sucontrols = new List<Control>();
        XDocument xdoc;
        const string Verdana = "Verdana";
        Font pfont = new Font(Verdana, 14f, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

        public void ShowFamily(int fid)
        {
            page = 1;
            xdoc = this.GetDocument("Checkin2/Family/" + fid + Program.QueryString);
            ShowFamily(xdoc);
        }
        public void ShowFamily(XDocument x)
        {
            xdoc = x;
            this.Focus();

            Program.FamilyId = x.Root.Attribute("familyid").Value.ToInt();

            if (!Program.SecurityCode.HasValue())
                Program.SecurityCode = x.Root.Attribute("securitycode").Value;
            label3.Text = Program.SecurityCode;

            Program.MaxLabels = x.Root.Attribute("maxlabels").Value.ToInt();

            list = new List<AttendLabel>();
            if (x.Descendants("attendee").Count() == 0)
            {
                ClearControls();
                var lab = new Label();
                lab.Font = pfont;
                lab.Location = new Point(15, 200);
                lab.AutoSize = true;
                PrintAll.Visible = false;
                PrintAll1.Visible = false;
                PrintAll2.Visible = false;
                pgup.Visible = false;
                pgdn.Visible = false;
                lab.Text = "Not Found, try another phone number?";
                this.Controls.Add(lab);
                Return.Text = "Try again";
                controls.Add(lab);
                return;
            }

            foreach (var e in x.Descendants("attendee"))
            {
                var a = new AttendLabel();
                a.cinfo = new ClassInfo
                {
                    oid = e.Attribute("orgid").Value.ToInt(),
                    pid = e.Attribute("id").Value.ToInt(),
                    mv = e.Attribute("mv").Value,
                };
                a.name = e.Attribute("name").Value;
                a.first = e.Attribute("first").Value;
                a.last = e.Attribute("last").Value;
                a.dob = e.Attribute("dob").Value;
                a.church = e.Attribute("church").Value;

                a.preferredname = e.Attribute("preferredname").Value;
                a.goesby = e.Attribute("goesby").Value;
                a.email = e.Attribute("email").Value;
                a.addr = e.Attribute("addr").Value;
                a.zip = e.Attribute("zip").Value;
                a.home = e.Attribute("home").Value.FmtFone();
                a.cell = e.Attribute("cell").Value.FmtFone();
                a.gender = e.Attribute("gender").Value.ToInt();
                a.marital = e.Attribute("marital").Value.ToInt();
                a.emphone = e.Attribute("emphone").Value.FmtFone();
                a.emfriend = e.Attribute("emfriend").Value;
                a.allergies = e.Attribute("allergies").Value;
                a.grade = e.Attribute("grade").Value;
                a.activeother = e.Attribute("activeother").Value;
                a.parent = e.Attribute("parent").Value;

                if (e.Attribute("parents") != null)
                    a.parents = e.Attribute("parents").Value;

                a.org = e.Attribute("org").Value;
                a.orgname = e.Attribute("orgname").Value;
                a.custody = bool.Parse(e.Attribute("custody").Value);
                a.transport = bool.Parse(e.Attribute("transport").Value);
                a.location = e.Attribute("loc").Value;
                a.leader = e.Attribute("leader").Value;
                a.NumLabels = int.Parse(e.Attribute("numlabels").Value);
                a.Row = list.Count;
                a.CheckedIn = bool.Parse(e.Attribute("checkedin").Value);
                a.HasPicture = bool.Parse(e.Attribute("haspicture").Value);
                a.RequiresSecurityLabel = bool.Parse(e.Attribute("requiressecuritylabel").Value);
                a.leadtime = double.Parse(e.Attribute("leadtime").Value);

                DateTime dt;
                if (DateTime.TryParse(e.Attribute("hour").Value, out dt))
                    a.cinfo.hour = dt;
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
            Return.Text = "Print Labels, Return";

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

            while (true)
            {
                twidab = widname = widorg = twidlb = maxheight = 0;
                font = new Font(Verdana, points, FontStyle.Regular,
                     GraphicsUnit.Point, ((byte)(0)));
                labfont = new Font(Verdana, points,
                     ((FontStyle)((FontStyle.Italic | FontStyle.Underline))),
                     GraphicsUnit.Point, ((byte)(0)));
                maxheight = 0;
                foreach (var c in list)
                {
                    var size = g.MeasureString(Present, labfont);
                    twidab = Math.Max(twidab, (int)Math.Ceiling(size.Width));
                    twidab = Math.Max(twidab, bwid);

                    size = g.MeasureString(c.name, font);
                    widname = Math.Max(widname, (int)Math.Ceiling(size.Width) + ExtraPixelsName);

                    size = g.MeasureString(Labels, labfont);
                    twidlb = Math.Max(twidlb, (int)Math.Ceiling(size.Width));
                    twidlb = Math.Max(twidlb, mwid);

                    size = g.MeasureString(String.Format("{0:h:mm tt} {1}", c.cinfo.hour, c.org), font);
                    widorg = Math.Max(widorg, (int)Math.Ceiling(size.Width));

                    size = g.MeasureString("|", labfont);
                    maxheight = Math.Max(maxheight, (int)Math.Ceiling(size.Height));
                }

                totalwid = sep + twidab + sep + widname + sep + widorg
                                          + sep + twidlb + sep;
                if (totalwid > 1024)
                {
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
            head.Text = "Meeting";
            this.Controls.Add(head);
            controls.Add(head);

            for (var r = srow; r < erow; r++)
            {
                var c = list[r];
                if (c.cinfo.mv == "V")
                    c.cinfo.mv = "G";
                if (classlist.Count > 0)
                {
                    var q = from cl in classlist
                            where cl.oid == c.cinfo.oid && cl.pid == c.cinfo.pid
                            select cl;
                    if (q.Count() > 0 && c.CheckedIn)
                        c.WasChecked = true;
                }

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

                if (c.cinfo.oid == 0 || Program.TooEarlyOrLate(c.leadtime))
                {
                    ab.Enabled = false;
                    ab.BackColor = SystemColors.Control;
                    ab.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
                }

                ab.ForeColor = Color.White;
                ab.Font = new Font("Wingdings", 24, FontStyle.Bold,
                     GraphicsUnit.Point, ((byte)(2)));
                ab.Name = "attend" + c.Row;
                ab.TextAlign = ContentAlignment.TopCenter;
                ab.UseVisualStyleBackColor = false;
                this.Controls.Add(ab);
                ab.KeyDown += new KeyEventHandler(AttendButton_KeyDown);
                ab.Click += new EventHandler(Attend_Click);
                ab.Text = c.CheckedIn ? STR_CheckMark : String.Empty;
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
                if (c.cinfo.oid != 0)
                    if (c.cinfo.mv == "M")
                        nam.ForeColor = Color.Blue;
                    else
                        nam.ForeColor = Color.DarkGreen;
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
                org.Text = String.Format("{0:h:mm tt} {1}", c.cinfo.hour, c.org);
                org.TextAlign = ContentAlignment.MiddleLeft;
                org.Name = "org" + c.Row;
                if (c.cinfo.oid != 0)
                    if (c.cinfo.mv == "M")
                        org.ForeColor = Color.Blue;
                    else
                        org.ForeColor = Color.DarkGreen;
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
            if (e.KeyValue == 27)
            {
                Program.TimerStop();
                Util.UnLockFamily();
                this.GoHome(string.Empty);
            }
        }

        void PrintLabel_Click(object sender, EventArgs e)
        {
            Program.TimerReset();
            var c = list[(int)menu.Tag];
            var li = new LabelInfo
            {
                allergies = c.allergies,
                pid = c.cinfo.pid,
                mv = c.cinfo.mv,
                n = c.NumLabels,
                first = c.first,
                last = c.last,
                location = c.location,
                hour = c.cinfo.hour,
                org = c.org,
                custody = c.custody,
                transport = c.transport,
                requiressecuritylabel = c.RequiresSecurityLabel,
                securitycode = Program.SecurityCode,
                parents = c.parents
            };

            //			IEnumerable<LabelInfo> liList = new[] { li };
            //			PrinterHelper.doPrinting(liList, true);

            if (!Program.settings.useOldDatamaxFormat)
            {
                IEnumerable<LabelInfo> liList = new[] { li };
                PrinterHelper.doPrinting(liList, true);
            }
            else
            {
                int iLabelSize = PrinterHelper.getPageHeight(Program.settings.printer);

                using (var ms = new MemoryStream())
                {
                    if (iLabelSize >= 170 && iLabelSize <= 230)
                        ms.LabelKiosk2(li);
                    else
                        ms.LabelKiosk(li);
                    PrintRawHelper.SendDocToPrinter(Program.settings.printer, ms);
                }
            }

            RemoveMenu();
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
            if (c.cinfo.oid == 0)
                return;
            Cursor.Current = Cursors.WaitCursor;
            Program.CursorShow();
            if (ab.Text == String.Empty)
            {
                ab.Text = STR_CheckMark;
                c.CheckedIn = true;
                c.WasChecked = true;
            }
            else
            {
                ab.Text = String.Empty;
                c.CheckedIn = false;
                c.WasChecked = false;
            }
            var info = new Util.ClassCheckedInfo { c = c.cinfo, ischecked = c.CheckedIn };
            c.lastpress = DateTime.Now;
            ComputeLabels();
            var bw = new BackgroundWorker();
            bw.DoWork += CheckUnCheckDoWork;
            bw.RunWorkerCompleted += CheckUncheckCompleted;
            bw.RunWorkerAsync(info);
        }

        void ShowPic_Click(object sender, EventArgs e)
        {
            Program.TimerReset();
            var eb = sender as Button;
            var ab = this.Controls[this.Controls.IndexOfKey("attend" + eb.Tag.ToString())] as Button;
            var c = list[(int)ab.Tag];
            Program.PeopleId = c.cinfo.pid;
            var f = new CommonPicture();
            f.ShowDialog();
        }

        public List<ClassInfo> classlist = new List<ClassInfo>();
        private Label mask;
        private AttendMenu menu;

        void Menu_Click(object sender, EventArgs e)
        {
            Program.TimerReset();
            var MenuButton = sender as Button;
            menu = new AttendMenu();
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
            menu.VisitClass += Visit_Click;
            var c = list[(int)menu.Tag];
            nam.Enabled = false;
            menu.EditRecord += EditRecord_Click;
            menu.PrintLabel += PrintLabel_Click;
            menu.AddFamily += AddToFamily_Click;
            menu.JoinClass += Join_Click;
            if (c.cinfo.oid != 0)
            {
                menu.DropJoin.Visible = true;
                if (c.cinfo.mv == "M")
                {
                    menu.DropJoinClass += DropThis_Click;
                    menu.DropJoin.Text = "Drop This Class";
                }
                else
                {
                    menu.DropJoinClass += JoinThis_Click;
                    menu.DropJoin.Text = "Join This Class";
                }
            }
            else
                menu.DropJoin.Visible = false;
            menu.CancelMenu += new EventHandler(CancelMenu_Click);
            menu.Show();
            menu.BringToFront();
        }

        void Join_Click(object sender, EventArgs e)
        {
            var c = list[(int)menu.Tag];
            SaveClasses();
            RemoveMenu();
            this.Swap(Program.attendHome.classes);
            Program.JoiningNotAttending = true;
            Program.attendHome.classes.ShowResults(c.cinfo.pid);
        }
        void DropThis_Click(object sender, EventArgs e)
        {
            var c = list[(int)menu.Tag];
            var org = this.Controls[this.Controls.IndexOfKey("org" + menu.Tag.ToString())] as Label;
            SaveClasses();

            Util.JoinUnJoin(c.cinfo, false);
            RemoveMenu();

            if (Program.baseform.textbox.Parent is AttendHome)
                Program.attendHome.family.ShowFamily(Program.FamilyId);
            else if (Program.baseform.textbox.Parent is BuildingHome)
                Program.buildingHome.family.ShowFamily(Program.FamilyId);
        }
        void JoinThis_Click(object sender, EventArgs e)
        {
            var c = list[(int)menu.Tag];
            var org = this.Controls[this.Controls.IndexOfKey("org" + menu.Tag.ToString())] as Label;
            SaveClasses();

            Util.JoinUnJoin(c.cinfo, true);
            RemoveMenu();
            if (Program.baseform.textbox.Parent is AttendHome)
                Program.attendHome.family.ShowFamily(Program.FamilyId);
            else if (Program.baseform.textbox.Parent is BuildingHome)
                Program.buildingHome.family.ShowFamily(Program.FamilyId);
        }

        void CancelMenu_Click(object sender, EventArgs e)
        {
            RemoveMenu();
        }
        void Visit_Click(object sender, EventArgs e)
        {
            var c = list[(int)menu.Tag];
            SaveClasses();
            RemoveMenu();
            this.Swap(Program.attendHome.classes);
            Program.attendHome.classes.ShowResults(c.cinfo.pid);
        }
        private void SaveClasses()
        {
            classlist = new List<ClassInfo>();
            for (var r = 0; r < list.Count; r++)
            {
                var cc = list[r];
                if (cc.WasChecked)
                {
                    cc.cinfo.nlabels = cc.NumLabels;
                    classlist.Add(cc.cinfo);
                }
            }
        }
        private void ComputeLabels()
        {
            int canprint;
            int willprint;
            var can = list.Where(c => c.CheckedIn && c.NumLabels > 0);
            var will = can.Where(c => c.WasChecked);
            canprint = can.Count();
            willprint = will.Count();
            var show = canprint > willprint;
            PrintAll.Visible = show;
            PrintAll1.Visible = show;
            PrintAll2.Visible = show;
            if (PrintAll.Text.HasValue() || willprint > 0)
                Return.Text = "Print Labels, Return";
            else
                Return.Text = "Return";
        }

        void EditRecord_Click(object sender, EventArgs e)
        {
            var c = list[(int)menu.Tag];

            var home = Program.attendHome;
            Program.PeopleId = c.cinfo.pid;
            home.SetFields(c.last, c.email, c.addr, c.zip, c.home, c.parent, c.emfriend, c.emphone, c.activeother, c.church);
            home.first.textBox1.Text = c.first;
            home.goesby.textBox1.Text = c.goesby;
            home.dob.textBox1.Text = c.dob;
            home.cellphone.textBox1.Text = c.cell.FmtFone();
            home.gendermarital.Marital = c.marital;
            home.gendermarital.Gender = c.gender;
            if (Program.settings.askChurch)
                home.gendermarital.ActiveOther.CheckState =
                     c.activeother == bool.TrueString ? CheckState.Checked :
                     c.activeother == bool.FalseString ? CheckState.Unchecked : CheckState.Indeterminate;
            if (Program.settings.askGrade)
                home.grade.textBox1.Text = c.grade;
            home.allergy.textBox1.Text = c.allergies;
            if (Program.settings.askFriend)
            {
                home.parent.textBox1.Text = c.parent;
                home.emfriend.textBox1.Text = c.emfriend;
                home.emphone.textBox1.Text = c.emphone.FmtFone();
            }
            Util.UnLockFamily();
            SaveClasses();

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

        PleaseWait PleaseWaitForm = null;

        private void DoPrinting(object sender, DoWorkEventArgs e)
        {
            if (list == null)
                return;

            var qlist = list.Where(c => c.CheckedIn && c.NumLabels > 0);

            if (!PrintAll.Text.HasValue()) qlist = qlist.Where(c => c.WasChecked);

            var q = from c in qlist
                    select new LabelInfo
                    {
                        allergies = c.allergies,
                        pid = c.cinfo.pid,
                        mv = c.cinfo.mv,
                        n = c.NumLabels,
                        first = c.preferredname,
                        last = c.last,
                        location = c.location,
                        hour = c.cinfo.hour,
                        org = c.orgname,
                        custody = c.custody,
                        transport = c.transport,
                        requiressecuritylabel = c.RequiresSecurityLabel,
                        securitycode = Program.SecurityCode,
                        dob = (c.dob != null && c.dob.Length > 0 ? DateTime.Parse(c.dob) : DateTime.Now),
                        parents = c.parents
                    };

            Util.UnLockFamily();

            if (Program.settings.printMode == "Print To Server")
            {
                PrintServerLabels(q);
                return;
            }
            else if (Program.settings.printMode == "Cloud Printing")
            {
                return;
            }

            if (q.Count() > 0)
            {
                if (!Program.settings.useOldDatamaxFormat)
                {
                    if (!PrinterHelper.doPrinting(q))
                    {
                        MessageBox.Show("There were no labels to print, please adjust your printer page size.  If the page size is correct, please contact support for assistance.", "Printer Page Size Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    DoPrinting doprint = new DoPrinting();
                    int iLabelSize = PrinterHelper.getPageHeight(Program.settings.printer);

                    using (var ms = new MemoryStream())
                    {
                        if (iLabelSize >= 151 && iLabelSize <= 250)
                            doprint.PrintLabels2(ms, q);
                        else
                            doprint.PrintLabels(ms, q);

                        doprint.FinishUp(ms);
                    }
                }
            }
        }

        private void PrintingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (PleaseWaitForm != null)
            {
                PleaseWaitForm.Hide();
                PleaseWaitForm.Dispose();
                PleaseWaitForm = null;
            }

            Program.FamilyId = 0;

            classlist = new List<ClassInfo>();

            PrintAll.Text = string.Empty;

            this.GoHome(string.Empty);
        }
        private void PrintServerLabels(IEnumerable<LabelInfo> q)
        {
            if (list == null)
                return;

            Util.UploadPrintJob(q);
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
        }

        private void MagicButton_Click(object sender, EventArgs e)
        {
            if (!Program.CheckAdminPIN()) return;

            Program.TimerStop();
            if (list.Count == 0)
            {
                if (Program.baseform.textbox.Parent is AttendHome)
                    this.Swap(Program.attendHome.namesearch);
                else if (Program.baseform.textbox.Parent is BuildingHome)
                    this.Swap(Program.buildingHome.namesearch);
                return;
            }
            foreach (var c in sucontrols)
            {
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
            SaveClasses();
            Util.UnLockFamily();
            RemoveMenu();
            Program.attendHome.SetFields(c.last, c.email, c.addr, c.zip, c.home, c.parent, c.emfriend, c.emphone, c.activeother, c.church);
            this.Swap(Program.attendHome.first);
        }

        private void CheckUnCheckDoWork(object sender, DoWorkEventArgs e)
        {
            var info = e.Argument as Util.ClassCheckedInfo;
            Util.AttendUnAttend(info);

        }
        private void CheckUncheckCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor.Current = Cursors.Default;
            Program.CursorHide();
        }

        private void Return_Click(object sender, EventArgs e)
        {
            Program.TimerStop();

            if (Return.Text.Contains("Try Again"))
            {
                this.GoHome(string.Empty);
                return;
            }
            PleaseWaitForm = new PleaseWait();
            PleaseWaitForm.Show();

            var bw = new BackgroundWorker();
            bw.DoWork += DoPrinting;
            bw.RunWorkerCompleted += PrintingCompleted;
            bw.RunWorkerAsync();
        }
        private void PrintAll_Click(object sender, EventArgs e)
        {
            PrintAll.Text = PrintAll.Text.HasValue() ? String.Empty : STR_CheckMark;
            ComputeLabels();
        }
    }
    public class AttendLabel
    {
        public DateTime? lastpress { get; set; }
        public ClassInfo cinfo { get; set; }
        public string name { get; set; }
        public string first { get; set; }
        public string last { get; set; }
        public string dob { get; set; }
        public string church { get; set; }

        public string preferredname { get; set; }
        public string goesby { get; set; }
        public string email { get; set; }
        public string addr { get; set; }
        public string zip { get; set; }
        public string home { get; set; }
        public string cell { get; set; }
        public string emfriend { get; set; }
        public string emphone { get; set; }
        public string allergies { get; set; }
        public string grade { get; set; }
        public string activeother { get; set; }
        public string parent { get; set; }
        public string parents { get; set; }
        public bool custody { get; set; }
        public bool transport { get; set; }

        public int gender { get; set; }
        public int marital { get; set; }

        public string orgname { get; set; }
        public string leader { get; set; }
        public string org { get; set; }
        public string location { get; set; }
        public int NumLabels { get; set; }
        public int Row { get; set; }
        public bool CheckedIn { get; set; }
        public bool WasChecked { get; set; }
        public bool HasPicture { get; set; }
        public bool RequiresSecurityLabel { get; set; }
        public double leadtime { get; set; }
    }

    // all of these come from the attribtues on the attendee element
    // attributes have the same name unless noted otherwise
    [Serializable]
    public class LabelInfo
    {
        public int n { get; set; } // numlabels attribute
        public DateTime dob { get; set; } // dob
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
        public string securitycode { get; set; }
        public string parents { get; set; }

        public int age
        {
            get
            {
                DateTime now = DateTime.Now;
                int age = now.Year - dob.Year;
                if (now < dob.AddYears(age)) age--;
                return age;
            }
        }

        public string birthday
        {
            get
            {
                return dob.ToString("d");
            }
        }

        public string date
        {
            get
            {
                if (hour != null) return hour.Value.ToString("d");
                else return "";
            }
        }

        public string time
        {
            get
            {
                if (hour != null) return hour.Value.ToString("t");
                else return "";
            }
        }

        public string extra
        {
            get
            {
                if (custody || transport || allergies.Length > 0) return ("Extra - " + (allergies.Length > 0 ? "A|" : "") + (custody ? "C|" : "") + (transport ? "T" : "")).TrimEnd(new char[] { '|' });
                else return "Extra";
            }
        }

        public string guest
        {
            get
            {
                if (custody || transport || allergies.Length > 0) return ("Guest - " + (allergies.Length > 0 ? "A|" : "") + (custody ? "C|" : "") + (transport ? "T" : "")).TrimEnd(new char[] { '|' });
                else return "Guest";
            }
        }

        public string guestoption
        {
            get
            {
                if (mv == "M") return "";
                else return "Guest";
            }
        }

        public string info
        {
            get
            {
                return ((allergies.Length > 0 ? "A|" : "") + (custody ? "C|" : "") + (transport ? "T" : "")).TrimEnd(new char[] { '|' });
            }
        }
    }
    [Serializable]
    public class PrintJob
    {
        // securitycode comes from the attribute on the root element (Attendees)
        public string securitycode { get; set; }
        // the following is a list of each person/class that was checked present
        public List<LabelInfo> list { get; set; }
    }
    [Serializable]
    public class PrintJobs
    {
        public List<PrintJob> jobs { get; set; }
    }
}
