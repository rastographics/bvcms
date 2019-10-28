using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RandNumber")]
    public partial class RandNumber
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private double? _RandNumberX;

        public RandNumber()
        {
        }

        [Column(Name = "RandNumber", Storage = "_RandNumberX", DbType = "float")]
        public double? RandNumberX
        {
            get => _RandNumberX;

            set
            {
                if (_RandNumberX != value)
                {
                    _RandNumberX = value;
                }
            }
        }
    }
}
