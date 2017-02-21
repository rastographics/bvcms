using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData.View
{
	[Table(Name="CheckinFamilyMembers")]
	public partial class CheckinFamilyMember
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
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

		
		
		[Column(Name="Id", Storage="_Id", DbType="int")]
		public int? Id
		{
			get
			{
				return this._Id;
			}

			set
			{
				if (this._Id != value)
					this._Id = value;
			}

		}

		
		[Column(Name="Position", Storage="_Position", DbType="int")]
		public int? Position
		{
			get
			{
				return this._Position;
			}

			set
			{
				if (this._Position != value)
					this._Position = value;
			}

		}

		
		[Column(Name="MemberVisitor", Storage="_MemberVisitor", DbType="char(1)")]
		public string MemberVisitor
		{
			get
			{
				return this._MemberVisitor;
			}

			set
			{
				if (this._MemberVisitor != value)
					this._MemberVisitor = value;
			}

		}

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(150)")]
		public string Name
		{
			get
			{
				return this._Name;
			}

			set
			{
				if (this._Name != value)
					this._Name = value;
			}

		}

		
		[Column(Name="First", Storage="_First", DbType="nvarchar(50)")]
		public string First
		{
			get
			{
				return this._First;
			}

			set
			{
				if (this._First != value)
					this._First = value;
			}

		}

		
		[Column(Name="PreferredName", Storage="_PreferredName", DbType="nvarchar(50)")]
		public string PreferredName
		{
			get
			{
				return this._PreferredName;
			}

			set
			{
				if (this._PreferredName != value)
					this._PreferredName = value;
			}

		}

		
		[Column(Name="AltName", Storage="_AltName", DbType="nvarchar(50)")]
		public string AltName
		{
			get
			{
				return this._AltName;
			}

			set
			{
				if (this._AltName != value)
					this._AltName = value;
			}

		}

		
		[Column(Name="Last", Storage="_Last", DbType="nvarchar(100)")]
		public string Last
		{
			get
			{
				return this._Last;
			}

			set
			{
				if (this._Last != value)
					this._Last = value;
			}

		}

		
		[Column(Name="BYear", Storage="_BYear", DbType="int")]
		public int? BYear
		{
			get
			{
				return this._BYear;
			}

			set
			{
				if (this._BYear != value)
					this._BYear = value;
			}

		}

		
		[Column(Name="BMon", Storage="_BMon", DbType="int")]
		public int? BMon
		{
			get
			{
				return this._BMon;
			}

			set
			{
				if (this._BMon != value)
					this._BMon = value;
			}

		}

		
		[Column(Name="BDay", Storage="_BDay", DbType="int")]
		public int? BDay
		{
			get
			{
				return this._BDay;
			}

			set
			{
				if (this._BDay != value)
					this._BDay = value;
			}

		}

		
		[Column(Name="Class", Storage="_ClassX", DbType="nvarchar(100)")]
		public string ClassX
		{
			get
			{
				return this._ClassX;
			}

			set
			{
				if (this._ClassX != value)
					this._ClassX = value;
			}

		}

		
		[Column(Name="Leader", Storage="_Leader", DbType="nvarchar(100)")]
		public string Leader
		{
			get
			{
				return this._Leader;
			}

			set
			{
				if (this._Leader != value)
					this._Leader = value;
			}

		}

		
		[Column(Name="OrgId", Storage="_OrgId", DbType="int")]
		public int? OrgId
		{
			get
			{
				return this._OrgId;
			}

			set
			{
				if (this._OrgId != value)
					this._OrgId = value;
			}

		}

		
		[Column(Name="Location", Storage="_Location", DbType="nvarchar(200)")]
		public string Location
		{
			get
			{
				return this._Location;
			}

			set
			{
				if (this._Location != value)
					this._Location = value;
			}

		}

		
		[Column(Name="Age", Storage="_Age", DbType="int")]
		public int? Age
		{
			get
			{
				return this._Age;
			}

			set
			{
				if (this._Age != value)
					this._Age = value;
			}

		}

		
		[Column(Name="Gender", Storage="_Gender", DbType="nvarchar(10)")]
		public string Gender
		{
			get
			{
				return this._Gender;
			}

			set
			{
				if (this._Gender != value)
					this._Gender = value;
			}

		}

		
		[Column(Name="NumLabels", Storage="_NumLabels", DbType="int")]
		public int? NumLabels
		{
			get
			{
				return this._NumLabels;
			}

			set
			{
				if (this._NumLabels != value)
					this._NumLabels = value;
			}

		}

		
		[Column(Name="hour", Storage="_Hour", DbType="datetime")]
		public DateTime? Hour
		{
			get
			{
				return this._Hour;
			}

			set
			{
				if (this._Hour != value)
					this._Hour = value;
			}

		}

		
		[Column(Name="CheckedIn", Storage="_CheckedIn", DbType="bit")]
		public bool? CheckedIn
		{
			get
			{
				return this._CheckedIn;
			}

			set
			{
				if (this._CheckedIn != value)
					this._CheckedIn = value;
			}

		}

		
		[Column(Name="goesby", Storage="_Goesby", DbType="nvarchar(50)")]
		public string Goesby
		{
			get
			{
				return this._Goesby;
			}

			set
			{
				if (this._Goesby != value)
					this._Goesby = value;
			}

		}

		
		[Column(Name="email", Storage="_Email", DbType="nvarchar(150)")]
		public string Email
		{
			get
			{
				return this._Email;
			}

			set
			{
				if (this._Email != value)
					this._Email = value;
			}

		}

		
		[Column(Name="addr", Storage="_Addr", DbType="nvarchar(100)")]
		public string Addr
		{
			get
			{
				return this._Addr;
			}

			set
			{
				if (this._Addr != value)
					this._Addr = value;
			}

		}

		
		[Column(Name="zip", Storage="_Zip", DbType="nvarchar(15)")]
		public string Zip
		{
			get
			{
				return this._Zip;
			}

			set
			{
				if (this._Zip != value)
					this._Zip = value;
			}

		}

		
		[Column(Name="home", Storage="_Home", DbType="nvarchar(20)")]
		public string Home
		{
			get
			{
				return this._Home;
			}

			set
			{
				if (this._Home != value)
					this._Home = value;
			}

		}

		
		[Column(Name="cell", Storage="_Cell", DbType="nvarchar(20)")]
		public string Cell
		{
			get
			{
				return this._Cell;
			}

			set
			{
				if (this._Cell != value)
					this._Cell = value;
			}

		}

		
		[Column(Name="marital", Storage="_Marital", DbType="int")]
		public int? Marital
		{
			get
			{
				return this._Marital;
			}

			set
			{
				if (this._Marital != value)
					this._Marital = value;
			}

		}

		
		[Column(Name="genderid", Storage="_Genderid", DbType="int")]
		public int? Genderid
		{
			get
			{
				return this._Genderid;
			}

			set
			{
				if (this._Genderid != value)
					this._Genderid = value;
			}

		}

		
		[Column(Name="CampusId", Storage="_CampusId", DbType="int")]
		public int? CampusId
		{
			get
			{
				return this._CampusId;
			}

			set
			{
				if (this._CampusId != value)
					this._CampusId = value;
			}

		}

		
		[Column(Name="allergies", Storage="_Allergies", DbType="nvarchar(1000)")]
		public string Allergies
		{
			get
			{
				return this._Allergies;
			}

			set
			{
				if (this._Allergies != value)
					this._Allergies = value;
			}

		}

		
		[Column(Name="emfriend", Storage="_Emfriend", DbType="nvarchar(100)")]
		public string Emfriend
		{
			get
			{
				return this._Emfriend;
			}

			set
			{
				if (this._Emfriend != value)
					this._Emfriend = value;
			}

		}

		
		[Column(Name="emphone", Storage="_Emphone", DbType="nvarchar(100)")]
		public string Emphone
		{
			get
			{
				return this._Emphone;
			}

			set
			{
				if (this._Emphone != value)
					this._Emphone = value;
			}

		}

		
		[Column(Name="activeother", Storage="_Activeother", DbType="bit")]
		public bool? Activeother
		{
			get
			{
				return this._Activeother;
			}

			set
			{
				if (this._Activeother != value)
					this._Activeother = value;
			}

		}

		
		[Column(Name="parent", Storage="_Parent", DbType="nvarchar(100)")]
		public string Parent
		{
			get
			{
				return this._Parent;
			}

			set
			{
				if (this._Parent != value)
					this._Parent = value;
			}

		}

		
		[Column(Name="grade", Storage="_Grade", DbType="int")]
		public int? Grade
		{
			get
			{
				return this._Grade;
			}

			set
			{
				if (this._Grade != value)
					this._Grade = value;
			}

		}

		
		[Column(Name="HasPicture", Storage="_HasPicture", DbType="bit")]
		public bool? HasPicture
		{
			get
			{
				return this._HasPicture;
			}

			set
			{
				if (this._HasPicture != value)
					this._HasPicture = value;
			}

		}

		
		[Column(Name="Custody", Storage="_Custody", DbType="bit")]
		public bool? Custody
		{
			get
			{
				return this._Custody;
			}

			set
			{
				if (this._Custody != value)
					this._Custody = value;
			}

		}

		
		[Column(Name="Transport", Storage="_Transport", DbType="bit")]
		public bool? Transport
		{
			get
			{
				return this._Transport;
			}

			set
			{
				if (this._Transport != value)
					this._Transport = value;
			}

		}

		
		[Column(Name="RequiresSecurityLabel", Storage="_RequiresSecurityLabel", DbType="bit")]
		public bool? RequiresSecurityLabel
		{
			get
			{
				return this._RequiresSecurityLabel;
			}

			set
			{
				if (this._RequiresSecurityLabel != value)
					this._RequiresSecurityLabel = value;
			}

		}

		
		[Column(Name="church", Storage="_Church", DbType="nvarchar(130)")]
		public string Church
		{
			get
			{
				return this._Church;
			}

			set
			{
				if (this._Church != value)
					this._Church = value;
			}

		}

		
    }

}
