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
		
		private int? _HTMLContentId;
		
		private int? _PythonContentId;
		
		private int? _SQLContentId;
		
		private bool _Enabled;
		
		private int _Order;
		
		private bool _System;

        private int _CachePolicy;

        private int _CacheHours;

        private DateTime? _CacheExpires;

        private string _CachedContent;

        private EntitySet<DashboardWidgetRole> _DashboardWidgetRoles;
    	
		private EntityRef<Content> _HTMLContent;
		
		private EntityRef<Content> _PythonContent;
		
		private EntityRef<Content> _SQLContent;
		
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

        partial void OnCachePolicyChanging(int value);
        partial void OnCachePolicyChanged();

        partial void OnCacheHoursChanging(int value);
        partial void OnCacheHoursChanged();

        partial void OnCacheExpiresChanging(DateTime? value);
        partial void OnCacheExpiresChanged();

        partial void OnCachedContentChanging(string value);
        partial void OnCachedContentChanged();

        #endregion
        public DashboardWidget()
		{
			
			this._DashboardWidgetRoles = new EntitySet<DashboardWidgetRole>(new Action< DashboardWidgetRole>(this.attach_DashboardWidgetRoles), new Action< DashboardWidgetRole>(this.detach_DashboardWidgetRoles)); 
			
			
			this._HTMLContent = default(EntityRef<Content>); 
			
			this._PythonContent = default(EntityRef<Content>); 
			
			this._SQLContent = default(EntityRef<Content>); 
			
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

		
		[Column(Name="HTMLContentId", UpdateCheck=UpdateCheck.Never, Storage="_HTMLContentId", DbType="int")]
		[IsForeignKey]
		public int? HTMLContentId
		{
			get { return this._HTMLContentId; }

			set
			{
				if (this._HTMLContentId != value)
				{
				
					if (this._HTMLContent.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnHTMLContentChanging(value);
					this.SendPropertyChanging();
					this._HTMLContentId = value;
					this.SendPropertyChanged("HTMLContentId");
					this.OnHTMLContentChanged();
				}

			}

		}


        [Column(Name = "PythonContentId", UpdateCheck = UpdateCheck.Never, Storage = "_PythonContentId", DbType = "int")]
        [IsForeignKey]
        public int? PythonContentId
        {
            get { return this._PythonContentId; }

            set
            {
                if (this._PythonContentId != value)
                {

                    if (this._PythonContent.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnPythonContentChanging(value);
                    this.SendPropertyChanging();
                    this._PythonContentId = value;
                    this.SendPropertyChanged("PythonContentId");
                    this.OnPythonContentChanged();
                }

            }

        }

        [Column(Name = "SQLContentId", UpdateCheck = UpdateCheck.Never, Storage = "_SQLContentId", DbType = "int")]
        [IsForeignKey]
        public int? SQLContentId
        {
            get { return this._SQLContentId; }

            set
            {
                if (this._SQLContentId != value)
                {

                    if (this._SQLContent.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnSQLContentChanging(value);
                    this.SendPropertyChanging();
                    this._SQLContentId = value;
                    this.SendPropertyChanged("SQLContentId");
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


        [Column(Name = "CachePolicy", UpdateCheck = UpdateCheck.Never, Storage = "_CachePolicy", DbType = "int NOT NULL")]
        public int CachePolicy
        {
            get { return this._CachePolicy; }

            set
            {
                if (this._CachePolicy != value)
                {

                    this.OnCachePolicyChanging(value);
                    this.SendPropertyChanging();
                    this._CachePolicy = value;
                    this.SendPropertyChanged("CachePolicy");
                    this.OnCachePolicyChanged();
                }

            }

        }


        [Column(Name = "CacheHours", UpdateCheck = UpdateCheck.Never, Storage = "_CacheHours", DbType = "int NOT NULL")]
        public int CacheHours
        {
            get { return this._CacheHours; }

            set
            {
                if (this._CacheHours != value)
                {

                    this.OnCacheHoursChanging(value);
                    this.SendPropertyChanging();
                    this._CacheHours = value;
                    this.SendPropertyChanged("CacheHours");
                    this.OnCacheHoursChanged();
                }

            }

        }


        [Column(Name = "CacheExpires", UpdateCheck = UpdateCheck.Never, Storage = "_CacheExpires", DbType = "datetime")]
        public DateTime? CacheExpires
        {
            get { return this._CacheExpires; }

            set
            {
                if (this._CacheExpires != value)
                {

                    this.OnCacheExpiresChanging(value);
                    this.SendPropertyChanging();
                    this._CacheExpires = value;
                    this.SendPropertyChanged("CacheExpires");
                    this.OnCacheExpiresChanged();
                }

            }

        }


        [Column(Name = "CachedContent", UpdateCheck = UpdateCheck.Never, Storage = "_CachedContent", DbType = "nvarchar")]
        public string CachedContent
        {
            get { return this._CachedContent; }

            set
            {
                if (this._CachedContent != value)
                {

                    this.OnCachedContentChanging(value);
                    this.SendPropertyChanging();
                    this._CachedContent = value;
                    this.SendPropertyChanged("CachedContent");
                    this.OnCachedContentChanged();
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
    	
		[Association(Name= "FK_Dashboard_HTMLContent", Storage="_HTMLContent", ThisKey="HTMLContentId", IsForeignKey=true)]
		public Content HTMLContent
		{
			get { return this._HTMLContent.Entity; }

			set
			{
				Content previousValue = this._HTMLContent.Entity;
				if (((previousValue != value) 
							|| (this._HTMLContent.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._HTMLContent.Entity = null;
					}

					this._HTMLContent.Entity = value;
					if (value != null)
					{
						this._HTMLContentId = value.Id;
					}

					else
					{
						
						this._HTMLContentId = default(int?);
						
					}

					this.SendPropertyChanged("HTMLContent");
				}

			}

		}


        [Association(Name = "FK_Dashboard_PythonContent", Storage = "_PythonContent", ThisKey = "PythonContentId", IsForeignKey = true)]
        public Content PythonContent
        {
            get { return this._PythonContent.Entity; }

            set
            {
                Content previousValue = this._PythonContent.Entity;
                if (((previousValue != value)
                            || (this._PythonContent.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if (previousValue != null)
                    {
                        this._PythonContent.Entity = null;
                    }

                    this._PythonContent.Entity = value;
                    if (value != null)
                    {
                        this._PythonContentId = value.Id;
                    }

                    else
                    {

                        this._PythonContentId = default(int?);

                    }

                    this.SendPropertyChanged("PythonContent");
                }

            }

        }


        [Association(Name = "FK_Dashboard_SQLContent", Storage = "_SQLContent", ThisKey = "SQLContentId", IsForeignKey = true)]
        public Content SQLContent
        {
            get { return this._SQLContent.Entity; }

            set
            {
                Content previousValue = this._SQLContent.Entity;
                if (((previousValue != value)
                            || (this._SQLContent.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if (previousValue != null)
                    {
                        this._SQLContent.Entity = null;
                    }

                    this._SQLContent.Entity = value;
                    if (value != null)
                    {
                        this._SQLContentId = value.Id;
                    }

                    else
                    {

                        this._SQLContentId = default(int?);

                    }

                    this.SendPropertyChanged("SQLContent");
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

