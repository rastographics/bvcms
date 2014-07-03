using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBTransactionUpdate : TNBTransactionBase
	{
		public TNBTransactionUpdate()
		{
			setUserPass();

			nvc.Add(FIELD_TYPE, TYPE_UPDATE);
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
