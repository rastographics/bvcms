using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MeetingCategory")]
    public partial class MeetingCategory : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private long _Id;

        private string _Description;

        private DateTime? _NotBeforeDate;

        private DateTime? _NotAfterDate;

        private bool _IsExpired;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(long value);
        partial void OnIdChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnNotBeforeDateChanging(DateTime? value);
        partial void OnNotBeforeDateChanged();

        partial void OnNotAfterDateChanging(DateTime? value);
        partial void OnNotAfterDateChanged();

        partial void OnIsExpiredChanging(bool value);
        partial void OnIsExpiredChanged();

        #endregion

        public MeetingCategory()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "bigint NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public long Id
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

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(100)")]
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

        [Column(Name = "NotBeforeDate", UpdateCheck = UpdateCheck.Never, Storage = "_NotBeforeDate", DbType = "datetime")]
        public DateTime? NotBeforeDate
        {
            get => _NotBeforeDate;

            set
            {
                if (_NotBeforeDate != value)
                {
                    OnNotBeforeDateChanging(value);
                    SendPropertyChanging();
                    _NotBeforeDate = value;
                    SendPropertyChanged("NotBeforeDate");
                    OnNotBeforeDateChanged();
                }
            }
        }

        [Column(Name = "NotAfterDate", UpdateCheck = UpdateCheck.Never, Storage = "_NotAfterDate", DbType = "datetime")]
        public DateTime? NotAfterDate
        {
            get => _NotAfterDate;

            set
            {
                if (_NotAfterDate != value)
                {
                    OnNotAfterDateChanging(value);
                    SendPropertyChanging();
                    _NotAfterDate = value;
                    SendPropertyChanged("NotAfterDate");
                    OnNotAfterDateChanged();
                }
            }
        }

        [Column(Name = "IsExpired", UpdateCheck = UpdateCheck.Never, Storage = "_IsExpired", DbType = "bit NOT NULL")]
        public bool IsExpired
        {
            get => _IsExpired;

            set
            {
                if (_IsExpired != value)
                {
                    OnIsExpiredChanging(value);
                    SendPropertyChanging();
                    _IsExpired = value;
                    SendPropertyChanged("IsExpired");
                    OnIsExpiredChanged();
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
