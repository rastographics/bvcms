using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "CheckinByDate")]
    public partial class CheckinByDate
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _PeopleId;

        private string _Name;

        private int? _OrgId;

        private string _OrgName;

        private string _Time;

        private bool? _Present;

        public CheckinByDate()
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

        [Column(Name = "OrgName", Storage = "_OrgName", DbType = "varchar(100)")]
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

        [Column(Name = "time", Storage = "_Time", DbType = "time")]
        public string Time
        {
            get => _Time;

            set
            {
                if (_Time != value)
                {
                    _Time = value;
                }
            }
        }

        [Column(Name = "present", Storage = "_Present", DbType = "bit")]
        public bool? Present
        {
            get => _Present;

            set
            {
                if (_Present != value)
                {
                    _Present = value;
                }
            }
        }
    }
}
