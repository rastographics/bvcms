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
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._GatewayDetailId;

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
            get => this._GatewayId;

            set
            {
                if (this._GatewayId != value)
                {
                    if (this._Gateways.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

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
            get => this._GatewayDetailName;

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
            get => this._GatewayDetailValue;

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

        [Column(Name = "IsBoolean", UpdateCheck = UpdateCheck.Never, Storage = "_IsBoolean", DbType = "bit NOT NULL")]
        public bool IsBoolean
        {
            get => this._IsBoolean;

            set
            {
                if (this._IsBoolean != value)
                {
                    this.OnIsBooleanChanging(value);
                    this.SendPropertyChanging();
                    this._IsBoolean = value;
                    this.SendPropertyChanged("IsBoolean");
                    this.OnIsBooleanChanged();
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
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
