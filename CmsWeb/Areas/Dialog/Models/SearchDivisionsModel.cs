/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using CmsData;
using CmsData.View;
using CmsWeb.Models;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Areas.Dialog.Models
{
    public class SearchDivisionsModel : PagedTableModel<SearchDivision, SearchDivision>
    {
        public string name { get; set; }
        public bool singlemode { get; set; }
        public bool ordered { get; set; }
        public int Id { get; set; }
        public int? TargetDivision { get; set; }
        public bool Adding { get; set; }

        public SearchDivisionsModel()
        {
            AjaxPager = true;
            PageSize = 10;
            ShowPageSize = false;
        }
        public SearchDivisionsModel(int id)
            : this()
        {
            Id = id;
        }

        public void AddRemoveDiv()
        {
            var d = DbUtil.Db.DivOrgs.SingleOrDefault(dd => dd.DivId == TargetDivision && dd.OrgId == Id);
            if (Adding && d == null && TargetDivision.HasValue)
            {
                d = new DivOrg { OrgId = Id, DivId = TargetDivision.Value };
                DbUtil.Db.DivOrgs.InsertOnSubmit(d);
                DbUtil.Db.SubmitChanges();
            }
            else if (!Adding && d != null)
            {
                DbUtil.Db.DivOrgs.DeleteOnSubmit(d);
                DbUtil.Db.SubmitChanges();
            }
            Page = 1;
        }

        public override IQueryable<SearchDivision> DefineModelList()
        {

            return DbUtil.Db.SearchDivisions(Id, name);
        }

        public override IQueryable<SearchDivision> DefineModelSort(IQueryable<SearchDivision> q)
        {
            q = from sd in q
                orderby sd.IsMain descending, sd.IsChecked descending, sd.Program, sd.Division
                select sd;
            return q;
        }

        public override IEnumerable<SearchDivision> DefineViewList(IQueryable<SearchDivision> q)
        {
            return q;
        }
    }
}
