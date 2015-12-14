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
	[Table(Name="ContributionsBasic")]
	public partial class ContributionsBasic
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _ContributionId;
		
		private int? _PeopleId;
		
		private int _FamilyId;
		
		private decimal? _ContributionAmount;
		
		private DateTime? _ContributionDate;
		
		private string _CheckNo;
		
		private int _ContributionTypeId;
		
		private int _FundId;
		
		private int _BundleHeaderTypeId;
		
		
		public ContributionsBasic()
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

		
		[Column(Name="ContributionTypeId", Storage="_ContributionTypeId", DbType="int NOT NULL")]
		public int ContributionTypeId
		{
			get
			{
				return this._ContributionTypeId;
			}

			set
			{
				if (this._ContributionTypeId != value)
					this._ContributionTypeId = value;
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

		
		[Column(Name="BundleHeaderTypeId", Storage="_BundleHeaderTypeId", DbType="int NOT NULL")]
		public int BundleHeaderTypeId
		{
			get
			{
				return this._BundleHeaderTypeId;
			}

			set
			{
				if (this._BundleHeaderTypeId != value)
					this._BundleHeaderTypeId = value;
			}

		}

		
    }

}
