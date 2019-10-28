using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.QBConnections")]
    public partial class QBConnection : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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


        [Column(Name = "Creation", UpdateCheck = UpdateCheck.Never, Storage = "_Creation", DbType = "datetime NOT NULL")]
        public DateTime Creation
        {
            get => this._Creation;

            set
            {
                if (this._Creation != value)
                {

                    this.OnCreationChanging(value);
                    this.SendPropertyChanging();
                    this._Creation = value;
                    this.SendPropertyChanged("Creation");
                    this.OnCreationChanged();
                }

            }

        }


        [Column(Name = "UserID", UpdateCheck = UpdateCheck.Never, Storage = "_UserID", DbType = "int NOT NULL")]
        public int UserID
        {
            get => this._UserID;

            set
            {
                if (this._UserID != value)
                {

                    this.OnUserIDChanging(value);
                    this.SendPropertyChanging();
                    this._UserID = value;
                    this.SendPropertyChanged("UserID");
                    this.OnUserIDChanged();
                }

            }

        }


        [Column(Name = "Active", UpdateCheck = UpdateCheck.Never, Storage = "_Active", DbType = "tinyint NOT NULL")]
        public byte Active
        {
            get => this._Active;

            set
            {
                if (this._Active != value)
                {

                    this.OnActiveChanging(value);
                    this.SendPropertyChanging();
                    this._Active = value;
                    this.SendPropertyChanged("Active");
                    this.OnActiveChanged();
                }

            }

        }


        [Column(Name = "DataSource", UpdateCheck = UpdateCheck.Never, Storage = "_DataSource", DbType = "char(3) NOT NULL")]
        public string DataSource
        {
            get => this._DataSource;

            set
            {
                if (this._DataSource != value)
                {

                    this.OnDataSourceChanging(value);
                    this.SendPropertyChanging();
                    this._DataSource = value;
                    this.SendPropertyChanged("DataSource");
                    this.OnDataSourceChanged();
                }

            }

        }


        [Column(Name = "Token", UpdateCheck = UpdateCheck.Never, Storage = "_Token", DbType = "nvarchar NOT NULL")]
        public string Token
        {
            get => this._Token;

            set
            {
                if (this._Token != value)
                {

                    this.OnTokenChanging(value);
                    this.SendPropertyChanging();
                    this._Token = value;
                    this.SendPropertyChanged("Token");
                    this.OnTokenChanged();
                }

            }

        }


        [Column(Name = "Secret", UpdateCheck = UpdateCheck.Never, Storage = "_Secret", DbType = "nvarchar NOT NULL")]
        public string Secret
        {
            get => this._Secret;

            set
            {
                if (this._Secret != value)
                {

                    this.OnSecretChanging(value);
                    this.SendPropertyChanging();
                    this._Secret = value;
                    this.SendPropertyChanged("Secret");
                    this.OnSecretChanged();
                }

            }

        }


        [Column(Name = "RealmID", UpdateCheck = UpdateCheck.Never, Storage = "_RealmID", DbType = "nvarchar NOT NULL")]
        public string RealmID
        {
            get => this._RealmID;

            set
            {
                if (this._RealmID != value)
                {

                    this.OnRealmIDChanging(value);
                    this.SendPropertyChanging();
                    this._RealmID = value;
                    this.SendPropertyChanged("RealmID");
                    this.OnRealmIDChanged();
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

