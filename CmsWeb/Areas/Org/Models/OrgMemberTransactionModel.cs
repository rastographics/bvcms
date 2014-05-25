using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgMemberTransactionModel
    {
        private int? orgId;
        private int? peopleId;
        private OrganizationMember om;
        public CmsData.View.TransactionSummary TransactionSummary;
        private void Populate()
        {
            var q = from mm in DbUtil.Db.OrganizationMembers
                    let ts = DbUtil.Db.ViewTransactionSummaries.SingleOrDefault(tt => tt.RegId == mm.TranId)
                    where mm.OrganizationId == OrgId && mm.PeopleId == PeopleId
                    select new
                    {
                        mm.Person.Name,
                        mm.Organization.OrganizationName,
                        om = mm,
                        ts
                    };
            var i = q.SingleOrDefault();
            if (i == null)
                return;
            Name = i.Name;
            OrgName = i.OrganizationName;
            om = i.om;
            TransactionSummary = i.ts;
        }

        public int? OrgId
        {
            get { return orgId; }
            set
            {
                orgId = value;
                if (peopleId.HasValue)
                    Populate();
            }
        }
        public int? PeopleId
        {
            get { return peopleId; }
            set
            {
                peopleId = value;
                if (orgId.HasValue)
                    Populate();
            }
        }
        public string Name { get; set; }
        public string OrgName { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Payment { get; set; }

        internal void PostTransaction()
        {
            var reason = TransactionSummary == null
                ? "Inital Tran"
                : "Adjustment";
            om.AddTransaction(DbUtil.Db, reason, Payment ?? 0, Amount);
        }

    }
}
