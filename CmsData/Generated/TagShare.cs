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
	[Table(Name="dbo.TagShare")]
	public partial class TagShare : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _TagId;
		
		private int _PeopleId;
		
   		
    	
		private EntityRef<Person> _Person;
		
		private EntityRef<Tag> _Tag;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnTagIdChanging(int value);
		partial void OnTagIdChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
    #endregion
		public TagShare()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
			this._Tag = default(EntityRef<Tag>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="TagId", UpdateCheck=UpdateCheck.Never, Storage="_TagId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int TagId
		{
			get { return this._TagId; }

			set
			{
				if (this._TagId != value)
				{
				
					if (this._Tag.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnTagIdChanging(value);
					this.SendPropertyChanging();
					this._TagId = value;
					this.SendPropertyChanged("TagId");
					this.OnTagIdChanged();
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

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_TagShare_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.TagShares.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.TagShares.Add(this);
						
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

		
		[Association(Name="FK_TagShare_Tag", Storage="_Tag", ThisKey="TagId", IsForeignKey=true)]
		public Tag Tag
		{
			get { return this._Tag.Entity; }

			set
			{
				Tag previousValue = this._Tag.Entity;
				if (((previousValue != value) 
							|| (this._Tag.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Tag.Entity = null;
						previousValue.TagShares.Remove(this);
					}

					this._Tag.Entity = value;
					if (value != null)
					{
						value.TagShares.Add(this);
						
						this._TagId = value.Id;
						
					}

					else
					{
						
						this._TagId = default(int);
						
					}

					this.SendPropertyChanged("Tag");
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

