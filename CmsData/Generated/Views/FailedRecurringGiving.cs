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
	[Table(Name="FailedRecurringGiving")]
	public partial class FailedRecurringGiving
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private DateTime? _Dt;
		
		
		public FailedRecurringGiving()
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

		
		[Column(Name="Dt", Storage="_Dt", DbType="datetime")]
		public DateTime? Dt
		{
			get
			{
				return this._Dt;
			}

			set
			{
				if (this._Dt != value)
					this._Dt = value;
			}

		}

		
    }

}
