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
	[Table(Name="dbo.RecurringAmounts")]
	public partial class RecurringAmount : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private int _FundId;
		
		private decimal? _Amt;
    	
		private EntityRef<ContributionFund> _ContributionFund;
		
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnFundIdChanging(int value);
		partial void OnFundIdChanged();
		
		partial void OnAmtChanging(decimal? value);
		partial void OnAmtChanged();

        #endregion
        public RecurringAmount()
		{
			
			
			this._ContributionFund = default(EntityRef<ContributionFund>); 
			
			this._Person = default(EntityRef<Person>); 
			
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

		
		[Column(Name="FundId", UpdateCheck=UpdateCheck.Never, Storage="_FundId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int FundId
		{
			get { return this._FundId; }

			set
			{
				if (this._FundId != value)
				{
				
					if (this._ContributionFund.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnFundIdChanging(value);
					this.SendPropertyChanging();
					this._FundId = value;
					this.SendPropertyChanged("FundId");
					this.OnFundIdChanged();
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

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name="FK_RecurringAmounts_ContributionFund", Storage="_ContributionFund", ThisKey="FundId", IsForeignKey=true)]
		public ContributionFund ContributionFund
		{
			get { return this._ContributionFund.Entity; }

			set
			{
				ContributionFund previousValue = this._ContributionFund.Entity;
				if (((previousValue != value) 
							|| (this._ContributionFund.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ContributionFund.Entity = null;
						previousValue.RecurringAmounts.Remove(this);
					}

					this._ContributionFund.Entity = value;
					if (value != null)
					{
						value.RecurringAmounts.Add(this);
						
						this._FundId = value.FundId;
						
					}

					else
					{
						
						this._FundId = default(int);
						
					}

					this.SendPropertyChanged("ContributionFund");
				}

			}

		}

		
		[Association(Name="FK_RecurringAmounts_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.RecurringAmounts.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.RecurringAmounts.Add(this);
						
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

