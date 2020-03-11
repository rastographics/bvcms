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
	[Table(Name="dbo.DashboardWidgetRoles")]
	public partial class DashboardWidgetRole : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _WidgetId;
		
		private int _RoleId;
		
   		
    	
		private EntityRef<Role> _Role;
		
		private EntityRef<DashboardWidget> _DashboardWidget;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnWidgetIdChanging(int value);
		partial void OnWidgetIdChanged();
		
		partial void OnRoleIdChanging(int value);
		partial void OnRoleIdChanged();
		
    #endregion
		public DashboardWidgetRole()
		{
			
			
			this._Role = default(EntityRef<Role>); 
			
			this._DashboardWidget = default(EntityRef<DashboardWidget>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="WidgetId", UpdateCheck=UpdateCheck.Never, Storage="_WidgetId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int WidgetId
		{
			get { return this._WidgetId; }

			set
			{
				if (this._WidgetId != value)
				{
				
					if (this._DashboardWidget.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnWidgetIdChanging(value);
					this.SendPropertyChanging();
					this._WidgetId = value;
					this.SendPropertyChanged("WidgetId");
					this.OnWidgetIdChanged();
				}

			}

		}

		
		[Column(Name="RoleId", UpdateCheck=UpdateCheck.Never, Storage="_RoleId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int RoleId
		{
			get { return this._RoleId; }

			set
			{
				if (this._RoleId != value)
				{
				
					if (this._Role.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnRoleIdChanging(value);
					this.SendPropertyChanging();
					this._RoleId = value;
					this.SendPropertyChanged("RoleId");
					this.OnRoleIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK__Dashboard__RoleI__6478B84A", Storage="_Role", ThisKey="RoleId", IsForeignKey=true)]
		public Role Role
		{
			get { return this._Role.Entity; }

			set
			{
				Role previousValue = this._Role.Entity;
				if (((previousValue != value) 
							|| (this._Role.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Role.Entity = null;
						previousValue.DashboardWidgetRoles.Remove(this);
					}

					this._Role.Entity = value;
					if (value != null)
					{
						value.DashboardWidgetRoles.Add(this);
						
						this._RoleId = value.RoleId;
						
					}

					else
					{
						
						this._RoleId = default(int);
						
					}

					this.SendPropertyChanged("Role");
				}

			}

		}

		
		[Association(Name="FK__Dashboard__Widge__63849411", Storage="_DashboardWidget", ThisKey="WidgetId", IsForeignKey=true)]
		public DashboardWidget DashboardWidget
		{
			get { return this._DashboardWidget.Entity; }

			set
			{
				DashboardWidget previousValue = this._DashboardWidget.Entity;
				if (((previousValue != value) 
							|| (this._DashboardWidget.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._DashboardWidget.Entity = null;
						previousValue.DashboardWidgetRoles.Remove(this);
					}

					this._DashboardWidget.Entity = value;
					if (value != null)
					{
						value.DashboardWidgetRoles.Add(this);
						
						this._WidgetId = value.Id;
						
					}

					else
					{
						
						this._WidgetId = default(int);
						
					}

					this.SendPropertyChanged("DashboardWidget");
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

