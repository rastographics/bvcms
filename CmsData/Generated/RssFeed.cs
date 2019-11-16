using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.RssFeed")]
    public partial class RssFeed : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _Url;

        private string _Data;

        private string _ETag;

        private DateTime? _LastModified;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnUrlChanging(string value);
        partial void OnUrlChanged();

        partial void OnDataChanging(string value);
        partial void OnDataChanged();

        partial void OnETagChanging(string value);
        partial void OnETagChanged();

        partial void OnLastModifiedChanging(DateTime? value);
        partial void OnLastModifiedChanged();

        #endregion

        public RssFeed()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Url", UpdateCheck = UpdateCheck.Never, Storage = "_Url", DbType = "nvarchar(150) NOT NULL", IsPrimaryKey = true)]
        public string Url
        {
            get => _Url;

            set
            {
                if (_Url != value)
                {
                    OnUrlChanging(value);
                    SendPropertyChanging();
                    _Url = value;
                    SendPropertyChanged("Url");
                    OnUrlChanged();
                }
            }
        }

        [Column(Name = "Data", UpdateCheck = UpdateCheck.Never, Storage = "_Data", DbType = "nvarchar")]
        public string Data
        {
            get => _Data;

            set
            {
                if (_Data != value)
                {
                    OnDataChanging(value);
                    SendPropertyChanging();
                    _Data = value;
                    SendPropertyChanged("Data");
                    OnDataChanged();
                }
            }
        }

        [Column(Name = "ETag", UpdateCheck = UpdateCheck.Never, Storage = "_ETag", DbType = "nvarchar(150)")]
        public string ETag
        {
            get => _ETag;

            set
            {
                if (_ETag != value)
                {
                    OnETagChanging(value);
                    SendPropertyChanging();
                    _ETag = value;
                    SendPropertyChanged("ETag");
                    OnETagChanged();
                }
            }
        }

        [Column(Name = "LastModified", UpdateCheck = UpdateCheck.Never, Storage = "_LastModified", DbType = "datetime")]
        public DateTime? LastModified
        {
            get => _LastModified;

            set
            {
                if (_LastModified != value)
                {
                    OnLastModifiedChanging(value);
                    SendPropertyChanging();
                    _LastModified = value;
                    SendPropertyChanged("LastModified");
                    OnLastModifiedChanged();
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
