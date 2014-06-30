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
			setUserPass();

			nvc.Add(FIELD_TYPE, TYPE_REFUND);
		}

		public string TransactionID
		{
			set
			{
				nvc.Add(FIELD_TRANSACTION_ID, value);
			}
		}

		public string Amount
		{
			set
			{
				nvc.Add(FIELD_AMOUNT, value);
			}
		}
	}
}
