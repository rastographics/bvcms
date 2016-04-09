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
	[Table(Name="FamilyGiver")]
	public partial class FamilyGiver
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _FamilyId;
		
		private int _PeopleId;
		
		private bool? _FamGive;
		
		private bool? _FamPledge;
		
		
		public FamilyGiver()
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

		
		[Column(Name="FamGive", Storage="_FamGive", DbType="bit")]
		public bool? FamGive
		{
			get
			{
				return this._FamGive;
			}

			set
			{
				if (this._FamGive != value)
					this._FamGive = value;
			}

		}

		
		[Column(Name="FamPledge", Storage="_FamPledge", DbType="bit")]
		public bool? FamPledge
		{
			get
			{
				return this._FamPledge;
			}

			set
			{
				if (this._FamPledge != value)
					this._FamPledge = value;
			}

		}

		
    }

}
