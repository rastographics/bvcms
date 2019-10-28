using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpMeeting")]
    public partial class XpMeeting
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _MeetingId;

        private int _OrganizationId;

        private bool _GroupMeetingFlag;

        private string _Description;

        private string _Location;

        private int _NumPresent;

        private int _NumMembers;

        private int _NumVstMembers;

        private int _NumRepeatVst;

        private int _NumNewVisit;

        private DateTime? _MeetingDate;

        private int? _NumOutTown;

        private int? _NumOtherAttends;

        private int? _HeadCount;

        public XpMeeting()
        {
        }

        [Column(Name = "MeetingId", Storage = "_MeetingId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
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

        [Column(Name = "GroupMeetingFlag", Storage = "_GroupMeetingFlag", DbType = "bit NOT NULL")]
        public bool GroupMeetingFlag
        {
            get => _GroupMeetingFlag;

            set
            {
                if (_GroupMeetingFlag != value)
                {
                    _GroupMeetingFlag = value;
                }
            }
        }

        [Column(Name = "Description", Storage = "_Description", DbType = "nvarchar(100)")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    _Description = value;
                }
            }
        }

        [Column(Name = "Location", Storage = "_Location", DbType = "nvarchar(200)")]
        public string Location
        {
            get => _Location;

            set
            {
                if (_Location != value)
                {
                    _Location = value;
                }
            }
        }

        [Column(Name = "NumPresent", Storage = "_NumPresent", DbType = "int NOT NULL")]
        public int NumPresent
        {
            get => _NumPresent;

            set
            {
                if (_NumPresent != value)
                {
                    _NumPresent = value;
                }
            }
        }

        [Column(Name = "NumMembers", Storage = "_NumMembers", DbType = "int NOT NULL")]
        public int NumMembers
        {
            get => _NumMembers;

            set
            {
                if (_NumMembers != value)
                {
                    _NumMembers = value;
                }
            }
        }

        [Column(Name = "NumVstMembers", Storage = "_NumVstMembers", DbType = "int NOT NULL")]
        public int NumVstMembers
        {
            get => _NumVstMembers;

            set
            {
                if (_NumVstMembers != value)
                {
                    _NumVstMembers = value;
                }
            }
        }

        [Column(Name = "NumRepeatVst", Storage = "_NumRepeatVst", DbType = "int NOT NULL")]
        public int NumRepeatVst
        {
            get => _NumRepeatVst;

            set
            {
                if (_NumRepeatVst != value)
                {
                    _NumRepeatVst = value;
                }
            }
        }

        [Column(Name = "NumNewVisit", Storage = "_NumNewVisit", DbType = "int NOT NULL")]
        public int NumNewVisit
        {
            get => _NumNewVisit;

            set
            {
                if (_NumNewVisit != value)
                {
                    _NumNewVisit = value;
                }
            }
        }

        [Column(Name = "MeetingDate", Storage = "_MeetingDate", DbType = "datetime")]
        public DateTime? MeetingDate
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

        [Column(Name = "NumOutTown", Storage = "_NumOutTown", DbType = "int")]
        public int? NumOutTown
        {
            get => _NumOutTown;

            set
            {
                if (_NumOutTown != value)
                {
                    _NumOutTown = value;
                }
            }
        }

        [Column(Name = "NumOtherAttends", Storage = "_NumOtherAttends", DbType = "int")]
        public int? NumOtherAttends
        {
            get => _NumOtherAttends;

            set
            {
                if (_NumOtherAttends != value)
                {
                    _NumOtherAttends = value;
                }
            }
        }

        [Column(Name = "HeadCount", Storage = "_HeadCount", DbType = "int")]
        public int? HeadCount
        {
            get => _HeadCount;

            set
            {
                if (_HeadCount != value)
                {
                    _HeadCount = value;
                }
            }
        }
    }
}
