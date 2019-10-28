using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpPeople")]
    public partial class XpPerson
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private int _FamilyId;

        private string _TitleCode;

        private string _FirstName;

        private string _MiddleName;

        private string _MaidenName;

        private string _LastName;

        private string _SuffixCode;

        private string _NickName;

        private string _AddressLineOne;

        private string _AddressLineTwo;

        private string _CityName;

        private string _StateCode;

        private string _ZipCode;

        private string _CountryName;

        private string _CellPhone;

        private string _WorkPhone;

        private string _HomePhone;

        private string _EmailAddress;

        private string _EmailAddress2;

        private bool? _SendEmailAddress1;

        private bool? _SendEmailAddress2;

        private int? _BirthMonth;

        private int? _BirthDay;

        private int? _BirthYear;

        private string _Gender;

        private string _PositionInFamily;

        private bool _DoNotMailFlag;

        private bool _DoNotCallFlag;

        private bool _DoNotVisitFlag;

        private string _AddressType;

        private string _MaritalStatus;

        private string _MemberStatus;

        private string _DropType;

        private string _Origin;

        private string _EntryPoint;

        private string _InterestPoint;

        private string _BaptismType;

        private string _BaptismStatus;

        private string _DecisionTypeId;

        private string _NewMemberClassStatus;

        private string _LetterStatus;

        private string _JoinCode;

        private string _EnvelopeOption;

        private string _ResCode;

        private DateTime? _WeddingDate;

        private DateTime? _OriginDate;

        private DateTime? _BaptismSchedDate;

        private DateTime? _BaptismDate;

        private DateTime? _DecisionDate;

        private DateTime? _LetterDateRequested;

        private DateTime? _LetterDateReceived;

        private DateTime? _JoinDate;

        private DateTime? _DropDate;

        private DateTime? _DeceasedDate;

        private string _OtherPreviousChurch;

        private string _OtherNewChurch;

        private string _SchoolOther;

        private string _EmployerOther;

        private string _OccupationOther;

        private string _HobbyOther;

        private string _SkillOther;

        private string _InterestOther;

        private string _LetterStatusNotes;

        private string _Comments;

        private bool _ContributionsStatement;

        private string _StatementOption;

        private int? _SpouseId;

        private int? _Grade;

        private int? _BibleFellowshipClassId;

        private int? _CampusId;

        private string _AltName;

        private bool? _CustodyIssue;

        private bool? _OkTransport;

        private DateTime? _NewMemberClassDate;

        private bool _ReceiveSMS;

        private bool? _DoNotPublishPhones;

        private bool? _ElectronicStatement;

        public XpPerson()
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

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int NOT NULL")]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    _FamilyId = value;
                }
            }
        }

        [Column(Name = "TitleCode", Storage = "_TitleCode", DbType = "nvarchar(10)")]
        public string TitleCode
        {
            get => _TitleCode;

            set
            {
                if (_TitleCode != value)
                {
                    _TitleCode = value;
                }
            }
        }

        [Column(Name = "FirstName", Storage = "_FirstName", DbType = "nvarchar(25)")]
        public string FirstName
        {
            get => _FirstName;

            set
            {
                if (_FirstName != value)
                {
                    _FirstName = value;
                }
            }
        }

        [Column(Name = "MiddleName", Storage = "_MiddleName", DbType = "nvarchar(30)")]
        public string MiddleName
        {
            get => _MiddleName;

            set
            {
                if (_MiddleName != value)
                {
                    _MiddleName = value;
                }
            }
        }

        [Column(Name = "MaidenName", Storage = "_MaidenName", DbType = "nvarchar(20)")]
        public string MaidenName
        {
            get => _MaidenName;

            set
            {
                if (_MaidenName != value)
                {
                    _MaidenName = value;
                }
            }
        }

        [Column(Name = "LastName", Storage = "_LastName", DbType = "nvarchar(100) NOT NULL")]
        public string LastName
        {
            get => _LastName;

            set
            {
                if (_LastName != value)
                {
                    _LastName = value;
                }
            }
        }

        [Column(Name = "SuffixCode", Storage = "_SuffixCode", DbType = "nvarchar(10)")]
        public string SuffixCode
        {
            get => _SuffixCode;

            set
            {
                if (_SuffixCode != value)
                {
                    _SuffixCode = value;
                }
            }
        }

        [Column(Name = "NickName", Storage = "_NickName", DbType = "nvarchar(25)")]
        public string NickName
        {
            get => _NickName;

            set
            {
                if (_NickName != value)
                {
                    _NickName = value;
                }
            }
        }

        [Column(Name = "AddressLineOne", Storage = "_AddressLineOne", DbType = "nvarchar(100)")]
        public string AddressLineOne
        {
            get => _AddressLineOne;

            set
            {
                if (_AddressLineOne != value)
                {
                    _AddressLineOne = value;
                }
            }
        }

        [Column(Name = "AddressLineTwo", Storage = "_AddressLineTwo", DbType = "nvarchar(100)")]
        public string AddressLineTwo
        {
            get => _AddressLineTwo;

            set
            {
                if (_AddressLineTwo != value)
                {
                    _AddressLineTwo = value;
                }
            }
        }

        [Column(Name = "CityName", Storage = "_CityName", DbType = "nvarchar(30)")]
        public string CityName
        {
            get => _CityName;

            set
            {
                if (_CityName != value)
                {
                    _CityName = value;
                }
            }
        }

        [Column(Name = "StateCode", Storage = "_StateCode", DbType = "nvarchar(30)")]
        public string StateCode
        {
            get => _StateCode;

            set
            {
                if (_StateCode != value)
                {
                    _StateCode = value;
                }
            }
        }

        [Column(Name = "ZipCode", Storage = "_ZipCode", DbType = "nvarchar(15)")]
        public string ZipCode
        {
            get => _ZipCode;

            set
            {
                if (_ZipCode != value)
                {
                    _ZipCode = value;
                }
            }
        }

        [Column(Name = "CountryName", Storage = "_CountryName", DbType = "nvarchar(40)")]
        public string CountryName
        {
            get => _CountryName;

            set
            {
                if (_CountryName != value)
                {
                    _CountryName = value;
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

        [Column(Name = "EmailAddress2", Storage = "_EmailAddress2", DbType = "nvarchar(60)")]
        public string EmailAddress2
        {
            get => _EmailAddress2;

            set
            {
                if (_EmailAddress2 != value)
                {
                    _EmailAddress2 = value;
                }
            }
        }

        [Column(Name = "SendEmailAddress1", Storage = "_SendEmailAddress1", DbType = "bit")]
        public bool? SendEmailAddress1
        {
            get => _SendEmailAddress1;

            set
            {
                if (_SendEmailAddress1 != value)
                {
                    _SendEmailAddress1 = value;
                }
            }
        }

        [Column(Name = "SendEmailAddress2", Storage = "_SendEmailAddress2", DbType = "bit")]
        public bool? SendEmailAddress2
        {
            get => _SendEmailAddress2;

            set
            {
                if (_SendEmailAddress2 != value)
                {
                    _SendEmailAddress2 = value;
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

        [Column(Name = "PositionInFamily", Storage = "_PositionInFamily", DbType = "nvarchar(100)")]
        public string PositionInFamily
        {
            get => _PositionInFamily;

            set
            {
                if (_PositionInFamily != value)
                {
                    _PositionInFamily = value;
                }
            }
        }

        [Column(Name = "DoNotMailFlag", Storage = "_DoNotMailFlag", DbType = "bit NOT NULL")]
        public bool DoNotMailFlag
        {
            get => _DoNotMailFlag;

            set
            {
                if (_DoNotMailFlag != value)
                {
                    _DoNotMailFlag = value;
                }
            }
        }

        [Column(Name = "DoNotCallFlag", Storage = "_DoNotCallFlag", DbType = "bit NOT NULL")]
        public bool DoNotCallFlag
        {
            get => _DoNotCallFlag;

            set
            {
                if (_DoNotCallFlag != value)
                {
                    _DoNotCallFlag = value;
                }
            }
        }

        [Column(Name = "DoNotVisitFlag", Storage = "_DoNotVisitFlag", DbType = "bit NOT NULL")]
        public bool DoNotVisitFlag
        {
            get => _DoNotVisitFlag;

            set
            {
                if (_DoNotVisitFlag != value)
                {
                    _DoNotVisitFlag = value;
                }
            }
        }

        [Column(Name = "AddressType", Storage = "_AddressType", DbType = "nvarchar(100)")]
        public string AddressType
        {
            get => _AddressType;

            set
            {
                if (_AddressType != value)
                {
                    _AddressType = value;
                }
            }
        }

        [Column(Name = "MaritalStatus", Storage = "_MaritalStatus", DbType = "nvarchar(100)")]
        public string MaritalStatus
        {
            get => _MaritalStatus;

            set
            {
                if (_MaritalStatus != value)
                {
                    _MaritalStatus = value;
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

        [Column(Name = "DropType", Storage = "_DropType", DbType = "nvarchar(100)")]
        public string DropType
        {
            get => _DropType;

            set
            {
                if (_DropType != value)
                {
                    _DropType = value;
                }
            }
        }

        [Column(Name = "Origin", Storage = "_Origin", DbType = "nvarchar(100)")]
        public string Origin
        {
            get => _Origin;

            set
            {
                if (_Origin != value)
                {
                    _Origin = value;
                }
            }
        }

        [Column(Name = "EntryPoint", Storage = "_EntryPoint", DbType = "nvarchar(100)")]
        public string EntryPoint
        {
            get => _EntryPoint;

            set
            {
                if (_EntryPoint != value)
                {
                    _EntryPoint = value;
                }
            }
        }

        [Column(Name = "InterestPoint", Storage = "_InterestPoint", DbType = "nvarchar(100)")]
        public string InterestPoint
        {
            get => _InterestPoint;

            set
            {
                if (_InterestPoint != value)
                {
                    _InterestPoint = value;
                }
            }
        }

        [Column(Name = "BaptismType", Storage = "_BaptismType", DbType = "nvarchar(100)")]
        public string BaptismType
        {
            get => _BaptismType;

            set
            {
                if (_BaptismType != value)
                {
                    _BaptismType = value;
                }
            }
        }

        [Column(Name = "BaptismStatus", Storage = "_BaptismStatus", DbType = "nvarchar(100)")]
        public string BaptismStatus
        {
            get => _BaptismStatus;

            set
            {
                if (_BaptismStatus != value)
                {
                    _BaptismStatus = value;
                }
            }
        }

        [Column(Name = "DecisionTypeId", Storage = "_DecisionTypeId", DbType = "nvarchar(100)")]
        public string DecisionTypeId
        {
            get => _DecisionTypeId;

            set
            {
                if (_DecisionTypeId != value)
                {
                    _DecisionTypeId = value;
                }
            }
        }

        [Column(Name = "NewMemberClassStatus", Storage = "_NewMemberClassStatus", DbType = "nvarchar(100)")]
        public string NewMemberClassStatus
        {
            get => _NewMemberClassStatus;

            set
            {
                if (_NewMemberClassStatus != value)
                {
                    _NewMemberClassStatus = value;
                }
            }
        }

        [Column(Name = "LetterStatus", Storage = "_LetterStatus", DbType = "nvarchar(100)")]
        public string LetterStatus
        {
            get => _LetterStatus;

            set
            {
                if (_LetterStatus != value)
                {
                    _LetterStatus = value;
                }
            }
        }

        [Column(Name = "JoinCode", Storage = "_JoinCode", DbType = "nvarchar(100)")]
        public string JoinCode
        {
            get => _JoinCode;

            set
            {
                if (_JoinCode != value)
                {
                    _JoinCode = value;
                }
            }
        }

        [Column(Name = "EnvelopeOption", Storage = "_EnvelopeOption", DbType = "nvarchar(100)")]
        public string EnvelopeOption
        {
            get => _EnvelopeOption;

            set
            {
                if (_EnvelopeOption != value)
                {
                    _EnvelopeOption = value;
                }
            }
        }

        [Column(Name = "ResCode", Storage = "_ResCode", DbType = "nvarchar(100)")]
        public string ResCode
        {
            get => _ResCode;

            set
            {
                if (_ResCode != value)
                {
                    _ResCode = value;
                }
            }
        }

        [Column(Name = "WeddingDate", Storage = "_WeddingDate", DbType = "datetime")]
        public DateTime? WeddingDate
        {
            get => _WeddingDate;

            set
            {
                if (_WeddingDate != value)
                {
                    _WeddingDate = value;
                }
            }
        }

        [Column(Name = "OriginDate", Storage = "_OriginDate", DbType = "datetime")]
        public DateTime? OriginDate
        {
            get => _OriginDate;

            set
            {
                if (_OriginDate != value)
                {
                    _OriginDate = value;
                }
            }
        }

        [Column(Name = "BaptismSchedDate", Storage = "_BaptismSchedDate", DbType = "datetime")]
        public DateTime? BaptismSchedDate
        {
            get => _BaptismSchedDate;

            set
            {
                if (_BaptismSchedDate != value)
                {
                    _BaptismSchedDate = value;
                }
            }
        }

        [Column(Name = "BaptismDate", Storage = "_BaptismDate", DbType = "datetime")]
        public DateTime? BaptismDate
        {
            get => _BaptismDate;

            set
            {
                if (_BaptismDate != value)
                {
                    _BaptismDate = value;
                }
            }
        }

        [Column(Name = "DecisionDate", Storage = "_DecisionDate", DbType = "datetime")]
        public DateTime? DecisionDate
        {
            get => _DecisionDate;

            set
            {
                if (_DecisionDate != value)
                {
                    _DecisionDate = value;
                }
            }
        }

        [Column(Name = "LetterDateRequested", Storage = "_LetterDateRequested", DbType = "datetime")]
        public DateTime? LetterDateRequested
        {
            get => _LetterDateRequested;

            set
            {
                if (_LetterDateRequested != value)
                {
                    _LetterDateRequested = value;
                }
            }
        }

        [Column(Name = "LetterDateReceived", Storage = "_LetterDateReceived", DbType = "datetime")]
        public DateTime? LetterDateReceived
        {
            get => _LetterDateReceived;

            set
            {
                if (_LetterDateReceived != value)
                {
                    _LetterDateReceived = value;
                }
            }
        }

        [Column(Name = "JoinDate", Storage = "_JoinDate", DbType = "datetime")]
        public DateTime? JoinDate
        {
            get => _JoinDate;

            set
            {
                if (_JoinDate != value)
                {
                    _JoinDate = value;
                }
            }
        }

        [Column(Name = "DropDate", Storage = "_DropDate", DbType = "datetime")]
        public DateTime? DropDate
        {
            get => _DropDate;

            set
            {
                if (_DropDate != value)
                {
                    _DropDate = value;
                }
            }
        }

        [Column(Name = "DeceasedDate", Storage = "_DeceasedDate", DbType = "datetime")]
        public DateTime? DeceasedDate
        {
            get => _DeceasedDate;

            set
            {
                if (_DeceasedDate != value)
                {
                    _DeceasedDate = value;
                }
            }
        }

        [Column(Name = "OtherPreviousChurch", Storage = "_OtherPreviousChurch", DbType = "nvarchar(120)")]
        public string OtherPreviousChurch
        {
            get => _OtherPreviousChurch;

            set
            {
                if (_OtherPreviousChurch != value)
                {
                    _OtherPreviousChurch = value;
                }
            }
        }

        [Column(Name = "OtherNewChurch", Storage = "_OtherNewChurch", DbType = "nvarchar(60)")]
        public string OtherNewChurch
        {
            get => _OtherNewChurch;

            set
            {
                if (_OtherNewChurch != value)
                {
                    _OtherNewChurch = value;
                }
            }
        }

        [Column(Name = "SchoolOther", Storage = "_SchoolOther", DbType = "nvarchar(100)")]
        public string SchoolOther
        {
            get => _SchoolOther;

            set
            {
                if (_SchoolOther != value)
                {
                    _SchoolOther = value;
                }
            }
        }

        [Column(Name = "EmployerOther", Storage = "_EmployerOther", DbType = "nvarchar(120)")]
        public string EmployerOther
        {
            get => _EmployerOther;

            set
            {
                if (_EmployerOther != value)
                {
                    _EmployerOther = value;
                }
            }
        }

        [Column(Name = "OccupationOther", Storage = "_OccupationOther", DbType = "nvarchar(120)")]
        public string OccupationOther
        {
            get => _OccupationOther;

            set
            {
                if (_OccupationOther != value)
                {
                    _OccupationOther = value;
                }
            }
        }

        [Column(Name = "HobbyOther", Storage = "_HobbyOther", DbType = "nvarchar(40)")]
        public string HobbyOther
        {
            get => _HobbyOther;

            set
            {
                if (_HobbyOther != value)
                {
                    _HobbyOther = value;
                }
            }
        }

        [Column(Name = "SkillOther", Storage = "_SkillOther", DbType = "nvarchar(40)")]
        public string SkillOther
        {
            get => _SkillOther;

            set
            {
                if (_SkillOther != value)
                {
                    _SkillOther = value;
                }
            }
        }

        [Column(Name = "InterestOther", Storage = "_InterestOther", DbType = "nvarchar(40)")]
        public string InterestOther
        {
            get => _InterestOther;

            set
            {
                if (_InterestOther != value)
                {
                    _InterestOther = value;
                }
            }
        }

        [Column(Name = "LetterStatusNotes", Storage = "_LetterStatusNotes", DbType = "nvarchar(3000)")]
        public string LetterStatusNotes
        {
            get => _LetterStatusNotes;

            set
            {
                if (_LetterStatusNotes != value)
                {
                    _LetterStatusNotes = value;
                }
            }
        }

        [Column(Name = "Comments", Storage = "_Comments", DbType = "nvarchar(3000)")]
        public string Comments
        {
            get => _Comments;

            set
            {
                if (_Comments != value)
                {
                    _Comments = value;
                }
            }
        }

        [Column(Name = "ContributionsStatement", Storage = "_ContributionsStatement", DbType = "bit NOT NULL")]
        public bool ContributionsStatement
        {
            get => _ContributionsStatement;

            set
            {
                if (_ContributionsStatement != value)
                {
                    _ContributionsStatement = value;
                }
            }
        }

        [Column(Name = "StatementOption", Storage = "_StatementOption", DbType = "nvarchar(100)")]
        public string StatementOption
        {
            get => _StatementOption;

            set
            {
                if (_StatementOption != value)
                {
                    _StatementOption = value;
                }
            }
        }

        [Column(Name = "SpouseId", Storage = "_SpouseId", DbType = "int")]
        public int? SpouseId
        {
            get => _SpouseId;

            set
            {
                if (_SpouseId != value)
                {
                    _SpouseId = value;
                }
            }
        }

        [Column(Name = "Grade", Storage = "_Grade", DbType = "int")]
        public int? Grade
        {
            get => _Grade;

            set
            {
                if (_Grade != value)
                {
                    _Grade = value;
                }
            }
        }

        [Column(Name = "BibleFellowshipClassId", Storage = "_BibleFellowshipClassId", DbType = "int")]
        public int? BibleFellowshipClassId
        {
            get => _BibleFellowshipClassId;

            set
            {
                if (_BibleFellowshipClassId != value)
                {
                    _BibleFellowshipClassId = value;
                }
            }
        }

        [Column(Name = "CampusId", Storage = "_CampusId", DbType = "int")]
        public int? CampusId
        {
            get => _CampusId;

            set
            {
                if (_CampusId != value)
                {
                    _CampusId = value;
                }
            }
        }

        [Column(Name = "AltName", Storage = "_AltName", DbType = "nvarchar(100)")]
        public string AltName
        {
            get => _AltName;

            set
            {
                if (_AltName != value)
                {
                    _AltName = value;
                }
            }
        }

        [Column(Name = "CustodyIssue", Storage = "_CustodyIssue", DbType = "bit")]
        public bool? CustodyIssue
        {
            get => _CustodyIssue;

            set
            {
                if (_CustodyIssue != value)
                {
                    _CustodyIssue = value;
                }
            }
        }

        [Column(Name = "OkTransport", Storage = "_OkTransport", DbType = "bit")]
        public bool? OkTransport
        {
            get => _OkTransport;

            set
            {
                if (_OkTransport != value)
                {
                    _OkTransport = value;
                }
            }
        }

        [Column(Name = "NewMemberClassDate", Storage = "_NewMemberClassDate", DbType = "datetime")]
        public DateTime? NewMemberClassDate
        {
            get => _NewMemberClassDate;

            set
            {
                if (_NewMemberClassDate != value)
                {
                    _NewMemberClassDate = value;
                }
            }
        }

        [Column(Name = "ReceiveSMS", Storage = "_ReceiveSMS", DbType = "bit NOT NULL")]
        public bool ReceiveSMS
        {
            get => _ReceiveSMS;

            set
            {
                if (_ReceiveSMS != value)
                {
                    _ReceiveSMS = value;
                }
            }
        }

        [Column(Name = "DoNotPublishPhones", Storage = "_DoNotPublishPhones", DbType = "bit")]
        public bool? DoNotPublishPhones
        {
            get => _DoNotPublishPhones;

            set
            {
                if (_DoNotPublishPhones != value)
                {
                    _DoNotPublishPhones = value;
                }
            }
        }

        [Column(Name = "ElectronicStatement", Storage = "_ElectronicStatement", DbType = "bit")]
        public bool? ElectronicStatement
        {
            get => _ElectronicStatement;

            set
            {
                if (_ElectronicStatement != value)
                {
                    _ElectronicStatement = value;
                }
            }
        }
    }
}
