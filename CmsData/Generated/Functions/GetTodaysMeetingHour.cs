using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetTodaysMeetingHours")]
    public partial class GetTodaysMeetingHour
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private DateTime? _Hour;

        public GetTodaysMeetingHour()
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
