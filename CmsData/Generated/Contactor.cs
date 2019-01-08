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
	[Table(Name="dbo.Contactors")]
	public partial class Contactor : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _ContactId;
		
		private int _PeopleId;
		
   		
    	
		private EntityRef<Person> _person;
		
		private EntityRef<Contact> _contact;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnContactIdChanging(int value);
		partial void OnContactIdChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
    #endregion
		public Contactor()
		{
			
			
			this._person = default(EntityRef<Person>); 
			
			this._contact = default(EntityRef<Contact>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ContactId", UpdateCheck=UpdateCheck.Never, Storage="_ContactId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int ContactId
		{
			get { return this._ContactId; }

			set
			{
				if (this._ContactId != value)
				{
				
					if (this._contact.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnContactIdChanging(value);
					this.SendPropertyChanging();
					this._ContactId = value;
					this.SendPropertyChanged("ContactId");
					this.OnContactIdChanged();
				}

			}

		}

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="contactsMade__person", Storage="_person", ThisKey="PeopleId", IsForeignKey=true)]
		public Person person
		{
			get { return this._person.Entity; }

			set
			{
				Person previousValue = this._person.Entity;
				if (((previousValue != value) 
							|| (this._person.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._person.Entity = null;
						previousValue.contactsMade.Remove(this);
					}

					this._person.Entity = value;
					if (value != null)
					{
						value.contactsMade.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int);
						
					}

					this.SendPropertyChanged("person");
				}

			}

		}

		
		[Association(Name="contactsMakers__contact", Storage="_contact", ThisKey="ContactId", IsForeignKey=true)]
		public Contact contact
		{
			get { return this._contact.Entity; }

			set
			{
				Contact previousValue = this._contact.Entity;
				if (((previousValue != value) 
							|| (this._contact.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._contact.Entity = null;
						previousValue.contactsMakers.Remove(this);
					}

					this._contact.Entity = value;
					if (value != null)
					{
						value.contactsMakers.Add(this);
						
						this._ContactId = value.ContactId;
						
					}

					else
					{
						
						this._ContactId = default(int);
						
					}

					this.SendPropertyChanged("contact");
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

