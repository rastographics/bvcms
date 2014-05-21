using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBVaultACH : TNBVaultBase
	{
		public TNBVaultACH()
		{
			setDemoUserPass();

			nvc.Add( FIELD_FUNCTION, VALUE_FUNCTION_ADD_CUSTOMER );
		}

		public void setACHName(string value)
		{
			nvc.Add(FIELD_ACH_NAME, value);
		}

		public void setACHAccount(string value)
		{
			nvc.Add(FIELD_ACH_ACCOUNT, value);
		}

		public void setACHRouting(string value)
		{
			nvc.Add(FIELD_ACH_ROUTING, value);
		}
	}
}
