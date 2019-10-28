using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Setting")]
    public partial class Setting : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _Id;

        private string _SettingX;

        private bool? _System;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(string value);
        partial void OnIdChanged();

        partial void OnSettingXChanging(string value);
        partial void OnSettingXChanged();

        partial void OnSystemChanging(bool? value);
        partial void OnSystemChanged();

        #endregion

        public Setting()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "nvarchar(50) NOT NULL", IsPrimaryKey = true)]
        public string Id
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

        [Column(Name = "Setting", UpdateCheck = UpdateCheck.Never, Storage = "_SettingX", DbType = "nvarchar")]
        public string SettingX
        {
            get => _SettingX;

            set
            {
                if (_SettingX != value)
                {
                    OnSettingXChanging(value);
                    SendPropertyChanging();
                    _SettingX = value;
                    SendPropertyChanged("SettingX");
                    OnSettingXChanged();
                }
            }
        }

        [Column(Name = "System", UpdateCheck = UpdateCheck.Never, Storage = "_System", DbType = "bit")]
        public bool? System
        {
            get => _System;

            set
            {
                if (_System != value)
                {
                    OnSystemChanging(value);
                    SendPropertyChanging();
                    _System = value;
                    SendPropertyChanged("System");
                    OnSystemChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

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
    }
}
