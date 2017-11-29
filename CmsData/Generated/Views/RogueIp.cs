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
		
		
		private string _ClientIp;
		
		private int? _Cnt;
		
		private long? _Mdf;
		
		private DateTime? _St;
		
		private DateTime? _En;
		
		
		public RogueIp()
		{
		}

		
		
		[Column(Name="ClientIp", Storage="_ClientIp", DbType="nvarchar(50)")]
		public string ClientIp
		{
			get
			{
				return this._ClientIp;
			}

			set
			{
				if (this._ClientIp != value)
					this._ClientIp = value;
			}

		}

		
		[Column(Name="cnt", Storage="_Cnt", DbType="int")]
		public int? Cnt
		{
			get
			{
				return this._Cnt;
			}

			set
			{
				if (this._Cnt != value)
					this._Cnt = value;
			}

		}

		
		[Column(Name="mdf", Storage="_Mdf", DbType="bigint")]
		public long? Mdf
		{
			get
			{
				return this._Mdf;
			}

			set
			{
				if (this._Mdf != value)
					this._Mdf = value;
			}

		}

		
		[Column(Name="st", Storage="_St", DbType="datetime")]
		public DateTime? St
		{
			get
			{
				return this._St;
			}

			set
			{
				if (this._St != value)
					this._St = value;
			}

		}

		
		[Column(Name="en", Storage="_En", DbType="datetime")]
		public DateTime? En
		{
			get
			{
				return this._En;
			}

			set
			{
				if (this._En != value)
					this._En = value;
			}

		}

		
    }

}
