using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.SettingType")]
    public partial class SettingType : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs(string.Empty);

        #region Private Fields

        private int _SettingTypeId;

        private string _Name;

        private int _DisplayOrder;

        private EntitySet<SettingCategory> _SettingCategories;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnSettingTypeIdChanging(int value);
        partial void OnSettingTypeIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnDisplayOrderChanging(int value);
        partial void OnDisplayOrderChanged();

        #endregion

        public SettingType()
        {
            _SettingCategories = new EntitySet<SettingCategory>(new Action<SettingCategory>(attach_SettingCategories), new Action<SettingCategory>(detach_SettingCategories));

            OnCreated();
        }

        #region Columns

        [Column(Name = "SettingTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_SettingTypeId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int SettingTypeId
        {
            get => _SettingTypeId;

            set
            {
                if (_SettingTypeId != value)
                {
                    OnSettingTypeIdChanging(value);
                    SendPropertyChanging();
                    _SettingTypeId = value;
                    SendPropertyChanged("SettingTypeId");
                    OnSettingTypeIdChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "varchar(50) NOT NULL")]
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

        [Association(Name = "FK_SettingCategory_SettingType", Storage = "_SettingCategories", OtherKey = "SettingTypeId")]
        public EntitySet<SettingCategory> SettingCategories
        {
            get => _SettingCategories;

            set => _SettingCategories.Assign(value);
        }

        #endregion

        #region Foreign Keys

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

        private void attach_SettingCategories(SettingCategory entity)
        {
            SendPropertyChanging();
            entity.SettingType = this;
        }

        private void detach_SettingCategories(SettingCategory entity)
        {
            SendPropertyChanging();
            entity.SettingType = null;
        }
    }
}
