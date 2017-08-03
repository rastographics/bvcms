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
	[Table(Name="ChAiGiftData")]
	public partial class ChAiGiftDatum
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _PeopleId;
		
		private int _FamilyId;
		
		private int _ContributionId;
		
		private DateTime _CreatedDate;
		
		private DateTime? _ContributionDate;
		
		private decimal? _ContributionAmount;
		
		private string _Type;
		
		private string _Status;
		
		private string _BundleType;
		
		private string _FundName;
		
		private string _PaymentType;
		
		private string _CheckNo;
		
		
		public ChAiGiftDatum()
		{
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

		
		[Column(Name="FamilyId", Storage="_FamilyId", DbType="int NOT NULL")]
		public int FamilyId
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

		
		[Column(Name="CreatedDate", Storage="_CreatedDate", DbType="datetime NOT NULL")]
		public DateTime CreatedDate
		{
			get
			{
				return this._CreatedDate;
			}

			set
			{
				if (this._CreatedDate != value)
					this._CreatedDate = value;
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

		
		[Column(Name="ContributionAmount", Storage="_ContributionAmount", DbType="Decimal(11,2)")]
		public decimal? ContributionAmount
		{
			get
			{
				return this._ContributionAmount;
			}

			set
			{
				if (this._ContributionAmount != value)
					this._ContributionAmount = value;
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

		
		[Column(Name="PaymentType", Storage="_PaymentType", DbType="varchar(5) NOT NULL")]
		public string PaymentType
		{
			get
			{
				return this._PaymentType;
			}

			set
			{
				if (this._PaymentType != value)
					this._PaymentType = value;
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

		
    }

}
