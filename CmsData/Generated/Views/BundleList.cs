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
	[Table(Name="BundleList")]
	public partial class BundleList
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _BundleHeaderId;
		
		private string _HeaderType;
		
		private DateTime? _DepositDate;
		
		private decimal? _TotalBundle;
		
		private int? _FundId;
		
		private string _Fund;
		
		private string _Status;
		
		private int? _Open;
		
		private DateTime? _PostingDate;
		
		private decimal? _TotalItems;
		
		private int? _ItemCount;
		
		private decimal? _TotalNonTaxDed;
		
		private int _BundleStatusId;
		
		
		public BundleList()
		{
		}

		
		
		[Column(Name="BundleHeaderId", Storage="_BundleHeaderId", DbType="int NOT NULL")]
		public int BundleHeaderId
		{
			get
			{
				return this._BundleHeaderId;
			}

			set
			{
				if (this._BundleHeaderId != value)
					this._BundleHeaderId = value;
			}

		}

		
		[Column(Name="HeaderType", Storage="_HeaderType", DbType="nvarchar(50)")]
		public string HeaderType
		{
			get
			{
				return this._HeaderType;
			}

			set
			{
				if (this._HeaderType != value)
					this._HeaderType = value;
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

		
		[Column(Name="TotalBundle", Storage="_TotalBundle", DbType="Decimal(12,2)")]
		public decimal? TotalBundle
		{
			get
			{
				return this._TotalBundle;
			}

			set
			{
				if (this._TotalBundle != value)
					this._TotalBundle = value;
			}

		}

		
		[Column(Name="FundId", Storage="_FundId", DbType="int")]
		public int? FundId
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

		
		[Column(Name="open", Storage="_Open", DbType="int")]
		public int? Open
		{
			get
			{
				return this._Open;
			}

			set
			{
				if (this._Open != value)
					this._Open = value;
			}

		}

		
		[Column(Name="PostingDate", Storage="_PostingDate", DbType="datetime")]
		public DateTime? PostingDate
		{
			get
			{
				return this._PostingDate;
			}

			set
			{
				if (this._PostingDate != value)
					this._PostingDate = value;
			}

		}

		
		[Column(Name="TotalItems", Storage="_TotalItems", DbType="Decimal(38,2)")]
		public decimal? TotalItems
		{
			get
			{
				return this._TotalItems;
			}

			set
			{
				if (this._TotalItems != value)
					this._TotalItems = value;
			}

		}

		
		[Column(Name="ItemCount", Storage="_ItemCount", DbType="int")]
		public int? ItemCount
		{
			get
			{
				return this._ItemCount;
			}

			set
			{
				if (this._ItemCount != value)
					this._ItemCount = value;
			}

		}

		
		[Column(Name="TotalNonTaxDed", Storage="_TotalNonTaxDed", DbType="Decimal(38,2)")]
		public decimal? TotalNonTaxDed
		{
			get
			{
				return this._TotalNonTaxDed;
			}

			set
			{
				if (this._TotalNonTaxDed != value)
					this._TotalNonTaxDed = value;
			}

		}

		
		[Column(Name="BundleStatusId", Storage="_BundleStatusId", DbType="int NOT NULL")]
		public int BundleStatusId
		{
			get
			{
				return this._BundleStatusId;
			}

			set
			{
				if (this._BundleStatusId != value)
					this._BundleStatusId = value;
			}

		}

		
    }

}
