using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "BlogCategoriesView")]
    public partial class BlogCategoriesView
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Category;

        private int? _N;

        public BlogCategoriesView()
        {
        }

        [Column(Name = "Category", Storage = "_Category", DbType = "varchar(50)")]
        public string Category
        {
            get => _Category;

            set
            {
                if (_Category != value)
                {
                    _Category = value;
                }
            }
        }

        [Column(Name = "n", Storage = "_N", DbType = "int")]
        public int? N
        {
            get => _N;

            set
            {
                if (_N != value)
                {
                    _N = value;
                }
            }
        }
    }
}
