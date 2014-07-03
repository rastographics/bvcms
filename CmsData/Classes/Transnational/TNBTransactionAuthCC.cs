using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBTransactionAuthCC : TNBTransactionSaleCC
	{
		public TNBTransactionAuthCC()
		{
			setUserPass();

			nvc.Add(FIELD_TYPE, TYPE_AUTH);
		}
	}
}
