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
                var db = CMSDataContext.Create(Host);
                queryId = value;
                var filter = db.OrgFilter(queryId);
                OrgName = db.Organizations.Where(vv => vv.OrganizationId == filter.Id).Select(vv => vv.OrganizationName).Single();
                if (filter.GroupSelect == GroupSelectCode.Pending)
                {
                    Pending = true;
                }

                showHidden = filter.ShowHidden;
                OrgId = filter.Id;
                Count = db.OrgFilterIds(queryId).Count();
                GroupSelect = filter.GroupSelect;
            }
        }

        public OrgMembersUpdate(Guid id, string host)
            : this()
        {
            Host = host;
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
        private List<int> Pids(CMSDataContext db) => pids ?? (pids = (from p in db.OrgFilterIds(QueryId)
                                                   select p.PeopleId.Value).ToList());

        public string Host { get; set; }

        public void Update()
        {
            var db = CMSDataContext.Create(Host);
            foreach (var pid in Pids(db))
            {
                var om = db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == OrgId);

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
                    var et = (from e in db.EnrollmentTransactions
                              where e.PeopleId == om.PeopleId
                              where e.OrganizationId == OrgId
                              orderby e.TransactionDate
                              select e).First();
                    et.MemberTypeId = om.MemberTypeId;
                }

                db.SubmitChanges();
                foreach (var g in changes)
                {
                    DbUtil.LogActivity("OrgMem change " + g.Field, om.OrganizationId, om.PeopleId);
                }
            }
        }

        public string AddSmallGroup(int sgtagid)
        {
            var db = CMSDataContext.Create(Host);
            var name = db.MemberTags.Single(mm => mm.Id == sgtagid && mm.OrgId == OrgId).Name;
            var peopleIds = Pids(db);
            foreach (var pid in peopleIds)
            {
                var om = db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == OrgId);
                var nn = om.AddToGroup(db, sgtagid);
                if (nn == 1)
                {
                    DbUtil.LogActivity("OrgMem AddSubGroup " + name, om.OrganizationId, om.PeopleId);
                }
            }
            return $"{peopleIds.Count} added to sub-group {name}";
        }

        public void RemoveSmallGroup(int sgtagid)
        {
            var db = CMSDataContext.Create(Host);
            var name = db.MemberTags.Single(mm => mm.Id == sgtagid && mm.OrgId == OrgId).Name;
            var peopleIds = Pids(db);
            foreach (var pid in peopleIds)
            {
                var om = db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == OrgId);
                var mt = om.OrgMemMemTags.SingleOrDefault(t => t.MemberTagId == sgtagid);
                if (mt != null)
                {
                    db.OrgMemMemTags.DeleteOnSubmit(mt);
                }

                db.SubmitChanges();
                if (mt != null)
                {
                    DbUtil.LogActivity("OrgMem RemoveSubGroup " + name, om.OrganizationId, om.PeopleId);
                }
            }
            db.ExecuteCommand(@"
DELETE dbo.MemberTags
WHERE Id = {1} AND OrgId = {0}
AND NOT EXISTS(SELECT NULL FROM dbo.OrgMemMemTags WHERE OrgId = {0} AND MemberTagId = {1})
", OrgId, sgtagid);
        }

        public string CurrentOrgError;
        private Guid queryId;

        internal void PostTransactions()
        {
            var db = CMSDataContext.Create(Host);
            foreach (var pid in Pids(db))
            {
                var om = db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == OrgId);
                var ts = db.ViewTransactionSummaries.SingleOrDefault(
                        tt => tt.RegId == om.TranId && tt.PeopleId == om.PeopleId);
                var reason = ts == null ? "Initial Tran" : "Adjustment";
                var descriptionForPayment = OnlineRegModel.GetDescriptionForPayment(OrgId);
                om.AddTransaction(db, reason, Payment ?? 0, Description, Amount, AdjustFee, descriptionForPayment);
                db.SubmitChanges();
                DbUtil.LogActivity("OrgMem " + reason, OrgId, pid);
            }
        }

        public void AddNewSmallGroup()
        {
            var db = CMSDataContext.Create(Host);
            var o = db.LoadOrganizationById(OrgId);
            if (o != null)
            {
                var mt = new MemberTag { Name = NewGroup };
                o.MemberTags.Add(mt);
                db.SubmitChanges();
                DbUtil.LogActivity("OrgMem AddNewSubGroup " + NewGroup, OrgId);
                AddSmallGroup(mt.Id);
                NewGroup = null;
            }
            else
            {
                throw new Exception($"Org not found: {OrgId}");
            }
        }
    }
}
