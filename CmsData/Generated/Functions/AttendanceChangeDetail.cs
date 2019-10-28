using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "AttendanceChangeDetail")]
    public partial class AttendanceChangeDetail
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private int _OrganizationId;

        private bool? _Attended;

        private DateTime? _WeekDate;

        public AttendanceChangeDetail()
        {
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int NOT NULL")]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    _PeopleId = value;
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

        [Column(Name = "Attended", Storage = "_Attended", DbType = "bit")]
        public bool? Attended
        {
            get => _Attended;

            set
            {
                if (_Attended != value)
                {
                    _Attended = value;
                }
            }
        }

        [Column(Name = "WeekDate", Storage = "_WeekDate", DbType = "datetime")]
        public DateTime? WeekDate
        {
            get => _WeekDate;

            set
            {
                if (_WeekDate != value)
                {
                    _WeekDate = value;
                }
            }
        }
    }
}
