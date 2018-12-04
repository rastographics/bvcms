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
	[Table(Name="GetTotalContributionsDonor")]
	public partial class GetTotalContributionsDonor
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _CreditGiverId;
		
		private int? _CreditGiverId2;
		
		private string _HeadName;

        private string _Head_FirstName;

        private string _Head_LastName;

        private string _Email;

        private string _SpouseName;
		
		private int? _Count;
		
		private decimal? _Amount;
		
		private decimal? _PledgeAmount;
		
		private string _MainFellowship;
		
		private string _MemberStatus;
		
		private DateTime? _JoinDate;
		
		private int? _SpouseId;
		
		private string _Option;
		
		private string _Addr;
		
		private string _Addr2;
		
		private string _City;
		
		private string _St;
		
		private string _Zip;
		
		
		public GetTotalContributionsDonor()
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

        [Column(Name = "Head_LastName", Storage = "_Head_LastName", DbType = "nvarchar(139)")]
        public string Head_LastName
        {
            get
            {
                return this._Head_LastName;
            }

            set
            {
                if (this._Head_LastName != value)
                    this._Head_LastName = value;
            }

        }

        [Column(Name = "Head_FirstName", Storage = "_Head_FirstName", DbType = "nvarchar(139)")]
        public string Head_FirstName
        {
            get
            {
                return this._Head_FirstName;
            }

            set
            {
                if (this._Head_FirstName != value)
                    this._Head_FirstName = value;
            }

        }

        [Column(Name = "Email", Storage = "_Email", DbType = "nvarchar(139)")]
        public string Email
        {
            get
            {
                return this._Email;
            }

            set
            {
                if (this._Email != value)
                    this._Email = value;
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

		
		[Column(Name="MainFellowship", Storage="_MainFellowship", DbType="nvarchar(100) NOT NULL")]
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

		
		[Column(Name="MemberStatus", Storage="_MemberStatus", DbType="nvarchar(50)")]
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

		
		[Column(Name="JoinDate", Storage="_JoinDate", DbType="datetime")]
		public DateTime? JoinDate
		{
			get
			{
				return this._JoinDate;
			}

			set
			{
				if (this._JoinDate != value)
					this._JoinDate = value;
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

		
		[Column(Name="Option", Storage="_Option", DbType="nvarchar(100)")]
		public string Option
		{
			get
			{
				return this._Option;
			}

			set
			{
				if (this._Option != value)
					this._Option = value;
			}

		}

		
		[Column(Name="Addr", Storage="_Addr", DbType="nvarchar(100)")]
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

		
		[Column(Name="Addr2", Storage="_Addr2", DbType="nvarchar(100)")]
		public string Addr2
		{
			get
			{
				return this._Addr2;
			}

			set
			{
				if (this._Addr2 != value)
					this._Addr2 = value;
			}

		}

		
		[Column(Name="City", Storage="_City", DbType="nvarchar(30)")]
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

		
		[Column(Name="ST", Storage="_St", DbType="nvarchar(20)")]
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

		
		[Column(Name="Zip", Storage="_Zip", DbType="nvarchar(15)")]
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

		
    }

}
