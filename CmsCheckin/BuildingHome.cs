using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CmsCheckin
{
    public partial class BuildingHome : UserControl
    {
        public BuildingHome()
        {
			InitializeComponent();
			GuestOf.Visible = false;
			removeguestof.Visible = false;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                switch (keyData)
                {
                    case Keys.S | Keys.Alt:
                        this.Swap(namesearch);
                        break;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        void buttonclick(object sender, EventArgs e)
        {
            var b = sender as Button;
            var d = b.Name[6];
            KeyStroke(d);
        }

        private void PhoneNumber2_Load(object sender, EventArgs e)
        {
            button1.Click += buttonclick;
            button2.Click += buttonclick;
            button3.Click += buttonclick;
            button4.Click += buttonclick;
            button5.Click += buttonclick;
            button6.Click += buttonclick;
            button7.Click += buttonclick;
            button8.Click += buttonclick;
            button9.Click += buttonclick;
            button0.Click += buttonclick;
            button10.Click += button10_Click;
            button11.Click += button10_Click;
            button12.Click += button10_Click;
            textBox1.Focus();
            textBox1.Select(textBox1.Text.Length, 0);
        }

        private Button lastbutton;
        private string lastnumber;
        void button10_Click(object sender, EventArgs e)
        {
            var b = sender as Button;
            if (button10 == b)
                lastbutton = b;
            else if (lastbutton == button10 && button11 == b)
                lastbutton = b;
            else if (lastbutton == button11 && button12 == b)
            {
                textBox1.Text = lastnumber;
                lastbutton = b;
            }
        }

        private void buttongo_Click(object sender, EventArgs e)
        {
            lastbutton = null;
            var d = textBox1.Text.GetDigits().Length;
            lastnumber = textBox1.Text;
            Go();
        }

        private void buttonbs_Click(object sender, EventArgs e)
        {
            var b = sender as Button;
            BackSpace();
        }

		/*
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
                BackSpace();
            else if(e.KeyChar >= '0' && e.KeyChar <= '9')
                KeyStroke(e.KeyChar);
            else if(e.KeyChar == '\r')
            {
                lastnumber = textBox1.Text;
                Go();
            }
            e.Handled = true;
        }
		*/

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				e.Handled = true;
				lastnumber = textBox1.Text;
				Go();
			}
		}

        private void KeyStroke(char d)
        {
            lastbutton = null;
			var t = textBox1.Text.GetDigits();
			t += d;
			textBox1.Text = t;
			textBox1.Focus();
			textBox1.Select(textBox1.Text.Length, 0);
		}
        private void BackSpace()
        {
            lastbutton = null;
			if (textBox1.Text.Length == 0)
				return;
			textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
            textBox1.Focus();
            textBox1.Select(textBox1.Text.Length, 0);
        }
        private PleaseWait PleaseWaitForm;
        private void Go()
        {
            if (textBox1.Text == "010")
            {
                Application.Exit();
                return;
            }
            if (textBox1.Text == "411")
            {
                if (Program.CheckAdminPIN())
                    this.Swap(namesearch);
            }
            else
            {
                PleaseWaitForm = new PleaseWait();
                PleaseWaitForm.Show();
                buttongo.Enabled = false;
                var bw = new BackgroundWorker();
                bw.DoWork += backgroundWorker1_DoWork;
                bw.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
                bw.RunWorkerAsync(textBox1.Text);
            }
            textBox1.Text = string.Empty;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var ph = e.Argument as string;
			var lb = Program.attendant.NameDisplay;
			lb.SetPropertyThreadSafe(() => lb.Text, "finding " + ph);

            // Get Method
            //var url = "Checkin2/Find/" + ph + Program.QueryString + "&page=1&building=" + Program.Building + "&querybit=" + Program.BuildingInfo.querybit;
            //var x = this.GetDocument(url);

            var url = "Checkin2/Find/" + ph;

            var post = new NameValueCollection();
            post.Add("page", "1");
            post.Add("building", Program.settings.building);
            post.Add("querybit", Program.BuildingInfo.querybit);

            var x = this.PostDocument(url, post);

            e.Result = x;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PleaseWaitForm.Hide();
            PleaseWaitForm.Dispose();
            PleaseWaitForm = null;
            buttongo.Enabled = true;
            var x = e.Result as XDocument;
            if (x.Document.Root.Name == "Families")
            {
                this.Swap(families);
                families.ShowFamilies(x);
            }
            else
            {
				this.Swap(family);
				family.ShowFamily(x);
            }
        }

        private void MagicButton_Click(object sender, EventArgs e)
        {
            if( Program.CheckAdminPIN() )
                this.Swap(namesearch);
        }

        void first_GoBack(object sender, EventArgs e)
        {
            ClearFields();
            if (Program.editing && Program.FamilyId > 0)
            {
                first.Swap(family);
                family.ShowFamily(Program.FamilyId);
            }
            else
                first.GoHome(string.Empty);
        }
        void namesearch_GoNext(object sender, EventArgs e)
        {
            namesearch.Swap(names);
            string name = namesearch.textBox1.Text;
            namesearch.textBox1.Text = String.Empty;
            names.ShowResults(name, 1);
        }
        void namesearch_GoBack(object sender, EventArgs e)
        {
            namesearch.textBox1.Text = String.Empty;
            namesearch.GoHome(string.Empty);
        }
		private void Home2_Load(object sender, EventArgs e)
		{
			button1.Click += buttonclick;
			button2.Click += buttonclick;
			button3.Click += buttonclick;
			button4.Click += buttonclick;
			button5.Click += buttonclick;
			button6.Click += buttonclick;
			button7.Click += buttonclick;
			button8.Click += buttonclick;
			button9.Click += buttonclick;
			button0.Click += buttonclick;
			button10.Click += button10_Click;
			button11.Click += button10_Click;
			button12.Click += button10_Click;
			textBox1.Focus();
			textBox1.Select(textBox1.Text.Length, 0);

			Program.CursorHide();

			var form = Parent as BaseForm;

			this.Visible = true;
			this.textBox1.Focus();

			namesearch = new EnterText("Name Search", true);
			form.ControlsAdd(namesearch);

			family = new BuildingListFamily();
			form.ControlsAdd(family);

			families = new CommonListFamilies(family);
			form.ControlsAdd(families);

			names = new CommonListNames(family);
			form.ControlsAdd(names);

			first = new EnterText("First Name", true);
			form.ControlsAdd(first);

			goesby = new EnterText("Goes By", true);
			form.ControlsAdd(goesby);

			last = new EnterText("Last Name", true);
			form.ControlsAdd(last);

			email = new EnterText("Email");
			form.ControlsAdd(email);

			addr = new EnterText("Address", true);
			form.ControlsAdd(addr);

			zip = new EnterText("Zip");
			form.ControlsAdd(zip);

			dob = new EnterDate("Birthday");
			form.ControlsAdd(dob);

			cellphone = new EnterPhone("Cell Phone");
			form.ControlsAdd(cellphone);

			homephone = new EnterPhone("Home Phone");
			form.ControlsAdd(homephone);

			gendermarital = new EnterGenderMarital2();
			form.ControlsAdd(gendermarital);

			namesearch.GoBack += namesearch_GoBack;
			namesearch.GoNext += namesearch_GoNext;

			first.GoBack += first_GoBack;
			first.SetBackNext(null, goesby);
			goesby.SetBackNext(first, last);
			last.SetBackNext(goesby, email);
			email.SetBackNext(last, addr);
			addr.SetBackNext(email, zip);
			zip.SetBackNext(addr, dob);

			dob.SetBackNext(zip, cellphone);
			cellphone.SetBackNext(dob, homephone);

			homephone.SetBackNext(cellphone, null);
			homephone.GoNext += homephone_GoNext;

		}

		void homephone_GoNext(object sender, EventArgs e)
		{
            homephone.Swap(gendermarital);
            gendermarital.ShowScreen();
		}
		public CommonListFamilies families;
		public BuildingListFamily family;
		public EnterText namesearch;
		public CommonListNames names;

		public EnterText first;
		public EnterText goesby;
		public EnterText last;
		public EnterText email;
		public EnterText addr;
		public EnterText zip;
		public EnterDate dob;
		public EnterPhone cellphone;
		public EnterPhone homephone;
		public EnterGenderMarital2 gendermarital;

		public void ClearFields()
		{
			Program.SecurityCode = null;
			first.textBox1.Text = null;
			goesby.textBox1.Text = null;
			last.textBox1.Text = null;
			email.textBox1.Text = null;
			addr.textBox1.Text = null;
			zip.textBox1.Text = null;
			dob.textBox1.Text = null;
			cellphone.textBox1.Text = null;
			homephone.textBox1.Text = null;
			gendermarital.Gender = 0;
			gendermarital.Marital = 0;
		}
		public void SetFields(string Last, string Email, string Addr, string Zip, string Home)
		{
			last.textBox1.Text = Last;
			email.textBox1.Text = Email;
			addr.textBox1.Text = Addr;
			zip.textBox1.Text = Zip;
			homephone.textBox1.Text = Home;
		}

		private void removeguestof_Click(object sender, EventArgs e)
		{
			GuestOf.Visible = false;
			removeguestof.Visible = false;
			Program.addguests.Dispose();
			Program.addguests = null;
		}

		private void textBox1_Enter(object sender, EventArgs e)
		{
			if (Program.addguests != null)
			{
				GuestOf.Text = "Guest Of:\n" + Program.GuestOf().name;
				GuestOf.Visible = true;
				removeguestof.Visible = true;
			}
			else
			{
				GuestOf.Visible = false;
				removeguestof.Visible = false;
			}
		}
    }
}
