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
	[Table(Name="XpDivision")]
	public partial class XpDivision
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _Id;
		
		private string _Name;
		
		private int? _ProgId;
		
		
		public XpDivision()
		{
		}

		
		
		[Column(Name="Id", Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsDbGenerated=true)]
		public int Id
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

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(50)")]
		public string Name
		{
			get
			{
				return this._Name;
			}

			set
			{
				if (this._Name != value)
					this._Name = value;
			}

		}

		
		[Column(Name="ProgId", Storage="_ProgId", DbType="int")]
		public int? ProgId
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

		
    }

}
