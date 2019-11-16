using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RecentIncompleteRegistrations2")]
    public partial class RecentIncompleteRegistrations2
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _PeopleId;

        private int? _OrgId;

        private int? _DatumId;

        private DateTime? _Stamp;

        private string _OrgName;

        private string _Name;

        public RecentIncompleteRegistrations2()
        {
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

        [Column(Name = "Stamp", Storage = "_Stamp", DbType = "datetime")]
        public DateTime? Stamp
        {
            get => _Stamp;

            set
            {
                if (_Stamp != value)
                {
                    _Stamp = value;
                }
            }
        }

        [Column(Name = "OrgName", Storage = "_OrgName", DbType = "nvarchar(80)")]
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(80)")]
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
    }
}
