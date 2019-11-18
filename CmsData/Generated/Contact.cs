using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Contact")]
    public partial class Contact : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ContactId;

        private int _CreatedBy;

        private DateTime _CreatedDate;

        private int? _ContactTypeId;

        private DateTime _ContactDate;

        private int? _ContactReasonId;

        private int? _MinistryId;

        private bool? _NotAtHome;

        private bool? _LeftDoorHanger;

        private bool? _LeftMessage;

        private bool? _GospelShared;

        private bool? _PrayerRequest;

        private bool? _ContactMade;

        private bool? _GiftBagGiven;

        private string _Comments;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private string _LimitToRole;

        private int? _OrganizationId;

        private EntitySet<Contactee> _contactees;

        private EntitySet<Contactor> _contactsMakers;

        private EntitySet<ContactExtra> _ContactExtras;

        private EntitySet<Task> _TasksAssigned;

        private EntitySet<Task> _TasksCompleted;

        private EntityRef<Organization> _organization;

        private EntityRef<ContactType> _ContactType;

        private EntityRef<Ministry> _Ministry;

        private EntityRef<ContactReason> _ContactReason;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnContactIdChanging(int value);
        partial void OnContactIdChanged();

        partial void OnCreatedByChanging(int value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        partial void OnContactTypeIdChanging(int? value);
        partial void OnContactTypeIdChanged();

        partial void OnContactDateChanging(DateTime value);
        partial void OnContactDateChanged();

        partial void OnContactReasonIdChanging(int? value);
        partial void OnContactReasonIdChanged();

        partial void OnMinistryIdChanging(int? value);
        partial void OnMinistryIdChanged();

        partial void OnNotAtHomeChanging(bool? value);
        partial void OnNotAtHomeChanged();

        partial void OnLeftDoorHangerChanging(bool? value);
        partial void OnLeftDoorHangerChanged();

        partial void OnLeftMessageChanging(bool? value);
        partial void OnLeftMessageChanged();

        partial void OnGospelSharedChanging(bool? value);
        partial void OnGospelSharedChanged();

        partial void OnPrayerRequestChanging(bool? value);
        partial void OnPrayerRequestChanged();

        partial void OnContactMadeChanging(bool? value);
        partial void OnContactMadeChanged();

        partial void OnGiftBagGivenChanging(bool? value);
        partial void OnGiftBagGivenChanged();

        partial void OnCommentsChanging(string value);
        partial void OnCommentsChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        partial void OnLimitToRoleChanging(string value);
        partial void OnLimitToRoleChanged();

        partial void OnOrganizationIdChanging(int? value);
        partial void OnOrganizationIdChanged();

        #endregion

        public Contact()
        {
            _contactees = new EntitySet<Contactee>(new Action<Contactee>(attach_contactees), new Action<Contactee>(detach_contactees));

            _contactsMakers = new EntitySet<Contactor>(new Action<Contactor>(attach_contactsMakers), new Action<Contactor>(detach_contactsMakers));

            _ContactExtras = new EntitySet<ContactExtra>(new Action<ContactExtra>(attach_ContactExtras), new Action<ContactExtra>(detach_ContactExtras));

            _TasksAssigned = new EntitySet<Task>(new Action<Task>(attach_TasksAssigned), new Action<Task>(detach_TasksAssigned));

            _TasksCompleted = new EntitySet<Task>(new Action<Task>(attach_TasksCompleted), new Action<Task>(detach_TasksCompleted));

            _organization = default(EntityRef<Organization>);

            _ContactType = default(EntityRef<ContactType>);

            _Ministry = default(EntityRef<Ministry>);

            _ContactReason = default(EntityRef<ContactReason>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ContactId", UpdateCheck = UpdateCheck.Never, Storage = "_ContactId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ContactId
        {
            get => _ContactId;

            set
            {
                if (_ContactId != value)
                {
                    OnContactIdChanging(value);
                    SendPropertyChanging();
                    _ContactId = value;
                    SendPropertyChanged("ContactId");
                    OnContactIdChanged();
                }
            }
        }

        [Column(Name = "CreatedBy", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int NOT NULL")]
        public int CreatedBy
        {
            get => _CreatedBy;

            set
            {
                if (_CreatedBy != value)
                {
                    OnCreatedByChanging(value);
                    SendPropertyChanging();
                    _CreatedBy = value;
                    SendPropertyChanged("CreatedBy");
                    OnCreatedByChanged();
                }
            }
        }

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime NOT NULL")]
        public DateTime CreatedDate
        {
            get => _CreatedDate;

            set
            {
                if (_CreatedDate != value)
                {
                    OnCreatedDateChanging(value);
                    SendPropertyChanging();
                    _CreatedDate = value;
                    SendPropertyChanged("CreatedDate");
                    OnCreatedDateChanged();
                }
            }
        }

        [Column(Name = "ContactTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_ContactTypeId", DbType = "int")]
        [IsForeignKey]
        public int? ContactTypeId
        {
            get => _ContactTypeId;

            set
            {
                if (_ContactTypeId != value)
                {
                    if (_ContactType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnContactTypeIdChanging(value);
                    SendPropertyChanging();
                    _ContactTypeId = value;
                    SendPropertyChanged("ContactTypeId");
                    OnContactTypeIdChanged();
                }
            }
        }

        [Column(Name = "ContactDate", UpdateCheck = UpdateCheck.Never, Storage = "_ContactDate", DbType = "datetime NOT NULL")]
        public DateTime ContactDate
        {
            get => _ContactDate;

            set
            {
                if (_ContactDate != value)
                {
                    OnContactDateChanging(value);
                    SendPropertyChanging();
                    _ContactDate = value;
                    SendPropertyChanged("ContactDate");
                    OnContactDateChanged();
                }
            }
        }

        [Column(Name = "ContactReasonId", UpdateCheck = UpdateCheck.Never, Storage = "_ContactReasonId", DbType = "int")]
        [IsForeignKey]
        public int? ContactReasonId
        {
            get => _ContactReasonId;

            set
            {
                if (_ContactReasonId != value)
                {
                    if (_ContactReason.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnContactReasonIdChanging(value);
                    SendPropertyChanging();
                    _ContactReasonId = value;
                    SendPropertyChanged("ContactReasonId");
                    OnContactReasonIdChanged();
                }
            }
        }

        [Column(Name = "MinistryId", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryId", DbType = "int")]
        [IsForeignKey]
        public int? MinistryId
        {
            get => _MinistryId;

            set
            {
                if (_MinistryId != value)
                {
                    if (_Ministry.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnMinistryIdChanging(value);
                    SendPropertyChanging();
                    _MinistryId = value;
                    SendPropertyChanged("MinistryId");
                    OnMinistryIdChanged();
                }
            }
        }

        [Column(Name = "NotAtHome", UpdateCheck = UpdateCheck.Never, Storage = "_NotAtHome", DbType = "bit")]
        public bool? NotAtHome
        {
            get => _NotAtHome;

            set
            {
                if (_NotAtHome != value)
                {
                    OnNotAtHomeChanging(value);
                    SendPropertyChanging();
                    _NotAtHome = value;
                    SendPropertyChanged("NotAtHome");
                    OnNotAtHomeChanged();
                }
            }
        }

        [Column(Name = "LeftDoorHanger", UpdateCheck = UpdateCheck.Never, Storage = "_LeftDoorHanger", DbType = "bit")]
        public bool? LeftDoorHanger
        {
            get => _LeftDoorHanger;

            set
            {
                if (_LeftDoorHanger != value)
                {
                    OnLeftDoorHangerChanging(value);
                    SendPropertyChanging();
                    _LeftDoorHanger = value;
                    SendPropertyChanged("LeftDoorHanger");
                    OnLeftDoorHangerChanged();
                }
            }
        }

        [Column(Name = "LeftMessage", UpdateCheck = UpdateCheck.Never, Storage = "_LeftMessage", DbType = "bit")]
        public bool? LeftMessage
        {
            get => _LeftMessage;

            set
            {
                if (_LeftMessage != value)
                {
                    OnLeftMessageChanging(value);
                    SendPropertyChanging();
                    _LeftMessage = value;
                    SendPropertyChanged("LeftMessage");
                    OnLeftMessageChanged();
                }
            }
        }

        [Column(Name = "GospelShared", UpdateCheck = UpdateCheck.Never, Storage = "_GospelShared", DbType = "bit")]
        public bool? GospelShared
        {
            get => _GospelShared;

            set
            {
                if (_GospelShared != value)
                {
                    OnGospelSharedChanging(value);
                    SendPropertyChanging();
                    _GospelShared = value;
                    SendPropertyChanged("GospelShared");
                    OnGospelSharedChanged();
                }
            }
        }

        [Column(Name = "PrayerRequest", UpdateCheck = UpdateCheck.Never, Storage = "_PrayerRequest", DbType = "bit")]
        public bool? PrayerRequest
        {
            get => _PrayerRequest;

            set
            {
                if (_PrayerRequest != value)
                {
                    OnPrayerRequestChanging(value);
                    SendPropertyChanging();
                    _PrayerRequest = value;
                    SendPropertyChanged("PrayerRequest");
                    OnPrayerRequestChanged();
                }
            }
        }

        [Column(Name = "ContactMade", UpdateCheck = UpdateCheck.Never, Storage = "_ContactMade", DbType = "bit")]
        public bool? ContactMade
        {
            get => _ContactMade;

            set
            {
                if (_ContactMade != value)
                {
                    OnContactMadeChanging(value);
                    SendPropertyChanging();
                    _ContactMade = value;
                    SendPropertyChanged("ContactMade");
                    OnContactMadeChanged();
                }
            }
        }

        [Column(Name = "GiftBagGiven", UpdateCheck = UpdateCheck.Never, Storage = "_GiftBagGiven", DbType = "bit")]
        public bool? GiftBagGiven
        {
            get => _GiftBagGiven;

            set
            {
                if (_GiftBagGiven != value)
                {
                    OnGiftBagGivenChanging(value);
                    SendPropertyChanging();
                    _GiftBagGiven = value;
                    SendPropertyChanged("GiftBagGiven");
                    OnGiftBagGivenChanged();
                }
            }
        }

        [Column(Name = "Comments", UpdateCheck = UpdateCheck.Never, Storage = "_Comments", DbType = "nvarchar")]
        public string Comments
        {
            get => _Comments;

            set
            {
                if (_Comments != value)
                {
                    OnCommentsChanging(value);
                    SendPropertyChanging();
                    _Comments = value;
                    SendPropertyChanged("Comments");
                    OnCommentsChanged();
                }
            }
        }

        [Column(Name = "ModifiedBy", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedBy", DbType = "int")]
        public int? ModifiedBy
        {
            get => _ModifiedBy;

            set
            {
                if (_ModifiedBy != value)
                {
                    OnModifiedByChanging(value);
                    SendPropertyChanging();
                    _ModifiedBy = value;
                    SendPropertyChanged("ModifiedBy");
                    OnModifiedByChanged();
                }
            }
        }

        [Column(Name = "ModifiedDate", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedDate", DbType = "datetime")]
        public DateTime? ModifiedDate
        {
            get => _ModifiedDate;

            set
            {
                if (_ModifiedDate != value)
                {
                    OnModifiedDateChanging(value);
                    SendPropertyChanging();
                    _ModifiedDate = value;
                    SendPropertyChanged("ModifiedDate");
                    OnModifiedDateChanged();
                }
            }
        }

        [Column(Name = "LimitToRole", UpdateCheck = UpdateCheck.Never, Storage = "_LimitToRole", DbType = "nvarchar(50)")]
        public string LimitToRole
        {
            get => _LimitToRole;

            set
            {
                if (_LimitToRole != value)
                {
                    OnLimitToRoleChanging(value);
                    SendPropertyChanging();
                    _LimitToRole = value;
                    SendPropertyChanged("LimitToRole");
                    OnLimitToRoleChanged();
                }
            }
        }

        [Column(Name = "OrganizationId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationId", DbType = "int")]
        [IsForeignKey]
        public int? OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    if (_organization.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOrganizationIdChanging(value);
                    SendPropertyChanging();
                    _OrganizationId = value;
                    SendPropertyChanged("OrganizationId");
                    OnOrganizationIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "contactees__contact", Storage = "_contactees", OtherKey = "ContactId")]
        public EntitySet<Contactee> contactees
           {
               get => _contactees;

            set => _contactees.Assign(value);

           }

        [Association(Name = "contactsMakers__contact", Storage = "_contactsMakers", OtherKey = "ContactId")]
        public EntitySet<Contactor> contactsMakers
           {
               get => _contactsMakers;

            set => _contactsMakers.Assign(value);

           }

        [Association(Name = "FK_ContactExtra_Contact", Storage = "_ContactExtras", OtherKey = "ContactId")]
        public EntitySet<ContactExtra> ContactExtras
           {
               get => _ContactExtras;

            set => _ContactExtras.Assign(value);

           }

        [Association(Name = "TasksAssigned__SourceContact", Storage = "_TasksAssigned", OtherKey = "SourceContactId")]
        public EntitySet<Task> TasksAssigned
           {
               get => _TasksAssigned;

            set => _TasksAssigned.Assign(value);

           }

        [Association(Name = "TasksCompleted__CompletedContact", Storage = "_TasksCompleted", OtherKey = "CompletedContactId")]
        public EntitySet<Task> TasksCompleted
           {
               get => _TasksCompleted;

            set => _TasksCompleted.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "contactsHad__organization", Storage = "_organization", ThisKey = "OrganizationId", IsForeignKey = true)]
        public Organization organization
        {
            get => _organization.Entity;

            set
            {
                Organization previousValue = _organization.Entity;
                if (((previousValue != value)
                            || (_organization.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _organization.Entity = null;
                        previousValue.contactsHad.Remove(this);
                    }

                    _organization.Entity = value;
                    if (value != null)
                    {
                        value.contactsHad.Add(this);

                        _OrganizationId = value.OrganizationId;

                    }

                    else
                    {
                        _OrganizationId = default(int?);

                    }

                    SendPropertyChanged("organization");
                }
            }
        }

        [Association(Name = "FK_Contacts_ContactTypes", Storage = "_ContactType", ThisKey = "ContactTypeId", IsForeignKey = true)]
        public ContactType ContactType
        {
            get => _ContactType.Entity;

            set
            {
                ContactType previousValue = _ContactType.Entity;
                if (((previousValue != value)
                            || (_ContactType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ContactType.Entity = null;
                        previousValue.Contacts.Remove(this);
                    }

                    _ContactType.Entity = value;
                    if (value != null)
                    {
                        value.Contacts.Add(this);

                        _ContactTypeId = value.Id;

                    }

                    else
                    {
                        _ContactTypeId = default(int?);

                    }

                    SendPropertyChanged("ContactType");
                }
            }
        }

        [Association(Name = "FK_Contacts_Ministries", Storage = "_Ministry", ThisKey = "MinistryId", IsForeignKey = true)]
        public Ministry Ministry
        {
            get => _Ministry.Entity;

            set
            {
                Ministry previousValue = _Ministry.Entity;
                if (((previousValue != value)
                            || (_Ministry.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Ministry.Entity = null;
                        previousValue.Contacts.Remove(this);
                    }

                    _Ministry.Entity = value;
                    if (value != null)
                    {
                        value.Contacts.Add(this);

                        _MinistryId = value.MinistryId;

                    }

                    else
                    {
                        _MinistryId = default(int?);

                    }

                    SendPropertyChanged("Ministry");
                }
            }
        }

        [Association(Name = "FK_NewContacts_ContactReasons", Storage = "_ContactReason", ThisKey = "ContactReasonId", IsForeignKey = true)]
        public ContactReason ContactReason
        {
            get => _ContactReason.Entity;

            set
            {
                ContactReason previousValue = _ContactReason.Entity;
                if (((previousValue != value)
                            || (_ContactReason.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ContactReason.Entity = null;
                        previousValue.Contacts.Remove(this);
                    }

                    _ContactReason.Entity = value;
                    if (value != null)
                    {
                        value.Contacts.Add(this);

                        _ContactReasonId = value.Id;

                    }

                    else
                    {
                        _ContactReasonId = default(int?);

                    }

                    SendPropertyChanged("ContactReason");
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

        private void attach_contactees(Contactee entity)
        {
            SendPropertyChanging();
            entity.contact = this;
        }

        private void detach_contactees(Contactee entity)
        {
            SendPropertyChanging();
            entity.contact = null;
        }

        private void attach_contactsMakers(Contactor entity)
        {
            SendPropertyChanging();
            entity.contact = this;
        }

        private void detach_contactsMakers(Contactor entity)
        {
            SendPropertyChanging();
            entity.contact = null;
        }

        private void attach_ContactExtras(ContactExtra entity)
        {
            SendPropertyChanging();
            entity.Contact = this;
        }

        private void detach_ContactExtras(ContactExtra entity)
        {
            SendPropertyChanging();
            entity.Contact = null;
        }

        private void attach_TasksAssigned(Task entity)
        {
            SendPropertyChanging();
            entity.SourceContact = this;
        }

        private void detach_TasksAssigned(Task entity)
        {
            SendPropertyChanging();
            entity.SourceContact = null;
        }

        private void attach_TasksCompleted(Task entity)
        {
            SendPropertyChanging();
            entity.CompletedContact = this;
        }

        private void detach_TasksCompleted(Task entity)
        {
            SendPropertyChanging();
            entity.CompletedContact = null;
        }
    }
}
