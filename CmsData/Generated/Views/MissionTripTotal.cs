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
	[Table(Name="MissionTripTotals")]
	public partial class MissionTripTotal
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private string _Trip;
		
		private int? _PeopleId;
		
		private string _Name;
		
		private string _SortOrder;
		
		private decimal? _TripCost;
		
		private decimal? _Raised;
		
		private decimal? _Due;
		
		
		public MissionTripTotal()
		{
		}

		
		
		[Column(Name="OrganizationId", Storage="_OrganizationId", DbType="int NOT NULL")]
		public int OrganizationId
		{
			get
			{
				return this._OrganizationId;
			}

			set
			{
				if (this._OrganizationId != value)
					this._OrganizationId = value;
			}

		}

		
		[Column(Name="Trip", Storage="_Trip", DbType="nvarchar(100) NOT NULL")]
		public string Trip
		{
			get
			{
				return this._Trip;
			}

			set
			{
				if (this._Trip != value)
					this._Trip = value;
			}

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

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(138)")]
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

		
		[Column(Name="SortOrder", Storage="_SortOrder", DbType="nvarchar(139)")]
		public string SortOrder
		{
			get
			{
				return this._SortOrder;
			}

			set
			{
				if (this._SortOrder != value)
					this._SortOrder = value;
			}

		}

		
		[Column(Name="TripCost", Storage="_TripCost", DbType="money")]
		public decimal? TripCost
		{
			get
			{
				return this._TripCost;
			}

			set
			{
				if (this._TripCost != value)
					this._TripCost = value;
			}

		}

		
		[Column(Name="Raised", Storage="_Raised", DbType="money")]
		public decimal? Raised
		{
			get
			{
				return this._Raised;
			}

			set
			{
				if (this._Raised != value)
					this._Raised = value;
			}

		}

		
		[Column(Name="Due", Storage="_Due", DbType="money")]
		public decimal? Due
		{
			get
			{
				return this._Due;
			}

			set
			{
				if (this._Due != value)
					this._Due = value;
			}

		}

		
    }

}
