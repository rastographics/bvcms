using System;
using System.Windows.Forms;

namespace CmsCheckin
{
    public partial class AttendMenu : UserControl
    {
        public event EventHandler VisitClass;
        public event EventHandler EditRecord;
        public event EventHandler AddFamily;
        public event EventHandler JoinClass;
        public event EventHandler DropJoinClass;
        public event EventHandler PrintLabel;
        public event EventHandler CancelMenu;
        
        public AttendMenu()
        {
            InitializeComponent();
            Join.Enabled = !Program.DisableJoin;
            DropJoin.Enabled = !Program.DisableJoin;
        }

        private void Visit_Click(object sender, EventArgs e)
        {
            if (VisitClass != null)
                VisitClass(sender, e);
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            if (EditRecord != null)
                EditRecord(sender, e);
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (AddFamily != null)
                AddFamily(sender, e);
        }

        private void Join_Click(object sender, EventArgs e)
        {
            if (JoinClass != null)
                JoinClass(sender, e);
        }

        private void Print_Click(object sender, EventArgs e)
        {
            if (PrintLabel != null)
                PrintLabel(sender, e);
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (CancelMenu != null)
                CancelMenu(sender, e);
        }

        private void DropJoin_Click(object sender, EventArgs e)
        {
            if (DropJoinClass != null)
                DropJoinClass(sender, e);
        }

    }
}
