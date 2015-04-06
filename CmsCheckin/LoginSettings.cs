using System;
using System.Windows.Forms;
using System.Net;
using System.Xml.Linq;
using System.Drawing.Printing;
using CmsCheckin.Classes;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Drawing;

namespace CmsCheckin
{
	public partial class LoginSettings : Form, KeyboardInterface
	{
		public XDocument campuses { get; set; }

		private bool CancelClose { get; set; }
		private TextBox current = null;
		private Form keyboard;

		private Regex rx = new Regex("\\D");

		public LoginSettings()
		{
			InitializeComponent();

			building.Enter += onTextboxEnter;
			PrintKiosks.Enter += onTextboxEnter;
			PrinterWidth.Enter += onTextboxEnter;
			PrinterHeight.Enter += onTextboxEnter;
			AdminPIN.Enter += onTextboxEnter;
			AdminPINTimeout.Enter += onTextboxEnter;
		}

		private void onLoginSettingsLoad(object sender, EventArgs e)
		{
			this.CenterToScreen();
			this.Location = new Point(this.Location.X, this.Location.Y / 2);

			keyboard = new CommonKeyboard(this);
			keyboard.Show();
			attachKeyboard();

			var prtdoc = new PrintDocument();
			var defp = prtdoc.PrinterSettings.PrinterName;
			var printerList = new List<string>();

			for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++) {
				printerList.Add(PrinterSettings.InstalledPrinters[i]);
				Printer.Items.Add(PrinterSettings.InstalledPrinters[i]);
			}

			if (printerList.Contains(Settings1.Default.Printer)) {
				Printer.SelectedIndex = Printer.FindStringExact(Settings1.Default.Printer);
			} else {
				Printer.SelectedIndex = Printer.FindStringExact(defp);
			}

			DisableLocationLabels.Checked = Settings1.Default.DisableLocationLabels;
			BuildingAccessMode.Checked = Settings1.Default.BuildingMode;
			PrintKiosks.Text = Settings1.Default.Kiosks;
			PrintMode.Text = Settings1.Default.PrintMode;
			building.Text = Settings1.Default.Building;
			AdvancedPageSize.Checked = Settings1.Default.AdvancedPageSize;
			PrinterWidth.Text = Settings1.Default.PrinterWidth;
			PrinterHeight.Text = Settings1.Default.PrinterHeight;
			AdminPIN.Text = Settings1.Default.AdminPIN;
			AdminPINTimeout.Text = Settings1.Default.AdminPINTimeout;

			if (!Util.IsDebug()) {
				this.Height = 385;

				PrintTest.Enabled = false;
				label5.Enabled = false;
				LabelFormat.Enabled = false;
				LabelList.Enabled = false;
				label10.Enabled = false;
				LoadLabelList.Enabled = false;
				SaveLabel.Enabled = false;

				PrintTest.Visible = false;
				label5.Visible = false;
				LabelFormat.Visible = false;
				LabelList.Visible = false;
				label10.Visible = false;
				LoadLabelList.Visible = false;
				SaveLabel.Visible = false;
			} else {
				this.Height = 640;
			}

			if (PrintMode.Text == "Print From Server") {
				PrintKiosks.Enabled = true;
				label1.Enabled = true;
			} else {
				PrintKiosks.Enabled = false;
				label1.Enabled = false;
			}

			attachKeyboard();
		}

		private void onLoginSettingsClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = CancelClose;
			CancelClose = false;
		}

		private void onGoClick(object sender, EventArgs e)
		{
			Settings1.Default.Kiosks = PrintKiosks.Text;
			Settings1.Default.PrintMode = PrintMode.Text;
			Settings1.Default.Printer = Printer.Text;
			Settings1.Default.DisableLocationLabels = DisableLocationLabels.Checked;
			Settings1.Default.BuildingMode = BuildingAccessMode.Checked;
			Settings1.Default.Building = building.Text;
			Settings1.Default.PrinterWidth = PrinterWidth.Text;
			Settings1.Default.PrinterHeight = PrinterHeight.Text;
			Settings1.Default.AdvancedPageSize = AdvancedPageSize.Checked;
			Settings1.Default.AdminPIN = AdminPIN.Text;

			if (AdminPINTimeout.Text.Length > 0) {
				try {
					Program.settings.adminPINTimeout = int.Parse(AdminPINTimeout.Text);
					Settings1.Default.AdminPINTimeout = AdminPINTimeout.Text;
				} catch (Exception) {
					Program.settings.adminPINTimeout = 0;
					Settings1.Default.AdminPINTimeout = "0";
				}
			} else {
				Program.settings.adminPINTimeout = 0;
			}

			Settings1.Default.Save();

			Program.settings.printerWidth = PrinterWidth.Text;
			Program.settings.printerHeight = PrinterHeight.Text;
			Program.settings.disableLocationLabels = DisableLocationLabels.Checked;
			Program.settings.adminPIN = AdminPIN.Text;

			if (BuildingAccessMode.Checked == true) {
				try {
					Program.settings.building = building.Text;
					Program.BuildingInfo = Util.FetchBuildingInfo();
					if (Program.BuildingInfo.Activities.Count == 0) {
						CancelClose = true;
						return;
					}
				} catch (Exception) {
					MessageBox.Show("Cannot find " + Program.settings.createURL());
					CancelClose = true;
				}
			}

			var wc = Util.CreateWebClient();

			try {
				var url = Program.settings.createURI("Checkin2/Campuses");
				var str = wc.DownloadString(url);
				if (str == "not authorized") {
					MessageBox.Show(str);
					CancelClose = true;
					return;
				}
				campuses = XDocument.Parse(str);
			} catch (WebException) {
				MessageBox.Show("Cannot find " + Program.settings.createURL());
				CancelClose = true;
			}

			if (CancelClose == false && !BuildingAccessMode.Checked) {
				bool bHorizontalCheck = false;
				bool bVerticalCheck = false;

				if (AdvancedPageSize.Checked) {
					if (PrinterWidth.Text.Length == 0) PrinterWidth.Text = "0";
					if (PrinterHeight.Text.Length == 0) PrinterHeight.Text = "0";

					bHorizontalCheck = int.Parse(PrinterWidth.Text) >= 290;
					bVerticalCheck = (int.Parse(PrinterHeight.Text) > 70 && int.Parse(PrinterHeight.Text) < 130) ||
													(int.Parse(PrinterHeight.Text) > 170 && int.Parse(PrinterHeight.Text) < 230);
				} else {
					bHorizontalCheck = PrinterHelper.getPrinterWidth(Printer.Text) > 290;
					bVerticalCheck = (PrinterHelper.getPrinterHeight(Printer.Text) > 70 && PrinterHelper.getPrinterHeight(Printer.Text) < 130) ||
													(PrinterHelper.getPrinterHeight(Printer.Text) > 170 && PrinterHelper.getPrinterHeight(Printer.Text) < 230);
				}

				if (!bHorizontalCheck || !bVerticalCheck) {
					if (MessageBox.Show("The selected printer does not have a recommended page size.  Do you want to continue?", "Printer Configuration Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
						CancelClose = true;
					}
				}
			}

			if (CancelClose == false) {
				this.Hide();
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
			System.Diagnostics.Debug.Print("Loading Label List...");

			string[] labelList = PrinterHelper.fetchLabelList();

			if (labelList == null) return;

			LabelList.Items.Clear();
			LabelList.Text = "";
			LabelFormat.Text = "";

			foreach (var label in labelList) {
				LabelList.Items.Add(label);
			}
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
		}

		private void onLabelListChanged(object sender, EventArgs e)
		{
			string[] sLabelPieces = LabelList.Text.Split(new char[] { '~' });
			LabelFormat.Text = PrinterHelper.fetchLabelFormat(sLabelPieces[0], int.Parse(sLabelPieces[1])).Replace("~", "~\r\n");
		}

		private void onAdvancedPageSizeChanged(object sender, EventArgs e)
		{
			if (AdvancedPageSize.Checked) {
				PageWidthLabel.Enabled = true;
				PageHeightLabel.Enabled = true;

				PrinterWidth.Enabled = true;
				PrinterHeight.Enabled = true;

				SizeFromPrinter.Enabled = true;

				PrinterWidth.Text = "";
				PrinterHeight.Text = "";
			} else {
				PageWidthLabel.Enabled = false;
				PageHeightLabel.Enabled = false;

				PrinterWidth.Enabled = false;
				PrinterHeight.Enabled = false;

				SizeFromPrinter.Enabled = false;

				PrinterWidth.Text = "";
				PrinterHeight.Text = "";
			}
		}

		private void onPrintModeChanged(object sender, EventArgs e)
		{
			if (PrintMode.SelectedIndex == 2) {
				PrintKiosks.Enabled = true;
				label1.Enabled = true;
			} else {
				PrintKiosks.Enabled = false;
				label1.Enabled = false;
				PrintKiosks.Text = "";
			}
		}

		private void onTextboxEnter(object sender, EventArgs e)
		{
			current = (TextBox)sender;
		}

		private void limitNumbersOnly(object sender, EventArgs e)
		{
			TextBox tb = sender as TextBox;

			try {
				tb.Text = rx.Replace(tb.Text, "");
				tb.Select(tb.Text.Length, 0);
			} catch (ArgumentException) { }
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

		private void attachKeyboard()
		{
			keyboard.Location = new Point(this.Location.X + (this.Width / 2) - (keyboard.Width / 2), this.Location.Y + this.Height + 5);
		}

		private void onLoginSettingsMove(object sender, EventArgs e)
		{
			attachKeyboard();
		}
	}
}
