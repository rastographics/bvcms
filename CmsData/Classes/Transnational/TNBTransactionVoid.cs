using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	public class TNBTransactionVoid : TNBTransactionBase
	{
		public TNBTransactionVoid()
		{
			setUserPass();

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
