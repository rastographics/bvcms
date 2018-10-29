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
	[Table(Name="dbo.VolInterestInterestCodes")]
	public partial class VolInterestInterestCode : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private int _InterestCodeId;
		
   		
    	
		private EntityRef<Person> _Person;
		
		private EntityRef<VolInterestCode> _VolInterestCode;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnInterestCodeIdChanging(int value);
		partial void OnInterestCodeIdChanged();
		
    #endregion
		public VolInterestInterestCode()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
			this._VolInterestCode = default(EntityRef<VolInterestCode>); 
			
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

		
		[Column(Name="InterestCodeId", UpdateCheck=UpdateCheck.Never, Storage="_InterestCodeId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int InterestCodeId
		{
			get { return this._InterestCodeId; }

			set
			{
				if (this._InterestCodeId != value)
				{
				
					if (this._VolInterestCode.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnInterestCodeIdChanging(value);
					this.SendPropertyChanging();
					this._InterestCodeId = value;
					this.SendPropertyChanged("InterestCodeId");
					this.OnInterestCodeIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_VolInterestInterestCodes_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.VolInterestInterestCodes.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.VolInterestInterestCodes.Add(this);
						
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

		
		[Association(Name="FK_VolInterestInterestCodes_VolInterestCodes", Storage="_VolInterestCode", ThisKey="InterestCodeId", IsForeignKey=true)]
		public VolInterestCode VolInterestCode
		{
			get { return this._VolInterestCode.Entity; }

			set
			{
				VolInterestCode previousValue = this._VolInterestCode.Entity;
				if (((previousValue != value) 
							|| (this._VolInterestCode.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._VolInterestCode.Entity = null;
						previousValue.VolInterestInterestCodes.Remove(this);
					}

					this._VolInterestCode.Entity = value;
					if (value != null)
					{
						value.VolInterestInterestCodes.Add(this);
						
						this._InterestCodeId = value.Id;
						
					}

					else
					{
						
						this._InterestCodeId = default(int);
						
					}

					this.SendPropertyChanged("VolInterestCode");
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

