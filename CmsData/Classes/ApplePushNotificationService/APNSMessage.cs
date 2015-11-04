using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.ApplePushNotificationService
{
	public class APNSMessage
	{
		private Dictionary<string, Object> aps = new Dictionary<string, Object>();

		public void addAlert(APNSAlert alert)
		{
			aps.Add("alert", alert);
		}

		public void addBadgeCount(int count)
		{
			aps.Add("badge", count);
		}

		public void addSound(string sound)
		{
			aps.Add("sound", sound);
		}

		public void addContentAvailable()
		{
			aps.Add("content-available", 0);
		}

		public void addCommand(int command)
		{
			aps.Add("command", command);
		}

		public Dictionary<string, Object> getDictionary()
		{
			return aps;
		}
	}
}
