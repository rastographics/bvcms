using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "CheckinFamilyMembers")]
    public partial class CheckinFamilyMember
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Id;

        private int? _Position;

        private string _MemberVisitor;

        private string _Name;

        private string _First;

        private string _PreferredName;

        private string _AltName;

        private string _Last;

        private int? _BYear;

        private int? _BMon;

        private int? _BDay;

        private string _ClassX;

        private string _Leader;

        private int? _OrgId;

        private string _Location;

        private int? _Age;

        private string _Gender;

        private int? _NumLabels;

        private DateTime? _Hour;

        private bool? _CheckedIn;

        private string _Goesby;

        private string _Email;

        private string _Addr;

        private string _Zip;

        private string _Home;

        private string _Cell;

        private int? _Marital;

        private int? _Genderid;

        private int? _CampusId;

        private string _Allergies;

        private string _Emfriend;

        private string _Emphone;

        private bool? _Activeother;

        private string _Parent;

        private int? _Grade;

        private bool? _HasPicture;

        private bool? _Custody;

        private bool? _Transport;

        private bool? _RequiresSecurityLabel;

        private string _Church;

        public CheckinFamilyMember()
        {
        }

        [Column(Name = "Id", Storage = "_Id", DbType = "int")]
        public int? Id
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

        [Column(Name = "Position", Storage = "_Position", DbType = "int")]
        public int? Position
        {
            get => _Position;

            set
            {
                if (_Position != value)
                {
                    _Position = value;
                }
            }
        }

        [Column(Name = "MemberVisitor", Storage = "_MemberVisitor", DbType = "char(1)")]
        public string MemberVisitor
        {
            get => _MemberVisitor;

            set
            {
                if (_MemberVisitor != value)
                {
                    _MemberVisitor = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(150)")]
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

        [Column(Name = "PreferredName", Storage = "_PreferredName", DbType = "nvarchar(50)")]
        public string PreferredName
        {
            get => _PreferredName;

            set
            {
                if (_PreferredName != value)
                {
                    _PreferredName = value;
                }
            }
        }

        [Column(Name = "AltName", Storage = "_AltName", DbType = "nvarchar(50)")]
        public string AltName
        {
            get => _AltName;

            set
            {
                if (_AltName != value)
                {
                    _AltName = value;
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

        [Column(Name = "BYear", Storage = "_BYear", DbType = "int")]
        public int? BYear
        {
            get => _BYear;

            set
            {
                if (_BYear != value)
                {
                    _BYear = value;
                }
            }
        }

        [Column(Name = "BMon", Storage = "_BMon", DbType = "int")]
        public int? BMon
        {
            get => _BMon;

            set
            {
                if (_BMon != value)
                {
                    _BMon = value;
                }
            }
        }

        [Column(Name = "BDay", Storage = "_BDay", DbType = "int")]
        public int? BDay
        {
            get => _BDay;

            set
            {
                if (_BDay != value)
                {
                    _BDay = value;
                }
            }
        }

        [Column(Name = "Class", Storage = "_ClassX", DbType = "nvarchar(100)")]
        public string ClassX
        {
            get => _ClassX;

            set
            {
                if (_ClassX != value)
                {
                    _ClassX = value;
                }
            }
        }

        [Column(Name = "Leader", Storage = "_Leader", DbType = "nvarchar(100)")]
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

        [Column(Name = "Location", Storage = "_Location", DbType = "nvarchar(200)")]
        public string Location
        {
            get => _Location;

            set
            {
                if (_Location != value)
                {
                    _Location = value;
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

        [Column(Name = "Gender", Storage = "_Gender", DbType = "nvarchar(10)")]
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

        [Column(Name = "NumLabels", Storage = "_NumLabels", DbType = "int")]
        public int? NumLabels
        {
            get => _NumLabels;

            set
            {
                if (_NumLabels != value)
                {
                    _NumLabels = value;
                }
            }
        }

        [Column(Name = "hour", Storage = "_Hour", DbType = "datetime")]
        public DateTime? Hour
        {
            get => _Hour;

            set
            {
                if (_Hour != value)
                {
                    _Hour = value;
                }
            }
        }

        [Column(Name = "CheckedIn", Storage = "_CheckedIn", DbType = "bit")]
        public bool? CheckedIn
        {
            get => _CheckedIn;

            set
            {
                if (_CheckedIn != value)
                {
                    _CheckedIn = value;
                }
            }
        }

        [Column(Name = "goesby", Storage = "_Goesby", DbType = "nvarchar(50)")]
        public string Goesby
        {
            get => _Goesby;

            set
            {
                if (_Goesby != value)
                {
                    _Goesby = value;
                }
            }
        }

        [Column(Name = "email", Storage = "_Email", DbType = "nvarchar(150)")]
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

        [Column(Name = "addr", Storage = "_Addr", DbType = "nvarchar(100)")]
        public string Addr
        {
            get => _Addr;

            set
            {
                if (_Addr != value)
                {
                    _Addr = value;
                }
            }
        }

        [Column(Name = "zip", Storage = "_Zip", DbType = "nvarchar(15)")]
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

        [Column(Name = "home", Storage = "_Home", DbType = "nvarchar(20)")]
        public string Home
        {
            get => _Home;

            set
            {
                if (_Home != value)
                {
                    _Home = value;
                }
            }
        }

        [Column(Name = "cell", Storage = "_Cell", DbType = "nvarchar(20)")]
        public string Cell
        {
            get => _Cell;

            set
            {
                if (_Cell != value)
                {
                    _Cell = value;
                }
            }
        }

        [Column(Name = "marital", Storage = "_Marital", DbType = "int")]
        public int? Marital
        {
            get => _Marital;

            set
            {
                if (_Marital != value)
                {
                    _Marital = value;
                }
            }
        }

        [Column(Name = "genderid", Storage = "_Genderid", DbType = "int")]
        public int? Genderid
        {
            get => _Genderid;

            set
            {
                if (_Genderid != value)
                {
                    _Genderid = value;
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

        [Column(Name = "allergies", Storage = "_Allergies", DbType = "nvarchar(1000)")]
        public string Allergies
        {
            get => _Allergies;

            set
            {
                if (_Allergies != value)
                {
                    _Allergies = value;
                }
            }
        }

        [Column(Name = "emfriend", Storage = "_Emfriend", DbType = "nvarchar(100)")]
        public string Emfriend
        {
            get => _Emfriend;

            set
            {
                if (_Emfriend != value)
                {
                    _Emfriend = value;
                }
            }
        }

        [Column(Name = "emphone", Storage = "_Emphone", DbType = "nvarchar(100)")]
        public string Emphone
        {
            get => _Emphone;

            set
            {
                if (_Emphone != value)
                {
                    _Emphone = value;
                }
            }
        }

        [Column(Name = "activeother", Storage = "_Activeother", DbType = "bit")]
        public bool? Activeother
        {
            get => _Activeother;

            set
            {
                if (_Activeother != value)
                {
                    _Activeother = value;
                }
            }
        }

        [Column(Name = "parent", Storage = "_Parent", DbType = "nvarchar(100)")]
        public string Parent
        {
            get => _Parent;

            set
            {
                if (_Parent != value)
                {
                    _Parent = value;
                }
            }
        }

        [Column(Name = "grade", Storage = "_Grade", DbType = "int")]
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

        [Column(Name = "HasPicture", Storage = "_HasPicture", DbType = "bit")]
        public bool? HasPicture
        {
            get => _HasPicture;

            set
            {
                if (_HasPicture != value)
                {
                    _HasPicture = value;
                }
            }
        }

        [Column(Name = "Custody", Storage = "_Custody", DbType = "bit")]
        public bool? Custody
        {
            get => _Custody;

            set
            {
                if (_Custody != value)
                {
                    _Custody = value;
                }
            }
        }

        [Column(Name = "Transport", Storage = "_Transport", DbType = "bit")]
        public bool? Transport
        {
            get => _Transport;

            set
            {
                if (_Transport != value)
                {
                    _Transport = value;
                }
            }
        }

        [Column(Name = "RequiresSecurityLabel", Storage = "_RequiresSecurityLabel", DbType = "bit")]
        public bool? RequiresSecurityLabel
        {
            get => _RequiresSecurityLabel;

            set
            {
                if (_RequiresSecurityLabel != value)
                {
                    _RequiresSecurityLabel = value;
                }
            }
        }

        [Column(Name = "church", Storage = "_Church", DbType = "nvarchar(130)")]
        public string Church
        {
            get => _Church;

            set
            {
                if (_Church != value)
                {
                    _Church = value;
                }
            }
        }
    }
}
