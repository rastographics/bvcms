using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgSchedules2")]
    public partial class OrgSchedules2
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private DateTime? _SchedTime;

        private int? _SchedDay;

        public OrgSchedules2()
        {
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int NOT NULL")]
        public int OrganizationId
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

        [Column(Name = "SchedTime", Storage = "_SchedTime", DbType = "datetime")]
        public DateTime? SchedTime
        {
            get => _SchedTime;

            set
            {
                if (_SchedTime != value)
                {
                    _SchedTime = value;
                }
            }
        }

        [Column(Name = "SchedDay", Storage = "_SchedDay", DbType = "int")]
        public int? SchedDay
        {
            get => _SchedDay;

            set
            {
                if (_SchedDay != value)
                {
                    _SchedDay = value;
                }
            }
        }
    }
}
