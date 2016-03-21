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
	[Table(Name="Churches")]
	public partial class Church
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _C;
		
		
		public Church()
		{
		}

		
		
		[Column(Name="c", Storage="_C", DbType="nvarchar(120)")]
		public string C
		{
			get
			{
				return this._C;
			}

			set
			{
				if (this._C != value)
					this._C = value;
			}

		}

		
    }

}
