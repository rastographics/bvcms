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
	[Table(Name="TransactionSearch")]
	public partial class TransactionSearch
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Id;
		
		
		public TransactionSearch()
		{
		}

		
		
		[Column(Name="Id", Storage="_Id", DbType="int")]
		public int? Id
		{
			get
			{
				return this._Id;
			}

			set
			{
				if (this._Id != value)
					this._Id = value;
			}

		}

		
    }

}
