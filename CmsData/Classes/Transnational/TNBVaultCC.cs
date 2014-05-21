using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBVaultCC : TNBVaultBase
	{
		public TNBVaultCC()
		{
			setDemoUserPass();

			nvc.Add( FIELD_FUNCTION, VALUE_FUNCTION_ADD_CUSTOMER );
		}

		public void setCCNumber(string value)
		{
			nvc.Add(FIELD_CC_NUMBER, value);
		}

		public void setCCExp(string value)
		{
			nvc.Add(FIELD_CC_EXP, value);
		}
	}
}
