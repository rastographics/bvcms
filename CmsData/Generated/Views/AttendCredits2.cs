using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "AttendCredits2")]
    public partial class AttendCredits2
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private int _PeopleId;

        private bool? _Attended;

        private int? _WeekNumber;

        private int? _AttendanceTypeId;

        public AttendCredits2()
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

        [Column(Name = "WeekNumber", Storage = "_WeekNumber", DbType = "int")]
        public int? WeekNumber
        {
            get => _WeekNumber;

            set
            {
                if (_WeekNumber != value)
                {
                    _WeekNumber = value;
                }
            }
        }

        [Column(Name = "AttendanceTypeId", Storage = "_AttendanceTypeId", DbType = "int")]
        public int? AttendanceTypeId
        {
            get => _AttendanceTypeId;

            set
            {
                if (_AttendanceTypeId != value)
                {
                    _AttendanceTypeId = value;
                }
            }
        }
    }
}
