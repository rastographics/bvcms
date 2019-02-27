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
    [Table(Name = "dbo.GatewayDetails")]
    public partial class GatewayDetails
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields
        private int _GatewayDetailId;
        private int _GatewayId;
        private string _GatewayDetailName;
        private string _GatewayDetailValue;
        private bool _IsDefault;

        private EntityRef<Gateways> _Gateways;
        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnGatewayDetailIdChanging(int value);
        partial void OnGatewayDetailIdChanged();

        partial void OnGatewayIdChanging(int value);
        partial void OnGatewayIdChanged();

        partial void OnGatewayDetailNameChanging(string value);
        partial void OnGatewayDetailNameChanged();

        partial void OnGatewayDetailValueChanging(string value);
        partial void OnGatewayDetailValueChanged();

        partial void OnIsDefaultChanging(bool value);
        partial void OnIsDefaultChanged();
        #endregion

        public GatewayDetails()
        {
            OnCreated();
        }

        #region Columns
        [Column(Name = "GatewayDetailId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayDetailId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int GatewayDetailId
        {
            get { return this._GatewayDetailId; }

            set
            {
                if (this._GatewayDetailId != value)
                {
                    this.OnGatewayDetailIdChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayDetailId = value;
                    this.SendPropertyChanged("GatewayDetailId");
                    this.OnGatewayDetailIdChanged();
                }
            }
        }

        [Column(Name = "GatewayId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int GatewayId
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

        [Column(Name = "GatewayDetailName", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayDetailName", AutoSync = AutoSync.OnInsert, DbType = "nvarchar NOT NULL")]
        public string GatewayDetailName
        {
            get { return this._GatewayDetailName; }

            set
            {
                if (this._GatewayDetailName != value)
                {
                    this.OnGatewayDetailNameChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayDetailName = value;
                    this.SendPropertyChanged("GatewayDetailName");
                    this.OnGatewayDetailNameChanged();
                }
            }
        }

        [Column(Name = "GatewayDetailValue", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayDetailValue", AutoSync = AutoSync.OnInsert, DbType = "nvarchar NOT NULL")]
        public string GatewayDetailValue
        {
            get { return this._GatewayDetailValue; }

            set
            {
                if (this._GatewayDetailValue != value)
                {
                    this.OnGatewayDetailValueChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayDetailValue = value;
                    this.SendPropertyChanged("GatewayDetailValue");
                    this.OnGatewayDetailValueChanged();
                }
            }
        }

        [Column(Name = "IsDefault", UpdateCheck = UpdateCheck.Never, Storage = "_IsDefault", DbType = "bit NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public bool IsDefault
        {
            get { return this._IsDefault; }

            set
            {
                if (this._IsDefault != value)
                {
                    if (this._Gateways.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnIsDefaultChanging(value);
                    this.SendPropertyChanging();
                    this._IsDefault = value;
                    this.SendPropertyChanged("IsDefault");
                    this.OnIsDefaultChanged();
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
