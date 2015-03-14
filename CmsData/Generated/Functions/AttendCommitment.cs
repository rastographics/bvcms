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
	[Table(Name="AttendCommitments")]
	public partial class AttendCommitment
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _MeetingId;
		
		private DateTime _MeetingDate;
		
		private int _PeopleId;
		
		private string _Name2;
		
		private int? _Commitment;
		
		private bool? _Conflicts;
		
		
		public AttendCommitment()
		{
		}

		
		
		[Column(Name="MeetingId", Storage="_MeetingId", DbType="int NOT NULL")]
		public int MeetingId
		{
			get
			{
				return this._MeetingId;
			}

			set
			{
				if (this._MeetingId != value)
					this._MeetingId = value;
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

		
		[Column(Name="Name2", Storage="_Name2", DbType="nvarchar(139)")]
		public string Name2
		{
			get
			{
				return this._Name2;
			}

			set
			{
				if (this._Name2 != value)
					this._Name2 = value;
			}

		}

		
		[Column(Name="Commitment", Storage="_Commitment", DbType="int")]
		public int? Commitment
		{
			get
			{
				return this._Commitment;
			}

			set
			{
				if (this._Commitment != value)
					this._Commitment = value;
			}

		}

		
		[Column(Name="conflicts", Storage="_Conflicts", DbType="bit")]
		public bool? Conflicts
		{
			get
			{
				return this._Conflicts;
			}

			set
			{
				if (this._Conflicts != value)
					this._Conflicts = value;
			}

		}

		
    }

}
