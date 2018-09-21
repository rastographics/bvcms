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
	[Table(Name="RogueIps")]
	public partial class RogueIp
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _Ip;
		
		private string _Db;
		
		private DateTime? _Tm;
		
		
		public RogueIp()
		{
		}

		
		
		[Column(Name="ip", Storage="_Ip", DbType="varchar(50) NOT NULL")]
		public string Ip
		{
			get
			{
				return this._Ip;
			}

			set
			{
				if (this._Ip != value)
					this._Ip = value;
			}

		}

		
		[Column(Name="db", Storage="_Db", DbType="varchar(50)")]
		public string Db
		{
			get
			{
				return this._Db;
			}

			set
			{
				if (this._Db != value)
					this._Db = value;
			}

		}

		
		[Column(Name="tm", Storage="_Tm", DbType="datetime")]
		public DateTime? Tm
		{
			get
			{
				return this._Tm;
			}

			set
			{
				if (this._Tm != value)
					this._Tm = value;
			}

		}

		
    }

}
