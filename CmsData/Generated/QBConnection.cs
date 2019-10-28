using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.QBConnections")]
    public partial class QBConnection : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private DateTime _Creation;

        private int _UserID;

        private byte _Active;

        private string _DataSource;

        private string _Token;

        private string _Secret;

        private string _RealmID;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnCreationChanging(DateTime value);
        partial void OnCreationChanged();

        partial void OnUserIDChanging(int value);
        partial void OnUserIDChanged();

        partial void OnActiveChanging(byte value);
        partial void OnActiveChanged();

        partial void OnDataSourceChanging(string value);
        partial void OnDataSourceChanged();

        partial void OnTokenChanging(string value);
        partial void OnTokenChanged();

        partial void OnSecretChanging(string value);
        partial void OnSecretChanged();

        partial void OnRealmIDChanging(string value);
        partial void OnRealmIDChanged();

        #endregion

        public QBConnection()
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

        [Column(Name = "Creation", UpdateCheck = UpdateCheck.Never, Storage = "_Creation", DbType = "datetime NOT NULL")]
        public DateTime Creation
        {
            get => _Creation;

            set
            {
                if (_Creation != value)
                {
                    OnCreationChanging(value);
                    SendPropertyChanging();
                    _Creation = value;
                    SendPropertyChanged("Creation");
                    OnCreationChanged();
                }
            }
        }

        [Column(Name = "UserID", UpdateCheck = UpdateCheck.Never, Storage = "_UserID", DbType = "int NOT NULL")]
        public int UserID
        {
            get => _UserID;

            set
            {
                if (_UserID != value)
                {
                    OnUserIDChanging(value);
                    SendPropertyChanging();
                    _UserID = value;
                    SendPropertyChanged("UserID");
                    OnUserIDChanged();
                }
            }
        }

        [Column(Name = "Active", UpdateCheck = UpdateCheck.Never, Storage = "_Active", DbType = "tinyint NOT NULL")]
        public byte Active
        {
            get => _Active;

            set
            {
                if (_Active != value)
                {
                    OnActiveChanging(value);
                    SendPropertyChanging();
                    _Active = value;
                    SendPropertyChanged("Active");
                    OnActiveChanged();
                }
            }
        }

        [Column(Name = "DataSource", UpdateCheck = UpdateCheck.Never, Storage = "_DataSource", DbType = "char(3) NOT NULL")]
        public string DataSource
        {
            get => _DataSource;

            set
            {
                if (_DataSource != value)
                {
                    OnDataSourceChanging(value);
                    SendPropertyChanging();
                    _DataSource = value;
                    SendPropertyChanged("DataSource");
                    OnDataSourceChanged();
                }
            }
        }

        [Column(Name = "Token", UpdateCheck = UpdateCheck.Never, Storage = "_Token", DbType = "nvarchar NOT NULL")]
        public string Token
        {
            get => _Token;

            set
            {
                if (_Token != value)
                {
                    OnTokenChanging(value);
                    SendPropertyChanging();
                    _Token = value;
                    SendPropertyChanged("Token");
                    OnTokenChanged();
                }
            }
        }

        [Column(Name = "Secret", UpdateCheck = UpdateCheck.Never, Storage = "_Secret", DbType = "nvarchar NOT NULL")]
        public string Secret
        {
            get => _Secret;

            set
            {
                if (_Secret != value)
                {
                    OnSecretChanging(value);
                    SendPropertyChanging();
                    _Secret = value;
                    SendPropertyChanged("Secret");
                    OnSecretChanged();
                }
            }
        }

        [Column(Name = "RealmID", UpdateCheck = UpdateCheck.Never, Storage = "_RealmID", DbType = "nvarchar NOT NULL")]
        public string RealmID
        {
            get => _RealmID;

            set
            {
                if (_RealmID != value)
                {
                    OnRealmIDChanging(value);
                    SendPropertyChanging();
                    _RealmID = value;
                    SendPropertyChanged("RealmID");
                    OnRealmIDChanged();
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
