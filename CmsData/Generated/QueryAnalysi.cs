using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.QueryAnalysis")]
    public partial class QueryAnalysi : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private Guid _Id;

        private int? _Seconds;

        private int? _OriginalCount;

        private int? _ParsedCount;

        private string _Message;

        private bool? _Updated;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(Guid value);
        partial void OnIdChanged();

        partial void OnSecondsChanging(int? value);
        partial void OnSecondsChanged();

        partial void OnOriginalCountChanging(int? value);
        partial void OnOriginalCountChanged();

        partial void OnParsedCountChanging(int? value);
        partial void OnParsedCountChanged();

        partial void OnMessageChanging(string value);
        partial void OnMessageChanged();

        partial void OnUpdatedChanging(bool? value);
        partial void OnUpdatedChanged();

        #endregion

        public QueryAnalysi()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "uniqueidentifier NOT NULL")]
        public Guid Id
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

        [Column(Name = "Seconds", UpdateCheck = UpdateCheck.Never, Storage = "_Seconds", DbType = "int")]
        public int? Seconds
        {
            get => _Seconds;

            set
            {
                if (_Seconds != value)
                {
                    OnSecondsChanging(value);
                    SendPropertyChanging();
                    _Seconds = value;
                    SendPropertyChanged("Seconds");
                    OnSecondsChanged();
                }
            }
        }

        [Column(Name = "OriginalCount", UpdateCheck = UpdateCheck.Never, Storage = "_OriginalCount", DbType = "int")]
        public int? OriginalCount
        {
            get => _OriginalCount;

            set
            {
                if (_OriginalCount != value)
                {
                    OnOriginalCountChanging(value);
                    SendPropertyChanging();
                    _OriginalCount = value;
                    SendPropertyChanged("OriginalCount");
                    OnOriginalCountChanged();
                }
            }
        }

        [Column(Name = "ParsedCount", UpdateCheck = UpdateCheck.Never, Storage = "_ParsedCount", DbType = "int")]
        public int? ParsedCount
        {
            get => _ParsedCount;

            set
            {
                if (_ParsedCount != value)
                {
                    OnParsedCountChanging(value);
                    SendPropertyChanging();
                    _ParsedCount = value;
                    SendPropertyChanged("ParsedCount");
                    OnParsedCountChanged();
                }
            }
        }

        [Column(Name = "Message", UpdateCheck = UpdateCheck.Never, Storage = "_Message", DbType = "varchar")]
        public string Message
        {
            get => _Message;

            set
            {
                if (_Message != value)
                {
                    OnMessageChanging(value);
                    SendPropertyChanging();
                    _Message = value;
                    SendPropertyChanged("Message");
                    OnMessageChanged();
                }
            }
        }

        [Column(Name = "Updated", UpdateCheck = UpdateCheck.Never, Storage = "_Updated", DbType = "bit")]
        public bool? Updated
        {
            get => _Updated;

            set
            {
                if (_Updated != value)
                {
                    OnUpdatedChanging(value);
                    SendPropertyChanging();
                    _Updated = value;
                    SendPropertyChanged("Updated");
                    OnUpdatedChanged();
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
