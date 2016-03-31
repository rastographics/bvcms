using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class OrgMembersUpdate : IValidatableObject
    {
        public OrgMembersUpdate()
        {
            MemberType = new CodeInfo(0, "MemberType");
        }
        private int? id;
        public int? Id
        {
            get
            {
                if (!id.HasValue)
                {
//                    if (DbUtil.Db.CurrentOrg == null)
//                        throw new Exception("Current org no longer exists, aborting");
                    id = DbUtil.Db.CurrentOrgId0;
                }
                return id;
            }
            set
            {
                id = value;
                if (id > 0)
                {
                    OrgName = DbUtil.Db.LoadOrganizationById(id).OrganizationName;
                    if (DbUtil.Db.CurrentOrg.GroupSelect == GroupSelectCode.Pending)
                        Pending = true;
                }
            }
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

        public decimal? Amount { get; set; }
        public decimal? Payment { get; set; }
        public bool AdjustFee { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        public string Group
        {
            get
            {
                switch (DbUtil.Db.CurrentOrg.GroupSelect)
                {
                    case GroupSelectCode.Member:
                        return "Current Members";
                    case GroupSelectCode.Inactive:
                        return "Inactive Members";
                    case GroupSelectCode.Pending:
                        return "Pending Members";
                    case GroupSelectCode.Prospect:
                        return "Prospects";
                }
                return "People";
            }
        }

        public CodeInfo MemberType { get; set; }
        public DateTime? InactiveDate { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public bool MakeMemberTypeOriginal { get; set; }
        public bool Pending { get; set; }
        public bool RemoveInactiveDate { get; set; }
        public string NewGroup { get; set; }

        public IQueryable<OrgPerson> People(ICurrentOrg co)
        {
            var q = from p in DbUtil.Db.OrgPeople(id, co.GroupSelect,
                        co.First(), co.Last(), co.SgFilter, co.ShowHidden,
                        Util2.CurrentTagName, Util2.CurrentTagOwnerId,
                        co.FilterIndividuals, co.FilterTag, false, Util.UserPeopleId)
                    select p;
            return q;
        }

        public void Update()
        {
            var pids = (from p in People(DbUtil.Db.CurrentOrg) select p.PeopleId).ToList();
            foreach (var pid in pids)
            {
                DbUtil.DbDispose();
                DbUtil.Db = DbUtil.Create(Util.Host);
                var om = DbUtil.Db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == Id);

                var changes = new List<ChangeDetail>();
                if (InactiveDate.HasValue)
                    om.UpdateValue(changes, "InactiveDate", InactiveDate);
                if (RemoveInactiveDate)
                    om.UpdateValue(changes, "InactiveDate", null);

                if (EnrollmentDate.HasValue)
                    om.UpdateValue(changes, "EnrollmentDate", EnrollmentDate);

                om.Pending = Pending;

                if (MemberType.Value != "0")
                    om.UpdateValue(changes, "MemberTypeId", MemberType.Value.ToInt());

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
                foreach(var g in changes)
                    DbUtil.LogActivity("OrgMem change " + g.Field, om.OrganizationId, om.PeopleId);
            }
        }

        public string AddSmallGroup(int sgtagid)
        {
            var pids = (from p in People(DbUtil.Db.CurrentOrg) select p.PeopleId).ToList();
            var n = 0;
            var name = DbUtil.Db.MemberTags.Single(mm => mm.Id == sgtagid && mm.OrgId == Id).Name;
            foreach (var pid in pids)
            {
                DbUtil.DbDispose();
                DbUtil.Db = DbUtil.Create(Util.Host);
                var om = DbUtil.Db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == Id);
                var nn = om.AddToGroup(DbUtil.Db, sgtagid);
                n += nn;
                if(nn == 1)
                    DbUtil.LogActivity("OrgMem AddSubGroup " + name, om.OrganizationId, om.PeopleId);
            }
            return $"{n} added to sub-group {name}";
        }

        public void RemoveSmallGroup(int sgtagid)
        {
            var pids = (from p in People(DbUtil.Db.CurrentOrg) select p.PeopleId).ToList();
            var name = DbUtil.Db.MemberTags.Single(mm => mm.Id == sgtagid && mm.OrgId == Id).Name;
            foreach (var pid in pids)
            {
                DbUtil.DbDispose();
                DbUtil.Db = DbUtil.Create(Util.Host);
                var om = DbUtil.Db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == Id);
                var mt = om.OrgMemMemTags.SingleOrDefault(t => t.MemberTagId == sgtagid);
                if (mt != null)
                    DbUtil.Db.OrgMemMemTags.DeleteOnSubmit(mt);
                DbUtil.Db.SubmitChanges();
                if(mt != null)
                    DbUtil.LogActivity("OrgMem RemoveSubGroup " + name, om.OrganizationId, om.PeopleId);
            }
            DbUtil.Db = DbUtil.Create(Util.Host);
            DbUtil.Db.ExecuteCommand(@"
DELETE dbo.MemberTags
WHERE Id = {1} AND OrgId = {0}
AND NOT EXISTS(SELECT NULL FROM dbo.OrgMemMemTags WHERE OrgId = {0} AND MemberTagId = {1})
", Id, sgtagid);
        }

        public string CurrentOrgError;
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (id != DbUtil.Db.CurrentOrgId0)
            {
                CurrentOrgError = $"Current org has changed from {id} to {DbUtil.Db.CurrentOrgId0}";
                results.Add(new ValidationResult(CurrentOrgError));
                throw new Exception(CurrentOrgError);
            }
            return results;
        }
        internal void PostTransactions()
        {
            var pids = (from p in People(DbUtil.Db.CurrentOrg) select p.PeopleId).ToList();
            foreach (var pid in pids)
            {
                var db = DbUtil.Create(Util.Host);
                var om = db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == Id);
                var ts = db.ViewTransactionSummaries.SingleOrDefault(
                        tt => tt.RegId == om.TranId && tt.PeopleId == om.PeopleId);
                var reason = ts == null ? "Initial Tran" : "Adjustment";
                om.AddTransaction(db, reason, Payment ?? 0, Description, Amount, AdjustFee);
                db.SubmitChanges();
                DbUtil.LogActivity("OrgMem " + reason, Id, pid);
            }
        }

        public void AddNewSmallGroup()
        {
            var o = DbUtil.Db.LoadOrganizationById(Id);
            var mt = new MemberTag { Name = NewGroup };
            o.MemberTags.Add(mt);
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("OrgMem AddNewSubGroup " + NewGroup, Id);
            AddSmallGroup(mt.Id);
            NewGroup = null;
        }
    }
}
