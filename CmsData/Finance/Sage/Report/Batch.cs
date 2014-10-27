using System;
using System.Xml.Linq;

namespace CmsData.Finance.Sage.Report
{
    internal class Batch
    {
        public int MerchantId { get; private set; }

        public DateTime Date { get; private set; }

        public string Reference { get; private set; }

        public decimal Net { get; private set; }

        public int Count { get; private set; }

        public string File { get; private set; }

        public BatchType Type { get; private set; }

        public Batch(XElement data)
        {
            MerchantId = int.Parse(data.Element("merchant_id").Value);
            Date = DateTime.Parse(data.Element("date").Value);
            Reference = data.Element("reference").Value.Trim();
            Net = decimal.Parse(data.Element("net").Value);
            Count = int.Parse(data.Element("count").Value);
            File = data.Element("file").Value;
            Type = GetBatchType(data.Element("type").Value);
        }

        private BatchType GetBatchType(string type)
        {
            switch (type)
            {
                case "bankcard":
                    return BatchType.CreditCard;
                case "eft":
                    return BatchType.Ach;
                default:
                    return BatchType.Unknown;
            }
        }
    }
}
