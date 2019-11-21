using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "DownlineDetails")]
    public partial class DownlineDetail
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _OrganizationName;

        private string _Leader;

        private string _Student;

        private string _Trace;

        private int? _OrgId;

        private int? _LeaderId;

        private int? _DiscipleId;

        private DateTime? _StartDt;

        private DateTime? _EndDt;

        private int? _MaxRows;

        public DownlineDetail()
        {
        }

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "nvarchar(100) NOT NULL")]
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

        [Column(Name = "Leader", Storage = "_Leader", DbType = "nvarchar(139)")]
        public string Leader
        {
            get => _Leader;

            set
            {
                if (_Leader != value)
                {
                    _Leader = value;
                }
            }
        }

        [Column(Name = "Student", Storage = "_Student", DbType = "nvarchar(139)")]
        public string Student
        {
            get => _Student;

            set
            {
                if (_Student != value)
                {
                    _Student = value;
                }
            }
        }

        [Column(Name = "Trace", Storage = "_Trace", DbType = "varchar(400)")]
        public string Trace
        {
            get => _Trace;

            set
            {
                if (_Trace != value)
                {
                    _Trace = value;
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

        [Column(Name = "DiscipleId", Storage = "_DiscipleId", DbType = "int")]
        public int? DiscipleId
        {
            get => _DiscipleId;

            set
            {
                if (_DiscipleId != value)
                {
                    _DiscipleId = value;
                }
            }
        }

        [Column(Name = "StartDt", Storage = "_StartDt", DbType = "datetime")]
        public DateTime? StartDt
        {
            get => _StartDt;

            set
            {
                if (_StartDt != value)
                {
                    _StartDt = value;
                }
            }
        }

        [Column(Name = "EndDt", Storage = "_EndDt", DbType = "datetime")]
        public DateTime? EndDt
        {
            get => _EndDt;

            set
            {
                if (_EndDt != value)
                {
                    _EndDt = value;
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
