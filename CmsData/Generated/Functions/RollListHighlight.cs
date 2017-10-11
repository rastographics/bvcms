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
	[Table(Name="RollListHighlight")]
	public partial class RollListHighlight
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Section;
		
		private int? _PeopleId;
		
		private string _Name;
		
		private string _Last;
		
		private int? _FamilyId;
		
		private string _First;
		
		private string _Email;
		
		private bool? _Attended;
		
		private int? _CommitmentId;
		
		private string _CurrMemberType;
		
		private string _MemberType;
		
		private string _AttendType;
		
		private int? _OtherAttends;
		
		private bool? _CurrMember;
		
		private bool? _Highlight;
		
		private string _ChurchMemberStatus;
		
		private string _IPadAttendanceExtra;
		
		
		public RollListHighlight()
		{
		}

		
		
		[Column(Name="Section", Storage="_Section", DbType="int")]
		public int? Section
		{
			get
			{
				return this._Section;
			}

			set
			{
				if (this._Section != value)
					this._Section = value;
			}

		}

		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int")]
		public int? PeopleId
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

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(100)")]
		public string Name
		{
			get
			{
				return this._Name;
			}

			set
			{
				if (this._Name != value)
					this._Name = value;
			}

		}

		
		[Column(Name="Last", Storage="_Last", DbType="nvarchar(100)")]
		public string Last
		{
			get
			{
				return this._Last;
			}

			set
			{
				if (this._Last != value)
					this._Last = value;
			}

		}

		
		[Column(Name="FamilyId", Storage="_FamilyId", DbType="int")]
		public int? FamilyId
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

		
		[Column(Name="First", Storage="_First", DbType="nvarchar(50)")]
		public string First
		{
			get
			{
				return this._First;
			}

			set
			{
				if (this._First != value)
					this._First = value;
			}

		}

		
		[Column(Name="Email", Storage="_Email", DbType="nvarchar(100)")]
		public string Email
		{
			get
			{
				return this._Email;
			}

			set
			{
				if (this._Email != value)
					this._Email = value;
			}

		}

		
		[Column(Name="Attended", Storage="_Attended", DbType="bit")]
		public bool? Attended
		{
			get
			{
				return this._Attended;
			}

			set
			{
				if (this._Attended != value)
					this._Attended = value;
			}

		}

		
		[Column(Name="CommitmentId", Storage="_CommitmentId", DbType="int")]
		public int? CommitmentId
		{
			get
			{
				return this._CommitmentId;
			}

			set
			{
				if (this._CommitmentId != value)
					this._CommitmentId = value;
			}

		}

		
		[Column(Name="CurrMemberType", Storage="_CurrMemberType", DbType="nvarchar(100)")]
		public string CurrMemberType
		{
			get
			{
				return this._CurrMemberType;
			}

			set
			{
				if (this._CurrMemberType != value)
					this._CurrMemberType = value;
			}

		}

		
		[Column(Name="MemberType", Storage="_MemberType", DbType="nvarchar(100)")]
		public string MemberType
		{
			get
			{
				return this._MemberType;
			}

			set
			{
				if (this._MemberType != value)
					this._MemberType = value;
			}

		}

		
		[Column(Name="AttendType", Storage="_AttendType", DbType="nvarchar(100)")]
		public string AttendType
		{
			get
			{
				return this._AttendType;
			}

			set
			{
				if (this._AttendType != value)
					this._AttendType = value;
			}

		}

		
		[Column(Name="OtherAttends", Storage="_OtherAttends", DbType="int")]
		public int? OtherAttends
		{
			get
			{
				return this._OtherAttends;
			}

			set
			{
				if (this._OtherAttends != value)
					this._OtherAttends = value;
			}

		}

		
		[Column(Name="CurrMember", Storage="_CurrMember", DbType="bit")]
		public bool? CurrMember
		{
			get
			{
				return this._CurrMember;
			}

			set
			{
				if (this._CurrMember != value)
					this._CurrMember = value;
			}

		}

		
		[Column(Name="Highlight", Storage="_Highlight", DbType="bit")]
		public bool? Highlight
		{
			get
			{
				return this._Highlight;
			}

			set
			{
				if (this._Highlight != value)
					this._Highlight = value;
			}

		}

		
		[Column(Name="ChurchMemberStatus", Storage="_ChurchMemberStatus", DbType="nvarchar(100)")]
		public string ChurchMemberStatus
		{
			get
			{
				return this._ChurchMemberStatus;
			}

			set
			{
				if (this._ChurchMemberStatus != value)
					this._ChurchMemberStatus = value;
			}

		}

		
		[Column(Name="iPadAttendanceExtra", Storage="_IPadAttendanceExtra", DbType="nvarchar(500)")]
		public string IPadAttendanceExtra
		{
			get
			{
				return this._IPadAttendanceExtra;
			}

			set
			{
				if (this._IPadAttendanceExtra != value)
					this._IPadAttendanceExtra = value;
			}

		}

		
    }

}
