using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetTotalContributionsDonor")]
    public partial class GetTotalContributionsDonor
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
        private int _ContributionStatusId;
        private int _ContributionTypeId;

        public GetTotalContributionsDonor()
        {
        }

        [Column(Name = "ContributionStatusId", Storage = "_ContributionStatusId", DbType = "int NOT NULL")]
        public int ContributionStatusId
        {
            get => _ContributionStatusId;

            set
            {
                if (_ContributionStatusId != value)
                {
                    _ContributionStatusId = value;
                }
            }
        }

        [Column(Name = "ContributionTypeId", Storage = "_ContributionTypeId", DbType = "int NOT NULL")]
        public int ContributionTypeId
        {
            get => _ContributionTypeId;

            set
            {
                if (_ContributionTypeId != value)
                {
                    _ContributionTypeId = value;
                }
            }
        }

        [Column(Name = "CreditGiverId", Storage = "_CreditGiverId", DbType = "int")]
        public int? CreditGiverId
        {
            get => _CreditGiverId;

            set
            {
                if (_CreditGiverId != value)
                {
                    _CreditGiverId = value;
                }
            }
        }

        [Column(Name = "CreditGiverId2", Storage = "_CreditGiverId2", DbType = "int")]
        public int? CreditGiverId2
        {
            get => _CreditGiverId2;

            set
            {
                if (_CreditGiverId2 != value)
                {
                    _CreditGiverId2 = value;
                }
            }
        }

        [Column(Name = "HeadName", Storage = "_HeadName", DbType = "nvarchar(139)")]
        public string HeadName
        {
            get => _HeadName;

            set
            {
                if (_HeadName != value)
                {
                    _HeadName = value;
                }
            }
        }

        [Column(Name = "Head_LastName", Storage = "_Head_LastName", DbType = "nvarchar(139)")]
        public string Head_LastName
        {
            get => _Head_LastName;

            set
            {
                if (_Head_LastName != value)
                {
                    _Head_LastName = value;
                }
            }
        }

        [Column(Name = "Head_FirstName", Storage = "_Head_FirstName", DbType = "nvarchar(139)")]
        public string Head_FirstName
        {
            get => _Head_FirstName;

            set
            {
                if (_Head_FirstName != value)
                {
                    _Head_FirstName = value;
                }
            }
        }

        [Column(Name = "Email", Storage = "_Email", DbType = "nvarchar(139)")]
        public string Email
        {
            get => _Email;

            set
            {
                if (_Email != value)
                {
                    _Email = value;
                }
            }
        }

        [Column(Name = "SpouseName", Storage = "_SpouseName", DbType = "nvarchar(139)")]
        public string SpouseName
        {
            get => _SpouseName;

            set
            {
                if (_SpouseName != value)
                {
                    _SpouseName = value;
                }
            }
        }

        [Column(Name = "Count", Storage = "_Count", DbType = "int")]
        public int? Count
        {
            get => _Count;

            set
            {
                if (_Count != value)
                {
                    _Count = value;
                }
            }
        }

        [Column(Name = "Amount", Storage = "_Amount", DbType = "Decimal(38,2)")]
        public decimal? Amount
        {
            get => _Amount;

            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                }
            }
        }

        [Column(Name = "PledgeAmount", Storage = "_PledgeAmount", DbType = "Decimal(38,2)")]
        public decimal? PledgeAmount
        {
            get => _PledgeAmount;

            set
            {
                if (_PledgeAmount != value)
                {
                    _PledgeAmount = value;
                }
            }
        }

        [Column(Name = "MainFellowship", Storage = "_MainFellowship", DbType = "nvarchar(100) NOT NULL")]
        public string MainFellowship
        {
            get => _MainFellowship;

            set
            {
                if (_MainFellowship != value)
                {
                    _MainFellowship = value;
                }
            }
        }

        [Column(Name = "MemberStatus", Storage = "_MemberStatus", DbType = "nvarchar(50)")]
        public string MemberStatus
        {
            get => _MemberStatus;

            set
            {
                if (_MemberStatus != value)
                {
                    _MemberStatus = value;
                }
            }
        }

        [Column(Name = "JoinDate", Storage = "_JoinDate", DbType = "datetime")]
        public DateTime? JoinDate
        {
            get => _JoinDate;

            set
            {
                if (_JoinDate != value)
                {
                    _JoinDate = value;
                }
            }
        }

        [Column(Name = "SpouseId", Storage = "_SpouseId", DbType = "int")]
        public int? SpouseId
        {
            get => _SpouseId;

            set
            {
                if (_SpouseId != value)
                {
                    _SpouseId = value;
                }
            }
        }

        [Column(Name = "Option", Storage = "_Option", DbType = "nvarchar(100)")]
        public string Option
        {
            get => _Option;

            set
            {
                if (_Option != value)
                {
                    _Option = value;
                }
            }
        }

        [Column(Name = "Addr", Storage = "_Addr", DbType = "nvarchar(100)")]
        public string Addr
        {
            get => _Addr;

            set
            {
                if (_Addr != value)
                {
                    _Addr = value;
                }
            }
        }

        [Column(Name = "Addr2", Storage = "_Addr2", DbType = "nvarchar(100)")]
        public string Addr2
        {
            get => _Addr2;

            set
            {
                if (_Addr2 != value)
                {
                    _Addr2 = value;
                }
            }
        }

        [Column(Name = "City", Storage = "_City", DbType = "nvarchar(30)")]
        public string City
        {
            get => _City;

            set
            {
                if (_City != value)
                {
                    _City = value;
                }
            }
        }

        [Column(Name = "ST", Storage = "_St", DbType = "nvarchar(20)")]
        public string St
        {
            get => _St;

            set
            {
                if (_St != value)
                {
                    _St = value;
                }
            }
        }

        [Column(Name = "Zip", Storage = "_Zip", DbType = "nvarchar(15)")]
        public string Zip
        {
            get => _Zip;

            set
            {
                if (_Zip != value)
                {
                    _Zip = value;
                }
            }
        }
    }
}
