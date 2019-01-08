using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using CmsData.Infrastructure;

namespace CmsData
{
	[Table(Name="dbo.Meetings")]
	public partial class Meeting : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _MeetingId;
		
		private int _CreatedBy;
		
		private DateTime _CreatedDate;
		
		private int _OrganizationId;
		
		private int _NumPresent;
		
		private int _NumMembers;
		
		private int _NumVstMembers;
		
		private int _NumRepeatVst;
		
		private int _NumNewVisit;
		
		private string _Location;
		
		private DateTime? _MeetingDate;
		
		private bool _GroupMeetingFlag;
		
		private string _Description;
		
		private int? _NumOutTown;
		
		private int? _NumOtherAttends;
		
		private int? _AttendCreditId;
		
		private int? _ScheduleId;
		
		private bool? _NoAutoAbsents;
		
		private int? _HeadCount;
		
		private int? _MaxCount;
		
   		
   		private EntitySet<Attend> _Attends;
		
   		private EntitySet<MeetingExtra> _MeetingExtras;
		
   		private EntitySet<VolRequest> _VolRequests;
		
    	
		private EntityRef<AttendCredit> _AttendCredit;
		
		private EntityRef<Organization> _Organization;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnMeetingIdChanging(int value);
		partial void OnMeetingIdChanged();
		
		partial void OnCreatedByChanging(int value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime value);
		partial void OnCreatedDateChanged();
		
		partial void OnOrganizationIdChanging(int value);
		partial void OnOrganizationIdChanged();
		
		partial void OnNumPresentChanging(int value);
		partial void OnNumPresentChanged();
		
		partial void OnNumMembersChanging(int value);
		partial void OnNumMembersChanged();
		
		partial void OnNumVstMembersChanging(int value);
		partial void OnNumVstMembersChanged();
		
		partial void OnNumRepeatVstChanging(int value);
		partial void OnNumRepeatVstChanged();
		
		partial void OnNumNewVisitChanging(int value);
		partial void OnNumNewVisitChanged();
		
		partial void OnLocationChanging(string value);
		partial void OnLocationChanged();
		
		partial void OnMeetingDateChanging(DateTime? value);
		partial void OnMeetingDateChanged();
		
		partial void OnGroupMeetingFlagChanging(bool value);
		partial void OnGroupMeetingFlagChanged();
		
		partial void OnDescriptionChanging(string value);
		partial void OnDescriptionChanged();
		
		partial void OnNumOutTownChanging(int? value);
		partial void OnNumOutTownChanged();
		
		partial void OnNumOtherAttendsChanging(int? value);
		partial void OnNumOtherAttendsChanged();
		
		partial void OnAttendCreditIdChanging(int? value);
		partial void OnAttendCreditIdChanged();
		
		partial void OnScheduleIdChanging(int? value);
		partial void OnScheduleIdChanged();
		
		partial void OnNoAutoAbsentsChanging(bool? value);
		partial void OnNoAutoAbsentsChanged();
		
		partial void OnHeadCountChanging(int? value);
		partial void OnHeadCountChanged();
		
		partial void OnMaxCountChanging(int? value);
		partial void OnMaxCountChanged();
		
    #endregion
		public Meeting()
		{
			
			this._Attends = new EntitySet<Attend>(new Action< Attend>(this.attach_Attends), new Action< Attend>(this.detach_Attends)); 
			
			this._MeetingExtras = new EntitySet<MeetingExtra>(new Action< MeetingExtra>(this.attach_MeetingExtras), new Action< MeetingExtra>(this.detach_MeetingExtras)); 
			
			this._VolRequests = new EntitySet<VolRequest>(new Action< VolRequest>(this.attach_VolRequests), new Action< VolRequest>(this.detach_VolRequests)); 
			
			
			this._AttendCredit = default(EntityRef<AttendCredit>); 
			
			this._Organization = default(EntityRef<Organization>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="MeetingId", UpdateCheck=UpdateCheck.Never, Storage="_MeetingId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int MeetingId
		{
			get { return this._MeetingId; }

			set
			{
				if (this._MeetingId != value)
				{
				
                    this.OnMeetingIdChanging(value);
					this.SendPropertyChanging();
					this._MeetingId = value;
					this.SendPropertyChanged("MeetingId");
					this.OnMeetingIdChanged();
				}

			}

		}

		
		[Column(Name="CreatedBy", UpdateCheck=UpdateCheck.Never, Storage="_CreatedBy", DbType="int NOT NULL")]
		public int CreatedBy
		{
			get { return this._CreatedBy; }

			set
			{
				if (this._CreatedBy != value)
				{
				
                    this.OnCreatedByChanging(value);
					this.SendPropertyChanging();
					this._CreatedBy = value;
					this.SendPropertyChanged("CreatedBy");
					this.OnCreatedByChanged();
				}

			}

		}

		
		[Column(Name="CreatedDate", UpdateCheck=UpdateCheck.Never, Storage="_CreatedDate", DbType="datetime NOT NULL")]
		public DateTime CreatedDate
		{
			get { return this._CreatedDate; }

			set
			{
				if (this._CreatedDate != value)
				{
				
                    this.OnCreatedDateChanging(value);
					this.SendPropertyChanging();
					this._CreatedDate = value;
					this.SendPropertyChanged("CreatedDate");
					this.OnCreatedDateChanged();
				}

			}

		}

		
		[Column(Name="OrganizationId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int OrganizationId
		{
			get { return this._OrganizationId; }

			set
			{
				if (this._OrganizationId != value)
				{
				
					if (this._Organization.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrganizationIdChanging(value);
					this.SendPropertyChanging();
					this._OrganizationId = value;
					this.SendPropertyChanged("OrganizationId");
					this.OnOrganizationIdChanged();
				}

			}

		}

		
		[Column(Name="NumPresent", UpdateCheck=UpdateCheck.Never, Storage="_NumPresent", DbType="int NOT NULL")]
		public int NumPresent
		{
			get { return this._NumPresent; }

			set
			{
				if (this._NumPresent != value)
				{
				
                    this.OnNumPresentChanging(value);
					this.SendPropertyChanging();
					this._NumPresent = value;
					this.SendPropertyChanged("NumPresent");
					this.OnNumPresentChanged();
				}

			}

		}

		
		[Column(Name="NumMembers", UpdateCheck=UpdateCheck.Never, Storage="_NumMembers", DbType="int NOT NULL")]
		public int NumMembers
		{
			get { return this._NumMembers; }

			set
			{
				if (this._NumMembers != value)
				{
				
                    this.OnNumMembersChanging(value);
					this.SendPropertyChanging();
					this._NumMembers = value;
					this.SendPropertyChanged("NumMembers");
					this.OnNumMembersChanged();
				}

			}

		}

		
		[Column(Name="NumVstMembers", UpdateCheck=UpdateCheck.Never, Storage="_NumVstMembers", DbType="int NOT NULL")]
		public int NumVstMembers
		{
			get { return this._NumVstMembers; }

			set
			{
				if (this._NumVstMembers != value)
				{
				
                    this.OnNumVstMembersChanging(value);
					this.SendPropertyChanging();
					this._NumVstMembers = value;
					this.SendPropertyChanged("NumVstMembers");
					this.OnNumVstMembersChanged();
				}

			}

		}

		
		[Column(Name="NumRepeatVst", UpdateCheck=UpdateCheck.Never, Storage="_NumRepeatVst", DbType="int NOT NULL")]
		public int NumRepeatVst
		{
			get { return this._NumRepeatVst; }

			set
			{
				if (this._NumRepeatVst != value)
				{
				
                    this.OnNumRepeatVstChanging(value);
					this.SendPropertyChanging();
					this._NumRepeatVst = value;
					this.SendPropertyChanged("NumRepeatVst");
					this.OnNumRepeatVstChanged();
				}

			}

		}

		
		[Column(Name="NumNewVisit", UpdateCheck=UpdateCheck.Never, Storage="_NumNewVisit", DbType="int NOT NULL")]
		public int NumNewVisit
		{
			get { return this._NumNewVisit; }

			set
			{
				if (this._NumNewVisit != value)
				{
				
                    this.OnNumNewVisitChanging(value);
					this.SendPropertyChanging();
					this._NumNewVisit = value;
					this.SendPropertyChanged("NumNewVisit");
					this.OnNumNewVisitChanged();
				}

			}

		}

		
		[Column(Name="Location", UpdateCheck=UpdateCheck.Never, Storage="_Location", DbType="nvarchar(200)")]
		public string Location
		{
			get { return this._Location; }

			set
			{
				if (this._Location != value)
				{
				
                    this.OnLocationChanging(value);
					this.SendPropertyChanging();
					this._Location = value;
					this.SendPropertyChanged("Location");
					this.OnLocationChanged();
				}

			}

		}

		
		[Column(Name="MeetingDate", UpdateCheck=UpdateCheck.Never, Storage="_MeetingDate", DbType="datetime")]
		public DateTime? MeetingDate
		{
			get { return this._MeetingDate; }

			set
			{
				if (this._MeetingDate != value)
				{
				
                    this.OnMeetingDateChanging(value);
					this.SendPropertyChanging();
					this._MeetingDate = value;
					this.SendPropertyChanged("MeetingDate");
					this.OnMeetingDateChanged();
				}

			}

		}

		
		[Column(Name="GroupMeetingFlag", UpdateCheck=UpdateCheck.Never, Storage="_GroupMeetingFlag", DbType="bit NOT NULL")]
		public bool GroupMeetingFlag
		{
			get { return this._GroupMeetingFlag; }

			set
			{
				if (this._GroupMeetingFlag != value)
				{
				
                    this.OnGroupMeetingFlagChanging(value);
					this.SendPropertyChanging();
					this._GroupMeetingFlag = value;
					this.SendPropertyChanged("GroupMeetingFlag");
					this.OnGroupMeetingFlagChanged();
				}

			}

		}

		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar(100)")]
		public string Description
		{
			get { return this._Description; }

			set
			{
				if (this._Description != value)
				{
				
                    this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}

			}

		}

		
		[Column(Name="NumOutTown", UpdateCheck=UpdateCheck.Never, Storage="_NumOutTown", DbType="int")]
		public int? NumOutTown
		{
			get { return this._NumOutTown; }

			set
			{
				if (this._NumOutTown != value)
				{
				
                    this.OnNumOutTownChanging(value);
					this.SendPropertyChanging();
					this._NumOutTown = value;
					this.SendPropertyChanged("NumOutTown");
					this.OnNumOutTownChanged();
				}

			}

		}

		
		[Column(Name="NumOtherAttends", UpdateCheck=UpdateCheck.Never, Storage="_NumOtherAttends", DbType="int")]
		public int? NumOtherAttends
		{
			get { return this._NumOtherAttends; }

			set
			{
				if (this._NumOtherAttends != value)
				{
				
                    this.OnNumOtherAttendsChanging(value);
					this.SendPropertyChanging();
					this._NumOtherAttends = value;
					this.SendPropertyChanged("NumOtherAttends");
					this.OnNumOtherAttendsChanged();
				}

			}

		}

		
		[Column(Name="AttendCreditId", UpdateCheck=UpdateCheck.Never, Storage="_AttendCreditId", DbType="int")]
		[IsForeignKey]
		public int? AttendCreditId
		{
			get { return this._AttendCreditId; }

			set
			{
				if (this._AttendCreditId != value)
				{
				
					if (this._AttendCredit.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnAttendCreditIdChanging(value);
					this.SendPropertyChanging();
					this._AttendCreditId = value;
					this.SendPropertyChanged("AttendCreditId");
					this.OnAttendCreditIdChanged();
				}

			}

		}

		
		[Column(Name="ScheduleId", UpdateCheck=UpdateCheck.Never, Storage="_ScheduleId", DbType="int", IsDbGenerated=true)]
		public int? ScheduleId
		{
			get { return this._ScheduleId; }

			set
			{
				if (this._ScheduleId != value)
				{
				
                    this.OnScheduleIdChanging(value);
					this.SendPropertyChanging();
					this._ScheduleId = value;
					this.SendPropertyChanged("ScheduleId");
					this.OnScheduleIdChanged();
				}

			}

		}

		
		[Column(Name="NoAutoAbsents", UpdateCheck=UpdateCheck.Never, Storage="_NoAutoAbsents", DbType="bit")]
		public bool? NoAutoAbsents
		{
			get { return this._NoAutoAbsents; }

			set
			{
				if (this._NoAutoAbsents != value)
				{
				
                    this.OnNoAutoAbsentsChanging(value);
					this.SendPropertyChanging();
					this._NoAutoAbsents = value;
					this.SendPropertyChanged("NoAutoAbsents");
					this.OnNoAutoAbsentsChanged();
				}

			}

		}

		
		[Column(Name="HeadCount", UpdateCheck=UpdateCheck.Never, Storage="_HeadCount", DbType="int")]
		public int? HeadCount
		{
			get { return this._HeadCount; }

			set
			{
				if (this._HeadCount != value)
				{
				
                    this.OnHeadCountChanging(value);
					this.SendPropertyChanging();
					this._HeadCount = value;
					this.SendPropertyChanged("HeadCount");
					this.OnHeadCountChanged();
				}

			}

		}

		
		[Column(Name="MaxCount", UpdateCheck=UpdateCheck.Never, Storage="_MaxCount", DbType="int", IsDbGenerated=true)]
		public int? MaxCount
		{
			get { return this._MaxCount; }

			set
			{
				if (this._MaxCount != value)
				{
				
                    this.OnMaxCountChanging(value);
					this.SendPropertyChanging();
					this._MaxCount = value;
					this.SendPropertyChanged("MaxCount");
					this.OnMaxCountChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_AttendWithAbsents_TBL_MEETINGS_TBL", Storage="_Attends", OtherKey="MeetingId")]
   		public EntitySet<Attend> Attends
   		{
   		    get { return this._Attends; }

			set	{ this._Attends.Assign(value); }

   		}

		
   		[Association(Name="FK_MeetingExtra_Meetings", Storage="_MeetingExtras", OtherKey="MeetingId")]
   		public EntitySet<MeetingExtra> MeetingExtras
   		{
   		    get { return this._MeetingExtras; }

			set	{ this._MeetingExtras.Assign(value); }

   		}

		
   		[Association(Name="VolRequests__Meeting", Storage="_VolRequests", OtherKey="MeetingId")]
   		public EntitySet<VolRequest> VolRequests
   		{
   		    get { return this._VolRequests; }

			set	{ this._VolRequests.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_Meetings_AttendCredit", Storage="_AttendCredit", ThisKey="AttendCreditId", IsForeignKey=true)]
		public AttendCredit AttendCredit
		{
			get { return this._AttendCredit.Entity; }

			set
			{
				AttendCredit previousValue = this._AttendCredit.Entity;
				if (((previousValue != value) 
							|| (this._AttendCredit.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._AttendCredit.Entity = null;
						previousValue.Meetings.Remove(this);
					}

					this._AttendCredit.Entity = value;
					if (value != null)
					{
						value.Meetings.Add(this);
						
						this._AttendCreditId = value.Id;
						
					}

					else
					{
						
						this._AttendCreditId = default(int?);
						
					}

					this.SendPropertyChanged("AttendCredit");
				}

			}

		}

		
		[Association(Name="FK_MEETINGS_TBL_ORGANIZATIONS_TBL", Storage="_Organization", ThisKey="OrganizationId", IsForeignKey=true)]
		public Organization Organization
		{
			get { return this._Organization.Entity; }

			set
			{
				Organization previousValue = this._Organization.Entity;
				if (((previousValue != value) 
							|| (this._Organization.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Organization.Entity = null;
						previousValue.Meetings.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.Meetings.Add(this);
						
						this._OrganizationId = value.OrganizationId;
						
					}

					else
					{
						
						this._OrganizationId = default(int);
						
					}

					this.SendPropertyChanged("Organization");
				}

			}

		}

		
	#endregion
	
		public event PropertyChangingEventHandler PropertyChanging;
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
				this.PropertyChanging(this, emptyChangingEventArgs);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

   		
		private void attach_Attends(Attend entity)
		{
			this.SendPropertyChanging();
			entity.Meeting = this;
		}

		private void detach_Attends(Attend entity)
		{
			this.SendPropertyChanging();
			entity.Meeting = null;
		}

		
		private void attach_MeetingExtras(MeetingExtra entity)
		{
			this.SendPropertyChanging();
			entity.Meeting = this;
		}

		private void detach_MeetingExtras(MeetingExtra entity)
		{
			this.SendPropertyChanging();
			entity.Meeting = null;
		}

		
		private void attach_VolRequests(VolRequest entity)
		{
			this.SendPropertyChanging();
			entity.Meeting = this;
		}

		private void detach_VolRequests(VolRequest entity)
		{
			this.SendPropertyChanging();
			entity.Meeting = null;
		}

		
	}

}

