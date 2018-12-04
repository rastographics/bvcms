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
	[Table(Name="dbo.FamilyExtra")]
	public partial class FamilyExtra : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _FamilyId;
		
		private string _Field;
		
		private string _StrValue;
		
		private DateTime? _DateValue;
		
		private DateTime _TransactionTime;
		
		private string _Data;
		
		private int? _IntValue;
		
		private bool? _BitValue;
		
		private string _FieldValue;
		
		private bool? _UseAllValues;
		
		private string _Type;
		
   		
    	
		private EntityRef<Family> _Family;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnFamilyIdChanging(int value);
		partial void OnFamilyIdChanged();
		
		partial void OnFieldChanging(string value);
		partial void OnFieldChanged();
		
		partial void OnStrValueChanging(string value);
		partial void OnStrValueChanged();
		
		partial void OnDateValueChanging(DateTime? value);
		partial void OnDateValueChanged();
		
		partial void OnTransactionTimeChanging(DateTime value);
		partial void OnTransactionTimeChanged();
		
		partial void OnDataChanging(string value);
		partial void OnDataChanged();
		
		partial void OnIntValueChanging(int? value);
		partial void OnIntValueChanged();
		
		partial void OnBitValueChanging(bool? value);
		partial void OnBitValueChanged();
		
		partial void OnFieldValueChanging(string value);
		partial void OnFieldValueChanged();
		
		partial void OnUseAllValuesChanging(bool? value);
		partial void OnUseAllValuesChanged();
		
		partial void OnTypeChanging(string value);
		partial void OnTypeChanged();
		
    #endregion
		public FamilyExtra()
		{
			
			
			this._Family = default(EntityRef<Family>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="FamilyId", UpdateCheck=UpdateCheck.Never, Storage="_FamilyId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int FamilyId
		{
			get { return this._FamilyId; }

			set
			{
				if (this._FamilyId != value)
				{
				
					if (this._Family.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnFamilyIdChanging(value);
					this.SendPropertyChanging();
					this._FamilyId = value;
					this.SendPropertyChanged("FamilyId");
					this.OnFamilyIdChanged();
				}

			}

		}

		
		[Column(Name="Field", UpdateCheck=UpdateCheck.Never, Storage="_Field", DbType="nvarchar(50) NOT NULL", IsPrimaryKey=true)]
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

		
		[Column(Name="FieldValue", UpdateCheck=UpdateCheck.Never, Storage="_FieldValue", DbType="nvarchar(251)", IsDbGenerated=true)]
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

		
		[Column(Name="Type", UpdateCheck=UpdateCheck.Never, Storage="_Type", DbType="varchar(22) NOT NULL", IsDbGenerated=true)]
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
    	
		[Association(Name="FK_FamilyExtra_Family", Storage="_Family", ThisKey="FamilyId", IsForeignKey=true)]
		public Family Family
		{
			get { return this._Family.Entity; }

			set
			{
				Family previousValue = this._Family.Entity;
				if (((previousValue != value) 
							|| (this._Family.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Family.Entity = null;
						previousValue.FamilyExtras.Remove(this);
					}

					this._Family.Entity = value;
					if (value != null)
					{
						value.FamilyExtras.Add(this);
						
						this._FamilyId = value.FamilyId;
						
					}

					else
					{
						
						this._FamilyId = default(int);
						
					}

					this.SendPropertyChanged("Family");
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

