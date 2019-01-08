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
	[Table(Name="dbo.Preferences")]
	public partial class Preference : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _UserId;
		
		private string _PreferenceX;
		
		private string _ValueX;
		
   		
    	
		private EntityRef<User> _User;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnUserIdChanging(int value);
		partial void OnUserIdChanged();
		
		partial void OnPreferenceXChanging(string value);
		partial void OnPreferenceXChanged();
		
		partial void OnValueXChanging(string value);
		partial void OnValueXChanged();
		
    #endregion
		public Preference()
		{
			
			
			this._User = default(EntityRef<User>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="UserId", UpdateCheck=UpdateCheck.Never, Storage="_UserId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int UserId
		{
			get { return this._UserId; }

			set
			{
				if (this._UserId != value)
				{
				
					if (this._User.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnUserIdChanging(value);
					this.SendPropertyChanging();
					this._UserId = value;
					this.SendPropertyChanged("UserId");
					this.OnUserIdChanged();
				}

			}

		}

		
		[Column(Name="Preference", UpdateCheck=UpdateCheck.Never, Storage="_PreferenceX", DbType="nvarchar(30) NOT NULL", IsPrimaryKey=true)]
		public string PreferenceX
		{
			get { return this._PreferenceX; }

			set
			{
				if (this._PreferenceX != value)
				{
				
                    this.OnPreferenceXChanging(value);
					this.SendPropertyChanging();
					this._PreferenceX = value;
					this.SendPropertyChanged("PreferenceX");
					this.OnPreferenceXChanged();
				}

			}

		}

		
		[Column(Name="Value", UpdateCheck=UpdateCheck.Never, Storage="_ValueX", DbType="nvarchar")]
		public string ValueX
		{
			get { return this._ValueX; }

			set
			{
				if (this._ValueX != value)
				{
				
                    this.OnValueXChanging(value);
					this.SendPropertyChanging();
					this._ValueX = value;
					this.SendPropertyChanged("ValueX");
					this.OnValueXChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_UserPreferences_Users", Storage="_User", ThisKey="UserId", IsForeignKey=true)]
		public User User
		{
			get { return this._User.Entity; }

			set
			{
				User previousValue = this._User.Entity;
				if (((previousValue != value) 
							|| (this._User.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._User.Entity = null;
						previousValue.Preferences.Remove(this);
					}

					this._User.Entity = value;
					if (value != null)
					{
						value.Preferences.Add(this);
						
						this._UserId = value.UserId;
						
					}

					else
					{
						
						this._UserId = default(int);
						
					}

					this.SendPropertyChanged("User");
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

