using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.AuditValues")]
    public partial class AuditValue : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _AuditId;

        private string _MemberName;

        private string _OldValue;

        private string _NewValue;

        private EntityRef<Audit> _Audit;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnAuditIdChanging(int value);
        partial void OnAuditIdChanged();

        partial void OnMemberNameChanging(string value);
        partial void OnMemberNameChanged();

        partial void OnOldValueChanging(string value);
        partial void OnOldValueChanged();

        partial void OnNewValueChanging(string value);
        partial void OnNewValueChanged();

        #endregion

        public AuditValue()
        {
            _Audit = default(EntityRef<Audit>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "AuditId", UpdateCheck = UpdateCheck.Never, Storage = "_AuditId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int AuditId
        {
            get => _AuditId;

            set
            {
                if (_AuditId != value)
                {
                    if (_Audit.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnAuditIdChanging(value);
                    SendPropertyChanging();
                    _AuditId = value;
                    SendPropertyChanged("AuditId");
                    OnAuditIdChanged();
                }
            }
        }

        [Column(Name = "MemberName", UpdateCheck = UpdateCheck.Never, Storage = "_MemberName", DbType = "nvarchar(50) NOT NULL", IsPrimaryKey = true)]
        public string MemberName
        {
            get => _MemberName;

            set
            {
                if (_MemberName != value)
                {
                    OnMemberNameChanging(value);
                    SendPropertyChanging();
                    _MemberName = value;
                    SendPropertyChanged("MemberName");
                    OnMemberNameChanged();
                }
            }
        }

        [Column(Name = "OldValue", UpdateCheck = UpdateCheck.Never, Storage = "_OldValue", DbType = "nvarchar")]
        public string OldValue
        {
            get => _OldValue;

            set
            {
                if (_OldValue != value)
                {
                    OnOldValueChanging(value);
                    SendPropertyChanging();
                    _OldValue = value;
                    SendPropertyChanged("OldValue");
                    OnOldValueChanged();
                }
            }
        }

        [Column(Name = "NewValue", UpdateCheck = UpdateCheck.Never, Storage = "_NewValue", DbType = "nvarchar")]
        public string NewValue
        {
            get => _NewValue;

            set
            {
                if (_NewValue != value)
                {
                    OnNewValueChanging(value);
                    SendPropertyChanging();
                    _NewValue = value;
                    SendPropertyChanged("NewValue");
                    OnNewValueChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_AuditValues_Audits", Storage = "_Audit", ThisKey = "AuditId", IsForeignKey = true)]
        public Audit Audit
        {
            get => _Audit.Entity;

            set
            {
                Audit previousValue = _Audit.Entity;
                if (((previousValue != value)
                            || (_Audit.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Audit.Entity = null;
                        previousValue.AuditValues.Remove(this);
                    }

                    _Audit.Entity = value;
                    if (value != null)
                    {
                        value.AuditValues.Add(this);

                        _AuditId = value.Id;

                    }

                    else
                    {
                        _AuditId = default(int);

                    }

                    SendPropertyChanged("Audit");
                }
            }
        }

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
