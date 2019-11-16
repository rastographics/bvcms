using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "UserRoles")]
    public partial class UserRole
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Name;

        private string _EmailAddress;

        private string _Roles;

        public UserRole()
        {
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

        [Column(Name = "Roles", Storage = "_Roles", DbType = "nvarchar")]
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
