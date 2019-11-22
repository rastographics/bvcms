using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "SundayDates")]
    public partial class SundayDate
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private DateTime? _Dt;

        public SundayDate()
        {
        }

        [Column(Name = "dt", Storage = "_Dt", DbType = "datetime")]
        public DateTime? Dt
        {
            get => _Dt;

            set
            {
                if (_Dt != value)
                {
                    _Dt = value;
                }
            }
        }
    }
}
