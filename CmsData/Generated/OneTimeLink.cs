using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.OneTimeLinks")]
    public partial class OneTimeLink : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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


        [Column(Name = "querystring", UpdateCheck = UpdateCheck.Never, Storage = "_Querystring", DbType = "nvarchar(2000)")]
        public string Querystring
        {
            get => this._Querystring;

            set
            {
                if (this._Querystring != value)
                {

                    this.OnQuerystringChanging(value);
                    this.SendPropertyChanging();
                    this._Querystring = value;
                    this.SendPropertyChanged("Querystring");
                    this.OnQuerystringChanged();
                }

            }

        }


        [Column(Name = "used", UpdateCheck = UpdateCheck.Never, Storage = "_Used", DbType = "bit NOT NULL")]
        public bool Used
        {
            get => this._Used;

            set
            {
                if (this._Used != value)
                {

                    this.OnUsedChanging(value);
                    this.SendPropertyChanging();
                    this._Used = value;
                    this.SendPropertyChanged("Used");
                    this.OnUsedChanged();
                }

            }

        }


        [Column(Name = "expires", UpdateCheck = UpdateCheck.Never, Storage = "_Expires", DbType = "datetime")]
        public DateTime? Expires
        {
            get => this._Expires;

            set
            {
                if (this._Expires != value)
                {

                    this.OnExpiresChanging(value);
                    this.SendPropertyChanging();
                    this._Expires = value;
                    this.SendPropertyChanged("Expires");
                    this.OnExpiresChanged();
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

