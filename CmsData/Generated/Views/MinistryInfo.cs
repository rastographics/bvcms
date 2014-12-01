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
	[Table(Name="MinistryInfo")]
	public partial class MinistryInfo
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private int? _LastContactReceivedId;
		
		private DateTime? _LastContactReceivedDt;
		
		private int? _LastContactMadeId;
		
		private DateTime? _LastContactMadeDt;
		
		private int? _LastTaskAboutId;
		
		private DateTime? _LastTaskAboutDt;
		
		private int? _LastTaskDelegatedId;
		
		private DateTime? _LastTaskDelegatedDt;
		
		
		public MinistryInfo()
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

		
		[Column(Name="LastContactReceivedId", Storage="_LastContactReceivedId", DbType="int")]
		public int? LastContactReceivedId
		{
			get
			{
				return this._LastContactReceivedId;
			}

			set
			{
				if (this._LastContactReceivedId != value)
					this._LastContactReceivedId = value;
			}

		}

		
		[Column(Name="LastContactReceivedDt", Storage="_LastContactReceivedDt", DbType="datetime")]
		public DateTime? LastContactReceivedDt
		{
			get
			{
				return this._LastContactReceivedDt;
			}

			set
			{
				if (this._LastContactReceivedDt != value)
					this._LastContactReceivedDt = value;
			}

		}

		
		[Column(Name="LastContactMadeId", Storage="_LastContactMadeId", DbType="int")]
		public int? LastContactMadeId
		{
			get
			{
				return this._LastContactMadeId;
			}

			set
			{
				if (this._LastContactMadeId != value)
					this._LastContactMadeId = value;
			}

		}

		
		[Column(Name="LastContactMadeDt", Storage="_LastContactMadeDt", DbType="datetime")]
		public DateTime? LastContactMadeDt
		{
			get
			{
				return this._LastContactMadeDt;
			}

			set
			{
				if (this._LastContactMadeDt != value)
					this._LastContactMadeDt = value;
			}

		}

		
		[Column(Name="LastTaskAboutId", Storage="_LastTaskAboutId", DbType="int")]
		public int? LastTaskAboutId
		{
			get
			{
				return this._LastTaskAboutId;
			}

			set
			{
				if (this._LastTaskAboutId != value)
					this._LastTaskAboutId = value;
			}

		}

		
		[Column(Name="LastTaskAboutDt", Storage="_LastTaskAboutDt", DbType="datetime")]
		public DateTime? LastTaskAboutDt
		{
			get
			{
				return this._LastTaskAboutDt;
			}

			set
			{
				if (this._LastTaskAboutDt != value)
					this._LastTaskAboutDt = value;
			}

		}

		
		[Column(Name="LastTaskDelegatedId", Storage="_LastTaskDelegatedId", DbType="int")]
		public int? LastTaskDelegatedId
		{
			get
			{
				return this._LastTaskDelegatedId;
			}

			set
			{
				if (this._LastTaskDelegatedId != value)
					this._LastTaskDelegatedId = value;
			}

		}

		
		[Column(Name="LastTaskDelegatedDt", Storage="_LastTaskDelegatedDt", DbType="datetime")]
		public DateTime? LastTaskDelegatedDt
		{
			get
			{
				return this._LastTaskDelegatedDt;
			}

			set
			{
				if (this._LastTaskDelegatedDt != value)
					this._LastTaskDelegatedDt = value;
			}

		}

		
    }

}
