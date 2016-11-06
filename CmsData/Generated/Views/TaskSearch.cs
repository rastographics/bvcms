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
	[Table(Name="TaskSearch")]
	public partial class TaskSearch
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private DateTime? _Created;
		
		private string _Status;
		
		private DateTime? _Due;
		
		private DateTime? _Completed;
		
		private bool _Archive;
		
		private string _Notes;
		
		private string _Description;
		
		private bool? _ForceCompleteWContact;
		
		private string _DeclineReason;
		
		private string _LimitToRole;
		
		private string _Originator;
		
		private string _Owner;
		
		private string _DelegateX;
		
		private string _About;
		
		private string _Originator2;
		
		private string _Owner2;
		
		private string _Delegate2;
		
		private string _About2;
		
		private int _Id;
		
		private int? _StatusId;
		
		private int _OwnerId;
		
		private int? _CoOwnerId;
		
		private int? _OrginatorId;
		
		private int? _WhoId;
		
		private int? _SourceContactId;
		
		private int? _CompletedContactId;
		
		
		public TaskSearch()
		{
		}

		
		
		[Column(Name="Created", Storage="_Created", DbType="date")]
		public DateTime? Created
		{
			get
			{
				return this._Created;
			}

			set
			{
				if (this._Created != value)
					this._Created = value;
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

		
		[Column(Name="Due", Storage="_Due", DbType="date")]
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

		
		[Column(Name="Completed", Storage="_Completed", DbType="date")]
		public DateTime? Completed
		{
			get
			{
				return this._Completed;
			}

			set
			{
				if (this._Completed != value)
					this._Completed = value;
			}

		}

		
		[Column(Name="Archive", Storage="_Archive", DbType="bit NOT NULL")]
		public bool Archive
		{
			get
			{
				return this._Archive;
			}

			set
			{
				if (this._Archive != value)
					this._Archive = value;
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

		
		[Column(Name="LimitToRole", Storage="_LimitToRole", DbType="nvarchar(50)")]
		public string LimitToRole
		{
			get
			{
				return this._LimitToRole;
			}

			set
			{
				if (this._LimitToRole != value)
					this._LimitToRole = value;
			}

		}

		
		[Column(Name="Originator", Storage="_Originator", DbType="nvarchar(138)")]
		public string Originator
		{
			get
			{
				return this._Originator;
			}

			set
			{
				if (this._Originator != value)
					this._Originator = value;
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

		
		[Column(Name="Delegate", Storage="_DelegateX", DbType="nvarchar(138)")]
		public string DelegateX
		{
			get
			{
				return this._DelegateX;
			}

			set
			{
				if (this._DelegateX != value)
					this._DelegateX = value;
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

		
		[Column(Name="Originator2", Storage="_Originator2", DbType="nvarchar(139)")]
		public string Originator2
		{
			get
			{
				return this._Originator2;
			}

			set
			{
				if (this._Originator2 != value)
					this._Originator2 = value;
			}

		}

		
		[Column(Name="Owner2", Storage="_Owner2", DbType="nvarchar(139)")]
		public string Owner2
		{
			get
			{
				return this._Owner2;
			}

			set
			{
				if (this._Owner2 != value)
					this._Owner2 = value;
			}

		}

		
		[Column(Name="Delegate2", Storage="_Delegate2", DbType="nvarchar(139)")]
		public string Delegate2
		{
			get
			{
				return this._Delegate2;
			}

			set
			{
				if (this._Delegate2 != value)
					this._Delegate2 = value;
			}

		}

		
		[Column(Name="About2", Storage="_About2", DbType="nvarchar(139)")]
		public string About2
		{
			get
			{
				return this._About2;
			}

			set
			{
				if (this._About2 != value)
					this._About2 = value;
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

		
		[Column(Name="OrginatorId", Storage="_OrginatorId", DbType="int")]
		public int? OrginatorId
		{
			get
			{
				return this._OrginatorId;
			}

			set
			{
				if (this._OrginatorId != value)
					this._OrginatorId = value;
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

		
		[Column(Name="CompletedContactId", Storage="_CompletedContactId", DbType="int")]
		public int? CompletedContactId
		{
			get
			{
				return this._CompletedContactId;
			}

			set
			{
				if (this._CompletedContactId != value)
					this._CompletedContactId = value;
			}

		}

		
    }

}
