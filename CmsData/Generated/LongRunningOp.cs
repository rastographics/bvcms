using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.LongRunningOp")]
    public partial class LongRunningOp : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Operation;

        private DateTime? _Started;

        private int? _Count;

        private int? _Processed;

        private DateTime? _Completed;

        private string _ElapsedTime;

        private string _CustomMessage;

        private Guid? _QueryId;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnOperationChanging(string value);
        partial void OnOperationChanged();

        partial void OnStartedChanging(DateTime? value);
        partial void OnStartedChanged();

        partial void OnCountChanging(int? value);
        partial void OnCountChanged();

        partial void OnProcessedChanging(int? value);
        partial void OnProcessedChanged();

        partial void OnCompletedChanging(DateTime? value);
        partial void OnCompletedChanged();

        partial void OnElapsedTimeChanging(string value);
        partial void OnElapsedTimeChanged();

        partial void OnCustomMessageChanging(string value);
        partial void OnCustomMessageChanged();

        partial void OnQueryIdChanging(Guid? value);
        partial void OnQueryIdChanged();

        #endregion

        public LongRunningOp()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "operation", UpdateCheck = UpdateCheck.Never, Storage = "_Operation", DbType = "nvarchar(25) NOT NULL", IsPrimaryKey = true)]
        public string Operation
        {
            get => _Operation;

            set
            {
                if (_Operation != value)
                {
                    OnOperationChanging(value);
                    SendPropertyChanging();
                    _Operation = value;
                    SendPropertyChanged("Operation");
                    OnOperationChanged();
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

        [Column(Name = "ElapsedTime", UpdateCheck = UpdateCheck.Never, Storage = "_ElapsedTime", DbType = "varchar(20)", IsDbGenerated = true)]
        public string ElapsedTime
        {
            get => _ElapsedTime;

            set
            {
                if (_ElapsedTime != value)
                {
                    OnElapsedTimeChanging(value);
                    SendPropertyChanging();
                    _ElapsedTime = value;
                    SendPropertyChanged("ElapsedTime");
                    OnElapsedTimeChanged();
                }
            }
        }

        [Column(Name = "CustomMessage", UpdateCheck = UpdateCheck.Never, Storage = "_CustomMessage", DbType = "nvarchar(80)")]
        public string CustomMessage
        {
            get => _CustomMessage;

            set
            {
                if (_CustomMessage != value)
                {
                    OnCustomMessageChanging(value);
                    SendPropertyChanging();
                    _CustomMessage = value;
                    SendPropertyChanged("CustomMessage");
                    OnCustomMessageChanged();
                }
            }
        }

        [Column(Name = "QueryId", UpdateCheck = UpdateCheck.Never, Storage = "_QueryId", DbType = "uniqueidentifier")]
        public Guid? QueryId
        {
            get => _QueryId;

            set
            {
                if (_QueryId != value)
                {
                    OnQueryIdChanging(value);
                    SendPropertyChanging();
                    _QueryId = value;
                    SendPropertyChanged("QueryId");
                    OnQueryIdChanged();
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
