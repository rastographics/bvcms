using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net;
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

            keyboard = new CommonKeyboard(this);
        }

        private void onLoginLoad(object sender, EventArgs e)
        {
            this.CenterToScreen();
            this.Location = new Point(this.Location.X, this.Location.Y / 2);

            updateViews();

            //if (Program.settings.popupForVersion < 1) {
            //	MessageBox.Show("The Check-In program has been updated,\nplease verify the following settings:\n\n" +
            //		"Login Page\n\n- Server name (e.g. <yourchurch>.tpsdb.com)\n- Username\n- Password\n- Printer\n- Advanced Page Size (Optional)\n\n" +
            //		"Settings Page\n\n- Campus\n- Early Checkin Hours\n- Late Checkin Minutes\n- Checkboxes at the bottom", "New Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            //	Program.settings.setPopupForVersion(1);
            //}

            keyboard.Show();
            attachKeyboard();

            this.Focus();

            URL.Text = Program.settings.subdomain.Replace(".tpsdb.com", "").Replace(".bvcms.com", "");
            username.Text = Program.settings.user;

            if (username.Text.Length > 0)
            {
                current = password;
                this.ActiveControl = password;
            }
            else
            {
                current = URL;
                this.ActiveControl = URL;
            }
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
            Program.settings.save();

            try
            {
                var wc = Util.CreateWebClient();

                var exists = Program.settings.createURI("CheckInAPI/Exists");
                var existsResults = wc.DownloadString(exists);

                if (existsResults != "1")
                {
                    MessageBox.Show("The server you enter is not valid, please try again.\n\n" + Program.settings.createURL(), "Connection Error");
                }
                else
                {
                    var auth = Program.settings.createURI("CheckInAPI/Authenticate");
                    var authResults = wc.DownloadString(auth);

                    BaseMessage bm = JsonConvert.DeserializeObject<BaseMessage>(authResults);

                    if (bm.error == 0)
                    {
                        CheckInInformation info = JsonConvert.DeserializeObject<CheckInInformation>(bm.data);

                        if (info != null)
                        {
                            Program.settingsList = info.settings;
                            Program.campusList = info.campuses;

                            Program.settingsList.Insert(0, new CheckInSettingsEntry() { name = "<Current>", settings = "" });

                            DialogResult = DialogResult.OK;
                            this.Hide();
                        }

                    }
                    else
                    {
                        if (bm.error == -6)
                        {
                            MessageBox.Show("Your username or password was incorrect, please try again.", "Server Error");
                        }
                        else
                        {
                            MessageBox.Show("Error: " + bm.error, "Server Error");
                        }
                    }
                }
            }
            catch (WebException)
            {
                MessageBox.Show("Could not connect to: " + Program.settings.createURL(), "Connection Error");
            }
        }

        private void onProtocolClick(object sender, EventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Alt) && ModifierKeys.HasFlag(Keys.Shift))
            {
                Program.settings.ssl = !Program.settings.ssl;

                Program.settings.useTPSDB = Program.settings.ssl;

                updateViews();
            }
        }

        private void updateViews()
        {
            if (Program.settings.ssl)
            {
                protocolLabel.Text = "https://";
            }
            else
            {
                protocolLabel.Text = "http://";
            }

            if (Program.settings.useTPSDB)
            {
                domainLabel.Visible = true;
                URL.Width = 260;
            }
            else
            {
                domainLabel.Visible = false;
                URL.Width = 330;
            }
        }

        private void onPasswordKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                onLoginClick(null, null);
            }
        }

        private void onDomainLabelClick(object sender, EventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Alt) && ModifierKeys.HasFlag(Keys.Shift))
            {
                Program.settings.useTPSDB = !Program.settings.useTPSDB;
                updateViews();
            }
        }
    }
}
