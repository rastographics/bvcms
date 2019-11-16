using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.UserRole")]
    public partial class UserRole : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _UserId;

        private int _RoleId;

        private EntityRef<Role> _Role;

        private EntityRef<User> _User;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnUserIdChanging(int value);
        partial void OnUserIdChanged();

        partial void OnRoleIdChanging(int value);
        partial void OnRoleIdChanged();

        #endregion

        public UserRole()
        {
            _Role = default(EntityRef<Role>);

            _User = default(EntityRef<User>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "UserId", UpdateCheck = UpdateCheck.Never, Storage = "_UserId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int UserId
        {
            get => _UserId;

            set
            {
                if (_UserId != value)
                {
                    if (_User.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnUserIdChanging(value);
                    SendPropertyChanging();
                    _UserId = value;
                    SendPropertyChanged("UserId");
                    OnUserIdChanged();
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
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

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

        [Association(Name = "FK_UserRole_Roles", Storage = "_Role", ThisKey = "RoleId", IsForeignKey = true)]
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
                        previousValue.UserRoles.Remove(this);
                    }

                    _Role.Entity = value;
                    if (value != null)
                    {
                        value.UserRoles.Add(this);

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

        [Association(Name = "FK_UserRole_Users", Storage = "_User", ThisKey = "UserId", IsForeignKey = true)]
        public User User
        {
            get => _User.Entity;

            set
            {
                User previousValue = _User.Entity;
                if (((previousValue != value)
                            || (_User.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _User.Entity = null;
                        previousValue.UserRoles.Remove(this);
                    }

                    _User.Entity = value;
                    if (value != null)
                    {
                        value.UserRoles.Add(this);

                        _UserId = value.UserId;

                    }

                    else
                    {
                        _UserId = default(int);

                    }

                    SendPropertyChanged("User");
                }
            }
        }

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
    }
}
