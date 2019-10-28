using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.OrgContent")]
    public partial class OrgContent : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _Id;

        private int? _OrgId;

        private bool? _AllowInactive;

        private bool? _PublicView;

        private int? _ImageId;

        private bool? _Landing;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnOrgIdChanging(int? value);
        partial void OnOrgIdChanged();

        partial void OnAllowInactiveChanging(bool? value);
        partial void OnAllowInactiveChanged();

        partial void OnPublicViewChanging(bool? value);
        partial void OnPublicViewChanged();

        partial void OnImageIdChanging(int? value);
        partial void OnImageIdChanged();

        partial void OnLandingChanging(bool? value);
        partial void OnLandingChanged();

        #endregion
        public OrgContent()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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


        [Column(Name = "OrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OrgId", DbType = "int")]
        public int? OrgId
        {
            get => this._OrgId;

            set
            {
                if (this._OrgId != value)
                {

                    this.OnOrgIdChanging(value);
                    this.SendPropertyChanging();
                    this._OrgId = value;
                    this.SendPropertyChanged("OrgId");
                    this.OnOrgIdChanged();
                }

            }

        }


        [Column(Name = "AllowInactive", UpdateCheck = UpdateCheck.Never, Storage = "_AllowInactive", DbType = "bit")]
        public bool? AllowInactive
        {
            get => this._AllowInactive;

            set
            {
                if (this._AllowInactive != value)
                {

                    this.OnAllowInactiveChanging(value);
                    this.SendPropertyChanging();
                    this._AllowInactive = value;
                    this.SendPropertyChanged("AllowInactive");
                    this.OnAllowInactiveChanged();
                }

            }

        }


        [Column(Name = "PublicView", UpdateCheck = UpdateCheck.Never, Storage = "_PublicView", DbType = "bit")]
        public bool? PublicView
        {
            get => this._PublicView;

            set
            {
                if (this._PublicView != value)
                {

                    this.OnPublicViewChanging(value);
                    this.SendPropertyChanging();
                    this._PublicView = value;
                    this.SendPropertyChanged("PublicView");
                    this.OnPublicViewChanged();
                }

            }

        }


        [Column(Name = "ImageId", UpdateCheck = UpdateCheck.Never, Storage = "_ImageId", DbType = "int")]
        public int? ImageId
        {
            get => this._ImageId;

            set
            {
                if (this._ImageId != value)
                {

                    this.OnImageIdChanging(value);
                    this.SendPropertyChanging();
                    this._ImageId = value;
                    this.SendPropertyChanged("ImageId");
                    this.OnImageIdChanged();
                }

            }

        }


        [Column(Name = "Landing", UpdateCheck = UpdateCheck.Never, Storage = "_Landing", DbType = "bit")]
        public bool? Landing
        {
            get => this._Landing;

            set
            {
                if (this._Landing != value)
                {

                    this.OnLandingChanging(value);
                    this.SendPropertyChanging();
                    this._Landing = value;
                    this.SendPropertyChanged("Landing");
                    this.OnLandingChanged();
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

