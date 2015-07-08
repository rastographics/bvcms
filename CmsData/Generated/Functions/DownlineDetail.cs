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
	[Table(Name="DownlineDetails")]
	public partial class DownlineDetail
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Level;
		
		private string _OrganizationName;
		
		private string _Leader;
		
		private string _Student;
		
		private string _Trace;
		
		private int? _OrgId;
		
		private int? _LeaderId;
		
		private int? _DiscipleId;
		
		private int? _MaxRows;
		
		
		public DownlineDetail()
		{
		}

		
		
		[Column(Name="Level", Storage="_Level", DbType="int")]
		public int? Level
		{
			get
			{
				return this._Level;
			}

			set
			{
				if (this._Level != value)
					this._Level = value;
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

		
		[Column(Name="Leader", Storage="_Leader", DbType="nvarchar(139)")]
		public string Leader
		{
			get
			{
				return this._Leader;
			}

			set
			{
				if (this._Leader != value)
					this._Leader = value;
			}

		}

		
		[Column(Name="Student", Storage="_Student", DbType="nvarchar(139)")]
		public string Student
		{
			get
			{
				return this._Student;
			}

			set
			{
				if (this._Student != value)
					this._Student = value;
			}

		}

		
		[Column(Name="Trace", Storage="_Trace", DbType="varchar(400)")]
		public string Trace
		{
			get
			{
				return this._Trace;
			}

			set
			{
				if (this._Trace != value)
					this._Trace = value;
			}

		}

		
		[Column(Name="OrgId", Storage="_OrgId", DbType="int")]
		public int? OrgId
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

		
		[Column(Name="DiscipleId", Storage="_DiscipleId", DbType="int")]
		public int? DiscipleId
		{
			get
			{
				return this._DiscipleId;
			}

			set
			{
				if (this._DiscipleId != value)
					this._DiscipleId = value;
			}

		}

		
		[Column(Name="MaxRows", Storage="_MaxRows", DbType="int")]
		public int? MaxRows
		{
			get
			{
				return this._MaxRows;
			}

			set
			{
				if (this._MaxRows != value)
					this._MaxRows = value;
			}

		}

		
    }

}
