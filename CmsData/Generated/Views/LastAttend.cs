using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "LastAttends")]
    public partial class LastAttend
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Name2;

        private int _OrganizationId;

        private string _OrganizationName;

        private string _LastAttendX;

        private string _HomePhone;

        private string _CellPhone;

        private string _EmailAddress;

        private bool? _HasHomePhone;

        private bool? _HasCellPhone;

        private bool? _HasEmail;

        public LastAttend()
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

        [Column(Name = "Name2", Storage = "_Name2", DbType = "nvarchar(139)")]
        public string Name2
        {
            get => _Name2;

            set
            {
                if (_Name2 != value)
                {
                    _Name2 = value;
                }
            }
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int NOT NULL")]
        public int OrganizationId
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

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "nvarchar(100) NOT NULL")]
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

        [Column(Name = "LastAttend", Storage = "_LastAttendX", DbType = "nvarchar(4000)")]
        public string LastAttendX
        {
            get => _LastAttendX;

            set
            {
                if (_LastAttendX != value)
                {
                    _LastAttendX = value;
                }
            }
        }

        [Column(Name = "HomePhone", Storage = "_HomePhone", DbType = "nvarchar(32)")]
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

        [Column(Name = "CellPhone", Storage = "_CellPhone", DbType = "nvarchar(32)")]
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

        [Column(Name = "HasHomePhone", Storage = "_HasHomePhone", DbType = "bit")]
        public bool? HasHomePhone
        {
            get => _HasHomePhone;

            set
            {
                if (_HasHomePhone != value)
                {
                    _HasHomePhone = value;
                }
            }
        }

        [Column(Name = "HasCellPhone", Storage = "_HasCellPhone", DbType = "bit")]
        public bool? HasCellPhone
        {
            get => _HasCellPhone;

            set
            {
                if (_HasCellPhone != value)
                {
                    _HasCellPhone = value;
                }
            }
        }

        [Column(Name = "HasEmail", Storage = "_HasEmail", DbType = "bit")]
        public bool? HasEmail
        {
            get => _HasEmail;

            set
            {
                if (_HasEmail != value)
                {
                    _HasEmail = value;
                }
            }
        }
    }
}
