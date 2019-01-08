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
	[Table(Name="XpContactee")]
	public partial class XpContactee
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _ContactId;
		
		private int _PeopleId;
		
		
		public XpContactee()
		{
		}

		
		
		[Column(Name="ContactId", Storage="_ContactId", DbType="int NOT NULL")]
		public int ContactId
		{
			get
			{
				return this._ContactId;
			}

			set
			{
				if (this._ContactId != value)
					this._ContactId = value;
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

		
    }

}
