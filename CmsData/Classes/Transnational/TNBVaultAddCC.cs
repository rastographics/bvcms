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

			nvc.Add(FIELD_FUNCTION, VALUE_FUNCTION_ADD_CUSTOMER);
		}

		public string CCNumber
		{
			set
			{
				nvc.Add(FIELD_CC_NUMBER, value);
			}
		}

		public string CCExp
		{
			set
			{
				nvc.Add(FIELD_CC_EXP, value);
			}
		}

		public string FirstName
		{
			set
			{
				nvc.Add(FIELD_FIRST_NAME, value);
			}
		}

		public string LastName
		{
			set
			{
				nvc.Add(FIELD_LAST_NAME, value);
			}
		}

		public string Address
		{
			set
			{
				nvc.Add(FIELD_ADDRESS1, value);
			}
		}

		public string City
		{
			set
			{
				nvc.Add(FIELD_CITY, value);
			}
		}

		public string State
		{
			set
			{
				nvc.Add(FIELD_STATE, value);
			}
		}

		public string Zip
		{
			set
			{
				nvc.Add(FIELD_ZIP, value);
			}
		}

		public string Country
		{
			set
			{
				nvc.Add(FIELD_COUNTRY, value);
			}
		}

		public string Phone
		{
			set
			{
				nvc.Add(FIELD_PHONE, value);
			}
		}

		public string EMail
		{
			set
			{
				nvc.Add(FIELD_EMAIL, value);
			}
		}
	}
}
