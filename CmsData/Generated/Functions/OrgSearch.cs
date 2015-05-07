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
	[Table(Name="OrgSearch")]
	public partial class OrgSearch
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
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

		
		
		[Column(Name="OrganizationId", Storage="_OrganizationId", DbType="int NOT NULL")]
		public int OrganizationId
		{
			get
			{
				return this._OrganizationId;
			}

			set
			{
				if (this._OrganizationId != value)
					this._OrganizationId = value;
			}

		}

		
		[Column(Name="OrganizationName", Storage="_OrganizationName", DbType="nvarchar(100) NOT NULL")]
		public string OrganizationName
		{
			get
			{
				return this._OrganizationName;
			}

			set
			{
				if (this._OrganizationName != value)
					this._OrganizationName = value;
			}

		}

		
		[Column(Name="OrganizationStatusId", Storage="_OrganizationStatusId", DbType="int NOT NULL")]
		public int OrganizationStatusId
		{
			get
			{
				return this._OrganizationStatusId;
			}

			set
			{
				if (this._OrganizationStatusId != value)
					this._OrganizationStatusId = value;
			}

		}

		
		[Column(Name="Program", Storage="_Program", DbType="nvarchar(50)")]
		public string Program
		{
			get
			{
				return this._Program;
			}

			set
			{
				if (this._Program != value)
					this._Program = value;
			}

		}

		
		[Column(Name="ProgramId", Storage="_ProgramId", DbType="int")]
		public int? ProgramId
		{
			get
			{
				return this._ProgramId;
			}

			set
			{
				if (this._ProgramId != value)
					this._ProgramId = value;
			}

		}

		
		[Column(Name="Division", Storage="_Division", DbType="nvarchar(50)")]
		public string Division
		{
			get
			{
				return this._Division;
			}

			set
			{
				if (this._Division != value)
					this._Division = value;
			}

		}

		
		[Column(Name="Divisions", Storage="_Divisions", DbType="nvarchar")]
		public string Divisions
		{
			get
			{
				return this._Divisions;
			}

			set
			{
				if (this._Divisions != value)
					this._Divisions = value;
			}

		}

		
		[Column(Name="ScheduleDescription", Storage="_ScheduleDescription", DbType="nvarchar(20)")]
		public string ScheduleDescription
		{
			get
			{
				return this._ScheduleDescription;
			}

			set
			{
				if (this._ScheduleDescription != value)
					this._ScheduleDescription = value;
			}

		}

		
		[Column(Name="ScheduleId", Storage="_ScheduleId", DbType="int")]
		public int? ScheduleId
		{
			get
			{
				return this._ScheduleId;
			}

			set
			{
				if (this._ScheduleId != value)
					this._ScheduleId = value;
			}

		}

		
		[Column(Name="SchedTime", Storage="_SchedTime", DbType="datetime")]
		public DateTime? SchedTime
		{
			get
			{
				return this._SchedTime;
			}

			set
			{
				if (this._SchedTime != value)
					this._SchedTime = value;
			}

		}

		
		[Column(Name="Campus", Storage="_Campus", DbType="nvarchar(100)")]
		public string Campus
		{
			get
			{
				return this._Campus;
			}

			set
			{
				if (this._Campus != value)
					this._Campus = value;
			}

		}

		
		[Column(Name="LeaderType", Storage="_LeaderType", DbType="nvarchar(100)")]
		public string LeaderType
		{
			get
			{
				return this._LeaderType;
			}

			set
			{
				if (this._LeaderType != value)
					this._LeaderType = value;
			}

		}

		
		[Column(Name="LeaderName", Storage="_LeaderName", DbType="nvarchar(50)")]
		public string LeaderName
		{
			get
			{
				return this._LeaderName;
			}

			set
			{
				if (this._LeaderName != value)
					this._LeaderName = value;
			}

		}

		
		[Column(Name="Location", Storage="_Location", DbType="nvarchar(200)")]
		public string Location
		{
			get
			{
				return this._Location;
			}

			set
			{
				if (this._Location != value)
					this._Location = value;
			}

		}

		
		[Column(Name="ClassFilled", Storage="_ClassFilled", DbType="bit")]
		public bool? ClassFilled
		{
			get
			{
				return this._ClassFilled;
			}

			set
			{
				if (this._ClassFilled != value)
					this._ClassFilled = value;
			}

		}

		
		[Column(Name="RegistrationClosed", Storage="_RegistrationClosed", DbType="bit")]
		public bool? RegistrationClosed
		{
			get
			{
				return this._RegistrationClosed;
			}

			set
			{
				if (this._RegistrationClosed != value)
					this._RegistrationClosed = value;
			}

		}

		
		[Column(Name="AppCategory", Storage="_AppCategory", DbType="varchar(15)")]
		public string AppCategory
		{
			get
			{
				return this._AppCategory;
			}

			set
			{
				if (this._AppCategory != value)
					this._AppCategory = value;
			}

		}

		
		[Column(Name="RegStart", Storage="_RegStart", DbType="datetime")]
		public DateTime? RegStart
		{
			get
			{
				return this._RegStart;
			}

			set
			{
				if (this._RegStart != value)
					this._RegStart = value;
			}

		}

		
		[Column(Name="RegEnd", Storage="_RegEnd", DbType="datetime")]
		public DateTime? RegEnd
		{
			get
			{
				return this._RegEnd;
			}

			set
			{
				if (this._RegEnd != value)
					this._RegEnd = value;
			}

		}

		
		[Column(Name="PublicSortOrder", Storage="_PublicSortOrder", DbType="varchar(15)")]
		public string PublicSortOrder
		{
			get
			{
				return this._PublicSortOrder;
			}

			set
			{
				if (this._PublicSortOrder != value)
					this._PublicSortOrder = value;
			}

		}

		
		[Column(Name="FirstMeetingDate", Storage="_FirstMeetingDate", DbType="datetime")]
		public DateTime? FirstMeetingDate
		{
			get
			{
				return this._FirstMeetingDate;
			}

			set
			{
				if (this._FirstMeetingDate != value)
					this._FirstMeetingDate = value;
			}

		}

		
		[Column(Name="LastMeetingDate", Storage="_LastMeetingDate", DbType="datetime")]
		public DateTime? LastMeetingDate
		{
			get
			{
				return this._LastMeetingDate;
			}

			set
			{
				if (this._LastMeetingDate != value)
					this._LastMeetingDate = value;
			}

		}

		
		[Column(Name="MemberCount", Storage="_MemberCount", DbType="int")]
		public int? MemberCount
		{
			get
			{
				return this._MemberCount;
			}

			set
			{
				if (this._MemberCount != value)
					this._MemberCount = value;
			}

		}

		
		[Column(Name="RegistrationTypeId", Storage="_RegistrationTypeId", DbType="int")]
		public int? RegistrationTypeId
		{
			get
			{
				return this._RegistrationTypeId;
			}

			set
			{
				if (this._RegistrationTypeId != value)
					this._RegistrationTypeId = value;
			}

		}

		
		[Column(Name="CanSelfCheckin", Storage="_CanSelfCheckin", DbType="bit")]
		public bool? CanSelfCheckin
		{
			get
			{
				return this._CanSelfCheckin;
			}

			set
			{
				if (this._CanSelfCheckin != value)
					this._CanSelfCheckin = value;
			}

		}

		
		[Column(Name="LeaderId", Storage="_LeaderId", DbType="int")]
		public int? LeaderId
		{
			get
			{
				return this._LeaderId;
			}

			set
			{
				if (this._LeaderId != value)
					this._LeaderId = value;
			}

		}

		
		[Column(Name="PrevMemberCount", Storage="_PrevMemberCount", DbType="int")]
		public int? PrevMemberCount
		{
			get
			{
				return this._PrevMemberCount;
			}

			set
			{
				if (this._PrevMemberCount != value)
					this._PrevMemberCount = value;
			}

		}

		
		[Column(Name="ProspectCount", Storage="_ProspectCount", DbType="int")]
		public int? ProspectCount
		{
			get
			{
				return this._ProspectCount;
			}

			set
			{
				if (this._ProspectCount != value)
					this._ProspectCount = value;
			}

		}

		
		[Column(Name="Description", Storage="_Description", DbType="nvarchar")]
		public string Description
		{
			get
			{
				return this._Description;
			}

			set
			{
				if (this._Description != value)
					this._Description = value;
			}

		}

		
		[Column(Name="UseRegisterLink2", Storage="_UseRegisterLink2", DbType="bit")]
		public bool? UseRegisterLink2
		{
			get
			{
				return this._UseRegisterLink2;
			}

			set
			{
				if (this._UseRegisterLink2 != value)
					this._UseRegisterLink2 = value;
			}

		}

		
		[Column(Name="DivisionId", Storage="_DivisionId", DbType="int")]
		public int? DivisionId
		{
			get
			{
				return this._DivisionId;
			}

			set
			{
				if (this._DivisionId != value)
					this._DivisionId = value;
			}

		}

		
		[Column(Name="BirthDayStart", Storage="_BirthDayStart", DbType="datetime")]
		public DateTime? BirthDayStart
		{
			get
			{
				return this._BirthDayStart;
			}

			set
			{
				if (this._BirthDayStart != value)
					this._BirthDayStart = value;
			}

		}

		
		[Column(Name="BirthDayEnd", Storage="_BirthDayEnd", DbType="datetime")]
		public DateTime? BirthDayEnd
		{
			get
			{
				return this._BirthDayEnd;
			}

			set
			{
				if (this._BirthDayEnd != value)
					this._BirthDayEnd = value;
			}

		}

		
		[Column(Name="Tag", Storage="_Tag", DbType="varchar(6) NOT NULL")]
		public string Tag
		{
			get
			{
				return this._Tag;
			}

			set
			{
				if (this._Tag != value)
					this._Tag = value;
			}

		}

		
		[Column(Name="ChangeMain", Storage="_ChangeMain", DbType="int NOT NULL")]
		public int ChangeMain
		{
			get
			{
				return this._ChangeMain;
			}

			set
			{
				if (this._ChangeMain != value)
					this._ChangeMain = value;
			}

		}

		
    }

}
