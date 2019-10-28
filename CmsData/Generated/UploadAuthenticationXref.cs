using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "disc.UploadAuthenticationXref")]
    public partial class UploadAuthenticationXref : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _Postinguser;

        private string _Postsfor;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnPostinguserChanging(string value);
        partial void OnPostinguserChanged();

        partial void OnPostsforChanging(string value);
        partial void OnPostsforChanged();

        #endregion

        public UploadAuthenticationXref()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "postinguser", UpdateCheck = UpdateCheck.Never, Storage = "_Postinguser", DbType = "nvarchar(20) NOT NULL", IsPrimaryKey = true)]
        public string Postinguser
        {
            get => _Postinguser;

            set
            {
                if (_Postinguser != value)
                {
                    OnPostinguserChanging(value);
                    SendPropertyChanging();
                    _Postinguser = value;
                    SendPropertyChanged("Postinguser");
                    OnPostinguserChanged();
                }
            }
        }

        [Column(Name = "postsfor", UpdateCheck = UpdateCheck.Never, Storage = "_Postsfor", DbType = "nvarchar(20) NOT NULL", IsPrimaryKey = true)]
        public string Postsfor
        {
            get => _Postsfor;

            set
            {
                if (_Postsfor != value)
                {
                    OnPostsforChanging(value);
                    SendPropertyChanging();
                    _Postsfor = value;
                    SendPropertyChanged("Postsfor");
                    OnPostsforChanged();
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
