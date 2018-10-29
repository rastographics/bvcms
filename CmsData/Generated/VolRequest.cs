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
	[Table(Name="dbo.VolRequest")]
	public partial class VolRequest : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _MeetingId;
		
		private int _RequestorId;
		
		private DateTime _Requested;
		
		private int _VolunteerId;
		
		private DateTime? _Responded;
		
		private bool? _CanVol;
		
   		
    	
		private EntityRef<Meeting> _Meeting;
		
		private EntityRef<Person> _Requestor;
		
		private EntityRef<Person> _Volunteer;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnMeetingIdChanging(int value);
		partial void OnMeetingIdChanged();
		
		partial void OnRequestorIdChanging(int value);
		partial void OnRequestorIdChanged();
		
		partial void OnRequestedChanging(DateTime value);
		partial void OnRequestedChanged();
		
		partial void OnVolunteerIdChanging(int value);
		partial void OnVolunteerIdChanged();
		
		partial void OnRespondedChanging(DateTime? value);
		partial void OnRespondedChanged();
		
		partial void OnCanVolChanging(bool? value);
		partial void OnCanVolChanged();
		
    #endregion
		public VolRequest()
		{
			
			
			this._Meeting = default(EntityRef<Meeting>); 
			
			this._Requestor = default(EntityRef<Person>); 
			
			this._Volunteer = default(EntityRef<Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="MeetingId", UpdateCheck=UpdateCheck.Never, Storage="_MeetingId", DbType="int NOT NULL", IsPrimaryKey=true)]
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

		
		[Column(Name="RequestorId", UpdateCheck=UpdateCheck.Never, Storage="_RequestorId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int RequestorId
		{
			get { return this._RequestorId; }

			set
			{
				if (this._RequestorId != value)
				{
				
					if (this._Requestor.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnRequestorIdChanging(value);
					this.SendPropertyChanging();
					this._RequestorId = value;
					this.SendPropertyChanged("RequestorId");
					this.OnRequestorIdChanged();
				}

			}

		}

		
		[Column(Name="Requested", UpdateCheck=UpdateCheck.Never, Storage="_Requested", DbType="datetime NOT NULL", IsPrimaryKey=true)]
		public DateTime Requested
		{
			get { return this._Requested; }

			set
			{
				if (this._Requested != value)
				{
				
                    this.OnRequestedChanging(value);
					this.SendPropertyChanging();
					this._Requested = value;
					this.SendPropertyChanged("Requested");
					this.OnRequestedChanged();
				}

			}

		}

		
		[Column(Name="VolunteerId", UpdateCheck=UpdateCheck.Never, Storage="_VolunteerId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int VolunteerId
		{
			get { return this._VolunteerId; }

			set
			{
				if (this._VolunteerId != value)
				{
				
					if (this._Volunteer.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnVolunteerIdChanging(value);
					this.SendPropertyChanging();
					this._VolunteerId = value;
					this.SendPropertyChanged("VolunteerId");
					this.OnVolunteerIdChanged();
				}

			}

		}

		
		[Column(Name="Responded", UpdateCheck=UpdateCheck.Never, Storage="_Responded", DbType="datetime")]
		public DateTime? Responded
		{
			get { return this._Responded; }

			set
			{
				if (this._Responded != value)
				{
				
                    this.OnRespondedChanging(value);
					this.SendPropertyChanging();
					this._Responded = value;
					this.SendPropertyChanged("Responded");
					this.OnRespondedChanged();
				}

			}

		}

		
		[Column(Name="CanVol", UpdateCheck=UpdateCheck.Never, Storage="_CanVol", DbType="bit")]
		public bool? CanVol
		{
			get { return this._CanVol; }

			set
			{
				if (this._CanVol != value)
				{
				
                    this.OnCanVolChanging(value);
					this.SendPropertyChanging();
					this._CanVol = value;
					this.SendPropertyChanged("CanVol");
					this.OnCanVolChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="VolRequests__Meeting", Storage="_Meeting", ThisKey="MeetingId", IsForeignKey=true)]
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
						previousValue.VolRequests.Remove(this);
					}

					this._Meeting.Entity = value;
					if (value != null)
					{
						value.VolRequests.Add(this);
						
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

		
		[Association(Name="VolRequests__Requestor", Storage="_Requestor", ThisKey="RequestorId", IsForeignKey=true)]
		public Person Requestor
		{
			get { return this._Requestor.Entity; }

			set
			{
				Person previousValue = this._Requestor.Entity;
				if (((previousValue != value) 
							|| (this._Requestor.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Requestor.Entity = null;
						previousValue.VolRequests.Remove(this);
					}

					this._Requestor.Entity = value;
					if (value != null)
					{
						value.VolRequests.Add(this);
						
						this._RequestorId = value.PeopleId;
						
					}

					else
					{
						
						this._RequestorId = default(int);
						
					}

					this.SendPropertyChanged("Requestor");
				}

			}

		}

		
		[Association(Name="VolResponses__Volunteer", Storage="_Volunteer", ThisKey="VolunteerId", IsForeignKey=true)]
		public Person Volunteer
		{
			get { return this._Volunteer.Entity; }

			set
			{
				Person previousValue = this._Volunteer.Entity;
				if (((previousValue != value) 
							|| (this._Volunteer.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Volunteer.Entity = null;
						previousValue.VolResponses.Remove(this);
					}

					this._Volunteer.Entity = value;
					if (value != null)
					{
						value.VolResponses.Add(this);
						
						this._VolunteerId = value.PeopleId;
						
					}

					else
					{
						
						this._VolunteerId = default(int);
						
					}

					this.SendPropertyChanged("Volunteer");
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

