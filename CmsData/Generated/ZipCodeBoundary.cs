using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ZipCodeBoundary")]
    public partial class ZipCodeBoundary : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._Id;

            set
            {
                if (this._Id != value)
                {

                    this.OnIdChanging(value);
                    this.SendPropertyChanging();
                    this._Id = value;
                    this.SendPropertyChanged("Id");
                    this.OnIdChanged();
                }

            }

        }


        [Column(Name = "State", UpdateCheck = UpdateCheck.Never, Storage = "_State", DbType = "varchar(256) NOT NULL")]
        public string State
        {
            get => this._State;

            set
            {
                if (this._State != value)
                {

                    this.OnStateChanging(value);
                    this.SendPropertyChanging();
                    this._State = value;
                    this.SendPropertyChanged("State");
                    this.OnStateChanged();
                }

            }

        }


        [Column(Name = "ZipCode", UpdateCheck = UpdateCheck.Never, Storage = "_ZipCode", DbType = "char(5) NOT NULL")]
        public string ZipCode
        {
            get => this._ZipCode;

            set
            {
                if (this._ZipCode != value)
                {

                    this.OnZipCodeChanging(value);
                    this.SendPropertyChanging();
                    this._ZipCode = value;
                    this.SendPropertyChanged("ZipCode");
                    this.OnZipCodeChanged();
                }

            }

        }


        [Column(Name = "Lat", UpdateCheck = UpdateCheck.Never, Storage = "_Lat", DbType = "float NOT NULL")]
        public double Lat
        {
            get => this._Lat;

            set
            {
                if (this._Lat != value)
                {

                    this.OnLatChanging(value);
                    this.SendPropertyChanging();
                    this._Lat = value;
                    this.SendPropertyChanged("Lat");
                    this.OnLatChanged();
                }

            }

        }


        [Column(Name = "Lon", UpdateCheck = UpdateCheck.Never, Storage = "_Lon", DbType = "float NOT NULL")]
        public double Lon
        {
            get => this._Lon;

            set
            {
                if (this._Lon != value)
                {

                    this.OnLonChanging(value);
                    this.SendPropertyChanging();
                    this._Lon = value;
                    this.SendPropertyChanged("Lon");
                    this.OnLonChanged();
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

