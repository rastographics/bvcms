using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.FamilyCheckinLock")]
    public partial class FamilyCheckinLock : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _FamilyId;

        private bool _Locked;

        private DateTime _Created;

        private EntityRef<Family> _Family;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnFamilyIdChanging(int value);
        partial void OnFamilyIdChanged();

        partial void OnLockedChanging(bool value);
        partial void OnLockedChanged();

        partial void OnCreatedChanging(DateTime value);
        partial void OnCreatedChanged();

        #endregion

        public FamilyCheckinLock()
        {
            _Family = default(EntityRef<Family>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "FamilyId", UpdateCheck = UpdateCheck.Never, Storage = "_FamilyId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    if (_Family.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnFamilyIdChanging(value);
                    SendPropertyChanging();
                    _FamilyId = value;
                    SendPropertyChanged("FamilyId");
                    OnFamilyIdChanged();
                }
            }
        }

        [Column(Name = "Locked", UpdateCheck = UpdateCheck.Never, Storage = "_Locked", DbType = "bit NOT NULL")]
        public bool Locked
        {
            get => _Locked;

            set
            {
                if (_Locked != value)
                {
                    OnLockedChanging(value);
                    SendPropertyChanging();
                    _Locked = value;
                    SendPropertyChanged("Locked");
                    OnLockedChanged();
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

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_FamilyCheckinLock_FamilyCheckinLock1", Storage = "_Family", ThisKey = "FamilyId", IsForeignKey = true)]
        public Family Family
        {
            get => _Family.Entity;

            set
            {
                Family previousValue = _Family.Entity;
                if (((previousValue != value)
                            || (_Family.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Family.Entity = null;
                        previousValue.FamilyCheckinLocks.Remove(this);
                    }

                    _Family.Entity = value;
                    if (value != null)
                    {
                        value.FamilyCheckinLocks.Add(this);

                        _FamilyId = value.FamilyId;

                    }

                    else
                    {
                        _FamilyId = default(int);

                    }

                    SendPropertyChanged("Family");
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
