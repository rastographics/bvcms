using System;
using System.Windows.Forms;
using System.Net;
using System.Xml.Linq;
using System.Drawing.Printing;
using CmsCheckin.Classes;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using Newtonsoft.Json;
using System.Text;

namespace CmsCheckin
{
    public partial class LoginSettings : Form, KeyboardInterface
    {
        private static List<WeekDay> weekDays = new List<WeekDay>() {
			new WeekDay { id = "0", name = "Sunday" },
			new WeekDay { id = "1", name = "Monday" },
			new WeekDay { id = "2", name = "Tuesday" },
			new WeekDay { id = "3", name = "Wednesday" },
			new WeekDay { id = "4", name = "Thursday" },
			new WeekDay { id = "5", name = "Friday" },
			new WeekDay { id = "6", name = "Saturday" }
		};

        private bool CancelClose { get; set; }

        private ComboBox currentComboBox = null;
        private TextBox currentTextBox = null;
        private Form keyboard;

        private Regex rx = new Regex("\\D");

        public LoginSettings()
        {
            InitializeComponent();

            DayOfWeekCombo.DataSource = new BindingSource(weekDays, null);
            DayOfWeekCombo.DisplayMember = "Name";
            DayOfWeekCombo.ValueMember = "ID";
            DayOfWeekCombo.SelectedIndex = (int)DateTime.Now.DayOfWeek;

            CampusCombo.DataSource = new BindingSource(Program.campusList, null);
            CampusCombo.DisplayMember = "Name";
            CampusCombo.ValueMember = "ID";

            SettingsCombo.DataSource = new BindingSource(Program.settingsList, null);
            SettingsCombo.DisplayMember = "Name";
            SettingsCombo.Enter += onComboBoxEnter;

            Building.Enter += onTextboxEnter;

            PrintKiosks.Enter += onTextboxEnter;
            KioskName.Enter += onTextboxEnter;

            PrinterWidth.Enter += onTextboxEnter;
            PrinterHeight.Enter += onTextboxEnter;

            AdminPIN.Enter += onTextboxEnter;
            AdminPINTimeout.Enter += onTextboxEnter;

            keyboard = new CommonKeyboard(this);

            updateDisplay();
        }

        private void onLoginSettingsLoad(object sender, EventArgs e)
        {
            this.CenterToScreen();
            this.Location = new Point(this.Location.X, this.Location.Y / 2);

            keyboard.Show();
            attachKeyboard();

            var prtdoc = new PrintDocument();
            var defp = prtdoc.PrinterSettings.PrinterName;
            var printerList = new List<string>();

            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                printerList.Add(PrinterSettings.InstalledPrinters[i]);
                Printer.Items.Add(PrinterSettings.InstalledPrinters[i]);
            }

            if (printerList.Contains(Program.settings.printer))
            {
                Printer.SelectedIndex = Printer.FindStringExact(Program.settings.printer);
            }
            else
            {
                Printer.SelectedIndex = Printer.FindStringExact(defp);
            }

            if (!Util.IsDebug())
            {
                this.Height = 390;

                PrintTest.Enabled = false;
                FormatLabel.Enabled = false;
                LabelFormat.Enabled = false;
                LabelList.Enabled = false;
                NameLabel.Enabled = false;
                LoadLabelList.Enabled = false;
                SaveLabel.Enabled = false;

                PrintTest.Visible = false;
                FormatLabel.Visible = false;
                LabelFormat.Visible = false;
                LabelList.Visible = false;
                NameLabel.Visible = false;
                LoadLabelList.Visible = false;
                SaveLabel.Visible = false;
            }
            else
            {
                this.Height = 640;
            }

            if (PrintMode.Text == "Print From Server")
            {
                PrintKiosks.Enabled = true;
                KiosksToPrintForLabel.Enabled = true;
            }
            else
            {
                PrintKiosks.Enabled = false;
                KiosksToPrintForLabel.Enabled = false;
            }

            attachKeyboard();
        }

        private void onLoginSettingsClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = CancelClose;
            CancelClose = false;

            keyboard.Close();
            keyboard.Dispose();
        }

        private void onStartClick(object sender, EventArgs e)
        {
            updateSettings();
            Program.settings.save();

            if (Program.settings.buildingMode == true)
            {
                try
                {
                    Program.BuildingInfo = Util.FetchBuildingInfo();

                    if (Program.BuildingInfo.Activities.Count == 0)
                    {
                        CancelClose = true;
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Cannot find " + Program.settings.createURL());
                    CancelClose = true;
                }
            }

            if (CancelClose == false && !BuildingAccessMode.Checked)
            {
                bool bHorizontalCheck = false;
                bool bVerticalCheck = false;

                if (PrinterWidth.Text.Length == 0) PrinterWidth.Text = "0";
                if (PrinterHeight.Text.Length == 0) PrinterHeight.Text = "0";

                bHorizontalCheck = int.Parse(PrinterWidth.Text) >= 250;
                bVerticalCheck = int.Parse(PrinterHeight.Text) >= 51 && int.Parse(PrinterHeight.Text) < 250;

                if (!bHorizontalCheck || !bVerticalCheck)
                {
                    if (MessageBox.Show("The selected printer page size is not within the usable range.  Labels may not print at all.  Do you want to continue?", "Printer Configuration Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        CancelClose = true;
                    }
                }
            }

            if (CancelClose == false)
            {
                DialogResult = DialogResult.OK;
                this.Hide();
            }
            else
            {
                CancelClose = false;
            }
        }

        private void onPrintTestClick(object sender, EventArgs e)
        {
            Program.settings.printerWidth = PrinterWidth.Text;
            Program.settings.printerHeight = PrinterHeight.Text;

            string[] sLabelPieces = LabelList.Text.Split(new char[] { '~' });

            if (sLabelPieces.Length >= 2) PrinterHelper.printTestLabel(Printer.Text, LabelFormat.Text);
            else PrinterHelper.printTestLabel(Printer.Text, LabelFormat.Text.Replace("\r\n", ""));
        }

        private void onLoadLabelsClick(object sender, EventArgs e)
        {
            string[] labelList = PrinterHelper.fetchLabelList();

            if (labelList == null)
            {
                MessageBox.Show("Could fetch label formats.", "Label List Error");
                return;
            }

            LabelList.Items.Clear();
            LabelList.Text = "";
            LabelFormat.Text = "";

            foreach (var label in labelList)
            {
                LabelList.Items.Add(label);
            }

            MessageBox.Show("Label list load complete!", "Label List");
        }

        private void onSaveLabelClick(object sender, EventArgs e)
        {
            string[] sLabelPieces = LabelList.Text.Split(new char[] { '~' });
            PrinterHelper.saveLabelFormat(sLabelPieces[0], sLabelPieces[1], LabelFormat.Text.Replace("\r\n", ""));

            LoadLabelList.PerformClick();
        }

        private void onFromPrinterClick(object sender, EventArgs e)
        {
            PrinterWidth.Text = PrinterHelper.getPrinterWidth(Printer.Text).ToString();
            PrinterHeight.Text = PrinterHelper.getPrinterHeight(Printer.Text).ToString();
        }

        private void onPrinterChanged(object sender, EventArgs e)
        {
            LabelPrinterSize.Text = "Label Size: " + PrinterHelper.getPrinterWidth(Printer.Text) + " X " + PrinterHelper.getPrinterHeight(Printer.Text);

            if (Printer.Text.Contains("Datamax"))
            {
                UseOldDatamaxFormat.Enabled = true;
            }
            else
            {
                UseOldDatamaxFormat.Enabled = false;
                UseOldDatamaxFormat.Checked = false;
            }
        }

        private void onLabelListChanged(object sender, EventArgs e)
        {
            string[] sLabelPieces = LabelList.Text.Split(new char[] { '~' });
            LabelFormat.Text = PrinterHelper.fetchLabelFormat(sLabelPieces[0], int.Parse(sLabelPieces[1])).Replace("~", "~\r\n");
        }

        private void onAdvancedPageSizeChanged(object sender, EventArgs e)
        {
            if (AdvancedPageSize.Checked)
            {
                PageWidthLabel.Enabled = true;
                PageHeightLabel.Enabled = true;

                PrinterWidth.Enabled = true;
                PrinterHeight.Enabled = true;

                SizeFromPrinter.Enabled = true;

                PrinterWidth.Text = "";
                PrinterHeight.Text = "";
            }
            else
            {
                PageWidthLabel.Enabled = false;
                PageHeightLabel.Enabled = false;

                PrinterWidth.Enabled = false;
                PrinterHeight.Enabled = false;

                SizeFromPrinter.Enabled = false;

                PrinterWidth.Text = "";
                PrinterHeight.Text = "";
            }
        }

        private void onBuildingModeChanged(object sender, EventArgs e)
        {
            if (BuildingAccessMode.Checked)
            {
                Building.Enabled = true;
                Building.Text = "";
            }
            else
            {
                Building.Enabled = false;
                Building.Text = "";
            }
        }

        private void onPrintModeChanged(object sender, EventArgs e)
        {
            if (PrintMode.SelectedIndex == 2)
            {
                PrintKiosks.Enabled = true;
                KiosksToPrintForLabel.Enabled = true;

                advancedPrinterOptionsGroup.Enabled = true;
                Printer.Enabled = true;
                PrinterLabel.Enabled = true;
                KioskName.Enabled = true;
                KioskNameLabel.Enabled = true;
                labelOptionsGroup.Enabled = true;

                mainOptionsGroup.Enabled = false;
                buildingOptionsGroup.Enabled = false;
                askForOptioonsGroup.Enabled = false;
                otherOptionsGroup.Enabled = false;
                adminOptionsGroup.Enabled = false;
            }
            else if (PrintMode.SelectedIndex == 3)
            {
                advancedPrinterOptionsGroup.Enabled = false;
                PrintKiosks.Enabled = false;
                KiosksToPrintForLabel.Enabled = false;
                Printer.Enabled = false;
                PrinterLabel.Enabled = false;
                KioskName.Enabled = false;
                KioskNameLabel.Enabled = false;
                labelOptionsGroup.Enabled = false;

                mainOptionsGroup.Enabled = true;
                buildingOptionsGroup.Enabled = true;
                askForOptioonsGroup.Enabled = true;
                otherOptionsGroup.Enabled = true;
                adminOptionsGroup.Enabled = true;
            }
            else
            {
                PrintKiosks.Enabled = false;
                KiosksToPrintForLabel.Enabled = false;
                PrintKiosks.Text = "";

                mainOptionsGroup.Enabled = true;
                buildingOptionsGroup.Enabled = true;
                askForOptioonsGroup.Enabled = true;
                otherOptionsGroup.Enabled = true;
                adminOptionsGroup.Enabled = true;

                advancedPrinterOptionsGroup.Enabled = true;
                Printer.Enabled = true;
                PrinterLabel.Enabled = true;
                KioskName.Enabled = true;
                KioskNameLabel.Enabled = true;
                labelOptionsGroup.Enabled = true;
            }
        }

        private void onTextboxEnter(object sender, EventArgs e)
        {
            currentTextBox = (TextBox)sender;
            currentComboBox = null;
        }

        private void onComboBoxEnter(object sender, EventArgs e)
        {
            currentComboBox = (ComboBox)sender;
            currentTextBox = null;
        }

        private void limitNumbersOnly(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;

            try
            {
                tb.Text = rx.Replace(tb.Text, "");
                tb.Select(tb.Text.Length, 0);
            }
            catch (ArgumentException) { }
        }

        public void onKeyboardKeyPress(string key)
        {
            if (currentTextBox != null)
            {
                currentTextBox.Text += key;
                currentTextBox.Focus();
                currentTextBox.Select(currentTextBox.Text.Length, 0);
            }
            else if (currentComboBox != null)
            {
                currentComboBox.Text += key;
                currentComboBox.Focus();
                currentComboBox.Select(currentComboBox.Text.Length, 0);
            }
        }

        public void onBackspaceKeyPress()
        {
            if (currentTextBox != null)
            {
                var t = currentTextBox.Text;
                var len = t.Length - 1;

                if (len < 0) len = 0;

                currentTextBox.Text = t.Substring(0, len);
                currentTextBox.Focus();
                currentTextBox.Select(currentTextBox.Text.Length, 0);
            }
            else if (currentComboBox != null)
            {
                var t = currentComboBox.Text;
                var len = t.Length - 1;

                if (len < 0) len = 0;

                currentComboBox.Text = t.Substring(0, len);
                currentComboBox.Focus();
                currentComboBox.Select(currentComboBox.Text.Length, 0);
            }
        }

        private void attachKeyboard()
        {
            keyboard.Location = new Point(this.Location.X + (this.Width / 2) - (keyboard.Width / 2), this.Location.Y + this.Height + 5);
        }

        private void onLoginSettingsMove(object sender, EventArgs e)
        {
            attachKeyboard();
        }

        private void updateSettings()
        {
            // First Column
            if (CampusCombo.SelectedIndex >= 0)
            {
                Program.settings.campusID = ((CheckInCampus)Program.campusList[CampusCombo.SelectedIndex]).id;
                Program.settings.campus = ((CheckInCampus)Program.campusList[CampusCombo.SelectedIndex]).name;
            }
            else
            {
                Program.settings.campusID = "0";
            }

            Program.settings.dayOfWeek = DayOfWeekCombo.SelectedIndex;

            int.TryParse(EarlyHours.Text, out Program.settings.earlyHours);
            int.TryParse(LateMinutes.Text, out Program.settings.lateMinutes);

            Program.settings.buildingMode = BuildingAccessMode.Checked;
            Program.settings.building = Building.Text;

            // Second Column
            Program.settings.printMode = PrintMode.Text;
            Program.settings.printForKiosks = PrintKiosks.Text;
            Program.settings.printer = Printer.Text;
            Program.settings.kioskName = KioskName.Text;

            Program.settings.advancedPageSize = AdvancedPageSize.Checked;
            Program.settings.printerWidth = PrinterWidth.Text;
            Program.settings.printerHeight = PrinterHeight.Text;

            // Third Column
            Program.settings.disableLocationLabels = DisableLocationLabels.Checked;
            Program.settings.extraLabel = ExtraBlankLabel.Checked;
            Program.settings.securityLabelPerChild = SecurityLabelPerChild.Checked;
            Program.settings.useOldDatamaxFormat = UseOldDatamaxFormat.Checked;

            Program.settings.askFriend = AskFriend.Checked;
            Program.settings.askChurch = AskChurch.Checked;
            Program.settings.askChurchName = AskChurchName.Checked;
            Program.settings.askGrade = AskGrade.Checked;

            Program.settings.fullScreen = FullScreen.Checked;
            Program.settings.hideCursor = HideCursor.Checked;
            Program.settings.enableTimer = EnableTimer.Checked;
            Program.settings.disableJoin = DisableJoin.Checked;

            // Four Column			
            Program.settings.adminPIN = AdminPIN.Text;
            Program.settings.adminPINTimeout = AdminPINTimeout.Text;
        }

        private void updateDisplay()
        {
            // First Column
            CampusCombo.SelectedIndex = CampusCombo.FindStringExact(Program.settings.campus);

            DayOfWeekCombo.Text = Program.settings.dayOfWeek.ToString();

            EarlyHours.Text = Program.settings.earlyHours.ToString();
            LateMinutes.Text = Program.settings.lateMinutes.ToString();

            BuildingAccessMode.Checked = Program.settings.buildingMode;
            Building.Text = Program.settings.building;

            // Second Column
            PrintMode.Text = Program.settings.printMode;
            PrintKiosks.Text = Program.settings.printForKiosks;
            Printer.Text = Program.settings.printer;
            KioskName.Text = Program.settings.kioskName;

            AdvancedPageSize.Checked = Program.settings.advancedPageSize;
            PrinterWidth.Text = Program.settings.printerWidth;
            PrinterHeight.Text = Program.settings.printerHeight;

            // Third Column
            DisableLocationLabels.Checked = Program.settings.disableLocationLabels;
            ExtraBlankLabel.Checked = Program.settings.extraLabel;
            SecurityLabelPerChild.Checked = Program.settings.securityLabelPerChild;
            UseOldDatamaxFormat.Checked = Program.settings.useOldDatamaxFormat;

            AskFriend.Checked = Program.settings.askFriend;
            AskChurch.Checked = Program.settings.askChurch;
            AskChurchName.Checked = Program.settings.askChurchName;
            AskGrade.Checked = Program.settings.askGrade;

            FullScreen.Checked = Program.settings.fullScreen;
            HideCursor.Checked = Program.settings.hideCursor;
            EnableTimer.Checked = Program.settings.enableTimer;
            DisableJoin.Checked = Program.settings.disableJoin;

            // Four Column			
            AdminPIN.Text = Program.settings.adminPIN;
            AdminPINTimeout.Text = Program.settings.adminPINTimeout;
        }

        private void onSettingsSave(object sender, EventArgs e)
        {
            if (SettingsCombo.Text.Length > 0 && SettingsCombo.Text != "<Current>")
            {
                updateSettings();

                CheckInSettingsEntry settings = new CheckInSettingsEntry();
                settings.name = SettingsCombo.Text.Trim();
                settings.settings = JsonConvert.SerializeObject(Program.settings);

                try
                {
                    var wc = Util.CreateWebClient();
                    var post = new NameValueCollection();
                    var msg = new BaseMessage();

                    msg.data = JsonConvert.SerializeObject(settings);

                    post.Add("data", JsonConvert.SerializeObject(msg));

                    var save = Program.settings.createURI("CheckInAPI/SaveSettings");
                    var saveResults = wc.UploadValues(save, post);

                    BaseMessage bm = JsonConvert.DeserializeObject<BaseMessage>(Encoding.ASCII.GetString(saveResults));

                    if (bm.error == 0)
                    {
                        MessageBox.Show("Your settings have been saved!", "Save Complete");
                        Program.updateSettingsList(settings.name, settings.settings);
                        rebindSettingsList();
                    }
                    else
                    {
                        MessageBox.Show("The server you enter is not valid, please try again.\n\n" + Program.settings.createURL(), "Communication Error");
                    }
                }
                catch (WebException)
                {
                    MessageBox.Show("Could not connect to: " + Program.settings.createURL(), "Communication Error");
                }
            }
            else
            {
                if (SettingsCombo.Text.Length == 0)
                {
                    MessageBox.Show("The settings name cannot be blank, please save as a different name.", "Invalid Settings Name");
                }
                else
                {
                    MessageBox.Show("The settings name cannot be \"<Current>\", please save as a different name.", "Invalid Settings Name");
                }
            }
        }

        private void onSettingsNameChanged(object sender, EventArgs e)
        {
            string name = SettingsCombo.Text;

            if (name != "<Current>")
            {
                CheckInSettingsEntry entry = Program.getSettingsFromList(name);

                if (entry != null)
                {
                    Program.settings.copy(JsonConvert.DeserializeObject<Settings>(entry.settings));
                    updateDisplay();
                }
            }
        }

        private void rebindSettingsList()
        {
            SettingsCombo.DataSource = null;
            SettingsCombo.DataSource = new BindingSource(Program.settingsList, null);
            SettingsCombo.DisplayMember = "Name";
        }
    }
}
