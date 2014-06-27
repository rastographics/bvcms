using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBTransactionCapture : TNBTransactionBase
	{
		public TNBTransactionCapture()
		{
			setDemoUserPass();

			nvc.Add(FIELD_TYPE, TYPE_CAPTURE);
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
