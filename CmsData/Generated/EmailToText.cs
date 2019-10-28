using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.EmailToText")]
    public partial class EmailToText : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Carrier;

        private string _Domain;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnCarrierChanging(string value);
        partial void OnCarrierChanged();

        partial void OnDomainChanging(string value);
        partial void OnDomainChanged();

        #endregion

        public EmailToText()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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

        [Column(Name = "Carrier", UpdateCheck = UpdateCheck.Never, Storage = "_Carrier", DbType = "nvarchar(50)")]
        public string Carrier
        {
            get => _Carrier;

            set
            {
                if (_Carrier != value)
                {
                    OnCarrierChanging(value);
                    SendPropertyChanging();
                    _Carrier = value;
                    SendPropertyChanged("Carrier");
                    OnCarrierChanged();
                }
            }
        }

        [Column(Name = "domain", UpdateCheck = UpdateCheck.Never, Storage = "_Domain", DbType = "nvarchar(50)")]
        public string Domain
        {
            get => _Domain;

            set
            {
                if (_Domain != value)
                {
                    OnDomainChanging(value);
                    SendPropertyChanging();
                    _Domain = value;
                    SendPropertyChanged("Domain");
                    OnDomainChanged();
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
