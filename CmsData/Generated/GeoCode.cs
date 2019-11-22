using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.GeoCodes")]
    public partial class GeoCode : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _Address;

        private double _Latitude;

        private double _Longitude;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnAddressChanging(string value);
        partial void OnAddressChanged();

        partial void OnLatitudeChanging(double value);
        partial void OnLatitudeChanged();

        partial void OnLongitudeChanging(double value);
        partial void OnLongitudeChanged();

        #endregion

        public GeoCode()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Address", UpdateCheck = UpdateCheck.Never, Storage = "_Address", DbType = "nvarchar(80) NOT NULL", IsPrimaryKey = true)]
        public string Address
        {
            get => _Address;

            set
            {
                if (_Address != value)
                {
                    OnAddressChanging(value);
                    SendPropertyChanging();
                    _Address = value;
                    SendPropertyChanged("Address");
                    OnAddressChanged();
                }
            }
        }

        [Column(Name = "Latitude", UpdateCheck = UpdateCheck.Never, Storage = "_Latitude", DbType = "float NOT NULL")]
        public double Latitude
        {
            get => _Latitude;

            set
            {
                if (_Latitude != value)
                {
                    OnLatitudeChanging(value);
                    SendPropertyChanging();
                    _Latitude = value;
                    SendPropertyChanged("Latitude");
                    OnLatitudeChanged();
                }
            }
        }

        [Column(Name = "Longitude", UpdateCheck = UpdateCheck.Never, Storage = "_Longitude", DbType = "float NOT NULL")]
        public double Longitude
        {
            get => _Longitude;

            set
            {
                if (_Longitude != value)
                {
                    OnLongitudeChanging(value);
                    SendPropertyChanging();
                    _Longitude = value;
                    SendPropertyChanged("Longitude");
                    OnLongitudeChanged();
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
