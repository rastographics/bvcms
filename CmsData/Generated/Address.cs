using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Address")]
    public partial class Address : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _Id;

        private string _AddressX;

        private string _Address2;

        private string _City;

        private string _State;

        private string _Zip;

        private bool? _BadAddress;

        private DateTime? _FromDt;

        private DateTime? _ToDt;

        private string _Type;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnAddressXChanging(string value);
        partial void OnAddressXChanged();

        partial void OnAddress2Changing(string value);
        partial void OnAddress2Changed();

        partial void OnCityChanging(string value);
        partial void OnCityChanged();

        partial void OnStateChanging(string value);
        partial void OnStateChanged();

        partial void OnZipChanging(string value);
        partial void OnZipChanged();

        partial void OnBadAddressChanging(bool? value);
        partial void OnBadAddressChanged();

        partial void OnFromDtChanging(DateTime? value);
        partial void OnFromDtChanged();

        partial void OnToDtChanging(DateTime? value);
        partial void OnToDtChanged();

        partial void OnTypeChanging(string value);
        partial void OnTypeChanged();

        #endregion
        public Address()
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


        [Column(Name = "Address", UpdateCheck = UpdateCheck.Never, Storage = "_AddressX", DbType = "nvarchar(50)")]
        public string AddressX
        {
            get => this._AddressX;

            set
            {
                if (this._AddressX != value)
                {

                    this.OnAddressXChanging(value);
                    this.SendPropertyChanging();
                    this._AddressX = value;
                    this.SendPropertyChanged("AddressX");
                    this.OnAddressXChanged();
                }

            }

        }


        [Column(Name = "Address2", UpdateCheck = UpdateCheck.Never, Storage = "_Address2", DbType = "nvarchar(50)")]
        public string Address2
        {
            get => this._Address2;

            set
            {
                if (this._Address2 != value)
                {

                    this.OnAddress2Changing(value);
                    this.SendPropertyChanging();
                    this._Address2 = value;
                    this.SendPropertyChanged("Address2");
                    this.OnAddress2Changed();
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


        [Column(Name = "State", UpdateCheck = UpdateCheck.Never, Storage = "_State", DbType = "nvarchar(50)")]
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


        [Column(Name = "Zip", UpdateCheck = UpdateCheck.Never, Storage = "_Zip", DbType = "nvarchar(50)")]
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


        [Column(Name = "BadAddress", UpdateCheck = UpdateCheck.Never, Storage = "_BadAddress", DbType = "bit")]
        public bool? BadAddress
        {
            get => this._BadAddress;

            set
            {
                if (this._BadAddress != value)
                {

                    this.OnBadAddressChanging(value);
                    this.SendPropertyChanging();
                    this._BadAddress = value;
                    this.SendPropertyChanged("BadAddress");
                    this.OnBadAddressChanged();
                }

            }

        }


        [Column(Name = "FromDt", UpdateCheck = UpdateCheck.Never, Storage = "_FromDt", DbType = "datetime")]
        public DateTime? FromDt
        {
            get => this._FromDt;

            set
            {
                if (this._FromDt != value)
                {

                    this.OnFromDtChanging(value);
                    this.SendPropertyChanging();
                    this._FromDt = value;
                    this.SendPropertyChanged("FromDt");
                    this.OnFromDtChanged();
                }

            }

        }


        [Column(Name = "ToDt", UpdateCheck = UpdateCheck.Never, Storage = "_ToDt", DbType = "datetime")]
        public DateTime? ToDt
        {
            get => this._ToDt;

            set
            {
                if (this._ToDt != value)
                {

                    this.OnToDtChanging(value);
                    this.SendPropertyChanging();
                    this._ToDt = value;
                    this.SendPropertyChanged("ToDt");
                    this.OnToDtChanged();
                }

            }

        }


        [Column(Name = "Type", UpdateCheck = UpdateCheck.Never, Storage = "_Type", DbType = "nvarchar(50)")]
        public string Type
        {
            get => this._Type;

            set
            {
                if (this._Type != value)
                {

                    this.OnTypeChanging(value);
                    this.SendPropertyChanging();
                    this._Type = value;
                    this.SendPropertyChanged("Type");
                    this.OnTypeChanged();
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

