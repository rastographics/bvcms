using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UtilityExtensions;

namespace CmsData.Finance.TransNational.Query
{
    internal class Transaction
    {
        public string TransactionId { get; private set; }

        public TransactionType TransactionType { get; private set; }

        public string Condition { get; private set; }

        public string OrderId { get; private set; }

        public string OrderDescription { get; private set; }

        public string Name { get; private set; }

        public string AuthorizationCode { get; private set; }

        public IEnumerable<Action> Actions { get; private set; }

        public string LastDigits { get; set; }

        public Transaction(XElement data)
        {
            TransactionId = data.Element("transaction_id").Value;
            TransactionType = GetTransactionType(data.Element("transaction_type").Value);
            Condition = data.Element("condition").Value;
            OrderId = data.Element("order_id").Value;
            OrderDescription = data.Element("order_description").Value;

            if (TransactionType == TransactionType.CreditCard)
            {
                Name = string.Format("{0} {1}", data.Element("first_name").Value, data.Element("last_name").Value);
                LastDigits = data.Element("cc_number").Value.Last(4);
            }
            else
            {
                Name = data.Element("check_name").Value;
                LastDigits = data.Element("check_account").Value.Last(4);
            }

            AuthorizationCode = data.Element("authorization_code").Value;

            Actions = from item in data.Descendants("action")
                      select new Action(item);
        }

        private TransactionType GetTransactionType(string transactionType)
        {
            switch (transactionType)
            {
                case "cc":
                    return TransactionType.CreditCard;
                case "ck":
                    return TransactionType.Ach;
                default:
                    return TransactionType.Unknown;
            }
        }
    }
}
