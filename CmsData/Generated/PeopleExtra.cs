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
	[Table(Name="dbo.PeopleExtra")]
	public partial class PeopleExtra : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private DateTime _TransactionTime;
		
		private string _Field;
		
		private string _StrValue;
		
		private DateTime? _DateValue;
		
		private string _Data;
		
		private int? _IntValue;
		
		private int? _IntValue2;
		
		private bool? _BitValue;
		
		private string _FieldValue;
		
		private bool? _UseAllValues;
		
		private int _Instance;
		
		private bool? _IsAttributes;
		
		private string _Type;
		
		private string _Metadata;
		
   		
    	
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnTransactionTimeChanging(DateTime value);
		partial void OnTransactionTimeChanged();
		
		partial void OnFieldChanging(string value);
		partial void OnFieldChanged();
		
		partial void OnStrValueChanging(string value);
		partial void OnStrValueChanged();
		
		partial void OnDateValueChanging(DateTime? value);
		partial void OnDateValueChanged();
		
		partial void OnDataChanging(string value);
		partial void OnDataChanged();
		
		partial void OnIntValueChanging(int? value);
		partial void OnIntValueChanged();
		
		partial void OnIntValue2Changing(int? value);
		partial void OnIntValue2Changed();
		
		partial void OnBitValueChanging(bool? value);
		partial void OnBitValueChanged();
		
		partial void OnFieldValueChanging(string value);
		partial void OnFieldValueChanged();
		
		partial void OnUseAllValuesChanging(bool? value);
		partial void OnUseAllValuesChanged();
		
		partial void OnInstanceChanging(int value);
		partial void OnInstanceChanged();
		
		partial void OnIsAttributesChanging(bool? value);
		partial void OnIsAttributesChanged();
		
		partial void OnTypeChanging(string value);
		partial void OnTypeChanged();
		
		partial void OnMetadataChanging(string value);
		partial void OnMetadataChanged();
		
    #endregion
		public PeopleExtra()
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

		
		[Column(Name="TransactionTime", UpdateCheck=UpdateCheck.Never, Storage="_TransactionTime", DbType="datetime NOT NULL")]
		public DateTime TransactionTime
		{
			get { return this._TransactionTime; }

			set
			{
				if (this._TransactionTime != value)
				{
				
                    this.OnTransactionTimeChanging(value);
					this.SendPropertyChanging();
					this._TransactionTime = value;
					this.SendPropertyChanged("TransactionTime");
					this.OnTransactionTimeChanged();
				}

			}

		}

		
		[Column(Name="Field", UpdateCheck=UpdateCheck.Never, Storage="_Field", DbType="nvarchar(150) NOT NULL", IsPrimaryKey=true)]
		public string Field
		{
			get { return this._Field; }

			set
			{
				if (this._Field != value)
				{
				
                    this.OnFieldChanging(value);
					this.SendPropertyChanging();
					this._Field = value;
					this.SendPropertyChanged("Field");
					this.OnFieldChanged();
				}

			}

		}

		
		[Column(Name="StrValue", UpdateCheck=UpdateCheck.Never, Storage="_StrValue", DbType="nvarchar(200)")]
		public string StrValue
		{
			get { return this._StrValue; }

			set
			{
				if (this._StrValue != value)
				{
				
                    this.OnStrValueChanging(value);
					this.SendPropertyChanging();
					this._StrValue = value;
					this.SendPropertyChanged("StrValue");
					this.OnStrValueChanged();
				}

			}

		}

		
		[Column(Name="DateValue", UpdateCheck=UpdateCheck.Never, Storage="_DateValue", DbType="datetime")]
		public DateTime? DateValue
		{
			get { return this._DateValue; }

			set
			{
				if (this._DateValue != value)
				{
				
                    this.OnDateValueChanging(value);
					this.SendPropertyChanging();
					this._DateValue = value;
					this.SendPropertyChanged("DateValue");
					this.OnDateValueChanged();
				}

			}

		}

		
		[Column(Name="Data", UpdateCheck=UpdateCheck.Never, Storage="_Data", DbType="nvarchar")]
		public string Data
		{
			get { return this._Data; }

			set
			{
				if (this._Data != value)
				{
				
                    this.OnDataChanging(value);
					this.SendPropertyChanging();
					this._Data = value;
					this.SendPropertyChanged("Data");
					this.OnDataChanged();
				}

			}

		}

		
		[Column(Name="IntValue", UpdateCheck=UpdateCheck.Never, Storage="_IntValue", DbType="int")]
		public int? IntValue
		{
			get { return this._IntValue; }

			set
			{
				if (this._IntValue != value)
				{
				
                    this.OnIntValueChanging(value);
					this.SendPropertyChanging();
					this._IntValue = value;
					this.SendPropertyChanged("IntValue");
					this.OnIntValueChanged();
				}

			}

		}

		
		[Column(Name="IntValue2", UpdateCheck=UpdateCheck.Never, Storage="_IntValue2", DbType="int")]
		public int? IntValue2
		{
			get { return this._IntValue2; }

			set
			{
				if (this._IntValue2 != value)
				{
				
                    this.OnIntValue2Changing(value);
					this.SendPropertyChanging();
					this._IntValue2 = value;
					this.SendPropertyChanged("IntValue2");
					this.OnIntValue2Changed();
				}

			}

		}

		
		[Column(Name="BitValue", UpdateCheck=UpdateCheck.Never, Storage="_BitValue", DbType="bit")]
		public bool? BitValue
		{
			get { return this._BitValue; }

			set
			{
				if (this._BitValue != value)
				{
				
                    this.OnBitValueChanging(value);
					this.SendPropertyChanging();
					this._BitValue = value;
					this.SendPropertyChanged("BitValue");
					this.OnBitValueChanged();
				}

			}

		}

		
		[Column(Name="FieldValue", UpdateCheck=UpdateCheck.Never, Storage="_FieldValue", DbType="nvarchar(351)", IsDbGenerated=true)]
		public string FieldValue
		{
			get { return this._FieldValue; }

			set
			{
				if (this._FieldValue != value)
				{
				
                    this.OnFieldValueChanging(value);
					this.SendPropertyChanging();
					this._FieldValue = value;
					this.SendPropertyChanged("FieldValue");
					this.OnFieldValueChanged();
				}

			}

		}

		
		[Column(Name="UseAllValues", UpdateCheck=UpdateCheck.Never, Storage="_UseAllValues", DbType="bit")]
		public bool? UseAllValues
		{
			get { return this._UseAllValues; }

			set
			{
				if (this._UseAllValues != value)
				{
				
                    this.OnUseAllValuesChanging(value);
					this.SendPropertyChanging();
					this._UseAllValues = value;
					this.SendPropertyChanged("UseAllValues");
					this.OnUseAllValuesChanged();
				}

			}

		}

		
		[Column(Name="Instance", UpdateCheck=UpdateCheck.Never, Storage="_Instance", DbType="int NOT NULL", IsPrimaryKey=true)]
		public int Instance
		{
			get { return this._Instance; }

			set
			{
				if (this._Instance != value)
				{
				
                    this.OnInstanceChanging(value);
					this.SendPropertyChanging();
					this._Instance = value;
					this.SendPropertyChanged("Instance");
					this.OnInstanceChanged();
				}

			}

		}

		
		[Column(Name="IsAttributes", UpdateCheck=UpdateCheck.Never, Storage="_IsAttributes", DbType="bit")]
		public bool? IsAttributes
		{
			get { return this._IsAttributes; }

			set
			{
				if (this._IsAttributes != value)
				{
				
                    this.OnIsAttributesChanging(value);
					this.SendPropertyChanging();
					this._IsAttributes = value;
					this.SendPropertyChanged("IsAttributes");
					this.OnIsAttributesChanged();
				}

			}

		}

		
		[Column(Name="Type", UpdateCheck=UpdateCheck.Never, Storage="_Type", DbType="varchar(18) NOT NULL", IsDbGenerated=true)]
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

		
		[Column(Name="Metadata", UpdateCheck=UpdateCheck.Never, Storage="_Metadata", DbType="nvarchar")]
		public string Metadata
		{
			get { return this._Metadata; }

			set
			{
				if (this._Metadata != value)
				{
				
                    this.OnMetadataChanging(value);
					this.SendPropertyChanging();
					this._Metadata = value;
					this.SendPropertyChanged("Metadata");
					this.OnMetadataChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_PeopleExtra_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.PeopleExtras.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.PeopleExtras.Add(this);
						
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

