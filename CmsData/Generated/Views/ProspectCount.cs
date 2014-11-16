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
	[Table(Name="ProspectCounts")]
	public partial class ProspectCount
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private int? _Prospectcount;
		
		
		public ProspectCount()
		{
		}

		
		
		[Column(Name="OrganizationId", Storage="_OrganizationId", DbType="int NOT NULL")]
		public int OrganizationId
		{
			get
			{
				return this._OrganizationId;
			}

			set
			{
				if (this._OrganizationId != value)
					this._OrganizationId = value;
			}

		}

		
		[Column(Name="prospectcount", Storage="_Prospectcount", DbType="int")]
		public int? Prospectcount
		{
			get
			{
				return this._Prospectcount;
			}

			set
			{
				if (this._Prospectcount != value)
					this._Prospectcount = value;
			}

		}

		
    }

}
