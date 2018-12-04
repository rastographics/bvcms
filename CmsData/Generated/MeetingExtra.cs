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
	[Table(Name="dbo.MeetingExtra")]
	public partial class MeetingExtra : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _MeetingId;
		
		private string _Field;
		
		private string _Data;
		
		private string _DataType;
		
		private string _StrValue;
		
		private DateTime? _DateValue;
		
		private int? _IntValue;
		
		private bool? _BitValue;
		
		private DateTime? _TransactionTime;
		
		private bool? _UseAllValues;
		
		private string _Type;
		
   		
    	
		private EntityRef<Meeting> _Meeting;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnMeetingIdChanging(int value);
		partial void OnMeetingIdChanged();
		
		partial void OnFieldChanging(string value);
		partial void OnFieldChanged();
		
		partial void OnDataChanging(string value);
		partial void OnDataChanged();
		
		partial void OnDataTypeChanging(string value);
		partial void OnDataTypeChanged();
		
		partial void OnStrValueChanging(string value);
		partial void OnStrValueChanged();
		
		partial void OnDateValueChanging(DateTime? value);
		partial void OnDateValueChanged();
		
		partial void OnIntValueChanging(int? value);
		partial void OnIntValueChanged();
		
		partial void OnBitValueChanging(bool? value);
		partial void OnBitValueChanged();
		
		partial void OnTransactionTimeChanging(DateTime? value);
		partial void OnTransactionTimeChanged();
		
		partial void OnUseAllValuesChanging(bool? value);
		partial void OnUseAllValuesChanged();
		
		partial void OnTypeChanging(string value);
		partial void OnTypeChanged();
		
    #endregion
		public MeetingExtra()
		{
			
			
			this._Meeting = default(EntityRef<Meeting>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="MeetingId", UpdateCheck=UpdateCheck.Never, Storage="_MeetingId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int MeetingId
		{
			get { return this._MeetingId; }

			set
			{
				if (this._MeetingId != value)
				{
				
					if (this._Meeting.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnMeetingIdChanging(value);
					this.SendPropertyChanging();
					this._MeetingId = value;
					this.SendPropertyChanged("MeetingId");
					this.OnMeetingIdChanged();
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

		
		[Column(Name="DataType", UpdateCheck=UpdateCheck.Never, Storage="_DataType", DbType="nvarchar(5)")]
		public string DataType
		{
			get { return this._DataType; }

			set
			{
				if (this._DataType != value)
				{
				
                    this.OnDataTypeChanging(value);
					this.SendPropertyChanging();
					this._DataType = value;
					this.SendPropertyChanged("DataType");
					this.OnDataTypeChanged();
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

		
		[Column(Name="TransactionTime", UpdateCheck=UpdateCheck.Never, Storage="_TransactionTime", DbType="datetime")]
		public DateTime? TransactionTime
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
    	
		[Association(Name="FK_MeetingExtra_Meetings", Storage="_Meeting", ThisKey="MeetingId", IsForeignKey=true)]
		public Meeting Meeting
		{
			get { return this._Meeting.Entity; }

			set
			{
				Meeting previousValue = this._Meeting.Entity;
				if (((previousValue != value) 
							|| (this._Meeting.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Meeting.Entity = null;
						previousValue.MeetingExtras.Remove(this);
					}

					this._Meeting.Entity = value;
					if (value != null)
					{
						value.MeetingExtras.Add(this);
						
						this._MeetingId = value.MeetingId;
						
					}

					else
					{
						
						this._MeetingId = default(int);
						
					}

					this.SendPropertyChanged("Meeting");
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

