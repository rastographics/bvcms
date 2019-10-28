using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "InvolvementCurrent")]
    public partial class InvolvementCurrent
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrgType;

        private string _OrgCode;

        private int _OrgTypeSort;

        private string _Name;

        private string _DivisionName;

        private string _ProgramName;

        private string _LeaderName;

        private string _Location;

        private DateTime? _MeetingTime;

        private string _MemberType;

        private DateTime? _EnrollDate;

        private decimal? _AttendPct;

        private bool? _HasDirectory;

        private bool? _IsLeaderAttendanceType;

        private bool? _IsProspect;

        private int _PeopleId;

        private int? _LeaderId;

        private bool? _Pending;

        private string _LimitToRole;

        private int _SecurityTypeId;

        private bool? _IsMissionTripOrg;

        public InvolvementCurrent()
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

        [Column(Name = "OrgType", Storage = "_OrgType", DbType = "nvarchar(50) NOT NULL")]
        public string OrgType
        {
            get => _OrgType;

            set
            {
                if (_OrgType != value)
                {
                    _OrgType = value;
                }
            }
        }

        [Column(Name = "OrgCode", Storage = "_OrgCode", DbType = "nvarchar(20)")]
        public string OrgCode
        {
            get => _OrgCode;

            set
            {
                if (_OrgCode != value)
                {
                    _OrgCode = value;
                }
            }
        }

        [Column(Name = "OrgTypeSort", Storage = "_OrgTypeSort", DbType = "int NOT NULL")]
        public int OrgTypeSort
        {
            get => _OrgTypeSort;

            set
            {
                if (_OrgTypeSort != value)
                {
                    _OrgTypeSort = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(100) NOT NULL")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    _Name = value;
                }
            }
        }

        [Column(Name = "DivisionName", Storage = "_DivisionName", DbType = "nvarchar(50)")]
        public string DivisionName
        {
            get => _DivisionName;

            set
            {
                if (_DivisionName != value)
                {
                    _DivisionName = value;
                }
            }
        }

        [Column(Name = "ProgramName", Storage = "_ProgramName", DbType = "nvarchar(50)")]
        public string ProgramName
        {
            get => _ProgramName;

            set
            {
                if (_ProgramName != value)
                {
                    _ProgramName = value;
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

        [Column(Name = "EnrollDate", Storage = "_EnrollDate", DbType = "datetime")]
        public DateTime? EnrollDate
        {
            get => _EnrollDate;

            set
            {
                if (_EnrollDate != value)
                {
                    _EnrollDate = value;
                }
            }
        }

        [Column(Name = "AttendPct", Storage = "_AttendPct", DbType = "real")]
        public decimal? AttendPct
        {
            get => _AttendPct;

            set
            {
                if (_AttendPct != value)
                {
                    _AttendPct = value;
                }
            }
        }

        [Column(Name = "HasDirectory", Storage = "_HasDirectory", DbType = "bit")]
        public bool? HasDirectory
        {
            get => _HasDirectory;

            set
            {
                if (_HasDirectory != value)
                {
                    _HasDirectory = value;
                }
            }
        }

        [Column(Name = "IsLeaderAttendanceType", Storage = "_IsLeaderAttendanceType", DbType = "bit")]
        public bool? IsLeaderAttendanceType
        {
            get => _IsLeaderAttendanceType;

            set
            {
                if (_IsLeaderAttendanceType != value)
                {
                    _IsLeaderAttendanceType = value;
                }
            }
        }

        [Column(Name = "IsProspect", Storage = "_IsProspect", DbType = "bit")]
        public bool? IsProspect
        {
            get => _IsProspect;

            set
            {
                if (_IsProspect != value)
                {
                    _IsProspect = value;
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

        [Column(Name = "LeaderId", Storage = "_LeaderId", DbType = "int")]
        public int? LeaderId
        {
            get => _LeaderId;

            set
            {
                if (_LeaderId != value)
                {
                    _LeaderId = value;
                }
            }
        }

        [Column(Name = "Pending", Storage = "_Pending", DbType = "bit")]
        public bool? Pending
        {
            get => _Pending;

            set
            {
                if (_Pending != value)
                {
                    _Pending = value;
                }
            }
        }

        [Column(Name = "LimitToRole", Storage = "_LimitToRole", DbType = "nvarchar(20)")]
        public string LimitToRole
        {
            get => _LimitToRole;

            set
            {
                if (_LimitToRole != value)
                {
                    _LimitToRole = value;
                }
            }
        }

        [Column(Name = "SecurityTypeId", Storage = "_SecurityTypeId", DbType = "int NOT NULL")]
        public int SecurityTypeId
        {
            get => _SecurityTypeId;

            set
            {
                if (_SecurityTypeId != value)
                {
                    _SecurityTypeId = value;
                }
            }
        }

        [Column(Name = "IsMissionTripOrg", Storage = "_IsMissionTripOrg", DbType = "bit")]
        public bool? IsMissionTripOrg
        {
            get => _IsMissionTripOrg;

            set
            {
                if (_IsMissionTripOrg != value)
                {
                    _IsMissionTripOrg = value;
                }
            }
        }
    }
}
