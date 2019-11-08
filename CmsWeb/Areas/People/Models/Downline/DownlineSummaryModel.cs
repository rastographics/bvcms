using CmsData;
using CmsData.View;
using CmsWeb.Constants;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{

    public class DownlineSummaryModel : PagedTableModel<DownlineSummary, DownlineSummary>
    {
        public int? CategoryId { get; set; }


        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public DownlineSummaryModel() : base()
        {
            Init();
        }

        public DownlineSummaryModel(CMSDataContext db) : base(db)
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();
            Sort = "";
            Direction = "";
            AjaxPager = true;
            UseDbPager = true;
        }

        private string category;
        public string Category
        {
            get
            {
                if (!category.HasValue() && CategoryId.HasValue)
                {
                    category = CurrentDatabase.DownlineCategories(CategoryId).Single().Name;
                }

                return category;
            }
        }

        private List<DownlineSummary> rows;

        private List<DownlineSummary> GetList()
        {
            if (rows != null)
            {
                return rows;
            }

            rows = (from a in CurrentDatabase.DownlineSummary(CategoryId, Page, PageSize)
                    select a).ToList();
            count = rows.Count == 0 ? 0 : rows[0].MaxRows;
            return rows;
        }

        public override IQueryable<DownlineSummary> DefineModelList()
        {
            return GetList().AsQueryable();
        }

        public override IQueryable<DownlineSummary> DefineModelSort(IQueryable<DownlineSummary> q)
        {
            return q;
        }

        public override IEnumerable<DownlineSummary> DefineViewList(IQueryable<DownlineSummary> q)
        {
            return q;
        }
    }
}
