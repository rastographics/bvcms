using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.SMSNumbers")]
    public partial class SMSNumber : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _GroupID;

        private string _Number;

        private DateTime _LastUpdated;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnGroupIDChanging(int value);
        partial void OnGroupIDChanged();

        partial void OnNumberChanging(string value);
        partial void OnNumberChanged();

        partial void OnLastUpdatedChanging(DateTime value);
        partial void OnLastUpdatedChanged();

        #endregion

        public SMSNumber()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "ID", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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

        [Column(Name = "GroupID", UpdateCheck = UpdateCheck.Never, Storage = "_GroupID", DbType = "int NOT NULL")]
        public int GroupID
        {
            get => _GroupID;

            set
            {
                if (_GroupID != value)
                {
                    OnGroupIDChanging(value);
                    SendPropertyChanging();
                    _GroupID = value;
                    SendPropertyChanged("GroupID");
                    OnGroupIDChanged();
                }
            }
        }

        [Column(Name = "Number", UpdateCheck = UpdateCheck.Never, Storage = "_Number", DbType = "nvarchar(50) NOT NULL")]
        public string Number
        {
            get => _Number;

            set
            {
                if (_Number != value)
                {
                    OnNumberChanging(value);
                    SendPropertyChanging();
                    _Number = value;
                    SendPropertyChanged("Number");
                    OnNumberChanged();
                }
            }
        }

        [Column(Name = "LastUpdated", UpdateCheck = UpdateCheck.Never, Storage = "_LastUpdated", DbType = "datetime NOT NULL")]
        public DateTime LastUpdated
        {
            get => _LastUpdated;

            set
            {
                if (_LastUpdated != value)
                {
                    OnLastUpdatedChanging(value);
                    SendPropertyChanging();
                    _LastUpdated = value;
                    SendPropertyChanged("LastUpdated");
                    OnLastUpdatedChanged();
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
