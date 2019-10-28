using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.PostalLookup")]
    public partial class PostalLookup : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            get => _PostalCode;

            set
            {
                if (_PostalCode != value)
                {
                    OnPostalCodeChanging(value);
                    SendPropertyChanging();
                    _PostalCode = value;
                    SendPropertyChanged("PostalCode");
                    OnPostalCodeChanged();
                }
            }
        }

        [Column(Name = "CityName", UpdateCheck = UpdateCheck.Never, Storage = "_CityName", DbType = "nvarchar(20)")]
        public string CityName
        {
            get => _CityName;

            set
            {
                if (_CityName != value)
                {
                    OnCityNameChanging(value);
                    SendPropertyChanging();
                    _CityName = value;
                    SendPropertyChanged("CityName");
                    OnCityNameChanged();
                }
            }
        }

        [Column(Name = "StateCode", UpdateCheck = UpdateCheck.Never, Storage = "_StateCode", DbType = "nvarchar(20)")]
        public string StateCode
        {
            get => _StateCode;

            set
            {
                if (_StateCode != value)
                {
                    OnStateCodeChanging(value);
                    SendPropertyChanging();
                    _StateCode = value;
                    SendPropertyChanged("StateCode");
                    OnStateCodeChanged();
                }
            }
        }

        [Column(Name = "CountryName", UpdateCheck = UpdateCheck.Never, Storage = "_CountryName", DbType = "nvarchar(30)")]
        public string CountryName
        {
            get => _CountryName;

            set
            {
                if (_CountryName != value)
                {
                    OnCountryNameChanging(value);
                    SendPropertyChanging();
                    _CountryName = value;
                    SendPropertyChanged("CountryName");
                    OnCountryNameChanged();
                }
            }
        }

        [Column(Name = "ResCodeId", UpdateCheck = UpdateCheck.Never, Storage = "_ResCodeId", DbType = "int")]
        public int? ResCodeId
        {
            get => _ResCodeId;

            set
            {
                if (_ResCodeId != value)
                {
                    OnResCodeIdChanging(value);
                    SendPropertyChanging();
                    _ResCodeId = value;
                    SendPropertyChanged("ResCodeId");
                    OnResCodeIdChanged();
                }
            }
        }

        [Column(Name = "Hardwired", UpdateCheck = UpdateCheck.Never, Storage = "_Hardwired", DbType = "bit")]
        public bool? Hardwired
        {
            get => _Hardwired;

            set
            {
                if (_Hardwired != value)
                {
                    OnHardwiredChanging(value);
                    SendPropertyChanging();
                    _Hardwired = value;
                    SendPropertyChanged("Hardwired");
                    OnHardwiredChanged();
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
