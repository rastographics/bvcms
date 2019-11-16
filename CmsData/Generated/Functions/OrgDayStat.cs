using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgDayStats")]
    public partial class OrgDayStat
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _OrganizationId;

        private string _OrganizationName;

        private string _Leader;

        private string _Location;

        private DateTime? _MeetingTime;

        private int? _Attends;

        private int? _Guests;

        private int? _Members;

        private int? _NewMembers;

        private int? _Dropped;

        private int? _Members7;

        public OrgDayStat()
        {
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

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "varchar(200)")]
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

        [Column(Name = "Leader", Storage = "_Leader", DbType = "nvarchar(80)")]
        public string Leader
        {
            get => _Leader;

            set
            {
                if (_Leader != value)
                {
                    _Leader = value;
                }
            }
        }

        [Column(Name = "Location", Storage = "_Location", DbType = "nvarchar(80)")]
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

        [Column(Name = "MeetingTime", Storage = "_MeetingTime", DbType = "datetime")]
        public DateTime? MeetingTime
        {
            get => _MeetingTime;

            set
            {
                if (_MeetingTime != value)
                {
                    _MeetingTime = value;
                }
            }
        }

        [Column(Name = "Attends", Storage = "_Attends", DbType = "int")]
        public int? Attends
        {
            get => _Attends;

            set
            {
                if (_Attends != value)
                {
                    _Attends = value;
                }
            }
        }

        [Column(Name = "Guests", Storage = "_Guests", DbType = "int")]
        public int? Guests
        {
            get => _Guests;

            set
            {
                if (_Guests != value)
                {
                    _Guests = value;
                }
            }
        }

        [Column(Name = "Members", Storage = "_Members", DbType = "int")]
        public int? Members
        {
            get => _Members;

            set
            {
                if (_Members != value)
                {
                    _Members = value;
                }
            }
        }

        [Column(Name = "NewMembers", Storage = "_NewMembers", DbType = "int")]
        public int? NewMembers
        {
            get => _NewMembers;

            set
            {
                if (_NewMembers != value)
                {
                    _NewMembers = value;
                }
            }
        }

        [Column(Name = "Dropped", Storage = "_Dropped", DbType = "int")]
        public int? Dropped
        {
            get => _Dropped;

            set
            {
                if (_Dropped != value)
                {
                    _Dropped = value;
                }
            }
        }

        [Column(Name = "Members7", Storage = "_Members7", DbType = "int")]
        public int? Members7
        {
            get => _Members7;

            set
            {
                if (_Members7 != value)
                {
                    _Members7 = value;
                }
            }
        }
    }
}
