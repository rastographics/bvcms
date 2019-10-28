using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.GeoCodes")]
    public partial class GeoCode : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._Address;

            set
            {
                if (this._Address != value)
                {

                    this.OnAddressChanging(value);
                    this.SendPropertyChanging();
                    this._Address = value;
                    this.SendPropertyChanged("Address");
                    this.OnAddressChanged();
                }

            }

        }


        [Column(Name = "Latitude", UpdateCheck = UpdateCheck.Never, Storage = "_Latitude", DbType = "float NOT NULL")]
        public double Latitude
        {
            get => this._Latitude;

            set
            {
                if (this._Latitude != value)
                {

                    this.OnLatitudeChanging(value);
                    this.SendPropertyChanging();
                    this._Latitude = value;
                    this.SendPropertyChanged("Latitude");
                    this.OnLatitudeChanged();
                }

            }

        }


        [Column(Name = "Longitude", UpdateCheck = UpdateCheck.Never, Storage = "_Longitude", DbType = "float NOT NULL")]
        public double Longitude
        {
            get => this._Longitude;

            set
            {
                if (this._Longitude != value)
                {

                    this.OnLongitudeChanging(value);
                    this.SendPropertyChanging();
                    this._Longitude = value;
                    this.SendPropertyChanged("Longitude");
                    this.OnLongitudeChanged();
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
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }

}

