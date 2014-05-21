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
	}
}
