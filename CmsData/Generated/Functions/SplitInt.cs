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
	[Table(Name="SplitInts")]
	public partial class SplitInt
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _ValueX;
		
		
		public SplitInt()
		{
		}

		
		
		[Column(Name="Value", Storage="_ValueX", DbType="int")]
		public int? ValueX
		{
			get
			{
				return this._ValueX;
			}

			set
			{
				if (this._ValueX != value)
					this._ValueX = value;
			}

		}

		
    }

}
