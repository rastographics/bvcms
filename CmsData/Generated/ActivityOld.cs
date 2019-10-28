using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ActivityOld")]
    public partial class ActivityOld : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private long _Id;

        private DateTime? _ActivityDate;

        private int? _UserId;

        private string _Activity;

        private string _PageUrl;

        private string _Machine;

        private int? _OrgId;

        private int? _PeopleId;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(long value);
        partial void OnIdChanged();

        partial void OnActivityDateChanging(DateTime? value);
        partial void OnActivityDateChanged();

        partial void OnUserIdChanging(int? value);
        partial void OnUserIdChanged();

        partial void OnActivityChanging(string value);
        partial void OnActivityChanged();

        partial void OnPageUrlChanging(string value);
        partial void OnPageUrlChanged();

        partial void OnMachineChanging(string value);
        partial void OnMachineChanged();

        partial void OnOrgIdChanging(int? value);
        partial void OnOrgIdChanged();

        partial void OnPeopleIdChanging(int? value);
        partial void OnPeopleIdChanged();

        #endregion

        public ActivityOld()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "bigint NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id
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

        [Column(Name = "ActivityDate", UpdateCheck = UpdateCheck.Never, Storage = "_ActivityDate", DbType = "datetime")]
        public DateTime? ActivityDate
        {
            get => _ActivityDate;

            set
            {
                if (_ActivityDate != value)
                {
                    OnActivityDateChanging(value);
                    SendPropertyChanging();
                    _ActivityDate = value;
                    SendPropertyChanged("ActivityDate");
                    OnActivityDateChanged();
                }
            }
        }

        [Column(Name = "UserId", UpdateCheck = UpdateCheck.Never, Storage = "_UserId", DbType = "int")]
        public int? UserId
        {
            get => _UserId;

            set
            {
                if (_UserId != value)
                {
                    OnUserIdChanging(value);
                    SendPropertyChanging();
                    _UserId = value;
                    SendPropertyChanged("UserId");
                    OnUserIdChanged();
                }
            }
        }

        [Column(Name = "Activity", UpdateCheck = UpdateCheck.Never, Storage = "_Activity", DbType = "varchar(200)")]
        public string Activity
        {
            get => _Activity;

            set
            {
                if (_Activity != value)
                {
                    OnActivityChanging(value);
                    SendPropertyChanging();
                    _Activity = value;
                    SendPropertyChanged("Activity");
                    OnActivityChanged();
                }
            }
        }

        [Column(Name = "PageUrl", UpdateCheck = UpdateCheck.Never, Storage = "_PageUrl", DbType = "varchar(410)")]
        public string PageUrl
        {
            get => _PageUrl;

            set
            {
                if (_PageUrl != value)
                {
                    OnPageUrlChanging(value);
                    SendPropertyChanging();
                    _PageUrl = value;
                    SendPropertyChanged("PageUrl");
                    OnPageUrlChanged();
                }
            }
        }

        [Column(Name = "Machine", UpdateCheck = UpdateCheck.Never, Storage = "_Machine", DbType = "varchar(50)")]
        public string Machine
        {
            get => _Machine;

            set
            {
                if (_Machine != value)
                {
                    OnMachineChanging(value);
                    SendPropertyChanging();
                    _Machine = value;
                    SendPropertyChanged("Machine");
                    OnMachineChanged();
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
