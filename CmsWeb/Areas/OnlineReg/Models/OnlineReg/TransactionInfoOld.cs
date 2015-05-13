using System;

// todo: This class is here to remain compatible with some older data that was serialized and saved in some table
namespace CmsWeb.Areas.OnlineReg.Models
{
    [Serializable]
    public class TransactionInfo
    {
        public string Header { get; set; }
        public string URL { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
        public string Participants { get; set; }
        public bool testing { get; set; }
        public int orgid { get; set; }
        public PeopleInfo[] people { get; set; }
        public class PeopleInfo
        {
            public int pid { get; set; }
            public string name { get; set; }
            public decimal amt { get; set; }
        }
    }
}
