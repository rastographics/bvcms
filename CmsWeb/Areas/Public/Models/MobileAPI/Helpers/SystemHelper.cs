using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Security;
using UtilityExtensions;

namespace CmsWeb.MobileAPI
{
	// Temporary Class for testing
	public static class SystemHelper
	{
		public static bool Authenticate()
		{
			string username = "", password = "";
			var auth = HttpContextFactory.Current.Request.Headers["Authorization"];

			if (auth.HasValue())
			{
				try
				{
					var encoded = Convert.FromBase64String(auth.Substring(6));
					var decoded = Encoding.ASCII.GetString(encoded);
					var cred = decoded.SplitStr(":", 2);
					username = cred[0];
					password = cred[1];
				}
				catch (Exception) { }
			}
			else
			{
				username = HttpContextFactory.Current.Request.Headers["username"];
				password = HttpContextFactory.Current.Request.Headers["password"];
			}

			return Membership.Provider.ValidateUser(username, password); ;
		}
	}
}