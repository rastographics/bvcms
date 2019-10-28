using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MobileAppRoom")]
    public partial class MobileAppRoom : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _Id;

        private int _Floor;

        private string _Name;

        private string _Room;

        private int _X;

        private int _Y;

        private bool _Enabled;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnFloorChanging(int value);
        partial void OnFloorChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnRoomChanging(string value);
        partial void OnRoomChanged();

        partial void OnXChanging(int value);
        partial void OnXChanged();

        partial void OnYChanging(int value);
        partial void OnYChanged();

        partial void OnEnabledChanging(bool value);
        partial void OnEnabledChanged();

        #endregion
        public MobileAppRoom()
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


        [Column(Name = "floor", UpdateCheck = UpdateCheck.Never, Storage = "_Floor", DbType = "int NOT NULL")]
        public int Floor
        {
            get => this._Floor;

            set
            {
                if (this._Floor != value)
                {

                    this.OnFloorChanging(value);
                    this.SendPropertyChanging();
                    this._Floor = value;
                    this.SendPropertyChanged("Floor");
                    this.OnFloorChanged();
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


        [Column(Name = "room", UpdateCheck = UpdateCheck.Never, Storage = "_Room", DbType = "nvarchar(50) NOT NULL")]
        public string Room
        {
            get => this._Room;

            set
            {
                if (this._Room != value)
                {

                    this.OnRoomChanging(value);
                    this.SendPropertyChanging();
                    this._Room = value;
                    this.SendPropertyChanged("Room");
                    this.OnRoomChanged();
                }

            }

        }


        [Column(Name = "x", UpdateCheck = UpdateCheck.Never, Storage = "_X", DbType = "int NOT NULL")]
        public int X
        {
            get => this._X;

            set
            {
                if (this._X != value)
                {

                    this.OnXChanging(value);
                    this.SendPropertyChanging();
                    this._X = value;
                    this.SendPropertyChanged("X");
                    this.OnXChanged();
                }

            }

        }


        [Column(Name = "y", UpdateCheck = UpdateCheck.Never, Storage = "_Y", DbType = "int NOT NULL")]
        public int Y
        {
            get => this._Y;

            set
            {
                if (this._Y != value)
                {

                    this.OnYChanging(value);
                    this.SendPropertyChanging();
                    this._Y = value;
                    this.SendPropertyChanged("Y");
                    this.OnYChanged();
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

