using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBVaultAddCC : TNBVaultBase
	{
		public TNBVaultAddCC()
		{
			setUserPass();

			nvc.Add( FIELD_FUNCTION, VALUE_FUNCTION_ADD_CUSTOMER );
		}

		public void setCCNumber(string value)
		{
			nvc.Add(FIELD_CC_NUMBER, value);
		}

		public void setCCExp(string value)
		{
			nvc.Add(FIELD_CC_EXP, value);
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
