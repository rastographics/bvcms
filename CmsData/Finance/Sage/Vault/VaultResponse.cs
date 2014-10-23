
using System;
using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Vault
{
    internal class VaultResponse : Response
    {
        public bool Success { get; private set; }
        
        public Guid VaultGuid { get; private set; }

        public VaultResponse(string xmlResponse)
            : base(xmlResponse)
        {
            Success = Data.Element("SUCCESS").Value.Trim() == "true";
            VaultGuid = Guid.Parse(Data.Element("GUID").Value.Trim());
        }
    }
}
