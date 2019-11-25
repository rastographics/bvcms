using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.GatewayConfigurationTemplate")]
    public partial class GatewayConfigurationTemplate
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _GatewayDetailId;
        private int _GatewayId;
        private string _GatewayDetailName;
        private string _GatewayDetailValue;
        private bool _IsBoolean;

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

        partial void OnIsBooleanChanging(bool value);
        partial void OnIsBooleanChanged();
        #endregion

        public GatewayConfigurationTemplate()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "GatewayDetailId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayDetailId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int GatewayDetailId
        {
            get => _GatewayDetailId;

            set
            {
                if (_GatewayDetailId != value)
                {
                    OnGatewayDetailIdChanging(value);
                    SendPropertyChanging();
                    _GatewayDetailId = value;
                    SendPropertyChanged("GatewayDetailId");
                    OnGatewayDetailIdChanged();
                }
            }
        }

        [Column(Name = "GatewayId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int GatewayId
        {
            get => _GatewayId;

            set
            {
                if (_GatewayId != value)
                {
                    if (_Gateways.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnGatewayIdChanging(value);
                    SendPropertyChanging();
                    _GatewayId = value;
                    SendPropertyChanged("GatewayId");
                    OnGatewayIdChanged();
                }
            }
        }

        [Column(Name = "GatewayDetailName", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayDetailName", AutoSync = AutoSync.OnInsert, DbType = "nvarchar NOT NULL")]
        public string GatewayDetailName
        {
            get => _GatewayDetailName;

            set
            {
                if (_GatewayDetailName != value)
                {
                    OnGatewayDetailNameChanging(value);
                    SendPropertyChanging();
                    _GatewayDetailName = value;
                    SendPropertyChanged("GatewayDetailName");
                    OnGatewayDetailNameChanged();
                }
            }
        }

        [Column(Name = "GatewayDetailValue", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayDetailValue", AutoSync = AutoSync.OnInsert, DbType = "nvarchar NOT NULL")]
        public string GatewayDetailValue
        {
            get => _GatewayDetailValue;

            set
            {
                if (_GatewayDetailValue != value)
                {
                    OnGatewayDetailValueChanging(value);
                    SendPropertyChanging();
                    _GatewayDetailValue = value;
                    SendPropertyChanged("GatewayDetailValue");
                    OnGatewayDetailValueChanged();
                }
            }
        }

        [Column(Name = "IsBoolean", UpdateCheck = UpdateCheck.Never, Storage = "_IsBoolean", DbType = "bit NOT NULL")]
        public bool IsBoolean
        {
            get => _IsBoolean;

            set
            {
                if (_IsBoolean != value)
                {
                    OnIsBooleanChanging(value);
                    SendPropertyChanging();
                    _IsBoolean = value;
                    SendPropertyChanged("IsBoolean");
                    OnIsBooleanChanged();
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
