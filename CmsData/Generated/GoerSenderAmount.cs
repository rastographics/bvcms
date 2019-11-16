using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.GoerSenderAmounts")]
    public partial class GoerSenderAmount : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _OrgId;

        private int _SupporterId;

        private int? _GoerId;

        private decimal? _Amount;

        private DateTime? _Created;

        private bool? _InActive;

        private bool? _NoNoticeToGoer;

        private EntityRef<Organization> _Organization;

        private EntityRef<Person> _Goer;

        private EntityRef<Person> _Sender;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnOrgIdChanging(int value);
        partial void OnOrgIdChanged();

        partial void OnSupporterIdChanging(int value);
        partial void OnSupporterIdChanged();

        partial void OnGoerIdChanging(int? value);
        partial void OnGoerIdChanged();

        partial void OnAmountChanging(decimal? value);
        partial void OnAmountChanged();

        partial void OnCreatedChanging(DateTime? value);
        partial void OnCreatedChanged();

        partial void OnInActiveChanging(bool? value);
        partial void OnInActiveChanged();

        partial void OnNoNoticeToGoerChanging(bool? value);
        partial void OnNoNoticeToGoerChanged();

        #endregion

        public GoerSenderAmount()
        {
            _Organization = default(EntityRef<Organization>);

            _Goer = default(EntityRef<Person>);

            _Sender = default(EntityRef<Person>);

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

        [Column(Name = "OrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OrgId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int OrgId
        {
            get => _OrgId;

            set
            {
                if (_OrgId != value)
                {
                    if (_Organization.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOrgIdChanging(value);
                    SendPropertyChanging();
                    _OrgId = value;
                    SendPropertyChanged("OrgId");
                    OnOrgIdChanged();
                }
            }
        }

        [Column(Name = "SupporterId", UpdateCheck = UpdateCheck.Never, Storage = "_SupporterId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int SupporterId
        {
            get => _SupporterId;

            set
            {
                if (_SupporterId != value)
                {
                    if (_Goer.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnSupporterIdChanging(value);
                    SendPropertyChanging();
                    _SupporterId = value;
                    SendPropertyChanged("SupporterId");
                    OnSupporterIdChanged();
                }
            }
        }

        [Column(Name = "GoerId", UpdateCheck = UpdateCheck.Never, Storage = "_GoerId", DbType = "int")]
        [IsForeignKey]
        public int? GoerId
        {
            get => _GoerId;

            set
            {
                if (_GoerId != value)
                {
                    if (_Sender.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnGoerIdChanging(value);
                    SendPropertyChanging();
                    _GoerId = value;
                    SendPropertyChanged("GoerId");
                    OnGoerIdChanged();
                }
            }
        }

        [Column(Name = "Amount", UpdateCheck = UpdateCheck.Never, Storage = "_Amount", DbType = "money")]
        public decimal? Amount
        {
            get => _Amount;

            set
            {
                if (_Amount != value)
                {
                    OnAmountChanging(value);
                    SendPropertyChanging();
                    _Amount = value;
                    SendPropertyChanged("Amount");
                    OnAmountChanged();
                }
            }
        }

        [Column(Name = "Created", UpdateCheck = UpdateCheck.Never, Storage = "_Created", DbType = "datetime")]
        public DateTime? Created
        {
            get => _Created;

            set
            {
                if (_Created != value)
                {
                    OnCreatedChanging(value);
                    SendPropertyChanging();
                    _Created = value;
                    SendPropertyChanged("Created");
                    OnCreatedChanged();
                }
            }
        }

        [Column(Name = "InActive", UpdateCheck = UpdateCheck.Never, Storage = "_InActive", DbType = "bit")]
        public bool? InActive
        {
            get => _InActive;

            set
            {
                if (_InActive != value)
                {
                    OnInActiveChanging(value);
                    SendPropertyChanging();
                    _InActive = value;
                    SendPropertyChanged("InActive");
                    OnInActiveChanged();
                }
            }
        }

        [Column(Name = "NoNoticeToGoer", UpdateCheck = UpdateCheck.Never, Storage = "_NoNoticeToGoer", DbType = "bit")]
        public bool? NoNoticeToGoer
        {
            get => _NoNoticeToGoer;

            set
            {
                if (_NoNoticeToGoer != value)
                {
                    OnNoNoticeToGoerChanging(value);
                    SendPropertyChanging();
                    _NoNoticeToGoer = value;
                    SendPropertyChanged("NoNoticeToGoer");
                    OnNoNoticeToGoerChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_GoerSenderAmounts_Organizations", Storage = "_Organization", ThisKey = "OrgId", IsForeignKey = true)]
        public Organization Organization
        {
            get => _Organization.Entity;

            set
            {
                Organization previousValue = _Organization.Entity;
                if (((previousValue != value)
                            || (_Organization.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Organization.Entity = null;
                        previousValue.GoerSenderAmounts.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.GoerSenderAmounts.Add(this);

                        _OrgId = value.OrganizationId;

                    }

                    else
                    {
                        _OrgId = default(int);

                    }

                    SendPropertyChanged("Organization");
                }
            }
        }

        [Association(Name = "GoerAmounts__Goer", Storage = "_Goer", ThisKey = "GoerId", IsForeignKey = true)]
        public Person Goer
        {
            get => _Goer.Entity;

            set
            {
                Person previousValue = _Goer.Entity;
                if (((previousValue != value)
                            || (_Goer.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Goer.Entity = null;
                        previousValue.GoerAmounts.Remove(this);
                    }

                    _Goer.Entity = value;
                    if (value != null)
                    {
                        value.GoerAmounts.Add(this);

                        _GoerId = value.PeopleId;

                    }

                    else
                    {
                        _GoerId = default(int);

                    }

                    SendPropertyChanged("Goer");
                }
            }
        }

        [Association(Name = "SenderAmounts__Sender", Storage = "_Sender", ThisKey = "SupporterId", IsForeignKey = true)]
        public Person Sender
        {
            get => _Sender.Entity;

            set
            {
                Person previousValue = _Sender.Entity;
                if (((previousValue != value)
                            || (_Sender.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Sender.Entity = null;
                        previousValue.SenderAmounts.Remove(this);
                    }

                    _Sender.Entity = value;
                    if (value != null)
                    {
                        value.SenderAmounts.Add(this);

                        _SupporterId = value.PeopleId;

                    }

                    else
                    {
                        _SupporterId = default(int);

                    }

                    SendPropertyChanged("Sender");
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
