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
	[Table(Name="XpDivOrg")]
	public partial class XpDivOrg
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _DivId;
		
		private int _OrgId;
		
		
		public XpDivOrg()
		{
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

		
		[Column(Name="OrgId", Storage="_OrgId", DbType="int NOT NULL")]
		public int OrgId
		{
			get
			{
				return this._OrgId;
			}

			set
			{
				if (this._OrgId != value)
					this._OrgId = value;
			}

		}

		
    }

}
