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
    [Table(Name = "lookup.Gateways")]
    public partial class Gateways : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields
        private int _GatewayId;
        private string _GatewayName;
        private int _GatewayServiceTypeId;

        private EntityRef<GatewayServiceType> _GatewayServiceType;
        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnGatewayIdChanging(int value);
        partial void OnGatewayIdChanged();

        partial void OnGatewayNameChanging(string value);
        partial void OnGatewayNameChanged();

        partial void OnGatewayServiceTypeIdChanging(int value);
        partial void OnGatewayServiceTypeIdChanged();
        #endregion

        public Gateways()
        {
            OnCreated();
        }

        #region Columns
        [Column(Name = "GatewayId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int GatewayId
        {
            get { return this._GatewayId; }

            set
            {
                if (this._GatewayId != value)
                {
                    this.OnGatewayIdChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayId = value;
                    this.SendPropertyChanged("GatewayId");
                    this.OnGatewayIdChanged();
                }
            }
        }

        [Column(Name = "GatewayName", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayName", AutoSync = AutoSync.OnInsert, DbType = "nvarchar NOT NULL")]
        public string GatewayName
        {
            get { return this._GatewayName; }

            set
            {
                if (this._GatewayName != value)
                {
                    this.OnGatewayNameChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayName = value;
                    this.SendPropertyChanged("GatewayName");
                    this.OnGatewayNameChanged();
                }
            }
        }

        [Column(Name = "GatewayServiceTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayServiceTypeId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int GatewayServiceTypeId
        {
            get { return this._GatewayServiceTypeId; }

            set
            {
                if (this._GatewayServiceTypeId != value)
                {
                    if (this._GatewayServiceType.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnGatewayServiceTypeIdChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayServiceTypeId = value;
                    this.SendPropertyChanged("GatewayServiceTypeId");
                    this.OnGatewayServiceTypeIdChanged();
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
