using System;
using System.Linq;
using System.Xml.Linq;

namespace CmsData.Finance.Sage.Report
{
    internal class VirtualCheckRejectsResponse
    {
        public string RejectCode { get; private set; }

        public decimal RejectAmount { get; private set; }

        public DateTime RejectDate { get; private set; }

        public string CustomerName { get; private set; }

        public string CustomerNumber { get; private set; }

        public string OriginalTrace { get; private set; }

        public VirtualCheckRejectsResponse(string xmlResponse)
        {
            var xmlDocument = XDocument.Parse(xmlResponse);
            var data = xmlDocument.Descendants("Table1").First();

            RejectCode = data.Element("REJECT_CODE").Value;
            RejectAmount = decimal.Parse(data.Element("REJECT_AMOUNT").Value);
            RejectDate = DateTime.Parse(data.Element("REJECT_DATE").Value);
            CustomerName = data.Element("CUSTOMER_NAME").Value;
            CustomerNumber = data.Element("CUSTOMER_NUMBER").Value;
            OriginalTrace = data.Element("ORIG_TRACE").Value;
        }
    }
}
