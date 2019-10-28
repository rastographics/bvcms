using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.GatewayDetails")]
    public partial class GatewayDetails
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields
        private int _GatewayDetailId;
        private int _GatewayAccountId;
        private string _GatewayDetailName;
        private string _GatewayDetailValue;
        private bool _IsDefault;
        private bool _IsBoolean;

        private EntityRef<GatewayAccount> _GatewayAccount;
        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnGatewayDetailIdChanging(int value);
        partial void OnGatewayDetailIdChanged();

        partial void OnGatewayAccountIdChanging(int value);
        partial void OnGatewayAccountIdChanged();

        partial void OnGatewayDetailNameChanging(string value);
        partial void OnGatewayDetailNameChanged();

        partial void OnGatewayDetailValueChanging(string value);
        partial void OnGatewayDetailValueChanged();

        partial void OnIsDefaultChanging(bool value);
        partial void OnIsDefaultChanged();

        partial void OnIsBooleanChanging(bool value);
        partial void OnIsBooleanChanged();
        #endregion

        public GatewayDetails()
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

        [Column(Name = "GatewayAccountId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayAccountId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int GatewayAccountId
        {
            get => this._GatewayAccountId;

            set
            {
                if (this._GatewayAccountId != value)
                {
                    if (this._GatewayAccount.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    this.OnGatewayAccountIdChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayAccountId = value;
                    this.SendPropertyChanged("GatewayAccountId");
                    this.OnGatewayAccountIdChanged();
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

        [Column(Name = "IsDefault", UpdateCheck = UpdateCheck.Never, Storage = "_IsDefault", DbType = "bit NOT NULL", IsPrimaryKey = true)]
        public bool IsDefault
        {
            get => this._IsDefault;

            set
            {
                if (this._IsDefault != value)
                {
                    this.OnIsDefaultChanging(value);
                    this.SendPropertyChanging();
                    this._IsDefault = value;
                    this.SendPropertyChanged("IsDefault");
                    this.OnIsDefaultChanged();
                }
            }
        }

        [Column(Name = "IsBoolean", UpdateCheck = UpdateCheck.Never, Storage = "_IsBoolean", DbType = "bit NOT NULL", IsPrimaryKey = true)]
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
