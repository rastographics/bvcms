using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBTransactionRefund : TNBTransactionBase
	{
		public TNBTransactionRefund()
		{
			setDemoUserPass();

			nvc.Add(FIELD_TYPE, TYPE_REFUND);
		}

		public void setTransactionID(string value)
		{
			nvc.Add(FIELD_TRANSACTION_ID, value);
		}

		public void setAmount(string value)
		{
			nvc.Add(FIELD_AMOUNT, value);
		}
	}
}
