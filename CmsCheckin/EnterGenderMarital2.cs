using System;
using System.Text;
using System.Windows.Forms;

namespace CmsCheckin
{
    public partial class EnterGenderMarital2 : UserControl
    {
        public EnterGenderMarital2()
        {
            InitializeComponent();
        }
        public void ShowScreen()
        {
			var home = Program.buildingHome;
            first.Text = home.first.textBox1.Text;
            goesby.Text = home.goesby.textBox1.Text;
            last.Text = home.last.textBox1.Text;
            email.Text = home.email.textBox1.Text;
            dob.Text = home.dob.textBox1.Text;
            cellphone.Text = home.cellphone.textBox1.Text;
            homephone.Text = home.homephone.textBox1.Text;
            addr.Text = home.addr.textBox1.Text;
            zip.Text = home.zip.textBox1.Text;

//            if (Marital == 0 && dob.Text.Age().ToInt() < 18)
//                single.Checked = true;
            Program.TimerStart(timer1_Tick);

        }

        void timer1_Tick(object sender, EventArgs e)
        {
            Program.TimerStop();
            Util.UnLockFamily();
            Program.ClearFields();
            this.GoHome("");
        }

        private void buttongo_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
                return;
            Program.TimerStop();
            if (Marital == 0 || Gender == 0)
                return;
            AddPersonGoHome();
        }
        private void AddPersonGoHome()
        {
            var gender = Gender;
            var marital = Marital;
            if (cellphone.Text.HasValue() && !homephone.Text.HasValue())
                Program.buildingHome.homephone.textBox1.Text = cellphone.Text;
            if (Program.editing)
                this.EditPerson(Program.PeopleId, first.Text, last.Text, goesby.Text, dob.Text, email.Text, addr.Text, zip.Text, cellphone.Text, homephone.Text, marital, gender);
            else
            {
                if (Program.FamilyId == 0 && !homephone.Text.HasValue() && cellphone.Text.HasValue())
                    homephone.Text = cellphone.Text;
                this.AddPerson(first.Text, last.Text, goesby.Text, dob.Text, email.Text, addr.Text, zip.Text, cellphone.Text, homephone.Text, null, null, null, null, null, null, CheckState.Indeterminate, marital, gender);
            }
            Util.UnLockFamily();

            string ph;
            if (!string.IsNullOrEmpty(homephone.Text))
                ph = homephone.Text;
            else
                if (!string.IsNullOrEmpty(cellphone.Text))
                    ph = cellphone.Text;
                else
                    ph = "";
			Program.ClearFields();
			this.Swap(Program.buildingHome.family);
			Program.buildingHome.family.ShowFamily(Program.FamilyId);
        }

        public int Gender
        {
            get { return Male.Checked ? 1 : Female.Checked ? 2 : 0; }
            set
            {
                switch (value)
                {
                    case 1:
                        Male.Checked = true;
                        break;
                    case 2:
                        Female.Checked = true;
                        break;
                    default:
                        Male.Checked = false;
                        Female.Checked = false;
                        break;
                }
            }
        }
        public int Marital
        {
            get
            {
                var marital = 0;
                if (married.Checked)
                    marital = 20;
                else if (single.Checked)
                    marital = 10;
                else if (separated.Checked)
                    marital = 30;
                else if (divorced.Checked)
                    marital = 40;
                else if (widowed.Checked)
                    marital = 50;
                else
                    marital = 0;
                return marital;
            }
            set
            {
                switch (value)
                {
                    case 10:
                        single.Checked = true;
                        break;
                    case 20:
                        married.Checked = true;
                        break;
                    case 30:
                        separated.Checked = true;
                        break;
                    case 40:
                        divorced.Checked = true;
                        break;
                    case 50:
                        widowed.Checked = true;
                        break;
                    default:
                        single.Checked = false;
                        married.Checked = false;
                        separated.Checked = false;
                        divorced.Checked = false;
                        widowed.Checked = false;
                        break;
                }
            }
        }
        private void GoBack_Click(object sender, EventArgs e)
        {
            this.Swap(Program.buildingHome.homephone);
        }

        private bool ValidateFields()
        {
            var sb = new StringBuilder();
            if (Marital == 0)
                sb.AppendLine("marital status needed");
            if (Gender == 0)
                sb.AppendLine("gender needed");
            if (!first.Text.HasValue())
                sb.AppendLine("first name needed");
            if (!last.Text.HasValue())
                sb.AppendLine("last name needed");
            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Try again", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
			Program.ClearFields();
            this.GoHome(string.Empty);
        }
    }
}
