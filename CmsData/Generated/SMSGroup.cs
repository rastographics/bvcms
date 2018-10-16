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
	[Table(Name="dbo.SMSGroups")]
	public partial class SMSGroup : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Name;
		
		private string _Description;
		
		private bool _SystemFlag;
		
		private bool _IsDeleted;
		
   		private EntitySet<SMSGroupMember> _SMSGroupMembers;
		
   		private EntitySet<SMSList> _SMSLists;
		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnDescriptionChanging(string value);
		partial void OnDescriptionChanged();
		
		partial void OnSystemFlagChanging(bool value);
		partial void OnSystemFlagChanged();
		
		partial void OnIsDeletedChanging(bool value);
		partial void OnIsDeletedChanged();
		
    #endregion
		public SMSGroup()
		{
			
			this._SMSGroupMembers = new EntitySet<SMSGroupMember>(new Action< SMSGroupMember>(this.attach_SMSGroupMembers), new Action< SMSGroupMember>(this.detach_SMSGroupMembers)); 
			
			this._SMSLists = new EntitySet<SMSList>(new Action< SMSList>(this.attach_SMSLists), new Action< SMSList>(this.detach_SMSLists)); 
			
			
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

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(50) NOT NULL")]
		public string Name
		{
			get { return this._Name; }

			set
			{
				if (this._Name != value)
				{
				
                    this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}

			}

		}

		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar NOT NULL")]
		public string Description
		{
			get { return this._Description; }

			set
			{
				if (this._Description != value)
				{
				
                    this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}

			}

		}
		
		[Column(Name="SystemFlag", UpdateCheck=UpdateCheck.Never, Storage="_SystemFlag", DbType="bit NOT NULL")]
		public bool SystemFlag
		{
			get { return this._SystemFlag; }

			set
			{
				if (this._SystemFlag != value)
				{
				
                    this.OnSystemFlagChanging(value);
					this.SendPropertyChanging();
					this._SystemFlag = value;
					this.SendPropertyChanged("SystemFlag");
					this.OnSystemFlagChanged();
				}

			}

		}

		
		[Column(Name="IsDeleted", UpdateCheck=UpdateCheck.Never, Storage="_IsDeleted", DbType="bit NOT NULL")]
		public bool IsDeleted
		{
			get { return this._IsDeleted; }

			set
			{
				if (this._IsDeleted != value)
				{
				
                    this.OnIsDeletedChanging(value);
					this.SendPropertyChanging();
					this._IsDeleted = value;
					this.SendPropertyChanged("IsDeleted");
					this.OnIsDeletedChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_SMSGroupMembers_SMSGroups", Storage="_SMSGroupMembers", OtherKey="GroupID")]
   		public EntitySet<SMSGroupMember> SMSGroupMembers
   		{
   		    get { return this._SMSGroupMembers; }

			set	{ this._SMSGroupMembers.Assign(value); }

   		}

		
   		[Association(Name="FK_SMSList_SMSGroups", Storage="_SMSLists", OtherKey="SendGroupID")]
   		public EntitySet<SMSList> SMSLists
   		{
   		    get { return this._SMSLists; }

			set	{ this._SMSLists.Assign(value); }

   		}

		
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

   		
		private void attach_SMSGroupMembers(SMSGroupMember entity)
		{
			this.SendPropertyChanging();
			entity.SMSGroup = this;
		}

		private void detach_SMSGroupMembers(SMSGroupMember entity)
		{
			this.SendPropertyChanging();
			entity.SMSGroup = null;
		}

		
		private void attach_SMSLists(SMSList entity)
		{
			this.SendPropertyChanging();
			entity.SMSGroup = this;
		}

		private void detach_SMSLists(SMSList entity)
		{
			this.SendPropertyChanging();
			entity.SMSGroup = null;
		}

		
	}

}

