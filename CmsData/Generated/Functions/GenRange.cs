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
	[Table(Name="GenRanges")]
	public partial class GenRange
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Amt;
		
		private int? _MinAmt;
		
		private int? _MaxAmt;
		
		
		public GenRange()
		{
		}

		
		
		[Column(Name="amt", Storage="_Amt", DbType="int")]
		public int? Amt
		{
			get
			{
				return this._Amt;
			}

			set
			{
				if (this._Amt != value)
					this._Amt = value;
			}

		}

		
		[Column(Name="MinAmt", Storage="_MinAmt", DbType="int")]
		public int? MinAmt
		{
			get
			{
				return this._MinAmt;
			}

			set
			{
				if (this._MinAmt != value)
					this._MinAmt = value;
			}

		}

		
		[Column(Name="MaxAmt", Storage="_MaxAmt", DbType="int")]
		public int? MaxAmt
		{
			get
			{
				return this._MaxAmt;
			}

			set
			{
				if (this._MaxAmt != value)
					this._MaxAmt = value;
			}

		}

		
    }

}
