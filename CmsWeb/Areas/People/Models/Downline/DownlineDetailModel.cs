using CmsData;
using CmsData.View;
using CmsWeb.Models;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{

    public class DownlineDetailModel : PagedTableModel<DownlineDetail, DownlineDetail>
    {
        public int? CategoryId { get; set; }
        public int? DownlineId { get; set; }
        public int? Level { get; set; }
        public int? DownlineCount { get; set; }
        public int? DownlineLevels { get; set; }
        public string Trace { get; set; }

        public DownlineDetailModel()
            : base("", "", true)
        {
            UseDbPager = true;
        }


        private string name;
        public string Name
        {
            get
            {
                if (!name.HasValue() && DownlineId.HasValue)
                {
                    var i = DbUtil.Db.DownlineLeaders.Single(dd => dd.CategoryId == CategoryId && dd.PeopleId == DownlineId);
                    name = i.Name;
                    DownlineCount = i.Cnt;
                    DownlineLevels = i.Levels;
                }
                return name;
            }
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

        private List<DownlineDetail> rows;

        private List<DownlineDetail> GetList()
        {
            if (rows != null)
            {
                return rows;
            }

            rows = (from a in DbUtil.Db.DownlineDetails(CategoryId, DownlineId, Level, Page, PageSize)
                    select a).ToList();
            count = rows.Count == 0 ? 0 : rows[0].MaxRows;
            return rows;
        }

        public override IQueryable<DownlineDetail> DefineModelList()
        {
            return GetList().AsQueryable();
        }

        public override IQueryable<DownlineDetail> DefineModelSort(IQueryable<DownlineDetail> q)
        {
            return q;
        }

        public override IEnumerable<DownlineDetail> DefineViewList(IQueryable<DownlineDetail> q)
        {
            return q;
        }

        public IEnumerable<DownlineSingleTrace> TraceList()
        {
            return DbUtil.Db.DownlineSingleTrace(CategoryId, DownlineId, Trace).OrderBy(mm => mm.Generation);
        }
    }
}
