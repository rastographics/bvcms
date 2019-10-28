using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "FilterOnlineReg")]
    public partial class FilterOnlineReg
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Oid;

        public FilterOnlineReg()
        {
        }

        [Column(Name = "oid", Storage = "_Oid", DbType = "int")]
        public int? Oid
        {
            get => _Oid;

            set
            {
                if (_Oid != value)
                {
                    _Oid = value;
                }
            }
        }
    }
}
