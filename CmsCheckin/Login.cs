using System;
using System.Windows.Forms;
using System.Net;
using System.Xml.Linq;
using System.Drawing.Printing;
using CmsCheckin.Classes;
using System.Text.RegularExpressions;

namespace CmsCheckin
{
	public partial class Login : Form
	{
		public Login()
		{
			InitializeComponent();

			b1.Click += buttonclick;
			b2.Click += buttonclick;
			b3.Click += buttonclick;
			b4.Click += buttonclick;
			b5.Click += buttonclick;
			b6.Click += buttonclick;
			b7.Click += buttonclick;
			b8.Click += buttonclick;
			b9.Click += buttonclick;
			b0.Click += buttonclick;
			bdash.Click += buttonclick;
			bequal.Click += buttonclick;

			bq.Click += buttonclick;
			bw.Click += buttonclick;
			be.Click += buttonclick;
			br.Click += buttonclick;
			bt.Click += buttonclick;
			by.Click += buttonclick;
			bu.Click += buttonclick;
			bi.Click += buttonclick;
			bo.Click += buttonclick;
			bp.Click += buttonclick;
			blbrace.Click += buttonclick;
			brbrace.Click += buttonclick;

			ba.Click += buttonclick;
			bs.Click += buttonclick;
			bd.Click += buttonclick;
			bf.Click += buttonclick;
			bg.Click += buttonclick;
			bh.Click += buttonclick;
			bj.Click += buttonclick;
			bk.Click += buttonclick;
			bl.Click += buttonclick;

			bz.Click += buttonclick;
			bx.Click += buttonclick;
			bc.Click += buttonclick;
			bv.Click += buttonclick;
			bb.Click += buttonclick;
			bn.Click += buttonclick;
			bm.Click += buttonclick;

			bcomma.Click += buttonclick;
			bdot.Click += buttonclick;
			bslash.Click += buttonclick;

			//username.KeyPress += textBox_KeyPress;
			//password.KeyPress += textBox_KeyPress;
			//URL.KeyPress += textBox_KeyPress;

			username.Enter += textbox_Enter;
			password.Enter += textbox_Enter;
			URL.Enter += textbox_Enter;
			building.Enter += textbox_Enter;
			PrintKiosks.Enter += textbox_Enter;
			PrinterWidth.Enter += textbox_Enter;
			PrinterHeight.Enter += textbox_Enter;

			password.Focus();
		}

		public XDocument campuses { get; set; }
		private void button1_Click(object sender, EventArgs e)
		{
			Settings1.Default.URL = URL.Text;
			Settings1.Default.username = username.Text;
			Settings1.Default.Kiosks = PrintKiosks.Text;
			Settings1.Default.PrintMode = PrintMode.Text;
			Settings1.Default.Printer = Printer.Text;
			Settings1.Default.DisableLocationLabels = DisableLocationLabels.Checked;
			Settings1.Default.BuildingMode = BuildingAccessMode.Checked;
			Settings1.Default.Building = building.Text;
			Settings1.Default.PrinterWidth = PrinterWidth.Text;
			Settings1.Default.PrinterHeight = PrinterHeight.Text;
			Settings1.Default.AdvancedPageSize = AdvancedPageSize.Checked;
			Settings1.Default.UseSSL = UseSSL.Checked;
			Settings1.Default.AdminPIN = AdminPIN.Text;

			if (AdminPINTimeout.Text.Length > 0)
			{
				try
				{
					Program.AdminPINTimeout = int.Parse(AdminPINTimeout.Text);
					Settings1.Default.AdminPINTimeout = AdminPINTimeout.Text;
				}
				catch (Exception)
				{
					Program.AdminPINTimeout = 0;
					Settings1.Default.AdminPINTimeout = "0";
				}
			}
			else
			{
				Program.AdminPINTimeout = 0;
			}

			Settings1.Default.Save();

			if (URL.Text.StartsWith("localhost") || !UseSSL.Checked)
				Program.URL = "http://" + URL.Text;
			else if (Settings1.Default.UseSSL)
				Program.URL = "https://" + URL.Text;
			else
				Program.URL = "http://" + URL.Text;

			Program.Username = username.Text;
			Program.Password = password.Text;
			Program.PrinterWidth = PrinterWidth.Text;
			Program.PrinterHeight = PrinterHeight.Text;
			Program.DisableLocationLabels = DisableLocationLabels.Checked;
			Program.AdminPIN = AdminPIN.Text;
			
			if (BuildingAccessMode.Checked == true)
			{
				try
				{
					Program.Building = building.Text;
					Program.BuildingInfo = Util.FetchBuildingInfo();
					if (Program.BuildingInfo.Activities.Count == 0)
					{
						CancelClose = true;
						return;
					}
				}
				catch (Exception)
				{
					MessageBox.Show("cannot find " + Program.URL);
					CancelClose = true;
				}
			}
			var wc = Util.CreateWebClient();
			try
			{
				var url = new Uri(new Uri(Program.URL), "Checkin2/Campuses");
				var str = wc.DownloadString(url);
				if (str == "not authorized")
				{
					MessageBox.Show(str);
					CancelClose = true;
					return;
				}
				campuses = XDocument.Parse(str);
			}
			catch (WebException)
			{
				MessageBox.Show("cannot find " + Program.URL);
				CancelClose = true;
			}

			if (CancelClose == false && !BuildingAccessMode.Checked)
			{
				bool bHorizontalCheck = false;
				bool bVerticalCheck = false;

				if (AdvancedPageSize.Checked)
				{
					if (PrinterWidth.Text.Length == 0) PrinterWidth.Text = "0";
					if (PrinterHeight.Text.Length == 0) PrinterHeight.Text = "0";

					bHorizontalCheck = int.Parse(PrinterWidth.Text) >= 290;
					bVerticalCheck = (int.Parse(PrinterHeight.Text) > 70 && int.Parse(PrinterHeight.Text) < 130) ||
													(int.Parse(PrinterHeight.Text) > 170 && int.Parse(PrinterHeight.Text) < 230);
				}
				else
				{
					bHorizontalCheck = PrinterHelper.getPrinterWidth(Printer.Text) > 290;
					bVerticalCheck = (PrinterHelper.getPrinterHeight(Printer.Text) > 70 && PrinterHelper.getPrinterHeight(Printer.Text) < 130) ||
													(PrinterHelper.getPrinterHeight(Printer.Text) > 170 && PrinterHelper.getPrinterHeight(Printer.Text) < 230);
				}

				if (!bHorizontalCheck || !bVerticalCheck)
				{
					if (MessageBox.Show("The selected printer does not have a valid page size.  Do you want to continue?", "Printer Configuration Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					{
						CancelClose = true;
					}
				}
			}

			if (CancelClose == false)
			{
				this.Hide();
			}
		}

		TextBox current = null;
		private void Login_Load(object sender, EventArgs e)
		{
			password.Focus();
			var prtdoc = new PrintDocument();
			var defp = prtdoc.PrinterSettings.PrinterName;
			foreach (var s in PrinterSettings.InstalledPrinters)
				Printer.Items.Add(s);

			Printer.SelectedIndex = Printer.FindStringExact(defp);
			if (Settings1.Default.Printer.HasValue())
				Printer.SelectedIndex = Printer.FindStringExact(Settings1.Default.Printer);
			DisableLocationLabels.Checked = Settings1.Default.DisableLocationLabels;
			BuildingAccessMode.Checked = Settings1.Default.BuildingMode;
			URL.Text = Settings1.Default.URL;
			username.Text = Settings1.Default.username;
			PrintKiosks.Text = Settings1.Default.Kiosks;
			PrintMode.Text = Settings1.Default.PrintMode;
			building.Text = Settings1.Default.Building;
			AdvancedPageSize.Checked = Settings1.Default.AdvancedPageSize;
			PrinterWidth.Text = Settings1.Default.PrinterWidth;
			PrinterHeight.Text = Settings1.Default.PrinterHeight;
			UseSSL.Checked = Settings1.Default.UseSSL;
			AdminPIN.Text = Settings1.Default.AdminPIN;
			AdminPINTimeout.Text = Settings1.Default.AdminPINTimeout;

			if (!Util.IsDebug())
			{
				this.Height = 570;

				PrintTest.Enabled = false;
				label5.Enabled = false;
				LabelFormat.Enabled = false;
				LabelList.Enabled = false;
				label10.Enabled = false;
				LoadLabelList.Enabled = false;
				SaveLabel.Enabled = false;
				UseSSL.Enabled = false;

				PrintTest.Visible = false;
				label5.Visible = false;
				LabelFormat.Visible = false;
				LabelList.Visible = false;
				label10.Visible = false;
				LoadLabelList.Visible = false;
				SaveLabel.Visible = false;
				UseSSL.Visible = false;

			}

			if (PrintMode.Text == "Print From Server")
			{
				PrintKiosks.Enabled = true;
				label12.Enabled = true;
				label1.Enabled = true;
			}
			else
			{
				PrintKiosks.Enabled = false;
				label12.Enabled = false;
				label1.Enabled = false;
			}
		}

		void buttonclick(object sender, EventArgs e)
		{
			var b = sender as Button;
			var d = b.Text[0];
			KeyStroke(d);
		}

		private void textBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (current == null) return;

			if (e.KeyChar == '\b') BackSpace();
			else KeyStroke(e.KeyChar);

			e.Handled = true;
		}

		private void KeyStroke(char d)
		{
			if (current == null) return;

			current.Text += d;
			current.Focus();
			current.Select(current.Text.Length, 0);
		}

		private void BackSpace()
		{
			if (current == null) return;

			var t = current.Text;
			var len = t.Length - 1;

			if (len < 0) len = 0;

			current.Text = t.Substring(0, len);
			current.Focus();
			current.Select(current.Text.Length, 0);
		}

		private void textbox_Enter(object sender, EventArgs e)
		{
			current = (TextBox)sender;
		}

		private void bbs_Click(object sender, EventArgs e)
		{
			BackSpace();
		}

		public bool CancelClose { get; set; }
		private void Login_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = CancelClose;
			CancelClose = false;
		}

		private void bshift_Click(object sender, EventArgs e)
		{
			if (ba.Text == "A")
			{
				ba.Text = "a";
				bb.Text = "b";
				bc.Text = "c";
				bd.Text = "d";
				be.Text = "e";
				bf.Text = "f";
				bg.Text = "g";
				bh.Text = "h";
				bi.Text = "i";
				bj.Text = "j";
				bk.Text = "k";
				bl.Text = "l";
				bm.Text = "m";
				bn.Text = "n";
				bo.Text = "o";
				bp.Text = "p";
				bq.Text = "q";
				br.Text = "r";
				bs.Text = "s";
				bt.Text = "t";
				bu.Text = "u";
				bv.Text = "v";
				bw.Text = "w";
				bx.Text = "x";
				by.Text = "y";
				bz.Text = "z";

				b1.Text = "1";
				b2.Text = "2";
				b3.Text = "3";
				b4.Text = "4";
				b5.Text = "5";
				b6.Text = "6";
				b7.Text = "7";
				b8.Text = "8";
				b9.Text = "9";
				b0.Text = "0";
				bdash.Text = "-";
				bequal.Text = "=";
				blbrace.Text = "[";
				brbrace.Text = "]";
				bcolon.Text = ":";
				bcomma.Text = ",";
				bdot.Text = ".";
				bslash.Text = "/";

			}
			else
			{
				ba.Text = "A";
				bb.Text = "B";
				bc.Text = "C";
				bd.Text = "D";
				be.Text = "E";
				bf.Text = "F";
				bg.Text = "G";
				bh.Text = "H";
				bi.Text = "I";
				bj.Text = "J";
				bk.Text = "K";
				bl.Text = "L";
				bm.Text = "M";
				bn.Text = "N";
				bo.Text = "O";
				bp.Text = "P";
				bq.Text = "Q";
				br.Text = "R";
				bs.Text = "S";
				bt.Text = "T";
				bu.Text = "U";
				bv.Text = "V";
				bw.Text = "W";
				bx.Text = "X";
				by.Text = "Y";
				bz.Text = "Z";

				b1.Text = "!";
				b2.Text = "@";
				b3.Text = "#";
				b4.Text = "$";
				b5.Text = "%";
				b6.Text = "^";
				b7.Text = "&&";
				b8.Text = "*";
				b9.Text = "(";
				b0.Text = ")";
				bdash.Text = "_";
				bequal.Text = "+";
				blbrace.Text = "{";
				brbrace.Text = "}";
				bcolon.Text = ";";
				bcomma.Text = "<";
				bdot.Text = ">";
				bslash.Text = "?";
			}
		}

		private void Printer_SelectedIndexChanged(object sender, EventArgs e)
		{
			LabelPrinterSize.Text = "Label Size: " + PrinterHelper.getPrinterWidth(Printer.Text) + " X " + PrinterHelper.getPrinterHeight(Printer.Text);
		}

		private void PrintTest_Click(object sender, EventArgs e)
		{
			Program.PrinterWidth = PrinterWidth.Text;
			Program.PrinterHeight = PrinterHeight.Text;

			string[] sLabelPieces = LabelList.Text.Split(new char[] { '~' });

			if (sLabelPieces.Length >= 2) PrinterHelper.printTestLabel(Printer.Text, LabelFormat.Text);
			else PrinterHelper.printTestLabel(Printer.Text, LabelFormat.Text.Replace("\r\n", ""));
		}

		private void LoadLabelList_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.Print("Loading Label List...");

			if (URL.Text.Contains("localhost")) Program.URL = "http://" + URL.Text;
			else Program.URL = "https://" + URL.Text;

			Program.Username = username.Text;
			Program.Password = password.Text;

			string[] labelList = PrinterHelper.fetchLabelList();

			if (labelList == null) return;

			LabelList.Items.Clear();
			LabelList.Text = "";
			LabelFormat.Text = "";

			foreach (var label in labelList)
			{
				LabelList.Items.Add(label);
			}
		}

		private void LabelList_SelectedIndexChanged(object sender, EventArgs e)
		{
			string[] sLabelPieces = LabelList.Text.Split(new char[] { '~' });
			LabelFormat.Text = PrinterHelper.fetchLabelFormat(sLabelPieces[0], int.Parse(sLabelPieces[1])).Replace("~", "~\r\n");
		}

		private void SaveLabel_Click(object sender, EventArgs e)
		{
			string[] sLabelPieces = LabelList.Text.Split(new char[] { '~' });
			PrinterHelper.saveLabelFormat(sLabelPieces[0], sLabelPieces[1], LabelFormat.Text.Replace("\r\n", ""));

			LoadLabelList.PerformClick();
		}

		private void SizeFromPrinter_Click(object sender, EventArgs e)
		{
			PrinterWidth.Text = PrinterHelper.getPrinterWidth(Printer.Text).ToString();
			PrinterHeight.Text = PrinterHelper.getPrinterHeight(Printer.Text).ToString();
		}

		private void AdvancedPageSize_CheckedChanged(object sender, EventArgs e)
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

		private void PrintMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (PrintMode.SelectedIndex == 2)
			{
				PrintKiosks.Enabled = true;
				label12.Enabled = true;
				label1.Enabled = true;
			}
			else
			{
				PrintKiosks.Enabled = false;
				label12.Enabled = false;
				label1.Enabled = false;
				PrintKiosks.Text = "";
			}
		}

		private Regex rx = new Regex("\\D");

		private void NumbersOnly(object sender, EventArgs e)
		{
			TextBox tb = sender as TextBox;

			try
			{
				tb.Text = rx.Replace(tb.Text, "");
				tb.Select(tb.Text.Length, 0);
			}
			catch (ArgumentException) { }
		}
	}
}
