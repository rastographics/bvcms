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
			setDemoUserPass();

			nvc.Add(FIELD_TYPE, TYPE_UPDATE);
		}

		public void setTransactionID(string value)
		{
			nvc.Add(FIELD_TRANSACTION_ID, value);
		}
	}
}
