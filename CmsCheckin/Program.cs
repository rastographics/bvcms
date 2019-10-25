using System;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using CmsCheckin.Dialogs;
using System.Collections.Generic;
using System.Collections;

namespace CmsCheckin
{
	static class Program
	{
		// Settings
		public static List<CheckInSettingsEntry> settingsList = new List<CheckInSettingsEntry>();
		public static List<CheckInCampus> campusList = new List<CheckInCampus>();

		public static Settings settings = new Settings();

		// Usage Varirables
		public static DateTime AdminPINLastAccess { get; set; }
		public static BaseBuildingInfo BuildingInfo { get; set; }
		public static int FamilyId { get; set; }
		public static int PeopleId { get; set; }
		public static string SecurityCode { get; set; }
		public static int? Grade { get; set; }
		public static bool editing { get; set; }
		public static BuildingAddGuests addguests;

		private static bool cursorShowing = true;

		public static BuildingAttendant attendant;
		public static BaseForm baseform;
		public static AttendHome attendHome;
		public static BuildingHome buildingHome;
		public static int MaxLabels { get; set; }
		public static bool JoiningNotAttending { get; set; }

		public static Timer timer;

		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            settings.load();

			AdminPINLastAccess = DateTime.Now.AddYears(-1);

			var login = new Login();
			var loginResults = login.ShowDialog();

			if (loginResults == DialogResult.Cancel)
				return;

			var loginSettings = new LoginSettings();
			var loginSettingsResults = loginSettings.ShowDialog();

			if (loginSettingsResults == DialogResult.Cancel)
				return;

			if (settings.buildingMode) {
				attendant = new BuildingAttendant();
				attendant.Location = settings.attendantLastPosition();

				buildingHome = new BuildingHome();

				baseform = new BaseForm(buildingHome);
				baseform.StartPosition = FormStartPosition.Manual;
				baseform.Location = settings.baseLastPosition();

				if (settings.fullScreen) {
					baseform.WindowState = FormWindowState.Maximized;
					baseform.FormBorderStyle = FormBorderStyle.None;
				} else {
					baseform.FormBorderStyle = FormBorderStyle.FixedSingle;
					baseform.ControlBox = false;
				}

				attendant.StartPosition = FormStartPosition.Manual;
				Application.Run(attendant);
				return;
			}

			if (settings.printMode == "Print From Server") {
				var p = new AttendPrintingServer();
				Application.Run(p);
				return;
			}

			attendHome = new AttendHome();

			baseform = new BaseForm(attendHome);
			baseform.StartPosition = FormStartPosition.Manual;
			baseform.Location = settings.baseLastPosition();

			if (settings.fullScreen) {
				baseform.WindowState = FormWindowState.Maximized;
				baseform.FormBorderStyle = FormBorderStyle.None;
			}

			Application.Run(baseform);
		}

		public static void updateSettingsList(string name, string settings)
		{
			foreach (CheckInSettingsEntry entry in settingsList) {
				if (entry.name == name) {
					entry.settings = settings;
					return;
				}
			}

			settingsList.Add(new CheckInSettingsEntry(name, settings));
		}

		public static CheckInSettingsEntry getSettingsFromList(string name)
		{
			foreach (CheckInSettingsEntry entry in settingsList) {
				if (entry.name == name) {
					return entry;
				}
			}

			return null;
		}

		public static bool TooEarlyOrLate(double earlyhours)
		{
			// Convert from earlyhours to lateminutes
			var lateminutes = -earlyhours * 60;
			return earlyhours > settings.earlyHours || lateminutes > settings.lateMinutes;
		}

		public static bool CheckAdminPIN()
		{
			TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - AdminPINLastAccess.Ticks);

			if (ts.TotalSeconds < settings.adminPINTimeoutNumber) {
				SetAdminLastAccess();
				return true;
			}

			if (Program.settings.adminPIN.Length > 0) {
				PINDialog pd = new PINDialog();
				var results = pd.ShowDialog();

				if (results == DialogResult.OK) {
					if (pd.sPIN == Program.settings.adminPIN) {
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
				var rb = addguests.groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);

				if (rb != null)
					return rb.Tag as PersonInfo;
			}

			return null;
		}

		public static string QueryString
		{
			get { return string.Format("?campus={0}&thisday={1}&kioskmode={2}&kiosk={3}", settings.campusID, settings.dayOfWeek, false, settings.kioskName); }
		}

		public static CheckState ActiveOther(string s)
		{
			return s == bool.TrueString || s == "Checked" ? CheckState.Checked :
			s == bool.FalseString || s == "Unchecked" ? CheckState.Unchecked : CheckState.Indeterminate;
		}

		public static void TimerReset()
		{
			if (!settings.enableTimer)
				return;

			if (timer == null)
				return;

			timer.Stop();
			timer.Start();
		}

		public static void TimerStart(EventHandler t)
		{
			if (!settings.enableTimer)
				return;

			if (timer != null)
				TimerStop();

			timer = new Timer();
			timer.Interval = 60000;
			timer.Tick += t;
			timer.Start();
		}

		public static void TimerStop()
		{
			if (!settings.enableTimer)
				return;

			if (timer == null)
				return;

			timer.Stop();
			timer.Dispose();
			timer = null;
		}

		public static void CursorShow()
		{
			if (cursorShowing)
				return;

			Cursor.Show();
			cursorShowing = true;
		}

		public static void CursorHide()
		{
			if (!Program.settings.hideCursor)
				return;

			if (!cursorShowing)
				return;

			Cursor.Hide();
			cursorShowing = false;
		}

		public static void ClearFields()
		{
			if (baseform.textbox.Parent is AttendHome)
				attendHome.ClearFields();
			else if (baseform.textbox.Parent is BuildingHome)
				buildingHome.ClearFields();
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
