using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Address")]
    public partial class Address : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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

        [Column(Name = "Address", UpdateCheck = UpdateCheck.Never, Storage = "_AddressX", DbType = "nvarchar(50)")]
        public string AddressX
        {
            get => _AddressX;

            set
            {
                if (_AddressX != value)
                {
                    OnAddressXChanging(value);
                    SendPropertyChanging();
                    _AddressX = value;
                    SendPropertyChanged("AddressX");
                    OnAddressXChanged();
                }
            }
        }

        [Column(Name = "Address2", UpdateCheck = UpdateCheck.Never, Storage = "_Address2", DbType = "nvarchar(50)")]
        public string Address2
        {
            get => _Address2;

            set
            {
                if (_Address2 != value)
                {
                    OnAddress2Changing(value);
                    SendPropertyChanging();
                    _Address2 = value;
                    SendPropertyChanged("Address2");
                    OnAddress2Changed();
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

        [Column(Name = "State", UpdateCheck = UpdateCheck.Never, Storage = "_State", DbType = "nvarchar(50)")]
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

        [Column(Name = "Zip", UpdateCheck = UpdateCheck.Never, Storage = "_Zip", DbType = "nvarchar(50)")]
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

        [Column(Name = "BadAddress", UpdateCheck = UpdateCheck.Never, Storage = "_BadAddress", DbType = "bit")]
        public bool? BadAddress
        {
            get => _BadAddress;

            set
            {
                if (_BadAddress != value)
                {
                    OnBadAddressChanging(value);
                    SendPropertyChanging();
                    _BadAddress = value;
                    SendPropertyChanged("BadAddress");
                    OnBadAddressChanged();
                }
            }
        }

        [Column(Name = "FromDt", UpdateCheck = UpdateCheck.Never, Storage = "_FromDt", DbType = "datetime")]
        public DateTime? FromDt
        {
            get => _FromDt;

            set
            {
                if (_FromDt != value)
                {
                    OnFromDtChanging(value);
                    SendPropertyChanging();
                    _FromDt = value;
                    SendPropertyChanged("FromDt");
                    OnFromDtChanged();
                }
            }
        }

        [Column(Name = "ToDt", UpdateCheck = UpdateCheck.Never, Storage = "_ToDt", DbType = "datetime")]
        public DateTime? ToDt
        {
            get => _ToDt;

            set
            {
                if (_ToDt != value)
                {
                    OnToDtChanging(value);
                    SendPropertyChanging();
                    _ToDt = value;
                    SendPropertyChanged("ToDt");
                    OnToDtChanged();
                }
            }
        }

        [Column(Name = "Type", UpdateCheck = UpdateCheck.Never, Storage = "_Type", DbType = "nvarchar(50)")]
        public string Type
        {
            get => _Type;

            set
            {
                if (_Type != value)
                {
                    OnTypeChanging(value);
                    SendPropertyChanging();
                    _Type = value;
                    SendPropertyChanged("Type");
                    OnTypeChanged();
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
