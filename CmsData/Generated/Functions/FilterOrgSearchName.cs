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
	[Table(Name="FilterOrgSearchName")]
	public partial class FilterOrgSearchName
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Oid;
		
		
		public FilterOrgSearchName()
		{
		}

		
		
		[Column(Name="oid", Storage="_Oid", DbType="int")]
		public int? Oid
		{
			get
			{
				return this._Oid;
			}

			set
			{
				if (this._Oid != value)
					this._Oid = value;
			}

		}

		
    }

}
