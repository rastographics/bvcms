using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RegistrationList")]
    public partial class RegistrationList
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Id;

        private DateTime? _Stamp;

        private int? _OrganizationId;

        private string _OrganizationName;

        private int? _PeopleId;

        private string _Name;

        private string _Dob;

        private string _First;

        private string _Last;

        private int? _PeopleId1;

        private int? _PeopleId2;

        private int? _PeopleId3;

        private int? _PeopleId4;

        private int? _Cnt;

        private bool? _Mobile;

        private string _RegisterLinkType;

        private bool? _Completed;

        private bool? _Abandoned;

        private int? _UserPeopleId;

        private bool? _Expired;

        private DateTime? _RegStart;

        private DateTime? _RegEnd;

        public RegistrationList()
        {
        }

        [Column(Name = "Id", Storage = "_Id", DbType = "int NOT NULL")]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    _Id = value;
                }
            }
        }

        [Column(Name = "Stamp", Storage = "_Stamp", DbType = "datetime")]
        public DateTime? Stamp
        {
            get => _Stamp;

            set
            {
                if (_Stamp != value)
                {
                    _Stamp = value;
                }
            }
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int")]
        public int? OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
                }
            }
        }

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "nvarchar(100)")]
        public string OrganizationName
        {
            get => _OrganizationName;

            set
            {
                if (_OrganizationName != value)
                {
                    _OrganizationName = value;
                }
            }
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(138)")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    _Name = value;
                }
            }
        }

        [Column(Name = "dob", Storage = "_Dob", DbType = "varchar(50)")]
        public string Dob
        {
            get => _Dob;

            set
            {
                if (_Dob != value)
                {
                    _Dob = value;
                }
            }
        }

        [Column(Name = "first", Storage = "_First", DbType = "varchar(50)")]
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

        [Column(Name = "last", Storage = "_Last", DbType = "varchar(50)")]
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

        [Column(Name = "PeopleId1", Storage = "_PeopleId1", DbType = "int")]
        public int? PeopleId1
        {
            get => _PeopleId1;

            set
            {
                if (_PeopleId1 != value)
                {
                    _PeopleId1 = value;
                }
            }
        }

        [Column(Name = "PeopleId2", Storage = "_PeopleId2", DbType = "int")]
        public int? PeopleId2
        {
            get => _PeopleId2;

            set
            {
                if (_PeopleId2 != value)
                {
                    _PeopleId2 = value;
                }
            }
        }

        [Column(Name = "PeopleId3", Storage = "_PeopleId3", DbType = "int")]
        public int? PeopleId3
        {
            get => _PeopleId3;

            set
            {
                if (_PeopleId3 != value)
                {
                    _PeopleId3 = value;
                }
            }
        }

        [Column(Name = "PeopleId4", Storage = "_PeopleId4", DbType = "int")]
        public int? PeopleId4
        {
            get => _PeopleId4;

            set
            {
                if (_PeopleId4 != value)
                {
                    _PeopleId4 = value;
                }
            }
        }

        [Column(Name = "cnt", Storage = "_Cnt", DbType = "int")]
        public int? Cnt
        {
            get => _Cnt;

            set
            {
                if (_Cnt != value)
                {
                    _Cnt = value;
                }
            }
        }

        [Column(Name = "mobile", Storage = "_Mobile", DbType = "bit")]
        public bool? Mobile
        {
            get => _Mobile;

            set
            {
                if (_Mobile != value)
                {
                    _Mobile = value;
                }
            }
        }

        [Column(Name = "registerLinkType", Storage = "_RegisterLinkType", DbType = "varchar(50)")]
        public string RegisterLinkType
        {
            get => _RegisterLinkType;

            set
            {
                if (_RegisterLinkType != value)
                {
                    _RegisterLinkType = value;
                }
            }
        }

        [Column(Name = "completed", Storage = "_Completed", DbType = "bit")]
        public bool? Completed
        {
            get => _Completed;

            set
            {
                if (_Completed != value)
                {
                    _Completed = value;
                }
            }
        }

        [Column(Name = "abandoned", Storage = "_Abandoned", DbType = "bit")]
        public bool? Abandoned
        {
            get => _Abandoned;

            set
            {
                if (_Abandoned != value)
                {
                    _Abandoned = value;
                }
            }
        }

        [Column(Name = "UserPeopleId", Storage = "_UserPeopleId", DbType = "int")]
        public int? UserPeopleId
        {
            get => _UserPeopleId;

            set
            {
                if (_UserPeopleId != value)
                {
                    _UserPeopleId = value;
                }
            }
        }

        [Column(Name = "expired", Storage = "_Expired", DbType = "bit")]
        public bool? Expired
        {
            get => _Expired;

            set
            {
                if (_Expired != value)
                {
                    _Expired = value;
                }
            }
        }

        [Column(Name = "RegStart", Storage = "_RegStart", DbType = "datetime")]
        public DateTime? RegStart
        {
            get => _RegStart;

            set
            {
                if (_RegStart != value)
                {
                    _RegStart = value;
                }
            }
        }

        [Column(Name = "RegEnd", Storage = "_RegEnd", DbType = "datetime")]
        public DateTime? RegEnd
        {
            get => _RegEnd;

            set
            {
                if (_RegEnd != value)
                {
                    _RegEnd = value;
                }
            }
        }
    }
}
