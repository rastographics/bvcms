using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Promotion")]
    public partial class Promotion : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int? _FromDivId;

        private int? _ToDivId;

        private string _Description;

        private string _Sort;

        private EntityRef<Division> _FromDivision;

        private EntityRef<Division> _ToDivision;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnFromDivIdChanging(int? value);
        partial void OnFromDivIdChanged();

        partial void OnToDivIdChanging(int? value);
        partial void OnToDivIdChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnSortChanging(string value);
        partial void OnSortChanged();

        #endregion

        public Promotion()
        {
            _FromDivision = default(EntityRef<Division>);

            _ToDivision = default(EntityRef<Division>);

            OnCreated();
        }

        #region Columns

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

        [Column(Name = "FromDivId", UpdateCheck = UpdateCheck.Never, Storage = "_FromDivId", DbType = "int")]
        [IsForeignKey]
        public int? FromDivId
        {
            get => _FromDivId;

            set
            {
                if (_FromDivId != value)
                {
                    if (_FromDivision.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnFromDivIdChanging(value);
                    SendPropertyChanging();
                    _FromDivId = value;
                    SendPropertyChanged("FromDivId");
                    OnFromDivIdChanged();
                }
            }
        }

        [Column(Name = "ToDivId", UpdateCheck = UpdateCheck.Never, Storage = "_ToDivId", DbType = "int")]
        [IsForeignKey]
        public int? ToDivId
        {
            get => _ToDivId;

            set
            {
                if (_ToDivId != value)
                {
                    if (_ToDivision.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnToDivIdChanging(value);
                    SendPropertyChanging();
                    _ToDivId = value;
                    SendPropertyChanged("ToDivId");
                    OnToDivIdChanged();
                }
            }
        }

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(200)")]
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

        [Column(Name = "Sort", UpdateCheck = UpdateCheck.Never, Storage = "_Sort", DbType = "nvarchar(10)")]
        public string Sort
        {
            get => _Sort;

            set
            {
                if (_Sort != value)
                {
                    OnSortChanging(value);
                    SendPropertyChanging();
                    _Sort = value;
                    SendPropertyChanged("Sort");
                    OnSortChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FromPromotions__FromDivision", Storage = "_FromDivision", ThisKey = "FromDivId", IsForeignKey = true)]
        public Division FromDivision
        {
            get => _FromDivision.Entity;

            set
            {
                Division previousValue = _FromDivision.Entity;
                if (((previousValue != value)
                            || (_FromDivision.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _FromDivision.Entity = null;
                        previousValue.FromPromotions.Remove(this);
                    }

                    _FromDivision.Entity = value;
                    if (value != null)
                    {
                        value.FromPromotions.Add(this);

                        _FromDivId = value.Id;

                    }

                    else
                    {
                        _FromDivId = default(int?);

                    }

                    SendPropertyChanged("FromDivision");
                }
            }
        }

        [Association(Name = "ToPromotions__ToDivision", Storage = "_ToDivision", ThisKey = "ToDivId", IsForeignKey = true)]
        public Division ToDivision
        {
            get => _ToDivision.Entity;

            set
            {
                Division previousValue = _ToDivision.Entity;
                if (((previousValue != value)
                            || (_ToDivision.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ToDivision.Entity = null;
                        previousValue.ToPromotions.Remove(this);
                    }

                    _ToDivision.Entity = value;
                    if (value != null)
                    {
                        value.ToPromotions.Add(this);

                        _ToDivId = value.Id;

                    }

                    else
                    {
                        _ToDivId = default(int?);

                    }

                    SendPropertyChanged("ToDivision");
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
