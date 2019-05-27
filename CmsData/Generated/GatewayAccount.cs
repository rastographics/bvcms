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
    [Table(Name = "dbo.GatewayAccount")]
    public partial class GatewayAccount : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields
        private int _GatewayAccountId;
        private string _GatewayAccountName;
        private int? _GatewayId;

        private EntityRef<Gateways> _Gateways;
        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnGatewayAccountIdChanging(int value);
        partial void OnGatewayAccountIdChanged();

        partial void OnGatewayAccountNameChanging(string value);
        partial void OnGatewayAccountNameChanged();

        partial void OnGatewayIdChanging(int? value);
        partial void OnGatewayIdChanged();
        #endregion

        public GatewayAccount()
        {
            OnCreated();
        }

        #region Columns
        [Column(Name = "GatewayAccountId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayAccountId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int GatewayAccountId
        {
            get { return this._GatewayAccountId; }

            set
            {
                if (this._GatewayAccountId != value)
                {
                    this.OnGatewayAccountIdChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayAccountId = value;
                    this.SendPropertyChanged("GatewayAccountId");
                    this.OnGatewayAccountIdChanged();
                }
            }
        }

        [Column(Name = "GatewayAccountName", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayAccountName", AutoSync = AutoSync.OnInsert, DbType = "nvarchar NOT NULL")]
        public string GatewayAccountName
        {
            get { return this._GatewayAccountName; }

            set
            {
                if (this._GatewayAccountName != value)
                {
                    this.OnGatewayAccountNameChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayAccountName = value;
                    this.SendPropertyChanged("GatewayAccountName");
                    this.OnGatewayAccountNameChanged();
                }
            }
        }

        [Column(Name = "GatewayId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int? GatewayId
        {
            get { return this._GatewayId; }

            set
            {
                if (this._GatewayId != value)
                {
                    if (this._Gateways.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnGatewayIdChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayId = value;
                    this.SendPropertyChanged("GatewayId");
                    this.OnGatewayIdChanged();
                }
            }
        }
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
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
