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
	[Table(Name="dbo.DashboardWidgets")]
	public partial class DashboardWidget : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Name;
		
		private string _Description;
		
		private int? _HTMLContent;
		
		private int? _PythonContent;
		
		private int? _SQLContent;
		
		private bool _Enabled;
		
		private int _Order;
		
		private bool _System;
		
   		
   		private EntitySet<DashboardWidgetRole> _DashboardWidgetRoles;
		
    	
		private EntityRef<Content> _Content;
		
		private EntityRef<Content> _Content;
		
		private EntityRef<Content> _Content;
		
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
		
		partial void OnHTMLContentChanging(int? value);
		partial void OnHTMLContentChanged();
		
		partial void OnPythonContentChanging(int? value);
		partial void OnPythonContentChanged();
		
		partial void OnSQLContentChanging(int? value);
		partial void OnSQLContentChanged();
		
		partial void OnEnabledChanging(bool value);
		partial void OnEnabledChanged();
		
		partial void OnOrderChanging(int value);
		partial void OnOrderChanged();
		
		partial void OnSystemChanging(bool value);
		partial void OnSystemChanged();
		
    #endregion
		public DashboardWidget()
		{
			
			this._DashboardWidgetRoles = new EntitySet<DashboardWidgetRole>(new Action< DashboardWidgetRole>(this.attach_DashboardWidgetRoles), new Action< DashboardWidgetRole>(this.detach_DashboardWidgetRoles)); 
			
			
			this._Content = default(EntityRef<Content>); 
			
			this._Content = default(EntityRef<Content>); 
			
			this._Content = default(EntityRef<Content>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
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

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(140) NOT NULL")]
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

		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="varchar(400)")]
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

		
		[Column(Name="HTMLContent", UpdateCheck=UpdateCheck.Never, Storage="_HTMLContent", DbType="int")]
		[IsForeignKey]
		public int? HTMLContent
		{
			get { return this._HTMLContent; }

			set
			{
				if (this._HTMLContent != value)
				{
				
					if (this._Content.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnHTMLContentChanging(value);
					this.SendPropertyChanging();
					this._HTMLContent = value;
					this.SendPropertyChanged("HTMLContent");
					this.OnHTMLContentChanged();
				}

			}

		}

		
		[Column(Name="PythonContent", UpdateCheck=UpdateCheck.Never, Storage="_PythonContent", DbType="int")]
		[IsForeignKey]
		public int? PythonContent
		{
			get { return this._PythonContent; }

			set
			{
				if (this._PythonContent != value)
				{
				
					if (this._Content.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPythonContentChanging(value);
					this.SendPropertyChanging();
					this._PythonContent = value;
					this.SendPropertyChanged("PythonContent");
					this.OnPythonContentChanged();
				}

			}

		}

		
		[Column(Name="SQLContent", UpdateCheck=UpdateCheck.Never, Storage="_SQLContent", DbType="int")]
		[IsForeignKey]
		public int? SQLContent
		{
			get { return this._SQLContent; }

			set
			{
				if (this._SQLContent != value)
				{
				
					if (this._Content.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnSQLContentChanging(value);
					this.SendPropertyChanging();
					this._SQLContent = value;
					this.SendPropertyChanged("SQLContent");
					this.OnSQLContentChanged();
				}

			}

		}

		
		[Column(Name="Enabled", UpdateCheck=UpdateCheck.Never, Storage="_Enabled", DbType="bit NOT NULL")]
		public bool Enabled
		{
			get { return this._Enabled; }

			set
			{
				if (this._Enabled != value)
				{
				
                    this.OnEnabledChanging(value);
					this.SendPropertyChanging();
					this._Enabled = value;
					this.SendPropertyChanged("Enabled");
					this.OnEnabledChanged();
				}

			}

		}

		
		[Column(Name="Order", UpdateCheck=UpdateCheck.Never, Storage="_Order", DbType="int NOT NULL")]
		public int Order
		{
			get { return this._Order; }

			set
			{
				if (this._Order != value)
				{
				
                    this.OnOrderChanging(value);
					this.SendPropertyChanging();
					this._Order = value;
					this.SendPropertyChanged("Order");
					this.OnOrderChanged();
				}

			}

		}

		
		[Column(Name="System", UpdateCheck=UpdateCheck.Never, Storage="_System", DbType="bit NOT NULL")]
		public bool System
		{
			get { return this._System; }

			set
			{
				if (this._System != value)
				{
				
                    this.OnSystemChanging(value);
					this.SendPropertyChanging();
					this._System = value;
					this.SendPropertyChanged("System");
					this.OnSystemChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK__Dashboard__Widge__63849411", Storage="_DashboardWidgetRoles", OtherKey="WidgetId")]
   		public EntitySet<DashboardWidgetRole> DashboardWidgetRoles
   		{
   		    get { return this._DashboardWidgetRoles; }

			set	{ this._DashboardWidgetRoles.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK__Dashboard__HTMLC__5CD79682", Storage="_Content", ThisKey="HTMLContent", IsForeignKey=true)]
		public Content Content
		{
			get { return this._Content.Entity; }

			set
			{
				Content previousValue = this._Content.Entity;
				if (((previousValue != value) 
							|| (this._Content.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Content.Entity = null;
						previousValue.DashboardWidgets.Remove(this);
					}

					this._Content.Entity = value;
					if (value != null)
					{
						value.DashboardWidgets.Add(this);
						
						this._HTMLContent = value.Id;
						
					}

					else
					{
						
						this._HTMLContent = default(int?);
						
					}

					this.SendPropertyChanged("Content");
				}

			}

		}

		
		[Association(Name="FK__Dashboard__Pytho__5DCBBABB", Storage="_Content", ThisKey="PythonContent", IsForeignKey=true)]
		public Content Content
		{
			get { return this._Content.Entity; }

			set
			{
				Content previousValue = this._Content.Entity;
				if (((previousValue != value) 
							|| (this._Content.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Content.Entity = null;
						previousValue.DashboardWidgets.Remove(this);
					}

					this._Content.Entity = value;
					if (value != null)
					{
						value.DashboardWidgets.Add(this);
						
						this._PythonContent = value.Id;
						
					}

					else
					{
						
						this._PythonContent = default(int?);
						
					}

					this.SendPropertyChanged("Content");
				}

			}

		}

		
		[Association(Name="FK__Dashboard__SQLCo__5EBFDEF4", Storage="_Content", ThisKey="SQLContent", IsForeignKey=true)]
		public Content Content
		{
			get { return this._Content.Entity; }

			set
			{
				Content previousValue = this._Content.Entity;
				if (((previousValue != value) 
							|| (this._Content.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Content.Entity = null;
						previousValue.DashboardWidgets.Remove(this);
					}

					this._Content.Entity = value;
					if (value != null)
					{
						value.DashboardWidgets.Add(this);
						
						this._SQLContent = value.Id;
						
					}

					else
					{
						
						this._SQLContent = default(int?);
						
					}

					this.SendPropertyChanged("Content");
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

   		
		private void attach_DashboardWidgetRoles(DashboardWidgetRole entity)
		{
			this.SendPropertyChanging();
			entity.DashboardWidget = this;
		}

		private void detach_DashboardWidgetRoles(DashboardWidgetRole entity)
		{
			this.SendPropertyChanging();
			entity.DashboardWidget = null;
		}

		
	}

}

