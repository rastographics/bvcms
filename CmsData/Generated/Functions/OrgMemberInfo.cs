using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgMemberInfo")]
    public partial class OrgMemberInfo
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private int? _LastMeetingId;

        private int? _ContacteeId;

        private int? _ContactorId;

        private int? _TaskAboutId;

        private int? _TaskDelegatedId;

        private DateTime? _LastAttendDt;

        private DateTime? _ContactReceived;

        private DateTime? _ContactMade;

        private DateTime? _TaskAboutDt;

        private DateTime? _TaskDelegatedDt;

        public OrgMemberInfo()
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

        [Column(Name = "LastMeetingId", Storage = "_LastMeetingId", DbType = "int")]
        public int? LastMeetingId
        {
            get => _LastMeetingId;

            set
            {
                if (_LastMeetingId != value)
                {
                    _LastMeetingId = value;
                }
            }
        }

        [Column(Name = "ContacteeId", Storage = "_ContacteeId", DbType = "int")]
        public int? ContacteeId
        {
            get => _ContacteeId;

            set
            {
                if (_ContacteeId != value)
                {
                    _ContacteeId = value;
                }
            }
        }

        [Column(Name = "ContactorId", Storage = "_ContactorId", DbType = "int")]
        public int? ContactorId
        {
            get => _ContactorId;

            set
            {
                if (_ContactorId != value)
                {
                    _ContactorId = value;
                }
            }
        }

        [Column(Name = "TaskAboutId", Storage = "_TaskAboutId", DbType = "int")]
        public int? TaskAboutId
        {
            get => _TaskAboutId;

            set
            {
                if (_TaskAboutId != value)
                {
                    _TaskAboutId = value;
                }
            }
        }

        [Column(Name = "TaskDelegatedId", Storage = "_TaskDelegatedId", DbType = "int")]
        public int? TaskDelegatedId
        {
            get => _TaskDelegatedId;

            set
            {
                if (_TaskDelegatedId != value)
                {
                    _TaskDelegatedId = value;
                }
            }
        }

        [Column(Name = "LastAttendDt", Storage = "_LastAttendDt", DbType = "datetime")]
        public DateTime? LastAttendDt
        {
            get => _LastAttendDt;

            set
            {
                if (_LastAttendDt != value)
                {
                    _LastAttendDt = value;
                }
            }
        }

        [Column(Name = "ContactReceived", Storage = "_ContactReceived", DbType = "datetime")]
        public DateTime? ContactReceived
        {
            get => _ContactReceived;

            set
            {
                if (_ContactReceived != value)
                {
                    _ContactReceived = value;
                }
            }
        }

        [Column(Name = "ContactMade", Storage = "_ContactMade", DbType = "datetime")]
        public DateTime? ContactMade
        {
            get => _ContactMade;

            set
            {
                if (_ContactMade != value)
                {
                    _ContactMade = value;
                }
            }
        }

        [Column(Name = "TaskAboutDt", Storage = "_TaskAboutDt", DbType = "datetime")]
        public DateTime? TaskAboutDt
        {
            get => _TaskAboutDt;

            set
            {
                if (_TaskAboutDt != value)
                {
                    _TaskAboutDt = value;
                }
            }
        }

        [Column(Name = "TaskDelegatedDt", Storage = "_TaskDelegatedDt", DbType = "datetime")]
        public DateTime? TaskDelegatedDt
        {
            get => _TaskDelegatedDt;

            set
            {
                if (_TaskDelegatedDt != value)
                {
                    _TaskDelegatedDt = value;
                }
            }
        }
    }
}
