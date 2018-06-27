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
	[Table(Name="GetContributionsDetails")]
	public partial class GetContributionsDetail
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _FamilyId;
		
		private int? _PeopleId;
		
		private DateTime? _DateX;
		
		private int? _CreditGiverId;
		
		private int? _CreditGiverId2;
		
		private int? _SpouseId;
		
		private string _HeadName;
		
		private string _SpouseName;
		
		private decimal? _Amount;
		
		private decimal? _PledgeAmount;
		
		private int? _BundleHeaderId;
		
		private string _ContributionDesc;
		
		private string _CheckNo;
		
		private int _FundId;
		
		private string _FundName;
		
		private int _OpenPledgeFund;
		
		private string _BundleType;
		
		private string _BundleStatus;
		
		private int _ContributionId;
		
		
		public GetContributionsDetail()
		{
		}

		
		
		[Column(Name="FamilyId", Storage="_FamilyId", DbType="int")]
		public int? FamilyId
		{
			get
			{
				return this._FamilyId;
			}

			set
			{
				if (this._FamilyId != value)
					this._FamilyId = value;
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

		
		[Column(Name="CreditGiverId", Storage="_CreditGiverId", DbType="int")]
		public int? CreditGiverId
		{
			get
			{
				return this._CreditGiverId;
			}

			set
			{
				if (this._CreditGiverId != value)
					this._CreditGiverId = value;
			}

		}

		
		[Column(Name="CreditGiverId2", Storage="_CreditGiverId2", DbType="int")]
		public int? CreditGiverId2
		{
			get
			{
				return this._CreditGiverId2;
			}

			set
			{
				if (this._CreditGiverId2 != value)
					this._CreditGiverId2 = value;
			}

		}

		
		[Column(Name="SpouseId", Storage="_SpouseId", DbType="int")]
		public int? SpouseId
		{
			get
			{
				return this._SpouseId;
			}

			set
			{
				if (this._SpouseId != value)
					this._SpouseId = value;
			}

		}

		
		[Column(Name="HeadName", Storage="_HeadName", DbType="nvarchar(139)")]
		public string HeadName
		{
			get
			{
				return this._HeadName;
			}

			set
			{
				if (this._HeadName != value)
					this._HeadName = value;
			}

		}

		
		[Column(Name="SpouseName", Storage="_SpouseName", DbType="nvarchar(139)")]
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

		
		[Column(Name="PledgeAmount", Storage="_PledgeAmount", DbType="Decimal(11,2)")]
		public decimal? PledgeAmount
		{
			get
			{
				return this._PledgeAmount;
			}

			set
			{
				if (this._PledgeAmount != value)
					this._PledgeAmount = value;
			}

		}

		
		[Column(Name="BundleHeaderId", Storage="_BundleHeaderId", DbType="int")]
		public int? BundleHeaderId
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

		
		[Column(Name="ContributionDesc", Storage="_ContributionDesc", DbType="nvarchar(256)")]
		public string ContributionDesc
		{
			get
			{
				return this._ContributionDesc;
			}

			set
			{
				if (this._ContributionDesc != value)
					this._ContributionDesc = value;
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

		
		[Column(Name="FundName", Storage="_FundName", DbType="nvarchar(256) NOT NULL")]
		public string FundName
		{
			get
			{
				return this._FundName;
			}

			set
			{
				if (this._FundName != value)
					this._FundName = value;
			}

		}

		
		[Column(Name="OpenPledgeFund", Storage="_OpenPledgeFund", DbType="int NOT NULL")]
		public int OpenPledgeFund
		{
			get
			{
				return this._OpenPledgeFund;
			}

			set
			{
				if (this._OpenPledgeFund != value)
					this._OpenPledgeFund = value;
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

		
		[Column(Name="BundleStatus", Storage="_BundleStatus", DbType="nvarchar(50)")]
		public string BundleStatus
		{
			get
			{
				return this._BundleStatus;
			}

			set
			{
				if (this._BundleStatus != value)
					this._BundleStatus = value;
			}

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

		
    }

}
