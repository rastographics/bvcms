using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OnlineRegMatches")]
    public partial class OnlineRegMatch
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _PeopleId;

        private string _First;

        private string _Last;

        private string _Nick;

        private string _Middle;

        private string _Maiden;

        private int? _BMon;

        private int? _BDay;

        private int? _BYear;

        private string _Email;

        private string _Member;

        public OnlineRegMatch()
        {
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
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

        [Column(Name = "First", Storage = "_First", DbType = "nvarchar(100)")]
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

        [Column(Name = "Last", Storage = "_Last", DbType = "nvarchar(100)")]
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

        [Column(Name = "Nick", Storage = "_Nick", DbType = "nvarchar(100)")]
        public string Nick
        {
            get => _Nick;

            set
            {
                if (_Nick != value)
                {
                    _Nick = value;
                }
            }
        }

        [Column(Name = "Middle", Storage = "_Middle", DbType = "nvarchar(100)")]
        public string Middle
        {
            get => _Middle;

            set
            {
                if (_Middle != value)
                {
                    _Middle = value;
                }
            }
        }

        [Column(Name = "Maiden", Storage = "_Maiden", DbType = "nvarchar(100)")]
        public string Maiden
        {
            get => _Maiden;

            set
            {
                if (_Maiden != value)
                {
                    _Maiden = value;
                }
            }
        }

        [Column(Name = "BMon", Storage = "_BMon", DbType = "int")]
        public int? BMon
        {
            get => _BMon;

            set
            {
                if (_BMon != value)
                {
                    _BMon = value;
                }
            }
        }

        [Column(Name = "BDay", Storage = "_BDay", DbType = "int")]
        public int? BDay
        {
            get => _BDay;

            set
            {
                if (_BDay != value)
                {
                    _BDay = value;
                }
            }
        }

        [Column(Name = "BYear", Storage = "_BYear", DbType = "int")]
        public int? BYear
        {
            get => _BYear;

            set
            {
                if (_BYear != value)
                {
                    _BYear = value;
                }
            }
        }

        [Column(Name = "Email", Storage = "_Email", DbType = "varchar(100)")]
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

        [Column(Name = "Member", Storage = "_Member", DbType = "nvarchar(100)")]
        public string Member
        {
            get => _Member;

            set
            {
                if (_Member != value)
                {
                    _Member = value;
                }
            }
        }
    }
}
