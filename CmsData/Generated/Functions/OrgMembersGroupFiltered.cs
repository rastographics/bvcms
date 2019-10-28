using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgMembersGroupFiltered")]
    public partial class OrgMembersGroupFiltered
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _PeopleId;

        private int? _OrganizationId;

        private int? _Age;

        private int? _Grade;

        private int? _MemberTypeId;

        private string _MemberType;

        private int? _BirthYear;

        private int? _BirthMonth;

        private int? _BirthDay;

        private string _OrganizationName;

        private string _Name2;

        private string _Name;

        private string _Gender;

        private int? _HashNum;

        private string _Request;

        private string _Groups;

        public OrgMembersGroupFiltered()
        {
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
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

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int")]
        public int? OrganizationId
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

        [Column(Name = "Age", Storage = "_Age", DbType = "int")]
        public int? Age
        {
            get => _Age;

            set
            {
                if (_Age != value)
                {
                    _Age = value;
                }
            }
        }

        [Column(Name = "Grade", Storage = "_Grade", DbType = "int")]
        public int? Grade
        {
            get => _Grade;

            set
            {
                if (_Grade != value)
                {
                    _Grade = value;
                }
            }
        }

        [Column(Name = "MemberTypeId", Storage = "_MemberTypeId", DbType = "int")]
        public int? MemberTypeId
        {
            get => _MemberTypeId;

            set
            {
                if (_MemberTypeId != value)
                {
                    _MemberTypeId = value;
                }
            }
        }

        [Column(Name = "MemberType", Storage = "_MemberType", DbType = "varchar(100)")]
        public string MemberType
        {
            get => _MemberType;

            set
            {
                if (_MemberType != value)
                {
                    _MemberType = value;
                }
            }
        }

        [Column(Name = "BirthYear", Storage = "_BirthYear", DbType = "int")]
        public int? BirthYear
        {
            get => _BirthYear;

            set
            {
                if (_BirthYear != value)
                {
                    _BirthYear = value;
                }
            }
        }

        [Column(Name = "BirthMonth", Storage = "_BirthMonth", DbType = "int")]
        public int? BirthMonth
        {
            get => _BirthMonth;

            set
            {
                if (_BirthMonth != value)
                {
                    _BirthMonth = value;
                }
            }
        }

        [Column(Name = "BirthDay", Storage = "_BirthDay", DbType = "int")]
        public int? BirthDay
        {
            get => _BirthDay;

            set
            {
                if (_BirthDay != value)
                {
                    _BirthDay = value;
                }
            }
        }

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "nvarchar(100)")]
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

        [Column(Name = "Name2", Storage = "_Name2", DbType = "nvarchar(200)")]
        public string Name2
        {
            get => _Name2;

            set
            {
                if (_Name2 != value)
                {
                    _Name2 = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(200)")]
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

        [Column(Name = "Gender", Storage = "_Gender", DbType = "varchar(10)")]
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

        [Column(Name = "HashNum", Storage = "_HashNum", DbType = "int")]
        public int? HashNum
        {
            get => _HashNum;

            set
            {
                if (_HashNum != value)
                {
                    _HashNum = value;
                }
            }
        }

        [Column(Name = "Request", Storage = "_Request", DbType = "nvarchar(140)")]
        public string Request
        {
            get => _Request;

            set
            {
                if (_Request != value)
                {
                    _Request = value;
                }
            }
        }

        [Column(Name = "Groups", Storage = "_Groups", DbType = "nvarchar")]
        public string Groups
        {
            get => _Groups;

            set
            {
                if (_Groups != value)
                {
                    _Groups = value;
                }
            }
        }
    }
}
