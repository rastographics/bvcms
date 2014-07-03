namespace CmsData.Classes.Transnational
{
	class TNBVaultTransaction : TNBVaultBase
	{
		public TNBVaultTransaction()
		{
			setUserPass();
		}

		public string VaultID
		{
			set
			{
				nvc.Add(FIELD_VAULT_ID, value.ToString());
			}
		}

		public string Amount
		{
			set
			{
				nvc.Add(FIELD_AMOUNT, value);
			}
		}
	}
}
