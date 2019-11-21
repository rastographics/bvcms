using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "DownlineLevels")]
    public partial class DownlineLevel
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Level;

        private string _OrganizationName;

        private string _Leader;

        private int? _OrgId;

        private int? _LeaderId;

        private int? _Cnt;

        private DateTime? _StartDt;

        private DateTime? _EndDt;

        private int? _MaxRows;

        public DownlineLevel()
        {
        }

        [Column(Name = "Level", Storage = "_Level", DbType = "int")]
        public int? Level
        {
            get => _Level;

            set
            {
                if (_Level != value)
                {
                    _Level = value;
                }
            }
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

        [Column(Name = "Cnt", Storage = "_Cnt", DbType = "int")]
        public int? Cnt
        {
            get => _Cnt;

            set
            {
                if (_Cnt != value)
                {
                    _Cnt = value;
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
