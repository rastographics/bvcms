/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Reports.Models;
using UtilityExtensions;
using System.Data.Linq.SqlClient;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace CmsWeb.Models
{
    public class SearchDivisionsModel : PagedTableModel<Division, DivInfo>
    {
        public string name { get; set; }
        public int maxitems { get; set; }
        public int count { get; set; }
        public int listcount { get; set; }
        public bool singlemode { get; set; }
        public bool ordered { get; set; }
        public int id { get; set; }
        public int? topid { get; set; }

        public SearchDivisionsModel()
        {
            PageSize = 10;
            ShowPageSize = false;
        }
        private IEnumerable<DivInfo> CheckedDivisionList(IQueryable<Division> query)
        {
            return DivisionList(query, ck: true);
        }
        private IEnumerable<DivInfo> DivisionList(IQueryable<Division> query)
        {
            return DivisionList(query, ck: false);
        }
        private IEnumerable<DivInfo> DivisionList(IQueryable<Division> query, bool ck)
        {
            var q = from d in query
                    select new DivInfo
                    {
                        DivId = d.Id,
                        Name = d.Name,
                        Program = d.Program.Name,
                        Programs = string.Join("|", d.ProgDivs.Where(pp => pp.ProgId != d.ProgId).Select(pp => pp.Program.Name)),
                        IsChecked = ck,
                    };
            return q;
        }

        public override IQueryable<Division> DefineModelList()
        {
            var divisions = DbUtil.Db.Divisions.AsQueryable();
            if (name.HasValue())
            {
                if (name.AllDigits())
                    divisions = from d in divisions
                                where d.Id == name.ToInt()
                                select d;
                else
                    divisions = from d in divisions
                                where d.Name.Contains(name)
                                select d;
            }
            return divisions;
        }

        public override IQueryable<Division> DefineModelSort(IQueryable<Division> q)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DivInfo> DefineViewList(IQueryable<Division> q)
        {
            var list = new List<DivInfo>();
            if (!singlemode)
            {
                list = CheckedDivisionList(
                        from dd in DbUtil.Db.DivOrgs
                        where dd.OrgId == id
                        orderby dd.Id ?? 99 // puts null values at end
                        select dd.Division).ToList();
                var ids = list.Select(p => p.DivId).ToArray();
                topid = ids.FirstOrDefault();
                // filter out checked ones
                q = q.Where(d => !ids.Contains(d.Id));
            }
            count = q.Count() + list.Count;
            q = q.OrderBy(d => d.Program.Name).ThenBy(d => d.Name);
            list.AddRange(DivisionList(q).Take(maxitems));
            listcount = list.Count;
            return list;
        }
    }
    public class DivInfo
    {
        public int DivId { get; set; }
        public string Name { get; set; }
        public string Program { get; set; }
        public string Programs { get; set; }
        public bool IsChecked { get; set; }
    }
}
