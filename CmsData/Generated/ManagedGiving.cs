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
	[Table(Name="dbo.ManagedGiving")]
	public partial class ManagedGiving : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private DateTime? _StartWhen;
		
		private DateTime? _NextDate;
		
		private string _SemiEvery;
		
		private int? _Day1;
		
		private int? _Day2;
		
		private int? _EveryN;
		
		private string _Period;
		
		private DateTime? _StopWhen;
		
		private int? _StopAfter;
		
		private string _Type;
		
   		
    	
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnStartWhenChanging(DateTime? value);
		partial void OnStartWhenChanged();
		
		partial void OnNextDateChanging(DateTime? value);
		partial void OnNextDateChanged();
		
		partial void OnSemiEveryChanging(string value);
		partial void OnSemiEveryChanged();
		
		partial void OnDay1Changing(int? value);
		partial void OnDay1Changed();
		
		partial void OnDay2Changing(int? value);
		partial void OnDay2Changed();
		
		partial void OnEveryNChanging(int? value);
		partial void OnEveryNChanged();
		
		partial void OnPeriodChanging(string value);
		partial void OnPeriodChanged();
		
		partial void OnStopWhenChanging(DateTime? value);
		partial void OnStopWhenChanged();
		
		partial void OnStopAfterChanging(int? value);
		partial void OnStopAfterChanged();
		
		partial void OnTypeChanging(string value);
		partial void OnTypeChanged();
		
    #endregion
		public ManagedGiving()
		{
			
			
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

		
		[Column(Name="StartWhen", UpdateCheck=UpdateCheck.Never, Storage="_StartWhen", DbType="datetime")]
		public DateTime? StartWhen
		{
			get { return this._StartWhen; }

			set
			{
				if (this._StartWhen != value)
				{
				
                    this.OnStartWhenChanging(value);
					this.SendPropertyChanging();
					this._StartWhen = value;
					this.SendPropertyChanged("StartWhen");
					this.OnStartWhenChanged();
				}

			}

		}

		
		[Column(Name="NextDate", UpdateCheck=UpdateCheck.Never, Storage="_NextDate", DbType="datetime")]
		public DateTime? NextDate
		{
			get { return this._NextDate; }

			set
			{
				if (this._NextDate != value)
				{
				
                    this.OnNextDateChanging(value);
					this.SendPropertyChanging();
					this._NextDate = value;
					this.SendPropertyChanged("NextDate");
					this.OnNextDateChanged();
				}

			}

		}

		
		[Column(Name="SemiEvery", UpdateCheck=UpdateCheck.Never, Storage="_SemiEvery", DbType="nvarchar(2)")]
		public string SemiEvery
		{
			get { return this._SemiEvery; }

			set
			{
				if (this._SemiEvery != value)
				{
				
                    this.OnSemiEveryChanging(value);
					this.SendPropertyChanging();
					this._SemiEvery = value;
					this.SendPropertyChanged("SemiEvery");
					this.OnSemiEveryChanged();
				}

			}

		}

		
		[Column(Name="Day1", UpdateCheck=UpdateCheck.Never, Storage="_Day1", DbType="int")]
		public int? Day1
		{
			get { return this._Day1; }

			set
			{
				if (this._Day1 != value)
				{
				
                    this.OnDay1Changing(value);
					this.SendPropertyChanging();
					this._Day1 = value;
					this.SendPropertyChanged("Day1");
					this.OnDay1Changed();
				}

			}

		}

		
		[Column(Name="Day2", UpdateCheck=UpdateCheck.Never, Storage="_Day2", DbType="int")]
		public int? Day2
		{
			get { return this._Day2; }

			set
			{
				if (this._Day2 != value)
				{
				
                    this.OnDay2Changing(value);
					this.SendPropertyChanging();
					this._Day2 = value;
					this.SendPropertyChanged("Day2");
					this.OnDay2Changed();
				}

			}

		}

		
		[Column(Name="EveryN", UpdateCheck=UpdateCheck.Never, Storage="_EveryN", DbType="int")]
		public int? EveryN
		{
			get { return this._EveryN; }

			set
			{
				if (this._EveryN != value)
				{
				
                    this.OnEveryNChanging(value);
					this.SendPropertyChanging();
					this._EveryN = value;
					this.SendPropertyChanged("EveryN");
					this.OnEveryNChanged();
				}

			}

		}

		
		[Column(Name="Period", UpdateCheck=UpdateCheck.Never, Storage="_Period", DbType="nvarchar(2)")]
		public string Period
		{
			get { return this._Period; }

			set
			{
				if (this._Period != value)
				{
				
                    this.OnPeriodChanging(value);
					this.SendPropertyChanging();
					this._Period = value;
					this.SendPropertyChanged("Period");
					this.OnPeriodChanged();
				}

			}

		}

		
		[Column(Name="StopWhen", UpdateCheck=UpdateCheck.Never, Storage="_StopWhen", DbType="datetime")]
		public DateTime? StopWhen
		{
			get { return this._StopWhen; }

			set
			{
				if (this._StopWhen != value)
				{
				
                    this.OnStopWhenChanging(value);
					this.SendPropertyChanging();
					this._StopWhen = value;
					this.SendPropertyChanged("StopWhen");
					this.OnStopWhenChanged();
				}

			}

		}

		
		[Column(Name="StopAfter", UpdateCheck=UpdateCheck.Never, Storage="_StopAfter", DbType="int")]
		public int? StopAfter
		{
			get { return this._StopAfter; }

			set
			{
				if (this._StopAfter != value)
				{
				
                    this.OnStopAfterChanging(value);
					this.SendPropertyChanging();
					this._StopAfter = value;
					this.SendPropertyChanged("StopAfter");
					this.OnStopAfterChanged();
				}

			}

		}

		
		[Column(Name="type", UpdateCheck=UpdateCheck.Never, Storage="_Type", DbType="nvarchar(2)")]
		public string Type
		{
			get { return this._Type; }

			set
			{
				if (this._Type != value)
				{
				
                    this.OnTypeChanging(value);
					this.SendPropertyChanging();
					this._Type = value;
					this.SendPropertyChanged("Type");
					this.OnTypeChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_ManagedGiving_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.ManagedGivings.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.ManagedGivings.Add(this);
						
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

