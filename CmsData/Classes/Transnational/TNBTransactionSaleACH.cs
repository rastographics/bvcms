using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBTransactionSaleACH : TNBTransactionBase
	{
		public TNBTransactionSaleACH()
		{
			setDemoUserPass();

			nvc.Add(FIELD_TYPE, TYPE_SALE);
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

		public void setAmount(string value)
		{
			nvc.Add(FIELD_AMOUNT, value);
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

		public void setAddress2(string value)
		{
			nvc.Add(FIELD_ADDRESS2, value);
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

		public void setIPAddress(string value)
		{
			nvc.Add(FIELD_IP, value);
		}
	}
}
