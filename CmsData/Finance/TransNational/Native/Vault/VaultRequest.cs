using CmsData.Finance.TransNational.Native.Core;

namespace CmsData.Finance.TransNational.Native.Vault
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
