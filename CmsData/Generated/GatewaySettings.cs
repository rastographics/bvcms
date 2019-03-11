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
    [Table(Name = "dbo.GatewaySettings")]
    public partial class GatewaySettings : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields
        private int _GatewaySettingId;
        private int _GatewayId;
        private int _ProcessId;

        private EntityRef<Gateways> _Gateways;
        private EntityRef<PaymmentProcess> _Process;
        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnGatewaySettingIdChanging(int value);
        partial void OnGatewaySettingIdChanged();

        partial void OnGatewayIdChanging(int value);
        partial void OnGatewayIdChanged();

        partial void OnProcessIdChanging(int value);
        partial void OnProcessIdChanged();
        #endregion

        public GatewaySettings()
        {
            OnCreated();
        }

        #region Columns
        [Column(Name = "GatewaySettingId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewaySettingId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int GatewaySettingId
        {
            get { return this._GatewaySettingId; }

            set
            {
                if (this._GatewaySettingId != value)
                {
                    this.OnGatewaySettingIdChanging(value);
                    this.SendPropertyChanging();
                    this._GatewaySettingId = value;
                    this.SendPropertyChanged("GatewaySettingId");
                    this.OnProcessIdChanged();
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

        [Column(Name = "ProcessId", UpdateCheck = UpdateCheck.Never, Storage = "_ProcessId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int ProcessId
        {
            get { return this._ProcessId; }

            set
            {
                if (this._ProcessId != value)
                {
                    if (this._Process.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnProcessIdChanging(value);
                    this.SendPropertyChanging();
                    this._ProcessId = value;
                    this.SendPropertyChanged("ProcessId");
                    this.OnProcessIdChanged();
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
