using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.SettingCategory")]
    public partial class SettingCategory : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs(string.Empty);

        #region Private Fields

        private int _SettingCategoryId;

        private string _Name;

        private int _SettingTypeId;

        private int _DisplayOrder;

        private EntitySet<SettingMetadatum> _SettingMetadatas;

        private EntityRef<SettingType> _SettingType;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnSettingCategoryIdChanging(int value);
        partial void OnSettingCategoryIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnSettingTypeIdChanging(int value);
        partial void OnSettingTypeIdChanged();

        partial void OnDisplayOrderChanging(int value);
        partial void OnDisplayOrderChanged();

        #endregion

        public SettingCategory()
        {
            _SettingMetadatas = new EntitySet<SettingMetadatum>(new Action<SettingMetadatum>(attach_SettingMetadatas), new Action<SettingMetadatum>(detach_SettingMetadatas));

            _SettingType = default(EntityRef<SettingType>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "SettingCategoryId", UpdateCheck = UpdateCheck.Never, Storage = "_SettingCategoryId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int SettingCategoryId
        {
            get => _SettingCategoryId;

            set
            {
                if (_SettingCategoryId != value)
                {
                    OnSettingCategoryIdChanging(value);
                    SendPropertyChanging();
                    _SettingCategoryId = value;
                    SendPropertyChanged("SettingCategoryId");
                    OnSettingCategoryIdChanged();
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

        [Column(Name = "SettingTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_SettingTypeId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int SettingTypeId
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

        [Column(Name = "DisplayOrder", UpdateCheck = UpdateCheck.Never, Storage = "_DisplayOrder", DbType = "int NOT NULL")]
        public int DisplayOrder
        {
            get => _DisplayOrder;

            set
            {
                if (_DisplayOrder != value)
                {
                    OnDisplayOrderChanging(value);
                    SendPropertyChanging();
                    _DisplayOrder = value;
                    SendPropertyChanged("DisplayOrder");
                    OnDisplayOrderChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_SettingMetadata_SettingCategory", Storage = "_SettingMetadatas", OtherKey = "SettingCategoryId")]
        public EntitySet<SettingMetadatum> SettingMetadatas
        {
            get => _SettingMetadatas;
            set => _SettingMetadatas.Assign(value);
        }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_SettingCategory_SettingType", Storage = "_SettingType", ThisKey = "SettingTypeId", IsForeignKey = true)]
        public SettingType SettingType
        {
            get => _SettingType.Entity;

            set
            {
                SettingType previousValue = _SettingType.Entity;
                if (previousValue != value || _SettingType.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _SettingType.Entity = null;
                        previousValue.SettingCategories.Remove(this);
                    }

                    _SettingType.Entity = value;
                    if (value != null)
                    {
                        value.SettingCategories.Add(this);

                        _SettingTypeId = value.SettingTypeId;
                    }
                    else
                    {
                        _SettingTypeId = default(int);
                    }

                    SendPropertyChanged("SettingType");
                }
            }
        }

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            PropertyChanging?.Invoke(this, emptyChangingEventArgs);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void attach_SettingMetadatas(SettingMetadatum entity)
        {
            SendPropertyChanging();
            entity.SettingCategory = this;
        }

        private void detach_SettingMetadatas(SettingMetadatum entity)
        {
            SendPropertyChanging();
            entity.SettingCategory = null;
        }
    }
}
