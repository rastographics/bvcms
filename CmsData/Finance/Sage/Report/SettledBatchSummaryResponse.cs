using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CmsData.Finance.Sage.Report
{
    internal class SettledBatchSummaryResponse
    {
        public IEnumerable<Batch> Batches { get; private set; }

        public SettledBatchSummaryResponse(string xmlResponse)
        {
            var xmlDocument = XDocument.Parse(xmlResponse);
            var xmlBatches = xmlDocument.Descendants("Table");
            var batches = xmlBatches.Select(xmlBatch => new Batch(xmlBatch)).ToList();

            Batches = batches;
        }
    }
}
