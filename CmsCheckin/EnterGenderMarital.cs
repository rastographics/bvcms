using System;
using System.Text;
using System.Windows.Forms;

namespace CmsCheckin
{
	public partial class EnterGenderMarital : UserControl
	{
		public EnterGenderMarital()
		{
			InitializeComponent();
		}

		public void ShowScreen()
		{
			var home = Program.attendHome;
			first.Text = home.first.textBox1.Text;
			goesby.Text = home.goesby.textBox1.Text;
			last.Text = home.last.textBox1.Text;
			email.Text = home.email.textBox1.Text;
			dob.Text = home.dob.textBox1.Text;
			cellphone.Text = home.cellphone.textBox1.Text;
			homephone.Text = home.homephone.textBox1.Text;
			addr.Text = home.addr.textBox1.Text;
			zip.Text = home.zip.textBox1.Text;
			allergies.Text = home.allergy.textBox1.Text;

			if (Program.settings.askGrade)
				grade.Text = home.grade.textBox1.Text;

			if (Program.settings.askFriend)
			{
				ParentName.Text = home.parent.textBox1.Text;
				EmFriend.Text = home.emfriend.textBox1.Text;
				EmPhone.Text = home.emphone.textBox1.Text;
			}

			if (Program.settings.askChurchName)
				churchname.Text = home.church.textBox1.Text;

			ActiveOther.Visible = Program.settings.askChurch;
			churchname.Visible = Program.settings.askChurchName;
			churchnameLab.Visible = Program.settings.askChurchName;
			emfriendlab.Visible = Program.settings.askFriend;
			emphonelab.Visible = Program.settings.askFriend;
			emergencylab.Visible = Program.settings.askFriend;
			EmPhone.Visible = Program.settings.askFriend;
			EmFriend.Visible = Program.settings.askFriend;
			grade.Visible = Program.settings.askGrade;
			gradelab.Visible = Program.settings.askGrade;

			if (Marital == 0 && dob.Text.Age().ToInt() <= 16)
				single.Checked = true;

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
				Program.attendHome.homephone.textBox1.Text = cellphone.Text;

			if (Program.editing)
				this.EditPerson(Program.PeopleId, first.Text, last.Text, goesby.Text, dob.Text, email.Text, addr.Text, zip.Text, cellphone.Text, homephone.Text, allergies.Text, grade.Text, ParentName.Text, EmFriend.Text, EmPhone.Text, churchname.Text, ActiveOther.CheckState, marital, gender);
			else
			{
				if (Program.FamilyId == 0 && !homephone.Text.HasValue() && cellphone.Text.HasValue())
					homephone.Text = cellphone.Text;
				this.AddPerson(first.Text, last.Text, goesby.Text, dob.Text, email.Text, addr.Text, zip.Text, cellphone.Text, homephone.Text, allergies.Text, grade.Text, ParentName.Text, EmFriend.Text, EmPhone.Text, churchname.Text, ActiveOther.CheckState, marital, gender);
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
			if (Program.editing)
			{
				this.Swap(Program.attendHome.family);
				Program.attendHome.family.ShowFamily(Program.FamilyId);
			}
			else
			{
				this.Swap(Program.attendHome.classes);
				Program.attendHome.classes.ShowResults(Program.PeopleId);
			}
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
			this.Swap(Program.attendHome.allergy);
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
			if (Program.settings.askChurch && ActiveOther.CheckState == CheckState.Indeterminate)
				sb.AppendLine("Active Other Church needed");
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

		private void button2_Click(object sender, EventArgs e)
		{
			var f = new CommonTakePicture();
			f.ShowDialog();
		}
	}
}
