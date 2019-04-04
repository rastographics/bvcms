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
    [Table(Name = "dbo.PaymentProcess")]
    public partial class PaymentProcess : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields
        private int _ProcessId;
        private string _ProcessName;
        private int? _GatewayAccountId;

        private EntityRef<GatewayAccount> _GatewayAccount;
        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnProcessIdChanging(int value);
        partial void OnProcessIdChanged();

        partial void OnProcessNameChanging(string value);
        partial void OnProcessNameChanged();

        partial void OnGatewayAccountIdChanging(int? value);
        partial void OnGatewayAccountIdChanged();
        #endregion

        public PaymentProcess()
        {
            OnCreated();
        }

        #region Columns
        [Column(Name = "ProcessId", UpdateCheck = UpdateCheck.Never, Storage = "_ProcessId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ProcessId
        {
            get { return this._ProcessId; }

            set
            {
                if (this._ProcessId != value)
                {
                    this.OnProcessIdChanging(value);
                    this.SendPropertyChanging();
                    this._ProcessId = value;
                    this.SendPropertyChanged("ProcessId");
                    this.OnProcessIdChanged();
                }
            }
        }

        [Column(Name = "ProcessName", UpdateCheck = UpdateCheck.Never, Storage = "_ProcessName", AutoSync = AutoSync.OnInsert, DbType = "nvarchar NOT NULL")]
        public string ProcessName
        {
            get { return this._ProcessName; }

            set
            {
                if (this._ProcessName != value)
                {
                    this.OnProcessNameChanging(value);
                    this.SendPropertyChanging();
                    this._ProcessName = value;
                    this.SendPropertyChanged("ProcessName");
                    this.OnProcessNameChanged();
                }
            }
        }

        [Column(Name = "GatewayAccountId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayAccountId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int? GatewayAccountId
        {
            get { return this._GatewayAccountId; }

            set
            {
                if (this._GatewayAccountId != value)
                {
                    if (this._GatewayAccount.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnGatewayAccountIdChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayAccountId = value;
                    this.SendPropertyChanged("GatewayAccountId");
                    this.OnGatewayAccountIdChanged();
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
