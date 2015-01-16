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
	[Table(Name="VisitNumberSinceDate")]
	public partial class VisitNumberSinceDate
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Name;
		
		private DateTime _MeetingDate;
		
		private long? _Rank;
		
		
		public VisitNumberSinceDate()
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

		
		[Column(Name="MeetingDate", Storage="_MeetingDate", DbType="datetime NOT NULL")]
		public DateTime MeetingDate
		{
			get
			{
				return this._MeetingDate;
			}

			set
			{
				if (this._MeetingDate != value)
					this._MeetingDate = value;
			}

		}

		
		[Column(Name="Rank", Storage="_Rank", DbType="bigint")]
		public long? Rank
		{
			get
			{
				return this._Rank;
			}

			set
			{
				if (this._Rank != value)
					this._Rank = value;
			}

		}

		
    }

}
