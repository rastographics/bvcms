using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ChangeLog")]
    public partial class ChangeLog : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _PeopleId;

        private int? _FamilyId;

        private int _UserPeopleId;

        private DateTime _Created;

        private string _Field;

        private string _Data;

        private int _Id;

        private string _Before;

        private string _After;

        private EntitySet<ChangeDetail> _ChangeDetails;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnFamilyIdChanging(int? value);
        partial void OnFamilyIdChanged();

        partial void OnUserPeopleIdChanging(int value);
        partial void OnUserPeopleIdChanged();

        partial void OnCreatedChanging(DateTime value);
        partial void OnCreatedChanged();

        partial void OnFieldChanging(string value);
        partial void OnFieldChanged();

        partial void OnDataChanging(string value);
        partial void OnDataChanged();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnBeforeChanging(string value);
        partial void OnBeforeChanged();

        partial void OnAfterChanging(string value);
        partial void OnAfterChanged();

        #endregion

        public ChangeLog()
        {
            _ChangeDetails = new EntitySet<ChangeDetail>(new Action<ChangeDetail>(attach_ChangeDetails), new Action<ChangeDetail>(detach_ChangeDetails));

            OnCreated();
        }

        #region Columns

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL")]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "FamilyId", UpdateCheck = UpdateCheck.Never, Storage = "_FamilyId", DbType = "int")]
        public int? FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    OnFamilyIdChanging(value);
                    SendPropertyChanging();
                    _FamilyId = value;
                    SendPropertyChanged("FamilyId");
                    OnFamilyIdChanged();
                }
            }
        }

        [Column(Name = "UserPeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_UserPeopleId", DbType = "int NOT NULL")]
        public int UserPeopleId
        {
            get => _UserPeopleId;

            set
            {
                if (_UserPeopleId != value)
                {
                    OnUserPeopleIdChanging(value);
                    SendPropertyChanging();
                    _UserPeopleId = value;
                    SendPropertyChanged("UserPeopleId");
                    OnUserPeopleIdChanged();
                }
            }
        }

        [Column(Name = "Created", UpdateCheck = UpdateCheck.Never, Storage = "_Created", DbType = "datetime NOT NULL")]
        public DateTime Created
        {
            get => _Created;

            set
            {
                if (_Created != value)
                {
                    OnCreatedChanging(value);
                    SendPropertyChanging();
                    _Created = value;
                    SendPropertyChanged("Created");
                    OnCreatedChanged();
                }
            }
        }

        [Column(Name = "Field", UpdateCheck = UpdateCheck.Never, Storage = "_Field", DbType = "nvarchar(50)")]
        public string Field
        {
            get => _Field;

            set
            {
                if (_Field != value)
                {
                    OnFieldChanging(value);
                    SendPropertyChanging();
                    _Field = value;
                    SendPropertyChanged("Field");
                    OnFieldChanged();
                }
            }
        }

        [Column(Name = "Data", UpdateCheck = UpdateCheck.Never, Storage = "_Data", DbType = "nvarchar")]
        public string Data
        {
            get => _Data;

            set
            {
                if (_Data != value)
                {
                    OnDataChanging(value);
                    SendPropertyChanging();
                    _Data = value;
                    SendPropertyChanged("Data");
                    OnDataChanged();
                }
            }
        }

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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

        [Column(Name = "Before", UpdateCheck = UpdateCheck.Never, Storage = "_Before", DbType = "nvarchar")]
        public string Before
        {
            get => _Before;

            set
            {
                if (_Before != value)
                {
                    OnBeforeChanging(value);
                    SendPropertyChanging();
                    _Before = value;
                    SendPropertyChanged("Before");
                    OnBeforeChanged();
                }
            }
        }

        [Column(Name = "After", UpdateCheck = UpdateCheck.Never, Storage = "_After", DbType = "nvarchar")]
        public string After
        {
            get => _After;

            set
            {
                if (_After != value)
                {
                    OnAfterChanging(value);
                    SendPropertyChanging();
                    _After = value;
                    SendPropertyChanged("After");
                    OnAfterChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_ChangeDetails_ChangeLog", Storage = "_ChangeDetails", OtherKey = "Id")]
        public EntitySet<ChangeDetail> ChangeDetails
           {
               get => _ChangeDetails;

            set => _ChangeDetails.Assign(value);

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

        private void attach_ChangeDetails(ChangeDetail entity)
        {
            SendPropertyChanging();
            entity.ChangeLog = this;
        }

        private void detach_ChangeDetails(ChangeDetail entity)
        {
            SendPropertyChanging();
            entity.ChangeLog = null;
        }
    }
}
