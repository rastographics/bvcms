using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ChangeDetails")]
    public partial class ChangeDetail : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Field;

        private string _Before;

        private string _After;

        private EntityRef<ChangeLog> _ChangeLog;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnFieldChanging(string value);
        partial void OnFieldChanged();

        partial void OnBeforeChanging(string value);
        partial void OnBeforeChanged();

        partial void OnAfterChanging(string value);
        partial void OnAfterChanged();

        #endregion

        public ChangeDetail()
        {
            _ChangeLog = default(EntityRef<ChangeLog>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    if (_ChangeLog.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

        [Column(Name = "Field", UpdateCheck = UpdateCheck.Never, Storage = "_Field", DbType = "nvarchar(50) NOT NULL", IsPrimaryKey = true)]
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

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_ChangeDetails_ChangeLog", Storage = "_ChangeLog", ThisKey = "Id", IsForeignKey = true)]
        public ChangeLog ChangeLog
        {
            get => _ChangeLog.Entity;

            set
            {
                ChangeLog previousValue = _ChangeLog.Entity;
                if (((previousValue != value)
                            || (_ChangeLog.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ChangeLog.Entity = null;
                        previousValue.ChangeDetails.Remove(this);
                    }

                    _ChangeLog.Entity = value;
                    if (value != null)
                    {
                        value.ChangeDetails.Add(this);

                        _Id = value.Id;

                    }

                    else
                    {
                        _Id = default(int);

                    }

                    SendPropertyChanged("ChangeLog");
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
