using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBTransactionSaleCC : TNBTransactionBase
	{
		public TNBTransactionSaleCC()
		{
			setDemoUserPass();

			nvc.Add(FIELD_TYPE, TYPE_SALE);
		}

		public void setCCNumber(string value)
		{
			nvc.Add(FIELD_CC_NUMBER, value);
		}

		public void setCCExp(string value)
		{
			nvc.Add(FIELD_CC_EXP, value);
		}

		public void setCCCVV(string value)
		{
			nvc.Add(FIELD_CC_CVV, value);
		}

		public void setAmount(string value)
		{
			nvc.Add(FIELD_AMOUNT, value);
		}
	}
}
