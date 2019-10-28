using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ChAiIndividualData")]
    public partial class ChAiIndividualDatum
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private int _FamilyId;

        private DateTime? _CreatedDate;

        private int? _HeadOfHouseholdId;

        private DateTime? _FamilyCreatedDate;

        private string _MemberStatus;

        private string _Salutation;

        private string _FirstName;

        private string _LastName;

        private string _FamilyPosition;

        private string _MaritalStatus;

        private string _Gender;

        private string _Address1;

        private string _Address2;

        private string _City;

        private string _State;

        private string _Zip;

        private string _HomePhone;

        private string _EmailAddress;

        private string _BirthDate;

        private DateTime? _LastModified;

        public ChAiIndividualDatum()
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

        [Column(Name = "HeadOfHouseholdId", Storage = "_HeadOfHouseholdId", DbType = "int")]
        public int? HeadOfHouseholdId
        {
            get => _HeadOfHouseholdId;

            set
            {
                if (_HeadOfHouseholdId != value)
                {
                    _HeadOfHouseholdId = value;
                }
            }
        }

        [Column(Name = "FamilyCreatedDate", Storage = "_FamilyCreatedDate", DbType = "datetime")]
        public DateTime? FamilyCreatedDate
        {
            get => _FamilyCreatedDate;

            set
            {
                if (_FamilyCreatedDate != value)
                {
                    _FamilyCreatedDate = value;
                }
            }
        }

        [Column(Name = "MemberStatus", Storage = "_MemberStatus", DbType = "nvarchar(50)")]
        public string MemberStatus
        {
            get => _MemberStatus;

            set
            {
                if (_MemberStatus != value)
                {
                    _MemberStatus = value;
                }
            }
        }

        [Column(Name = "Salutation", Storage = "_Salutation", DbType = "nvarchar(10)")]
        public string Salutation
        {
            get => _Salutation;

            set
            {
                if (_Salutation != value)
                {
                    _Salutation = value;
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

        [Column(Name = "FamilyPosition", Storage = "_FamilyPosition", DbType = "nvarchar(100)")]
        public string FamilyPosition
        {
            get => _FamilyPosition;

            set
            {
                if (_FamilyPosition != value)
                {
                    _FamilyPosition = value;
                }
            }
        }

        [Column(Name = "MaritalStatus", Storage = "_MaritalStatus", DbType = "nvarchar(100)")]
        public string MaritalStatus
        {
            get => _MaritalStatus;

            set
            {
                if (_MaritalStatus != value)
                {
                    _MaritalStatus = value;
                }
            }
        }

        [Column(Name = "Gender", Storage = "_Gender", DbType = "nvarchar(100)")]
        public string Gender
        {
            get => _Gender;

            set
            {
                if (_Gender != value)
                {
                    _Gender = value;
                }
            }
        }

        [Column(Name = "Address1", Storage = "_Address1", DbType = "nvarchar(100)")]
        public string Address1
        {
            get => _Address1;

            set
            {
                if (_Address1 != value)
                {
                    _Address1 = value;
                }
            }
        }

        [Column(Name = "Address2", Storage = "_Address2", DbType = "nvarchar(100)")]
        public string Address2
        {
            get => _Address2;

            set
            {
                if (_Address2 != value)
                {
                    _Address2 = value;
                }
            }
        }

        [Column(Name = "City", Storage = "_City", DbType = "nvarchar(30)")]
        public string City
        {
            get => _City;

            set
            {
                if (_City != value)
                {
                    _City = value;
                }
            }
        }

        [Column(Name = "State", Storage = "_State", DbType = "nvarchar(20)")]
        public string State
        {
            get => _State;

            set
            {
                if (_State != value)
                {
                    _State = value;
                }
            }
        }

        [Column(Name = "Zip", Storage = "_Zip", DbType = "nvarchar(5)")]
        public string Zip
        {
            get => _Zip;

            set
            {
                if (_Zip != value)
                {
                    _Zip = value;
                }
            }
        }

        [Column(Name = "HomePhone", Storage = "_HomePhone", DbType = "nvarchar(32)")]
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

        [Column(Name = "BirthDate", Storage = "_BirthDate", DbType = "varchar(20)")]
        public string BirthDate
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

        [Column(Name = "LastModified", Storage = "_LastModified", DbType = "datetime")]
        public DateTime? LastModified
        {
            get => _LastModified;

            set
            {
                if (_LastModified != value)
                {
                    _LastModified = value;
                }
            }
        }
    }
}
