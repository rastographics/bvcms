using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	public class TNBVaultBase
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

		public const string FIELD_FUNCTION = "customer_vault";
		public const string FIELD_VAULT_ID = "customer_vault_id";

		public const string FIELD_USERNAME = "username";
		public const string FIELD_PASSWORD = "password";

		public const string FIELD_CC_NUMBER = "ccnumber";
		public const string FIELD_CC_EXP = "ccexp";

		public const string FIELD_ACH_NAME = "account_name";
		public const string FIELD_ACH_ACCOUNT = "account";
		public const string FIELD_ACH_ROUTING = "routing";

		public const string FIELD_AMOUNT = "amount";

		public const string FIELD_FIRST_NAME = "first_name";
		public const string FIELD_LAST_NAME = "last_name";
		public const string FIELD_ADDRESS1 = "address1";
		public const string FIELD_CITY = "city";
		public const string FIELD_STATE = "state";
		public const string FIELD_ZIP = "zip";
		public const string FIELD_COUNTRY = "country";
		public const string FIELD_PHONE = "phone";
		public const string FIELD_EMAIL = "email";

		public const string VALUE_FUNCTION_ADD_CUSTOMER = "add_customer";
		public const string VALUE_FUNCTION_UPDATE_CUSTOMER = "update_customer";
		public const string VALUE_FUNCTION_DELETE_CUSTOMER = "delete_customer";
	}
}
