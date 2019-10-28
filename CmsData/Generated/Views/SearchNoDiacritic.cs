using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "SearchNoDiacritics")]
    public partial class SearchNoDiacritic
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private int _FamilyId;

        private string _EmailAddress;

        private string _EmailAddress2;

        private string _CellPhone;

        private string _HomePhone;

        private string _WorkPhone;

        private int? _BirthMonth;

        private int? _BirthDay;

        private int? _BirthYear;

        private string _LastName;

        private string _FirstName;

        private string _NickName;

        private string _MiddleName;

        private string _MaidenName;

        private string _FirstName2;

        public SearchNoDiacritic()
        {
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
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

        [Column(Name = "EmailAddress", Storage = "_EmailAddress", DbType = "nvarchar(150)")]
        public string EmailAddress
        {
            get => _EmailAddress;

            set
            {
                if (_EmailAddress != value)
                {
                    _EmailAddress = value;
                }
            }
        }

        [Column(Name = "EmailAddress2", Storage = "_EmailAddress2", DbType = "nvarchar(60)")]
        public string EmailAddress2
        {
            get => _EmailAddress2;

            set
            {
                if (_EmailAddress2 != value)
                {
                    _EmailAddress2 = value;
                }
            }
        }

        [Column(Name = "CellPhone", Storage = "_CellPhone", DbType = "nvarchar(20)")]
        public string CellPhone
        {
            get => _CellPhone;

            set
            {
                if (_CellPhone != value)
                {
                    _CellPhone = value;
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

        [Column(Name = "WorkPhone", Storage = "_WorkPhone", DbType = "nvarchar(20)")]
        public string WorkPhone
        {
            get => _WorkPhone;

            set
            {
                if (_WorkPhone != value)
                {
                    _WorkPhone = value;
                }
            }
        }

        [Column(Name = "BirthMonth", Storage = "_BirthMonth", DbType = "int")]
        public int? BirthMonth
        {
            get => _BirthMonth;

            set
            {
                if (_BirthMonth != value)
                {
                    _BirthMonth = value;
                }
            }
        }

        [Column(Name = "BirthDay", Storage = "_BirthDay", DbType = "int")]
        public int? BirthDay
        {
            get => _BirthDay;

            set
            {
                if (_BirthDay != value)
                {
                    _BirthDay = value;
                }
            }
        }

        [Column(Name = "BirthYear", Storage = "_BirthYear", DbType = "int")]
        public int? BirthYear
        {
            get => _BirthYear;

            set
            {
                if (_BirthYear != value)
                {
                    _BirthYear = value;
                }
            }
        }

        [Column(Name = "LastName", Storage = "_LastName", DbType = "varchar(30)")]
        public string LastName
        {
            get => _LastName;

            set
            {
                if (_LastName != value)
                {
                    _LastName = value;
                }
            }
        }

        [Column(Name = "FirstName", Storage = "_FirstName", DbType = "varchar(30)")]
        public string FirstName
        {
            get => _FirstName;

            set
            {
                if (_FirstName != value)
                {
                    _FirstName = value;
                }
            }
        }

        [Column(Name = "NickName", Storage = "_NickName", DbType = "varchar(30)")]
        public string NickName
        {
            get => _NickName;

            set
            {
                if (_NickName != value)
                {
                    _NickName = value;
                }
            }
        }

        [Column(Name = "MiddleName", Storage = "_MiddleName", DbType = "varchar(30)")]
        public string MiddleName
        {
            get => _MiddleName;

            set
            {
                if (_MiddleName != value)
                {
                    _MiddleName = value;
                }
            }
        }

        [Column(Name = "MaidenName", Storage = "_MaidenName", DbType = "varchar(30)")]
        public string MaidenName
        {
            get => _MaidenName;

            set
            {
                if (_MaidenName != value)
                {
                    _MaidenName = value;
                }
            }
        }

        [Column(Name = "FirstName2", Storage = "_FirstName2", DbType = "varchar(30)")]
        public string FirstName2
        {
            get => _FirstName2;

            set
            {
                if (_FirstName2 != value)
                {
                    _FirstName2 = value;
                }
            }
        }
    }
}
