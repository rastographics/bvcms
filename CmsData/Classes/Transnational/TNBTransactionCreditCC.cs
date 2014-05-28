using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBTransactionCreditCC : TNBTransactionSaleCC
	{
		public TNBTransactionCreditCC()
		{
			setDemoUserPass();

			nvc.Add(FIELD_TYPE, TYPE_CREDIT);
		}
	}
}
