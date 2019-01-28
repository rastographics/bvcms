using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MeetingCategory")]
    public partial class MeetingCategory : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private long _Id;

        private string _Description;

        private DateTime? _NotBeforeDate;

        private DateTime? _NotAfterDate;

        private bool _IsExpired;

        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(long value);
        partial void OnIdChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnNotBeforeDateChanging(DateTime? value);
        partial void OnNotBeforeDateChanged();

        partial void OnNotAfterDateChanging(DateTime? value);
        partial void OnNotAfterDateChanged();

        partial void OnIsExpiredChanging(bool value);
        partial void OnIsExpiredChanged();

        #endregion
        public MeetingCategory()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "bigint NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public long Id
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


        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(100)")]
        public string Description
        {
            get { return this._Description; }

            set
            {
                if (this._Description != value)
                {

                    this.OnDescriptionChanging(value);
                    this.SendPropertyChanging();
                    this._Description = value;
                    this.SendPropertyChanged("Description");
                    this.OnDescriptionChanged();
                }

            }

        }


        [Column(Name = "NotBeforeDate", UpdateCheck = UpdateCheck.Never, Storage = "_NotBeforeDate", DbType = "datetime")]
        public DateTime? NotBeforeDate
        {
            get { return this._NotBeforeDate; }

            set
            {
                if (this._NotBeforeDate != value)
                {

                    this.OnNotBeforeDateChanging(value);
                    this.SendPropertyChanging();
                    this._NotBeforeDate = value;
                    this.SendPropertyChanged("NotBeforeDate");
                    this.OnNotBeforeDateChanged();
                }

            }

        }


        [Column(Name = "NotAfterDate", UpdateCheck = UpdateCheck.Never, Storage = "_NotAfterDate", DbType = "datetime")]
        public DateTime? NotAfterDate
        {
            get { return this._NotAfterDate; }

            set
            {
                if (this._NotAfterDate != value)
                {

                    this.OnNotAfterDateChanging(value);
                    this.SendPropertyChanging();
                    this._NotAfterDate = value;
                    this.SendPropertyChanged("NotAfterDate");
                    this.OnNotAfterDateChanged();
                }

            }

        }

        [Column(Name = "IsExpired", UpdateCheck = UpdateCheck.Never, Storage = "_IsExpired", DbType = "bit NOT NULL")]
        public bool IsExpired
        {
            get { return this._IsExpired; }

            set
            {
                if (this._IsExpired != value)
                {

                    this.OnIsExpiredChanging(value);
                    this.SendPropertyChanging();
                    this._IsExpired = value;
                    this.SendPropertyChanged("IsExpired");
                    this.OnIsExpiredChanged();
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

