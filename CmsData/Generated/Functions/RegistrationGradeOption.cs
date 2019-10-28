using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RegistrationGradeOptions")]
    public partial class RegistrationGradeOption
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrganizationName;

        private string _GradeOption;

        private string _GradeCode;

        private int? _Cnt;

        public RegistrationGradeOption()
        {
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

        [Column(Name = "GradeOption", Storage = "_GradeOption", DbType = "varchar(100)")]
        public string GradeOption
        {
            get => _GradeOption;

            set
            {
                if (_GradeOption != value)
                {
                    _GradeOption = value;
                }
            }
        }

        [Column(Name = "GradeCode", Storage = "_GradeCode", DbType = "varchar(100)")]
        public string GradeCode
        {
            get => _GradeCode;

            set
            {
                if (_GradeCode != value)
                {
                    _GradeCode = value;
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
    }
}
