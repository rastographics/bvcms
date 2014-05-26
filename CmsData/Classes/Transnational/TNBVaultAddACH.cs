using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBVaultAddACH : TNBVaultBase
	{
		public TNBVaultAddACH()
		{
			setUserPass();

			nvc.Add(FIELD_FUNCTION, VALUE_FUNCTION_ADD_CUSTOMER);
		}

		public void setACHName(string value)
		{
			nvc.Add(FIELD_ACH_NAME, value);
		}

		public void setACHAccount(string value)
		{
			nvc.Add(FIELD_ACH_ACCOUNT, value);
		}

		public void setACHRouting(string value)
		{
			nvc.Add(FIELD_ACH_ROUTING, value);
		}

		public void setFirstName(string value)
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
	}
}
