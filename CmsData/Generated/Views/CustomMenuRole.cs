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
	[Table(Name="CustomMenuRoles")]
	public partial class CustomMenuRole
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _Link;
		
		private string _Role;
		
		private string _Name;
		
		private int _Col;
		
		
		public CustomMenuRole()
		{
		}

		
		
		[Column(Name="Link", Storage="_Link", DbType="varchar(100)")]
		public string Link
		{
			get
			{
				return this._Link;
			}

			set
			{
				if (this._Link != value)
					this._Link = value;
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

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar")]
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

		
		[Column(Name="Col", Storage="_Col", DbType="int NOT NULL")]
		public int Col
		{
			get
			{
				return this._Col;
			}

			set
			{
				if (this._Col != value)
					this._Col = value;
			}

		}

		
    }

}
