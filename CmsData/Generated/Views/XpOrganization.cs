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
	[Table(Name="XpOrganization")]
	public partial class XpOrganization
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
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

		
		[Column(Name="OrganizationStatus", Storage="_OrganizationStatus", DbType="nvarchar(50)")]
		public string OrganizationStatus
		{
			get
			{
				return this._OrganizationStatus;
			}

			set
			{
				if (this._OrganizationStatus != value)
					this._OrganizationStatus = value;
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

		
		[Column(Name="LeaderMemberType", Storage="_LeaderMemberType", DbType="nvarchar(100)")]
		public string LeaderMemberType
		{
			get
			{
				return this._LeaderMemberType;
			}

			set
			{
				if (this._LeaderMemberType != value)
					this._LeaderMemberType = value;
			}

		}

		
		[Column(Name="GradeAgeStart", Storage="_GradeAgeStart", DbType="int")]
		public int? GradeAgeStart
		{
			get
			{
				return this._GradeAgeStart;
			}

			set
			{
				if (this._GradeAgeStart != value)
					this._GradeAgeStart = value;
			}

		}

		
		[Column(Name="GradeAgeEnd", Storage="_GradeAgeEnd", DbType="int")]
		public int? GradeAgeEnd
		{
			get
			{
				return this._GradeAgeEnd;
			}

			set
			{
				if (this._GradeAgeEnd != value)
					this._GradeAgeEnd = value;
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

		
		[Column(Name="ParentOrgId", Storage="_ParentOrgId", DbType="int")]
		public int? ParentOrgId
		{
			get
			{
				return this._ParentOrgId;
			}

			set
			{
				if (this._ParentOrgId != value)
					this._ParentOrgId = value;
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

		
		[Column(Name="PhoneNumber", Storage="_PhoneNumber", DbType="nvarchar(25)")]
		public string PhoneNumber
		{
			get
			{
				return this._PhoneNumber;
			}

			set
			{
				if (this._PhoneNumber != value)
					this._PhoneNumber = value;
			}

		}

		
		[Column(Name="IsBibleFellowshipOrg", Storage="_IsBibleFellowshipOrg", DbType="bit")]
		public bool? IsBibleFellowshipOrg
		{
			get
			{
				return this._IsBibleFellowshipOrg;
			}

			set
			{
				if (this._IsBibleFellowshipOrg != value)
					this._IsBibleFellowshipOrg = value;
			}

		}

		
		[Column(Name="Offsite", Storage="_Offsite", DbType="bit")]
		public bool? Offsite
		{
			get
			{
				return this._Offsite;
			}

			set
			{
				if (this._Offsite != value)
					this._Offsite = value;
			}

		}

		
		[Column(Name="OrganizationType", Storage="_OrganizationType", DbType="nvarchar(50)")]
		public string OrganizationType
		{
			get
			{
				return this._OrganizationType;
			}

			set
			{
				if (this._OrganizationType != value)
					this._OrganizationType = value;
			}

		}

		
		[Column(Name="PublishDirectory", Storage="_PublishDirectory", DbType="int")]
		public int? PublishDirectory
		{
			get
			{
				return this._PublishDirectory;
			}

			set
			{
				if (this._PublishDirectory != value)
					this._PublishDirectory = value;
			}

		}

		
		[Column(Name="IsRecreationTeam", Storage="_IsRecreationTeam", DbType="bit NOT NULL")]
		public bool IsRecreationTeam
		{
			get
			{
				return this._IsRecreationTeam;
			}

			set
			{
				if (this._IsRecreationTeam != value)
					this._IsRecreationTeam = value;
			}

		}

		
		[Column(Name="NotWeekly", Storage="_NotWeekly", DbType="bit")]
		public bool? NotWeekly
		{
			get
			{
				return this._NotWeekly;
			}

			set
			{
				if (this._NotWeekly != value)
					this._NotWeekly = value;
			}

		}

		
		[Column(Name="IsMissionTrip", Storage="_IsMissionTrip", DbType="bit")]
		public bool? IsMissionTrip
		{
			get
			{
				return this._IsMissionTrip;
			}

			set
			{
				if (this._IsMissionTrip != value)
					this._IsMissionTrip = value;
			}

		}

		
    }

}
