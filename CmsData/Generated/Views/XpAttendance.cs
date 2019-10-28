using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpAttendance")]
    public partial class XpAttendance
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private int _MeetingId;

        private int _OrganizationId;

        private DateTime _MeetingDate;

        private bool _AttendanceFlag;

        private string _AttendanceType;

        private string _MemberType;

        public XpAttendance()
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

        [Column(Name = "MeetingId", Storage = "_MeetingId", DbType = "int NOT NULL")]
        public int MeetingId
        {
            get => _MeetingId;

            set
            {
                if (_MeetingId != value)
                {
                    _MeetingId = value;
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

        [Column(Name = "MeetingDate", Storage = "_MeetingDate", DbType = "datetime NOT NULL")]
        public DateTime MeetingDate
        {
            get => _MeetingDate;

            set
            {
                if (_MeetingDate != value)
                {
                    _MeetingDate = value;
                }
            }
        }

        [Column(Name = "AttendanceFlag", Storage = "_AttendanceFlag", DbType = "bit NOT NULL")]
        public bool AttendanceFlag
        {
            get => _AttendanceFlag;

            set
            {
                if (_AttendanceFlag != value)
                {
                    _AttendanceFlag = value;
                }
            }
        }

        [Column(Name = "AttendanceType", Storage = "_AttendanceType", DbType = "nvarchar(100)")]
        public string AttendanceType
        {
            get => _AttendanceType;

            set
            {
                if (_AttendanceType != value)
                {
                    _AttendanceType = value;
                }
            }
        }

        [Column(Name = "MemberType", Storage = "_MemberType", DbType = "nvarchar(100)")]
        public string MemberType
        {
            get => _MemberType;

            set
            {
                if (_MemberType != value)
                {
                    _MemberType = value;
                }
            }
        }
    }
}
