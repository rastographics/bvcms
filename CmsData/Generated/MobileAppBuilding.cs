using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MobileAppBuilding")]
    public partial class MobileAppBuilding : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _Id;

        private string _Name;

        private int _Order;

        private bool _Enabled;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnOrderChanging(int value);
        partial void OnOrderChanged();

        partial void OnEnabledChanging(bool value);
        partial void OnEnabledChanged();

        #endregion
        public MobileAppBuilding()
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


        [Column(Name = "name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(100) NOT NULL")]
        public string Name
        {
            get => this._Name;

            set
            {
                if (this._Name != value)
                {

                    this.OnNameChanging(value);
                    this.SendPropertyChanging();
                    this._Name = value;
                    this.SendPropertyChanged("Name");
                    this.OnNameChanged();
                }

            }

        }


        [Column(Name = "order", UpdateCheck = UpdateCheck.Never, Storage = "_Order", DbType = "int NOT NULL")]
        public int Order
        {
            get => this._Order;

            set
            {
                if (this._Order != value)
                {

                    this.OnOrderChanging(value);
                    this.SendPropertyChanging();
                    this._Order = value;
                    this.SendPropertyChanged("Order");
                    this.OnOrderChanged();
                }

            }

        }


        [Column(Name = "enabled", UpdateCheck = UpdateCheck.Never, Storage = "_Enabled", DbType = "bit NOT NULL")]
        public bool Enabled
        {
            get => this._Enabled;

            set
            {
                if (this._Enabled != value)
                {

                    this.OnEnabledChanging(value);
                    this.SendPropertyChanging();
                    this._Enabled = value;
                    this.SendPropertyChanged("Enabled");
                    this.OnEnabledChanged();
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

