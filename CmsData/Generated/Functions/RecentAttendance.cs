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
	[Table(Name="RecentAttendance")]
	public partial class RecentAttendance
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Name;
		
		private DateTime _Lastattend;
		
		private decimal? _AttendPct;
		
		private string _AttendStr;
		
		private string _Attendtype;
		
		private int _Visitor;
		
		
		public RecentAttendance()
		{
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

		
		[Column(Name="NAME", Storage="_Name", DbType="nvarchar(138)")]
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

		
		[Column(Name="lastattend", Storage="_Lastattend", DbType="datetime NOT NULL")]
		public DateTime Lastattend
		{
			get
			{
				return this._Lastattend;
			}

			set
			{
				if (this._Lastattend != value)
					this._Lastattend = value;
			}

		}

		
		[Column(Name="AttendPct", Storage="_AttendPct", DbType="real")]
		public decimal? AttendPct
		{
			get
			{
				return this._AttendPct;
			}

			set
			{
				if (this._AttendPct != value)
					this._AttendPct = value;
			}

		}

		
		[Column(Name="AttendStr", Storage="_AttendStr", DbType="nvarchar(300)")]
		public string AttendStr
		{
			get
			{
				return this._AttendStr;
			}

			set
			{
				if (this._AttendStr != value)
					this._AttendStr = value;
			}

		}

		
		[Column(Name="attendtype", Storage="_Attendtype", DbType="nvarchar(100)")]
		public string Attendtype
		{
			get
			{
				return this._Attendtype;
			}

			set
			{
				if (this._Attendtype != value)
					this._Attendtype = value;
			}

		}

		
		[Column(Name="visitor", Storage="_Visitor", DbType="int NOT NULL")]
		public int Visitor
		{
			get
			{
				return this._Visitor;
			}

			set
			{
				if (this._Visitor != value)
					this._Visitor = value;
			}

		}

		
    }

}
