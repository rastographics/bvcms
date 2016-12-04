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
	[Table(Name="AttendanceChange")]
	public partial class AttendanceChange
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private decimal _Pct1;
		
		private decimal _Pct2;
		
		private decimal? _PctChange;
		
		private string _Change;
		
		
		public AttendanceChange()
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

		
		[Column(Name="Pct1", Storage="_Pct1", DbType="Decimal(28,12) NOT NULL")]
		public decimal Pct1
		{
			get
			{
				return this._Pct1;
			}

			set
			{
				if (this._Pct1 != value)
					this._Pct1 = value;
			}

		}

		
		[Column(Name="Pct2", Storage="_Pct2", DbType="Decimal(28,12) NOT NULL")]
		public decimal Pct2
		{
			get
			{
				return this._Pct2;
			}

			set
			{
				if (this._Pct2 != value)
					this._Pct2 = value;
			}

		}

		
		[Column(Name="PctChange", Storage="_PctChange", DbType="Decimal(38,6)")]
		public decimal? PctChange
		{
			get
			{
				return this._PctChange;
			}

			set
			{
				if (this._PctChange != value)
					this._PctChange = value;
			}

		}

		
		[Column(Name="CHANGE", Storage="_Change", DbType="nvarchar(4000)")]
		public string Change
		{
			get
			{
				return this._Change;
			}

			set
			{
				if (this._Change != value)
					this._Change = value;
			}

		}

		
    }

}
