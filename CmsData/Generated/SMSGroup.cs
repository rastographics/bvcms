using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.SMSGroups")]
    public partial class SMSGroup : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            _SMSGroupMembers = new EntitySet<SMSGroupMember>(new Action<SMSGroupMember>(attach_SMSGroupMembers), new Action<SMSGroupMember>(detach_SMSGroupMembers));

            _SMSLists = new EntitySet<SMSList>(new Action<SMSList>(attach_SMSLists), new Action<SMSList>(detach_SMSLists));

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

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(50) NOT NULL")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    OnNameChanging(value);
                    SendPropertyChanging();
                    _Name = value;
                    SendPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar NOT NULL")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    OnDescriptionChanging(value);
                    SendPropertyChanging();
                    _Description = value;
                    SendPropertyChanged("Description");
                    OnDescriptionChanged();
                }
            }
        }

        [Column(Name = "SystemFlag", UpdateCheck = UpdateCheck.Never, Storage = "_SystemFlag", DbType = "bit NOT NULL")]
        public bool SystemFlag
        {
            get => _SystemFlag;

            set
            {
                if (_SystemFlag != value)
                {
                    OnSystemFlagChanging(value);
                    SendPropertyChanging();
                    _SystemFlag = value;
                    SendPropertyChanged("SystemFlag");
                    OnSystemFlagChanged();
                }
            }
        }

        [Column(Name = "IsDeleted", UpdateCheck = UpdateCheck.Never, Storage = "_IsDeleted", DbType = "bit NOT NULL")]
        public bool IsDeleted
        {
            get => _IsDeleted;

            set
            {
                if (_IsDeleted != value)
                {
                    OnIsDeletedChanging(value);
                    SendPropertyChanging();
                    _IsDeleted = value;
                    SendPropertyChanged("IsDeleted");
                    OnIsDeletedChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_SMSGroupMembers_SMSGroups", Storage = "_SMSGroupMembers", OtherKey = "GroupID")]
        public EntitySet<SMSGroupMember> SMSGroupMembers
           {
               get => _SMSGroupMembers;

            set => _SMSGroupMembers.Assign(value);

           }

        [Association(Name = "FK_SMSList_SMSGroups", Storage = "_SMSLists", OtherKey = "SendGroupID")]
        public EntitySet<SMSList> SMSLists
           {
               get => _SMSLists;

            set => _SMSLists.Assign(value);

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

        private void attach_SMSGroupMembers(SMSGroupMember entity)
        {
            SendPropertyChanging();
            entity.SMSGroup = this;
        }

        private void detach_SMSGroupMembers(SMSGroupMember entity)
        {
            SendPropertyChanging();
            entity.SMSGroup = null;
        }

        private void attach_SMSLists(SMSList entity)
        {
            SendPropertyChanging();
            entity.SMSGroup = this;
        }

        private void detach_SMSLists(SMSList entity)
        {
            SendPropertyChanging();
            entity.SMSGroup = null;
        }
    }
}
