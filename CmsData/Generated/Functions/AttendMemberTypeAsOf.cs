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
	[Table(Name="AttendMemberTypeAsOf")]
	public partial class AttendMemberTypeAsOf
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _PeopleId;
		
		private int? _MemberTypeId;
		
		private string _MemberType;
		
		
		public AttendMemberTypeAsOf()
		{
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

		
		[Column(Name="MemberTypeId", Storage="_MemberTypeId", DbType="int")]
		public int? MemberTypeId
		{
			get
			{
				return this._MemberTypeId;
			}

			set
			{
				if (this._MemberTypeId != value)
					this._MemberTypeId = value;
			}

		}

		
		[Column(Name="MemberType", Storage="_MemberType", DbType="varchar(100)")]
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

		
    }

}
