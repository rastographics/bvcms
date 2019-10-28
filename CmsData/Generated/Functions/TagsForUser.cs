using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "TagsForUser")]
    public partial class TagsForUser
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Id;

        private string _Name;

        private int _TypeId;

        private string _Owner;

        public TagsForUser()
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

        [Column(Name = "Name", Storage = "_Name", DbType = "varchar(50) NOT NULL")]
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

        [Column(Name = "TypeId", Storage = "_TypeId", DbType = "int NOT NULL")]
        public int TypeId
        {
            get => _TypeId;

            set
            {
                if (_TypeId != value)
                {
                    _TypeId = value;
                }
            }
        }

        [Column(Name = "Owner", Storage = "_Owner", DbType = "varchar(50)")]
        public string Owner
        {
            get => _Owner;

            set
            {
                if (_Owner != value)
                {
                    _Owner = value;
                }
            }
        }
    }
}
