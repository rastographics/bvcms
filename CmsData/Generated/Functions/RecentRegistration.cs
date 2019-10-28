using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RecentRegistrations")]
    public partial class RecentRegistration
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private DateTime? _Dt1;

        private DateTime? _Dt2;

        private int? _Cnt;

        private int _OrganizationId;

        private string _OrganizationName;

        private bool? _Completed;

        public RecentRegistration()
        {
        }

        [Column(Name = "dt1", Storage = "_Dt1", DbType = "datetime")]
        public DateTime? Dt1
        {
            get => _Dt1;

            set
            {
                if (_Dt1 != value)
                {
                    _Dt1 = value;
                }
            }
        }

        [Column(Name = "dt2", Storage = "_Dt2", DbType = "datetime")]
        public DateTime? Dt2
        {
            get => _Dt2;

            set
            {
                if (_Dt2 != value)
                {
                    _Dt2 = value;
                }
            }
        }

        [Column(Name = "cnt", Storage = "_Cnt", DbType = "int")]
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

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int NOT NULL")]
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

        [Column(Name = "completed", Storage = "_Completed", DbType = "bit")]
        public bool? Completed
        {
            get => _Completed;

            set
            {
                if (_Completed != value)
                {
                    _Completed = value;
                }
            }
        }
    }
}
