using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "UserLeaders")]
    public partial class UserLeader
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Name;

        private string _Username;

        private int? _UserId;

        private string _Access;

        private string _OrgLeaderOnly;

        public UserLeader()
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

        [Column(Name = "Username", Storage = "_Username", DbType = "nvarchar(50)")]
        public string Username
        {
            get => _Username;

            set
            {
                if (_Username != value)
                {
                    _Username = value;
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

        [Column(Name = "Access", Storage = "_Access", DbType = "varchar(6)")]
        public string Access
        {
            get => _Access;

            set
            {
                if (_Access != value)
                {
                    _Access = value;
                }
            }
        }

        [Column(Name = "OrgLeaderOnly", Storage = "_OrgLeaderOnly", DbType = "varchar(14)")]
        public string OrgLeaderOnly
        {
            get => _OrgLeaderOnly;

            set
            {
                if (_OrgLeaderOnly != value)
                {
                    _OrgLeaderOnly = value;
                }
            }
        }
    }
}
