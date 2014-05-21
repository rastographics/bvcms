using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;

namespace CmsWeb.MobileAPI
{
	public class MobilePaymentInfo
	{
		public const int TYPE_UNKNOWN = 0;
		public const int TYPE_BANK_ACCOUNT = 1;
		public const int TYPE_CREDIT_CARD = 2;

		public int type = TYPE_UNKNOWN;
		public string masked = "";

		public MobilePaymentInfo populate(PaymentInfo pi, int newType )
		{
			type = newType;

			switch( newType )
			{
				case TYPE_BANK_ACCOUNT:
					masked = pi.MaskedAccount;
					break;

				case TYPE_CREDIT_CARD:
					masked = pi.MaskedCard;
					break;
			}

			return this;
		}
	}
}