using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.OneTimeLinks")]
    public partial class OneTimeLink : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private Guid _Id;

        private string _Querystring;

        private bool _Used;

        private DateTime? _Expires;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(Guid value);
        partial void OnIdChanged();

        partial void OnQuerystringChanging(string value);
        partial void OnQuerystringChanged();

        partial void OnUsedChanging(bool value);
        partial void OnUsedChanged();

        partial void OnExpiresChanging(DateTime? value);
        partial void OnExpiresChanged();

        #endregion

        public OneTimeLink()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "uniqueidentifier NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "querystring", UpdateCheck = UpdateCheck.Never, Storage = "_Querystring", DbType = "nvarchar(2000)")]
        public string Querystring
        {
            get => _Querystring;

            set
            {
                if (_Querystring != value)
                {
                    OnQuerystringChanging(value);
                    SendPropertyChanging();
                    _Querystring = value;
                    SendPropertyChanged("Querystring");
                    OnQuerystringChanged();
                }
            }
        }

        [Column(Name = "used", UpdateCheck = UpdateCheck.Never, Storage = "_Used", DbType = "bit NOT NULL")]
        public bool Used
        {
            get => _Used;

            set
            {
                if (_Used != value)
                {
                    OnUsedChanging(value);
                    SendPropertyChanging();
                    _Used = value;
                    SendPropertyChanged("Used");
                    OnUsedChanged();
                }
            }
        }

        [Column(Name = "expires", UpdateCheck = UpdateCheck.Never, Storage = "_Expires", DbType = "datetime")]
        public DateTime? Expires
        {
            get => _Expires;

            set
            {
                if (_Expires != value)
                {
                    OnExpiresChanging(value);
                    SendPropertyChanging();
                    _Expires = value;
                    SendPropertyChanged("Expires");
                    OnExpiresChanged();
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
