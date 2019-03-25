using CmsData;
using CmsData.Codes;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgChildrenModel
    {
        public string namesearch { get; set; }
        public string sort { get; set; }
        public Organization org { get; set; }
        public Organization parentorg { get; set; }

        public bool canedit => HttpContextFactory.Current.User.IsInRole("Edit");

        public IList<int> List { get; set; } = new List<int>();

        public int? orgid
        {
            get { return org?.OrganizationId; }
            set
            {
                org = DbUtil.Db.LoadOrganizationById(value);
                parentorg = DbUtil.Db.LoadOrganizationById(org.ParentOrgId);
            }
        }

        public OrgChildrenModel() { }
        public IEnumerable<OrgInfo> FetchOrgList()
        {
            var showAllChildOrgs = DbUtil.Db.Setting("UX-ManageShowAllChildOrgs", true);

            var q = from o in DbUtil.Db.Organizations
                    let org = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == org.OrganizationId)
                    let ck = (o.ParentOrgId ?? 0) == org.OrganizationId
                    let ot = o.ParentOrgId != null && o.ParentOrgId != org.OrganizationId
                    let pa = o.ChildOrgs.Any()
                    where o.DivOrgs.Any(dd => org.DivOrgs.Select(oo => oo.DivId).Contains(dd.DivId)) || o.ParentOrgId == orgid || (o.ParentOrgId.HasValue && showAllChildOrgs)
                    where namesearch == null || o.OrganizationName.Contains(namesearch) || o.DivOrgs.Any(dd => dd.Division.Name.Contains(namesearch)) || ck
                    where o.OrganizationId != org.OrganizationId
                    where o.OrganizationStatusId == OrgStatusCode.Active || o.ParentOrgId == orgid
                    orderby !ck, ot, pa, o.OrganizationName
                    select new OrgInfo
                    {
                        Id = o.OrganizationId,
                        ParentId = o.ParentOrgId,
                        Name = o.OrganizationName,
                        Leader = o.LeaderName,
                        Program = o.Division.Program.Name,
                        Division = o.Division.Name,
                        DivId = o.DivisionId ?? 0,
                        Divisions = string.Join(",", o.DivOrgs.Select(d => d.Division.Name)),
                        Location = o.Location,
                        isChecked = ck,
                        isOther = ot,
                        isParent = pa,
                        ParentName = o.ParentOrg.OrganizationName
                    };
            return q;
        }

        public class OrgInfo
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string Name { get; set; }
            public string Leader { get; set; }
            public string Division { get; set; }
            public int DivId { get; set; }
            public string Program { get; set; }
            public string Divisions { get; set; }
            public string Location { get; set; }
            public string ParentName { get; set; }
            public bool isChecked { get; set; }
            public bool isOther { get; set; }
            public bool isParent { get; set; }
            public string ToolTip => $"{Name} ({Id})|Program:{Program}|Division: {Division}({DivId})|Leader: {Leader}|Location: {Location}|Divisions: {Divisions}";
        }
    }
}
