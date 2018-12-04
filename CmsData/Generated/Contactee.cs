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
	[Table(Name="dbo.Contactees")]
	public partial class Contactee : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _ContactId;
		
		private int _PeopleId;
		
		private bool? _ProfessionOfFaith;
		
		private bool? _PrayedForPerson;
		
   		
    	
		private EntityRef<Contact> _contact;
		
		private EntityRef<Person> _person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnContactIdChanging(int value);
		partial void OnContactIdChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnProfessionOfFaithChanging(bool? value);
		partial void OnProfessionOfFaithChanged();
		
		partial void OnPrayedForPersonChanging(bool? value);
		partial void OnPrayedForPersonChanged();
		
    #endregion
		public Contactee()
		{
			
			
			this._contact = default(EntityRef<Contact>); 
			
			this._person = default(EntityRef<Person>); 
			
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

		
		[Column(Name="ProfessionOfFaith", UpdateCheck=UpdateCheck.Never, Storage="_ProfessionOfFaith", DbType="bit")]
		public bool? ProfessionOfFaith
		{
			get { return this._ProfessionOfFaith; }

			set
			{
				if (this._ProfessionOfFaith != value)
				{
				
                    this.OnProfessionOfFaithChanging(value);
					this.SendPropertyChanging();
					this._ProfessionOfFaith = value;
					this.SendPropertyChanged("ProfessionOfFaith");
					this.OnProfessionOfFaithChanged();
				}

			}

		}

		
		[Column(Name="PrayedForPerson", UpdateCheck=UpdateCheck.Never, Storage="_PrayedForPerson", DbType="bit")]
		public bool? PrayedForPerson
		{
			get { return this._PrayedForPerson; }

			set
			{
				if (this._PrayedForPerson != value)
				{
				
                    this.OnPrayedForPersonChanging(value);
					this.SendPropertyChanging();
					this._PrayedForPerson = value;
					this.SendPropertyChanged("PrayedForPerson");
					this.OnPrayedForPersonChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="contactees__contact", Storage="_contact", ThisKey="ContactId", IsForeignKey=true)]
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
						previousValue.contactees.Remove(this);
					}

					this._contact.Entity = value;
					if (value != null)
					{
						value.contactees.Add(this);
						
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

		
		[Association(Name="contactsHad__person", Storage="_person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.contactsHad.Remove(this);
					}

					this._person.Entity = value;
					if (value != null)
					{
						value.contactsHad.Add(this);
						
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

