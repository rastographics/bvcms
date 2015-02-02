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
	[Table(Name="FailedEmails")]
	public partial class FailedEmail
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private DateTime? _Time;
		
		private int _Id;
		
		private int _PeopleId;
		
		private string _Fail;
		
		
		public FailedEmail()
		{
		}

		
		
		[Column(Name="time", Storage="_Time", DbType="datetime")]
		public DateTime? Time
		{
			get
			{
				return this._Time;
			}

			set
			{
				if (this._Time != value)
					this._Time = value;
			}

		}

		
		[Column(Name="Id", Storage="_Id", DbType="int NOT NULL")]
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

		
		[Column(Name="Fail", Storage="_Fail", DbType="nvarchar(20)")]
		public string Fail
		{
			get
			{
				return this._Fail;
			}

			set
			{
				if (this._Fail != value)
					this._Fail = value;
			}

		}

		
    }

}
