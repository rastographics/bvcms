using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using CmsData.Infrastructure;

namespace CmsData
{
    [Table(Name = "dbo.ApiUserInfo")]
    public partial class ApiUserInfo : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _Id;

        private string _ApiKey;

        private string _IpAddress;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnApiKeyChanging(string value);
        partial void OnApiKeyChanged();

        partial void OnIpAddressChanging(string value);
        partial void OnIpAddressChanged();

        #endregion
        public ApiUserInfo()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get { return this._Id; }

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


        [Column(Name = "ApiKey", UpdateCheck = UpdateCheck.Never, Storage = "_ApiKey", DbType = "nvarchar(50) NOT NULL")]
        public string ApiKey
        {
            get { return this._ApiKey; }

            set
            {
                if (this._ApiKey != value)
                {

                    this.OnApiKeyChanging(value);
                    this.SendPropertyChanging();
                    this._ApiKey = value;
                    this.SendPropertyChanged("ApiKey");
                    this.OnApiKeyChanged();
                }

            }

        }


        [Column(Name = "IpAddress", UpdateCheck = UpdateCheck.Never, Storage = "_IpAddress", DbType = "nvarchar(20) NOT NULL")]
        public string IpAddress
        {
            get { return this._IpAddress; }

            set
            {
                if (this._IpAddress != value)
                {

                    this.OnIpAddressChanging(value);
                    this.SendPropertyChanging();
                    this._IpAddress = value;
                    this.SendPropertyChanged("IpAddress");
                    this.OnIpAddressChanged();
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
                this.PropertyChanging(this, emptyChangingEventArgs);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


    }

}

