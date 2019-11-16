using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ActivityLogSearch")]
    public partial class ActivityLogSearch
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Machine;

        private DateTime? _DateX;

        private int? _UserId;

        private string _UserName;

        private string _Activity;

        private string _OrgName;

        private int? _OrgId;

        private int? _PeopleId;

        private string _PersonName;

        private int? _DatumId;

        private int? _MaxRows;

        public ActivityLogSearch()
        {
        }

        [Column(Name = "Machine", Storage = "_Machine", DbType = "varchar(50)")]
        public string Machine
        {
            get => _Machine;

            set
            {
                if (_Machine != value)
                {
                    _Machine = value;
                }
            }
        }

        [Column(Name = "date", Storage = "_DateX", DbType = "datetime")]
        public DateTime? DateX
        {
            get => _DateX;

            set
            {
                if (_DateX != value)
                {
                    _DateX = value;
                }
            }
        }

        [Column(Name = "UserId", Storage = "_UserId", DbType = "int")]
        public int? UserId
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

        [Column(Name = "UserName", Storage = "_UserName", DbType = "nvarchar(50)")]
        public string UserName
        {
            get => _UserName;

            set
            {
                if (_UserName != value)
                {
                    _UserName = value;
                }
            }
        }

        [Column(Name = "Activity", Storage = "_Activity", DbType = "nvarchar(200)")]
        public string Activity
        {
            get => _Activity;

            set
            {
                if (_Activity != value)
                {
                    _Activity = value;
                }
            }
        }

        [Column(Name = "OrgName", Storage = "_OrgName", DbType = "nvarchar(100)")]
        public string OrgName
        {
            get => _OrgName;

            set
            {
                if (_OrgName != value)
                {
                    _OrgName = value;
                }
            }
        }

        [Column(Name = "OrgId", Storage = "_OrgId", DbType = "int")]
        public int? OrgId
        {
            get => _OrgId;

            set
            {
                if (_OrgId != value)
                {
                    _OrgId = value;
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

        [Column(Name = "PersonName", Storage = "_PersonName", DbType = "nvarchar(139)")]
        public string PersonName
        {
            get => _PersonName;

            set
            {
                if (_PersonName != value)
                {
                    _PersonName = value;
                }
            }
        }

        [Column(Name = "DatumId", Storage = "_DatumId", DbType = "int")]
        public int? DatumId
        {
            get => _DatumId;

            set
            {
                if (_DatumId != value)
                {
                    _DatumId = value;
                }
            }
        }

        [Column(Name = "MaxRows", Storage = "_MaxRows", DbType = "int")]
        public int? MaxRows
        {
            get => _MaxRows;

            set
            {
                if (_MaxRows != value)
                {
                    _MaxRows = value;
                }
            }
        }
    }
}
