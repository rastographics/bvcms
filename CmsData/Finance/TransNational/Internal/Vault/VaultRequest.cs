using CmsData.Finance.TransNational.Internal.Core;

namespace CmsData.Finance.TransNational.Internal.Vault
{
    internal abstract class VaultRequest : Request
    {
        private const string URL = "https://secure.networkmerchants.com/api/transact.php";

        protected VaultRequest(string userName, string password) : base(URL, userName, password) { }

        public new VaultResponse Execute()
        {
            var response = base.Execute();
            return new VaultResponse(response);
        }
    }
}
