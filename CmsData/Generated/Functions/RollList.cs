using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RollList")]
    public partial class RollList
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Section;

        private int? _PeopleId;

        private string _Name;

        private string _Last;

        private int? _FamilyId;

        private string _First;

        private string _Email;

        private bool? _Attended;

        private int? _CommitmentId;

        private string _CurrMemberType;

        private string _MemberType;

        private string _AttendType;

        private int? _OtherAttends;

        private bool? _CurrMember;

        private bool? _Conflict;

        private string _ChurchMemberStatus;

        public RollList()
        {
        }

        [Column(Name = "Section", Storage = "_Section", DbType = "int")]
        public int? Section
        {
            get => _Section;

            set
            {
                if (_Section != value)
                {
                    _Section = value;
                }
            }
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(100)")]
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

        [Column(Name = "Last", Storage = "_Last", DbType = "nvarchar(100)")]
        public string Last
        {
            get => _Last;

            set
            {
                if (_Last != value)
                {
                    _Last = value;
                }
            }
        }

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int")]
        public int? FamilyId
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

        [Column(Name = "First", Storage = "_First", DbType = "nvarchar(50)")]
        public string First
        {
            get => _First;

            set
            {
                if (_First != value)
                {
                    _First = value;
                }
            }
        }

        [Column(Name = "Email", Storage = "_Email", DbType = "nvarchar(100)")]
        public string Email
        {
            get => _Email;

            set
            {
                if (_Email != value)
                {
                    _Email = value;
                }
            }
        }

        [Column(Name = "Attended", Storage = "_Attended", DbType = "bit")]
        public bool? Attended
        {
            get => _Attended;

            set
            {
                if (_Attended != value)
                {
                    _Attended = value;
                }
            }
        }

        [Column(Name = "CommitmentId", Storage = "_CommitmentId", DbType = "int")]
        public int? CommitmentId
        {
            get => _CommitmentId;

            set
            {
                if (_CommitmentId != value)
                {
                    _CommitmentId = value;
                }
            }
        }

        [Column(Name = "CurrMemberType", Storage = "_CurrMemberType", DbType = "nvarchar(100)")]
        public string CurrMemberType
        {
            get => _CurrMemberType;

            set
            {
                if (_CurrMemberType != value)
                {
                    _CurrMemberType = value;
                }
            }
        }

        [Column(Name = "MemberType", Storage = "_MemberType", DbType = "nvarchar(100)")]
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

        [Column(Name = "AttendType", Storage = "_AttendType", DbType = "nvarchar(100)")]
        public string AttendType
        {
            get => _AttendType;

            set
            {
                if (_AttendType != value)
                {
                    _AttendType = value;
                }
            }
        }

        [Column(Name = "OtherAttends", Storage = "_OtherAttends", DbType = "int")]
        public int? OtherAttends
        {
            get => _OtherAttends;

            set
            {
                if (_OtherAttends != value)
                {
                    _OtherAttends = value;
                }
            }
        }

        [Column(Name = "CurrMember", Storage = "_CurrMember", DbType = "bit")]
        public bool? CurrMember
        {
            get => _CurrMember;

            set
            {
                if (_CurrMember != value)
                {
                    _CurrMember = value;
                }
            }
        }

        [Column(Name = "Conflict", Storage = "_Conflict", DbType = "bit")]
        public bool? Conflict
        {
            get => _Conflict;

            set
            {
                if (_Conflict != value)
                {
                    _Conflict = value;
                }
            }
        }

        [Column(Name = "ChurchMemberStatus", Storage = "_ChurchMemberStatus", DbType = "nvarchar(100)")]
        public string ChurchMemberStatus
        {
            get => _ChurchMemberStatus;

            set
            {
                if (_ChurchMemberStatus != value)
                {
                    _ChurchMemberStatus = value;
                }
            }
        }
    }
}
