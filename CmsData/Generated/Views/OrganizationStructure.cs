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
	[Table(Name="OrganizationStructure")]
	public partial class OrganizationStructure
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _Program;
		
		private string _Division;
		
		private string _OrgStatus;
		
		private string _Organization;
		
		private int? _Members;
		
		private int? _Previous;
		
		private int? _Vistors;
		
		private int? _Meetings;
		
		private int _ProgId;
		
		private int _DivId;
		
		private int _OrgId;
		
		
		public OrganizationStructure()
		{
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

		
		[Column(Name="OrgStatus", Storage="_OrgStatus", DbType="nvarchar(50)")]
		public string OrgStatus
		{
			get
			{
				return this._OrgStatus;
			}

			set
			{
				if (this._OrgStatus != value)
					this._OrgStatus = value;
			}

		}

		
		[Column(Name="Organization", Storage="_Organization", DbType="nvarchar(100) NOT NULL")]
		public string Organization
		{
			get
			{
				return this._Organization;
			}

			set
			{
				if (this._Organization != value)
					this._Organization = value;
			}

		}

		
		[Column(Name="Members", Storage="_Members", DbType="int")]
		public int? Members
		{
			get
			{
				return this._Members;
			}

			set
			{
				if (this._Members != value)
					this._Members = value;
			}

		}

		
		[Column(Name="Previous", Storage="_Previous", DbType="int")]
		public int? Previous
		{
			get
			{
				return this._Previous;
			}

			set
			{
				if (this._Previous != value)
					this._Previous = value;
			}

		}

		
		[Column(Name="Vistors", Storage="_Vistors", DbType="int")]
		public int? Vistors
		{
			get
			{
				return this._Vistors;
			}

			set
			{
				if (this._Vistors != value)
					this._Vistors = value;
			}

		}

		
		[Column(Name="Meetings", Storage="_Meetings", DbType="int")]
		public int? Meetings
		{
			get
			{
				return this._Meetings;
			}

			set
			{
				if (this._Meetings != value)
					this._Meetings = value;
			}

		}

		
		[Column(Name="ProgId", Storage="_ProgId", DbType="int NOT NULL")]
		public int ProgId
		{
			get
			{
				return this._ProgId;
			}

			set
			{
				if (this._ProgId != value)
					this._ProgId = value;
			}

		}

		
		[Column(Name="DivId", Storage="_DivId", DbType="int NOT NULL")]
		public int DivId
		{
			get
			{
				return this._DivId;
			}

			set
			{
				if (this._DivId != value)
					this._DivId = value;
			}

		}

		
		[Column(Name="OrgId", Storage="_OrgId", DbType="int NOT NULL")]
		public int OrgId
		{
			get
			{
				return this._OrgId;
			}

			set
			{
				if (this._OrgId != value)
					this._OrgId = value;
			}

		}

		
    }

}
