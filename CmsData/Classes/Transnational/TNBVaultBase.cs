using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBVaultBase
	{
		protected NameValueCollection nvc = new NameValueCollection();

		protected void setDemoUserPass()
		{
			nvc.Add(FIELD_USERNAME, "demo");
			nvc.Add(FIELD_PASSWORD, "password");
		}

		public void setFirstName( string value )
		{
			nvc.Add(FIELD_FIRST_NAME, value);
		}

		public void setLastName(string value)
		{
			nvc.Add(FIELD_LAST_NAME, value);
		}

		public void setAddress(string value)
		{
			nvc.Add(FIELD_ADDRESS1, value);
		}

		public void setCity(string value)
		{
			nvc.Add(FIELD_CITY, value);
		}

		public void setState(string value)
		{
			nvc.Add(FIELD_STATE, value);
		}

		public void setZip(string value)
		{
			nvc.Add(FIELD_ZIP, value);
		}

		public void setCountry(string value)
		{
			nvc.Add(FIELD_COUNTRY, value);
		}

		public void setPhone(string value)
		{
			nvc.Add(FIELD_PHONE, value);
		}

		public void setEMail(string value)
		{
			nvc.Add(FIELD_EMAIL, value);
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
