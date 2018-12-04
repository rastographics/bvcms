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
	[Table(Name="dbo.EmailOptOut")]
	public partial class EmailOptOut : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _ToPeopleId;
		
		private string _FromEmail;
		
		private DateTime? _DateX;
		
   		
    	
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnToPeopleIdChanging(int value);
		partial void OnToPeopleIdChanged();
		
		partial void OnFromEmailChanging(string value);
		partial void OnFromEmailChanged();
		
		partial void OnDateXChanging(DateTime? value);
		partial void OnDateXChanged();
		
    #endregion
		public EmailOptOut()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ToPeopleId", UpdateCheck=UpdateCheck.Never, Storage="_ToPeopleId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int ToPeopleId
		{
			get { return this._ToPeopleId; }

			set
			{
				if (this._ToPeopleId != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnToPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._ToPeopleId = value;
					this.SendPropertyChanged("ToPeopleId");
					this.OnToPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="FromEmail", UpdateCheck=UpdateCheck.Never, Storage="_FromEmail", DbType="nvarchar(50) NOT NULL", IsPrimaryKey=true)]
		public string FromEmail
		{
			get { return this._FromEmail; }

			set
			{
				if (this._FromEmail != value)
				{
				
                    this.OnFromEmailChanging(value);
					this.SendPropertyChanging();
					this._FromEmail = value;
					this.SendPropertyChanged("FromEmail");
					this.OnFromEmailChanged();
				}

			}

		}

		
		[Column(Name="Date", UpdateCheck=UpdateCheck.Never, Storage="_DateX", DbType="datetime")]
		public DateTime? DateX
		{
			get { return this._DateX; }

			set
			{
				if (this._DateX != value)
				{
				
                    this.OnDateXChanging(value);
					this.SendPropertyChanging();
					this._DateX = value;
					this.SendPropertyChanged("DateX");
					this.OnDateXChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_EmailOptOut_People", Storage="_Person", ThisKey="ToPeopleId", IsForeignKey=true)]
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
						previousValue.EmailOptOuts.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.EmailOptOuts.Add(this);
						
						this._ToPeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._ToPeopleId = default(int);
						
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

