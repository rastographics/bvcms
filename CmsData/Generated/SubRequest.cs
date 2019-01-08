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
	[Table(Name="dbo.SubRequest")]
	public partial class SubRequest : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _AttendId;
		
		private int _RequestorId;
		
		private DateTime _Requested;
		
		private int _SubstituteId;
		
		private DateTime? _Responded;
		
		private bool? _CanSub;
		
   		
    	
		private EntityRef<Attend> _Attend;
		
		private EntityRef<Person> _Requestor;
		
		private EntityRef<Person> _Substitute;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnAttendIdChanging(int value);
		partial void OnAttendIdChanged();
		
		partial void OnRequestorIdChanging(int value);
		partial void OnRequestorIdChanged();
		
		partial void OnRequestedChanging(DateTime value);
		partial void OnRequestedChanged();
		
		partial void OnSubstituteIdChanging(int value);
		partial void OnSubstituteIdChanged();
		
		partial void OnRespondedChanging(DateTime? value);
		partial void OnRespondedChanged();
		
		partial void OnCanSubChanging(bool? value);
		partial void OnCanSubChanged();
		
    #endregion
		public SubRequest()
		{
			
			
			this._Attend = default(EntityRef<Attend>); 
			
			this._Requestor = default(EntityRef<Person>); 
			
			this._Substitute = default(EntityRef<Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="AttendId", UpdateCheck=UpdateCheck.Never, Storage="_AttendId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int AttendId
		{
			get { return this._AttendId; }

			set
			{
				if (this._AttendId != value)
				{
				
					if (this._Attend.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnAttendIdChanging(value);
					this.SendPropertyChanging();
					this._AttendId = value;
					this.SendPropertyChanged("AttendId");
					this.OnAttendIdChanged();
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

		
		[Column(Name="SubstituteId", UpdateCheck=UpdateCheck.Never, Storage="_SubstituteId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int SubstituteId
		{
			get { return this._SubstituteId; }

			set
			{
				if (this._SubstituteId != value)
				{
				
					if (this._Substitute.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnSubstituteIdChanging(value);
					this.SendPropertyChanging();
					this._SubstituteId = value;
					this.SendPropertyChanged("SubstituteId");
					this.OnSubstituteIdChanged();
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

		
		[Column(Name="CanSub", UpdateCheck=UpdateCheck.Never, Storage="_CanSub", DbType="bit")]
		public bool? CanSub
		{
			get { return this._CanSub; }

			set
			{
				if (this._CanSub != value)
				{
				
                    this.OnCanSubChanging(value);
					this.SendPropertyChanging();
					this._CanSub = value;
					this.SendPropertyChanged("CanSub");
					this.OnCanSubChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="SubRequests__Attend", Storage="_Attend", ThisKey="AttendId", IsForeignKey=true)]
		public Attend Attend
		{
			get { return this._Attend.Entity; }

			set
			{
				Attend previousValue = this._Attend.Entity;
				if (((previousValue != value) 
							|| (this._Attend.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Attend.Entity = null;
						previousValue.SubRequests.Remove(this);
					}

					this._Attend.Entity = value;
					if (value != null)
					{
						value.SubRequests.Add(this);
						
						this._AttendId = value.AttendId;
						
					}

					else
					{
						
						this._AttendId = default(int);
						
					}

					this.SendPropertyChanged("Attend");
				}

			}

		}

		
		[Association(Name="SubRequests__Requestor", Storage="_Requestor", ThisKey="RequestorId", IsForeignKey=true)]
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
						previousValue.SubRequests.Remove(this);
					}

					this._Requestor.Entity = value;
					if (value != null)
					{
						value.SubRequests.Add(this);
						
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

		
		[Association(Name="SubResponses__Substitute", Storage="_Substitute", ThisKey="SubstituteId", IsForeignKey=true)]
		public Person Substitute
		{
			get { return this._Substitute.Entity; }

			set
			{
				Person previousValue = this._Substitute.Entity;
				if (((previousValue != value) 
							|| (this._Substitute.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Substitute.Entity = null;
						previousValue.SubResponses.Remove(this);
					}

					this._Substitute.Entity = value;
					if (value != null)
					{
						value.SubResponses.Add(this);
						
						this._SubstituteId = value.PeopleId;
						
					}

					else
					{
						
						this._SubstituteId = default(int);
						
					}

					this.SendPropertyChanged("Substitute");
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

