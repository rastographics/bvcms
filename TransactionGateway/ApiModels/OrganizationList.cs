using System.Collections.Generic;

namespace TransactionGateway.ApiModels
{
    public class OrganizationList : BaseResponse
    {
        public IEnumerable<Organization> Items { get; set; }
    }
}
