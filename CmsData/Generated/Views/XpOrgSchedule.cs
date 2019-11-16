using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpOrgSchedule")]
    public partial class XpOrgSchedule
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Id;

        private int _OrganizationId;

        private string _SchedTime;

        private int? _SchedDay;

        public XpOrgSchedule()
        {
        }

        [Column(Name = "Id", Storage = "_Id", DbType = "int NOT NULL")]
        public int Id
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

        [Column(Name = "SchedTime", Storage = "_SchedTime", DbType = "nvarchar(4000)")]
        public string SchedTime
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
