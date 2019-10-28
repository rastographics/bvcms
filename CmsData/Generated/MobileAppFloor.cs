using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MobileAppFloor")]
    public partial class MobileAppFloor : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _Id;

        private int _Campus;

        private string _Name;

        private string _Image;

        private int _Order;

        private bool _Enabled;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnCampusChanging(int value);
        partial void OnCampusChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnImageChanging(string value);
        partial void OnImageChanged();

        partial void OnOrderChanging(int value);
        partial void OnOrderChanged();

        partial void OnEnabledChanging(bool value);
        partial void OnEnabledChanged();

        #endregion
        public MobileAppFloor()
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


        [Column(Name = "campus", UpdateCheck = UpdateCheck.Never, Storage = "_Campus", DbType = "int NOT NULL")]
        public int Campus
        {
            get => this._Campus;

            set
            {
                if (this._Campus != value)
                {

                    this.OnCampusChanging(value);
                    this.SendPropertyChanging();
                    this._Campus = value;
                    this.SendPropertyChanged("Campus");
                    this.OnCampusChanged();
                }

            }

        }


        [Column(Name = "name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(50) NOT NULL")]
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


        [Column(Name = "image", UpdateCheck = UpdateCheck.Never, Storage = "_Image", DbType = "nvarchar(250) NOT NULL")]
        public string Image
        {
            get => this._Image;

            set
            {
                if (this._Image != value)
                {

                    this.OnImageChanging(value);
                    this.SendPropertyChanging();
                    this._Image = value;
                    this.SendPropertyChanged("Image");
                    this.OnImageChanged();
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

