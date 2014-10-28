using System;
using System.Xml.Linq;

namespace CmsData.Finance.Sage.Report
{
    internal class Transaction
    {
        public string Reference { get; private set; }

        public string OrderNumber { get; private set; }

        public string Name { get; private set; }

        public decimal TotalAmount { get; private set; }

        public TransactionType TransactionType { get; private set; }

        public bool Approved { get; private set; }

        public string Message { get; private set; }
        
        public DateTime Date { get; private set; }

        public DateTime SettleDate { get; private set; }

        public Transaction(XElement data)
        {
            Reference = data.Element("reference").Value.Trim();
            OrderNumber = data.Element("order_number").Value.Trim();
            Name = data.Element("name").Value.Trim();
            TotalAmount = decimal.Parse(data.Element("total_amount").Value.Trim());
            TransactionType = GetTransactionType(data.Element("transaction_code").Value.Trim());
            Approved = bool.Parse(data.Element("approved").Value.Trim());
            Message = data.Element("message").Value.Trim();
            Date = DateTime.Parse(data.Element("date").Value);
            SettleDate = DateTime.Parse(data.Element("settle_date").Value);
        }

        private TransactionType GetTransactionType(string transactionCode)
        {
            switch (transactionCode)
            {
                case "1":
                    return TransactionType.Sale;
                case "2":
                    return TransactionType.AuthOnly;
                case "3":
                    return TransactionType.ForceAuthSale;
                case "4":
                    return TransactionType.Void;
                case "6":
                    return TransactionType.Credit;
                default:
                    return TransactionType.Unknown;
            }
        }
    }
}
