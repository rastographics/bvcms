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
	[Table(Name="dbo.SMSGroupMembers")]
	public partial class SMSGroupMember : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _GroupID;
		
		private int _UserID;
		
   		
    	
		private EntityRef<SMSGroup> _SMSGroup;
		
		private EntityRef<User> _User;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnGroupIDChanging(int value);
		partial void OnGroupIDChanged();
		
		partial void OnUserIDChanging(int value);
		partial void OnUserIDChanged();
		
    #endregion
		public SMSGroupMember()
		{
			
			
			this._SMSGroup = default(EntityRef<SMSGroup>); 
			
			this._User = default(EntityRef<User>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ID", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get { return this._Id; }

			set
			{
				if (this._Id != value)
				{
				
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}

			}

		}

		
		[Column(Name="GroupID", UpdateCheck=UpdateCheck.Never, Storage="_GroupID", DbType="int NOT NULL")]
		[IsForeignKey]
		public int GroupID
		{
			get { return this._GroupID; }

			set
			{
				if (this._GroupID != value)
				{
				
					if (this._SMSGroup.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnGroupIDChanging(value);
					this.SendPropertyChanging();
					this._GroupID = value;
					this.SendPropertyChanged("GroupID");
					this.OnGroupIDChanged();
				}

			}

		}

		
		[Column(Name="UserID", UpdateCheck=UpdateCheck.Never, Storage="_UserID", DbType="int NOT NULL")]
		[IsForeignKey]
		public int UserID
		{
			get { return this._UserID; }

			set
			{
				if (this._UserID != value)
				{
				
					if (this._User.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnUserIDChanging(value);
					this.SendPropertyChanging();
					this._UserID = value;
					this.SendPropertyChanged("UserID");
					this.OnUserIDChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_SMSGroupMembers_SMSGroups", Storage="_SMSGroup", ThisKey="GroupID", IsForeignKey=true)]
		public SMSGroup SMSGroup
		{
			get { return this._SMSGroup.Entity; }

			set
			{
				SMSGroup previousValue = this._SMSGroup.Entity;
				if (((previousValue != value) 
							|| (this._SMSGroup.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._SMSGroup.Entity = null;
						previousValue.SMSGroupMembers.Remove(this);
					}

					this._SMSGroup.Entity = value;
					if (value != null)
					{
						value.SMSGroupMembers.Add(this);
						
						this._GroupID = value.Id;
						
					}

					else
					{
						
						this._GroupID = default(int);
						
					}

					this.SendPropertyChanged("SMSGroup");
				}

			}

		}

		
		[Association(Name="FK_SMSGroupMembers_Users", Storage="_User", ThisKey="UserID", IsForeignKey=true)]
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
						previousValue.SMSGroupMembers.Remove(this);
					}

					this._User.Entity = value;
					if (value != null)
					{
						value.SMSGroupMembers.Add(this);
						
						this._UserID = value.UserId;
						
					}

					else
					{
						
						this._UserID = default(int);
						
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

