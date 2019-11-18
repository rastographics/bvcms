using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "Attributes")]
    public partial class Attribute
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Field;

        private string _Name;

        private string _ValueX;

        private string _FieldAttr;

        public Attribute()
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

        [Column(Name = "Field", Storage = "_Field", DbType = "nvarchar(150) NOT NULL")]
        public string Field
        {
            get => _Field;

            set
            {
                if (_Field != value)
                {
                    _Field = value;
                }
            }
        }

        [Column(Name = "name", Storage = "_Name", DbType = "varchar(200)")]
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

        [Column(Name = "value", Storage = "_ValueX", DbType = "varchar")]
        public string ValueX
        {
            get => _ValueX;

            set
            {
                if (_ValueX != value)
                {
                    _ValueX = value;
                }
            }
        }

        [Column(Name = "FieldAttr", Storage = "_FieldAttr", DbType = "nvarchar")]
        public string FieldAttr
        {
            get => _FieldAttr;

            set
            {
                if (_FieldAttr != value)
                {
                    _FieldAttr = value;
                }
            }
        }
    }
}
