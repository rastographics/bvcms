using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Coupons")]
    public partial class Coupon : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _Id;

        private DateTime _Created;

        private DateTime? _Used;

        private DateTime? _Canceled;

        private decimal? _Amount;

        private int? _DivId;

        private int? _OrgId;

        private int? _PeopleId;

        private string _Name;

        private int? _UserId;

        private decimal? _RegAmount;

        private string _DivOrg;

        private bool? _MultiUse;

        private bool? _Generated;

        private EntityRef<Division> _Division;

        private EntityRef<Organization> _Organization;

        private EntityRef<Person> _Person;

        private EntityRef<User> _User;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(string value);
        partial void OnIdChanged();

        partial void OnCreatedChanging(DateTime value);
        partial void OnCreatedChanged();

        partial void OnUsedChanging(DateTime? value);
        partial void OnUsedChanged();

        partial void OnCanceledChanging(DateTime? value);
        partial void OnCanceledChanged();

        partial void OnAmountChanging(decimal? value);
        partial void OnAmountChanged();

        partial void OnDivIdChanging(int? value);
        partial void OnDivIdChanged();

        partial void OnOrgIdChanging(int? value);
        partial void OnOrgIdChanged();

        partial void OnPeopleIdChanging(int? value);
        partial void OnPeopleIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnUserIdChanging(int? value);
        partial void OnUserIdChanged();

        partial void OnRegAmountChanging(decimal? value);
        partial void OnRegAmountChanged();

        partial void OnDivOrgChanging(string value);
        partial void OnDivOrgChanged();

        partial void OnMultiUseChanging(bool? value);
        partial void OnMultiUseChanged();

        partial void OnGeneratedChanging(bool? value);
        partial void OnGeneratedChanged();

        #endregion

        public Coupon()
        {
            _Division = default(EntityRef<Division>);

            _Organization = default(EntityRef<Organization>);

            _Person = default(EntityRef<Person>);

            _User = default(EntityRef<User>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "nvarchar(50) NOT NULL", IsPrimaryKey = true)]
        public string Id
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

        [Column(Name = "Created", UpdateCheck = UpdateCheck.Never, Storage = "_Created", DbType = "datetime NOT NULL")]
        public DateTime Created
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

        [Column(Name = "Used", UpdateCheck = UpdateCheck.Never, Storage = "_Used", DbType = "datetime")]
        public DateTime? Used
        {
            get => _Used;

            set
            {
                if (_Used != value)
                {
                    OnUsedChanging(value);
                    SendPropertyChanging();
                    _Used = value;
                    SendPropertyChanged("Used");
                    OnUsedChanged();
                }
            }
        }

        [Column(Name = "Canceled", UpdateCheck = UpdateCheck.Never, Storage = "_Canceled", DbType = "datetime")]
        public DateTime? Canceled
        {
            get => _Canceled;

            set
            {
                if (_Canceled != value)
                {
                    OnCanceledChanging(value);
                    SendPropertyChanging();
                    _Canceled = value;
                    SendPropertyChanged("Canceled");
                    OnCanceledChanged();
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

        [Column(Name = "DivId", UpdateCheck = UpdateCheck.Never, Storage = "_DivId", DbType = "int")]
        [IsForeignKey]
        public int? DivId
        {
            get => _DivId;

            set
            {
                if (_DivId != value)
                {
                    if (_Division.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnDivIdChanging(value);
                    SendPropertyChanging();
                    _DivId = value;
                    SendPropertyChanged("DivId");
                    OnDivIdChanged();
                }
            }
        }

        [Column(Name = "OrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OrgId", DbType = "int")]
        [IsForeignKey]
        public int? OrgId
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

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        [IsForeignKey]
        public int? PeopleId
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

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(80)")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    OnNameChanging(value);
                    SendPropertyChanging();
                    _Name = value;
                    SendPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }

        [Column(Name = "UserId", UpdateCheck = UpdateCheck.Never, Storage = "_UserId", DbType = "int")]
        [IsForeignKey]
        public int? UserId
        {
            get => _UserId;

            set
            {
                if (_UserId != value)
                {
                    if (_User.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnUserIdChanging(value);
                    SendPropertyChanging();
                    _UserId = value;
                    SendPropertyChanged("UserId");
                    OnUserIdChanged();
                }
            }
        }

        [Column(Name = "RegAmount", UpdateCheck = UpdateCheck.Never, Storage = "_RegAmount", DbType = "money")]
        public decimal? RegAmount
        {
            get => _RegAmount;

            set
            {
                if (_RegAmount != value)
                {
                    OnRegAmountChanging(value);
                    SendPropertyChanging();
                    _RegAmount = value;
                    SendPropertyChanged("RegAmount");
                    OnRegAmountChanged();
                }
            }
        }

        [Column(Name = "DivOrg", UpdateCheck = UpdateCheck.Never, Storage = "_DivOrg", DbType = "nvarchar(34)", IsDbGenerated = true)]
        public string DivOrg
        {
            get => _DivOrg;

            set
            {
                if (_DivOrg != value)
                {
                    OnDivOrgChanging(value);
                    SendPropertyChanging();
                    _DivOrg = value;
                    SendPropertyChanged("DivOrg");
                    OnDivOrgChanged();
                }
            }
        }

        [Column(Name = "MultiUse", UpdateCheck = UpdateCheck.Never, Storage = "_MultiUse", DbType = "bit")]
        public bool? MultiUse
        {
            get => _MultiUse;

            set
            {
                if (_MultiUse != value)
                {
                    OnMultiUseChanging(value);
                    SendPropertyChanging();
                    _MultiUse = value;
                    SendPropertyChanged("MultiUse");
                    OnMultiUseChanged();
                }
            }
        }

        [Column(Name = "Generated", UpdateCheck = UpdateCheck.Never, Storage = "_Generated", DbType = "bit")]
        public bool? Generated
        {
            get => _Generated;

            set
            {
                if (_Generated != value)
                {
                    OnGeneratedChanging(value);
                    SendPropertyChanging();
                    _Generated = value;
                    SendPropertyChanged("Generated");
                    OnGeneratedChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Coupons_Division", Storage = "_Division", ThisKey = "DivId", IsForeignKey = true)]
        public Division Division
        {
            get => _Division.Entity;

            set
            {
                Division previousValue = _Division.Entity;
                if (((previousValue != value)
                            || (_Division.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Division.Entity = null;
                        previousValue.Coupons.Remove(this);
                    }

                    _Division.Entity = value;
                    if (value != null)
                    {
                        value.Coupons.Add(this);

                        _DivId = value.Id;

                    }

                    else
                    {
                        _DivId = default(int?);

                    }

                    SendPropertyChanged("Division");
                }
            }
        }

        [Association(Name = "FK_Coupons_Organizations", Storage = "_Organization", ThisKey = "OrgId", IsForeignKey = true)]
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
                        previousValue.Coupons.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.Coupons.Add(this);

                        _OrgId = value.OrganizationId;

                    }

                    else
                    {
                        _OrgId = default(int?);

                    }

                    SendPropertyChanged("Organization");
                }
            }
        }

        [Association(Name = "FK_Coupons_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.Coupons.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.Coupons.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int?);

                    }

                    SendPropertyChanged("Person");
                }
            }
        }

        [Association(Name = "FK_Coupons_Users", Storage = "_User", ThisKey = "UserId", IsForeignKey = true)]
        public User User
        {
            get => _User.Entity;

            set
            {
                User previousValue = _User.Entity;
                if (((previousValue != value)
                            || (_User.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _User.Entity = null;
                        previousValue.Coupons.Remove(this);
                    }

                    _User.Entity = value;
                    if (value != null)
                    {
                        value.Coupons.Add(this);

                        _UserId = value.UserId;

                    }

                    else
                    {
                        _UserId = default(int?);

                    }

                    SendPropertyChanged("User");
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
