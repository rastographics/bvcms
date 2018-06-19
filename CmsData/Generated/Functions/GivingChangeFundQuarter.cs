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
	[Table(Name="GivingChangeFundQuarters")]
	public partial class GivingChangeFundQuarter
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private decimal _TotalPeriod1;
		
		private decimal _TotalPeriod2;
		
		private decimal? _PctChange;
		
		private string _Change;
		
		
		public GivingChangeFundQuarter()
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

		
		[Column(Name="TotalPeriod1", Storage="_TotalPeriod1", DbType="Decimal(38,2) NOT NULL")]
		public decimal TotalPeriod1
		{
			get
			{
				return this._TotalPeriod1;
			}

			set
			{
				if (this._TotalPeriod1 != value)
					this._TotalPeriod1 = value;
			}

		}

		
		[Column(Name="TotalPeriod2", Storage="_TotalPeriod2", DbType="Decimal(38,2) NOT NULL")]
		public decimal TotalPeriod2
		{
			get
			{
				return this._TotalPeriod2;
			}

			set
			{
				if (this._TotalPeriod2 != value)
					this._TotalPeriod2 = value;
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

		
		[Column(Name="Change", Storage="_Change", DbType="nvarchar(4000)")]
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
