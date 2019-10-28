using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ChangeLogDetails")]
    public partial class ChangeLogDetail
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Id;

        private int _PeopleId;

        private string _Section;

        private DateTime _Created;

        private int? _FamilyId;

        private int _UserPeopleId;

        private string _Field;

        private string _Before;

        private string _After;

        public ChangeLogDetail()
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

        [Column(Name = "Section", Storage = "_Section", DbType = "nvarchar(50)")]
        public string Section
        {
            get => _Section;

            set
            {
                if (_Section != value)
                {
                    _Section = value;
                }
            }
        }

        [Column(Name = "Created", Storage = "_Created", DbType = "datetime NOT NULL")]
        public DateTime Created
        {
            get => _Created;

            set
            {
                if (_Created != value)
                {
                    _Created = value;
                }
            }
        }

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int")]
        public int? FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    _FamilyId = value;
                }
            }
        }

        [Column(Name = "UserPeopleId", Storage = "_UserPeopleId", DbType = "int NOT NULL")]
        public int UserPeopleId
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

        [Column(Name = "Field", Storage = "_Field", DbType = "nvarchar(50) NOT NULL")]
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

        [Column(Name = "Before", Storage = "_Before", DbType = "nvarchar")]
        public string Before
        {
            get => _Before;

            set
            {
                if (_Before != value)
                {
                    _Before = value;
                }
            }
        }

        [Column(Name = "After", Storage = "_After", DbType = "nvarchar")]
        public string After
        {
            get => _After;

            set
            {
                if (_After != value)
                {
                    _After = value;
                }
            }
        }
    }
}
