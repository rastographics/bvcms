using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CmsData.Finance.Sage.Report
{
    internal class VirtualCheckRejectsResponse
    {
        public IEnumerable<ReturnedCheck> ReturnedChecks { get; private set; }

        public VirtualCheckRejectsResponse(string xmlResponse)
        {
            var xmlDocument = XDocument.Parse(xmlResponse);
            var xmlReturnedChecks = xmlDocument.Descendants("Table");
            var returnedChecks = xmlReturnedChecks.Select(xmlReturnedCheck => new ReturnedCheck(xmlReturnedCheck)).ToList();

            ReturnedChecks = returnedChecks;
        }
    }
}
