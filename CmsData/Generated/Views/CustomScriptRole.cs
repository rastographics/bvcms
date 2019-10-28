using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "CustomScriptRoles")]
    public partial class CustomScriptRole
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Name;

        private string _Type;

        private string _Role;

        private int? _ShowOnOrgId;

        private string _ClassX;

        private string _Url;

        public CustomScriptRole()
        {
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "varchar(100)")]
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

        [Column(Name = "Type", Storage = "_Type", DbType = "varchar(100) NOT NULL")]
        public string Type
        {
            get => _Type;

            set
            {
                if (_Type != value)
                {
                    _Type = value;
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

        [Column(Name = "ShowOnOrgId", Storage = "_ShowOnOrgId", DbType = "int")]
        public int? ShowOnOrgId
        {
            get => _ShowOnOrgId;

            set
            {
                if (_ShowOnOrgId != value)
                {
                    _ShowOnOrgId = value;
                }
            }
        }

        [Column(Name = "class", Storage = "_ClassX", DbType = "nvarchar")]
        public string ClassX
        {
            get => _ClassX;

            set
            {
                if (_ClassX != value)
                {
                    _ClassX = value;
                }
            }
        }

        [Column(Name = "Url", Storage = "_Url", DbType = "varchar(200)")]
        public string Url
        {
            get => _Url;

            set
            {
                if (_Url != value)
                {
                    _Url = value;
                }
            }
        }
    }
}
