using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Audits")]
    public partial class Audit : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Action;

        private string _TableName;

        private int? _TableKey;

        private string _UserName;

        private DateTime _AuditDate;

        private EntitySet<AuditValue> _AuditValues;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnActionChanging(string value);
        partial void OnActionChanged();

        partial void OnTableNameChanging(string value);
        partial void OnTableNameChanged();

        partial void OnTableKeyChanging(int? value);
        partial void OnTableKeyChanged();

        partial void OnUserNameChanging(string value);
        partial void OnUserNameChanged();

        partial void OnAuditDateChanging(DateTime value);
        partial void OnAuditDateChanged();

        #endregion

        public Audit()
        {
            _AuditValues = new EntitySet<AuditValue>(new Action<AuditValue>(attach_AuditValues), new Action<AuditValue>(detach_AuditValues));

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

        [Column(Name = "Action", UpdateCheck = UpdateCheck.Never, Storage = "_Action", DbType = "nvarchar(20) NOT NULL")]
        public string Action
        {
            get => _Action;

            set
            {
                if (_Action != value)
                {
                    OnActionChanging(value);
                    SendPropertyChanging();
                    _Action = value;
                    SendPropertyChanged("Action");
                    OnActionChanged();
                }
            }
        }

        [Column(Name = "TableName", UpdateCheck = UpdateCheck.Never, Storage = "_TableName", DbType = "nvarchar(100) NOT NULL")]
        public string TableName
        {
            get => _TableName;

            set
            {
                if (_TableName != value)
                {
                    OnTableNameChanging(value);
                    SendPropertyChanging();
                    _TableName = value;
                    SendPropertyChanged("TableName");
                    OnTableNameChanged();
                }
            }
        }

        [Column(Name = "TableKey", UpdateCheck = UpdateCheck.Never, Storage = "_TableKey", DbType = "int")]
        public int? TableKey
        {
            get => _TableKey;

            set
            {
                if (_TableKey != value)
                {
                    OnTableKeyChanging(value);
                    SendPropertyChanging();
                    _TableKey = value;
                    SendPropertyChanged("TableKey");
                    OnTableKeyChanged();
                }
            }
        }

        [Column(Name = "UserName", UpdateCheck = UpdateCheck.Never, Storage = "_UserName", DbType = "nvarchar(50) NOT NULL")]
        public string UserName
        {
            get => _UserName;

            set
            {
                if (_UserName != value)
                {
                    OnUserNameChanging(value);
                    SendPropertyChanging();
                    _UserName = value;
                    SendPropertyChanged("UserName");
                    OnUserNameChanged();
                }
            }
        }

        [Column(Name = "AuditDate", UpdateCheck = UpdateCheck.Never, Storage = "_AuditDate", DbType = "smalldatetime NOT NULL")]
        public DateTime AuditDate
        {
            get => _AuditDate;

            set
            {
                if (_AuditDate != value)
                {
                    OnAuditDateChanging(value);
                    SendPropertyChanging();
                    _AuditDate = value;
                    SendPropertyChanged("AuditDate");
                    OnAuditDateChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_AuditValues_Audits", Storage = "_AuditValues", OtherKey = "AuditId")]
        public EntitySet<AuditValue> AuditValues
           {
               get => _AuditValues;

            set => _AuditValues.Assign(value);

           }

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

        private void attach_AuditValues(AuditValue entity)
        {
            SendPropertyChanging();
            entity.Audit = this;
        }

        private void detach_AuditValues(AuditValue entity)
        {
            SendPropertyChanging();
            entity.Audit = null;
        }
    }
}
