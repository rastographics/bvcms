using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MobileAppFloor")]
    public partial class MobileAppFloor : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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

        [Column(Name = "campus", UpdateCheck = UpdateCheck.Never, Storage = "_Campus", DbType = "int NOT NULL")]
        public int Campus
        {
            get => _Campus;

            set
            {
                if (_Campus != value)
                {
                    OnCampusChanging(value);
                    SendPropertyChanging();
                    _Campus = value;
                    SendPropertyChanged("Campus");
                    OnCampusChanged();
                }
            }
        }

        [Column(Name = "name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(50) NOT NULL")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    OnNameChanging(value);
                    SendPropertyChanging();
                    _Name = value;
                    SendPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }

        [Column(Name = "image", UpdateCheck = UpdateCheck.Never, Storage = "_Image", DbType = "nvarchar(250) NOT NULL")]
        public string Image
        {
            get => _Image;

            set
            {
                if (_Image != value)
                {
                    OnImageChanging(value);
                    SendPropertyChanging();
                    _Image = value;
                    SendPropertyChanged("Image");
                    OnImageChanged();
                }
            }
        }

        [Column(Name = "order", UpdateCheck = UpdateCheck.Never, Storage = "_Order", DbType = "int NOT NULL")]
        public int Order
        {
            get => _Order;

            set
            {
                if (_Order != value)
                {
                    OnOrderChanging(value);
                    SendPropertyChanging();
                    _Order = value;
                    SendPropertyChanged("Order");
                    OnOrderChanged();
                }
            }
        }

        [Column(Name = "enabled", UpdateCheck = UpdateCheck.Never, Storage = "_Enabled", DbType = "bit NOT NULL")]
        public bool Enabled
        {
            get => _Enabled;

            set
            {
                if (_Enabled != value)
                {
                    OnEnabledChanging(value);
                    SendPropertyChanging();
                    _Enabled = value;
                    SendPropertyChanged("Enabled");
                    OnEnabledChanged();
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
