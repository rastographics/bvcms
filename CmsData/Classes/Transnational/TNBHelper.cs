using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;

namespace CmsData.Classes.Transnational
{
	class TNBHelper
	{
		//public const string URL = "https://secure.nmi.com/api/transact.php";
		public const string URL = "https://secure.tnbcigateway.com/api/transact.php";

		public static void test()
		{
			TNBVaultAddCC addCC = new TNBVaultAddCC();
			addCC.setCCExp("10/10");
			addCC.setCCNumber("4111111111111111");

			TNBResponse tr = VaultTransaction(addCC);

			Console.WriteLine("Response: " + tr.raw);
		}

		public static TNBResponse Transaction(TNBTransactionBase tvb)
		{
			var wc = new WebClient();
			byte[] responseBytes = wc.UploadValues(URL, tvb.getPostValues());
			string responseString = Encoding.ASCII.GetString(responseBytes);

			return new TNBResponse().populate(HttpUtility.ParseQueryString(responseString), responseString);
		}

		public static TNBResponse VaultTransaction(TNBVaultBase tvb)
		{
			var wc = new WebClient();
			byte[] responseBytes = wc.UploadValues(URL, tvb.getPostValues());
			string responseString = Encoding.ASCII.GetString(responseBytes);

			return new TNBResponse().populate(HttpUtility.ParseQueryString(responseString), responseString);
		}
	}
}
