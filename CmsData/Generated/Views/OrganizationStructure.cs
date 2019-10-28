using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrganizationStructure")]
    public partial class OrganizationStructure
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Program;

        private string _Division;

        private string _OrgStatus;

        private string _Organization;

        private int? _Members;

        private int? _Previous;

        private int? _Vistors;

        private int? _Meetings;

        private int _ProgId;

        private int _DivId;

        private int _OrgId;

        public OrganizationStructure()
        {
        }

        [Column(Name = "Program", Storage = "_Program", DbType = "nvarchar(50)")]
        public string Program
        {
            get => _Program;

            set
            {
                if (_Program != value)
                {
                    _Program = value;
                }
            }
        }

        [Column(Name = "Division", Storage = "_Division", DbType = "nvarchar(50)")]
        public string Division
        {
            get => _Division;

            set
            {
                if (_Division != value)
                {
                    _Division = value;
                }
            }
        }

        [Column(Name = "OrgStatus", Storage = "_OrgStatus", DbType = "nvarchar(50)")]
        public string OrgStatus
        {
            get => _OrgStatus;

            set
            {
                if (_OrgStatus != value)
                {
                    _OrgStatus = value;
                }
            }
        }

        [Column(Name = "Organization", Storage = "_Organization", DbType = "nvarchar(100) NOT NULL")]
        public string Organization
        {
            get => _Organization;

            set
            {
                if (_Organization != value)
                {
                    _Organization = value;
                }
            }
        }

        [Column(Name = "Members", Storage = "_Members", DbType = "int")]
        public int? Members
        {
            get => _Members;

            set
            {
                if (_Members != value)
                {
                    _Members = value;
                }
            }
        }

        [Column(Name = "Previous", Storage = "_Previous", DbType = "int")]
        public int? Previous
        {
            get => _Previous;

            set
            {
                if (_Previous != value)
                {
                    _Previous = value;
                }
            }
        }

        [Column(Name = "Vistors", Storage = "_Vistors", DbType = "int")]
        public int? Vistors
        {
            get => _Vistors;

            set
            {
                if (_Vistors != value)
                {
                    _Vistors = value;
                }
            }
        }

        [Column(Name = "Meetings", Storage = "_Meetings", DbType = "int")]
        public int? Meetings
        {
            get => _Meetings;

            set
            {
                if (_Meetings != value)
                {
                    _Meetings = value;
                }
            }
        }

        [Column(Name = "ProgId", Storage = "_ProgId", DbType = "int NOT NULL")]
        public int ProgId
        {
            get => _ProgId;

            set
            {
                if (_ProgId != value)
                {
                    _ProgId = value;
                }
            }
        }

        [Column(Name = "DivId", Storage = "_DivId", DbType = "int NOT NULL")]
        public int DivId
        {
            get => _DivId;

            set
            {
                if (_DivId != value)
                {
                    _DivId = value;
                }
            }
        }

        [Column(Name = "OrgId", Storage = "_OrgId", DbType = "int NOT NULL")]
        public int OrgId
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
    }
}
