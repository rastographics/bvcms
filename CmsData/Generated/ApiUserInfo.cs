using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ApiUserInfo")]
    public partial class ApiUserInfo : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _ApiKey;

        private string _IpAddress;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnApiKeyChanging(string value);
        partial void OnApiKeyChanged();

        partial void OnIpAddressChanging(string value);
        partial void OnIpAddressChanged();

        #endregion

        public ApiUserInfo()
        {
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

        [Column(Name = "ApiKey", UpdateCheck = UpdateCheck.Never, Storage = "_ApiKey", DbType = "nvarchar(50) NOT NULL")]
        public string ApiKey
        {
            get => _ApiKey;

            set
            {
                if (_ApiKey != value)
                {
                    OnApiKeyChanging(value);
                    SendPropertyChanging();
                    _ApiKey = value;
                    SendPropertyChanged("ApiKey");
                    OnApiKeyChanged();
                }
            }
        }

        [Column(Name = "IpAddress", UpdateCheck = UpdateCheck.Never, Storage = "_IpAddress", DbType = "nvarchar(20) NOT NULL")]
        public string IpAddress
        {
            get => _IpAddress;

            set
            {
                if (_IpAddress != value)
                {
                    OnIpAddressChanging(value);
                    SendPropertyChanging();
                    _IpAddress = value;
                    SendPropertyChanged("IpAddress");
                    OnIpAddressChanged();
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
