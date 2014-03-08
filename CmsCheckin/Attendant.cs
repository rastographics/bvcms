using System;
using System.Windows.Forms;

namespace CmsCheckin
{
	public partial class Attendant : Form
	{
		public Attendant()
		{
			InitializeComponent();
		}

		private void ShowCheckin_Click(object sender, EventArgs e)
		{
			if (Program.baseform == null)
				Program.baseform = new BaseForm(new Home2());
			if (Program.baseform.Visible)
			{
				Program.baseform.Hide();
				ShowCheckin.Text = "Show Checkin";
			}
			else
			{
				Program.baseform.Show();
				ShowCheckin.Text = "Hide Checkin";
			}
		}
		public void AddHistory(PersonInfo p)
		{
			if (InvokeRequired) 
			{ 
				Invoke(new Action<PersonInfo>(AddHistory), new[] { p }); 
				return; 
			}
			history.Items.Insert( 0, p );
		}

		public void AddHistoryString( string item )
		{
			history.Items.Insert( 0, item );
		}

		private void Attendant_LocationChanged(object sender, EventArgs e)
		{
			Settings1.Default.AttendantLocX = this.Location.X;
			Settings1.Default.AttendantLocY = this.Location.Y;
			Settings1.Default.Save();
		}

		private void save_Click(object sender, EventArgs e)
		{
			var p = history.SelectedItem as PersonInfo;
			Util.AddUpdateNotes(p.pid, notes.Text);
			p.notes = notes.Text;
		}

		private void history_SelectedIndexChanged(object sender, EventArgs e)
		{
			var p = history.SelectedItem as PersonInfo;
			if (p == null)
				return;
			var pb = Program.attendant.pictureBox1;
			pb.Image = Util.GetImage(p.pid);
			var na = Program.attendant.NameDisplay;
			na.Text = p.name;
			notes.Text = Util.GetNotes(p.pid);
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			var p = history.SelectedItem as PersonInfo;
			if (p == null)
				return;
			System.Diagnostics.Process.Start(Program.URL + "/Person2/" + p.pid);
		}
	}
}
