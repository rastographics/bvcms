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
	[Table(Name="dbo.VoluteerApprovalIds")]
	public partial class VoluteerApprovalId : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private int _ApprovalId;
		
   		
    	
		private EntityRef<Person> _Person;
		
		private EntityRef<Volunteer> _Volunteer;
		
		private EntityRef<VolunteerCode> _VolunteerCode;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnApprovalIdChanging(int value);
		partial void OnApprovalIdChanged();
		
    #endregion
		public VoluteerApprovalId()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
			this._Volunteer = default(EntityRef<Volunteer>); 
			
			this._VolunteerCode = default(EntityRef<VolunteerCode>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._Volunteer.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="ApprovalId", UpdateCheck=UpdateCheck.Never, Storage="_ApprovalId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int ApprovalId
		{
			get { return this._ApprovalId; }

			set
			{
				if (this._ApprovalId != value)
				{
				
					if (this._VolunteerCode.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnApprovalIdChanging(value);
					this.SendPropertyChanging();
					this._ApprovalId = value;
					this.SendPropertyChanged("ApprovalId");
					this.OnApprovalIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_VoluteerApprovalIds_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.VoluteerApprovalIds.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.VoluteerApprovalIds.Add(this);
						
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

		
		[Association(Name="FK_VoluteerApprovalIds_Volunteer", Storage="_Volunteer", ThisKey="PeopleId", IsForeignKey=true)]
		public Volunteer Volunteer
		{
			get { return this._Volunteer.Entity; }

			set
			{
				Volunteer previousValue = this._Volunteer.Entity;
				if (((previousValue != value) 
							|| (this._Volunteer.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Volunteer.Entity = null;
						previousValue.VoluteerApprovalIds.Remove(this);
					}

					this._Volunteer.Entity = value;
					if (value != null)
					{
						value.VoluteerApprovalIds.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int);
						
					}

					this.SendPropertyChanged("Volunteer");
				}

			}

		}

		
		[Association(Name="FK_VoluteerApprovalIds_VolunteerCodes", Storage="_VolunteerCode", ThisKey="ApprovalId", IsForeignKey=true)]
		public VolunteerCode VolunteerCode
		{
			get { return this._VolunteerCode.Entity; }

			set
			{
				VolunteerCode previousValue = this._VolunteerCode.Entity;
				if (((previousValue != value) 
							|| (this._VolunteerCode.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._VolunteerCode.Entity = null;
						previousValue.VoluteerApprovalIds.Remove(this);
					}

					this._VolunteerCode.Entity = value;
					if (value != null)
					{
						value.VoluteerApprovalIds.Add(this);
						
						this._ApprovalId = value.Id;
						
					}

					else
					{
						
						this._ApprovalId = default(int);
						
					}

					this.SendPropertyChanged("VolunteerCode");
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

