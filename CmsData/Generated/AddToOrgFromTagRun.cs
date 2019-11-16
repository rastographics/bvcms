using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.AddToOrgFromTagRun")]
    public partial class AddToOrgFromTagRun : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int? _Orgid;

        private DateTime? _Started;

        private int? _Count;

        private int? _Processed;

        private DateTime? _Completed;

        private string _Error;

        private int _Running;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnOrgidChanging(int? value);
        partial void OnOrgidChanged();

        partial void OnStartedChanging(DateTime? value);
        partial void OnStartedChanged();

        partial void OnCountChanging(int? value);
        partial void OnCountChanged();

        partial void OnProcessedChanging(int? value);
        partial void OnProcessedChanged();

        partial void OnCompletedChanging(DateTime? value);
        partial void OnCompletedChanged();

        partial void OnErrorChanging(string value);
        partial void OnErrorChanged();

        partial void OnRunningChanging(int value);
        partial void OnRunningChanged();

        #endregion

        public AddToOrgFromTagRun()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

        [Column(Name = "orgid", UpdateCheck = UpdateCheck.Never, Storage = "_Orgid", DbType = "int")]
        public int? Orgid
        {
            get => _Orgid;

            set
            {
                if (_Orgid != value)
                {
                    OnOrgidChanging(value);
                    SendPropertyChanging();
                    _Orgid = value;
                    SendPropertyChanged("Orgid");
                    OnOrgidChanged();
                }
            }
        }

        [Column(Name = "started", UpdateCheck = UpdateCheck.Never, Storage = "_Started", DbType = "datetime")]
        public DateTime? Started
        {
            get => _Started;

            set
            {
                if (_Started != value)
                {
                    OnStartedChanging(value);
                    SendPropertyChanging();
                    _Started = value;
                    SendPropertyChanged("Started");
                    OnStartedChanged();
                }
            }
        }

        [Column(Name = "count", UpdateCheck = UpdateCheck.Never, Storage = "_Count", DbType = "int")]
        public int? Count
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

        [Column(Name = "processed", UpdateCheck = UpdateCheck.Never, Storage = "_Processed", DbType = "int")]
        public int? Processed
        {
            get => _Processed;

            set
            {
                if (_Processed != value)
                {
                    OnProcessedChanging(value);
                    SendPropertyChanging();
                    _Processed = value;
                    SendPropertyChanged("Processed");
                    OnProcessedChanged();
                }
            }
        }

        [Column(Name = "completed", UpdateCheck = UpdateCheck.Never, Storage = "_Completed", DbType = "datetime")]
        public DateTime? Completed
        {
            get => _Completed;

            set
            {
                if (_Completed != value)
                {
                    OnCompletedChanging(value);
                    SendPropertyChanging();
                    _Completed = value;
                    SendPropertyChanged("Completed");
                    OnCompletedChanged();
                }
            }
        }

        [Column(Name = "error", UpdateCheck = UpdateCheck.Never, Storage = "_Error", DbType = "nvarchar(200)")]
        public string Error
        {
            get => _Error;

            set
            {
                if (_Error != value)
                {
                    OnErrorChanging(value);
                    SendPropertyChanging();
                    _Error = value;
                    SendPropertyChanged("Error");
                    OnErrorChanged();
                }
            }
        }

        [Column(Name = "running", UpdateCheck = UpdateCheck.Never, Storage = "_Running", DbType = "int NOT NULL", IsDbGenerated = true)]
        public int Running
        {
            get => _Running;

            set
            {
                if (_Running != value)
                {
                    OnRunningChanging(value);
                    SendPropertyChanging();
                    _Running = value;
                    SendPropertyChanged("Running");
                    OnRunningChanged();
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
