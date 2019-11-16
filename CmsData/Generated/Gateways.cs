using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.Gateways")]
    public partial class Gateways : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            get => _GatewayId;

            set
            {
                if (_GatewayId != value)
                {
                    OnGatewayIdChanging(value);
                    SendPropertyChanging();
                    _GatewayId = value;
                    SendPropertyChanged("GatewayId");
                    OnGatewayIdChanged();
                }
            }
        }

        [Column(Name = "GatewayName", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayName", AutoSync = AutoSync.OnInsert, DbType = "nvarchar NOT NULL")]
        public string GatewayName
        {
            get => _GatewayName;

            set
            {
                if (_GatewayName != value)
                {
                    OnGatewayNameChanging(value);
                    SendPropertyChanging();
                    _GatewayName = value;
                    SendPropertyChanged("GatewayName");
                    OnGatewayNameChanged();
                }
            }
        }

        [Column(Name = "GatewayServiceTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayServiceTypeId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int GatewayServiceTypeId
        {
            get => _GatewayServiceTypeId;

            set
            {
                if (_GatewayServiceTypeId != value)
                {
                    if (_GatewayServiceType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnGatewayServiceTypeIdChanging(value);
                    SendPropertyChanging();
                    _GatewayServiceTypeId = value;
                    SendPropertyChanged("GatewayServiceTypeId");
                    OnGatewayServiceTypeIdChanged();
                }
            }
        }

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
