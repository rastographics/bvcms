using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ZipCodeBoundary")]
    public partial class ZipCodeBoundary : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _State;

        private string _ZipCode;

        private double _Lat;

        private double _Lon;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnStateChanging(string value);
        partial void OnStateChanged();

        partial void OnZipCodeChanging(string value);
        partial void OnZipCodeChanged();

        partial void OnLatChanging(double value);
        partial void OnLatChanged();

        partial void OnLonChanging(double value);
        partial void OnLonChanged();

        #endregion

        public ZipCodeBoundary()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "State", UpdateCheck = UpdateCheck.Never, Storage = "_State", DbType = "varchar(256) NOT NULL")]
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

        [Column(Name = "ZipCode", UpdateCheck = UpdateCheck.Never, Storage = "_ZipCode", DbType = "char(5) NOT NULL")]
        public string ZipCode
        {
            get => _ZipCode;

            set
            {
                if (_ZipCode != value)
                {
                    OnZipCodeChanging(value);
                    SendPropertyChanging();
                    _ZipCode = value;
                    SendPropertyChanged("ZipCode");
                    OnZipCodeChanged();
                }
            }
        }

        [Column(Name = "Lat", UpdateCheck = UpdateCheck.Never, Storage = "_Lat", DbType = "float NOT NULL")]
        public double Lat
        {
            get => _Lat;

            set
            {
                if (_Lat != value)
                {
                    OnLatChanging(value);
                    SendPropertyChanging();
                    _Lat = value;
                    SendPropertyChanged("Lat");
                    OnLatChanged();
                }
            }
        }

        [Column(Name = "Lon", UpdateCheck = UpdateCheck.Never, Storage = "_Lon", DbType = "float NOT NULL")]
        public double Lon
        {
            get => _Lon;

            set
            {
                if (_Lon != value)
                {
                    OnLonChanging(value);
                    SendPropertyChanging();
                    _Lon = value;
                    SendPropertyChanged("Lon");
                    OnLonChanged();
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
