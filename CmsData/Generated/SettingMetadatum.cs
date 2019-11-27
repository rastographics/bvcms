using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.SettingMetadata")]
    public partial class SettingMetadatum : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private string _SettingId;

        private string _DisplayName;

        private string _Description;

        private int? _DataType;

        private int? _SettingTypeId;

        private int? _SettingCategoryId;

        private EntityRef<Setting> _Setting;

        private EntityRef<SettingCategory> _SettingCategory;

        private EntityRef<SettingType> _SettingType;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnSettingIdChanging(string value);
        partial void OnSettingIdChanged();

        partial void OnDisplayNameChanging(string value);
        partial void OnDisplayNameChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnDataTypeChanging(int? value);
        partial void OnDataTypeChanged();

        partial void OnSettingTypeIdChanging(int? value);
        partial void OnSettingTypeIdChanged();

        partial void OnSettingCategoryIdChanging(int? value);
        partial void OnSettingCategoryIdChanged();

        #endregion

        public SettingMetadatum()
        {
            _Setting = default(EntityRef<Setting>);

            _SettingCategory = default(EntityRef<SettingCategory>);

            _SettingType = default(EntityRef<SettingType>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "SettingId", UpdateCheck = UpdateCheck.Never, Storage = "_SettingId", DbType = "nvarchar(50) NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public string SettingId
        {
            get => _SettingId;

            set
            {
                if (_SettingId != value)
                {
                    if (_Setting.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnSettingIdChanging(value);
                    SendPropertyChanging();
                    _SettingId = value;
                    SendPropertyChanged("SettingId");
                    OnSettingIdChanged();
                }
            }
        }

        [Column(Name = "DisplayName", UpdateCheck = UpdateCheck.Never, Storage = "_DisplayName", DbType = "varchar")]
        public string DisplayName
        {
            get => _DisplayName;

            set
            {
                if (_DisplayName != value)
                {
                    OnDisplayNameChanging(value);
                    SendPropertyChanging();
                    _DisplayName = value;
                    SendPropertyChanged("DisplayName");
                    OnDisplayNameChanged();
                }
            }
        }

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "varchar")]
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

        [Column(Name = "DataType", UpdateCheck = UpdateCheck.Never, Storage = "_DataType", DbType = "int")]
        public int? DataType
        {
            get => _DataType;

            set
            {
                if (_DataType != value)
                {
                    OnDataTypeChanging(value);
                    SendPropertyChanging();
                    _DataType = value;
                    SendPropertyChanged("DataType");
                    OnDataTypeChanged();
                }
            }
        }

        [Column(Name = "SettingTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_SettingTypeId", DbType = "int")]
        [IsForeignKey]
        public int? SettingTypeId
        {
            get => _SettingTypeId;

            set
            {
                if (_SettingTypeId != value)
                {
                    if (_SettingType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnSettingTypeIdChanging(value);
                    SendPropertyChanging();
                    _SettingTypeId = value;
                    SendPropertyChanged("SettingTypeId");
                    OnSettingTypeIdChanged();
                }
            }
        }

        [Column(Name = "SettingCategoryId", UpdateCheck = UpdateCheck.Never, Storage = "_SettingCategoryId", DbType = "int")]
        [IsForeignKey]
        public int? SettingCategoryId
        {
            get => _SettingCategoryId;

            set
            {
                if (_SettingCategoryId != value)
                {
                    if (_SettingCategory.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnSettingCategoryIdChanging(value);
                    SendPropertyChanging();
                    _SettingCategoryId = value;
                    SendPropertyChanged("SettingCategoryId");
                    OnSettingCategoryIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_SettingMetadata_Setting", Storage = "_Setting", ThisKey = "SettingId", IsForeignKey = true)]
        public Setting Setting
        {
            get => _Setting.Entity;

            set
            {
                Setting previousValue = _Setting.Entity;
                if (((previousValue != value)
                            || (_Setting.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Setting.Entity = null;
                        previousValue.SettingMetadatas.Remove(this);
                    }

                    _Setting.Entity = value;
                    if (value != null)
                    {
                        value.SettingMetadatas.Add(this);

                        _SettingId = value.Id;
                    }

                    else
                    {
                        _SettingId = default(string);
                    }

                    SendPropertyChanged("Setting");
                }
            }
        }

        [Association(Name = "FK_SettingMetadata_SettingCategory", Storage = "_SettingCategory", ThisKey = "SettingCategoryId", IsForeignKey = true)]
        public SettingCategory SettingCategory
        {
            get => _SettingCategory.Entity;

            set
            {
                SettingCategory previousValue = _SettingCategory.Entity;
                if (((previousValue != value)
                            || (_SettingCategory.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _SettingCategory.Entity = null;
                        previousValue.SettingMetadatas.Remove(this);
                    }

                    _SettingCategory.Entity = value;
                    if (value != null)
                    {
                        value.SettingMetadatas.Add(this);

                        _SettingCategoryId = value.SettingCategoryId;
                    }

                    else
                    {
                        _SettingCategoryId = default(int?);
                    }

                    SendPropertyChanged("SettingCategory");
                }
            }
        }

        [Association(Name = "FK_SettingMetadata_SettingType", Storage = "_SettingType", ThisKey = "SettingTypeId", IsForeignKey = true)]
        public SettingType SettingType
        {
            get => _SettingType.Entity;

            set
            {
                SettingType previousValue = _SettingType.Entity;
                if (((previousValue != value)
                            || (_SettingType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _SettingType.Entity = null;
                        previousValue.SettingMetadatas.Remove(this);
                    }

                    _SettingType.Entity = value;
                    if (value != null)
                    {
                        value.SettingMetadatas.Add(this);

                        _SettingTypeId = value.SettingTypeId;
                    }

                    else
                    {
                        _SettingTypeId = default(int?);
                    }

                    SendPropertyChanged("SettingType");
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
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
