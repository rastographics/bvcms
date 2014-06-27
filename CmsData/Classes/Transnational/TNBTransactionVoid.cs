using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBTransactionVoid : TNBTransactionBase
	{
		public TNBTransactionVoid()
		{
			setDemoUserPass();

			nvc.Add(FIELD_TYPE, TYPE_VOID);
		}

		public string TransactionID
		{
			set
			{
				nvc.Add(FIELD_TRANSACTION_ID, value);
			}
		}
	}
}
