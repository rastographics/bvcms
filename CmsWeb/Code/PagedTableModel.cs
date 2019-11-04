using CmsData;
using CmsWeb.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Models
{
    public abstract class PagedTableModel<TModel, TView> : PagerModel2
    {
        internal int? count;
        internal IQueryable<TModel> list;

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        protected PagedTableModel()
        {
            Init();
        }

        protected PagedTableModel(CMSDataContext db, string defaultSort = "", string defaultDirection = "", bool useAjax = false) : base(db)
        {
            CurrentDatabase = db;
            Init();
            Sort = defaultSort;
            Direction = defaultDirection;
            AjaxPager = useAjax;
        }

        protected virtual void Init()
        {
            GetCount = Count;
        }

        public bool UseDbPager { get; set; }

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
            if (q == null)
                throw new Exception($"sort not defined {SortExpression}");
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
