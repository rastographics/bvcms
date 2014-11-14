
using CmsData.Finance.TransNational.Core;

namespace CmsData.Finance.TransNational.Vault
{
    internal class CreateAchVaultRequest : VaultRequest
    {
        public CreateAchVaultRequest(string userName, string password, Ach ach) 
            : base(userName, password)
        {
            Data["customer_vault"] = "add_customer";
            Data["method"] = "check";
            Data["sec_code"] = "WEB";
            ach.SetAchData(Data);
        }
    }
}
