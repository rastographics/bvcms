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
	[Table(Name="ManagedGivingList")]
	public partial class ManagedGivingList
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _Name2;
		
		private int _PeopleId;
		
		private DateTime? _StartWhen;
		
		private DateTime? _NextDate;
		
		private string _SemiEvery;
		
		private int? _Day1;
		
		private int? _Day2;
		
		private int? _EveryN;
		
		private string _Period;
		
		private DateTime? _StopWhen;
		
		private int? _StopAfter;
		
		private string _Type;
		
		private decimal? _ActiveAmt;
		
		private decimal? _InactiveAmt;
		
		
		public ManagedGivingList()
		{
		}

		
		
		[Column(Name="Name2", Storage="_Name2", DbType="nvarchar(139)")]
		public string Name2
		{
			get
			{
				return this._Name2;
			}

			set
			{
				if (this._Name2 != value)
					this._Name2 = value;
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

		
		[Column(Name="StartWhen", Storage="_StartWhen", DbType="datetime")]
		public DateTime? StartWhen
		{
			get
			{
				return this._StartWhen;
			}

			set
			{
				if (this._StartWhen != value)
					this._StartWhen = value;
			}

		}

		
		[Column(Name="NextDate", Storage="_NextDate", DbType="datetime")]
		public DateTime? NextDate
		{
			get
			{
				return this._NextDate;
			}

			set
			{
				if (this._NextDate != value)
					this._NextDate = value;
			}

		}

		
		[Column(Name="SemiEvery", Storage="_SemiEvery", DbType="nvarchar(2)")]
		public string SemiEvery
		{
			get
			{
				return this._SemiEvery;
			}

			set
			{
				if (this._SemiEvery != value)
					this._SemiEvery = value;
			}

		}

		
		[Column(Name="Day1", Storage="_Day1", DbType="int")]
		public int? Day1
		{
			get
			{
				return this._Day1;
			}

			set
			{
				if (this._Day1 != value)
					this._Day1 = value;
			}

		}

		
		[Column(Name="Day2", Storage="_Day2", DbType="int")]
		public int? Day2
		{
			get
			{
				return this._Day2;
			}

			set
			{
				if (this._Day2 != value)
					this._Day2 = value;
			}

		}

		
		[Column(Name="EveryN", Storage="_EveryN", DbType="int")]
		public int? EveryN
		{
			get
			{
				return this._EveryN;
			}

			set
			{
				if (this._EveryN != value)
					this._EveryN = value;
			}

		}

		
		[Column(Name="Period", Storage="_Period", DbType="nvarchar(2)")]
		public string Period
		{
			get
			{
				return this._Period;
			}

			set
			{
				if (this._Period != value)
					this._Period = value;
			}

		}

		
		[Column(Name="StopWhen", Storage="_StopWhen", DbType="datetime")]
		public DateTime? StopWhen
		{
			get
			{
				return this._StopWhen;
			}

			set
			{
				if (this._StopWhen != value)
					this._StopWhen = value;
			}

		}

		
		[Column(Name="StopAfter", Storage="_StopAfter", DbType="int")]
		public int? StopAfter
		{
			get
			{
				return this._StopAfter;
			}

			set
			{
				if (this._StopAfter != value)
					this._StopAfter = value;
			}

		}

		
		[Column(Name="type", Storage="_Type", DbType="nvarchar(2)")]
		public string Type
		{
			get
			{
				return this._Type;
			}

			set
			{
				if (this._Type != value)
					this._Type = value;
			}

		}

		
		[Column(Name="ActiveAmt", Storage="_ActiveAmt", DbType="money")]
		public decimal? ActiveAmt
		{
			get
			{
				return this._ActiveAmt;
			}

			set
			{
				if (this._ActiveAmt != value)
					this._ActiveAmt = value;
			}

		}

		
		[Column(Name="InactiveAmt", Storage="_InactiveAmt", DbType="money")]
		public decimal? InactiveAmt
		{
			get
			{
				return this._InactiveAmt;
			}

			set
			{
				if (this._InactiveAmt != value)
					this._InactiveAmt = value;
			}

		}

		
    }

}
