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
	[Table(Name="XpContribution")]
	public partial class XpContribution
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _ContributionId;
		
		private int? _PeopleId;
		
		private string _BundleType;
		
		private DateTime? _DepositDate;
		
		private string _Fund;
		
		private string _Type;
		
		private DateTime? _DateX;
		
		private decimal? _Amount;
		
		private string _Description;
		
		private string _Status;
		
		private bool? _Pledge;
		
		private string _CheckNo;
		
		private string _Campus;
		
		
		public XpContribution()
		{
		}

		
		
		[Column(Name="ContributionId", Storage="_ContributionId", DbType="int NOT NULL")]
		public int ContributionId
		{
			get
			{
				return this._ContributionId;
			}

			set
			{
				if (this._ContributionId != value)
					this._ContributionId = value;
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

		
		[Column(Name="BundleType", Storage="_BundleType", DbType="nvarchar(50)")]
		public string BundleType
		{
			get
			{
				return this._BundleType;
			}

			set
			{
				if (this._BundleType != value)
					this._BundleType = value;
			}

		}

		
		[Column(Name="DepositDate", Storage="_DepositDate", DbType="datetime")]
		public DateTime? DepositDate
		{
			get
			{
				return this._DepositDate;
			}

			set
			{
				if (this._DepositDate != value)
					this._DepositDate = value;
			}

		}

		
		[Column(Name="Fund", Storage="_Fund", DbType="nvarchar(256)")]
		public string Fund
		{
			get
			{
				return this._Fund;
			}

			set
			{
				if (this._Fund != value)
					this._Fund = value;
			}

		}

		
		[Column(Name="Type", Storage="_Type", DbType="nvarchar(50)")]
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

		
		[Column(Name="Date", Storage="_DateX", DbType="datetime")]
		public DateTime? DateX
		{
			get
			{
				return this._DateX;
			}

			set
			{
				if (this._DateX != value)
					this._DateX = value;
			}

		}

		
		[Column(Name="Amount", Storage="_Amount", DbType="Decimal(11,2)")]
		public decimal? Amount
		{
			get
			{
				return this._Amount;
			}

			set
			{
				if (this._Amount != value)
					this._Amount = value;
			}

		}

		
		[Column(Name="Description", Storage="_Description", DbType="nvarchar(256)")]
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

		
		[Column(Name="Status", Storage="_Status", DbType="nvarchar(50)")]
		public string Status
		{
			get
			{
				return this._Status;
			}

			set
			{
				if (this._Status != value)
					this._Status = value;
			}

		}

		
		[Column(Name="Pledge", Storage="_Pledge", DbType="bit")]
		public bool? Pledge
		{
			get
			{
				return this._Pledge;
			}

			set
			{
				if (this._Pledge != value)
					this._Pledge = value;
			}

		}

		
		[Column(Name="CheckNo", Storage="_CheckNo", DbType="nvarchar(20)")]
		public string CheckNo
		{
			get
			{
				return this._CheckNo;
			}

			set
			{
				if (this._CheckNo != value)
					this._CheckNo = value;
			}

		}

		
		[Column(Name="Campus", Storage="_Campus", DbType="nvarchar(100)")]
		public string Campus
		{
			get
			{
				return this._Campus;
			}

			set
			{
				if (this._Campus != value)
					this._Campus = value;
			}

		}

		
    }

}
