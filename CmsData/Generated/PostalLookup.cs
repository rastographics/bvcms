using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.PostalLookup")]
    public partial class PostalLookup : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private string _PostalCode;

        private string _CityName;

        private string _StateCode;

        private string _CountryName;

        private int? _ResCodeId;

        private bool? _Hardwired;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnPostalCodeChanging(string value);
        partial void OnPostalCodeChanged();

        partial void OnCityNameChanging(string value);
        partial void OnCityNameChanged();

        partial void OnStateCodeChanging(string value);
        partial void OnStateCodeChanged();

        partial void OnCountryNameChanging(string value);
        partial void OnCountryNameChanged();

        partial void OnResCodeIdChanging(int? value);
        partial void OnResCodeIdChanged();

        partial void OnHardwiredChanging(bool? value);
        partial void OnHardwiredChanged();

        #endregion
        public PostalLookup()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "PostalCode", UpdateCheck = UpdateCheck.Never, Storage = "_PostalCode", DbType = "nvarchar(15) NOT NULL", IsPrimaryKey = true)]
        public string PostalCode
        {
            get => this._PostalCode;

            set
            {
                if (this._PostalCode != value)
                {

                    this.OnPostalCodeChanging(value);
                    this.SendPropertyChanging();
                    this._PostalCode = value;
                    this.SendPropertyChanged("PostalCode");
                    this.OnPostalCodeChanged();
                }

            }

        }


        [Column(Name = "CityName", UpdateCheck = UpdateCheck.Never, Storage = "_CityName", DbType = "nvarchar(20)")]
        public string CityName
        {
            get => this._CityName;

            set
            {
                if (this._CityName != value)
                {

                    this.OnCityNameChanging(value);
                    this.SendPropertyChanging();
                    this._CityName = value;
                    this.SendPropertyChanged("CityName");
                    this.OnCityNameChanged();
                }

            }

        }


        [Column(Name = "StateCode", UpdateCheck = UpdateCheck.Never, Storage = "_StateCode", DbType = "nvarchar(20)")]
        public string StateCode
        {
            get => this._StateCode;

            set
            {
                if (this._StateCode != value)
                {

                    this.OnStateCodeChanging(value);
                    this.SendPropertyChanging();
                    this._StateCode = value;
                    this.SendPropertyChanged("StateCode");
                    this.OnStateCodeChanged();
                }

            }

        }


        [Column(Name = "CountryName", UpdateCheck = UpdateCheck.Never, Storage = "_CountryName", DbType = "nvarchar(30)")]
        public string CountryName
        {
            get => this._CountryName;

            set
            {
                if (this._CountryName != value)
                {

                    this.OnCountryNameChanging(value);
                    this.SendPropertyChanging();
                    this._CountryName = value;
                    this.SendPropertyChanged("CountryName");
                    this.OnCountryNameChanged();
                }

            }

        }


        [Column(Name = "ResCodeId", UpdateCheck = UpdateCheck.Never, Storage = "_ResCodeId", DbType = "int")]
        public int? ResCodeId
        {
            get => this._ResCodeId;

            set
            {
                if (this._ResCodeId != value)
                {

                    this.OnResCodeIdChanging(value);
                    this.SendPropertyChanging();
                    this._ResCodeId = value;
                    this.SendPropertyChanged("ResCodeId");
                    this.OnResCodeIdChanged();
                }

            }

        }


        [Column(Name = "Hardwired", UpdateCheck = UpdateCheck.Never, Storage = "_Hardwired", DbType = "bit")]
        public bool? Hardwired
        {
            get => this._Hardwired;

            set
            {
                if (this._Hardwired != value)
                {

                    this.OnHardwiredChanging(value);
                    this.SendPropertyChanging();
                    this._Hardwired = value;
                    this.SendPropertyChanged("Hardwired");
                    this.OnHardwiredChanged();
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

