using System;
using System.Linq;
using System.Web;
using CmsData;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class OrgMembersUpdate
    {
        public OrgMembersUpdate()
        {
            MemberType = new CodeInfo(0, "MemberType");
            OrgName = (string)HttpContext.Current.Session["ActiveOrganization"];
            if (DbUtil.Db.CurrentOrg.GroupSelect == GroupSelectCode.Pending)
                Pending = true;
        }
        private int? id;
        public int? Id
        {
            get
            {
                if (!id.HasValue)
                    id = DbUtil.Db.CurrentOrg.Id;
                if (id != DbUtil.Db.CurrentOrg.Id)
                    throw new Exception("Current org has changed from {0} to {1}, aborting".Fmt(id, DbUtil.Db.CurrentOrgId0));
                return id;
            }
            set { id = value; }
        }

        private int? count;

        public int Count
        {
            get
            {
                if (!count.HasValue)
                    count = People(DbUtil.Db.CurrentOrg).Count();
                return count.Value;
            }
        }

        public string OrgName;
        public string Group
        {
            get
            {
                switch (DbUtil.Db.CurrentOrg.GroupSelect)
                {
                    case GroupSelectCode.Member:
                        return "Current Members";
                        break;
                    case GroupSelectCode.Inactive:
                        return "Inactive Members";
                        break;
                    case GroupSelectCode.Pending:
                        return "Pending Members";
                        break;
                    case GroupSelectCode.Prospect:
                        return "Prospects";
                        break;
                }
                return "People";
            }
        }

        public CodeInfo MemberType { get; set; }
        public DateTime? InactiveDate { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public bool MakeMemberTypeOriginal { get; set; }
        public bool Pending { get; set; }
        public bool RemoveFromEnrollmentHistory { get; set; }
        public bool RemoveInactiveDate { get; set; }
        public DateTime? DropDate { get; set; }

        public IQueryable<OrgPerson> People(ICurrentOrg co)
        {
            var q = from p in DbUtil.Db.OrgPeople(Id, co.GroupSelect,
                        co.First(), co.Last(), co.SgFilter, co.ShowHidden,
                        Util2.CurrentTag, Util2.CurrentTagOwnerId,
                        co.FilterIndividuals, co.FilterTag, false, Util.UserPeopleId)
                    select p;
            return q;
        }

        public void Drop()
        {
            var pids = (from p in People(DbUtil.Db.CurrentOrg) select p.PeopleId).ToList();
            foreach (var pid in pids)
            {
                DbUtil.DbDispose();
                DbUtil.Db = new CMSDataContext(Util.ConnectionString);
                var om = DbUtil.Db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == Id);
                if(DropDate.HasValue)
        			om.Drop(DbUtil.Db, DropDate.Value);
                else
        			om.Drop(DbUtil.Db);
                DbUtil.Db.SubmitChanges();
                if (RemoveFromEnrollmentHistory)
                {
                    DbUtil.DbDispose();
                    DbUtil.Db = new CMSDataContext(Util.ConnectionString);
                    var q = DbUtil.Db.EnrollmentTransactions.Where(tt => tt.OrganizationId == Id && tt.PeopleId == pid);
                    DbUtil.Db.EnrollmentTransactions.DeleteAllOnSubmit(q);
                    DbUtil.Db.SubmitChanges();
                }
            }
        }
        public void Update()
        {
            var pids = (from p in People(DbUtil.Db.CurrentOrg) select p.PeopleId).ToList();
            foreach (var pid in pids)
            {
                DbUtil.DbDispose();
                DbUtil.Db = new CMSDataContext(Util.ConnectionString);
                var om = DbUtil.Db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == Id);

                if (InactiveDate.HasValue)
                    om.InactiveDate = InactiveDate;
                if (RemoveInactiveDate)
                    om.InactiveDate = null;

                if (EnrollmentDate.HasValue)
                    om.EnrollmentDate = EnrollmentDate;

				om.Pending = Pending;

                if (MemberType.Value != "0")
                    om.MemberTypeId = MemberType.Value.ToInt();

				if (MakeMemberTypeOriginal)
				{
					var et = (from e in DbUtil.Db.EnrollmentTransactions
							  where e.PeopleId == om.PeopleId
							  where e.OrganizationId == Id
							  orderby e.TransactionDate
							  select e).First();
					et.MemberTypeId = om.MemberTypeId;
				}

                DbUtil.Db.SubmitChanges();
            }
        }
    }
}