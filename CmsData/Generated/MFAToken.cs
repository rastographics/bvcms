using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using CmsData.Infrastructure;

namespace CmsData
{
    [Table(Name = "dbo.MFATokens")]
    public partial class MFAToken : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private Guid _Id;

        private string _Key;

        private int? _UserId;

        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(Guid value);
        partial void OnIdChanged();

        partial void OnUserIdChanging(int? value);
        partial void OnUserIdChanged();

        partial void OnKeyChanging(string value);
        partial void OnKeyChanged();

        #endregion
        public MFAToken()
        {
            OnCreated();
        }


        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "uniqueidentifier NOT NULL", IsPrimaryKey = true, IsDbGenerated = true)]
        public Guid Id
        {
            get { return this._Id; }

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

        [Column(Name = "UserId", UpdateCheck = UpdateCheck.Never, Storage = "_UserId", DbType = "int")]
        [IsForeignKey]
        public int? UserId
        {
            get { return this._UserId; }

            set
            {
                if (this._UserId != value)
                {
                    this.OnUserIdChanging(value);
                    this.SendPropertyChanging();
                    this._UserId = value;
                    this.SendPropertyChanged("UserId");
                    this.OnUserIdChanged();
                }

            }

        }

        [Column(Name = "Key", UpdateCheck = UpdateCheck.Never, Storage = "_Key", DbType = "nvarchar(200)")]
        public string Key
        {
            get { return this._Key; }

            set
            {
                if (this._Key != value)
                {
                    this.OnKeyChanging(value);
                    this.SendPropertyChanging();
                    this._Key = value;
                    this.SendPropertyChanged("Key");
                    this.OnKeyChanged();
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
                this.PropertyChanging(this, emptyChangingEventArgs);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

