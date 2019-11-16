using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "FamilyMembers")]
    public partial class FamilyMember
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Id;

        private int _PortraitId;

        private DateTime? _PicDate;

        private int? _PicXPos;

        private int? _PicYPos;

        private DateTime? _DeceasedDate;

        private string _Name;

        private int? _Age;

        private int _GenderId;

        private string _Color;

        private int _PositionInFamilyId;

        private string _PositionInFamily;

        private string _SpouseIndicator;

        private string _Email;

        private bool? _IsDeceased;

        private string _MemberStatus;

        private string _Gender;

        public FamilyMember()
        {
        }

        [Column(Name = "Id", Storage = "_Id", DbType = "int NOT NULL")]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    _Id = value;
                }
            }
        }

        [Column(Name = "PortraitId", Storage = "_PortraitId", DbType = "int NOT NULL")]
        public int PortraitId
        {
            get => _PortraitId;

            set
            {
                if (_PortraitId != value)
                {
                    _PortraitId = value;
                }
            }
        }

        [Column(Name = "PicDate", Storage = "_PicDate", DbType = "datetime")]
        public DateTime? PicDate
        {
            get => _PicDate;

            set
            {
                if (_PicDate != value)
                {
                    _PicDate = value;
                }
            }
        }

        [Column(Name = "PicXPos", Storage = "_PicXPos", DbType = "int")]
        public int? PicXPos
        {
            get => _PicXPos;

            set
            {
                if (_PicXPos != value)
                {
                    _PicXPos = value;
                }
            }
        }

        [Column(Name = "PicYPos", Storage = "_PicYPos", DbType = "int")]
        public int? PicYPos
        {
            get => _PicYPos;

            set
            {
                if (_PicYPos != value)
                {
                    _PicYPos = value;
                }
            }
        }

        [Column(Name = "DeceasedDate", Storage = "_DeceasedDate", DbType = "datetime")]
        public DateTime? DeceasedDate
        {
            get => _DeceasedDate;

            set
            {
                if (_DeceasedDate != value)
                {
                    _DeceasedDate = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(138)")]
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

        [Column(Name = "Color", Storage = "_Color", DbType = "varchar(1) NOT NULL")]
        public string Color
        {
            get => _Color;

            set
            {
                if (_Color != value)
                {
                    _Color = value;
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

        [Column(Name = "PositionInFamily", Storage = "_PositionInFamily", DbType = "nvarchar(100)")]
        public string PositionInFamily
        {
            get => _PositionInFamily;

            set
            {
                if (_PositionInFamily != value)
                {
                    _PositionInFamily = value;
                }
            }
        }

        [Column(Name = "SpouseIndicator", Storage = "_SpouseIndicator", DbType = "varchar(1) NOT NULL")]
        public string SpouseIndicator
        {
            get => _SpouseIndicator;

            set
            {
                if (_SpouseIndicator != value)
                {
                    _SpouseIndicator = value;
                }
            }
        }

        [Column(Name = "Email", Storage = "_Email", DbType = "nvarchar(150)")]
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

        [Column(Name = "isDeceased", Storage = "_IsDeceased", DbType = "bit")]
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

        [Column(Name = "Gender", Storage = "_Gender", DbType = "nvarchar(20)")]
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
    }
}
