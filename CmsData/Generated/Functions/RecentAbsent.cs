using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RecentAbsents")]
    public partial class RecentAbsent
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrganizationName;

        private string _LeaderName;

        private int? _Consecutive;

        private int _PeopleId;

        private string _Name2;

        private string _HomePhone;

        private string _CellPhone;

        private string _EmailAddress;

        private DateTime? _LastAttend;

        private DateTime? _LastMeeting;

        private int _MeetingId;

        private int? _ConsecutiveAbsentsThreshold;

        public RecentAbsent()
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

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "nvarchar(100) NOT NULL")]
        public string OrganizationName
        {
            get => _OrganizationName;

            set
            {
                if (_OrganizationName != value)
                {
                    _OrganizationName = value;
                }
            }
        }

        [Column(Name = "LeaderName", Storage = "_LeaderName", DbType = "nvarchar(50)")]
        public string LeaderName
        {
            get => _LeaderName;

            set
            {
                if (_LeaderName != value)
                {
                    _LeaderName = value;
                }
            }
        }

        [Column(Name = "consecutive", Storage = "_Consecutive", DbType = "int")]
        public int? Consecutive
        {
            get => _Consecutive;

            set
            {
                if (_Consecutive != value)
                {
                    _Consecutive = value;
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

        [Column(Name = "Name2", Storage = "_Name2", DbType = "nvarchar(139)")]
        public string Name2
        {
            get => _Name2;

            set
            {
                if (_Name2 != value)
                {
                    _Name2 = value;
                }
            }
        }

        [Column(Name = "HomePhone", Storage = "_HomePhone", DbType = "nvarchar(20)")]
        public string HomePhone
        {
            get => _HomePhone;

            set
            {
                if (_HomePhone != value)
                {
                    _HomePhone = value;
                }
            }
        }

        [Column(Name = "CellPhone", Storage = "_CellPhone", DbType = "nvarchar(20)")]
        public string CellPhone
        {
            get => _CellPhone;

            set
            {
                if (_CellPhone != value)
                {
                    _CellPhone = value;
                }
            }
        }

        [Column(Name = "EmailAddress", Storage = "_EmailAddress", DbType = "nvarchar(150)")]
        public string EmailAddress
        {
            get => _EmailAddress;

            set
            {
                if (_EmailAddress != value)
                {
                    _EmailAddress = value;
                }
            }
        }

        [Column(Name = "LastAttend", Storage = "_LastAttend", DbType = "datetime")]
        public DateTime? LastAttend
        {
            get => _LastAttend;

            set
            {
                if (_LastAttend != value)
                {
                    _LastAttend = value;
                }
            }
        }

        [Column(Name = "LastMeeting", Storage = "_LastMeeting", DbType = "datetime")]
        public DateTime? LastMeeting
        {
            get => _LastMeeting;

            set
            {
                if (_LastMeeting != value)
                {
                    _LastMeeting = value;
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

        [Column(Name = "ConsecutiveAbsentsThreshold", Storage = "_ConsecutiveAbsentsThreshold", DbType = "int")]
        public int? ConsecutiveAbsentsThreshold
        {
            get => _ConsecutiveAbsentsThreshold;

            set
            {
                if (_ConsecutiveAbsentsThreshold != value)
                {
                    _ConsecutiveAbsentsThreshold = value;
                }
            }
        }
    }
}
