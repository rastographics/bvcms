using System;
using System.Windows.Forms;
using System.IO;
using CmsCheckin.Classes;

namespace CmsCheckin
{
	public partial class AttendPrintingServer : Form
	{
		// 360 tries of 10 seconds = 1 Hour
		//private const int MAX_TRIES = 360;

		// For testing only
		private const int MAX_TRIES = 360;

		private DateTime dtLastPrint;
		private const int INT_count = 5;

		private int triesLeft;

		public AttendPrintingServer()
		{
			InitializeComponent();
		}

		private void PrintingServer_Load(object sender, EventArgs e)
		{
			StartChecking();
		}

		private void StartChecking()
		{
			triesLeft = MAX_TRIES;

			timer1 = new Timer();
			timer1.Interval = 1000;
			timer1.Tick += new EventHandler(timer1_Tick);
			count = INT_count;
			timer1.Start();
		}

		private void CheckNow_Click(object sender, EventArgs e)
		{
			Countdown.Text = "Checking...";

			count = 0;
			StartChecking();
		}

		private void CheckServer()
		{
			int iLabelSize = PrinterHelper.getPageHeight(Program.Printer);

			timer1.Stop();
			Countdown.Text = "Checking...";
			Refresh();

			var pj = Util.FetchPrintJob();

			if (pj.jobs.Count > 0) {
				dtLastPrint = DateTime.Now;

				foreach (var j in pj.jobs) {
					Program.SecurityCode = j.securitycode;

					foreach (var e in j.list) { e.securitycode = Program.SecurityCode; }
					PrinterHelper.doPrinting(j.list);
				}

				triesLeft = MAX_TRIES;
			} else {
				triesLeft--;
			}

			count = INT_count;
			timer1.Start();
		}
		int count = 0;
		Timer timer1;

		void timer1_Tick(object sender, EventArgs e)
		{
			if (triesLeft <= 0) {
				timer1.Stop();
				timer1.Dispose();

				Countdown.Text = "-- IDLE --\nClick \"Check Now\" to resume";

				return;
			}

			if (count == 0)
				CheckServer();
			else {
				Countdown.Text = count.ToString();
				Refresh();
				count--;
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			const int WM_KEYDOWN = 0x100;
			const int WM_SYSKEYDOWN = 0x104;

			if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN)) {
				switch (keyData) {
					case Keys.Space:
					case Keys.Return:
						CheckServer();
						break;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
