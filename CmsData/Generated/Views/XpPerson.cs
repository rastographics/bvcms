using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData.View
{
	[Table(Name="XpPeople")]
	public partial class XpPerson
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
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

		
		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int NOT NULL")]
		public int PeopleId
		{
			get
			{
				return this._PeopleId;
			}

			set
			{
				if (this._PeopleId != value)
					this._PeopleId = value;
			}

		}

		
		[Column(Name="FamilyId", Storage="_FamilyId", DbType="int NOT NULL")]
		public int FamilyId
		{
			get
			{
				return this._FamilyId;
			}

			set
			{
				if (this._FamilyId != value)
					this._FamilyId = value;
			}

		}

		
		[Column(Name="TitleCode", Storage="_TitleCode", DbType="nvarchar(10)")]
		public string TitleCode
		{
			get
			{
				return this._TitleCode;
			}

			set
			{
				if (this._TitleCode != value)
					this._TitleCode = value;
			}

		}

		
		[Column(Name="FirstName", Storage="_FirstName", DbType="nvarchar(25)")]
		public string FirstName
		{
			get
			{
				return this._FirstName;
			}

			set
			{
				if (this._FirstName != value)
					this._FirstName = value;
			}

		}

		
		[Column(Name="MiddleName", Storage="_MiddleName", DbType="nvarchar(30)")]
		public string MiddleName
		{
			get
			{
				return this._MiddleName;
			}

			set
			{
				if (this._MiddleName != value)
					this._MiddleName = value;
			}

		}

		
		[Column(Name="MaidenName", Storage="_MaidenName", DbType="nvarchar(20)")]
		public string MaidenName
		{
			get
			{
				return this._MaidenName;
			}

			set
			{
				if (this._MaidenName != value)
					this._MaidenName = value;
			}

		}

		
		[Column(Name="LastName", Storage="_LastName", DbType="nvarchar(100) NOT NULL")]
		public string LastName
		{
			get
			{
				return this._LastName;
			}

			set
			{
				if (this._LastName != value)
					this._LastName = value;
			}

		}

		
		[Column(Name="SuffixCode", Storage="_SuffixCode", DbType="nvarchar(10)")]
		public string SuffixCode
		{
			get
			{
				return this._SuffixCode;
			}

			set
			{
				if (this._SuffixCode != value)
					this._SuffixCode = value;
			}

		}

		
		[Column(Name="NickName", Storage="_NickName", DbType="nvarchar(25)")]
		public string NickName
		{
			get
			{
				return this._NickName;
			}

			set
			{
				if (this._NickName != value)
					this._NickName = value;
			}

		}

		
		[Column(Name="AddressLineOne", Storage="_AddressLineOne", DbType="nvarchar(100)")]
		public string AddressLineOne
		{
			get
			{
				return this._AddressLineOne;
			}

			set
			{
				if (this._AddressLineOne != value)
					this._AddressLineOne = value;
			}

		}

		
		[Column(Name="AddressLineTwo", Storage="_AddressLineTwo", DbType="nvarchar(100)")]
		public string AddressLineTwo
		{
			get
			{
				return this._AddressLineTwo;
			}

			set
			{
				if (this._AddressLineTwo != value)
					this._AddressLineTwo = value;
			}

		}

		
		[Column(Name="CityName", Storage="_CityName", DbType="nvarchar(30)")]
		public string CityName
		{
			get
			{
				return this._CityName;
			}

			set
			{
				if (this._CityName != value)
					this._CityName = value;
			}

		}

		
		[Column(Name="StateCode", Storage="_StateCode", DbType="nvarchar(30)")]
		public string StateCode
		{
			get
			{
				return this._StateCode;
			}

			set
			{
				if (this._StateCode != value)
					this._StateCode = value;
			}

		}

		
		[Column(Name="ZipCode", Storage="_ZipCode", DbType="nvarchar(15)")]
		public string ZipCode
		{
			get
			{
				return this._ZipCode;
			}

			set
			{
				if (this._ZipCode != value)
					this._ZipCode = value;
			}

		}

		
		[Column(Name="CountryName", Storage="_CountryName", DbType="nvarchar(40)")]
		public string CountryName
		{
			get
			{
				return this._CountryName;
			}

			set
			{
				if (this._CountryName != value)
					this._CountryName = value;
			}

		}

		
		[Column(Name="CellPhone", Storage="_CellPhone", DbType="nvarchar(20)")]
		public string CellPhone
		{
			get
			{
				return this._CellPhone;
			}

			set
			{
				if (this._CellPhone != value)
					this._CellPhone = value;
			}

		}

		
		[Column(Name="WorkPhone", Storage="_WorkPhone", DbType="nvarchar(20)")]
		public string WorkPhone
		{
			get
			{
				return this._WorkPhone;
			}

			set
			{
				if (this._WorkPhone != value)
					this._WorkPhone = value;
			}

		}

		
		[Column(Name="HomePhone", Storage="_HomePhone", DbType="nvarchar(20)")]
		public string HomePhone
		{
			get
			{
				return this._HomePhone;
			}

			set
			{
				if (this._HomePhone != value)
					this._HomePhone = value;
			}

		}

		
		[Column(Name="EmailAddress", Storage="_EmailAddress", DbType="nvarchar(150)")]
		public string EmailAddress
		{
			get
			{
				return this._EmailAddress;
			}

			set
			{
				if (this._EmailAddress != value)
					this._EmailAddress = value;
			}

		}

		
		[Column(Name="EmailAddress2", Storage="_EmailAddress2", DbType="nvarchar(60)")]
		public string EmailAddress2
		{
			get
			{
				return this._EmailAddress2;
			}

			set
			{
				if (this._EmailAddress2 != value)
					this._EmailAddress2 = value;
			}

		}

		
		[Column(Name="SendEmailAddress1", Storage="_SendEmailAddress1", DbType="bit")]
		public bool? SendEmailAddress1
		{
			get
			{
				return this._SendEmailAddress1;
			}

			set
			{
				if (this._SendEmailAddress1 != value)
					this._SendEmailAddress1 = value;
			}

		}

		
		[Column(Name="SendEmailAddress2", Storage="_SendEmailAddress2", DbType="bit")]
		public bool? SendEmailAddress2
		{
			get
			{
				return this._SendEmailAddress2;
			}

			set
			{
				if (this._SendEmailAddress2 != value)
					this._SendEmailAddress2 = value;
			}

		}

		
		[Column(Name="BirthMonth", Storage="_BirthMonth", DbType="int")]
		public int? BirthMonth
		{
			get
			{
				return this._BirthMonth;
			}

			set
			{
				if (this._BirthMonth != value)
					this._BirthMonth = value;
			}

		}

		
		[Column(Name="BirthDay", Storage="_BirthDay", DbType="int")]
		public int? BirthDay
		{
			get
			{
				return this._BirthDay;
			}

			set
			{
				if (this._BirthDay != value)
					this._BirthDay = value;
			}

		}

		
		[Column(Name="BirthYear", Storage="_BirthYear", DbType="int")]
		public int? BirthYear
		{
			get
			{
				return this._BirthYear;
			}

			set
			{
				if (this._BirthYear != value)
					this._BirthYear = value;
			}

		}

		
		[Column(Name="Gender", Storage="_Gender", DbType="nvarchar(100)")]
		public string Gender
		{
			get
			{
				return this._Gender;
			}

			set
			{
				if (this._Gender != value)
					this._Gender = value;
			}

		}

		
		[Column(Name="PositionInFamily", Storage="_PositionInFamily", DbType="nvarchar(100)")]
		public string PositionInFamily
		{
			get
			{
				return this._PositionInFamily;
			}

			set
			{
				if (this._PositionInFamily != value)
					this._PositionInFamily = value;
			}

		}

		
		[Column(Name="DoNotMailFlag", Storage="_DoNotMailFlag", DbType="bit NOT NULL")]
		public bool DoNotMailFlag
		{
			get
			{
				return this._DoNotMailFlag;
			}

			set
			{
				if (this._DoNotMailFlag != value)
					this._DoNotMailFlag = value;
			}

		}

		
		[Column(Name="DoNotCallFlag", Storage="_DoNotCallFlag", DbType="bit NOT NULL")]
		public bool DoNotCallFlag
		{
			get
			{
				return this._DoNotCallFlag;
			}

			set
			{
				if (this._DoNotCallFlag != value)
					this._DoNotCallFlag = value;
			}

		}

		
		[Column(Name="DoNotVisitFlag", Storage="_DoNotVisitFlag", DbType="bit NOT NULL")]
		public bool DoNotVisitFlag
		{
			get
			{
				return this._DoNotVisitFlag;
			}

			set
			{
				if (this._DoNotVisitFlag != value)
					this._DoNotVisitFlag = value;
			}

		}

		
		[Column(Name="AddressType", Storage="_AddressType", DbType="nvarchar(100)")]
		public string AddressType
		{
			get
			{
				return this._AddressType;
			}

			set
			{
				if (this._AddressType != value)
					this._AddressType = value;
			}

		}

		
		[Column(Name="MaritalStatus", Storage="_MaritalStatus", DbType="nvarchar(100)")]
		public string MaritalStatus
		{
			get
			{
				return this._MaritalStatus;
			}

			set
			{
				if (this._MaritalStatus != value)
					this._MaritalStatus = value;
			}

		}

		
		[Column(Name="MemberStatus", Storage="_MemberStatus", DbType="nvarchar(50)")]
		public string MemberStatus
		{
			get
			{
				return this._MemberStatus;
			}

			set
			{
				if (this._MemberStatus != value)
					this._MemberStatus = value;
			}

		}

		
		[Column(Name="DropType", Storage="_DropType", DbType="nvarchar(100)")]
		public string DropType
		{
			get
			{
				return this._DropType;
			}

			set
			{
				if (this._DropType != value)
					this._DropType = value;
			}

		}

		
		[Column(Name="Origin", Storage="_Origin", DbType="nvarchar(100)")]
		public string Origin
		{
			get
			{
				return this._Origin;
			}

			set
			{
				if (this._Origin != value)
					this._Origin = value;
			}

		}

		
		[Column(Name="EntryPoint", Storage="_EntryPoint", DbType="nvarchar(100)")]
		public string EntryPoint
		{
			get
			{
				return this._EntryPoint;
			}

			set
			{
				if (this._EntryPoint != value)
					this._EntryPoint = value;
			}

		}

		
		[Column(Name="InterestPoint", Storage="_InterestPoint", DbType="nvarchar(100)")]
		public string InterestPoint
		{
			get
			{
				return this._InterestPoint;
			}

			set
			{
				if (this._InterestPoint != value)
					this._InterestPoint = value;
			}

		}

		
		[Column(Name="BaptismType", Storage="_BaptismType", DbType="nvarchar(100)")]
		public string BaptismType
		{
			get
			{
				return this._BaptismType;
			}

			set
			{
				if (this._BaptismType != value)
					this._BaptismType = value;
			}

		}

		
		[Column(Name="BaptismStatus", Storage="_BaptismStatus", DbType="nvarchar(100)")]
		public string BaptismStatus
		{
			get
			{
				return this._BaptismStatus;
			}

			set
			{
				if (this._BaptismStatus != value)
					this._BaptismStatus = value;
			}

		}

		
		[Column(Name="DecisionTypeId", Storage="_DecisionTypeId", DbType="nvarchar(100)")]
		public string DecisionTypeId
		{
			get
			{
				return this._DecisionTypeId;
			}

			set
			{
				if (this._DecisionTypeId != value)
					this._DecisionTypeId = value;
			}

		}

		
		[Column(Name="NewMemberClassStatus", Storage="_NewMemberClassStatus", DbType="nvarchar(100)")]
		public string NewMemberClassStatus
		{
			get
			{
				return this._NewMemberClassStatus;
			}

			set
			{
				if (this._NewMemberClassStatus != value)
					this._NewMemberClassStatus = value;
			}

		}

		
		[Column(Name="LetterStatus", Storage="_LetterStatus", DbType="nvarchar(100)")]
		public string LetterStatus
		{
			get
			{
				return this._LetterStatus;
			}

			set
			{
				if (this._LetterStatus != value)
					this._LetterStatus = value;
			}

		}

		
		[Column(Name="JoinCode", Storage="_JoinCode", DbType="nvarchar(100)")]
		public string JoinCode
		{
			get
			{
				return this._JoinCode;
			}

			set
			{
				if (this._JoinCode != value)
					this._JoinCode = value;
			}

		}

		
		[Column(Name="EnvelopeOption", Storage="_EnvelopeOption", DbType="nvarchar(100)")]
		public string EnvelopeOption
		{
			get
			{
				return this._EnvelopeOption;
			}

			set
			{
				if (this._EnvelopeOption != value)
					this._EnvelopeOption = value;
			}

		}

		
		[Column(Name="ResCode", Storage="_ResCode", DbType="nvarchar(100)")]
		public string ResCode
		{
			get
			{
				return this._ResCode;
			}

			set
			{
				if (this._ResCode != value)
					this._ResCode = value;
			}

		}

		
		[Column(Name="WeddingDate", Storage="_WeddingDate", DbType="datetime")]
		public DateTime? WeddingDate
		{
			get
			{
				return this._WeddingDate;
			}

			set
			{
				if (this._WeddingDate != value)
					this._WeddingDate = value;
			}

		}

		
		[Column(Name="OriginDate", Storage="_OriginDate", DbType="datetime")]
		public DateTime? OriginDate
		{
			get
			{
				return this._OriginDate;
			}

			set
			{
				if (this._OriginDate != value)
					this._OriginDate = value;
			}

		}

		
		[Column(Name="BaptismSchedDate", Storage="_BaptismSchedDate", DbType="datetime")]
		public DateTime? BaptismSchedDate
		{
			get
			{
				return this._BaptismSchedDate;
			}

			set
			{
				if (this._BaptismSchedDate != value)
					this._BaptismSchedDate = value;
			}

		}

		
		[Column(Name="BaptismDate", Storage="_BaptismDate", DbType="datetime")]
		public DateTime? BaptismDate
		{
			get
			{
				return this._BaptismDate;
			}

			set
			{
				if (this._BaptismDate != value)
					this._BaptismDate = value;
			}

		}

		
		[Column(Name="DecisionDate", Storage="_DecisionDate", DbType="datetime")]
		public DateTime? DecisionDate
		{
			get
			{
				return this._DecisionDate;
			}

			set
			{
				if (this._DecisionDate != value)
					this._DecisionDate = value;
			}

		}

		
		[Column(Name="LetterDateRequested", Storage="_LetterDateRequested", DbType="datetime")]
		public DateTime? LetterDateRequested
		{
			get
			{
				return this._LetterDateRequested;
			}

			set
			{
				if (this._LetterDateRequested != value)
					this._LetterDateRequested = value;
			}

		}

		
		[Column(Name="LetterDateReceived", Storage="_LetterDateReceived", DbType="datetime")]
		public DateTime? LetterDateReceived
		{
			get
			{
				return this._LetterDateReceived;
			}

			set
			{
				if (this._LetterDateReceived != value)
					this._LetterDateReceived = value;
			}

		}

		
		[Column(Name="JoinDate", Storage="_JoinDate", DbType="datetime")]
		public DateTime? JoinDate
		{
			get
			{
				return this._JoinDate;
			}

			set
			{
				if (this._JoinDate != value)
					this._JoinDate = value;
			}

		}

		
		[Column(Name="DropDate", Storage="_DropDate", DbType="datetime")]
		public DateTime? DropDate
		{
			get
			{
				return this._DropDate;
			}

			set
			{
				if (this._DropDate != value)
					this._DropDate = value;
			}

		}

		
		[Column(Name="DeceasedDate", Storage="_DeceasedDate", DbType="datetime")]
		public DateTime? DeceasedDate
		{
			get
			{
				return this._DeceasedDate;
			}

			set
			{
				if (this._DeceasedDate != value)
					this._DeceasedDate = value;
			}

		}

		
		[Column(Name="OtherPreviousChurch", Storage="_OtherPreviousChurch", DbType="nvarchar(120)")]
		public string OtherPreviousChurch
		{
			get
			{
				return this._OtherPreviousChurch;
			}

			set
			{
				if (this._OtherPreviousChurch != value)
					this._OtherPreviousChurch = value;
			}

		}

		
		[Column(Name="OtherNewChurch", Storage="_OtherNewChurch", DbType="nvarchar(60)")]
		public string OtherNewChurch
		{
			get
			{
				return this._OtherNewChurch;
			}

			set
			{
				if (this._OtherNewChurch != value)
					this._OtherNewChurch = value;
			}

		}

		
		[Column(Name="SchoolOther", Storage="_SchoolOther", DbType="nvarchar(100)")]
		public string SchoolOther
		{
			get
			{
				return this._SchoolOther;
			}

			set
			{
				if (this._SchoolOther != value)
					this._SchoolOther = value;
			}

		}

		
		[Column(Name="EmployerOther", Storage="_EmployerOther", DbType="nvarchar(120)")]
		public string EmployerOther
		{
			get
			{
				return this._EmployerOther;
			}

			set
			{
				if (this._EmployerOther != value)
					this._EmployerOther = value;
			}

		}

		
		[Column(Name="OccupationOther", Storage="_OccupationOther", DbType="nvarchar(120)")]
		public string OccupationOther
		{
			get
			{
				return this._OccupationOther;
			}

			set
			{
				if (this._OccupationOther != value)
					this._OccupationOther = value;
			}

		}

		
		[Column(Name="HobbyOther", Storage="_HobbyOther", DbType="nvarchar(40)")]
		public string HobbyOther
		{
			get
			{
				return this._HobbyOther;
			}

			set
			{
				if (this._HobbyOther != value)
					this._HobbyOther = value;
			}

		}

		
		[Column(Name="SkillOther", Storage="_SkillOther", DbType="nvarchar(40)")]
		public string SkillOther
		{
			get
			{
				return this._SkillOther;
			}

			set
			{
				if (this._SkillOther != value)
					this._SkillOther = value;
			}

		}

		
		[Column(Name="InterestOther", Storage="_InterestOther", DbType="nvarchar(40)")]
		public string InterestOther
		{
			get
			{
				return this._InterestOther;
			}

			set
			{
				if (this._InterestOther != value)
					this._InterestOther = value;
			}

		}

		
		[Column(Name="LetterStatusNotes", Storage="_LetterStatusNotes", DbType="nvarchar(3000)")]
		public string LetterStatusNotes
		{
			get
			{
				return this._LetterStatusNotes;
			}

			set
			{
				if (this._LetterStatusNotes != value)
					this._LetterStatusNotes = value;
			}

		}

		
		[Column(Name="Comments", Storage="_Comments", DbType="nvarchar(3000)")]
		public string Comments
		{
			get
			{
				return this._Comments;
			}

			set
			{
				if (this._Comments != value)
					this._Comments = value;
			}

		}

		
		[Column(Name="ContributionsStatement", Storage="_ContributionsStatement", DbType="bit NOT NULL")]
		public bool ContributionsStatement
		{
			get
			{
				return this._ContributionsStatement;
			}

			set
			{
				if (this._ContributionsStatement != value)
					this._ContributionsStatement = value;
			}

		}

		
		[Column(Name="StatementOption", Storage="_StatementOption", DbType="nvarchar(100)")]
		public string StatementOption
		{
			get
			{
				return this._StatementOption;
			}

			set
			{
				if (this._StatementOption != value)
					this._StatementOption = value;
			}

		}

		
		[Column(Name="SpouseId", Storage="_SpouseId", DbType="int")]
		public int? SpouseId
		{
			get
			{
				return this._SpouseId;
			}

			set
			{
				if (this._SpouseId != value)
					this._SpouseId = value;
			}

		}

		
		[Column(Name="Grade", Storage="_Grade", DbType="int")]
		public int? Grade
		{
			get
			{
				return this._Grade;
			}

			set
			{
				if (this._Grade != value)
					this._Grade = value;
			}

		}

		
		[Column(Name="BibleFellowshipClassId", Storage="_BibleFellowshipClassId", DbType="int")]
		public int? BibleFellowshipClassId
		{
			get
			{
				return this._BibleFellowshipClassId;
			}

			set
			{
				if (this._BibleFellowshipClassId != value)
					this._BibleFellowshipClassId = value;
			}

		}

		
		[Column(Name="CampusId", Storage="_CampusId", DbType="int")]
		public int? CampusId
		{
			get
			{
				return this._CampusId;
			}

			set
			{
				if (this._CampusId != value)
					this._CampusId = value;
			}

		}

		
		[Column(Name="AltName", Storage="_AltName", DbType="nvarchar(100)")]
		public string AltName
		{
			get
			{
				return this._AltName;
			}

			set
			{
				if (this._AltName != value)
					this._AltName = value;
			}

		}

		
		[Column(Name="CustodyIssue", Storage="_CustodyIssue", DbType="bit")]
		public bool? CustodyIssue
		{
			get
			{
				return this._CustodyIssue;
			}

			set
			{
				if (this._CustodyIssue != value)
					this._CustodyIssue = value;
			}

		}

		
		[Column(Name="OkTransport", Storage="_OkTransport", DbType="bit")]
		public bool? OkTransport
		{
			get
			{
				return this._OkTransport;
			}

			set
			{
				if (this._OkTransport != value)
					this._OkTransport = value;
			}

		}

		
		[Column(Name="NewMemberClassDate", Storage="_NewMemberClassDate", DbType="datetime")]
		public DateTime? NewMemberClassDate
		{
			get
			{
				return this._NewMemberClassDate;
			}

			set
			{
				if (this._NewMemberClassDate != value)
					this._NewMemberClassDate = value;
			}

		}

		
		[Column(Name="ReceiveSMS", Storage="_ReceiveSMS", DbType="bit NOT NULL")]
		public bool ReceiveSMS
		{
			get
			{
				return this._ReceiveSMS;
			}

			set
			{
				if (this._ReceiveSMS != value)
					this._ReceiveSMS = value;
			}

		}

		
		[Column(Name="DoNotPublishPhones", Storage="_DoNotPublishPhones", DbType="bit")]
		public bool? DoNotPublishPhones
		{
			get
			{
				return this._DoNotPublishPhones;
			}

			set
			{
				if (this._DoNotPublishPhones != value)
					this._DoNotPublishPhones = value;
			}

		}

		
		[Column(Name="ElectronicStatement", Storage="_ElectronicStatement", DbType="bit")]
		public bool? ElectronicStatement
		{
			get
			{
				return this._ElectronicStatement;
			}

			set
			{
				if (this._ElectronicStatement != value)
					this._ElectronicStatement = value;
			}

		}

		
    }

}
