using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ContributionsRun")]
    public partial class ContributionsRun : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _Id;

        private DateTime? _Started;

        private int? _Count;

        private int? _Processed;

        private DateTime? _Completed;

        private string _Error;

        private int? _LastSet;

        private int? _CurrSet;

        private string _Sets;

        private int _Running;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

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

        partial void OnLastSetChanging(int? value);
        partial void OnLastSetChanged();

        partial void OnCurrSetChanging(int? value);
        partial void OnCurrSetChanged();

        partial void OnSetsChanging(string value);
        partial void OnSetsChanged();

        partial void OnRunningChanging(int value);
        partial void OnRunningChanged();

        #endregion
        public ContributionsRun()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get => this._Id;

            set
            {
                if (this._Id != value)
                {

                    this.OnIdChanging(value);
                    this.SendPropertyChanging();
                    this._Id = value;
                    this.SendPropertyChanged("Id");
                    this.OnIdChanged();
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


        [Column(Name = "error", UpdateCheck = UpdateCheck.Never, Storage = "_Error", DbType = "nvarchar(200)")]
        public string Error
        {
            get => this._Error;

            set
            {
                if (this._Error != value)
                {

                    this.OnErrorChanging(value);
                    this.SendPropertyChanging();
                    this._Error = value;
                    this.SendPropertyChanged("Error");
                    this.OnErrorChanged();
                }

            }

        }


        [Column(Name = "LastSet", UpdateCheck = UpdateCheck.Never, Storage = "_LastSet", DbType = "int")]
        public int? LastSet
        {
            get => this._LastSet;

            set
            {
                if (this._LastSet != value)
                {

                    this.OnLastSetChanging(value);
                    this.SendPropertyChanging();
                    this._LastSet = value;
                    this.SendPropertyChanged("LastSet");
                    this.OnLastSetChanged();
                }

            }

        }


        [Column(Name = "CurrSet", UpdateCheck = UpdateCheck.Never, Storage = "_CurrSet", DbType = "int")]
        public int? CurrSet
        {
            get => this._CurrSet;

            set
            {
                if (this._CurrSet != value)
                {

                    this.OnCurrSetChanging(value);
                    this.SendPropertyChanging();
                    this._CurrSet = value;
                    this.SendPropertyChanged("CurrSet");
                    this.OnCurrSetChanged();
                }

            }

        }


        [Column(Name = "Sets", UpdateCheck = UpdateCheck.Never, Storage = "_Sets", DbType = "nvarchar(50)")]
        public string Sets
        {
            get => this._Sets;

            set
            {
                if (this._Sets != value)
                {

                    this.OnSetsChanging(value);
                    this.SendPropertyChanging();
                    this._Sets = value;
                    this.SendPropertyChanged("Sets");
                    this.OnSetsChanged();
                }

            }

        }


        [Column(Name = "running", UpdateCheck = UpdateCheck.Never, Storage = "_Running", DbType = "int NOT NULL", IsDbGenerated = true)]
        public int Running
        {
            get => this._Running;

            set
            {
                if (this._Running != value)
                {

                    this.OnRunningChanging(value);
                    this.SendPropertyChanging();
                    this._Running = value;
                    this.SendPropertyChanged("Running");
                    this.OnRunningChanged();
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

