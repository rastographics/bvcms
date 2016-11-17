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
	[Table(Name="AccessUserInfo")]
	public partial class AccessUserInfo
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _PeopleId;
		
		private string _Roles;
		
		private DateTime? _Lastactive;
		
		private string _First;
		
		private string _Goesby;
		
		private string _Last;
		
		private int _Married;
		
		private int _Gender;
		
		private string _Cphone;
		
		private string _Hphone;
		
		private string _Wphone;
		
		private int? _Bday;
		
		private int? _Bmon;
		
		private int? _Byear;
		
		private string _Company;
		
		private string _Email;
		
		private string _Emali2;
		
		private string _Username;
		
		
		public AccessUserInfo()
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

		
		[Column(Name="roles", Storage="_Roles", DbType="nvarchar")]
		public string Roles
		{
			get
			{
				return this._Roles;
			}

			set
			{
				if (this._Roles != value)
					this._Roles = value;
			}

		}

		
		[Column(Name="lastactive", Storage="_Lastactive", DbType="datetime")]
		public DateTime? Lastactive
		{
			get
			{
				return this._Lastactive;
			}

			set
			{
				if (this._Lastactive != value)
					this._Lastactive = value;
			}

		}

		
		[Column(Name="first", Storage="_First", DbType="nvarchar(25)")]
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

		
		[Column(Name="goesby", Storage="_Goesby", DbType="nvarchar(25)")]
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

		
		[Column(Name="last", Storage="_Last", DbType="nvarchar(100) NOT NULL")]
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

		
		[Column(Name="married", Storage="_Married", DbType="int NOT NULL")]
		public int Married
		{
			get
			{
				return this._Married;
			}

			set
			{
				if (this._Married != value)
					this._Married = value;
			}

		}

		
		[Column(Name="gender", Storage="_Gender", DbType="int NOT NULL")]
		public int Gender
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

		
		[Column(Name="cphone", Storage="_Cphone", DbType="nvarchar(20)")]
		public string Cphone
		{
			get
			{
				return this._Cphone;
			}

			set
			{
				if (this._Cphone != value)
					this._Cphone = value;
			}

		}

		
		[Column(Name="hphone", Storage="_Hphone", DbType="nvarchar(20)")]
		public string Hphone
		{
			get
			{
				return this._Hphone;
			}

			set
			{
				if (this._Hphone != value)
					this._Hphone = value;
			}

		}

		
		[Column(Name="wphone", Storage="_Wphone", DbType="nvarchar(20)")]
		public string Wphone
		{
			get
			{
				return this._Wphone;
			}

			set
			{
				if (this._Wphone != value)
					this._Wphone = value;
			}

		}

		
		[Column(Name="bday", Storage="_Bday", DbType="int")]
		public int? Bday
		{
			get
			{
				return this._Bday;
			}

			set
			{
				if (this._Bday != value)
					this._Bday = value;
			}

		}

		
		[Column(Name="bmon", Storage="_Bmon", DbType="int")]
		public int? Bmon
		{
			get
			{
				return this._Bmon;
			}

			set
			{
				if (this._Bmon != value)
					this._Bmon = value;
			}

		}

		
		[Column(Name="byear", Storage="_Byear", DbType="int")]
		public int? Byear
		{
			get
			{
				return this._Byear;
			}

			set
			{
				if (this._Byear != value)
					this._Byear = value;
			}

		}

		
		[Column(Name="company", Storage="_Company", DbType="nvarchar(120)")]
		public string Company
		{
			get
			{
				return this._Company;
			}

			set
			{
				if (this._Company != value)
					this._Company = value;
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

		
		[Column(Name="emali2", Storage="_Emali2", DbType="nvarchar(60)")]
		public string Emali2
		{
			get
			{
				return this._Emali2;
			}

			set
			{
				if (this._Emali2 != value)
					this._Emali2 = value;
			}

		}

		
		[Column(Name="Username", Storage="_Username", DbType="nvarchar(50) NOT NULL")]
		public string Username
		{
			get
			{
				return this._Username;
			}

			set
			{
				if (this._Username != value)
					this._Username = value;
			}

		}

		
    }

}
