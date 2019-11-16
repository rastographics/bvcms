using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "SplitInts")]
    public partial class SplitInt
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _ValueX;

        public SplitInt()
        {
        }

        [Column(Name = "Value", Storage = "_ValueX", DbType = "int")]
        public int? ValueX
        {
            get => _ValueX;

            set
            {
                if (_ValueX != value)
                {
                    _ValueX = value;
                }
            }
        }
    }
}
