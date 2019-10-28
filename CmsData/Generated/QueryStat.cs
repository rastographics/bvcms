using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.QueryStats")]
    public partial class QueryStat : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _RunId;

        private string _StatId;

        private DateTime _Runtime;

        private string _Description;

        private int _Count;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnRunIdChanging(int value);
        partial void OnRunIdChanged();

        partial void OnStatIdChanging(string value);
        partial void OnStatIdChanged();

        partial void OnRuntimeChanging(DateTime value);
        partial void OnRuntimeChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnCountChanging(int value);
        partial void OnCountChanged();

        #endregion

        public QueryStat()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "RunId", UpdateCheck = UpdateCheck.Never, Storage = "_RunId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int RunId
        {
            get => _RunId;

            set
            {
                if (_RunId != value)
                {
                    OnRunIdChanging(value);
                    SendPropertyChanging();
                    _RunId = value;
                    SendPropertyChanged("RunId");
                    OnRunIdChanged();
                }
            }
        }

        [Column(Name = "StatId", UpdateCheck = UpdateCheck.Never, Storage = "_StatId", DbType = "nvarchar(5) NOT NULL", IsPrimaryKey = true)]
        public string StatId
        {
            get => _StatId;

            set
            {
                if (_StatId != value)
                {
                    OnStatIdChanging(value);
                    SendPropertyChanging();
                    _StatId = value;
                    SendPropertyChanged("StatId");
                    OnStatIdChanged();
                }
            }
        }

        [Column(Name = "Runtime", UpdateCheck = UpdateCheck.Never, Storage = "_Runtime", DbType = "datetime NOT NULL")]
        public DateTime Runtime
        {
            get => _Runtime;

            set
            {
                if (_Runtime != value)
                {
                    OnRuntimeChanging(value);
                    SendPropertyChanging();
                    _Runtime = value;
                    SendPropertyChanged("Runtime");
                    OnRuntimeChanged();
                }
            }
        }

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(75) NOT NULL")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    OnDescriptionChanging(value);
                    SendPropertyChanging();
                    _Description = value;
                    SendPropertyChanged("Description");
                    OnDescriptionChanged();
                }
            }
        }

        [Column(Name = "Count", UpdateCheck = UpdateCheck.Never, Storage = "_Count", DbType = "int NOT NULL")]
        public int Count
        {
            get => _Count;

            set
            {
                if (_Count != value)
                {
                    OnCountChanging(value);
                    SendPropertyChanging();
                    _Count = value;
                    SendPropertyChanged("Count");
                    OnCountChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

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
