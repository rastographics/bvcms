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
	[Table(Name="dbo.MemberTags")]
	public partial class MemberTag : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Name;
		
		private int? _OrgId;
		
		private string _VolFrequency;
		
		private DateTime? _VolStartDate;
		
		private DateTime? _VolEndDate;
		
		private int? _NoCancelWeeks;
		
		private bool _CheckIn;

        private bool _CheckInOpen;

        private int _CheckInCapacity;

        private bool _CheckInOpenDefault;

        private int _CheckInCapacityDefault;
		
		private int? _ScheduleId;


        private EntitySet<OrgMemMemTag> _OrgMemMemTags;
		
    	
		private EntityRef<Organization> _Organization;
		
    	
		private EntityRef<OrgSchedule> _Schedule;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnOrgIdChanging(int? value);
		partial void OnOrgIdChanged();
		
		partial void OnVolFrequencyChanging(string value);
		partial void OnVolFrequencyChanged();
		
		partial void OnVolStartDateChanging(DateTime? value);
		partial void OnVolStartDateChanged();
		
		partial void OnVolEndDateChanging(DateTime? value);
		partial void OnVolEndDateChanged();
		
		partial void OnNoCancelWeeksChanging(int? value);
		partial void OnNoCancelWeeksChanged();
		
		partial void OnCheckInChanging(bool value);
		partial void OnCheckInChanged();

        partial void OnCheckInOpenChanging(bool value);
        partial void OnCheckInOpenChanged();

        partial void OnCheckInCapacityChanging(int value);
        partial void OnCheckInCapacityChanged();

        partial void OnCheckInOpenDefaultChanging(bool value);
        partial void OnCheckInOpenDefaultChanged();

        partial void OnCheckInCapacityDefaultChanging(int value);
        partial void OnCheckInCapacityDefaultChanged();

        partial void OnScheduleIdChanging(int? value);
        partial void OnScheduleIdChanged();

        #endregion
        public MemberTag()
		{
			
			this._OrgMemMemTags = new EntitySet<OrgMemMemTag>(new Action< OrgMemMemTag>(this.attach_OrgMemMemTags), new Action< OrgMemMemTag>(this.detach_OrgMemMemTags)); 
			
			
			this._Organization = default(EntityRef<Organization>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
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

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(200)")]
		public string Name
		{
			get { return this._Name; }

			set
			{
				if (this._Name != value)
				{
				
                    this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}

			}

		}

		
		[Column(Name="OrgId", UpdateCheck=UpdateCheck.Never, Storage="_OrgId", DbType="int")]
		[IsForeignKey]
		public int? OrgId
		{
			get { return this._OrgId; }

			set
			{
				if (this._OrgId != value)
				{
				
					if (this._Organization.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrgIdChanging(value);
					this.SendPropertyChanging();
					this._OrgId = value;
					this.SendPropertyChanged("OrgId");
					this.OnOrgIdChanged();
				}

			}

		}

		
		[Column(Name="VolFrequency", UpdateCheck=UpdateCheck.Never, Storage="_VolFrequency", DbType="nvarchar(2)")]
		public string VolFrequency
		{
			get { return this._VolFrequency; }

			set
			{
				if (this._VolFrequency != value)
				{
				
                    this.OnVolFrequencyChanging(value);
					this.SendPropertyChanging();
					this._VolFrequency = value;
					this.SendPropertyChanged("VolFrequency");
					this.OnVolFrequencyChanged();
				}

			}

		}

		
		[Column(Name="VolStartDate", UpdateCheck=UpdateCheck.Never, Storage="_VolStartDate", DbType="datetime")]
		public DateTime? VolStartDate
		{
			get { return this._VolStartDate; }

			set
			{
				if (this._VolStartDate != value)
				{
				
                    this.OnVolStartDateChanging(value);
					this.SendPropertyChanging();
					this._VolStartDate = value;
					this.SendPropertyChanged("VolStartDate");
					this.OnVolStartDateChanged();
				}

			}

		}

		
		[Column(Name="VolEndDate", UpdateCheck=UpdateCheck.Never, Storage="_VolEndDate", DbType="datetime")]
		public DateTime? VolEndDate
		{
			get { return this._VolEndDate; }

			set
			{
				if (this._VolEndDate != value)
				{
				
                    this.OnVolEndDateChanging(value);
					this.SendPropertyChanging();
					this._VolEndDate = value;
					this.SendPropertyChanged("VolEndDate");
					this.OnVolEndDateChanged();
				}

			}

		}

		
		[Column(Name="NoCancelWeeks", UpdateCheck=UpdateCheck.Never, Storage="_NoCancelWeeks", DbType="int")]
		public int? NoCancelWeeks
		{
			get { return this._NoCancelWeeks; }

			set
			{
				if (this._NoCancelWeeks != value)
				{
				
                    this.OnNoCancelWeeksChanging(value);
					this.SendPropertyChanging();
					this._NoCancelWeeks = value;
					this.SendPropertyChanged("NoCancelWeeks");
					this.OnNoCancelWeeksChanged();
				}

			}

		}

		
		[Column(Name="CheckIn", UpdateCheck=UpdateCheck.Never, Storage="_CheckIn", DbType="bit NOT NULL")]
		public bool CheckIn
		{
			get { return this._CheckIn; }

			set
			{
				if (this._CheckIn != value)
				{
				
                    this.OnCheckInChanging(value);
					this.SendPropertyChanging();
					this._CheckIn = value;
					this.SendPropertyChanged("CheckIn");
					this.OnCheckInChanged();
				}

			}

		}

		
		[Column(Name="CheckInOpen", UpdateCheck=UpdateCheck.Never, Storage="_CheckInOpen", DbType="bit NOT NULL")]
		public bool CheckInOpen
		{
			get { return this._CheckInOpen; }

			set
			{
				if (this._CheckInOpen != value)
				{
				
                    this.OnCheckInOpenChanging(value);
					this.SendPropertyChanging();
					this._CheckInOpen = value;
					this.SendPropertyChanged("CheckInOpen");
					this.OnCheckInOpenChanged();
				}

			}

		}

		
		[Column(Name="CheckInCapacity", UpdateCheck=UpdateCheck.Never, Storage="_CheckInCapacity", DbType="int NOT NULL")]
		public int CheckInCapacity
		{
			get { return this._CheckInCapacity; }

			set
			{
				if (this._CheckInCapacity != value)
				{
				
                    this.OnCheckInCapacityChanging(value);
					this.SendPropertyChanging();
					this._CheckInCapacity = value;
					this.SendPropertyChanged("CheckInCapacity");
					this.OnCheckInCapacityChanged();
				}

			}

		}



        [Column(Name = "CheckInOpenDefault", UpdateCheck = UpdateCheck.Never, Storage = "_CheckInOpenDefault", DbType = "bit NOT NULL")]
        public bool CheckInOpenDefault
        {
            get { return this._CheckInOpenDefault; }

            set
            {
                if (this._CheckInOpenDefault != value)
                {

                    this.OnCheckInOpenDefaultChanging(value);
                    this.SendPropertyChanging();
                    this._CheckInOpenDefault = value;
                    this.SendPropertyChanged("CheckInOpenDefault");
                    this.OnCheckInOpenDefaultChanged();
                }

            }

        }


        [Column(Name = "CheckInCapacityDefault", UpdateCheck = UpdateCheck.Never, Storage = "_CheckInCapacityDefault", DbType = "int NOT NULL")]
        public int CheckInCapacityDefault
        {
            get { return this._CheckInCapacityDefault; }

            set
            {
                if (this._CheckInCapacityDefault != value)
                {

                    this.OnCheckInCapacityDefaultChanging(value);
                    this.SendPropertyChanging();
                    this._CheckInCapacityDefault = value;
                    this.SendPropertyChanged("CheckInCapacityDefault");
                    this.OnCheckInCapacityDefaultChanged();
                }

            }

        }


        [Column(Name = "ScheduleId", UpdateCheck = UpdateCheck.Never, Storage = "_ScheduleId", DbType = "int")]
        [IsForeignKey]
        public int? ScheduleId
        {
            get { return this._ScheduleId; }

            set
            {
                if (this._ScheduleId != value)
                {

                    if (this._Organization.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnScheduleIdChanging(value);
                    this.SendPropertyChanging();
                    this._ScheduleId = value;
                    this.SendPropertyChanged("ScheduleId");
                    this.OnScheduleIdChanged();
                }

            }

        }


        #endregion

        #region Foreign Key Tables

        [Association(Name="FK_OrgMemMemTags_MemberTags", Storage="_OrgMemMemTags", OtherKey="MemberTagId")]
   		public EntitySet<OrgMemMemTag> OrgMemMemTags
   		{
   		    get { return this._OrgMemMemTags; }

			set	{ this._OrgMemMemTags.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_MemberTags_Organizations", Storage="_Organization", ThisKey="OrgId", IsForeignKey=true)]
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
						previousValue.MemberTags.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.MemberTags.Add(this);
						
						this._OrgId = value.OrganizationId;
						
					}

					else
					{
						
						this._OrgId = default(int?);
						
					}

					this.SendPropertyChanged("Organization");
				}

			}

        }

        [Association(Name = "FK_MemberTags_OrgSchedule", Storage = "_Schedule", ThisKey = "OrgId,ScheduleId", IsForeignKey = true)]
        public OrgSchedule Schedule
        {
            get { return this._Schedule.Entity; }

            set
            {
                OrgSchedule previousValue = this._Schedule.Entity;
                if (((previousValue != value)
                            || (this._Schedule.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();

                    this._Schedule.Entity = value;
                    if (value != null)
                    {
                        this._ScheduleId = value.ScheduleId;
                    }

                    else
                    {

                        this._ScheduleId = default(int?);

                    }

                    this.SendPropertyChanged("Schedule");
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

   		
		private void attach_OrgMemMemTags(OrgMemMemTag entity)
		{
			this.SendPropertyChanging();
			entity.MemberTag = this;
		}

		private void detach_OrgMemMemTags(OrgMemMemTag entity)
		{
			this.SendPropertyChanging();
			entity.MemberTag = null;
		}

		
	}

}

