using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ActivityLog")]
    public partial class ActivityLog : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private long _Id;

        private DateTime? _ActivityDate;

        private int? _UserId;

        private string _Activity;

        private string _PageUrl;

        private string _Machine;

        private int? _OrgId;

        private int? _PeopleId;

        private int? _DatumId;

        private string _ClientIp;



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

        partial void OnDatumIdChanging(int? value);
        partial void OnDatumIdChanged();

        partial void OnClientIpChanging(string value);
        partial void OnClientIpChanged();

        #endregion
        public ActivityLog()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "bigint NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public long Id
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


        [Column(Name = "ActivityDate", UpdateCheck = UpdateCheck.Never, Storage = "_ActivityDate", DbType = "datetime")]
        public DateTime? ActivityDate
        {
            get => this._ActivityDate;

            set
            {
                if (this._ActivityDate != value)
                {

                    this.OnActivityDateChanging(value);
                    this.SendPropertyChanging();
                    this._ActivityDate = value;
                    this.SendPropertyChanged("ActivityDate");
                    this.OnActivityDateChanged();
                }

            }

        }


        [Column(Name = "UserId", UpdateCheck = UpdateCheck.Never, Storage = "_UserId", DbType = "int")]
        public int? UserId
        {
            get => this._UserId;

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


        [Column(Name = "Activity", UpdateCheck = UpdateCheck.Never, Storage = "_Activity", DbType = "nvarchar(200)")]
        public string Activity
        {
            get => this._Activity;

            set
            {
                if (this._Activity != value)
                {

                    this.OnActivityChanging(value);
                    this.SendPropertyChanging();
                    this._Activity = value;
                    this.SendPropertyChanged("Activity");
                    this.OnActivityChanged();
                }

            }

        }


        [Column(Name = "PageUrl", UpdateCheck = UpdateCheck.Never, Storage = "_PageUrl", DbType = "nvarchar(410)")]
        public string PageUrl
        {
            get => this._PageUrl;

            set
            {
                if (this._PageUrl != value)
                {

                    this.OnPageUrlChanging(value);
                    this.SendPropertyChanging();
                    this._PageUrl = value;
                    this.SendPropertyChanged("PageUrl");
                    this.OnPageUrlChanged();
                }

            }

        }


        [Column(Name = "Machine", UpdateCheck = UpdateCheck.Never, Storage = "_Machine", DbType = "nvarchar(50)")]
        public string Machine
        {
            get => this._Machine;

            set
            {
                if (this._Machine != value)
                {

                    this.OnMachineChanging(value);
                    this.SendPropertyChanging();
                    this._Machine = value;
                    this.SendPropertyChanged("Machine");
                    this.OnMachineChanged();
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


        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
        {
            get => this._PeopleId;

            set
            {
                if (this._PeopleId != value)
                {

                    this.OnPeopleIdChanging(value);
                    this.SendPropertyChanging();
                    this._PeopleId = value;
                    this.SendPropertyChanged("PeopleId");
                    this.OnPeopleIdChanged();
                }

            }

        }


        [Column(Name = "DatumId", UpdateCheck = UpdateCheck.Never, Storage = "_DatumId", DbType = "int")]
        public int? DatumId
        {
            get => this._DatumId;

            set
            {
                if (this._DatumId != value)
                {

                    this.OnDatumIdChanging(value);
                    this.SendPropertyChanging();
                    this._DatumId = value;
                    this.SendPropertyChanged("DatumId");
                    this.OnDatumIdChanged();
                }

            }

        }


        [Column(Name = "ClientIp", UpdateCheck = UpdateCheck.Never, Storage = "_ClientIp", DbType = "nvarchar(50)")]
        public string ClientIp
        {
            get => this._ClientIp;

            set
            {
                if (this._ClientIp != value)
                {

                    this.OnClientIpChanging(value);
                    this.SendPropertyChanging();
                    this._ClientIp = value;
                    this.SendPropertyChanged("ClientIp");
                    this.OnClientIpChanged();
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

