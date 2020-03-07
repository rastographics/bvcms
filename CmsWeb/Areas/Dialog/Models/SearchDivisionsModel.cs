using CmsData;
using CmsData.View;
using CmsWeb.Constants;
using CmsWeb.Models;
using System;
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

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public SearchDivisionsModel()
        {
            AjaxPager = true;
            PageSize = 10;
            ShowPageSize = false;
        }

        public SearchDivisionsModel(CMSDataContext db, int id) : base(db)
        {
            Init();
            Id = id;
        }

        protected override void Init()
        {
            AjaxPager = true;
            PageSize = 10;
            ShowPageSize = false;
            base.Init();
        }

        public void AddRemoveDiv()
        {
            var d = CurrentDatabase.DivOrgs.SingleOrDefault(dd => dd.DivId == TargetDivision && dd.OrgId == Id);
            if (Adding && d == null && TargetDivision.HasValue)
            {
                d = new DivOrg { OrgId = Id, DivId = TargetDivision.Value };
                CurrentDatabase.DivOrgs.InsertOnSubmit(d);
                CurrentDatabase.SubmitChanges();
            }
            else if (!Adding && d != null)
            {
                CurrentDatabase.DivOrgs.DeleteOnSubmit(d);
                CurrentDatabase.SubmitChanges();
            }
            Page = 1;
        }

        public override IQueryable<SearchDivision> DefineModelList()
        {

            return CurrentDatabase.SearchDivisions(Id, name);
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
