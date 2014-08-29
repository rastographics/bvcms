using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	public class TNBVaultUpdateCC : TNBVaultAddCC
	{
		public TNBVaultUpdateCC()
		{
			setUserPass();

			nvc.Add( FIELD_FUNCTION, VALUE_FUNCTION_UPDATE_CUSTOMER );
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
