using System;
using System.Windows.Forms;

namespace CmsCheckin
{
    public partial class BuildingAttendant : Form
    {
        public BuildingAttendant()
        {
            InitializeComponent();
        }

        private void ShowCheckin_Click(object sender, EventArgs e)
        {
            if (Program.baseform == null)
                Program.baseform = new BaseForm(new BuildingHome());
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
            history.Items.Insert(0, p);
            history.SetSelected(0, true);
        }

        public void AddHistoryString(string item)
        {
            history.Items.Insert(0, item);
        }

        private void Attendant_LocationChanged(object sender, EventArgs e)
        {
            Program.settings.attendantPositionChanged(this.Location);
        }

        private void save_Click(object sender, EventArgs e)
        {
            var p = history.SelectedItem as PersonInfo;

            if (p != null)
            {
                save.Text = "Saving...";
                save.Invalidate();
                save.Update();
                save.Refresh();

                Util.AddUpdateNotes(p.pid, notes.Text);
                p.notes = notes.Text;
                save.Text = "Complete!";
            }
        }

        private void history_SelectedIndexChanged(object sender, EventArgs e)
        {
            save.Text = "Save";

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
            System.Diagnostics.Process.Start(Program.settings.createURL("Person2/" + p.pid));
        }

        private void onNotesTextChange(object sender, EventArgs e)
        {
            save.Text = "Save";
        }
    }
}
