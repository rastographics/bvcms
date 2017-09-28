using System;
using System.Windows.Forms;
using System.IO;
using CmsCheckin.Classes;

namespace CmsCheckin
{
	public partial class AttendPrintingServer : Form
	{
		private const int TIME_BETWEEN_TRIES = 5;
		private const int TIMEOUT_IN_MINUTES = 120;
		private const int SECONDS_IN_A_MINUTE = 60;

		private const int MAX_TRIES = (SECONDS_IN_A_MINUTE / TIME_BETWEEN_TRIES) * TIMEOUT_IN_MINUTES;

		private DateTime dtLastPrint;

		private int triesLeft;
		private int count = 0;
		private Timer timer;

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

			timer = new Timer();
			timer.Interval = 1000;
			timer.Tick += new EventHandler(Timer_Tick);
			count = TIME_BETWEEN_TRIES;
			timer.Start();
		}

		private void CheckNow_Click(object sender, EventArgs e)
		{
			Countdown.Text = "Checking...";

			count = 0;
			StartChecking();
		}

		private void CheckServer()
		{
			int iLabelSize = PrinterHelper.getPageHeight(Program.settings.printer);

			timer.Stop();
			Countdown.Text = "Checking...";
			Refresh();

			var pj = Util.FetchPrintJob();

			if (pj.jobs.Count > 0) {
				dtLastPrint = DateTime.Now;

				foreach (var j in pj.jobs) {
					Program.SecurityCode = j.securitycode;

					foreach (var e in j.list) { e.securitycode = Program.SecurityCode; }

					if (!Program.settings.useOldDatamaxFormat) {
						PrinterHelper.doPrinting(j.list);
					} else {
						var doprint = new DoPrinting();
						var ms = new MemoryStream();

						if (iLabelSize >= 170 && iLabelSize <= 230)
							doprint.PrintLabels2(ms, j.list);
						else
							doprint.PrintLabels(ms, j.list);
						doprint.FinishUp(ms);
					}

				}

				triesLeft = MAX_TRIES;
			} else {
				triesLeft--;
			}

			count = TIME_BETWEEN_TRIES;
			timer.Start();
		}

		void Timer_Tick(object sender, EventArgs e)
		{
			if (triesLeft <= 0) {
				timer.Stop();
				timer.Dispose();

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
