using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ZipCodes")]
    public partial class ZipCode : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._Zip;

            set
            {
                if (this._Zip != value)
                {

                    this.OnZipChanging(value);
                    this.SendPropertyChanging();
                    this._Zip = value;
                    this.SendPropertyChanged("Zip");
                    this.OnZipChanged();
                }

            }

        }


        [Column(Name = "state", UpdateCheck = UpdateCheck.Never, Storage = "_State", DbType = "char(2)")]
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


        [Column(Name = "City", UpdateCheck = UpdateCheck.Never, Storage = "_City", DbType = "nvarchar(50)")]
        public string City
        {
            get => this._City;

            set
            {
                if (this._City != value)
                {

                    this.OnCityChanging(value);
                    this.SendPropertyChanging();
                    this._City = value;
                    this.SendPropertyChanged("City");
                    this.OnCityChanged();
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

