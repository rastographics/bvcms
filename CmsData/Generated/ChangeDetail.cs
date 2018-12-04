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
	[Table(Name="dbo.ChangeDetails")]
	public partial class ChangeDetail : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Field;
		
		private string _Before;
		
		private string _After;
		
   		
    	
		private EntityRef<ChangeLog> _ChangeLog;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnFieldChanging(string value);
		partial void OnFieldChanged();
		
		partial void OnBeforeChanging(string value);
		partial void OnBeforeChanged();
		
		partial void OnAfterChanging(string value);
		partial void OnAfterChanged();
		
    #endregion
		public ChangeDetail()
		{
			
			
			this._ChangeLog = default(EntityRef<ChangeLog>); 
			
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
				
					if (this._ChangeLog.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
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

		
		[Column(Name="Before", UpdateCheck=UpdateCheck.Never, Storage="_Before", DbType="nvarchar")]
		public string Before
		{
			get { return this._Before; }

			set
			{
				if (this._Before != value)
				{
				
                    this.OnBeforeChanging(value);
					this.SendPropertyChanging();
					this._Before = value;
					this.SendPropertyChanged("Before");
					this.OnBeforeChanged();
				}

			}

		}

		
		[Column(Name="After", UpdateCheck=UpdateCheck.Never, Storage="_After", DbType="nvarchar")]
		public string After
		{
			get { return this._After; }

			set
			{
				if (this._After != value)
				{
				
                    this.OnAfterChanging(value);
					this.SendPropertyChanging();
					this._After = value;
					this.SendPropertyChanged("After");
					this.OnAfterChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_ChangeDetails_ChangeLog", Storage="_ChangeLog", ThisKey="Id", IsForeignKey=true)]
		public ChangeLog ChangeLog
		{
			get { return this._ChangeLog.Entity; }

			set
			{
				ChangeLog previousValue = this._ChangeLog.Entity;
				if (((previousValue != value) 
							|| (this._ChangeLog.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ChangeLog.Entity = null;
						previousValue.ChangeDetails.Remove(this);
					}

					this._ChangeLog.Entity = value;
					if (value != null)
					{
						value.ChangeDetails.Add(this);
						
						this._Id = value.Id;
						
					}

					else
					{
						
						this._Id = default(int);
						
					}

					this.SendPropertyChanged("ChangeLog");
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

