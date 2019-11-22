using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.PrintJob")]
    public partial class PrintJob : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _Id;

        private DateTime _Stamp;

        private string _Data;

        private string _JsonData;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(string value);
        partial void OnIdChanged();

        partial void OnStampChanging(DateTime value);
        partial void OnStampChanged();

        partial void OnDataChanging(string value);
        partial void OnDataChanged();

        partial void OnJsonDataChanging(string value);
        partial void OnJsonDataChanged();

        #endregion

        public PrintJob()
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

        [Column(Name = "Stamp", UpdateCheck = UpdateCheck.Never, Storage = "_Stamp", DbType = "datetime NOT NULL", IsPrimaryKey = true)]
        public DateTime Stamp
        {
            get => _Stamp;

            set
            {
                if (_Stamp != value)
                {
                    OnStampChanging(value);
                    SendPropertyChanging();
                    _Stamp = value;
                    SendPropertyChanged("Stamp");
                    OnStampChanged();
                }
            }
        }

        [Column(Name = "Data", UpdateCheck = UpdateCheck.Never, Storage = "_Data", DbType = "nvarchar")]
        public string Data
        {
            get => _Data;

            set
            {
                if (_Data != value)
                {
                    OnDataChanging(value);
                    SendPropertyChanging();
                    _Data = value;
                    SendPropertyChanged("Data");
                    OnDataChanged();
                }
            }
        }

        [Column(Name = "JsonData", UpdateCheck = UpdateCheck.Never, Storage = "_JsonData", DbType = "nvarchar")]
        public string JsonData
        {
            get => _JsonData;

            set
            {
                if (_JsonData != value)
                {
                    OnJsonDataChanging(value);
                    SendPropertyChanging();
                    _JsonData = value;
                    SendPropertyChanged("JsonData");
                    OnJsonDataChanged();
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
