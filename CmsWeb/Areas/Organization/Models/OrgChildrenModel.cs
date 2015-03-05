using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using UtilityExtensions;
using System.Text.RegularExpressions;
using System.Collections;
using CmsData.Codes;

namespace CmsWeb.Areas.Organization.Models
{
    public class OrgChildrenModel
    {
        public string namesearch { get; set; }
        public string sort { get; set; }
        public CmsData.Organization org { get; set; }
        public CmsData.Organization parentorg { get; set; }
        public bool canedit
        {
            get { return HttpContext.Current.User.IsInRole("Edit"); }
        }

        private IList<int> list = new List<int>();
        public IList<int> List
        {
            get { return list; }
            set { list = value; }
        }

        public int? orgid
        {
            get { return org == null ? null : (int?)org.OrganizationId; }
            set
            {
                org = DbUtil.Db.LoadOrganizationById(value);
                parentorg = DbUtil.Db.LoadOrganizationById(org.ParentOrgId);
            }
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
            public string ToolTip
            {
                get
                {
                    return "{0} ({1})|Program:{2}|Division: {3}({4})|Leader: {5}|Location: {6}|Divisions: {7}".Fmt(
                               Name,
                               Id,
                               Program,
                               Division,
							   DivId,
                               Leader,
                               Location,
                               Divisions
                               );
                }
            }
        }
        public IEnumerable<OrgInfo> FetchOrgList()
        {
            var q = from o in DbUtil.Db.Organizations
                    let org = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == org.OrganizationId)
                    let ck = (o.ParentOrgId ?? 0) == org.OrganizationId
                    let ot = o.ParentOrgId != null && o.ParentOrgId != org.OrganizationId
                    let pa = o.ChildOrgs.Count() > 0
					//where o.DivisionId == org.DivisionId
                    where o.DivOrgs.Any(dd => org.DivOrgs.Select(oo => oo.DivId).Contains(dd.DivId)) || o.ParentOrgId == orgid
                    where namesearch == null || o.OrganizationName.Contains(namesearch) || ck
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
                        isChecked = ck == true,
                        isOther = ot == true,
                        isParent = pa == true,
                        ParentName = o.ParentOrg.OrganizationName
                    };
            return q;
        }
    }
}
