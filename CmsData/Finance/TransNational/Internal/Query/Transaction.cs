using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CmsData.Finance.TransNational.Internal.Query
{
    internal class Transaction
    {
        public string TransactionId { get; private set; }

        public string TransactionType { get; private set; }

        public string Condition { get; private set; }

        public string OrderId { get; private set; }

        public string OrderDescription { get; private set; }

        public string AuthorizationCode { get; private set; }

        public IEnumerable<Action> Actions { get; private set; }

        public Transaction(XElement data)
        {
            TransactionId = data.Element("transaction_id").Value;
            TransactionType = data.Element("transaction_type").Value;
            Condition = data.Element("condition").Value;
            OrderId = data.Element("order_id").Value;
            OrderDescription = data.Element("order_description").Value;
            AuthorizationCode = data.Element("authorization_code").Value;

            Actions = from item in data.Descendants("action")
                      select new Action(item);
        }
    }
}
