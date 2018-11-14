using CmsData;
using CmsData.View;
using CmsWeb.Models;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{

    public class DownlineSummaryModel : PagedTableModel<DownlineSummary, DownlineSummary>
    {
        public int? CategoryId { get; set; }

        public DownlineSummaryModel()
            : base("", "", true)
        {
            UseDbPager = true;
        }

        private string category;
        public string Category
        {
            get
            {
                if (!category.HasValue() && CategoryId.HasValue)
                {
                    category = DbUtil.Db.DownlineCategories(CategoryId).Single().Name;
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

            rows = (from a in DbUtil.Db.DownlineSummary(CategoryId, Page, PageSize)
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
