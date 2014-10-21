using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CmsData.Finance.TransNational.Internal.Query
{
    internal class QueryResponse
    {
        public IEnumerable<Transaction> Transactions { get; private set; }

        public QueryResponse(string response)
        {
            var doc = XDocument.Parse(response);
            Transactions = from item in doc.Descendants("transaction") 
                           select new Transaction(item);
           
        }
    }
}
