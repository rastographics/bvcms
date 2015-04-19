using System;
using System.Collections.Generic;
using CmsWeb.Areas.OnlineReg.Models;

namespace CmsWeb.MobileAPI
{
	public class MobilePostTransaction
	{
		public const int TYPE_UNKNOWN = 0;
		public const int TYPE_BANK_ACCOUNT = 1;
		public const int TYPE_CREDIT_CARD = 2;

		public DateTime date = DateTime.Now;
		public int orgID = 0;
		public int peopleID = 0;

		public int paymentType = TYPE_UNKNOWN;
		public decimal amount = 0;

		public int transactionID = 0;

		public string description;

		public string gateway = OnlineRegModel.GetTransactionGateway();

		public string suffix;
		public string first;
		public string middle;
		public string last;

		public string address;
		public string city;
		public string state;
		public string zip;
		public string phone;
		public string email;

		public List<MobilePostTransactionItem> items;
	}
}