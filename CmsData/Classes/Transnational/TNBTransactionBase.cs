using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	public class TNBTransactionBase
	{
        public bool testing { get; set;  }

		protected NameValueCollection nvc = new NameValueCollection();

		protected void setUserPass()
		{
			if (testing)
			{
				nvc.Add(FIELD_USERNAME, "demo");
				nvc.Add(FIELD_PASSWORD, "password");
			}
			else
			{
				nvc.Add(FIELD_USERNAME, DbUtil.Db.Setting("TNBUsername", ""));
				nvc.Add(FIELD_PASSWORD, DbUtil.Db.Setting("TNBPassword", ""));
			}
		}

		public NameValueCollection getPostValues()
		{
			return nvc;
		}

		// -- FIELDS --
		public const string FIELD_TYPE = "type";
		public const string FIELD_TRANSACTION_ID = "transactionid";

		public const string FIELD_USERNAME = "username";
		public const string FIELD_PASSWORD = "password";

		public const string FIELD_CC_NUMBER = "ccnumber";
		public const string FIELD_CC_EXP = "ccexp";
		public const string FIELD_CC_CVV = "cvv";

		public const string FIELD_ACH_NAME = "checkname";
		public const string FIELD_ACH_ACCOUNT = "checkaccount";
		public const string FIELD_ACH_ROUTING = "checkaba";
		public const string FIELD_ACH_HOLDER_TYPE = "account_holder_type"; // Business or personal
		public const string FIELD_ACH_TYPE = "account_type"; // Checking or savings

		public const string FIELD_AMOUNT = "amount";

		public const string FIELD_IP = "ipaddress";
		public const string FIELD_ORDER_ID = "orderid";
		public const string FIELD_ORDER_DESCRIPTION = "orderdescription";
		public const string FIELD_PO_NUMBER = "ponumber";

		public const string FIELD_FIRST_NAME = "first_name";
		public const string FIELD_LAST_NAME = "last_name";
		public const string FIELD_ADDRESS1 = "address1";
		public const string FIELD_ADDRESS2 = "address2";
		public const string FIELD_CITY = "city";
		public const string FIELD_STATE = "state";
		public const string FIELD_ZIP = "zip";
		public const string FIELD_COUNTRY = "country";
		public const string FIELD_PHONE = "phone";
		public const string FIELD_EMAIL = "email";
		// -- END FIELDS --

		// -- TYPES --
		public const string TYPE_SALE = "sale";
		public const string TYPE_AUTH = "auth";
		public const string TYPE_CAPTURE = "capture";
		public const string TYPE_VOID = "void";
		public const string TYPE_REFUND = "refund";
		public const string TYPE_CREDIT = "credit";
		public const string TYPE_UPDATE = "update";

		public const string TYPE_ACH_HOLDER_BUSINESS = "business";
		public const string TYPE_ACH_HOLDER_PERSONAL = "personal";

		public const string TYPE_ACH_CHECKING = "checking";
		public const string TYPE_ACH_SAVINGS = "savings";
		// -- END TYPES --
	}
}
