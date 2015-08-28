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
	[Table(Name="FamilyFirstTimes")]
	public partial class FamilyFirstTime
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _FamilyId;
		
		private int? _HeadOfHouseholdId;
		
		private DateTime? _FirstDate;
		
		private DateTime? _CreatedDate;
		
		
		public FamilyFirstTime()
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

		
		[Column(Name="HeadOfHouseholdId", Storage="_HeadOfHouseholdId", DbType="int")]
		public int? HeadOfHouseholdId
		{
			get
			{
				return this._HeadOfHouseholdId;
			}

			set
			{
				if (this._HeadOfHouseholdId != value)
					this._HeadOfHouseholdId = value;
			}

		}

		
		[Column(Name="FirstDate", Storage="_FirstDate", DbType="datetime")]
		public DateTime? FirstDate
		{
			get
			{
				return this._FirstDate;
			}

			set
			{
				if (this._FirstDate != value)
					this._FirstDate = value;
			}

		}

		
		[Column(Name="CreatedDate", Storage="_CreatedDate", DbType="datetime")]
		public DateTime? CreatedDate
		{
			get
			{
				return this._CreatedDate;
			}

			set
			{
				if (this._CreatedDate != value)
					this._CreatedDate = value;
			}

		}

		
    }

}
