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
