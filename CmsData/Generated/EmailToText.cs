using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.EmailToText")]
    public partial class EmailToText : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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


        [Column(Name = "Carrier", UpdateCheck = UpdateCheck.Never, Storage = "_Carrier", DbType = "nvarchar(50)")]
        public string Carrier
        {
            get => this._Carrier;

            set
            {
                if (this._Carrier != value)
                {

                    this.OnCarrierChanging(value);
                    this.SendPropertyChanging();
                    this._Carrier = value;
                    this.SendPropertyChanged("Carrier");
                    this.OnCarrierChanged();
                }

            }

        }


        [Column(Name = "domain", UpdateCheck = UpdateCheck.Never, Storage = "_Domain", DbType = "nvarchar(50)")]
        public string Domain
        {
            get => this._Domain;

            set
            {
                if (this._Domain != value)
                {

                    this.OnDomainChanging(value);
                    this.SendPropertyChanging();
                    this._Domain = value;
                    this.SendPropertyChanged("Domain");
                    this.OnDomainChanged();
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

