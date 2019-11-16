using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "CustomMenuRoles")]
    public partial class CustomMenuRole
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Link;

        private string _Role;

        private string _Name;

        private int _Col;

        public CustomMenuRole()
        {
        }

        [Column(Name = "Link", Storage = "_Link", DbType = "varchar(100)")]
        public string Link
        {
            get => _Link;

            set
            {
                if (_Link != value)
                {
                    _Link = value;
                }
            }
        }

        [Column(Name = "Role", Storage = "_Role", DbType = "nvarchar")]
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar")]
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

        [Column(Name = "Col", Storage = "_Col", DbType = "int NOT NULL")]
        public int Col
        {
            get => _Col;

            set
            {
                if (_Col != value)
                {
                    _Col = value;
                }
            }
        }
    }
}
