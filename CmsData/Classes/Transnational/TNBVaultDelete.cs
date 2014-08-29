using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	public class TNBVaultDelete : TNBVaultBase
	{
		public TNBVaultDelete()
		{
			setUserPass();

			nvc.Add( FIELD_FUNCTION, VALUE_FUNCTION_DELETE_CUSTOMER );
		}

		public string VaultID
		{
			set
			{
				nvc.Add(FIELD_VAULT_ID, value);
			}
		}
	}
}
