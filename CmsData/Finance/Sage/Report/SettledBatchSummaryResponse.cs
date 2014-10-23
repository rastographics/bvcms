using System;
using System.Linq;
using System.Xml.Linq;
using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Report
{
    internal class SettledBatchSummaryResponse
    {
        public int MerchantId { get; private set; }

        public DateTime Date { get; private set; }

        public string Reference { get; private set; }

        public decimal Net { get; private set; }

        public int Count { get; private set; }

        public string File { get; private set; }

        public PaymentMethodType Type { get; private set; }

        public SettledBatchSummaryResponse(string xmlResponse)
        {
            var xmlDocument = XDocument.Parse(xmlResponse);
            var data = xmlDocument.Descendants("Table1").First();

            MerchantId = int.Parse(data.Element("merchant_id").Value);
            Date = DateTime.Parse(data.Element("date").Value);
            Reference = data.Element("reference").Value.Trim();
            Net = decimal.Parse(data.Element("net").Value);
            Count = int.Parse(data.Element("count").Value);
            File = data.Element("file").Value;
            Type = GetPaymentMethodType(data.Element("type").Value);
        }

        private PaymentMethodType GetPaymentMethodType(string type)
        {
            switch (type)
            {
                case "bankcard":
                    return PaymentMethodType.CreditCard;
                case "eft":
                    return PaymentMethodType.Ach;
                default:
                    return PaymentMethodType.Unknown;
            }
        }
    }
}
