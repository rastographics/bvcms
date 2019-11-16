using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.EmailQueue")]
    public partial class EmailQueue : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private DateTime? _SendWhen;

        private string _Subject;

        private string _Body;

        private string _FromAddr;

        private DateTime? _Sent;

        private DateTime? _Started;

        private DateTime _Queued;

        private string _FromName;

        private int? _QueuedBy;

        private bool? _Redacted;

        private bool? _Transactional;

        private bool? _PublicX;

        private string _Error;

        private bool? _CCParents;

        private bool? _NoReplacements;

        private int? _SendFromOrgId;

        private bool? _FinanceOnly;

        private string _CClist;

        private bool? _Testing;

        private bool? _ReadyToSend;

        private EntitySet<EmailLink> _EmailLinks;

        private EntitySet<EmailQueueTo> _EmailQueueTos;

        private EntitySet<EmailResponse> _EmailResponses;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnSendWhenChanging(DateTime? value);
        partial void OnSendWhenChanged();

        partial void OnSubjectChanging(string value);
        partial void OnSubjectChanged();

        partial void OnBodyChanging(string value);
        partial void OnBodyChanged();

        partial void OnFromAddrChanging(string value);
        partial void OnFromAddrChanged();

        partial void OnSentChanging(DateTime? value);
        partial void OnSentChanged();

        partial void OnStartedChanging(DateTime? value);
        partial void OnStartedChanged();

        partial void OnQueuedChanging(DateTime value);
        partial void OnQueuedChanged();

        partial void OnFromNameChanging(string value);
        partial void OnFromNameChanged();

        partial void OnQueuedByChanging(int? value);
        partial void OnQueuedByChanged();

        partial void OnRedactedChanging(bool? value);
        partial void OnRedactedChanged();

        partial void OnTransactionalChanging(bool? value);
        partial void OnTransactionalChanged();

        partial void OnPublicXChanging(bool? value);
        partial void OnPublicXChanged();

        partial void OnErrorChanging(string value);
        partial void OnErrorChanged();

        partial void OnCCParentsChanging(bool? value);
        partial void OnCCParentsChanged();

        partial void OnNoReplacementsChanging(bool? value);
        partial void OnNoReplacementsChanged();

        partial void OnSendFromOrgIdChanging(int? value);
        partial void OnSendFromOrgIdChanged();

        partial void OnFinanceOnlyChanging(bool? value);
        partial void OnFinanceOnlyChanged();

        partial void OnCClistChanging(string value);
        partial void OnCClistChanged();

        partial void OnTestingChanging(bool? value);
        partial void OnTestingChanged();

        partial void OnReadyToSendChanging(bool? value);
        partial void OnReadyToSendChanged();

        #endregion

        public EmailQueue()
        {
            _EmailLinks = new EntitySet<EmailLink>(new Action<EmailLink>(attach_EmailLinks), new Action<EmailLink>(detach_EmailLinks));

            _EmailQueueTos = new EntitySet<EmailQueueTo>(new Action<EmailQueueTo>(attach_EmailQueueTos), new Action<EmailQueueTo>(detach_EmailQueueTos));

            _EmailResponses = new EntitySet<EmailResponse>(new Action<EmailResponse>(attach_EmailResponses), new Action<EmailResponse>(detach_EmailResponses));

            _Person = default(EntityRef<Person>);

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

        [Column(Name = "SendWhen", UpdateCheck = UpdateCheck.Never, Storage = "_SendWhen", DbType = "datetime")]
        public DateTime? SendWhen
        {
            get => _SendWhen;

            set
            {
                if (_SendWhen != value)
                {
                    OnSendWhenChanging(value);
                    SendPropertyChanging();
                    _SendWhen = value;
                    SendPropertyChanged("SendWhen");
                    OnSendWhenChanged();
                }
            }
        }

        [Column(Name = "Subject", UpdateCheck = UpdateCheck.Never, Storage = "_Subject", DbType = "nvarchar(200)")]
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

        [Column(Name = "Body", UpdateCheck = UpdateCheck.Never, Storage = "_Body", DbType = "nvarchar")]
        public string Body
        {
            get => _Body;

            set
            {
                if (_Body != value)
                {
                    OnBodyChanging(value);
                    SendPropertyChanging();
                    _Body = value;
                    SendPropertyChanged("Body");
                    OnBodyChanged();
                }
            }
        }

        [Column(Name = "FromAddr", UpdateCheck = UpdateCheck.Never, Storage = "_FromAddr", DbType = "nvarchar(100)")]
        public string FromAddr
        {
            get => _FromAddr;

            set
            {
                if (_FromAddr != value)
                {
                    OnFromAddrChanging(value);
                    SendPropertyChanging();
                    _FromAddr = value;
                    SendPropertyChanged("FromAddr");
                    OnFromAddrChanged();
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

        [Column(Name = "Started", UpdateCheck = UpdateCheck.Never, Storage = "_Started", DbType = "datetime")]
        public DateTime? Started
        {
            get => _Started;

            set
            {
                if (_Started != value)
                {
                    OnStartedChanging(value);
                    SendPropertyChanging();
                    _Started = value;
                    SendPropertyChanged("Started");
                    OnStartedChanged();
                }
            }
        }

        [Column(Name = "Queued", UpdateCheck = UpdateCheck.Never, Storage = "_Queued", DbType = "datetime NOT NULL")]
        public DateTime Queued
        {
            get => _Queued;

            set
            {
                if (_Queued != value)
                {
                    OnQueuedChanging(value);
                    SendPropertyChanging();
                    _Queued = value;
                    SendPropertyChanged("Queued");
                    OnQueuedChanged();
                }
            }
        }

        [Column(Name = "FromName", UpdateCheck = UpdateCheck.Never, Storage = "_FromName", DbType = "nvarchar(60)")]
        public string FromName
        {
            get => _FromName;

            set
            {
                if (_FromName != value)
                {
                    OnFromNameChanging(value);
                    SendPropertyChanging();
                    _FromName = value;
                    SendPropertyChanged("FromName");
                    OnFromNameChanged();
                }
            }
        }

        [Column(Name = "QueuedBy", UpdateCheck = UpdateCheck.Never, Storage = "_QueuedBy", DbType = "int")]
        [IsForeignKey]
        public int? QueuedBy
        {
            get => _QueuedBy;

            set
            {
                if (_QueuedBy != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnQueuedByChanging(value);
                    SendPropertyChanging();
                    _QueuedBy = value;
                    SendPropertyChanged("QueuedBy");
                    OnQueuedByChanged();
                }
            }
        }

        [Column(Name = "Redacted", UpdateCheck = UpdateCheck.Never, Storage = "_Redacted", DbType = "bit")]
        public bool? Redacted
        {
            get => _Redacted;

            set
            {
                if (_Redacted != value)
                {
                    OnRedactedChanging(value);
                    SendPropertyChanging();
                    _Redacted = value;
                    SendPropertyChanged("Redacted");
                    OnRedactedChanged();
                }
            }
        }

        [Column(Name = "Transactional", UpdateCheck = UpdateCheck.Never, Storage = "_Transactional", DbType = "bit")]
        public bool? Transactional
        {
            get => _Transactional;

            set
            {
                if (_Transactional != value)
                {
                    OnTransactionalChanging(value);
                    SendPropertyChanging();
                    _Transactional = value;
                    SendPropertyChanged("Transactional");
                    OnTransactionalChanged();
                }
            }
        }

        [Column(Name = "Public", UpdateCheck = UpdateCheck.Never, Storage = "_PublicX", DbType = "bit")]
        public bool? PublicX
        {
            get => _PublicX;

            set
            {
                if (_PublicX != value)
                {
                    OnPublicXChanging(value);
                    SendPropertyChanging();
                    _PublicX = value;
                    SendPropertyChanged("PublicX");
                    OnPublicXChanged();
                }
            }
        }

        [Column(Name = "Error", UpdateCheck = UpdateCheck.Never, Storage = "_Error", DbType = "nvarchar(200)")]
        public string Error
        {
            get => _Error;

            set
            {
                if (_Error != value)
                {
                    OnErrorChanging(value);
                    SendPropertyChanging();
                    _Error = value;
                    SendPropertyChanged("Error");
                    OnErrorChanged();
                }
            }
        }

        [Column(Name = "CCParents", UpdateCheck = UpdateCheck.Never, Storage = "_CCParents", DbType = "bit")]
        public bool? CCParents
        {
            get => _CCParents;

            set
            {
                if (_CCParents != value)
                {
                    OnCCParentsChanging(value);
                    SendPropertyChanging();
                    _CCParents = value;
                    SendPropertyChanged("CCParents");
                    OnCCParentsChanged();
                }
            }
        }

        [Column(Name = "NoReplacements", UpdateCheck = UpdateCheck.Never, Storage = "_NoReplacements", DbType = "bit")]
        public bool? NoReplacements
        {
            get => _NoReplacements;

            set
            {
                if (_NoReplacements != value)
                {
                    OnNoReplacementsChanging(value);
                    SendPropertyChanging();
                    _NoReplacements = value;
                    SendPropertyChanged("NoReplacements");
                    OnNoReplacementsChanged();
                }
            }
        }

        [Column(Name = "SendFromOrgId", UpdateCheck = UpdateCheck.Never, Storage = "_SendFromOrgId", DbType = "int")]
        public int? SendFromOrgId
        {
            get => _SendFromOrgId;

            set
            {
                if (_SendFromOrgId != value)
                {
                    OnSendFromOrgIdChanging(value);
                    SendPropertyChanging();
                    _SendFromOrgId = value;
                    SendPropertyChanged("SendFromOrgId");
                    OnSendFromOrgIdChanged();
                }
            }
        }

        [Column(Name = "FinanceOnly", UpdateCheck = UpdateCheck.Never, Storage = "_FinanceOnly", DbType = "bit")]
        public bool? FinanceOnly
        {
            get => _FinanceOnly;

            set
            {
                if (_FinanceOnly != value)
                {
                    OnFinanceOnlyChanging(value);
                    SendPropertyChanging();
                    _FinanceOnly = value;
                    SendPropertyChanged("FinanceOnly");
                    OnFinanceOnlyChanged();
                }
            }
        }

        [Column(Name = "CClist", UpdateCheck = UpdateCheck.Never, Storage = "_CClist", DbType = "nvarchar")]
        public string CClist
        {
            get => _CClist;

            set
            {
                if (_CClist != value)
                {
                    OnCClistChanging(value);
                    SendPropertyChanging();
                    _CClist = value;
                    SendPropertyChanged("CClist");
                    OnCClistChanged();
                }
            }
        }

        [Column(Name = "Testing", UpdateCheck = UpdateCheck.Never, Storage = "_Testing", DbType = "bit")]
        public bool? Testing
        {
            get => _Testing;

            set
            {
                if (_Testing != value)
                {
                    OnTestingChanging(value);
                    SendPropertyChanging();
                    _Testing = value;
                    SendPropertyChanged("Testing");
                    OnTestingChanged();
                }
            }
        }

        [Column(Name = "ReadyToSend", UpdateCheck = UpdateCheck.Never, Storage = "_ReadyToSend", DbType = "bit")]
        public bool? ReadyToSend
        {
            get => _ReadyToSend;

            set
            {
                if (_ReadyToSend != value)
                {
                    OnReadyToSendChanging(value);
                    SendPropertyChanging();
                    _ReadyToSend = value;
                    SendPropertyChanged("ReadyToSend");
                    OnReadyToSendChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_EmailLinks_EmailQueue", Storage = "_EmailLinks", OtherKey = "EmailID")]
        public EntitySet<EmailLink> EmailLinks
           {
               get => _EmailLinks;

            set => _EmailLinks.Assign(value);

           }

        [Association(Name = "FK_EmailQueueTo_EmailQueue", Storage = "_EmailQueueTos", OtherKey = "Id")]
        public EntitySet<EmailQueueTo> EmailQueueTos
           {
               get => _EmailQueueTos;

            set => _EmailQueueTos.Assign(value);

           }

        [Association(Name = "FK_EmailResponses_EmailQueue", Storage = "_EmailResponses", OtherKey = "EmailQueueId")]
        public EntitySet<EmailResponse> EmailResponses
           {
               get => _EmailResponses;

            set => _EmailResponses.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_EmailQueue_People", Storage = "_Person", ThisKey = "QueuedBy", IsForeignKey = true)]
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
                        previousValue.EmailQueues.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.EmailQueues.Add(this);

                        _QueuedBy = value.PeopleId;

                    }

                    else
                    {
                        _QueuedBy = default(int?);

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

        private void attach_EmailLinks(EmailLink entity)
        {
            SendPropertyChanging();
            entity.EmailQueue = this;
        }

        private void detach_EmailLinks(EmailLink entity)
        {
            SendPropertyChanging();
            entity.EmailQueue = null;
        }

        private void attach_EmailQueueTos(EmailQueueTo entity)
        {
            SendPropertyChanging();
            entity.EmailQueue = this;
        }

        private void detach_EmailQueueTos(EmailQueueTo entity)
        {
            SendPropertyChanging();
            entity.EmailQueue = null;
        }

        private void attach_EmailResponses(EmailResponse entity)
        {
            SendPropertyChanging();
            entity.EmailQueue = this;
        }

        private void detach_EmailResponses(EmailResponse entity)
        {
            SendPropertyChanging();
            entity.EmailQueue = null;
        }
    }
}
