using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetTodaysMeetingHours3")]
    public partial class GetTodaysMeetingHours3
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private DateTime? _Hour;

        private int? _OrganizationId;

        public GetTodaysMeetingHours3()
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

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int")]
        public int? OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
                }
            }
        }
    }
}
