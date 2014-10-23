using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Vault
{
    internal abstract class VaultRequest : Request
    {
        private const string BASE_ADDRESS = "https://gateway.sagepayments.net/web_services/wsVault/wsVault.asmx/";

        protected VaultRequest(string id, string key, string operation) 
            : base(BASE_ADDRESS, operation, id, key) { }

        public new VaultResponse Execute()
        {
            var response = base.Execute();
            return new VaultResponse(response);
        }
    }
}
