using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "disc.UploadAuthenticationXref")]
    public partial class UploadAuthenticationXref : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._Postinguser;

            set
            {
                if (this._Postinguser != value)
                {

                    this.OnPostinguserChanging(value);
                    this.SendPropertyChanging();
                    this._Postinguser = value;
                    this.SendPropertyChanged("Postinguser");
                    this.OnPostinguserChanged();
                }

            }

        }


        [Column(Name = "postsfor", UpdateCheck = UpdateCheck.Never, Storage = "_Postsfor", DbType = "nvarchar(20) NOT NULL", IsPrimaryKey = true)]
        public string Postsfor
        {
            get => this._Postsfor;

            set
            {
                if (this._Postsfor != value)
                {

                    this.OnPostsforChanging(value);
                    this.SendPropertyChanging();
                    this._Postsfor = value;
                    this.SendPropertyChanged("Postsfor");
                    this.OnPostsforChanged();
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

