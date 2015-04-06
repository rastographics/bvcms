using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
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

			if (username.Text.Length > 0) {
				current = password;
				this.ActiveControl = password;
			} else {
				current = URL;
				this.ActiveControl = URL;
			}
		}

		private void saveSettings()
		{
			//if (URL.Text.StartsWith("localhost") || !UseSSL.Checked)
			//	Program.URL = "http://" + URL.Text;
			//else if (Settings1.Default.UseSSL)
			//	Program.URL = "https://" + URL.Text;
			//else
			//	Program.URL = "http://" + URL.Text;

			Settings1.Default.URL = URL.Text;
			Settings1.Default.username = username.Text;
			Settings1.Default.Save();

			Program.settings.user = username.Text;
			Program.settings.pass = password.Text;
		}

		private void onFormMove(object sender, EventArgs e)
		{
			attachKeyboard();
		}

		private void attachKeyboard()
		{
			keyboard.Location = new Point(this.Location.X + (this.Width / 2) - (keyboard.Width / 2), this.Location.Y + this.Height + 5);
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

		private void onLoginClick(object sender, EventArgs e)
		{
			Program.settings.subdomain = URL.Text;
			Program.settings.user = username.Text;
			Program.settings.pass = password.Text;

			try {
				var wc = Util.CreateWebClient();

				var exists = Program.settings.createURI("CheckInAPI/Exists");
				var existsResults = wc.DownloadString(exists);

				if (existsResults != "1") {
					MessageBox.Show("The server you enter is not valid, please try again.\n\n" + Program.settings.createURL(), "Server not found!");
					return;
				} else {
					var auth = Program.settings.createURI("CheckIn/Authenticate");
					var authResults = wc.DownloadString(auth);

					BaseMessage bm = JsonConvert.DeserializeObject<BaseMessage>(authResults);

					if (bm.error == 0) {
						List<CheckInSetting> settings = JsonConvert.DeserializeObject<List<CheckInSetting>>(bm.data);
						this.Hide();
					} else {

					}
				}
			} catch (WebException) {
				MessageBox.Show("Could not connect to: " + Program.settings.createURL());
			}
		}

		private void onProtocolClick(object sender, EventArgs e)
		{
			if (ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Alt) && ModifierKeys.HasFlag(Keys.Shift)) {
				Program.settings.ssl = !Program.settings.ssl;
			}
		}
	}
}
