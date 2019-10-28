using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PotentialDups")]
    public partial class PotentialDup
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId0;

        private int _PeopleId;

        private bool? _S0;

        private bool? _S1;

        private bool? _S2;

        private bool? _S3;

        private bool? _S4;

        private bool? _S5;

        private bool? _S6;

        private string _First;

        private string _Last;

        private string _Nick;

        private string _Middle;

        private string _Maiden;

        private int? _BMon;

        private int? _BDay;

        private int? _BYear;

        private string _Email;

        private string _FamAddr;

        private string _PerAddr;

        private string _Member;

        public PotentialDup()
        {
        }

        [Column(Name = "PeopleId0", Storage = "_PeopleId0", DbType = "int NOT NULL")]
        public int PeopleId0
        {
            get => _PeopleId0;

            set
            {
                if (_PeopleId0 != value)
                {
                    _PeopleId0 = value;
                }
            }
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

        [Column(Name = "S0", Storage = "_S0", DbType = "bit")]
        public bool? S0
        {
            get => _S0;

            set
            {
                if (_S0 != value)
                {
                    _S0 = value;
                }
            }
        }

        [Column(Name = "S1", Storage = "_S1", DbType = "bit")]
        public bool? S1
        {
            get => _S1;

            set
            {
                if (_S1 != value)
                {
                    _S1 = value;
                }
            }
        }

        [Column(Name = "S2", Storage = "_S2", DbType = "bit")]
        public bool? S2
        {
            get => _S2;

            set
            {
                if (_S2 != value)
                {
                    _S2 = value;
                }
            }
        }

        [Column(Name = "S3", Storage = "_S3", DbType = "bit")]
        public bool? S3
        {
            get => _S3;

            set
            {
                if (_S3 != value)
                {
                    _S3 = value;
                }
            }
        }

        [Column(Name = "S4", Storage = "_S4", DbType = "bit")]
        public bool? S4
        {
            get => _S4;

            set
            {
                if (_S4 != value)
                {
                    _S4 = value;
                }
            }
        }

        [Column(Name = "S5", Storage = "_S5", DbType = "bit")]
        public bool? S5
        {
            get => _S5;

            set
            {
                if (_S5 != value)
                {
                    _S5 = value;
                }
            }
        }

        [Column(Name = "S6", Storage = "_S6", DbType = "bit")]
        public bool? S6
        {
            get => _S6;

            set
            {
                if (_S6 != value)
                {
                    _S6 = value;
                }
            }
        }

        [Column(Name = "First", Storage = "_First", DbType = "nvarchar(25)")]
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

        [Column(Name = "Last", Storage = "_Last", DbType = "nvarchar(100) NOT NULL")]
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

        [Column(Name = "Nick", Storage = "_Nick", DbType = "nvarchar(25)")]
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

        [Column(Name = "Middle", Storage = "_Middle", DbType = "nvarchar(30)")]
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

        [Column(Name = "Maiden", Storage = "_Maiden", DbType = "nvarchar(20)")]
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

        [Column(Name = "Email", Storage = "_Email", DbType = "nvarchar(150)")]
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

        [Column(Name = "FamAddr", Storage = "_FamAddr", DbType = "nvarchar(100)")]
        public string FamAddr
        {
            get => _FamAddr;

            set
            {
                if (_FamAddr != value)
                {
                    _FamAddr = value;
                }
            }
        }

        [Column(Name = "PerAddr", Storage = "_PerAddr", DbType = "nvarchar(100)")]
        public string PerAddr
        {
            get => _PerAddr;

            set
            {
                if (_PerAddr != value)
                {
                    _PerAddr = value;
                }
            }
        }

        [Column(Name = "Member", Storage = "_Member", DbType = "nvarchar(50)")]
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
