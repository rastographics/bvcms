using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpFamily")]
    public partial class XpFamily
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _FamilyId;

        private DateTime? _AddressFromDate;

        private DateTime? _AddressToDate;

        private string _AddressLineOne;

        private string _AddressLineTwo;

        private string _CityName;

        private string _StateCode;

        private string _ZipCode;

        private string _CountryName;

        private string _ResCode;

        private string _HomePhone;

        private int? _HeadOfHouseholdId;

        private int? _HeadOfHouseholdSpouseId;

        private int? _CoupleFlag;

        private string _Comments;

        public XpFamily()
        {
        }

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int NOT NULL")]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    _FamilyId = value;
                }
            }
        }

        [Column(Name = "AddressFromDate", Storage = "_AddressFromDate", DbType = "datetime")]
        public DateTime? AddressFromDate
        {
            get => _AddressFromDate;

            set
            {
                if (_AddressFromDate != value)
                {
                    _AddressFromDate = value;
                }
            }
        }

        [Column(Name = "AddressToDate", Storage = "_AddressToDate", DbType = "datetime")]
        public DateTime? AddressToDate
        {
            get => _AddressToDate;

            set
            {
                if (_AddressToDate != value)
                {
                    _AddressToDate = value;
                }
            }
        }

        [Column(Name = "AddressLineOne", Storage = "_AddressLineOne", DbType = "nvarchar(100)")]
        public string AddressLineOne
        {
            get => _AddressLineOne;

            set
            {
                if (_AddressLineOne != value)
                {
                    _AddressLineOne = value;
                }
            }
        }

        [Column(Name = "AddressLineTwo", Storage = "_AddressLineTwo", DbType = "nvarchar(100)")]
        public string AddressLineTwo
        {
            get => _AddressLineTwo;

            set
            {
                if (_AddressLineTwo != value)
                {
                    _AddressLineTwo = value;
                }
            }
        }

        [Column(Name = "CityName", Storage = "_CityName", DbType = "nvarchar(30)")]
        public string CityName
        {
            get => _CityName;

            set
            {
                if (_CityName != value)
                {
                    _CityName = value;
                }
            }
        }

        [Column(Name = "StateCode", Storage = "_StateCode", DbType = "nvarchar(30)")]
        public string StateCode
        {
            get => _StateCode;

            set
            {
                if (_StateCode != value)
                {
                    _StateCode = value;
                }
            }
        }

        [Column(Name = "ZipCode", Storage = "_ZipCode", DbType = "nvarchar(15)")]
        public string ZipCode
        {
            get => _ZipCode;

            set
            {
                if (_ZipCode != value)
                {
                    _ZipCode = value;
                }
            }
        }

        [Column(Name = "CountryName", Storage = "_CountryName", DbType = "nvarchar(40)")]
        public string CountryName
        {
            get => _CountryName;

            set
            {
                if (_CountryName != value)
                {
                    _CountryName = value;
                }
            }
        }

        [Column(Name = "ResCode", Storage = "_ResCode", DbType = "nvarchar(100)")]
        public string ResCode
        {
            get => _ResCode;

            set
            {
                if (_ResCode != value)
                {
                    _ResCode = value;
                }
            }
        }

        [Column(Name = "HomePhone", Storage = "_HomePhone", DbType = "nvarchar(20)")]
        public string HomePhone
        {
            get => _HomePhone;

            set
            {
                if (_HomePhone != value)
                {
                    _HomePhone = value;
                }
            }
        }

        [Column(Name = "HeadOfHouseholdId", Storage = "_HeadOfHouseholdId", DbType = "int")]
        public int? HeadOfHouseholdId
        {
            get => _HeadOfHouseholdId;

            set
            {
                if (_HeadOfHouseholdId != value)
                {
                    _HeadOfHouseholdId = value;
                }
            }
        }

        [Column(Name = "HeadOfHouseholdSpouseId", Storage = "_HeadOfHouseholdSpouseId", DbType = "int")]
        public int? HeadOfHouseholdSpouseId
        {
            get => _HeadOfHouseholdSpouseId;

            set
            {
                if (_HeadOfHouseholdSpouseId != value)
                {
                    _HeadOfHouseholdSpouseId = value;
                }
            }
        }

        [Column(Name = "CoupleFlag", Storage = "_CoupleFlag", DbType = "int")]
        public int? CoupleFlag
        {
            get => _CoupleFlag;

            set
            {
                if (_CoupleFlag != value)
                {
                    _CoupleFlag = value;
                }
            }
        }

        [Column(Name = "Comments", Storage = "_Comments", DbType = "nvarchar(3000)")]
        public string Comments
        {
            get => _Comments;

            set
            {
                if (_Comments != value)
                {
                    _Comments = value;
                }
            }
        }
    }
}
