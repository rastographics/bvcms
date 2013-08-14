using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData
{
	[Table(Name="dbo.UserCanEmailFor")]
	public partial class UserCanEmailFor : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _UserId;
		
		private int _CanEmailFor;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnUserIdChanging(int value);
		partial void OnUserIdChanged();
		
		partial void OnCanEmailForChanging(int value);
		partial void OnCanEmailForChanged();
		
    #endregion
		public UserCanEmailFor()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="UserId", UpdateCheck=UpdateCheck.Never, Storage="_UserId", DbType="int NOT NULL", IsPrimaryKey=true)]
		public int UserId
		{
			get { return this._UserId; }

			set
			{
				if (this._UserId != value)
				{
				
                    this.OnUserIdChanging(value);
					this.SendPropertyChanging();
					this._UserId = value;
					this.SendPropertyChanged("UserId");
					this.OnUserIdChanged();
				}

			}

		}

		
		[Column(Name="CanEmailFor", UpdateCheck=UpdateCheck.Never, Storage="_CanEmailFor", DbType="int NOT NULL", IsPrimaryKey=true)]
		public int CanEmailFor
		{
			get { return this._CanEmailFor; }

			set
			{
				if (this._CanEmailFor != value)
				{
				
                    this.OnCanEmailForChanging(value);
					this.SendPropertyChanging();
					this._CanEmailFor = value;
					this.SendPropertyChanged("CanEmailFor");
					this.OnCanEmailForChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
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

