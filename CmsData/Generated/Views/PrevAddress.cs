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
	[Table(Name="PrevAddress")]
	public partial class PrevAddress
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _PrevAddr;
		
		private string _PrevAddr2;
		
		private string _PrevCity;
		
		private string _PrevState;
		
		private string _PrevZip;
		
		
		public PrevAddress()
		{
		}

		
		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int NOT NULL")]
		public int PeopleId
		{
			get
			{
				return this._PeopleId;
			}

			set
			{
				if (this._PeopleId != value)
					this._PeopleId = value;
			}

		}

		
		[Column(Name="PrevAddr", Storage="_PrevAddr", DbType="nvarchar")]
		public string PrevAddr
		{
			get
			{
				return this._PrevAddr;
			}

			set
			{
				if (this._PrevAddr != value)
					this._PrevAddr = value;
			}

		}

		
		[Column(Name="PrevAddr2", Storage="_PrevAddr2", DbType="nvarchar")]
		public string PrevAddr2
		{
			get
			{
				return this._PrevAddr2;
			}

			set
			{
				if (this._PrevAddr2 != value)
					this._PrevAddr2 = value;
			}

		}

		
		[Column(Name="PrevCity", Storage="_PrevCity", DbType="nvarchar")]
		public string PrevCity
		{
			get
			{
				return this._PrevCity;
			}

			set
			{
				if (this._PrevCity != value)
					this._PrevCity = value;
			}

		}

		
		[Column(Name="PrevState", Storage="_PrevState", DbType="nvarchar")]
		public string PrevState
		{
			get
			{
				return this._PrevState;
			}

			set
			{
				if (this._PrevState != value)
					this._PrevState = value;
			}

		}

		
		[Column(Name="PrevZip", Storage="_PrevZip", DbType="nvarchar")]
		public string PrevZip
		{
			get
			{
				return this._PrevZip;
			}

			set
			{
				if (this._PrevZip != value)
					this._PrevZip = value;
			}

		}

		
    }

}
