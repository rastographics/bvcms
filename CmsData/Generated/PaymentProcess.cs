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
    [Table(Name = "lookup.PaymentProcess")]
    public partial class PaymentProcess : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields
        private int _ProcessId;
        private string _ProcessName;
        private int _ProcessTypeId;

        private EntityRef<ProcessType> _ProcessType;
        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnProcessIdChanging(int value);
        partial void OnProcessIdChanged();

        partial void OnProcessNameChanging(string value);
        partial void OnProcessNameChanged();

        partial void OnProcessTypeIdChanging(int value);
        partial void OnProcessTypeIdChanged();
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

        [Column(Name = "ProcessTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_ProcessTypeId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int ProcessTypeId
        {
            get { return this._ProcessTypeId; }

            set
            {
                if (this._ProcessTypeId != value)
                {
                    if (this._ProcessType.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnProcessTypeIdChanging(value);
                    this.SendPropertyChanging();
                    this._ProcessTypeId = value;
                    this.SendPropertyChanged("ProcessTypeId");
                    this.OnProcessTypeIdChanged();
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
