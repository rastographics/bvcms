using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "DownlineSingleTrace")]
    public partial class DownlineSingleTrace
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Generation;

        private int? _LeaderId;

        private string _LeaderName;

        private int? _DiscipleId;

        private string _DiscipleName;

        private string _OrgName;

        private DateTime? _StartDt;

        private DateTime? _EndDt;

        private string _Trace;

        public DownlineSingleTrace()
        {
        }

        [Column(Name = "Generation", Storage = "_Generation", DbType = "int")]
        public int? Generation
        {
            get => _Generation;

            set
            {
                if (_Generation != value)
                {
                    _Generation = value;
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

        [Column(Name = "LeaderName", Storage = "_LeaderName", DbType = "varchar(100)")]
        public string LeaderName
        {
            get => _LeaderName;

            set
            {
                if (_LeaderName != value)
                {
                    _LeaderName = value;
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

        [Column(Name = "DiscipleName", Storage = "_DiscipleName", DbType = "varchar(100)")]
        public string DiscipleName
        {
            get => _DiscipleName;

            set
            {
                if (_DiscipleName != value)
                {
                    _DiscipleName = value;
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
    }
}
