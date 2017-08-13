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
	[Table(Name="OnlineRegMatches")]
	public partial class OnlineRegMatch
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _PeopleId;
		
		private string _First;
		
		private string _Last;
		
		private string _Nick;
		
		private string _Middle;
		
		private string _Maiden;
		
		private int? _BMon;
		
		private int? _BDay;
		
		private int? _BYear;
		
		private string _Email;
		
		private string _Member;
		
		
		public OnlineRegMatch()
		{
		}

		
		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int")]
		public int? PeopleId
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

		
		[Column(Name="First", Storage="_First", DbType="nvarchar(100)")]
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

		
		[Column(Name="Nick", Storage="_Nick", DbType="nvarchar(100)")]
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

		
		[Column(Name="Middle", Storage="_Middle", DbType="nvarchar(100)")]
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

		
		[Column(Name="Maiden", Storage="_Maiden", DbType="nvarchar(100)")]
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

		
		[Column(Name="Email", Storage="_Email", DbType="varchar(100)")]
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

		
		[Column(Name="Member", Storage="_Member", DbType="nvarchar(100)")]
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
