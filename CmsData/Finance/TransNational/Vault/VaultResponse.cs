
using CmsData.Finance.TransNational.Core;

namespace CmsData.Finance.TransNational.Vault
{
    internal class VaultResponse : Response
    {
        public string VaultId { get; private set; }

        public VaultResponse(string response) 
            : base(response)
        {
            VaultId = Data["customer_vault_id"];
        }
    }
}
