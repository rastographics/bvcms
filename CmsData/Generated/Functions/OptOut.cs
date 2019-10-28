using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OptOuts")]
    public partial class OptOut
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Name;

        private bool? _OptOutX;

        private int? _HhPeopleId;

        private string _HhName;

        private string _HhEmail;

        private int? _HhSpPeopleId;

        private string _HhSpName;

        private string _HhSpEmail;

        public OptOut()
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

        [Column(Name = "OptOut", Storage = "_OptOutX", DbType = "bit")]
        public bool? OptOutX
        {
            get => _OptOutX;

            set
            {
                if (_OptOutX != value)
                {
                    _OptOutX = value;
                }
            }
        }

        [Column(Name = "HhPeopleId", Storage = "_HhPeopleId", DbType = "int")]
        public int? HhPeopleId
        {
            get => _HhPeopleId;

            set
            {
                if (_HhPeopleId != value)
                {
                    _HhPeopleId = value;
                }
            }
        }

        [Column(Name = "HhName", Storage = "_HhName", DbType = "nvarchar(138)")]
        public string HhName
        {
            get => _HhName;

            set
            {
                if (_HhName != value)
                {
                    _HhName = value;
                }
            }
        }

        [Column(Name = "HhEmail", Storage = "_HhEmail", DbType = "nvarchar(150)")]
        public string HhEmail
        {
            get => _HhEmail;

            set
            {
                if (_HhEmail != value)
                {
                    _HhEmail = value;
                }
            }
        }

        [Column(Name = "HhSpPeopleId", Storage = "_HhSpPeopleId", DbType = "int")]
        public int? HhSpPeopleId
        {
            get => _HhSpPeopleId;

            set
            {
                if (_HhSpPeopleId != value)
                {
                    _HhSpPeopleId = value;
                }
            }
        }

        [Column(Name = "HhSpName", Storage = "_HhSpName", DbType = "nvarchar(138)")]
        public string HhSpName
        {
            get => _HhSpName;

            set
            {
                if (_HhSpName != value)
                {
                    _HhSpName = value;
                }
            }
        }

        [Column(Name = "HhSpEmail", Storage = "_HhSpEmail", DbType = "nvarchar(150)")]
        public string HhSpEmail
        {
            get => _HhSpEmail;

            set
            {
                if (_HhSpEmail != value)
                {
                    _HhSpEmail = value;
                }
            }
        }
    }
}
