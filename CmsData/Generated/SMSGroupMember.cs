using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.SMSGroupMembers")]
    public partial class SMSGroupMember : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            _SMSGroup = default(EntityRef<SMSGroup>);

            _User = default(EntityRef<User>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ID", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

        [Column(Name = "GroupID", UpdateCheck = UpdateCheck.Never, Storage = "_GroupID", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int GroupID
        {
            get => _GroupID;

            set
            {
                if (_GroupID != value)
                {
                    if (_SMSGroup.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnGroupIDChanging(value);
                    SendPropertyChanging();
                    _GroupID = value;
                    SendPropertyChanged("GroupID");
                    OnGroupIDChanged();
                }
            }
        }

        [Column(Name = "UserID", UpdateCheck = UpdateCheck.Never, Storage = "_UserID", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int UserID
        {
            get => _UserID;

            set
            {
                if (_UserID != value)
                {
                    if (_User.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnUserIDChanging(value);
                    SendPropertyChanging();
                    _UserID = value;
                    SendPropertyChanged("UserID");
                    OnUserIDChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_SMSGroupMembers_SMSGroups", Storage = "_SMSGroup", ThisKey = "GroupID", IsForeignKey = true)]
        public SMSGroup SMSGroup
        {
            get => _SMSGroup.Entity;

            set
            {
                SMSGroup previousValue = _SMSGroup.Entity;
                if (((previousValue != value)
                            || (_SMSGroup.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _SMSGroup.Entity = null;
                        previousValue.SMSGroupMembers.Remove(this);
                    }

                    _SMSGroup.Entity = value;
                    if (value != null)
                    {
                        value.SMSGroupMembers.Add(this);

                        _GroupID = value.Id;

                    }

                    else
                    {
                        _GroupID = default(int);

                    }

                    SendPropertyChanged("SMSGroup");
                }
            }
        }

        [Association(Name = "FK_SMSGroupMembers_Users", Storage = "_User", ThisKey = "UserID", IsForeignKey = true)]
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
                        previousValue.SMSGroupMembers.Remove(this);
                    }

                    _User.Entity = value;
                    if (value != null)
                    {
                        value.SMSGroupMembers.Add(this);

                        _UserID = value.UserId;

                    }

                    else
                    {
                        _UserID = default(int);

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
