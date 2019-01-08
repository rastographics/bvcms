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
	[Table(Name="dbo.TransactionPeople")]
	public partial class TransactionPerson : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _PeopleId;
		
		private decimal? _Amt;
		
		private int? _OrgId;
		
		private bool? _Donor;
		
   		
    	
		private EntityRef<Person> _Person;
		
		private EntityRef<Transaction> _Transaction;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnAmtChanging(decimal? value);
		partial void OnAmtChanged();
		
		partial void OnOrgIdChanging(int? value);
		partial void OnOrgIdChanged();
		
		partial void OnDonorChanging(bool? value);
		partial void OnDonorChanged();
		
    #endregion
		public TransactionPerson()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
			this._Transaction = default(EntityRef<Transaction>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int Id
		{
			get { return this._Id; }

			set
			{
				if (this._Id != value)
				{
				
					if (this._Transaction.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
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

		
		[Column(Name="Amt", UpdateCheck=UpdateCheck.Never, Storage="_Amt", DbType="money")]
		public decimal? Amt
		{
			get { return this._Amt; }

			set
			{
				if (this._Amt != value)
				{
				
                    this.OnAmtChanging(value);
					this.SendPropertyChanging();
					this._Amt = value;
					this.SendPropertyChanged("Amt");
					this.OnAmtChanged();
				}

			}

		}

		
		[Column(Name="OrgId", UpdateCheck=UpdateCheck.Never, Storage="_OrgId", DbType="int")]
		public int? OrgId
		{
			get { return this._OrgId; }

			set
			{
				if (this._OrgId != value)
				{
				
                    this.OnOrgIdChanging(value);
					this.SendPropertyChanging();
					this._OrgId = value;
					this.SendPropertyChanged("OrgId");
					this.OnOrgIdChanged();
				}

			}

		}

		
		[Column(Name="Donor", UpdateCheck=UpdateCheck.Never, Storage="_Donor", DbType="bit")]
		public bool? Donor
		{
			get { return this._Donor; }

			set
			{
				if (this._Donor != value)
				{
				
                    this.OnDonorChanging(value);
					this.SendPropertyChanging();
					this._Donor = value;
					this.SendPropertyChanged("Donor");
					this.OnDonorChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_TransactionPeople_Person", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.TransactionPeople.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.TransactionPeople.Add(this);
						
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

		
		[Association(Name="FK_TransactionPeople_Transaction", Storage="_Transaction", ThisKey="Id", IsForeignKey=true)]
		public Transaction Transaction
		{
			get { return this._Transaction.Entity; }

			set
			{
				Transaction previousValue = this._Transaction.Entity;
				if (((previousValue != value) 
							|| (this._Transaction.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Transaction.Entity = null;
						previousValue.TransactionPeople.Remove(this);
					}

					this._Transaction.Entity = value;
					if (value != null)
					{
						value.TransactionPeople.Add(this);
						
						this._Id = value.Id;
						
					}

					else
					{
						
						this._Id = default(int);
						
					}

					this.SendPropertyChanged("Transaction");
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

