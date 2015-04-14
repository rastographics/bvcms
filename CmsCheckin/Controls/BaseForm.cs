using System;
using System.Drawing;
using System.Windows.Forms;

namespace CmsCheckin
{
    public partial class BaseForm : Form
    {

		UserControl home;
        public BaseForm(UserControl home)
        {
			this.home = home;
            InitializeComponent();
        }

		public TextBox textbox;
        private void BaseForm_Load(object sender, EventArgs e)
        {
			Program.CursorHide();
            ControlsAdd(home);
			home.Visible = true;
			if (home is AttendHome)
				textbox = ((AttendHome)home).textBox1;
			else if (home is BuildingHome)
				textbox = ((BuildingHome)home).textBox1;
        }


        public void ControlsAdd(UserControl ctl)
        {
            ctl.Location = new Point { X = (this.Width / 2) - (ctl.Width / 2), Y = 0 };
            ctl.Visible = false;
            Controls.Add(ctl);
        }

		private void BaseForm_Resize(object sender, EventArgs e)
		{
            home.Location = new Point { X = (this.Width / 2) - (home.Width / 2), Y = 0 };
		}

		private void BaseForm_LocationChanged(object sender, EventArgs e)
		{
			Program.settings.basePositionChanged(this.Location);
		}
    }
}
