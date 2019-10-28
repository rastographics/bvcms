using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.GatewayAccount")]
    public partial class GatewayAccount : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            get => _GatewayAccountId;

            set
            {
                if (_GatewayAccountId != value)
                {
                    OnGatewayAccountIdChanging(value);
                    SendPropertyChanging();
                    _GatewayAccountId = value;
                    SendPropertyChanged("GatewayAccountId");
                    OnGatewayAccountIdChanged();
                }
            }
        }

        [Column(Name = "GatewayAccountName", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayAccountName", AutoSync = AutoSync.OnInsert, DbType = "nvarchar NOT NULL")]
        public string GatewayAccountName
        {
            get => _GatewayAccountName;

            set
            {
                if (_GatewayAccountName != value)
                {
                    OnGatewayAccountNameChanging(value);
                    SendPropertyChanging();
                    _GatewayAccountName = value;
                    SendPropertyChanged("GatewayAccountName");
                    OnGatewayAccountNameChanged();
                }
            }
        }

        [Column(Name = "GatewayId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int? GatewayId
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
