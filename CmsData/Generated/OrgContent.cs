using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.OrgContent")]
    public partial class OrgContent : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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

        [Column(Name = "OrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OrgId", DbType = "int")]
        public int? OrgId
        {
            get => _OrgId;

            set
            {
                if (_OrgId != value)
                {
                    OnOrgIdChanging(value);
                    SendPropertyChanging();
                    _OrgId = value;
                    SendPropertyChanged("OrgId");
                    OnOrgIdChanged();
                }
            }
        }

        [Column(Name = "AllowInactive", UpdateCheck = UpdateCheck.Never, Storage = "_AllowInactive", DbType = "bit")]
        public bool? AllowInactive
        {
            get => _AllowInactive;

            set
            {
                if (_AllowInactive != value)
                {
                    OnAllowInactiveChanging(value);
                    SendPropertyChanging();
                    _AllowInactive = value;
                    SendPropertyChanged("AllowInactive");
                    OnAllowInactiveChanged();
                }
            }
        }

        [Column(Name = "PublicView", UpdateCheck = UpdateCheck.Never, Storage = "_PublicView", DbType = "bit")]
        public bool? PublicView
        {
            get => _PublicView;

            set
            {
                if (_PublicView != value)
                {
                    OnPublicViewChanging(value);
                    SendPropertyChanging();
                    _PublicView = value;
                    SendPropertyChanged("PublicView");
                    OnPublicViewChanged();
                }
            }
        }

        [Column(Name = "ImageId", UpdateCheck = UpdateCheck.Never, Storage = "_ImageId", DbType = "int")]
        public int? ImageId
        {
            get => _ImageId;

            set
            {
                if (_ImageId != value)
                {
                    OnImageIdChanging(value);
                    SendPropertyChanging();
                    _ImageId = value;
                    SendPropertyChanged("ImageId");
                    OnImageIdChanged();
                }
            }
        }

        [Column(Name = "Landing", UpdateCheck = UpdateCheck.Never, Storage = "_Landing", DbType = "bit")]
        public bool? Landing
        {
            get => _Landing;

            set
            {
                if (_Landing != value)
                {
                    OnLandingChanging(value);
                    SendPropertyChanging();
                    _Landing = value;
                    SendPropertyChanged("Landing");
                    OnLandingChanged();
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
