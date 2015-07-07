using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsData.View;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{

    public class DownlineModel : PagedTableModel<DownlineDetail, DownlineDetail>
    {
        public int? DownlineId { get; set; }
        public int? CategoryId { get; set; }
        public int? DownlineCount { get; set; }
        public string Trace { get; set; }

        public DownlineModel()
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
                    name = (from p in DbUtil.Db.People
                        where p.PeopleId == DownlineId
                        select p.Name).Single();
                    DownlineCount = DbUtil.Db.DownlineSummary(CategoryId, DownlineId, 1, 1).Single().Cnt;
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
                    category = DbUtil.Db.DownlineCategories(CategoryId).Single().Name;
                return category;
            }
        }

        private List<DownlineDetail> rows;

        private List<DownlineDetail> GetList()
        {
            if (rows != null)
                return rows;
            rows = (from a in DbUtil.Db.DownlineDetails(CategoryId, DownlineId, Page, PageSize)
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