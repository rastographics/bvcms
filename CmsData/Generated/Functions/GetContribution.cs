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
	[Table(Name="GetContributions")]
	public partial class GetContribution
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _First;
		
		private string _Spouse;
		
		private string _Last;
		
		private string _Addr;
		
		private string _City;
		
		private string _St;
		
		private string _Zip;
		
		private DateTime? _ContributionDate;
		
		private decimal? _Amt;
		
		
		public GetContribution()
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

		
		[Column(Name="First", Storage="_First", DbType="varchar(25)")]
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

		
		[Column(Name="Spouse", Storage="_Spouse", DbType="varchar(25)")]
		public string Spouse
		{
			get
			{
				return this._Spouse;
			}

			set
			{
				if (this._Spouse != value)
					this._Spouse = value;
			}

		}

		
		[Column(Name="LAST", Storage="_Last", DbType="varchar(100) NOT NULL")]
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

		
		[Column(Name="Addr", Storage="_Addr", DbType="varchar(100)")]
		public string Addr
		{
			get
			{
				return this._Addr;
			}

			set
			{
				if (this._Addr != value)
					this._Addr = value;
			}

		}

		
		[Column(Name="City", Storage="_City", DbType="varchar(30)")]
		public string City
		{
			get
			{
				return this._City;
			}

			set
			{
				if (this._City != value)
					this._City = value;
			}

		}

		
		[Column(Name="ST", Storage="_St", DbType="varchar(20)")]
		public string St
		{
			get
			{
				return this._St;
			}

			set
			{
				if (this._St != value)
					this._St = value;
			}

		}

		
		[Column(Name="Zip", Storage="_Zip", DbType="varchar(15)")]
		public string Zip
		{
			get
			{
				return this._Zip;
			}

			set
			{
				if (this._Zip != value)
					this._Zip = value;
			}

		}

		
		[Column(Name="ContributionDate", Storage="_ContributionDate", DbType="datetime")]
		public DateTime? ContributionDate
		{
			get
			{
				return this._ContributionDate;
			}

			set
			{
				if (this._ContributionDate != value)
					this._ContributionDate = value;
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

		
    }

}
