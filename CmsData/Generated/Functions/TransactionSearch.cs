using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "TransactionSearch")]
    public partial class TransactionSearch
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Id;

        public TransactionSearch()
        {
        }

        [Column(Name = "Id", Storage = "_Id", DbType = "int")]
        public int? Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    _Id = value;
                }
            }
        }
    }
}
