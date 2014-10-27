using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CmsData.Finance.Sage.Report
{
    internal class SettledBatchListingResponse
    {
        public IEnumerable<Transaction> Transactions { get; private set; }

        public SettledBatchListingResponse(string xmlResponse)
        {
            var xmlDocument = XDocument.Parse(xmlResponse);
            var xmlTransactions = xmlDocument.Descendants("Table");
            var transactions = xmlTransactions.Select(xmlTransaction => new Transaction(xmlTransaction)).ToList();

            Transactions = transactions;
        }
    }
}
