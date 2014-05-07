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
	[Table(Name="MemberData")]
	public partial class MemberDatum
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _First;
		
		private string _Last;
		
		private int? _Age;
		
		private string _Marital;
		
		private DateTime? _DecisionDt;
		
		private DateTime? _JoinDt;
		
		private string _Decision;
		
		private string _Baptism;
		
		
		public MemberDatum()
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

		
		[Column(Name="Marital", Storage="_Marital", DbType="nvarchar(100)")]
		public string Marital
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

		
		[Column(Name="DecisionDt", Storage="_DecisionDt", DbType="datetime")]
		public DateTime? DecisionDt
		{
			get
			{
				return this._DecisionDt;
			}

			set
			{
				if (this._DecisionDt != value)
					this._DecisionDt = value;
			}

		}

		
		[Column(Name="JoinDt", Storage="_JoinDt", DbType="datetime")]
		public DateTime? JoinDt
		{
			get
			{
				return this._JoinDt;
			}

			set
			{
				if (this._JoinDt != value)
					this._JoinDt = value;
			}

		}

		
		[Column(Name="Decision", Storage="_Decision", DbType="nvarchar(20)")]
		public string Decision
		{
			get
			{
				return this._Decision;
			}

			set
			{
				if (this._Decision != value)
					this._Decision = value;
			}

		}

		
		[Column(Name="Baptism", Storage="_Baptism", DbType="nvarchar(100)")]
		public string Baptism
		{
			get
			{
				return this._Baptism;
			}

			set
			{
				if (this._Baptism != value)
					this._Baptism = value;
			}

		}

		
    }

}
