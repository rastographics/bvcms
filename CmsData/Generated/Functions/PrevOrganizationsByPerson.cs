using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PrevOrganizationsByPerson")]
    public partial class PrevOrganizationsByPerson
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrganizationName;

        private string _Location;

        private int? _LeaderId;

        private string _LeaderFirst;

        private string _LeaderLast;

        private string _ScheduleName;

        public PrevOrganizationsByPerson()
        {
        }

        [Column(Name = "ORGANIZATION_ID", Storage = "_OrganizationId", DbType = "int NOT NULL")]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
                }
            }
        }

        [Column(Name = "ORGANIZATION_NAME", Storage = "_OrganizationName", DbType = "varchar(40) NOT NULL")]
        public string OrganizationName
        {
            get => _OrganizationName;

            set
            {
                if (_OrganizationName != value)
                {
                    _OrganizationName = value;
                }
            }
        }

        [Column(Name = "LOCATION", Storage = "_Location", DbType = "varchar(40)")]
        public string Location
        {
            get => _Location;

            set
            {
                if (_Location != value)
                {
                    _Location = value;
                }
            }
        }

        [Column(Name = "LeaderId", Storage = "_LeaderId", DbType = "int")]
        public int? LeaderId
        {
            get => _LeaderId;

            set
            {
                if (_LeaderId != value)
                {
                    _LeaderId = value;
                }
            }
        }

        [Column(Name = "LeaderFirst", Storage = "_LeaderFirst", DbType = "varchar(15)")]
        public string LeaderFirst
        {
            get => _LeaderFirst;

            set
            {
                if (_LeaderFirst != value)
                {
                    _LeaderFirst = value;
                }
            }
        }

        [Column(Name = "LeaderLast", Storage = "_LeaderLast", DbType = "varchar(20)")]
        public string LeaderLast
        {
            get => _LeaderLast;

            set
            {
                if (_LeaderLast != value)
                {
                    _LeaderLast = value;
                }
            }
        }

        [Column(Name = "SCHEDULE_NAME", Storage = "_ScheduleName", DbType = "varchar(40)")]
        public string ScheduleName
        {
            get => _ScheduleName;

            set
            {
                if (_ScheduleName != value)
                {
                    _ScheduleName = value;
                }
            }
        }
    }
}
