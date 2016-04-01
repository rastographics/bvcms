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
	[Table(Name="GetPledgedTotalsBothIfJoint")]
	public partial class GetPledgedTotalsBothIfJoint
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _FamilyId;
		
		private int? _PeopleId;
		
		private string _Name;
		
		private decimal? _PledgeAmount;
		
		
		public GetPledgedTotalsBothIfJoint()
		{
		}

		
		
		[Column(Name="FamilyId", Storage="_FamilyId", DbType="int NOT NULL")]
		public int FamilyId
		{
			get
			{
				return this._FamilyId;
			}

			set
			{
				if (this._FamilyId != value)
					this._FamilyId = value;
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

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(139)")]
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

		
		[Column(Name="PledgeAmount", Storage="_PledgeAmount", DbType="Decimal(38,2)")]
		public decimal? PledgeAmount
		{
			get
			{
				return this._PledgeAmount;
			}

			set
			{
				if (this._PledgeAmount != value)
					this._PledgeAmount = value;
			}

		}

		
    }

}
