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
	[Table(Name="TPStats")]
	public partial class TPStat
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Nrecs;
		
		private int? _Nmembers;
		
		private int? _Olgive;
		
		private int? _Give;
		
		private int _Wag;
		
		private int? _Checkin;
		
		private DateTime _Lastdt;
		
		private string _Lastp;
		
		private int? _Nlogs;
		
		private int? _Nlogs30;
		
		private int? _Norgs;
		
		private DateTime? _Created;
		
		private DateTime? _Converted;
		
		private int? _Nusers;
		
		private int? _Nmydata;
		
		private int? _Nadmins;
		
		private int? _Reg;
		
		private DateTime? _Firstactive;
		
		private int? _Notam;
		
		private int? _Campuses;
		
		
		public TPStat()
		{
		}

		
		
		[Column(Name="nrecs", Storage="_Nrecs", DbType="int")]
		public int? Nrecs
		{
			get
			{
				return this._Nrecs;
			}

			set
			{
				if (this._Nrecs != value)
					this._Nrecs = value;
			}

		}

		
		[Column(Name="nmembers", Storage="_Nmembers", DbType="int")]
		public int? Nmembers
		{
			get
			{
				return this._Nmembers;
			}

			set
			{
				if (this._Nmembers != value)
					this._Nmembers = value;
			}

		}

		
		[Column(Name="olgive", Storage="_Olgive", DbType="int")]
		public int? Olgive
		{
			get
			{
				return this._Olgive;
			}

			set
			{
				if (this._Olgive != value)
					this._Olgive = value;
			}

		}

		
		[Column(Name="give", Storage="_Give", DbType="int")]
		public int? Give
		{
			get
			{
				return this._Give;
			}

			set
			{
				if (this._Give != value)
					this._Give = value;
			}

		}

		
		[Column(Name="wag", Storage="_Wag", DbType="int NOT NULL")]
		public int Wag
		{
			get
			{
				return this._Wag;
			}

			set
			{
				if (this._Wag != value)
					this._Wag = value;
			}

		}

		
		[Column(Name="checkin", Storage="_Checkin", DbType="int")]
		public int? Checkin
		{
			get
			{
				return this._Checkin;
			}

			set
			{
				if (this._Checkin != value)
					this._Checkin = value;
			}

		}

		
		[Column(Name="lastdt", Storage="_Lastdt", DbType="datetime NOT NULL")]
		public DateTime Lastdt
		{
			get
			{
				return this._Lastdt;
			}

			set
			{
				if (this._Lastdt != value)
					this._Lastdt = value;
			}

		}

		
		[Column(Name="lastp", Storage="_Lastp", DbType="nvarchar(50) NOT NULL")]
		public string Lastp
		{
			get
			{
				return this._Lastp;
			}

			set
			{
				if (this._Lastp != value)
					this._Lastp = value;
			}

		}

		
		[Column(Name="nlogs", Storage="_Nlogs", DbType="int")]
		public int? Nlogs
		{
			get
			{
				return this._Nlogs;
			}

			set
			{
				if (this._Nlogs != value)
					this._Nlogs = value;
			}

		}

		
		[Column(Name="nlogs30", Storage="_Nlogs30", DbType="int")]
		public int? Nlogs30
		{
			get
			{
				return this._Nlogs30;
			}

			set
			{
				if (this._Nlogs30 != value)
					this._Nlogs30 = value;
			}

		}

		
		[Column(Name="norgs", Storage="_Norgs", DbType="int")]
		public int? Norgs
		{
			get
			{
				return this._Norgs;
			}

			set
			{
				if (this._Norgs != value)
					this._Norgs = value;
			}

		}

		
		[Column(Name="created", Storage="_Created", DbType="datetime")]
		public DateTime? Created
		{
			get
			{
				return this._Created;
			}

			set
			{
				if (this._Created != value)
					this._Created = value;
			}

		}

		
		[Column(Name="converted", Storage="_Converted", DbType="datetime")]
		public DateTime? Converted
		{
			get
			{
				return this._Converted;
			}

			set
			{
				if (this._Converted != value)
					this._Converted = value;
			}

		}

		
		[Column(Name="nusers", Storage="_Nusers", DbType="int")]
		public int? Nusers
		{
			get
			{
				return this._Nusers;
			}

			set
			{
				if (this._Nusers != value)
					this._Nusers = value;
			}

		}

		
		[Column(Name="nmydata", Storage="_Nmydata", DbType="int")]
		public int? Nmydata
		{
			get
			{
				return this._Nmydata;
			}

			set
			{
				if (this._Nmydata != value)
					this._Nmydata = value;
			}

		}

		
		[Column(Name="nadmins", Storage="_Nadmins", DbType="int")]
		public int? Nadmins
		{
			get
			{
				return this._Nadmins;
			}

			set
			{
				if (this._Nadmins != value)
					this._Nadmins = value;
			}

		}

		
		[Column(Name="reg", Storage="_Reg", DbType="int")]
		public int? Reg
		{
			get
			{
				return this._Reg;
			}

			set
			{
				if (this._Reg != value)
					this._Reg = value;
			}

		}

		
		[Column(Name="firstactive", Storage="_Firstactive", DbType="datetime")]
		public DateTime? Firstactive
		{
			get
			{
				return this._Firstactive;
			}

			set
			{
				if (this._Firstactive != value)
					this._Firstactive = value;
			}

		}

		
		[Column(Name="notam", Storage="_Notam", DbType="int")]
		public int? Notam
		{
			get
			{
				return this._Notam;
			}

			set
			{
				if (this._Notam != value)
					this._Notam = value;
			}

		}

		
		[Column(Name="campuses", Storage="_Campuses", DbType="int")]
		public int? Campuses
		{
			get
			{
				return this._Campuses;
			}

			set
			{
				if (this._Campuses != value)
					this._Campuses = value;
			}

		}

		
    }

}
