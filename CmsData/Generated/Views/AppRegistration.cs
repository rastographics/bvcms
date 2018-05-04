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
	[Table(Name="AppRegistrations")]
	public partial class AppRegistration
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private string _Title;
		
		private string _OrganizationName;
		
		private string _Description;
		
		private string _AppCategory;
		
		private string _PublicSortOrder;
		
		private bool? _UseRegisterLink2;
		
		private DateTime? _RegStart;
		
		private DateTime? _RegEnd;
		
		
		public AppRegistration()
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

		
		[Column(Name="Title", Storage="_Title", DbType="nvarchar(200)")]
		public string Title
		{
			get
			{
				return this._Title;
			}

			set
			{
				if (this._Title != value)
					this._Title = value;
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

		
    }

}
