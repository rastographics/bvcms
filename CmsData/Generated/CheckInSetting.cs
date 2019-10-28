using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CheckInSettings")]
    public partial class CheckInSetting : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Name;

        private string _Settings;

        private int _Version;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnSettingsChanging(string value);
        partial void OnSettingsChanged();

        partial void OnVersionChanging(int value);
        partial void OnVersionChanged();

        #endregion

        public CheckInSetting()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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

        [Column(Name = "name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(50) NOT NULL")]
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

        [Column(Name = "settings", UpdateCheck = UpdateCheck.Never, Storage = "_Settings", DbType = "nvarchar NOT NULL")]
        public string Settings
        {
            get => _Settings;

            set
            {
                if (_Settings != value)
                {
                    OnSettingsChanging(value);
                    SendPropertyChanging();
                    _Settings = value;
                    SendPropertyChanged("Settings");
                    OnSettingsChanged();
                }
            }
        }

        [Column(Name = "version", UpdateCheck = UpdateCheck.Never, Storage = "_Version", DbType = "int NOT NULL")]
        public int Version
        {
            get => _Version;

            set
            {
                if (_Version != value)
                {
                    OnVersionChanging(value);
                    SendPropertyChanging();
                    _Version = value;
                    SendPropertyChanged("Version");
                    OnVersionChanged();
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
