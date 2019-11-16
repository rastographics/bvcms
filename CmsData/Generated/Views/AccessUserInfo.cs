using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "AccessUserInfo")]
    public partial class AccessUserInfo
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _PeopleId;

        private string _Roles;

        private DateTime? _Lastactive;

        private string _First;

        private string _Goesby;

        private string _Last;

        private int _Married;

        private int _Gender;

        private string _Cphone;

        private string _Hphone;

        private string _Wphone;

        private int? _Bday;

        private int? _Bmon;

        private int? _Byear;

        private string _Company;

        private string _Email;

        private string _Emali2;

        private string _Username;

        public AccessUserInfo()
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

        [Column(Name = "roles", Storage = "_Roles", DbType = "nvarchar")]
        public string Roles
        {
            get => _Roles;

            set
            {
                if (_Roles != value)
                {
                    _Roles = value;
                }
            }
        }

        [Column(Name = "lastactive", Storage = "_Lastactive", DbType = "datetime")]
        public DateTime? Lastactive
        {
            get => _Lastactive;

            set
            {
                if (_Lastactive != value)
                {
                    _Lastactive = value;
                }
            }
        }

        [Column(Name = "first", Storage = "_First", DbType = "nvarchar(25)")]
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

        [Column(Name = "goesby", Storage = "_Goesby", DbType = "nvarchar(25)")]
        public string Goesby
        {
            get => _Goesby;

            set
            {
                if (_Goesby != value)
                {
                    _Goesby = value;
                }
            }
        }

        [Column(Name = "last", Storage = "_Last", DbType = "nvarchar(100) NOT NULL")]
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

        [Column(Name = "married", Storage = "_Married", DbType = "int NOT NULL")]
        public int Married
        {
            get => _Married;

            set
            {
                if (_Married != value)
                {
                    _Married = value;
                }
            }
        }

        [Column(Name = "gender", Storage = "_Gender", DbType = "int NOT NULL")]
        public int Gender
        {
            get => _Gender;

            set
            {
                if (_Gender != value)
                {
                    _Gender = value;
                }
            }
        }

        [Column(Name = "cphone", Storage = "_Cphone", DbType = "nvarchar(20)")]
        public string Cphone
        {
            get => _Cphone;

            set
            {
                if (_Cphone != value)
                {
                    _Cphone = value;
                }
            }
        }

        [Column(Name = "hphone", Storage = "_Hphone", DbType = "nvarchar(20)")]
        public string Hphone
        {
            get => _Hphone;

            set
            {
                if (_Hphone != value)
                {
                    _Hphone = value;
                }
            }
        }

        [Column(Name = "wphone", Storage = "_Wphone", DbType = "nvarchar(20)")]
        public string Wphone
        {
            get => _Wphone;

            set
            {
                if (_Wphone != value)
                {
                    _Wphone = value;
                }
            }
        }

        [Column(Name = "bday", Storage = "_Bday", DbType = "int")]
        public int? Bday
        {
            get => _Bday;

            set
            {
                if (_Bday != value)
                {
                    _Bday = value;
                }
            }
        }

        [Column(Name = "bmon", Storage = "_Bmon", DbType = "int")]
        public int? Bmon
        {
            get => _Bmon;

            set
            {
                if (_Bmon != value)
                {
                    _Bmon = value;
                }
            }
        }

        [Column(Name = "byear", Storage = "_Byear", DbType = "int")]
        public int? Byear
        {
            get => _Byear;

            set
            {
                if (_Byear != value)
                {
                    _Byear = value;
                }
            }
        }

        [Column(Name = "company", Storage = "_Company", DbType = "nvarchar(120)")]
        public string Company
        {
            get => _Company;

            set
            {
                if (_Company != value)
                {
                    _Company = value;
                }
            }
        }

        [Column(Name = "email", Storage = "_Email", DbType = "nvarchar(150)")]
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

        [Column(Name = "emali2", Storage = "_Emali2", DbType = "nvarchar(60)")]
        public string Emali2
        {
            get => _Emali2;

            set
            {
                if (_Emali2 != value)
                {
                    _Emali2 = value;
                }
            }
        }

        [Column(Name = "Username", Storage = "_Username", DbType = "nvarchar(50) NOT NULL")]
        public string Username
        {
            get => _Username;

            set
            {
                if (_Username != value)
                {
                    _Username = value;
                }
            }
        }
    }
}
