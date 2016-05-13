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
	[Table(Name="RegistrationGradeOptions")]
	public partial class RegistrationGradeOption
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private string _OrganizationName;
		
		private string _GradeOption;
		
		private string _GradeCode;
		
		private int? _Cnt;
		
		
		public RegistrationGradeOption()
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

		
		[Column(Name="GradeOption", Storage="_GradeOption", DbType="varchar(100)")]
		public string GradeOption
		{
			get
			{
				return this._GradeOption;
			}

			set
			{
				if (this._GradeOption != value)
					this._GradeOption = value;
			}

		}

		
		[Column(Name="GradeCode", Storage="_GradeCode", DbType="varchar(100)")]
		public string GradeCode
		{
			get
			{
				return this._GradeCode;
			}

			set
			{
				if (this._GradeCode != value)
					this._GradeCode = value;
			}

		}

		
		[Column(Name="Cnt", Storage="_Cnt", DbType="int")]
		public int? Cnt
		{
			get
			{
				return this._Cnt;
			}

			set
			{
				if (this._Cnt != value)
					this._Cnt = value;
			}

		}

		
    }

}
