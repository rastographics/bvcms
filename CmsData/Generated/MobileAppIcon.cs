using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MobileAppIcons")]
    public partial class MobileAppIcon : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _Id;

        private int _SetID;

        private int _ActionID;

        private string _Url;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnSetIDChanging(int value);
        partial void OnSetIDChanged();

        partial void OnActionIDChanging(int value);
        partial void OnActionIDChanged();

        partial void OnUrlChanging(string value);
        partial void OnUrlChanged();

        #endregion
        public MobileAppIcon()
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


        [Column(Name = "setID", UpdateCheck = UpdateCheck.Never, Storage = "_SetID", DbType = "int NOT NULL")]
        public int SetID
        {
            get => this._SetID;

            set
            {
                if (this._SetID != value)
                {

                    this.OnSetIDChanging(value);
                    this.SendPropertyChanging();
                    this._SetID = value;
                    this.SendPropertyChanged("SetID");
                    this.OnSetIDChanged();
                }

            }

        }


        [Column(Name = "actionID", UpdateCheck = UpdateCheck.Never, Storage = "_ActionID", DbType = "int NOT NULL")]
        public int ActionID
        {
            get => this._ActionID;

            set
            {
                if (this._ActionID != value)
                {

                    this.OnActionIDChanging(value);
                    this.SendPropertyChanging();
                    this._ActionID = value;
                    this.SendPropertyChanged("ActionID");
                    this.OnActionIDChanged();
                }

            }

        }


        [Column(Name = "url", UpdateCheck = UpdateCheck.Never, Storage = "_Url", DbType = "nvarchar(200) NOT NULL")]
        public string Url
        {
            get => this._Url;

            set
            {
                if (this._Url != value)
                {

                    this.OnUrlChanging(value);
                    this.SendPropertyChanging();
                    this._Url = value;
                    this.SendPropertyChanged("Url");
                    this.OnUrlChanged();
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

