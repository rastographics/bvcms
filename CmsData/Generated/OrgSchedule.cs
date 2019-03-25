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
	[Table(Name="dbo.OrgSchedule")]
	public partial class OrgSchedule : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _OrganizationId;
		
		private int _Id;
		
		private int? _ScheduleId;
		
		private DateTime? _SchedTime;
		
		private int? _SchedDay;
		
		private DateTime? _MeetingTime;
		
		private int? _AttendCreditId;
		
		private DateTime? _NextMeetingDate;
		
   		
    	
		private EntityRef<Organization> _Organization;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnOrganizationIdChanging(int value);
		partial void OnOrganizationIdChanged();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnScheduleIdChanging(int? value);
		partial void OnScheduleIdChanged();
		
		partial void OnSchedTimeChanging(DateTime? value);
		partial void OnSchedTimeChanged();
		
		partial void OnSchedDayChanging(int? value);
		partial void OnSchedDayChanged();
		
		partial void OnMeetingTimeChanging(DateTime? value);
		partial void OnMeetingTimeChanged();
		
		partial void OnAttendCreditIdChanging(int? value);
		partial void OnAttendCreditIdChanged();
		
		partial void OnNextMeetingDateChanging(DateTime? value);
		partial void OnNextMeetingDateChanged();
		
    #endregion
		public OrgSchedule()
		{
			
			
			this._Organization = default(EntityRef<Organization>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="OrganizationId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationId", DbType="int NOT NULL", IsPrimaryKey=true)]
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

		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="int NOT NULL", IsPrimaryKey=true)]
		public int Id
		{
			get { return this._Id; }

			set
			{
				if (this._Id != value)
				{
				
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}

			}

		}

		
		[Column(Name="ScheduleId", UpdateCheck=UpdateCheck.Never, Storage="_ScheduleId", DbType="int")]
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

		
		[Column(Name="SchedTime", UpdateCheck=UpdateCheck.Never, Storage="_SchedTime", DbType="datetime")]
		public DateTime? SchedTime
		{
			get { return this._SchedTime; }

			set
			{
				if (this._SchedTime != value)
				{
				
                    this.OnSchedTimeChanging(value);
					this.SendPropertyChanging();
					this._SchedTime = value;
					this.SendPropertyChanged("SchedTime");
					this.OnSchedTimeChanged();
				}

			}

		}

		
		[Column(Name="SchedDay", UpdateCheck=UpdateCheck.Never, Storage="_SchedDay", DbType="int")]
		public int? SchedDay
		{
			get { return this._SchedDay; }

			set
			{
				if (this._SchedDay != value)
				{
				
                    this.OnSchedDayChanging(value);
					this.SendPropertyChanging();
					this._SchedDay = value;
					this.SendPropertyChanged("SchedDay");
					this.OnSchedDayChanged();
				}

			}

		}

		
		[Column(Name="MeetingTime", UpdateCheck=UpdateCheck.Never, Storage="_MeetingTime", DbType="datetime")]
		public DateTime? MeetingTime
		{
			get { return this._MeetingTime; }

			set
			{
				if (this._MeetingTime != value)
				{
				
                    this.OnMeetingTimeChanging(value);
					this.SendPropertyChanging();
					this._MeetingTime = value;
					this.SendPropertyChanged("MeetingTime");
					this.OnMeetingTimeChanged();
				}

			}

		}

		
		[Column(Name="AttendCreditId", UpdateCheck=UpdateCheck.Never, Storage="_AttendCreditId", DbType="int")]
		public int? AttendCreditId
		{
			get { return this._AttendCreditId; }

			set
			{
				if (this._AttendCreditId != value)
				{
				
                    this.OnAttendCreditIdChanging(value);
					this.SendPropertyChanging();
					this._AttendCreditId = value;
					this.SendPropertyChanged("AttendCreditId");
					this.OnAttendCreditIdChanged();
				}

			}

		}

		
		[Column(Name="NextMeetingDate", UpdateCheck=UpdateCheck.Never, Storage="_NextMeetingDate", DbType="datetime", IsDbGenerated=true)]
		public DateTime? NextMeetingDate
		{
			get { return this._NextMeetingDate; }

			set
			{
				if (this._NextMeetingDate != value)
				{
				
                    this.OnNextMeetingDateChanging(value);
					this.SendPropertyChanging();
					this._NextMeetingDate = value;
					this.SendPropertyChanged("NextMeetingDate");
					this.OnNextMeetingDateChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_OrgSchedule_Organizations", Storage="_Organization", ThisKey="OrganizationId", IsForeignKey=true)]
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
						previousValue.OrgSchedules.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.OrgSchedules.Add(this);
						
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

   		
	}

}

