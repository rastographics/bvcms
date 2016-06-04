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
	[Table(Name="LastAddrElement")]
	public partial class LastAddrElement
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Prev;
		
		
		public LastAddrElement()
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

		
		[Column(Name="Prev", Storage="_Prev", DbType="nvarchar NOT NULL")]
		public string Prev
		{
			get
			{
				return this._Prev;
			}

			set
			{
				if (this._Prev != value)
					this._Prev = value;
			}

		}

		
    }

}
