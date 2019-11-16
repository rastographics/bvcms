using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "UserList")]
    public partial class UserList
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Username;

        private int _UserId;

        private string _Name;

        private string _Name2;

        private bool _IsApproved;

        private bool _MustChangePassword;

        private bool _IsLockedOut;

        private string _EmailAddress;

        private DateTime? _LastActivityDate;

        private int? _PeopleId;

        private string _Roles;

        public UserList()
        {
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

        [Column(Name = "UserId", Storage = "_UserId", DbType = "int NOT NULL")]
        public int UserId
        {
            get => _UserId;

            set
            {
                if (_UserId != value)
                {
                    _UserId = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(50)")]
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

        [Column(Name = "Name2", Storage = "_Name2", DbType = "nvarchar(50)")]
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

        [Column(Name = "IsApproved", Storage = "_IsApproved", DbType = "bit NOT NULL")]
        public bool IsApproved
        {
            get => _IsApproved;

            set
            {
                if (_IsApproved != value)
                {
                    _IsApproved = value;
                }
            }
        }

        [Column(Name = "MustChangePassword", Storage = "_MustChangePassword", DbType = "bit NOT NULL")]
        public bool MustChangePassword
        {
            get => _MustChangePassword;

            set
            {
                if (_MustChangePassword != value)
                {
                    _MustChangePassword = value;
                }
            }
        }

        [Column(Name = "IsLockedOut", Storage = "_IsLockedOut", DbType = "bit NOT NULL")]
        public bool IsLockedOut
        {
            get => _IsLockedOut;

            set
            {
                if (_IsLockedOut != value)
                {
                    _IsLockedOut = value;
                }
            }
        }

        [Column(Name = "EmailAddress", Storage = "_EmailAddress", DbType = "nvarchar(100)")]
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

        [Column(Name = "LastActivityDate", Storage = "_LastActivityDate", DbType = "datetime")]
        public DateTime? LastActivityDate
        {
            get => _LastActivityDate;

            set
            {
                if (_LastActivityDate != value)
                {
                    _LastActivityDate = value;
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

        [Column(Name = "Roles", Storage = "_Roles", DbType = "nvarchar(500)")]
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
    }
}
