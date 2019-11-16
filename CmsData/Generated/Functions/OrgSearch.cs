using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgSearch")]
    public partial class OrgSearch
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrganizationName;

        private int _OrganizationStatusId;

        private string _Program;

        private int? _ProgramId;

        private string _Division;

        private string _Divisions;

        private string _ScheduleDescription;

        private int? _ScheduleId;

        private DateTime? _SchedTime;

        private string _Campus;

        private string _LeaderType;

        private string _LeaderName;

        private string _Location;

        private bool? _ClassFilled;

        private bool? _RegistrationClosed;

        private string _AppCategory;

        private DateTime? _RegStart;

        private DateTime? _RegEnd;

        private string _PublicSortOrder;

        private DateTime? _FirstMeetingDate;

        private DateTime? _LastMeetingDate;

        private int? _MemberCount;

        private int? _RegistrationTypeId;

        private bool? _CanSelfCheckin;

        private int? _LeaderId;

        private int? _PrevMemberCount;

        private int? _ProspectCount;

        private string _Description;

        private bool? _UseRegisterLink2;

        private int? _DivisionId;

        private DateTime? _BirthDayStart;

        private DateTime? _BirthDayEnd;

        private string _Tag;

        private int _ChangeMain;

        public OrgSearch()
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

        [Column(Name = "OrganizationStatusId", Storage = "_OrganizationStatusId", DbType = "int NOT NULL")]
        public int OrganizationStatusId
        {
            get => _OrganizationStatusId;

            set
            {
                if (_OrganizationStatusId != value)
                {
                    _OrganizationStatusId = value;
                }
            }
        }

        [Column(Name = "Program", Storage = "_Program", DbType = "nvarchar(50)")]
        public string Program
        {
            get => _Program;

            set
            {
                if (_Program != value)
                {
                    _Program = value;
                }
            }
        }

        [Column(Name = "ProgramId", Storage = "_ProgramId", DbType = "int")]
        public int? ProgramId
        {
            get => _ProgramId;

            set
            {
                if (_ProgramId != value)
                {
                    _ProgramId = value;
                }
            }
        }

        [Column(Name = "Division", Storage = "_Division", DbType = "nvarchar(50)")]
        public string Division
        {
            get => _Division;

            set
            {
                if (_Division != value)
                {
                    _Division = value;
                }
            }
        }

        [Column(Name = "Divisions", Storage = "_Divisions", DbType = "nvarchar")]
        public string Divisions
        {
            get => _Divisions;

            set
            {
                if (_Divisions != value)
                {
                    _Divisions = value;
                }
            }
        }

        [Column(Name = "ScheduleDescription", Storage = "_ScheduleDescription", DbType = "nvarchar(20)")]
        public string ScheduleDescription
        {
            get => _ScheduleDescription;

            set
            {
                if (_ScheduleDescription != value)
                {
                    _ScheduleDescription = value;
                }
            }
        }

        [Column(Name = "ScheduleId", Storage = "_ScheduleId", DbType = "int")]
        public int? ScheduleId
        {
            get => _ScheduleId;

            set
            {
                if (_ScheduleId != value)
                {
                    _ScheduleId = value;
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

        [Column(Name = "Campus", Storage = "_Campus", DbType = "nvarchar(100)")]
        public string Campus
        {
            get => _Campus;

            set
            {
                if (_Campus != value)
                {
                    _Campus = value;
                }
            }
        }

        [Column(Name = "LeaderType", Storage = "_LeaderType", DbType = "nvarchar(100)")]
        public string LeaderType
        {
            get => _LeaderType;

            set
            {
                if (_LeaderType != value)
                {
                    _LeaderType = value;
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

        [Column(Name = "ClassFilled", Storage = "_ClassFilled", DbType = "bit")]
        public bool? ClassFilled
        {
            get => _ClassFilled;

            set
            {
                if (_ClassFilled != value)
                {
                    _ClassFilled = value;
                }
            }
        }

        [Column(Name = "RegistrationClosed", Storage = "_RegistrationClosed", DbType = "bit")]
        public bool? RegistrationClosed
        {
            get => _RegistrationClosed;

            set
            {
                if (_RegistrationClosed != value)
                {
                    _RegistrationClosed = value;
                }
            }
        }

        [Column(Name = "AppCategory", Storage = "_AppCategory", DbType = "varchar(15)")]
        public string AppCategory
        {
            get => _AppCategory;

            set
            {
                if (_AppCategory != value)
                {
                    _AppCategory = value;
                }
            }
        }

        [Column(Name = "RegStart", Storage = "_RegStart", DbType = "datetime")]
        public DateTime? RegStart
        {
            get => _RegStart;

            set
            {
                if (_RegStart != value)
                {
                    _RegStart = value;
                }
            }
        }

        [Column(Name = "RegEnd", Storage = "_RegEnd", DbType = "datetime")]
        public DateTime? RegEnd
        {
            get => _RegEnd;

            set
            {
                if (_RegEnd != value)
                {
                    _RegEnd = value;
                }
            }
        }

        [Column(Name = "PublicSortOrder", Storage = "_PublicSortOrder", DbType = "varchar(15)")]
        public string PublicSortOrder
        {
            get => _PublicSortOrder;

            set
            {
                if (_PublicSortOrder != value)
                {
                    _PublicSortOrder = value;
                }
            }
        }

        [Column(Name = "FirstMeetingDate", Storage = "_FirstMeetingDate", DbType = "datetime")]
        public DateTime? FirstMeetingDate
        {
            get => _FirstMeetingDate;

            set
            {
                if (_FirstMeetingDate != value)
                {
                    _FirstMeetingDate = value;
                }
            }
        }

        [Column(Name = "LastMeetingDate", Storage = "_LastMeetingDate", DbType = "datetime")]
        public DateTime? LastMeetingDate
        {
            get => _LastMeetingDate;

            set
            {
                if (_LastMeetingDate != value)
                {
                    _LastMeetingDate = value;
                }
            }
        }

        [Column(Name = "MemberCount", Storage = "_MemberCount", DbType = "int")]
        public int? MemberCount
        {
            get => _MemberCount;

            set
            {
                if (_MemberCount != value)
                {
                    _MemberCount = value;
                }
            }
        }

        [Column(Name = "RegistrationTypeId", Storage = "_RegistrationTypeId", DbType = "int")]
        public int? RegistrationTypeId
        {
            get => _RegistrationTypeId;

            set
            {
                if (_RegistrationTypeId != value)
                {
                    _RegistrationTypeId = value;
                }
            }
        }

        [Column(Name = "CanSelfCheckin", Storage = "_CanSelfCheckin", DbType = "bit")]
        public bool? CanSelfCheckin
        {
            get => _CanSelfCheckin;

            set
            {
                if (_CanSelfCheckin != value)
                {
                    _CanSelfCheckin = value;
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

        [Column(Name = "PrevMemberCount", Storage = "_PrevMemberCount", DbType = "int")]
        public int? PrevMemberCount
        {
            get => _PrevMemberCount;

            set
            {
                if (_PrevMemberCount != value)
                {
                    _PrevMemberCount = value;
                }
            }
        }

        [Column(Name = "ProspectCount", Storage = "_ProspectCount", DbType = "int")]
        public int? ProspectCount
        {
            get => _ProspectCount;

            set
            {
                if (_ProspectCount != value)
                {
                    _ProspectCount = value;
                }
            }
        }

        [Column(Name = "Description", Storage = "_Description", DbType = "nvarchar")]
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

        [Column(Name = "UseRegisterLink2", Storage = "_UseRegisterLink2", DbType = "bit")]
        public bool? UseRegisterLink2
        {
            get => _UseRegisterLink2;

            set
            {
                if (_UseRegisterLink2 != value)
                {
                    _UseRegisterLink2 = value;
                }
            }
        }

        [Column(Name = "DivisionId", Storage = "_DivisionId", DbType = "int")]
        public int? DivisionId
        {
            get => _DivisionId;

            set
            {
                if (_DivisionId != value)
                {
                    _DivisionId = value;
                }
            }
        }

        [Column(Name = "BirthDayStart", Storage = "_BirthDayStart", DbType = "datetime")]
        public DateTime? BirthDayStart
        {
            get => _BirthDayStart;

            set
            {
                if (_BirthDayStart != value)
                {
                    _BirthDayStart = value;
                }
            }
        }

        [Column(Name = "BirthDayEnd", Storage = "_BirthDayEnd", DbType = "datetime")]
        public DateTime? BirthDayEnd
        {
            get => _BirthDayEnd;

            set
            {
                if (_BirthDayEnd != value)
                {
                    _BirthDayEnd = value;
                }
            }
        }

        [Column(Name = "Tag", Storage = "_Tag", DbType = "varchar(6) NOT NULL")]
        public string Tag
        {
            get => _Tag;

            set
            {
                if (_Tag != value)
                {
                    _Tag = value;
                }
            }
        }

        [Column(Name = "ChangeMain", Storage = "_ChangeMain", DbType = "int NOT NULL")]
        public int ChangeMain
        {
            get => _ChangeMain;

            set
            {
                if (_ChangeMain != value)
                {
                    _ChangeMain = value;
                }
            }
        }
    }
}
