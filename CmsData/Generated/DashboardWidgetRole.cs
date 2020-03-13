using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.DashboardWidgetRoles")]
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
            _Role = default(EntityRef<Role>);

            _DashboardWidget = default(EntityRef<DashboardWidget>);

            OnCreated();
        }


        #region Columns

        [Column(Name = "WidgetId", UpdateCheck = UpdateCheck.Never, Storage = "_WidgetId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int WidgetId
        {
            get => _WidgetId;

            set
            {
                if (_WidgetId != value)
                {

                    if (_DashboardWidget.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    OnWidgetIdChanging(value);
                    SendPropertyChanging();
                    _WidgetId = value;
                    SendPropertyChanged("WidgetId");
                    OnWidgetIdChanged();
                }

            }

        }


        [Column(Name = "RoleId", UpdateCheck = UpdateCheck.Never, Storage = "_RoleId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int RoleId
        {
            get => _RoleId;

            set
            {
                if (_RoleId != value)
                {

                    if (_Role.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    OnRoleIdChanging(value);
                    SendPropertyChanging();
                    _RoleId = value;
                    SendPropertyChanged("RoleId");
                    OnRoleIdChanged();
                }

            }

        }


        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_WidgetRole_Roles", Storage = "_Role", ThisKey = "RoleId", IsForeignKey = true)]
        public Role Role
        {
            get => _Role.Entity;

            set
            {
                Role previousValue = _Role.Entity;
                if (((previousValue != value)
                            || (_Role.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Role.Entity = null;
                    }

                    _Role.Entity = value;
                    if (value != null)
                    {
                        _RoleId = value.RoleId;
                    }

                    else
                    {
                        _RoleId = default(int);
                    }

                    SendPropertyChanged("Role");
                }

            }

        }


        [Association(Name = "FK_WidgetRole_Widgets", Storage = "_DashboardWidget", ThisKey = "WidgetId", IsForeignKey = true)]
        public DashboardWidget DashboardWidget
        {
            get => _DashboardWidget.Entity;

            set
            {
                DashboardWidget previousValue = _DashboardWidget.Entity;
                if (((previousValue != value)
                            || (_DashboardWidget.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _DashboardWidget.Entity = null;
                    }

                    _DashboardWidget.Entity = value;
                    if (value != null)
                    {
                        _WidgetId = value.Id;
                    }

                    else
                    {
                        _WidgetId = default(int);
                    }

                    SendPropertyChanged("DashboardWidget");
                }

            }

        }


        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((PropertyChanging != null))
                PropertyChanging(this, emptyChangingEventArgs);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((PropertyChanged != null))
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


    }

}

