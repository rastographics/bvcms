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
    [Table(Name = "lookup.ProcessType")]
    public partial class ProcessType : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields
        private int _ProcessTypeId;
        private string _ProcessTypeName;
        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnProcessTypeIdChanging(int value);
        partial void OnProcessTypeIdChanged();

        partial void OnProcessTypeNameChanging(string value);
        partial void OnProcessTypeNameChanged();
        #endregion

        public ProcessType()
        {
            OnCreated();
        }

        #region Columns
        [Column(Name = "ProcessTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_ProcessTypeId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int PeopleId
        {
            get { return this._ProcessTypeId; }

            set
            {
                if (this._ProcessTypeId != value)
                {
                    this.OnProcessTypeIdChanging(value);
                    this.SendPropertyChanging();
                    this._ProcessTypeId = value;
                    this.SendPropertyChanged("ProcessTypeId");
                    this.OnProcessTypeIdChanged();
                }

            }

        }

        [Column(Name = "ProcessTypeName", UpdateCheck = UpdateCheck.Never, Storage = "_ProcessTypeName", DbType = "nvarchar")]
        public string ProcessTypeName
        {
            get { return this._ProcessTypeName; }

            set
            {
                if (this._ProcessTypeName != value)
                {
                    this.OnProcessTypeNameChanging(value);
                    this.SendPropertyChanging();
                    this._ProcessTypeName = value;
                    this.SendPropertyChanged("ProcessTypeName");
                    this.OnProcessTypeNameChanged();
                }

            }

        }
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
