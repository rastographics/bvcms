namespace CmsData.Classes.Transnational
{
	class TNBVaultTransaction : TNBVaultBase
	{
		public TNBVaultTransaction()
		{
			setDemoUserPass();
		}

		public void setVaultID(int value)
		{
			nvc.Add(FIELD_VAULT_ID, value.ToString());
		}

		public void setAmount(string value)
		{
			nvc.Add(FIELD_AMOUNT, value);
		}
	}
}
