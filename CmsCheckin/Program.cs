using System;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using CmsCheckin.Dialogs;

namespace CmsCheckin
{
	static class Program
	{
		// Settings
		public static string Username { get; set; }
		public static string Password { get; set; }
		public static string URL { get; set; }
		public static string Printer { get; set; }
		public static string PrinterWidth { get; set; }
		public static string PrinterHeight { get; set; }
		public static string PrintKiosks { get; set; }
		public static string AdminPassword { get; set; }
		public static string AdminPIN { get; set; }
		public static int AdminPINTimeout { get; set; }
		public static DateTime AdminPINLastAccess { get; set; }
		public static int EarlyCheckinHours { get; set; }
		public static int LateCheckinMinutes { get; set; }
		public static bool HideCursor { get; set; }
		public static bool EnableTimer { get; set; }
		public static bool DisableJoin { get; set; }
		public static bool FullScreen { get; set; }
		public static string KioskName { get; set; }
		public static bool AskEmFriend { get; set; }
		public static bool AskGrade { get; set; }
		public static bool AskChurch { get; set; }
		public static bool AskChurchName { get; set; }
		public static bool AskLabels { get; set; }
		public static bool DisableLocationLabels { get; set; }
		public static bool SecurityLabelPerChild { get; set; }
		public static string PrintMode { get; set; }
		public static bool BuildingMode { get; set; }
		public static string Building { get; set; }
		public static BaseBuildingInfo BuildingInfo { get; set; }

		// Usage Varirables
		public static int FamilyId { get; set; }
		public static int PeopleId { get; set; }
		public static int CampusId { get; set; }
		public static string SecurityCode { get; set; }
		public static int ThisDay { get; set; }
		public static int? Grade { get; set; }
		public static bool editing { get; set; }
		public static BuildingAddGuests addguests;

		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			AdminPINLastAccess = DateTime.Now.AddYears(-1);

			var start = new Login();
			start.ShowDialog();

			var login = new LoginSettings();

			if (!Util.IsDebug())
				login.FullScreen.Checked = true;

			login.password.Focus();

			var r = login.ShowDialog();
			if (r == DialogResult.Cancel)
				return;

			PrintMode = login.PrintMode.Text;
			PrintKiosks = login.PrintKiosks.Text;
			Printer = login.Printer.Text;
			DisableLocationLabels = login.DisableLocationLabels.Checked;
			BuildingMode = login.BuildingAccessMode.Checked;
			FullScreen = login.FullScreen.Checked;

			BaseForm b;
			if (BuildingMode) {
				attendant = new BuildingAttendant();
				attendant.Location = new Point(Settings1.Default.AttendantLocX, Settings1.Default.AttendantLocY);
				home2 = new BuildingHome();
				b = new BaseForm(home2);
				b.StartPosition = FormStartPosition.Manual;
				b.Location = new Point(Settings1.Default.BaseFormLocX, Settings1.Default.BaseFormLocY);
				baseform = b;
				if (FullScreen) {
					b.WindowState = FormWindowState.Maximized;
					b.FormBorderStyle = FormBorderStyle.None;
				} else {
					b.FormBorderStyle = FormBorderStyle.FixedSingle;
					b.ControlBox = false;
				}
				attendant.StartPosition = FormStartPosition.Manual;
				Application.Run(attendant);
				return;
			}

			var f = new LoginSettingsMore { campuses = login.campuses };
			var ret = f.ShowDialog();
			if (ret == DialogResult.Cancel)
				return;

			AdminPassword = f.AdminPassword;
			CampusId = f.CampusId;
			ThisDay = f.DayOfWeek;
			HideCursor = f.HideCursor.Checked;
			AskGrade = f.AskGrade.Checked;
			AskLabels = false;
			EarlyCheckinHours = int.Parse(f.LeadHours.Text);
			LateCheckinMinutes = int.Parse(f.LateMinutes.Text);
			AskEmFriend = f.AskEmFriend.Checked;
			KioskName = f.KioskName.Text;
			AskChurch = f.AskChurch.Checked;
			AskChurchName = f.AskChurchName.Checked;
			EnableTimer = f.EnableTimer.Checked;
			DisableJoin = f.DisableJoin.Checked;
			SecurityLabelPerChild = f.SecurityLabelPerChild.Checked;

			f.Dispose();

			if (PrintMode == "Print From Server") {
				var p = new AttendPrintingServer();
				Application.Run(p);
				return;
			}

			home = new AttendHome();
			b = new BaseForm(home);
			baseform = b;
			b.StartPosition = FormStartPosition.Manual;
			b.Location = new Point(Settings1.Default.BaseFormLocX, Settings1.Default.BaseFormLocY);

			if (FullScreen) {
				b.WindowState = FormWindowState.Maximized;
				b.FormBorderStyle = FormBorderStyle.None;
			}
			Application.Run(b);
		}

		public static bool TooEarlyOrLate(double earlyhours)
		{
			// Convert from earlyhours to lateminutes
			var lateminutes = -earlyhours * 60;
			return earlyhours > EarlyCheckinHours || lateminutes > LateCheckinMinutes;
		}

		public static bool CheckAdminPIN()
		{
			TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - AdminPINLastAccess.Ticks);

			if (ts.TotalSeconds < AdminPINTimeout) {
				SetAdminLastAccess();
				return true;
			}

			if (Program.AdminPIN.Length > 0) {
				PINDialog pd = new PINDialog();
				var results = pd.ShowDialog();

				if (results == DialogResult.OK) {
					if (pd.sPIN == Program.AdminPIN) {
						SetAdminLastAccess();
						return true;
					} else
						return false;
				} else
					return false;
			} else {
				return true;
			}
		}

		public static void SetAdminLastAccess()
		{
			AdminPINLastAccess = DateTime.Now;
		}

		public static PersonInfo GuestOf()
		{
			if (addguests != null) {
				var rb = addguests.groupBox1.Controls.OfType<RadioButton>()
					 .FirstOrDefault(r => r.Checked);
				if (rb != null)
					return rb.Tag as PersonInfo;
			}
			return null;
		}

		public static string QueryString
		{
			get { return string.Format("?campus={0}&thisday={1}&kioskmode={2}&kiosk={3}", CampusId, ThisDay, false, KioskName); }
		}
		public static CheckState ActiveOther(string s)
		{
			return s == bool.TrueString || s == "Checked" ? CheckState.Checked :
			s == bool.FalseString || s == "Unchecked" ? CheckState.Unchecked : CheckState.Indeterminate;
		}
		public static BuildingAttendant attendant;
		public static BaseForm baseform;
		public static AttendHome home;
		public static BuildingHome home2;
		public static int MaxLabels { get; set; }
		public static bool JoiningNotAttending { get; set; }

		public static Timer timer1;
		public static Timer timer2;

		public static void Timer2Reset()
		{
			if (!EnableTimer)
				return;
			if (timer2 == null)
				return;
			timer2.Stop();
			timer2.Start();
		}
		public static void Timer2Start(EventHandler t)
		{
			if (!EnableTimer)
				return;
			if (timer2 != null)
				Timer2Stop();
			timer2 = new Timer();
			timer2.Interval = 60000;
			timer2.Tick += t;
			timer2.Start();
		}
		public static void Timer2Stop()
		{
			if (timer2 == null)
				return;
			timer2.Stop();
			timer2.Dispose();
			timer2 = null;
		}
		public static void TimerReset()
		{
			if (!EnableTimer)
				return;
			if (timer1 == null)
				return;
			timer1.Stop();
			timer1.Start();
		}
		public static void TimerStart(EventHandler t)
		{
			if (!EnableTimer)
				return;
			if (timer1 != null)
				TimerStop();
			timer1 = new Timer();
			timer1.Interval = 60000;
			timer1.Tick += t;
			timer1.Start();
		}
		public static void TimerStop()
		{
			if (!EnableTimer)
				return;
			if (timer1 == null)
				return;
			timer1.Stop();
			timer1.Dispose();
			timer1 = null;
		}
		private static bool showing = true;
		public static void CursorShow()
		{
			if (showing)
				return;
			Cursor.Show();
			showing = true;
		}
		public static void CursorHide()
		{
			if (!Program.HideCursor)
				return;
			if (!showing)
				return;
			Cursor.Hide();
			showing = false;
		}
		public static void ClearFields()
		{
			if (baseform.textbox.Parent is AttendHome)
				home.ClearFields();
			else if (baseform.textbox.Parent is BuildingHome)
				home2.ClearFields();
		}
		private delegate void SetPropertyThreadSafeDelegate<TResult>(Control @this, Expression<Func<TResult>> property, TResult value);

		public static void SetPropertyThreadSafe<TResult>(this Control @this, Expression<Func<TResult>> property, TResult value)
		{
			var propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;

			if (propertyInfo == null ||
				//!@this.GetType().IsSubclassOf(propertyInfo.ReflectedType) ||
				 @this.GetType().GetProperty(propertyInfo.Name, propertyInfo.PropertyType) == null) {
				throw new ArgumentException("The lambda expression 'property' must reference a valid property on this Control.");
			}

			if (@this.InvokeRequired) {
				@this.Invoke(new SetPropertyThreadSafeDelegate<TResult>(SetPropertyThreadSafe), new object[] { @this, property, value });
			} else {
				@this.GetType().InvokeMember(propertyInfo.Name, BindingFlags.SetProperty, null, @this, new object[] { value });
			}
		}
	}
}
