using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetContributions")]
    public partial class GetContribution
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int NOT NULL")]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    _PeopleId = value;
                }
            }
        }

        [Column(Name = "First", Storage = "_First", DbType = "varchar(25)")]
        public string First
        {
            get => _First;

            set
            {
                if (_First != value)
                {
                    _First = value;
                }
            }
        }

        [Column(Name = "Spouse", Storage = "_Spouse", DbType = "varchar(25)")]
        public string Spouse
        {
            get => _Spouse;

            set
            {
                if (_Spouse != value)
                {
                    _Spouse = value;
                }
            }
        }

        [Column(Name = "LAST", Storage = "_Last", DbType = "varchar(100) NOT NULL")]
        public string Last
        {
            get => _Last;

            set
            {
                if (_Last != value)
                {
                    _Last = value;
                }
            }
        }

        [Column(Name = "Addr", Storage = "_Addr", DbType = "varchar(100)")]
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

        [Column(Name = "City", Storage = "_City", DbType = "varchar(30)")]
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

        [Column(Name = "ST", Storage = "_St", DbType = "varchar(20)")]
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

        [Column(Name = "Zip", Storage = "_Zip", DbType = "varchar(15)")]
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

        [Column(Name = "ContributionDate", Storage = "_ContributionDate", DbType = "datetime")]
        public DateTime? ContributionDate
        {
            get => _ContributionDate;

            set
            {
                if (_ContributionDate != value)
                {
                    _ContributionDate = value;
                }
            }
        }

        [Column(Name = "Amt", Storage = "_Amt", DbType = "Decimal(38,2)")]
        public decimal? Amt
        {
            get => _Amt;

            set
            {
                if (_Amt != value)
                {
                    _Amt = value;
                }
            }
        }
    }
}
