using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "StatusFlagNamesRoles")]
    public partial class StatusFlagNamesRole
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Id;

        private string _Flag;

        private string _Name;

        private string _Role;

        public StatusFlagNamesRole()
        {
        }

        [Column(Name = "Id", Storage = "_Id", DbType = "int")]
        public int? Id
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

        [Column(Name = "Flag", Storage = "_Flag", DbType = "nvarchar(200) NOT NULL")]
        public string Flag
        {
            get => _Flag;

            set
            {
                if (_Flag != value)
                {
                    _Flag = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(4000)")]
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

        [Column(Name = "Role", Storage = "_Role", DbType = "nvarchar(50)")]
        public string Role
        {
            get => _Role;

            set
            {
                if (_Role != value)
                {
                    _Role = value;
                }
            }
        }
    }
}
