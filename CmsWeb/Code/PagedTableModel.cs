using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Models
{
    public abstract class PagedTableModel<TModel, TView>
    {
        public PagerModel2 Pager { get; set; }
        protected PagedTableModel(string defaultSort, string defaultDirection)
        {
            Pager = new PagerModel2(Count) {Sort = defaultSort, Direction = defaultDirection};
        }
        private int? count;
        public int Count()
        {
            if (!count.HasValue)
                count = ModelList().Count();
            return count.Value;
        }
        public abstract IQueryable<TModel> ModelList();
        public abstract IQueryable<TModel> ApplySort();
        public abstract IEnumerable<TView> ViewList();
    }
}