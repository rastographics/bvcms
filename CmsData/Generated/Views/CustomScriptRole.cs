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
	[Table(Name="CustomScriptRoles")]
	public partial class CustomScriptRole
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _Name;
		
		private string _Type;
		
		private string _Role;
		
		private int? _ShowOnOrgId;
		
		private string _ClassX;
		
		private string _Url;
		
		
		public CustomScriptRole()
		{
		}

		
		
		[Column(Name="Name", Storage="_Name", DbType="varchar(100)")]
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

		
		[Column(Name="Type", Storage="_Type", DbType="varchar(100) NOT NULL")]
		public string Type
		{
			get
			{
				return this._Type;
			}

			set
			{
				if (this._Type != value)
					this._Type = value;
			}

		}

		
		[Column(Name="Role", Storage="_Role", DbType="nvarchar")]
		public string Role
		{
			get
			{
				return this._Role;
			}

			set
			{
				if (this._Role != value)
					this._Role = value;
			}

		}

		
		[Column(Name="ShowOnOrgId", Storage="_ShowOnOrgId", DbType="int")]
		public int? ShowOnOrgId
		{
			get
			{
				return this._ShowOnOrgId;
			}

			set
			{
				if (this._ShowOnOrgId != value)
					this._ShowOnOrgId = value;
			}

		}

		
		[Column(Name="class", Storage="_ClassX", DbType="nvarchar")]
		public string ClassX
		{
			get
			{
				return this._ClassX;
			}

			set
			{
				if (this._ClassX != value)
					this._ClassX = value;
			}

		}

		
		[Column(Name="Url", Storage="_Url", DbType="varchar(200)")]
		public string Url
		{
			get
			{
				return this._Url;
			}

			set
			{
				if (this._Url != value)
					this._Url = value;
			}

		}

		
    }

}
