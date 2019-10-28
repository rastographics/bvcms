using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PeopleBasicModifed")]
    public partial class PeopleBasicModifed
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private int _FamilyId;

        private int _PositionInFamilyId;

        private int? _CampusId;

        private string _FirstName;

        private string _LastName;

        private int _GenderId;

        private DateTime? _BirthDate;

        private string _EmailAddress;

        private int _MaritalStatusId;

        private string _PrimaryAddress;

        private string _PrimaryCity;

        private string _PrimaryState;

        private string _PrimaryZip;

        private string _HomePhone;

        private string _WorkPhone;

        private string _CellPhone;

        private DateTime? _CreatedDate;

        private DateTime? _ModifiedDate;

        private bool? _IsDeceased;

        public PeopleBasicModifed()
        {
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
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

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int NOT NULL")]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    _FamilyId = value;
                }
            }
        }

        [Column(Name = "PositionInFamilyId", Storage = "_PositionInFamilyId", DbType = "int NOT NULL")]
        public int PositionInFamilyId
        {
            get => _PositionInFamilyId;

            set
            {
                if (_PositionInFamilyId != value)
                {
                    _PositionInFamilyId = value;
                }
            }
        }

        [Column(Name = "CampusId", Storage = "_CampusId", DbType = "int")]
        public int? CampusId
        {
            get => _CampusId;

            set
            {
                if (_CampusId != value)
                {
                    _CampusId = value;
                }
            }
        }

        [Column(Name = "FirstName", Storage = "_FirstName", DbType = "nvarchar(25)")]
        public string FirstName
        {
            get => _FirstName;

            set
            {
                if (_FirstName != value)
                {
                    _FirstName = value;
                }
            }
        }

        [Column(Name = "LastName", Storage = "_LastName", DbType = "nvarchar(100) NOT NULL")]
        public string LastName
        {
            get => _LastName;

            set
            {
                if (_LastName != value)
                {
                    _LastName = value;
                }
            }
        }

        [Column(Name = "GenderId", Storage = "_GenderId", DbType = "int NOT NULL")]
        public int GenderId
        {
            get => _GenderId;

            set
            {
                if (_GenderId != value)
                {
                    _GenderId = value;
                }
            }
        }

        [Column(Name = "BirthDate", Storage = "_BirthDate", DbType = "datetime")]
        public DateTime? BirthDate
        {
            get => _BirthDate;

            set
            {
                if (_BirthDate != value)
                {
                    _BirthDate = value;
                }
            }
        }

        [Column(Name = "EmailAddress", Storage = "_EmailAddress", DbType = "nvarchar(150)")]
        public string EmailAddress
        {
            get => _EmailAddress;

            set
            {
                if (_EmailAddress != value)
                {
                    _EmailAddress = value;
                }
            }
        }

        [Column(Name = "MaritalStatusId", Storage = "_MaritalStatusId", DbType = "int NOT NULL")]
        public int MaritalStatusId
        {
            get => _MaritalStatusId;

            set
            {
                if (_MaritalStatusId != value)
                {
                    _MaritalStatusId = value;
                }
            }
        }

        [Column(Name = "PrimaryAddress", Storage = "_PrimaryAddress", DbType = "nvarchar(100)")]
        public string PrimaryAddress
        {
            get => _PrimaryAddress;

            set
            {
                if (_PrimaryAddress != value)
                {
                    _PrimaryAddress = value;
                }
            }
        }

        [Column(Name = "PrimaryCity", Storage = "_PrimaryCity", DbType = "nvarchar(30)")]
        public string PrimaryCity
        {
            get => _PrimaryCity;

            set
            {
                if (_PrimaryCity != value)
                {
                    _PrimaryCity = value;
                }
            }
        }

        [Column(Name = "PrimaryState", Storage = "_PrimaryState", DbType = "nvarchar(20)")]
        public string PrimaryState
        {
            get => _PrimaryState;

            set
            {
                if (_PrimaryState != value)
                {
                    _PrimaryState = value;
                }
            }
        }

        [Column(Name = "PrimaryZip", Storage = "_PrimaryZip", DbType = "nvarchar(15)")]
        public string PrimaryZip
        {
            get => _PrimaryZip;

            set
            {
                if (_PrimaryZip != value)
                {
                    _PrimaryZip = value;
                }
            }
        }

        [Column(Name = "HomePhone", Storage = "_HomePhone", DbType = "nvarchar(20)")]
        public string HomePhone
        {
            get => _HomePhone;

            set
            {
                if (_HomePhone != value)
                {
                    _HomePhone = value;
                }
            }
        }

        [Column(Name = "WorkPhone", Storage = "_WorkPhone", DbType = "nvarchar(20)")]
        public string WorkPhone
        {
            get => _WorkPhone;

            set
            {
                if (_WorkPhone != value)
                {
                    _WorkPhone = value;
                }
            }
        }

        [Column(Name = "CellPhone", Storage = "_CellPhone", DbType = "nvarchar(20)")]
        public string CellPhone
        {
            get => _CellPhone;

            set
            {
                if (_CellPhone != value)
                {
                    _CellPhone = value;
                }
            }
        }

        [Column(Name = "CreatedDate", Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime? CreatedDate
        {
            get => _CreatedDate;

            set
            {
                if (_CreatedDate != value)
                {
                    _CreatedDate = value;
                }
            }
        }

        [Column(Name = "ModifiedDate", Storage = "_ModifiedDate", DbType = "datetime")]
        public DateTime? ModifiedDate
        {
            get => _ModifiedDate;

            set
            {
                if (_ModifiedDate != value)
                {
                    _ModifiedDate = value;
                }
            }
        }

        [Column(Name = "IsDeceased", Storage = "_IsDeceased", DbType = "bit")]
        public bool? IsDeceased
        {
            get => _IsDeceased;

            set
            {
                if (_IsDeceased != value)
                {
                    _IsDeceased = value;
                }
            }
        }
    }
}
