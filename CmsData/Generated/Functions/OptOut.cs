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
	[Table(Name="OptOuts")]
	public partial class OptOut
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Name;
		
		private bool? _OptOutX;
		
		private int? _HhPeopleId;
		
		private string _HhName;
		
		private string _HhEmail;
		
		private int? _HhSpPeopleId;
		
		private string _HhSpName;
		
		private string _HhSpEmail;
		
		
		public OptOut()
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

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(138)")]
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

		
		[Column(Name="OptOut", Storage="_OptOutX", DbType="bit")]
		public bool? OptOutX
		{
			get
			{
				return this._OptOutX;
			}

			set
			{
				if (this._OptOutX != value)
					this._OptOutX = value;
			}

		}

		
		[Column(Name="HhPeopleId", Storage="_HhPeopleId", DbType="int")]
		public int? HhPeopleId
		{
			get
			{
				return this._HhPeopleId;
			}

			set
			{
				if (this._HhPeopleId != value)
					this._HhPeopleId = value;
			}

		}

		
		[Column(Name="HhName", Storage="_HhName", DbType="nvarchar(138)")]
		public string HhName
		{
			get
			{
				return this._HhName;
			}

			set
			{
				if (this._HhName != value)
					this._HhName = value;
			}

		}

		
		[Column(Name="HhEmail", Storage="_HhEmail", DbType="nvarchar(150)")]
		public string HhEmail
		{
			get
			{
				return this._HhEmail;
			}

			set
			{
				if (this._HhEmail != value)
					this._HhEmail = value;
			}

		}

		
		[Column(Name="HhSpPeopleId", Storage="_HhSpPeopleId", DbType="int")]
		public int? HhSpPeopleId
		{
			get
			{
				return this._HhSpPeopleId;
			}

			set
			{
				if (this._HhSpPeopleId != value)
					this._HhSpPeopleId = value;
			}

		}

		
		[Column(Name="HhSpName", Storage="_HhSpName", DbType="nvarchar(138)")]
		public string HhSpName
		{
			get
			{
				return this._HhSpName;
			}

			set
			{
				if (this._HhSpName != value)
					this._HhSpName = value;
			}

		}

		
		[Column(Name="HhSpEmail", Storage="_HhSpEmail", DbType="nvarchar(150)")]
		public string HhSpEmail
		{
			get
			{
				return this._HhSpEmail;
			}

			set
			{
				if (this._HhSpEmail != value)
					this._HhSpEmail = value;
			}

		}

		
    }

}
