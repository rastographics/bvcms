using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBTransactionSaleCC : TNBTransactionBase
	{
		public TNBTransactionSaleCC()
		{
			setUserPass();

			nvc.Add(FIELD_TYPE, TYPE_SALE);
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

		public string CCCVV
		{
			set
			{
				nvc.Add(FIELD_CC_CVV, value);
			}
		}

		public string Amount
		{
			set
			{
				nvc.Add(FIELD_AMOUNT, value);
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

		public string Address2
		{
			set
			{
				nvc.Add(FIELD_ADDRESS2, value);
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

		public string IPAddress
		{
			set
			{
				nvc.Add(FIELD_IP, value);
			}
		}
	}
}
