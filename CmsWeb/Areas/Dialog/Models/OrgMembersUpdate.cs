using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class OrgMembersUpdate
    {
        public OrgMembersUpdate()
        {
            MemberType = new CodeInfo(0, "MemberType");
        }

        public Guid QueryId
        {
            get { return queryId; }
            set
            {
                // this one does not use LongRunningOp, but it probably should
                queryId = value;
                var filter = DbUtil.Db.OrgFilter(queryId);
                OrgName = DbUtil.Db.Organizations.Where(vv => vv.OrganizationId == filter.Id).Select(vv => vv.OrganizationName).Single();
                if (filter.GroupSelect == GroupSelectCode.Pending)
                {
                    Pending = true;
                }

                showHidden = filter.ShowHidden;
                OrgId = filter.Id;
                Count = DbUtil.Db.OrgFilterIds(queryId).Count();
                GroupSelect = filter.GroupSelect;
            }
        }

        public OrgMembersUpdate(Guid id)
            : this()
        {
            QueryId = id;
        }

        public int? Count;

        public string OrgName { get; set; }
        public int OrgId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Payment { get; set; }
        public bool AdjustFee { get; set; }
        [StringLength(100)]
        public string Description { get; set; }

        private bool? showHidden;
        public string GroupSelect;
        public string DisplayGroup
        {
            get
            {
                switch (GroupSelect)
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

        private List<int> pids;
        private List<int> Pids => pids ?? (pids = (from p in DbUtil.Db.OrgFilterIds(QueryId)
                                                   select p.PeopleId.Value).ToList());

        public void Update()
        {
            foreach (var pid in Pids)
            {
                //DbDispose();
                //var Db = DbUtil.Create(Util.Host);
                var om = DbUtil.Db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == OrgId);

                var changes = new List<ChangeDetail>();
                if (InactiveDate.HasValue)
                {
                    om.UpdateValue(changes, "InactiveDate", InactiveDate);
                }

                if (RemoveInactiveDate)
                {
                    om.UpdateValue(changes, "InactiveDate", null);
                }

                if (EnrollmentDate.HasValue)
                {
                    om.UpdateValue(changes, "EnrollmentDate", EnrollmentDate);
                }

                om.Pending = Pending;

                if (MemberType.Value != "0")
                {
                    om.UpdateValue(changes, "MemberTypeId", MemberType.Value.ToInt());
                }

                if (MakeMemberTypeOriginal)
                {
                    var et = (from e in DbUtil.Db.EnrollmentTransactions
                              where e.PeopleId == om.PeopleId
                              where e.OrganizationId == OrgId
                              orderby e.TransactionDate
                              select e).First();
                    et.MemberTypeId = om.MemberTypeId;
                }

                DbUtil.Db.SubmitChanges();
                foreach (var g in changes)
                {
                    DbUtil.LogActivity("OrgMem change " + g.Field, om.OrganizationId, om.PeopleId);
                }
            }
        }

        public string AddSmallGroup(int sgtagid)
        {
            var name = DbUtil.Db.MemberTags.Single(mm => mm.Id == sgtagid && mm.OrgId == OrgId).Name;
            foreach (var pid in Pids)
            {
                //DbDispose();
                //var Db = DbUtil.Create(Util.Host);
                var om = DbUtil.Db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == OrgId);
                var nn = om.AddToGroup(DbUtil.Db, sgtagid);
                if (nn == 1)
                {
                    DbUtil.LogActivity("OrgMem AddSubGroup " + name, om.OrganizationId, om.PeopleId);
                }
            }
            return $"{Pids.Count} added to sub-group {name}";
        }

        public void RemoveSmallGroup(int sgtagid)
        {
            var name = DbUtil.Db.MemberTags.Single(mm => mm.Id == sgtagid && mm.OrgId == OrgId).Name;
            foreach (var pid in Pids)
            {
                //DbDispose();
                //var Db = DbUtil.Create(Util.Host);
                var om = DbUtil.Db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == OrgId);
                var mt = om.OrgMemMemTags.SingleOrDefault(t => t.MemberTagId == sgtagid);
                if (mt != null)
                {
                    DbUtil.Db.OrgMemMemTags.DeleteOnSubmit(mt);
                }

                DbUtil.Db.SubmitChanges();
                if (mt != null)
                {
                    DbUtil.LogActivity("OrgMem RemoveSubGroup " + name, om.OrganizationId, om.PeopleId);
                }
            }
            var Db = DbUtil.Create(Util.Host);
            DbUtil.Db.ExecuteCommand(@"
DELETE dbo.MemberTags
WHERE Id = {1} AND OrgId = {0}
AND NOT EXISTS(SELECT NULL FROM dbo.OrgMemMemTags WHERE OrgId = {0} AND MemberTagId = {1})
", OrgId, sgtagid);
        }

        public string CurrentOrgError;
        private Guid queryId;

        internal void PostTransactions()
        {
            foreach (var pid in Pids)
            {
                var db = DbUtil.Create(Util.Host);
                var om = DbUtil.Db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == OrgId);
                var ts = DbUtil.Db.ViewTransactionSummaries.SingleOrDefault(
                        tt => tt.RegId == om.TranId && tt.PeopleId == om.PeopleId);
                var reason = ts == null ? "Initial Tran" : "Adjustment";
                var descriptionForPayment = OnlineRegModel.GetDescriptionForPayment(OrgId);
                om.AddTransaction(DbUtil.Db, reason, Payment ?? 0, Description, Amount, AdjustFee, descriptionForPayment);
                DbUtil.Db.SubmitChanges();
                DbUtil.LogActivity("OrgMem " + reason, OrgId, pid);
            }
        }

        public void AddNewSmallGroup()
        {
            var o = DbUtil.Db.LoadOrganizationById(OrgId);
            var mt = new MemberTag { Name = NewGroup };
            o.MemberTags.Add(mt);
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("OrgMem AddNewSubGroup " + NewGroup, OrgId);
            AddSmallGroup(mt.Id);
            NewGroup = null;
        }
    }
}
