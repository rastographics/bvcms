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
	[Table(Name="dbo.Roles")]
	public partial class Role : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private string _RoleName;
		
		private int _RoleId;
		
		private bool? _Hardwired;
		
   		
   		private EntitySet<UserRole> _UserRoles;
		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnRoleNameChanging(string value);
		partial void OnRoleNameChanged();
		
		partial void OnRoleIdChanging(int value);
		partial void OnRoleIdChanged();
		
		partial void OnHardwiredChanging(bool? value);
		partial void OnHardwiredChanged();
		
    #endregion
		public Role()
		{
			
			this._UserRoles = new EntitySet<UserRole>(new Action< UserRole>(this.attach_UserRoles), new Action< UserRole>(this.detach_UserRoles)); 
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="RoleName", UpdateCheck=UpdateCheck.Never, Storage="_RoleName", DbType="nvarchar(50)")]
		public string RoleName
		{
			get { return this._RoleName; }

			set
			{
				if (this._RoleName != value)
				{
				
                    this.OnRoleNameChanging(value);
					this.SendPropertyChanging();
					this._RoleName = value;
					this.SendPropertyChanged("RoleName");
					this.OnRoleNameChanged();
				}

			}

		}

		
		[Column(Name="RoleId", UpdateCheck=UpdateCheck.Never, Storage="_RoleId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int RoleId
		{
			get { return this._RoleId; }

			set
			{
				if (this._RoleId != value)
				{
				
                    this.OnRoleIdChanging(value);
					this.SendPropertyChanging();
					this._RoleId = value;
					this.SendPropertyChanged("RoleId");
					this.OnRoleIdChanged();
				}

			}

		}

		
		[Column(Name="hardwired", UpdateCheck=UpdateCheck.Never, Storage="_Hardwired", DbType="bit")]
		public bool? Hardwired
		{
			get { return this._Hardwired; }

			set
			{
				if (this._Hardwired != value)
				{
				
                    this.OnHardwiredChanging(value);
					this.SendPropertyChanging();
					this._Hardwired = value;
					this.SendPropertyChanged("Hardwired");
					this.OnHardwiredChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_UserRole_Roles", Storage="_UserRoles", OtherKey="RoleId")]
   		public EntitySet<UserRole> UserRoles
   		{
   		    get { return this._UserRoles; }

			set	{ this._UserRoles.Assign(value); }

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

   		
		private void attach_UserRoles(UserRole entity)
		{
			this.SendPropertyChanging();
			entity.Role = this;
		}

		private void detach_UserRoles(UserRole entity)
		{
			this.SendPropertyChanging();
			entity.Role = null;
		}

		
	}

}

