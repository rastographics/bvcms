using CmsData;
using CmsData.Codes;
using CmsWeb.Constants;
using CmsWeb.Models;
using System;
using ImageData;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class OrgMemberMoveModel : PagedTableModel<Organization, OrgMoveInfo>
    {
        private int? orgId;
        private int? peopleId;
        private void Populate()
        {
            var i = (from mm in CurrentDatabase.OrganizationMembers
                     where mm.OrganizationId == OrgId && mm.PeopleId == PeopleId
                     select new
                     {
                         mm,
                         mm.Person.Name,
                         mm.Organization.OrganizationName,
                     }).Single();
            Name = i.Name;
            OrgName = i.OrganizationName;
        }

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public OrgMemberMoveModel()
        {
            Init();
        }

        public OrgMemberMoveModel(CMSDataContext db) : base(db)
        {
            Init();
        }

        override protected void Init()
        {
            base.Init();
            AjaxPager = true;
            pagesize = 10;
            ShowPageSize = false;
            MoveRegistrationData = true;
        }

        public bool MoveRegistrationData { get; set; }
        public string OrgSearch { get; set; }
        public string Group { get; set; }

        public int? OrgId
        {
            get { return orgId; }
            set
            {
                orgId = value;
                if (peopleId.HasValue)
                {
                    Populate();
                }
            }
        }
        public int? PeopleId
        {
            get { return peopleId; }
            set
            {
                peopleId = value;
                if (orgId.HasValue)
                {
                    Populate();
                }
            }
        }
        public string Name { get; set; }
        public string OrgName { get; set; }

        public override IQueryable<Organization> DefineModelList()
        {
            return from o in CurrentDatabase.Organizations
                   let org = CurrentDatabase.Organizations.Single(oo => oo.OrganizationId == OrgId)
                   where o.DivOrgs.Any(dd => org.DivOrgs.Any(oo => oo.DivId == dd.DivId))
                   where o.OrganizationId != OrgId
                   where o.OrganizationStatusId == OrgStatusCode.Active
                   where !OrgSearch.HasValue() || o.OrganizationName.Contains(OrgSearch)
                   select o;
        }

        public override IQueryable<Organization> DefineModelSort(IQueryable<Organization> q)
        {
            return q.OrderBy(m => m.OrganizationName);
        }

        public override IEnumerable<OrgMoveInfo> DefineViewList(IQueryable<Organization> q)
        {
            return from o in q
                   select new OrgMoveInfo
                   {
                       OrgName = o.OrganizationName,
                       ToOrgId = o.OrganizationId,
                       PeopleId = PeopleId.Value,
                       FromOrgId = OrgId.Value,
                       Program = o.Division.Program.Name,
                       Division = o.Division.Name,
                       orgSchedule = o.OrgSchedules.First()
                   };
        }

        public string Move(int toid, CMSDataContext db)
        {
            if (!PeopleId.HasValue || !OrgId.HasValue)
            {
                return "not moved";
            }

            OrganizationMember.MoveToOrg(db, CMSImageDataContext.Create(db.Host), PeopleId.Value, OrgId.Value, toid, MoveRegistrationData);
            DbUtil.LogActivity("OrgMem Move to " + toid, OrgId, PeopleId);

            var repairExe = HttpContextFactory.Current.Server.MapPath("~/bin/RepairOrg.exe");
            MoveOrgMembersModel.BackgroundRepairTransactions(repairExe, toid, db);
            MoveOrgMembersModel.BackgroundRepairTransactions(repairExe, OrgId.Value, db);
            return "moved";
        }
    }
}
