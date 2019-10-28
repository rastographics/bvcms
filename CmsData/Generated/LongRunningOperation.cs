using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.LongRunningOperation")]
    public partial class LongRunningOperation : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private Guid _QueryId;

        private string _Operation;

        private DateTime? _Started;

        private int? _Count;

        private int? _Processed;

        private DateTime? _Completed;

        private string _ElapsedTime;

        private string _CustomMessage;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnQueryIdChanging(Guid value);
        partial void OnQueryIdChanged();

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

        #endregion
        public LongRunningOperation()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "QueryId", UpdateCheck = UpdateCheck.Never, Storage = "_QueryId", DbType = "uniqueidentifier NOT NULL", IsPrimaryKey = true)]
        public Guid QueryId
        {
            get => this._QueryId;

            set
            {
                if (this._QueryId != value)
                {

                    this.OnQueryIdChanging(value);
                    this.SendPropertyChanging();
                    this._QueryId = value;
                    this.SendPropertyChanged("QueryId");
                    this.OnQueryIdChanged();
                }

            }

        }


        [Column(Name = "operation", UpdateCheck = UpdateCheck.Never, Storage = "_Operation", DbType = "nvarchar(25)")]
        public string Operation
        {
            get => this._Operation;

            set
            {
                if (this._Operation != value)
                {

                    this.OnOperationChanging(value);
                    this.SendPropertyChanging();
                    this._Operation = value;
                    this.SendPropertyChanged("Operation");
                    this.OnOperationChanged();
                }

            }

        }


        [Column(Name = "started", UpdateCheck = UpdateCheck.Never, Storage = "_Started", DbType = "datetime")]
        public DateTime? Started
        {
            get => this._Started;

            set
            {
                if (this._Started != value)
                {

                    this.OnStartedChanging(value);
                    this.SendPropertyChanging();
                    this._Started = value;
                    this.SendPropertyChanged("Started");
                    this.OnStartedChanged();
                }

            }

        }


        [Column(Name = "count", UpdateCheck = UpdateCheck.Never, Storage = "_Count", DbType = "int")]
        public int? Count
        {
            get => this._Count;

            set
            {
                if (this._Count != value)
                {

                    this.OnCountChanging(value);
                    this.SendPropertyChanging();
                    this._Count = value;
                    this.SendPropertyChanged("Count");
                    this.OnCountChanged();
                }

            }

        }


        [Column(Name = "processed", UpdateCheck = UpdateCheck.Never, Storage = "_Processed", DbType = "int")]
        public int? Processed
        {
            get => this._Processed;

            set
            {
                if (this._Processed != value)
                {

                    this.OnProcessedChanging(value);
                    this.SendPropertyChanging();
                    this._Processed = value;
                    this.SendPropertyChanged("Processed");
                    this.OnProcessedChanged();
                }

            }

        }


        [Column(Name = "completed", UpdateCheck = UpdateCheck.Never, Storage = "_Completed", DbType = "datetime")]
        public DateTime? Completed
        {
            get => this._Completed;

            set
            {
                if (this._Completed != value)
                {

                    this.OnCompletedChanging(value);
                    this.SendPropertyChanging();
                    this._Completed = value;
                    this.SendPropertyChanged("Completed");
                    this.OnCompletedChanged();
                }

            }

        }


        [Column(Name = "ElapsedTime", UpdateCheck = UpdateCheck.Never, Storage = "_ElapsedTime", DbType = "varchar(20)", IsDbGenerated = true)]
        public string ElapsedTime
        {
            get => this._ElapsedTime;

            set
            {
                if (this._ElapsedTime != value)
                {

                    this.OnElapsedTimeChanging(value);
                    this.SendPropertyChanging();
                    this._ElapsedTime = value;
                    this.SendPropertyChanged("ElapsedTime");
                    this.OnElapsedTimeChanged();
                }

            }

        }


        [Column(Name = "CustomMessage", UpdateCheck = UpdateCheck.Never, Storage = "_CustomMessage", DbType = "nvarchar(200)")]
        public string CustomMessage
        {
            get => this._CustomMessage;

            set
            {
                if (this._CustomMessage != value)
                {

                    this.OnCustomMessageChanging(value);
                    this.SendPropertyChanging();
                    this._CustomMessage = value;
                    this.SendPropertyChanged("CustomMessage");
                    this.OnCustomMessageChanged();
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
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }

}

