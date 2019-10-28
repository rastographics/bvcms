using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgFilterPeople")]
    public partial class OrgFilterPerson
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _PeopleId;

        private string _Tab;

        private string _GroupCode;

        private string _Name;

        private string _Name2;

        private int? _Age;

        private int? _BirthDay;

        private int? _BirthMonth;

        private int? _BirthYear;

        private bool? _IsDeceased;

        private string _Address;

        private string _Address2;

        private string _City;

        private string _St;

        private string _Zip;

        private string _EmailAddress;

        private string _HomePhone;

        private string _CellPhone;

        private string _WorkPhone;

        private string _MemberStatus;

        private int? _LeaderId;

        private string _LeaderName;

        private bool? _HasTag;

        private bool? _IsChecked;

        private decimal? _AttPct;

        private DateTime? _LastAttended;

        private DateTime? _Joined;

        private DateTime? _Dropped;

        private DateTime? _InactiveDate;

        private string _MemberCode;

        private string _MemberType;

        private bool? _Hidden;

        private string _Groups;

        private DateTime? _LastContactMadeDt;

        private int? _LastContactMadeId;

        private DateTime? _LastContactReceivedDt;

        private int? _LastContactReceivedId;

        private DateTime? _TaskAboutDt;

        private int? _TaskAboutId;

        private DateTime? _TaskDelegatedDt;

        private int? _TaskDelegatedId;

        public OrgFilterPerson()
        {
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
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

        [Column(Name = "Tab", Storage = "_Tab", DbType = "varchar(30)")]
        public string Tab
        {
            get => _Tab;

            set
            {
                if (_Tab != value)
                {
                    _Tab = value;
                }
            }
        }

        [Column(Name = "GroupCode", Storage = "_GroupCode", DbType = "char(2)")]
        public string GroupCode
        {
            get => _GroupCode;

            set
            {
                if (_GroupCode != value)
                {
                    _GroupCode = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(138)")]
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

        [Column(Name = "Age", Storage = "_Age", DbType = "int")]
        public int? Age
        {
            get => _Age;

            set
            {
                if (_Age != value)
                {
                    _Age = value;
                }
            }
        }

        [Column(Name = "BirthDay", Storage = "_BirthDay", DbType = "int")]
        public int? BirthDay
        {
            get => _BirthDay;

            set
            {
                if (_BirthDay != value)
                {
                    _BirthDay = value;
                }
            }
        }

        [Column(Name = "BirthMonth", Storage = "_BirthMonth", DbType = "int")]
        public int? BirthMonth
        {
            get => _BirthMonth;

            set
            {
                if (_BirthMonth != value)
                {
                    _BirthMonth = value;
                }
            }
        }

        [Column(Name = "BirthYear", Storage = "_BirthYear", DbType = "int")]
        public int? BirthYear
        {
            get => _BirthYear;

            set
            {
                if (_BirthYear != value)
                {
                    _BirthYear = value;
                }
            }
        }

        [Column(Name = "IsDeceased", Storage = "_IsDeceased", DbType = "bit")]
        public bool? IsDeceased
        {
            get => _IsDeceased;

            set
            {
                if (_IsDeceased != value)
                {
                    _IsDeceased = value;
                }
            }
        }

        [Column(Name = "Address", Storage = "_Address", DbType = "nvarchar(100)")]
        public string Address
        {
            get => _Address;

            set
            {
                if (_Address != value)
                {
                    _Address = value;
                }
            }
        }

        [Column(Name = "Address2", Storage = "_Address2", DbType = "nvarchar(100)")]
        public string Address2
        {
            get => _Address2;

            set
            {
                if (_Address2 != value)
                {
                    _Address2 = value;
                }
            }
        }

        [Column(Name = "City", Storage = "_City", DbType = "nvarchar(30)")]
        public string City
        {
            get => _City;

            set
            {
                if (_City != value)
                {
                    _City = value;
                }
            }
        }

        [Column(Name = "ST", Storage = "_St", DbType = "nvarchar(20)")]
        public string St
        {
            get => _St;

            set
            {
                if (_St != value)
                {
                    _St = value;
                }
            }
        }

        [Column(Name = "Zip", Storage = "_Zip", DbType = "nvarchar(15)")]
        public string Zip
        {
            get => _Zip;

            set
            {
                if (_Zip != value)
                {
                    _Zip = value;
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

        [Column(Name = "WorkPhone", Storage = "_WorkPhone", DbType = "nvarchar(20)")]
        public string WorkPhone
        {
            get => _WorkPhone;

            set
            {
                if (_WorkPhone != value)
                {
                    _WorkPhone = value;
                }
            }
        }

        [Column(Name = "MemberStatus", Storage = "_MemberStatus", DbType = "nvarchar(50)")]
        public string MemberStatus
        {
            get => _MemberStatus;

            set
            {
                if (_MemberStatus != value)
                {
                    _MemberStatus = value;
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

        [Column(Name = "HasTag", Storage = "_HasTag", DbType = "bit")]
        public bool? HasTag
        {
            get => _HasTag;

            set
            {
                if (_HasTag != value)
                {
                    _HasTag = value;
                }
            }
        }

        [Column(Name = "IsChecked", Storage = "_IsChecked", DbType = "bit")]
        public bool? IsChecked
        {
            get => _IsChecked;

            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                }
            }
        }

        [Column(Name = "AttPct", Storage = "_AttPct", DbType = "real")]
        public decimal? AttPct
        {
            get => _AttPct;

            set
            {
                if (_AttPct != value)
                {
                    _AttPct = value;
                }
            }
        }

        [Column(Name = "LastAttended", Storage = "_LastAttended", DbType = "datetime")]
        public DateTime? LastAttended
        {
            get => _LastAttended;

            set
            {
                if (_LastAttended != value)
                {
                    _LastAttended = value;
                }
            }
        }

        [Column(Name = "Joined", Storage = "_Joined", DbType = "datetime")]
        public DateTime? Joined
        {
            get => _Joined;

            set
            {
                if (_Joined != value)
                {
                    _Joined = value;
                }
            }
        }

        [Column(Name = "Dropped", Storage = "_Dropped", DbType = "datetime")]
        public DateTime? Dropped
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

        [Column(Name = "InactiveDate", Storage = "_InactiveDate", DbType = "datetime")]
        public DateTime? InactiveDate
        {
            get => _InactiveDate;

            set
            {
                if (_InactiveDate != value)
                {
                    _InactiveDate = value;
                }
            }
        }

        [Column(Name = "MemberCode", Storage = "_MemberCode", DbType = "nvarchar(20)")]
        public string MemberCode
        {
            get => _MemberCode;

            set
            {
                if (_MemberCode != value)
                {
                    _MemberCode = value;
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

        [Column(Name = "Hidden", Storage = "_Hidden", DbType = "bit")]
        public bool? Hidden
        {
            get => _Hidden;

            set
            {
                if (_Hidden != value)
                {
                    _Hidden = value;
                }
            }
        }

        [Column(Name = "Groups", Storage = "_Groups", DbType = "nvarchar")]
        public string Groups
        {
            get => _Groups;

            set
            {
                if (_Groups != value)
                {
                    _Groups = value;
                }
            }
        }

        [Column(Name = "LastContactMadeDt", Storage = "_LastContactMadeDt", DbType = "datetime")]
        public DateTime? LastContactMadeDt
        {
            get => _LastContactMadeDt;

            set
            {
                if (_LastContactMadeDt != value)
                {
                    _LastContactMadeDt = value;
                }
            }
        }

        [Column(Name = "LastContactMadeId", Storage = "_LastContactMadeId", DbType = "int")]
        public int? LastContactMadeId
        {
            get => _LastContactMadeId;

            set
            {
                if (_LastContactMadeId != value)
                {
                    _LastContactMadeId = value;
                }
            }
        }

        [Column(Name = "LastContactReceivedDt", Storage = "_LastContactReceivedDt", DbType = "datetime")]
        public DateTime? LastContactReceivedDt
        {
            get => _LastContactReceivedDt;

            set
            {
                if (_LastContactReceivedDt != value)
                {
                    _LastContactReceivedDt = value;
                }
            }
        }

        [Column(Name = "LastContactReceivedId", Storage = "_LastContactReceivedId", DbType = "int")]
        public int? LastContactReceivedId
        {
            get => _LastContactReceivedId;

            set
            {
                if (_LastContactReceivedId != value)
                {
                    _LastContactReceivedId = value;
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
    }
}
