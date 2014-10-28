using System;
using System.Net;

namespace CmsData.Finance.Sage.Vault
{
    internal class DeleteVaultRequest : VaultRequest
    {
        public DeleteVaultRequest(string id, string key, Guid vaultGuid)
            : base(id, key, "DELETE_DATA")
        {
            Data["GUID"] = vaultGuid.ToString().Replace("-", "");
        }

        public new bool Execute()
        {
            using (var client = new WebClient())
            {
                client.BaseAddress = BaseAddress;
                var result = client.UploadValues(Operation, "POST", Data);

                //TODO: There seems to be a bug with Sage on deleting a vault. It always returns true (even when it shouldn't) in some wierd boolean xml node, so for now I just always return true.
                return true;  
            }
        }
    }
}
