
using CmsData.Finance.TransNational.Native.Core;

namespace CmsData.Finance.TransNational.Native.Vault
{
    internal class CreateAchVaultRequest : VaultRequest
    {
        public CreateAchVaultRequest(string userName, string password, Ach ach) 
            : base(userName, password)
        {
            Data["customer_vault"] = "add_customer";
            Data["method"] = "check";
            ach.SetAchData(Data);
        }
    }
}
