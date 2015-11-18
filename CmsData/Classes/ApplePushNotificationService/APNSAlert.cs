using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.ApplePushNotificationService
{
	public class APNSAlert
	{
		public string title = "";
		public string body = "";

		public APNSAlert(string title, string body)
		{
			this.title = title;
			this.body = body;
		}
	}
}
