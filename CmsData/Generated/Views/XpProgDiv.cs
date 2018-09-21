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
	[Table(Name="XpProgDiv")]
	public partial class XpProgDiv
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _ProgId;
		
		private int _DivId;
		
		
		public XpProgDiv()
		{
		}

		
		
		[Column(Name="ProgId", Storage="_ProgId", DbType="int NOT NULL")]
		public int ProgId
		{
			get
			{
				return this._ProgId;
			}

			set
			{
				if (this._ProgId != value)
					this._ProgId = value;
			}

		}

		
		[Column(Name="DivId", Storage="_DivId", DbType="int NOT NULL")]
		public int DivId
		{
			get
			{
				return this._DivId;
			}

			set
			{
				if (this._DivId != value)
					this._DivId = value;
			}

		}

		
    }

}
