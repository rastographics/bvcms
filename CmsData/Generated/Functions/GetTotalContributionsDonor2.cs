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
	[Table(Name="GetTotalContributionsDonor2")]
	public partial class GetTotalContributionsDonor2
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _CreditGiverId;
		
		private string _HeadName;
		
		private string _SpouseName;
		
		private int? _Count;
		
		private decimal? _Amount;
		
		private decimal? _PledgeAmount;
		
		private string _MainFellowship;
		
		private string _MemberStatus;
		
		
		public GetTotalContributionsDonor2()
		{
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

		
		[Column(Name="HeadName", Storage="_HeadName", DbType="varchar(139)")]
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

		
		[Column(Name="SpouseName", Storage="_SpouseName", DbType="varchar(139)")]
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

		
		[Column(Name="Count", Storage="_Count", DbType="int")]
		public int? Count
		{
			get
			{
				return this._Count;
			}

			set
			{
				if (this._Count != value)
					this._Count = value;
			}

		}

		
		[Column(Name="Amount", Storage="_Amount", DbType="Decimal(38,2)")]
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

		
		[Column(Name="PledgeAmount", Storage="_PledgeAmount", DbType="Decimal(38,2)")]
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

		
		[Column(Name="MainFellowship", Storage="_MainFellowship", DbType="varchar(100) NOT NULL")]
		public string MainFellowship
		{
			get
			{
				return this._MainFellowship;
			}

			set
			{
				if (this._MainFellowship != value)
					this._MainFellowship = value;
			}

		}

		
		[Column(Name="MemberStatus", Storage="_MemberStatus", DbType="varchar(50)")]
		public string MemberStatus
		{
			get
			{
				return this._MemberStatus;
			}

			set
			{
				if (this._MemberStatus != value)
					this._MemberStatus = value;
			}

		}

		
    }

}
