using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.EmailQueueTo")]
    public partial class EmailQueueTo : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _PeopleId;

        private int? _OrgId;

        private DateTime? _Sent;

        private string _AddEmail;

        private Guid? _Guid;

        private string _Messageid;

        private int? _GoerSupportId;

        private int? _Parent1;

        private int? _Parent2;

        private bool? _Bounced;

        private bool? _SpamReport;

        private bool? _Blocked;

        private bool? _Expired;

        private bool? _SpamContent;

        private bool? _Invalid;

        private bool? _BouncedAddress;

        private bool? _SpamReporting;

        private string _DomainFrom;

        private EntityRef<EmailQueue> _EmailQueue;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnOrgIdChanging(int? value);
        partial void OnOrgIdChanged();

        partial void OnSentChanging(DateTime? value);
        partial void OnSentChanged();

        partial void OnAddEmailChanging(string value);
        partial void OnAddEmailChanged();

        partial void OnGuidChanging(Guid? value);
        partial void OnGuidChanged();

        partial void OnMessageidChanging(string value);
        partial void OnMessageidChanged();

        partial void OnGoerSupportIdChanging(int? value);
        partial void OnGoerSupportIdChanged();

        partial void OnParent1Changing(int? value);
        partial void OnParent1Changed();

        partial void OnParent2Changing(int? value);
        partial void OnParent2Changed();

        partial void OnBouncedChanging(bool? value);
        partial void OnBouncedChanged();

        partial void OnSpamReportChanging(bool? value);
        partial void OnSpamReportChanged();

        partial void OnBlockedChanging(bool? value);
        partial void OnBlockedChanged();

        partial void OnExpiredChanging(bool? value);
        partial void OnExpiredChanged();

        partial void OnSpamContentChanging(bool? value);
        partial void OnSpamContentChanged();

        partial void OnInvalidChanging(bool? value);
        partial void OnInvalidChanged();

        partial void OnBouncedAddressChanging(bool? value);
        partial void OnBouncedAddressChanged();

        partial void OnSpamReportingChanging(bool? value);
        partial void OnSpamReportingChanged();

        partial void OnDomainFromChanging(string value);
        partial void OnDomainFromChanged();

        #endregion

        public EmailQueueTo()
        {
            _EmailQueue = default(EntityRef<EmailQueue>);

            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    if (_EmailQueue.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
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

        [Column(Name = "Sent", UpdateCheck = UpdateCheck.Never, Storage = "_Sent", DbType = "datetime")]
        public DateTime? Sent
        {
            get => _Sent;

            set
            {
                if (_Sent != value)
                {
                    OnSentChanging(value);
                    SendPropertyChanging();
                    _Sent = value;
                    SendPropertyChanged("Sent");
                    OnSentChanged();
                }
            }
        }

        [Column(Name = "AddEmail", UpdateCheck = UpdateCheck.Never, Storage = "_AddEmail", DbType = "nvarchar")]
        public string AddEmail
        {
            get => _AddEmail;

            set
            {
                if (_AddEmail != value)
                {
                    OnAddEmailChanging(value);
                    SendPropertyChanging();
                    _AddEmail = value;
                    SendPropertyChanged("AddEmail");
                    OnAddEmailChanged();
                }
            }
        }

        [Column(Name = "guid", UpdateCheck = UpdateCheck.Never, Storage = "_Guid", DbType = "uniqueidentifier")]
        public Guid? Guid
        {
            get => _Guid;

            set
            {
                if (_Guid != value)
                {
                    OnGuidChanging(value);
                    SendPropertyChanging();
                    _Guid = value;
                    SendPropertyChanged("Guid");
                    OnGuidChanged();
                }
            }
        }

        [Column(Name = "messageid", UpdateCheck = UpdateCheck.Never, Storage = "_Messageid", DbType = "nvarchar(100)")]
        public string Messageid
        {
            get => _Messageid;

            set
            {
                if (_Messageid != value)
                {
                    OnMessageidChanging(value);
                    SendPropertyChanging();
                    _Messageid = value;
                    SendPropertyChanged("Messageid");
                    OnMessageidChanged();
                }
            }
        }

        [Column(Name = "GoerSupportId", UpdateCheck = UpdateCheck.Never, Storage = "_GoerSupportId", DbType = "int")]
        public int? GoerSupportId
        {
            get => _GoerSupportId;

            set
            {
                if (_GoerSupportId != value)
                {
                    OnGoerSupportIdChanging(value);
                    SendPropertyChanging();
                    _GoerSupportId = value;
                    SendPropertyChanged("GoerSupportId");
                    OnGoerSupportIdChanged();
                }
            }
        }

        [Column(Name = "Parent1", UpdateCheck = UpdateCheck.Never, Storage = "_Parent1", DbType = "int")]
        public int? Parent1
        {
            get => _Parent1;

            set
            {
                if (_Parent1 != value)
                {
                    OnParent1Changing(value);
                    SendPropertyChanging();
                    _Parent1 = value;
                    SendPropertyChanged("Parent1");
                    OnParent1Changed();
                }
            }
        }

        [Column(Name = "Parent2", UpdateCheck = UpdateCheck.Never, Storage = "_Parent2", DbType = "int")]
        public int? Parent2
        {
            get => _Parent2;

            set
            {
                if (_Parent2 != value)
                {
                    OnParent2Changing(value);
                    SendPropertyChanging();
                    _Parent2 = value;
                    SendPropertyChanged("Parent2");
                    OnParent2Changed();
                }
            }
        }

        [Column(Name = "Bounced", UpdateCheck = UpdateCheck.Never, Storage = "_Bounced", DbType = "bit")]
        public bool? Bounced
        {
            get => _Bounced;

            set
            {
                if (_Bounced != value)
                {
                    OnBouncedChanging(value);
                    SendPropertyChanging();
                    _Bounced = value;
                    SendPropertyChanged("Bounced");
                    OnBouncedChanged();
                }
            }
        }

        [Column(Name = "SpamReport", UpdateCheck = UpdateCheck.Never, Storage = "_SpamReport", DbType = "bit")]
        public bool? SpamReport
        {
            get => _SpamReport;

            set
            {
                if (_SpamReport != value)
                {
                    OnSpamReportChanging(value);
                    SendPropertyChanging();
                    _SpamReport = value;
                    SendPropertyChanged("SpamReport");
                    OnSpamReportChanged();
                }
            }
        }

        [Column(Name = "Blocked", UpdateCheck = UpdateCheck.Never, Storage = "_Blocked", DbType = "bit")]
        public bool? Blocked
        {
            get => _Blocked;

            set
            {
                if (_Blocked != value)
                {
                    OnBlockedChanging(value);
                    SendPropertyChanging();
                    _Blocked = value;
                    SendPropertyChanged("Blocked");
                    OnBlockedChanged();
                }
            }
        }

        [Column(Name = "Expired", UpdateCheck = UpdateCheck.Never, Storage = "_Expired", DbType = "bit")]
        public bool? Expired
        {
            get => _Expired;

            set
            {
                if (_Expired != value)
                {
                    OnExpiredChanging(value);
                    SendPropertyChanging();
                    _Expired = value;
                    SendPropertyChanged("Expired");
                    OnExpiredChanged();
                }
            }
        }

        [Column(Name = "SpamContent", UpdateCheck = UpdateCheck.Never, Storage = "_SpamContent", DbType = "bit")]
        public bool? SpamContent
        {
            get => _SpamContent;

            set
            {
                if (_SpamContent != value)
                {
                    OnSpamContentChanging(value);
                    SendPropertyChanging();
                    _SpamContent = value;
                    SendPropertyChanged("SpamContent");
                    OnSpamContentChanged();
                }
            }
        }

        [Column(Name = "Invalid", UpdateCheck = UpdateCheck.Never, Storage = "_Invalid", DbType = "bit")]
        public bool? Invalid
        {
            get => _Invalid;

            set
            {
                if (_Invalid != value)
                {
                    OnInvalidChanging(value);
                    SendPropertyChanging();
                    _Invalid = value;
                    SendPropertyChanged("Invalid");
                    OnInvalidChanged();
                }
            }
        }

        [Column(Name = "BouncedAddress", UpdateCheck = UpdateCheck.Never, Storage = "_BouncedAddress", DbType = "bit")]
        public bool? BouncedAddress
        {
            get => _BouncedAddress;

            set
            {
                if (_BouncedAddress != value)
                {
                    OnBouncedAddressChanging(value);
                    SendPropertyChanging();
                    _BouncedAddress = value;
                    SendPropertyChanged("BouncedAddress");
                    OnBouncedAddressChanged();
                }
            }
        }

        [Column(Name = "SpamReporting", UpdateCheck = UpdateCheck.Never, Storage = "_SpamReporting", DbType = "bit")]
        public bool? SpamReporting
        {
            get => _SpamReporting;

            set
            {
                if (_SpamReporting != value)
                {
                    OnSpamReportingChanging(value);
                    SendPropertyChanging();
                    _SpamReporting = value;
                    SendPropertyChanged("SpamReporting");
                    OnSpamReportingChanged();
                }
            }
        }

        [Column(Name = "DomainFrom", UpdateCheck = UpdateCheck.Never, Storage = "_DomainFrom", DbType = "varchar(30)")]
        public string DomainFrom
        {
            get => _DomainFrom;

            set
            {
                if (_DomainFrom != value)
                {
                    OnDomainFromChanging(value);
                    SendPropertyChanging();
                    _DomainFrom = value;
                    SendPropertyChanged("DomainFrom");
                    OnDomainFromChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_EmailQueueTo_EmailQueue", Storage = "_EmailQueue", ThisKey = "Id", IsForeignKey = true)]
        public EmailQueue EmailQueue
        {
            get => _EmailQueue.Entity;

            set
            {
                EmailQueue previousValue = _EmailQueue.Entity;
                if (((previousValue != value)
                            || (_EmailQueue.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _EmailQueue.Entity = null;
                        previousValue.EmailQueueTos.Remove(this);
                    }

                    _EmailQueue.Entity = value;
                    if (value != null)
                    {
                        value.EmailQueueTos.Add(this);

                        _Id = value.Id;

                    }

                    else
                    {
                        _Id = default(int);

                    }

                    SendPropertyChanged("EmailQueue");
                }
            }
        }

        [Association(Name = "FK_EmailQueueTo_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
        public Person Person
        {
            get => _Person.Entity;

            set
            {
                Person previousValue = _Person.Entity;
                if (((previousValue != value)
                            || (_Person.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Person.Entity = null;
                        previousValue.EmailQueueTos.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.EmailQueueTos.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int);

                    }

                    SendPropertyChanged("Person");
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
