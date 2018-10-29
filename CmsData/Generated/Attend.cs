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
	[Table(Name="dbo.Attend")]
	public partial class Attend : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private int _MeetingId;
		
		private int _OrganizationId;
		
		private DateTime _MeetingDate;
		
		private bool _AttendanceFlag;
		
		private int? _OtherOrgId;
		
		private int? _AttendanceTypeId;
		
		private int? _CreatedBy;
		
		private DateTime? _CreatedDate;
		
		private int _MemberTypeId;
		
		private int _AttendId;
		
		private int _OtherAttends;
		
		private bool? _BFCAttendance;
		
		private bool? _Registered;
		
		private int? _SeqNo;
		
		private int? _Commitment;
		
		private bool? _NoShow;
		
		private bool? _EffAttendFlag;
		
		private int _SubGroupID;
		
		private string _SubGroupName;
		
		private string _Pager;
		
   		
   		private EntitySet<SubRequest> _SubRequests;
		
    	
		private EntityRef<MemberType> _MemberType;
		
		private EntityRef<AttendType> _AttendType;
		
		private EntityRef<Meeting> _Meeting;
		
		private EntityRef<Organization> _Organization;
		
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnMeetingIdChanging(int value);
		partial void OnMeetingIdChanged();
		
		partial void OnOrganizationIdChanging(int value);
		partial void OnOrganizationIdChanged();
		
		partial void OnMeetingDateChanging(DateTime value);
		partial void OnMeetingDateChanged();
		
		partial void OnAttendanceFlagChanging(bool value);
		partial void OnAttendanceFlagChanged();
		
		partial void OnOtherOrgIdChanging(int? value);
		partial void OnOtherOrgIdChanged();
		
		partial void OnAttendanceTypeIdChanging(int? value);
		partial void OnAttendanceTypeIdChanged();
		
		partial void OnCreatedByChanging(int? value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime? value);
		partial void OnCreatedDateChanged();
		
		partial void OnMemberTypeIdChanging(int value);
		partial void OnMemberTypeIdChanged();
		
		partial void OnAttendIdChanging(int value);
		partial void OnAttendIdChanged();
		
		partial void OnOtherAttendsChanging(int value);
		partial void OnOtherAttendsChanged();
		
		partial void OnBFCAttendanceChanging(bool? value);
		partial void OnBFCAttendanceChanged();
		
		partial void OnRegisteredChanging(bool? value);
		partial void OnRegisteredChanged();
		
		partial void OnSeqNoChanging(int? value);
		partial void OnSeqNoChanged();
		
		partial void OnCommitmentChanging(int? value);
		partial void OnCommitmentChanged();
		
		partial void OnNoShowChanging(bool? value);
		partial void OnNoShowChanged();
		
		partial void OnEffAttendFlagChanging(bool? value);
		partial void OnEffAttendFlagChanged();
		
		partial void OnSubGroupIDChanging(int value);
		partial void OnSubGroupIDChanged();
		
		partial void OnSubGroupNameChanging(string value);
		partial void OnSubGroupNameChanged();
		
		partial void OnPagerChanging(string value);
		partial void OnPagerChanged();
		
    #endregion
		public Attend()
		{
			
			this._SubRequests = new EntitySet<SubRequest>(new Action< SubRequest>(this.attach_SubRequests), new Action< SubRequest>(this.detach_SubRequests)); 
			
			
			this._MemberType = default(EntityRef<MemberType>); 
			
			this._AttendType = default(EntityRef<AttendType>); 
			
			this._Meeting = default(EntityRef<Meeting>); 
			
			this._Organization = default(EntityRef<Organization>); 
			
			this._Person = default(EntityRef<Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="MeetingId", UpdateCheck=UpdateCheck.Never, Storage="_MeetingId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int MeetingId
		{
			get { return this._MeetingId; }

			set
			{
				if (this._MeetingId != value)
				{
				
					if (this._Meeting.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnMeetingIdChanging(value);
					this.SendPropertyChanging();
					this._MeetingId = value;
					this.SendPropertyChanged("MeetingId");
					this.OnMeetingIdChanged();
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

		
		[Column(Name="MeetingDate", UpdateCheck=UpdateCheck.Never, Storage="_MeetingDate", DbType="datetime NOT NULL")]
		public DateTime MeetingDate
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

		
		[Column(Name="AttendanceFlag", UpdateCheck=UpdateCheck.Never, Storage="_AttendanceFlag", DbType="bit NOT NULL")]
		public bool AttendanceFlag
		{
			get { return this._AttendanceFlag; }

			set
			{
				if (this._AttendanceFlag != value)
				{
				
                    this.OnAttendanceFlagChanging(value);
					this.SendPropertyChanging();
					this._AttendanceFlag = value;
					this.SendPropertyChanged("AttendanceFlag");
					this.OnAttendanceFlagChanged();
				}

			}

		}

		
		[Column(Name="OtherOrgId", UpdateCheck=UpdateCheck.Never, Storage="_OtherOrgId", DbType="int")]
		public int? OtherOrgId
		{
			get { return this._OtherOrgId; }

			set
			{
				if (this._OtherOrgId != value)
				{
				
                    this.OnOtherOrgIdChanging(value);
					this.SendPropertyChanging();
					this._OtherOrgId = value;
					this.SendPropertyChanged("OtherOrgId");
					this.OnOtherOrgIdChanged();
				}

			}

		}

		
		[Column(Name="AttendanceTypeId", UpdateCheck=UpdateCheck.Never, Storage="_AttendanceTypeId", DbType="int")]
		[IsForeignKey]
		public int? AttendanceTypeId
		{
			get { return this._AttendanceTypeId; }

			set
			{
				if (this._AttendanceTypeId != value)
				{
				
					if (this._AttendType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnAttendanceTypeIdChanging(value);
					this.SendPropertyChanging();
					this._AttendanceTypeId = value;
					this.SendPropertyChanged("AttendanceTypeId");
					this.OnAttendanceTypeIdChanged();
				}

			}

		}

		
		[Column(Name="CreatedBy", UpdateCheck=UpdateCheck.Never, Storage="_CreatedBy", DbType="int")]
		public int? CreatedBy
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

		
		[Column(Name="CreatedDate", UpdateCheck=UpdateCheck.Never, Storage="_CreatedDate", DbType="datetime")]
		public DateTime? CreatedDate
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

		
		[Column(Name="MemberTypeId", UpdateCheck=UpdateCheck.Never, Storage="_MemberTypeId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int MemberTypeId
		{
			get { return this._MemberTypeId; }

			set
			{
				if (this._MemberTypeId != value)
				{
				
					if (this._MemberType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnMemberTypeIdChanging(value);
					this.SendPropertyChanging();
					this._MemberTypeId = value;
					this.SendPropertyChanged("MemberTypeId");
					this.OnMemberTypeIdChanged();
				}

			}

		}

		
		[Column(Name="AttendId", UpdateCheck=UpdateCheck.Never, Storage="_AttendId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int AttendId
		{
			get { return this._AttendId; }

			set
			{
				if (this._AttendId != value)
				{
				
                    this.OnAttendIdChanging(value);
					this.SendPropertyChanging();
					this._AttendId = value;
					this.SendPropertyChanged("AttendId");
					this.OnAttendIdChanged();
				}

			}

		}

		
		[Column(Name="OtherAttends", UpdateCheck=UpdateCheck.Never, Storage="_OtherAttends", DbType="int NOT NULL")]
		public int OtherAttends
		{
			get { return this._OtherAttends; }

			set
			{
				if (this._OtherAttends != value)
				{
				
                    this.OnOtherAttendsChanging(value);
					this.SendPropertyChanging();
					this._OtherAttends = value;
					this.SendPropertyChanged("OtherAttends");
					this.OnOtherAttendsChanged();
				}

			}

		}

		
		[Column(Name="BFCAttendance", UpdateCheck=UpdateCheck.Never, Storage="_BFCAttendance", DbType="bit")]
		public bool? BFCAttendance
		{
			get { return this._BFCAttendance; }

			set
			{
				if (this._BFCAttendance != value)
				{
				
                    this.OnBFCAttendanceChanging(value);
					this.SendPropertyChanging();
					this._BFCAttendance = value;
					this.SendPropertyChanged("BFCAttendance");
					this.OnBFCAttendanceChanged();
				}

			}

		}

		
		[Column(Name="Registered", UpdateCheck=UpdateCheck.Never, Storage="_Registered", DbType="bit")]
		public bool? Registered
		{
			get { return this._Registered; }

			set
			{
				if (this._Registered != value)
				{
				
                    this.OnRegisteredChanging(value);
					this.SendPropertyChanging();
					this._Registered = value;
					this.SendPropertyChanged("Registered");
					this.OnRegisteredChanged();
				}

			}

		}

		
		[Column(Name="SeqNo", UpdateCheck=UpdateCheck.Never, Storage="_SeqNo", DbType="int")]
		public int? SeqNo
		{
			get { return this._SeqNo; }

			set
			{
				if (this._SeqNo != value)
				{
				
                    this.OnSeqNoChanging(value);
					this.SendPropertyChanging();
					this._SeqNo = value;
					this.SendPropertyChanged("SeqNo");
					this.OnSeqNoChanged();
				}

			}

		}

		
		[Column(Name="Commitment", UpdateCheck=UpdateCheck.Never, Storage="_Commitment", DbType="int")]
		public int? Commitment
		{
			get { return this._Commitment; }

			set
			{
				if (this._Commitment != value)
				{
				
                    this.OnCommitmentChanging(value);
					this.SendPropertyChanging();
					this._Commitment = value;
					this.SendPropertyChanged("Commitment");
					this.OnCommitmentChanged();
				}

			}

		}

		
		[Column(Name="NoShow", UpdateCheck=UpdateCheck.Never, Storage="_NoShow", DbType="bit")]
		public bool? NoShow
		{
			get { return this._NoShow; }

			set
			{
				if (this._NoShow != value)
				{
				
                    this.OnNoShowChanging(value);
					this.SendPropertyChanging();
					this._NoShow = value;
					this.SendPropertyChanged("NoShow");
					this.OnNoShowChanged();
				}

			}

		}

		
		[Column(Name="EffAttendFlag", UpdateCheck=UpdateCheck.Never, Storage="_EffAttendFlag", DbType="bit", IsDbGenerated=true)]
		public bool? EffAttendFlag
		{
			get { return this._EffAttendFlag; }

			set
			{
				if (this._EffAttendFlag != value)
				{
				
                    this.OnEffAttendFlagChanging(value);
					this.SendPropertyChanging();
					this._EffAttendFlag = value;
					this.SendPropertyChanged("EffAttendFlag");
					this.OnEffAttendFlagChanged();
				}

			}

		}

		
		[Column(Name="SubGroupID", UpdateCheck=UpdateCheck.Never, Storage="_SubGroupID", DbType="int NOT NULL")]
		public int SubGroupID
		{
			get { return this._SubGroupID; }

			set
			{
				if (this._SubGroupID != value)
				{
				
                    this.OnSubGroupIDChanging(value);
					this.SendPropertyChanging();
					this._SubGroupID = value;
					this.SendPropertyChanged("SubGroupID");
					this.OnSubGroupIDChanged();
				}

			}

		}

		
		[Column(Name="SubGroupName", UpdateCheck=UpdateCheck.Never, Storage="_SubGroupName", DbType="nvarchar(200) NOT NULL")]
		public string SubGroupName
		{
			get { return this._SubGroupName; }

			set
			{
				if (this._SubGroupName != value)
				{
				
                    this.OnSubGroupNameChanging(value);
					this.SendPropertyChanging();
					this._SubGroupName = value;
					this.SendPropertyChanged("SubGroupName");
					this.OnSubGroupNameChanged();
				}

			}

		}

		
		[Column(Name="Pager", UpdateCheck=UpdateCheck.Never, Storage="_Pager", DbType="nvarchar(20) NOT NULL")]
		public string Pager
		{
			get { return this._Pager; }

			set
			{
				if (this._Pager != value)
				{
				
                    this.OnPagerChanging(value);
					this.SendPropertyChanging();
					this._Pager = value;
					this.SendPropertyChanged("Pager");
					this.OnPagerChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="SubRequests__Attend", Storage="_SubRequests", OtherKey="AttendId")]
   		public EntitySet<SubRequest> SubRequests
   		{
   		    get { return this._SubRequests; }

			set	{ this._SubRequests.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_Attend_MemberType", Storage="_MemberType", ThisKey="MemberTypeId", IsForeignKey=true)]
		public MemberType MemberType
		{
			get { return this._MemberType.Entity; }

			set
			{
				MemberType previousValue = this._MemberType.Entity;
				if (((previousValue != value) 
							|| (this._MemberType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._MemberType.Entity = null;
						previousValue.Attends.Remove(this);
					}

					this._MemberType.Entity = value;
					if (value != null)
					{
						value.Attends.Add(this);
						
						this._MemberTypeId = value.Id;
						
					}

					else
					{
						
						this._MemberTypeId = default(int);
						
					}

					this.SendPropertyChanged("MemberType");
				}

			}

		}

		
		[Association(Name="FK_AttendWithAbsents_TBL_AttendType", Storage="_AttendType", ThisKey="AttendanceTypeId", IsForeignKey=true)]
		public AttendType AttendType
		{
			get { return this._AttendType.Entity; }

			set
			{
				AttendType previousValue = this._AttendType.Entity;
				if (((previousValue != value) 
							|| (this._AttendType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._AttendType.Entity = null;
						previousValue.Attends.Remove(this);
					}

					this._AttendType.Entity = value;
					if (value != null)
					{
						value.Attends.Add(this);
						
						this._AttendanceTypeId = value.Id;
						
					}

					else
					{
						
						this._AttendanceTypeId = default(int?);
						
					}

					this.SendPropertyChanged("AttendType");
				}

			}

		}

		
		[Association(Name="FK_AttendWithAbsents_TBL_MEETINGS_TBL", Storage="_Meeting", ThisKey="MeetingId", IsForeignKey=true)]
		public Meeting Meeting
		{
			get { return this._Meeting.Entity; }

			set
			{
				Meeting previousValue = this._Meeting.Entity;
				if (((previousValue != value) 
							|| (this._Meeting.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Meeting.Entity = null;
						previousValue.Attends.Remove(this);
					}

					this._Meeting.Entity = value;
					if (value != null)
					{
						value.Attends.Add(this);
						
						this._MeetingId = value.MeetingId;
						
					}

					else
					{
						
						this._MeetingId = default(int);
						
					}

					this.SendPropertyChanged("Meeting");
				}

			}

		}

		
		[Association(Name="FK_AttendWithAbsents_TBL_ORGANIZATIONS_TBL", Storage="_Organization", ThisKey="OrganizationId", IsForeignKey=true)]
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
						previousValue.Attends.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.Attends.Add(this);
						
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

		
		[Association(Name="FK_AttendWithAbsents_TBL_PEOPLE_TBL", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
		public Person Person
		{
			get { return this._Person.Entity; }

			set
			{
				Person previousValue = this._Person.Entity;
				if (((previousValue != value) 
							|| (this._Person.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Person.Entity = null;
						previousValue.Attends.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.Attends.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int);
						
					}

					this.SendPropertyChanged("Person");
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

   		
		private void attach_SubRequests(SubRequest entity)
		{
			this.SendPropertyChanging();
			entity.Attend = this;
		}

		private void detach_SubRequests(SubRequest entity)
		{
			this.SendPropertyChanging();
			entity.Attend = null;
		}

		
	}

}

