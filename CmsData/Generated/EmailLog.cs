using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.EmailLog")]
    public partial class EmailLog : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Fromaddr;

        private string _Toaddr;

        private DateTime? _Time;

        private string _Subject;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnFromaddrChanging(string value);
        partial void OnFromaddrChanged();

        partial void OnToaddrChanging(string value);
        partial void OnToaddrChanged();

        partial void OnTimeChanging(DateTime? value);
        partial void OnTimeChanged();

        partial void OnSubjectChanging(string value);
        partial void OnSubjectChanged();

        #endregion

        public EmailLog()
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

        [Column(Name = "fromaddr", UpdateCheck = UpdateCheck.Never, Storage = "_Fromaddr", DbType = "nvarchar(50)")]
        public string Fromaddr
        {
            get => _Fromaddr;

            set
            {
                if (_Fromaddr != value)
                {
                    OnFromaddrChanging(value);
                    SendPropertyChanging();
                    _Fromaddr = value;
                    SendPropertyChanged("Fromaddr");
                    OnFromaddrChanged();
                }
            }
        }

        [Column(Name = "toaddr", UpdateCheck = UpdateCheck.Never, Storage = "_Toaddr", DbType = "nvarchar(150)")]
        public string Toaddr
        {
            get => _Toaddr;

            set
            {
                if (_Toaddr != value)
                {
                    OnToaddrChanging(value);
                    SendPropertyChanging();
                    _Toaddr = value;
                    SendPropertyChanged("Toaddr");
                    OnToaddrChanged();
                }
            }
        }

        [Column(Name = "time", UpdateCheck = UpdateCheck.Never, Storage = "_Time", DbType = "datetime")]
        public DateTime? Time
        {
            get => _Time;

            set
            {
                if (_Time != value)
                {
                    OnTimeChanging(value);
                    SendPropertyChanging();
                    _Time = value;
                    SendPropertyChanged("Time");
                    OnTimeChanged();
                }
            }
        }

        [Column(Name = "subject", UpdateCheck = UpdateCheck.Never, Storage = "_Subject", DbType = "nvarchar(180)")]
        public string Subject
        {
            get => _Subject;

            set
            {
                if (_Subject != value)
                {
                    OnSubjectChanging(value);
                    SendPropertyChanging();
                    _Subject = value;
                    SendPropertyChanged("Subject");
                    OnSubjectChanged();
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
