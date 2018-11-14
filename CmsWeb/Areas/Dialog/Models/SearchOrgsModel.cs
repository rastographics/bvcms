/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using CmsData;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class SearchOrgsModel
    {
        public string name { get; set; }
        public int maxitems { get; set; }
        public int count { get; set; }
        public int listcount { get; set; }
        public bool singlemode { get; set; }
        public bool ordered { get; set; }
        public int id { get; set; }
        public List<int> cklist { get; set; }

        public SearchOrgsModel()
        {
            maxitems = 50;
        }
        private IQueryable<Organization> orgs;
        public IQueryable<Organization> FetchOrgs()
        {
            if (orgs != null)
            {
                return orgs;
            }

            orgs = from o in DbUtil.Db.Organizations
                   let org = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == id)
                   //where o.DivOrgs.Any(dd => org.DivOrgs.Any(oo => oo.DivId == dd.DivId))
                   where o.OrganizationId != id
                   where o.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Active
                   select o;
            if (name.HasValue())
            {
                if (name.AllDigits())
                {
                    orgs = from o in orgs
                           where o.OrganizationId == name.ToInt()
                           select o;
                }
                else
                {
                    orgs = from o in orgs
                           where o.OrganizationName.Contains(name)
                           || o.DivOrgs.Any(dd => dd.Division.Name.Contains(name))
                           select o;
                }
            }
            return orgs;
        }

        public List<OrgInfo> OrgList()
        {
            var orgs = FetchOrgs();

            var list = new List<OrgInfo>();
            if (!singlemode)
            {
                list = CheckedOrgList(
                        from oo in DbUtil.Db.Organizations
                        where cklist.Contains(oo.OrganizationId)
                        select oo).ToList();
                var d = new Dictionary<int, int>();
                var n = 0;
                foreach (var i in cklist)
                {
                    d.Add(n++, i);
                }

                list = (from op in list
                        join i in d on op.OrgId equals i.Value into j
                        from i in j
                        orderby i.Key
                        select op).ToList();

                // filter out checked ones
                orgs = orgs.Where(o => !cklist.Contains(o.OrganizationId));
            }
            count = orgs.Count() + list.Count;
            orgs = orgs.OrderBy(d => d.Division.Name).ThenBy(d => d.OrganizationName);
            list.AddRange(OrgList(orgs).Take(maxitems));
            listcount = list.Count;
            return list;
        }
        private IEnumerable<OrgInfo> CheckedOrgList(IQueryable<Organization> query)
        {
            return OrgList(query, ck: true);
        }
        private IEnumerable<OrgInfo> OrgList(IQueryable<Organization> query)
        {
            return OrgList(query, ck: false);
        }
        private IEnumerable<OrgInfo> OrgList(IQueryable<Organization> query, bool ck)
        {
            var q = from o in query
                    select new OrgInfo
                    {
                        OrgId = o.OrganizationId,
                        Name = o.OrganizationName,
                        Division = o.Division.Name,
                        Divisions = string.Join("|", o.DivOrgs.Where(pp => pp.DivId != o.DivisionId).Select(pp => pp.Division.Name)),
                        IsChecked = ck,
                        IsFirst = ck && cklist.Count > 0 && o.OrganizationId == cklist[0]
                    };
            return q;
        }
        public class OrgInfo
        {
            public int OrgId { get; set; }
            public string Name { get; set; }
            public string Division { get; set; }
            public string Divisions { get; set; }
            public bool IsChecked { get; set; }
            public bool IsFirst { get; set; }
        }
    }
}
