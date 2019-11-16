using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Roles")]
    public partial class Role : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _RoleName;

        private int _RoleId;

        private bool? _Hardwired;

        private int? _Priority;

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

        partial void OnPriorityChanging(int? value);
        partial void OnPriorityChanged();

        #endregion

        public Role()
        {
            _UserRoles = new EntitySet<UserRole>(new Action<UserRole>(attach_UserRoles), new Action<UserRole>(detach_UserRoles));

            OnCreated();
        }

        #region Columns

        [Column(Name = "RoleName", UpdateCheck = UpdateCheck.Never, Storage = "_RoleName", DbType = "nvarchar(50)")]
        public string RoleName
        {
            get => _RoleName;

            set
            {
                if (_RoleName != value)
                {
                    OnRoleNameChanging(value);
                    SendPropertyChanging();
                    _RoleName = value;
                    SendPropertyChanged("RoleName");
                    OnRoleNameChanged();
                }
            }
        }

        [Column(Name = "RoleId", UpdateCheck = UpdateCheck.Never, Storage = "_RoleId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int RoleId
        {
            get => _RoleId;

            set
            {
                if (_RoleId != value)
                {
                    OnRoleIdChanging(value);
                    SendPropertyChanging();
                    _RoleId = value;
                    SendPropertyChanged("RoleId");
                    OnRoleIdChanged();
                }
            }
        }

        [Column(Name = "hardwired", UpdateCheck = UpdateCheck.Never, Storage = "_Hardwired", DbType = "bit")]
        public bool? Hardwired
        {
            get => _Hardwired;

            set
            {
                if (_Hardwired != value)
                {
                    OnHardwiredChanging(value);
                    SendPropertyChanging();
                    _Hardwired = value;
                    SendPropertyChanged("Hardwired");
                    OnHardwiredChanged();
                }
            }
        }

        [Column(Name = "Priority", UpdateCheck = UpdateCheck.Never, Storage = "_Priority", DbType = "int")]
        public int? Priority
        {
            get => _Priority;

            set
            {
                if (_Priority != value)
                {
                    OnPriorityChanging(value);
                    SendPropertyChanging();
                    _Priority = value;
                    SendPropertyChanged("Priority");
                    OnPriorityChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_UserRole_Roles", Storage = "_UserRoles", OtherKey = "RoleId")]
        public EntitySet<UserRole> UserRoles
           {
               get => _UserRoles;

            set => _UserRoles.Assign(value);

           }

        #endregion

        #region Foreign Keys

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((PropertyChanging != null))
            {
                PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void attach_UserRoles(UserRole entity)
        {
            SendPropertyChanging();
            entity.Role = this;
        }

        private void detach_UserRoles(UserRole entity)
        {
            SendPropertyChanging();
            entity.Role = null;
        }
    }
}
