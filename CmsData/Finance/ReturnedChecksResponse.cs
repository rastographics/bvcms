using System.Collections.Generic;

namespace CmsData.Finance
{
    public class ReturnedChecksResponse
    {
        public IEnumerable<ReturnedCheck> ReturnedChecks { get; private set; }

        public ReturnedChecksResponse(IEnumerable<ReturnedCheck> returnedChecks)
        {
            ReturnedChecks = returnedChecks;
        }
    }
}