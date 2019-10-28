using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.QueryStats")]
    public partial class QueryStat : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._RunId;

            set
            {
                if (this._RunId != value)
                {

                    this.OnRunIdChanging(value);
                    this.SendPropertyChanging();
                    this._RunId = value;
                    this.SendPropertyChanged("RunId");
                    this.OnRunIdChanged();
                }

            }

        }


        [Column(Name = "StatId", UpdateCheck = UpdateCheck.Never, Storage = "_StatId", DbType = "nvarchar(5) NOT NULL", IsPrimaryKey = true)]
        public string StatId
        {
            get => this._StatId;

            set
            {
                if (this._StatId != value)
                {

                    this.OnStatIdChanging(value);
                    this.SendPropertyChanging();
                    this._StatId = value;
                    this.SendPropertyChanged("StatId");
                    this.OnStatIdChanged();
                }

            }

        }


        [Column(Name = "Runtime", UpdateCheck = UpdateCheck.Never, Storage = "_Runtime", DbType = "datetime NOT NULL")]
        public DateTime Runtime
        {
            get => this._Runtime;

            set
            {
                if (this._Runtime != value)
                {

                    this.OnRuntimeChanging(value);
                    this.SendPropertyChanging();
                    this._Runtime = value;
                    this.SendPropertyChanged("Runtime");
                    this.OnRuntimeChanged();
                }

            }

        }


        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(75) NOT NULL")]
        public string Description
        {
            get => this._Description;

            set
            {
                if (this._Description != value)
                {

                    this.OnDescriptionChanging(value);
                    this.SendPropertyChanging();
                    this._Description = value;
                    this.SendPropertyChanged("Description");
                    this.OnDescriptionChanged();
                }

            }

        }


        [Column(Name = "Count", UpdateCheck = UpdateCheck.Never, Storage = "_Count", DbType = "int NOT NULL")]
        public int Count
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

