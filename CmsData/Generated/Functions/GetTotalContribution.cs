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
	[Table(Name="GetTotalContributions")]
	public partial class GetTotalContribution
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Name;
		
		private string _SpouseName;
		
		private int _FundId;
		
		private string _FundDescription;
		
		private int? _Cnt;
		
		private decimal? _Amt;
		
		private decimal? _Plg;
		
		
		public GetTotalContribution()
		{
		}

		
		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int NOT NULL")]
		public int PeopleId
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

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(126)")]
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

		
		[Column(Name="SpouseName", Storage="_SpouseName", DbType="nvarchar(138)")]
		public string SpouseName
		{
			get
			{
				return this._SpouseName;
			}

			set
			{
				if (this._SpouseName != value)
					this._SpouseName = value;
			}

		}

		
		[Column(Name="FundId", Storage="_FundId", DbType="int NOT NULL")]
		public int FundId
		{
			get
			{
				return this._FundId;
			}

			set
			{
				if (this._FundId != value)
					this._FundId = value;
			}

		}

		
		[Column(Name="FundDescription", Storage="_FundDescription", DbType="nvarchar(256) NOT NULL")]
		public string FundDescription
		{
			get
			{
				return this._FundDescription;
			}

			set
			{
				if (this._FundDescription != value)
					this._FundDescription = value;
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

		
		[Column(Name="Amt", Storage="_Amt", DbType="Decimal(38,2)")]
		public decimal? Amt
		{
			get
			{
				return this._Amt;
			}

			set
			{
				if (this._Amt != value)
					this._Amt = value;
			}

		}

		
		[Column(Name="Plg", Storage="_Plg", DbType="Decimal(38,2)")]
		public decimal? Plg
		{
			get
			{
				return this._Plg;
			}

			set
			{
				if (this._Plg != value)
					this._Plg = value;
			}

		}

		
    }

}
