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
	[Table(Name="IncompleteTasks")]
	public partial class IncompleteTask
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private DateTime _CreatedOn;
		
		private string _Owner;
		
		private string _DelegatedTo;
		
		private string _Description;
		
		private string _DeclineReason;
		
		private string _About;
		
		private string _Notes;
		
		private string _Status;
		
		private bool? _ForceCompleteWContact;
		
		private int _Id;
		
		private int _OwnerId;
		
		private int? _CoOwnerId;
		
		private int? _WhoId;
		
		private int? _StatusId;
		
		private int? _SourceContactId;
		
		private DateTime? _Due;
		
		private int? _AboutPictureId;
		
		private int? _PictureX;
		
		private int? _PictureY;
		
		
		public IncompleteTask()
		{
		}

		
		
		[Column(Name="CreatedOn", Storage="_CreatedOn", DbType="datetime NOT NULL")]
		public DateTime CreatedOn
		{
			get
			{
				return this._CreatedOn;
			}

			set
			{
				if (this._CreatedOn != value)
					this._CreatedOn = value;
			}

		}

		
		[Column(Name="Owner", Storage="_Owner", DbType="nvarchar(138)")]
		public string Owner
		{
			get
			{
				return this._Owner;
			}

			set
			{
				if (this._Owner != value)
					this._Owner = value;
			}

		}

		
		[Column(Name="DelegatedTo", Storage="_DelegatedTo", DbType="nvarchar(138)")]
		public string DelegatedTo
		{
			get
			{
				return this._DelegatedTo;
			}

			set
			{
				if (this._DelegatedTo != value)
					this._DelegatedTo = value;
			}

		}

		
		[Column(Name="Description", Storage="_Description", DbType="nvarchar(100)")]
		public string Description
		{
			get
			{
				return this._Description;
			}

			set
			{
				if (this._Description != value)
					this._Description = value;
			}

		}

		
		[Column(Name="DeclineReason", Storage="_DeclineReason", DbType="nvarchar")]
		public string DeclineReason
		{
			get
			{
				return this._DeclineReason;
			}

			set
			{
				if (this._DeclineReason != value)
					this._DeclineReason = value;
			}

		}

		
		[Column(Name="About", Storage="_About", DbType="nvarchar(138)")]
		public string About
		{
			get
			{
				return this._About;
			}

			set
			{
				if (this._About != value)
					this._About = value;
			}

		}

		
		[Column(Name="Notes", Storage="_Notes", DbType="nvarchar")]
		public string Notes
		{
			get
			{
				return this._Notes;
			}

			set
			{
				if (this._Notes != value)
					this._Notes = value;
			}

		}

		
		[Column(Name="Status", Storage="_Status", DbType="nvarchar(100)")]
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

		
		[Column(Name="ForceCompleteWContact", Storage="_ForceCompleteWContact", DbType="bit")]
		public bool? ForceCompleteWContact
		{
			get
			{
				return this._ForceCompleteWContact;
			}

			set
			{
				if (this._ForceCompleteWContact != value)
					this._ForceCompleteWContact = value;
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

		
		[Column(Name="OwnerId", Storage="_OwnerId", DbType="int NOT NULL")]
		public int OwnerId
		{
			get
			{
				return this._OwnerId;
			}

			set
			{
				if (this._OwnerId != value)
					this._OwnerId = value;
			}

		}

		
		[Column(Name="CoOwnerId", Storage="_CoOwnerId", DbType="int")]
		public int? CoOwnerId
		{
			get
			{
				return this._CoOwnerId;
			}

			set
			{
				if (this._CoOwnerId != value)
					this._CoOwnerId = value;
			}

		}

		
		[Column(Name="WhoId", Storage="_WhoId", DbType="int")]
		public int? WhoId
		{
			get
			{
				return this._WhoId;
			}

			set
			{
				if (this._WhoId != value)
					this._WhoId = value;
			}

		}

		
		[Column(Name="StatusId", Storage="_StatusId", DbType="int")]
		public int? StatusId
		{
			get
			{
				return this._StatusId;
			}

			set
			{
				if (this._StatusId != value)
					this._StatusId = value;
			}

		}

		
		[Column(Name="SourceContactId", Storage="_SourceContactId", DbType="int")]
		public int? SourceContactId
		{
			get
			{
				return this._SourceContactId;
			}

			set
			{
				if (this._SourceContactId != value)
					this._SourceContactId = value;
			}

		}

		
		[Column(Name="Due", Storage="_Due", DbType="datetime")]
		public DateTime? Due
		{
			get
			{
				return this._Due;
			}

			set
			{
				if (this._Due != value)
					this._Due = value;
			}

		}

		
		[Column(Name="AboutPictureId", Storage="_AboutPictureId", DbType="int")]
		public int? AboutPictureId
		{
			get
			{
				return this._AboutPictureId;
			}

			set
			{
				if (this._AboutPictureId != value)
					this._AboutPictureId = value;
			}

		}

		
		[Column(Name="PictureX", Storage="_PictureX", DbType="int")]
		public int? PictureX
		{
			get
			{
				return this._PictureX;
			}

			set
			{
				if (this._PictureX != value)
					this._PictureX = value;
			}

		}

		
		[Column(Name="PictureY", Storage="_PictureY", DbType="int")]
		public int? PictureY
		{
			get
			{
				return this._PictureY;
			}

			set
			{
				if (this._PictureY != value)
					this._PictureY = value;
			}

		}

		
    }

}
