using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.EmailQueueToFail")]
    public partial class EmailQueueToFail : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int? _Id;

        private int? _PeopleId;

        private DateTime? _Time;

        private string _EventX;

        private string _Reason;

        private string _Bouncetype;

        private string _Email;

        private int _Pkey;

        private long? _Timestamp;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int? value);
        partial void OnIdChanged();

        partial void OnPeopleIdChanging(int? value);
        partial void OnPeopleIdChanged();

        partial void OnTimeChanging(DateTime? value);
        partial void OnTimeChanged();

        partial void OnEventXChanging(string value);
        partial void OnEventXChanged();

        partial void OnReasonChanging(string value);
        partial void OnReasonChanged();

        partial void OnBouncetypeChanging(string value);
        partial void OnBouncetypeChanged();

        partial void OnEmailChanging(string value);
        partial void OnEmailChanged();

        partial void OnPkeyChanging(int value);
        partial void OnPkeyChanged();

        partial void OnTimestampChanging(long? value);
        partial void OnTimestampChanged();

        #endregion

        public EmailQueueToFail()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int")]
        public int? Id
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

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
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

        [Column(Name = "event", UpdateCheck = UpdateCheck.Never, Storage = "_EventX", DbType = "nvarchar(20)")]
        public string EventX
        {
            get => _EventX;

            set
            {
                if (_EventX != value)
                {
                    OnEventXChanging(value);
                    SendPropertyChanging();
                    _EventX = value;
                    SendPropertyChanged("EventX");
                    OnEventXChanged();
                }
            }
        }

        [Column(Name = "reason", UpdateCheck = UpdateCheck.Never, Storage = "_Reason", DbType = "nvarchar(300)")]
        public string Reason
        {
            get => _Reason;

            set
            {
                if (_Reason != value)
                {
                    OnReasonChanging(value);
                    SendPropertyChanging();
                    _Reason = value;
                    SendPropertyChanged("Reason");
                    OnReasonChanged();
                }
            }
        }

        [Column(Name = "bouncetype", UpdateCheck = UpdateCheck.Never, Storage = "_Bouncetype", DbType = "nvarchar(20)")]
        public string Bouncetype
        {
            get => _Bouncetype;

            set
            {
                if (_Bouncetype != value)
                {
                    OnBouncetypeChanging(value);
                    SendPropertyChanging();
                    _Bouncetype = value;
                    SendPropertyChanged("Bouncetype");
                    OnBouncetypeChanged();
                }
            }
        }

        [Column(Name = "email", UpdateCheck = UpdateCheck.Never, Storage = "_Email", DbType = "nvarchar(100)")]
        public string Email
        {
            get => _Email;

            set
            {
                if (_Email != value)
                {
                    OnEmailChanging(value);
                    SendPropertyChanging();
                    _Email = value;
                    SendPropertyChanged("Email");
                    OnEmailChanged();
                }
            }
        }

        [Column(Name = "pkey", UpdateCheck = UpdateCheck.Never, Storage = "_Pkey", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Pkey
        {
            get => _Pkey;

            set
            {
                if (_Pkey != value)
                {
                    OnPkeyChanging(value);
                    SendPropertyChanging();
                    _Pkey = value;
                    SendPropertyChanged("Pkey");
                    OnPkeyChanged();
                }
            }
        }

        [Column(Name = "timestamp", UpdateCheck = UpdateCheck.Never, Storage = "_Timestamp", DbType = "bigint")]
        public long? Timestamp
        {
            get => _Timestamp;

            set
            {
                if (_Timestamp != value)
                {
                    OnTimestampChanging(value);
                    SendPropertyChanging();
                    _Timestamp = value;
                    SendPropertyChanged("Timestamp");
                    OnTimestampChanged();
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
