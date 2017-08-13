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
	[Table(Name="PotentialDups")]
	public partial class PotentialDup
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId0;
		
		private int _PeopleId;
		
		private bool? _S0;
		
		private bool? _S1;
		
		private bool? _S2;
		
		private bool? _S3;
		
		private bool? _S4;
		
		private bool? _S5;
		
		private bool? _S6;
		
		private string _First;
		
		private string _Last;
		
		private string _Nick;
		
		private string _Middle;
		
		private string _Maiden;
		
		private int? _BMon;
		
		private int? _BDay;
		
		private int? _BYear;
		
		private string _Email;
		
		private string _FamAddr;
		
		private string _PerAddr;
		
		private string _Member;
		
		
		public PotentialDup()
		{
		}

		
		
		[Column(Name="PeopleId0", Storage="_PeopleId0", DbType="int NOT NULL")]
		public int PeopleId0
		{
			get
			{
				return this._PeopleId0;
			}

			set
			{
				if (this._PeopleId0 != value)
					this._PeopleId0 = value;
			}

		}

		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int NOT NULL")]
		public int PeopleId
		{
			get
			{
				return this._PeopleId;
			}

			set
			{
				if (this._PeopleId != value)
					this._PeopleId = value;
			}

		}

		
		[Column(Name="S0", Storage="_S0", DbType="bit")]
		public bool? S0
		{
			get
			{
				return this._S0;
			}

			set
			{
				if (this._S0 != value)
					this._S0 = value;
			}

		}

		
		[Column(Name="S1", Storage="_S1", DbType="bit")]
		public bool? S1
		{
			get
			{
				return this._S1;
			}

			set
			{
				if (this._S1 != value)
					this._S1 = value;
			}

		}

		
		[Column(Name="S2", Storage="_S2", DbType="bit")]
		public bool? S2
		{
			get
			{
				return this._S2;
			}

			set
			{
				if (this._S2 != value)
					this._S2 = value;
			}

		}

		
		[Column(Name="S3", Storage="_S3", DbType="bit")]
		public bool? S3
		{
			get
			{
				return this._S3;
			}

			set
			{
				if (this._S3 != value)
					this._S3 = value;
			}

		}

		
		[Column(Name="S4", Storage="_S4", DbType="bit")]
		public bool? S4
		{
			get
			{
				return this._S4;
			}

			set
			{
				if (this._S4 != value)
					this._S4 = value;
			}

		}

		
		[Column(Name="S5", Storage="_S5", DbType="bit")]
		public bool? S5
		{
			get
			{
				return this._S5;
			}

			set
			{
				if (this._S5 != value)
					this._S5 = value;
			}

		}

		
		[Column(Name="S6", Storage="_S6", DbType="bit")]
		public bool? S6
		{
			get
			{
				return this._S6;
			}

			set
			{
				if (this._S6 != value)
					this._S6 = value;
			}

		}

		
		[Column(Name="First", Storage="_First", DbType="nvarchar(25)")]
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

		
		[Column(Name="Last", Storage="_Last", DbType="nvarchar(100) NOT NULL")]
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

		
		[Column(Name="Nick", Storage="_Nick", DbType="nvarchar(25)")]
		public string Nick
		{
			get
			{
				return this._Nick;
			}

			set
			{
				if (this._Nick != value)
					this._Nick = value;
			}

		}

		
		[Column(Name="Middle", Storage="_Middle", DbType="nvarchar(30)")]
		public string Middle
		{
			get
			{
				return this._Middle;
			}

			set
			{
				if (this._Middle != value)
					this._Middle = value;
			}

		}

		
		[Column(Name="Maiden", Storage="_Maiden", DbType="nvarchar(20)")]
		public string Maiden
		{
			get
			{
				return this._Maiden;
			}

			set
			{
				if (this._Maiden != value)
					this._Maiden = value;
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

		
		[Column(Name="Email", Storage="_Email", DbType="nvarchar(150)")]
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

		
		[Column(Name="FamAddr", Storage="_FamAddr", DbType="nvarchar(100)")]
		public string FamAddr
		{
			get
			{
				return this._FamAddr;
			}

			set
			{
				if (this._FamAddr != value)
					this._FamAddr = value;
			}

		}

		
		[Column(Name="PerAddr", Storage="_PerAddr", DbType="nvarchar(100)")]
		public string PerAddr
		{
			get
			{
				return this._PerAddr;
			}

			set
			{
				if (this._PerAddr != value)
					this._PerAddr = value;
			}

		}

		
		[Column(Name="Member", Storage="_Member", DbType="nvarchar(50)")]
		public string Member
		{
			get
			{
				return this._Member;
			}

			set
			{
				if (this._Member != value)
					this._Member = value;
			}

		}

		
    }

}
