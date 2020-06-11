using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Giving.Models
{
    public class GivingPaymentViewModel
    {
        public Guid? paymentMethodId { get; set; }
        public int? peopleId { get; set; }
        public int? paymentTypeId { get; set; }
        public bool isDefault { get; set; }
        public string name { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string nameOnAccount { get; set; }
        public string bankAccount { get; set; }
        public string bankRouting { get; set; }
        public string maskedDisplay { get; set; }
        public string last4 { get; set; }
        public string expiresMonth { get; set; }
        public string expiresYear { get; set; }
        public string cardNumber { get; set; }
        public string cvv { get; set; }
        public int? processTypeId { get; set; }
        public string address { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public string phone { get; set; }
        public string transactionTypeId { get; set; }
        public string emailAddress { get; set; }
        public int? incomingPeopleId { get; set; }
        public bool testing { get; set; }


        public Guid scheduledGiftId { get; set; }
        public int? scheduleTypeId { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }

        public Guid scheduledGiftAmountId { get; set; }
        public int? fundId { get; set; }
        public decimal? amount { get; set; }
    }
}
