using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetTodaysMeetingHours2")]
    public partial class GetTodaysMeetingHours2
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private DateTime? _Hour;

        public GetTodaysMeetingHours2()
        {
        }

        [Column(Name = "hour", Storage = "_Hour", DbType = "datetime")]
        public DateTime? Hour
        {
            get => _Hour;

            set
            {
                if (_Hour != value)
                {
                    _Hour = value;
                }
            }
        }
    }
}
