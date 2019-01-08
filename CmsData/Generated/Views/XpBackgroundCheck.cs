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
	[Table(Name="XpBackgroundCheck")]
	public partial class XpBackgroundCheck
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Status;
		
		private DateTime? _ProcessedDate;
		
		private string _Comments;
		
		private int? _MVRStatusId;
		
		private DateTime? _MVRProcessedDate;
		
		
		public XpBackgroundCheck()
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

		
		[Column(Name="Status", Storage="_Status", DbType="nvarchar(50)")]
		public string Status
		{
			get
			{
				return this._Status;
			}

			set
			{
				if (this._Status != value)
					this._Status = value;
			}

		}

		
		[Column(Name="ProcessedDate", Storage="_ProcessedDate", DbType="datetime")]
		public DateTime? ProcessedDate
		{
			get
			{
				return this._ProcessedDate;
			}

			set
			{
				if (this._ProcessedDate != value)
					this._ProcessedDate = value;
			}

		}

		
		[Column(Name="Comments", Storage="_Comments", DbType="nvarchar")]
		public string Comments
		{
			get
			{
				return this._Comments;
			}

			set
			{
				if (this._Comments != value)
					this._Comments = value;
			}

		}

		
		[Column(Name="MVRStatusId", Storage="_MVRStatusId", DbType="int")]
		public int? MVRStatusId
		{
			get
			{
				return this._MVRStatusId;
			}

			set
			{
				if (this._MVRStatusId != value)
					this._MVRStatusId = value;
			}

		}

		
		[Column(Name="MVRProcessedDate", Storage="_MVRProcessedDate", DbType="datetime")]
		public DateTime? MVRProcessedDate
		{
			get
			{
				return this._MVRProcessedDate;
			}

			set
			{
				if (this._MVRProcessedDate != value)
					this._MVRProcessedDate = value;
			}

		}

		
    }

}
