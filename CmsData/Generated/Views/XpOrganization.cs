using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpOrganization")]
    public partial class XpOrganization
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrganizationName;

        private string _Location;

        private string _OrganizationStatus;

        private int? _DivisionId;

        private string _LeaderMemberType;

        private int? _GradeAgeStart;

        private int? _GradeAgeEnd;

        private DateTime? _FirstMeetingDate;

        private DateTime? _LastMeetingDate;

        private int? _ParentOrgId;

        private int? _LeaderId;

        private string _Gender;

        private string _Description;

        private DateTime? _BirthDayStart;

        private DateTime? _BirthDayEnd;

        private string _PhoneNumber;

        private bool? _IsBibleFellowshipOrg;

        private bool? _Offsite;

        private string _OrganizationType;

        private int? _PublishDirectory;

        private bool _IsRecreationTeam;

        private bool? _NotWeekly;

        private bool? _IsMissionTrip;

        public XpOrganization()
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

        [Column(Name = "OrganizationStatus", Storage = "_OrganizationStatus", DbType = "nvarchar(50)")]
        public string OrganizationStatus
        {
            get => _OrganizationStatus;

            set
            {
                if (_OrganizationStatus != value)
                {
                    _OrganizationStatus = value;
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

        [Column(Name = "LeaderMemberType", Storage = "_LeaderMemberType", DbType = "nvarchar(100)")]
        public string LeaderMemberType
        {
            get => _LeaderMemberType;

            set
            {
                if (_LeaderMemberType != value)
                {
                    _LeaderMemberType = value;
                }
            }
        }

        [Column(Name = "GradeAgeStart", Storage = "_GradeAgeStart", DbType = "int")]
        public int? GradeAgeStart
        {
            get => _GradeAgeStart;

            set
            {
                if (_GradeAgeStart != value)
                {
                    _GradeAgeStart = value;
                }
            }
        }

        [Column(Name = "GradeAgeEnd", Storage = "_GradeAgeEnd", DbType = "int")]
        public int? GradeAgeEnd
        {
            get => _GradeAgeEnd;

            set
            {
                if (_GradeAgeEnd != value)
                {
                    _GradeAgeEnd = value;
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

        [Column(Name = "ParentOrgId", Storage = "_ParentOrgId", DbType = "int")]
        public int? ParentOrgId
        {
            get => _ParentOrgId;

            set
            {
                if (_ParentOrgId != value)
                {
                    _ParentOrgId = value;
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

        [Column(Name = "Gender", Storage = "_Gender", DbType = "nvarchar(100)")]
        public string Gender
        {
            get => _Gender;

            set
            {
                if (_Gender != value)
                {
                    _Gender = value;
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

        [Column(Name = "PhoneNumber", Storage = "_PhoneNumber", DbType = "nvarchar(25)")]
        public string PhoneNumber
        {
            get => _PhoneNumber;

            set
            {
                if (_PhoneNumber != value)
                {
                    _PhoneNumber = value;
                }
            }
        }

        [Column(Name = "IsBibleFellowshipOrg", Storage = "_IsBibleFellowshipOrg", DbType = "bit")]
        public bool? IsBibleFellowshipOrg
        {
            get => _IsBibleFellowshipOrg;

            set
            {
                if (_IsBibleFellowshipOrg != value)
                {
                    _IsBibleFellowshipOrg = value;
                }
            }
        }

        [Column(Name = "Offsite", Storage = "_Offsite", DbType = "bit")]
        public bool? Offsite
        {
            get => _Offsite;

            set
            {
                if (_Offsite != value)
                {
                    _Offsite = value;
                }
            }
        }

        [Column(Name = "OrganizationType", Storage = "_OrganizationType", DbType = "nvarchar(50)")]
        public string OrganizationType
        {
            get => _OrganizationType;

            set
            {
                if (_OrganizationType != value)
                {
                    _OrganizationType = value;
                }
            }
        }

        [Column(Name = "PublishDirectory", Storage = "_PublishDirectory", DbType = "int")]
        public int? PublishDirectory
        {
            get => _PublishDirectory;

            set
            {
                if (_PublishDirectory != value)
                {
                    _PublishDirectory = value;
                }
            }
        }

        [Column(Name = "IsRecreationTeam", Storage = "_IsRecreationTeam", DbType = "bit NOT NULL")]
        public bool IsRecreationTeam
        {
            get => _IsRecreationTeam;

            set
            {
                if (_IsRecreationTeam != value)
                {
                    _IsRecreationTeam = value;
                }
            }
        }

        [Column(Name = "NotWeekly", Storage = "_NotWeekly", DbType = "bit")]
        public bool? NotWeekly
        {
            get => _NotWeekly;

            set
            {
                if (_NotWeekly != value)
                {
                    _NotWeekly = value;
                }
            }
        }

        [Column(Name = "IsMissionTrip", Storage = "_IsMissionTrip", DbType = "bit")]
        public bool? IsMissionTrip
        {
            get => _IsMissionTrip;

            set
            {
                if (_IsMissionTrip != value)
                {
                    _IsMissionTrip = value;
                }
            }
        }
    }
}
