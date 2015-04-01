using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CmsCheckin
{
	public partial class Login : Form, KeyboardInterface
	{
		private Form keyboard;
		private TextBox current = null;

		public Login()
		{
			InitializeComponent();

			username.Enter += onTextboxEnter;
			password.Enter += onTextboxEnter;
			URL.Enter += onTextboxEnter;
		}

		private void onLoginLoad(object sender, EventArgs e)
		{
			this.CenterToScreen();
			this.Location = new Point(this.Location.X, this.Location.Y / 2);

			if (Settings1.Default.PopupForVersion < 1) {
				MessageBox.Show("The Check-In program has been updated,\nplease verify the following settings:\n\n" +
					"Login Page\n\n- Server name (e.g. <yourchurch>.tpsdb.com)\n- Username\n- Password\n- Printer\n- Advanced Page Size (Optional)\n\n" +
					"Settings Page\n\n- Campus\n- Early Checkin Hours\n- Late Checkin Minutes\n- Checkboxes at the bottom", "New Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				Settings1.Default.PopupForVersion = 1;
				Settings1.Default.Save();
			}

			keyboard = new CommonKeyboard(this);
			keyboard.Show();
			attachKeyboard();
		}

		private void onFormMove(object sender, EventArgs e)
		{
			attachKeyboard();
		}

		private void attachKeyboard()
		{
			keyboard.Location = new Point(this.Location.X + (this.Width / 2) - (keyboard.Width / 2), this.Location.Y + this.Height);
		}

		private void onLoginClosing(object sender, FormClosingEventArgs e)
		{
			keyboard.Close();
			keyboard.Dispose();
		}

		public void onKeyboardKeyPress(string key)
		{
			if (current == null) return;

			current.Text += key;
			current.Focus();
			current.Select(current.Text.Length, 0);
		}

		public void onBackspaceKeyPress()
		{
			if (current == null) return;

			var t = current.Text;
			var len = t.Length - 1;

			if (len < 0) len = 0;

			current.Text = t.Substring(0, len);
			current.Focus();
			current.Select(current.Text.Length, 0);
		}

		private void onTextboxEnter(object sender, EventArgs e)
		{
			current = (TextBox)sender;
		}
	}
}
