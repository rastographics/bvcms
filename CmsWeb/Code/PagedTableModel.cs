using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public abstract class PagedTableModel<TModel, TView> : PagerModel2
    {
        protected PagedTableModel(string defaultSort, string defaultDirection)
           : this()
        {
            Sort = defaultSort;
            Direction = defaultDirection;
        }
        protected PagedTableModel()
        {
            GetCount = Count;
        }
        protected PagedTableModel(string defaultSort, string defaultDirection, bool useAjax)
            : this()
        {
            Sort = defaultSort;
            Direction = defaultDirection;
            AjaxPager = useAjax;
        }
        public bool UseDbPager { get; set; }
        internal int? count;
        public virtual int Count()
        {
            if (!count.HasValue)
            {
                var li = ModelList();
                if (count.HasValue)
                    return count.Value;
                count = li.Count();
            }
            return count.Value;
        }

        internal IQueryable<TModel> list;
        public abstract IQueryable<TModel> DefineModelList();
        public abstract IQueryable<TModel> DefineModelSort(IQueryable<TModel> q);
        public abstract IEnumerable<TView> DefineViewList(IQueryable<TModel> q);

        private IQueryable<TModel> ModelList()
        {
            if (list != null)
                return list;
            return list = DefineModelList();
        }
        private IQueryable<TModel> SortedList()
        {
            var q = DefineModelSort(ModelList());
            if(q == null)
                throw new Exception("sort not defined {0}".Fmt(SortExpression));
            if (PageSize == 0 || UseDbPager)
                return q;
            return q.Skip(StartRow).Take(PageSize);
        }
        public IEnumerable<TView> ViewList()
        {
            return DefineViewList(SortedList());
        }
    }
}