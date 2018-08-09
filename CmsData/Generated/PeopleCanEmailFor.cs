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
	[Table(Name="dbo.PeopleCanEmailFor")]
	public partial class PeopleCanEmailFor : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _CanEmail;
		
		private int _OnBehalfOf;
		
   		
    	
		private EntityRef<Person> _PersonCanEmail;
		
		private EntityRef<Person> _OnBehalfOfPerson;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnCanEmailChanging(int value);
		partial void OnCanEmailChanged();
		
		partial void OnOnBehalfOfChanging(int value);
		partial void OnOnBehalfOfChanged();
		
    #endregion
		public PeopleCanEmailFor()
		{
			
			
			this._PersonCanEmail = default(EntityRef<Person>); 
			
			this._OnBehalfOfPerson = default(EntityRef<Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="CanEmail", UpdateCheck=UpdateCheck.Never, Storage="_CanEmail", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int CanEmail
		{
			get { return this._CanEmail; }

			set
			{
				if (this._CanEmail != value)
				{
				
					if (this._PersonCanEmail.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnCanEmailChanging(value);
					this.SendPropertyChanging();
					this._CanEmail = value;
					this.SendPropertyChanged("CanEmail");
					this.OnCanEmailChanged();
				}

			}

		}

		
		[Column(Name="OnBehalfOf", UpdateCheck=UpdateCheck.Never, Storage="_OnBehalfOf", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int OnBehalfOf
		{
			get { return this._OnBehalfOf; }

			set
			{
				if (this._OnBehalfOf != value)
				{
				
					if (this._OnBehalfOfPerson.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOnBehalfOfChanging(value);
					this.SendPropertyChanging();
					this._OnBehalfOf = value;
					this.SendPropertyChanged("OnBehalfOf");
					this.OnOnBehalfOfChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="OnBehalfOfPeople__PersonCanEmail", Storage="_PersonCanEmail", ThisKey="CanEmail", IsForeignKey=true)]
		public Person PersonCanEmail
		{
			get { return this._PersonCanEmail.Entity; }

			set
			{
				Person previousValue = this._PersonCanEmail.Entity;
				if (((previousValue != value) 
							|| (this._PersonCanEmail.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._PersonCanEmail.Entity = null;
						previousValue.OnBehalfOfPeople.Remove(this);
					}

					this._PersonCanEmail.Entity = value;
					if (value != null)
					{
						value.OnBehalfOfPeople.Add(this);
						
						this._CanEmail = value.PeopleId;
						
					}

					else
					{
						
						this._CanEmail = default(int);
						
					}

					this.SendPropertyChanged("PersonCanEmail");
				}

			}

		}

		
		[Association(Name="PersonsCanEmail__OnBehalfOfPerson", Storage="_OnBehalfOfPerson", ThisKey="OnBehalfOf", IsForeignKey=true)]
		public Person OnBehalfOfPerson
		{
			get { return this._OnBehalfOfPerson.Entity; }

			set
			{
				Person previousValue = this._OnBehalfOfPerson.Entity;
				if (((previousValue != value) 
							|| (this._OnBehalfOfPerson.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._OnBehalfOfPerson.Entity = null;
						previousValue.PersonsCanEmail.Remove(this);
					}

					this._OnBehalfOfPerson.Entity = value;
					if (value != null)
					{
						value.PersonsCanEmail.Add(this);
						
						this._OnBehalfOf = value.PeopleId;
						
					}

					else
					{
						
						this._OnBehalfOf = default(int);
						
					}

					this.SendPropertyChanged("OnBehalfOfPerson");
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

