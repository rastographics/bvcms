using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ZipCodes")]
    public partial class ZipCode : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _Zip;

        private string _State;

        private string _City;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnZipChanging(string value);
        partial void OnZipChanged();

        partial void OnStateChanging(string value);
        partial void OnStateChanged();

        partial void OnCityChanging(string value);
        partial void OnCityChanged();

        #endregion

        public ZipCode()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "zip", UpdateCheck = UpdateCheck.Never, Storage = "_Zip", DbType = "nvarchar(10) NOT NULL", IsPrimaryKey = true)]
        public string Zip
        {
            get => _Zip;

            set
            {
                if (_Zip != value)
                {
                    OnZipChanging(value);
                    SendPropertyChanging();
                    _Zip = value;
                    SendPropertyChanged("Zip");
                    OnZipChanged();
                }
            }
        }

        [Column(Name = "state", UpdateCheck = UpdateCheck.Never, Storage = "_State", DbType = "char(2)")]
        public string State
        {
            get => _State;

            set
            {
                if (_State != value)
                {
                    OnStateChanging(value);
                    SendPropertyChanging();
                    _State = value;
                    SendPropertyChanged("State");
                    OnStateChanged();
                }
            }
        }

        [Column(Name = "City", UpdateCheck = UpdateCheck.Never, Storage = "_City", DbType = "nvarchar(50)")]
        public string City
        {
            get => _City;

            set
            {
                if (_City != value)
                {
                    OnCityChanging(value);
                    SendPropertyChanging();
                    _City = value;
                    SendPropertyChanged("City");
                    OnCityChanged();
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
