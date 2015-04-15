using System;
using System.Windows.Forms;

namespace CmsCheckin
{
    public partial class CommonPicture : Form
    {
        public CommonPicture()
        {
            InitializeComponent();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var f = new CommonTakePicture();
            f.ShowDialog();
            this.Close();
        }

        private void TakePic_Click(object sender, EventArgs e)
        {
            var f = new CommonTakePicture();
            f.ShowDialog();
            this.Close();
        }

        private void Return_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Picture_Load(object sender, EventArgs e)
        {
			pictureBox1.Image = Util.GetImage(Program.PeopleId);
        }
    }
}
