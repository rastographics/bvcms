using System.Linq;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private class PayInfo
        {
            public string PayLink { get; set; }
            public decimal? Amount { get; set; }
            public decimal? AmountPaid { get; set; }
            public decimal? AmountDue { get; set; }
            public string RegisterMail { get; set; }
            public decimal AmountPaidWithSupporters { get; set; }
        }

        private PayInfo pi;

        private PayInfo GetPayInfo(int? orgid, int pid)
        {
            if (orgid == null)
            {
                return null;
            }

            return (
                from m in db.OrganizationMembers
                let ts = db.ViewTransactionSummaries.SingleOrDefault(tt => tt.RegId == m.TranId && tt.PeopleId == m.PeopleId)
                let mt = db.TotalPaid(m.OrganizationId, m.PeopleId)
                where m.PeopleId == pid && m.OrganizationId == orgid
                select new PayInfo
                {
                    PayLink = m.PayLink2(db),
                    Amount = ts.IndAmt,
                    AmountPaid = ts.IndPaid,
                    AmountDue = ts.IndDue,
                    RegisterMail = m.RegisterEmail,
                    AmountPaidWithSupporters = mt.Value
                }
            ).SingleOrDefault();
        }

    }
}
