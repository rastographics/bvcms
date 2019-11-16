using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.OrgMemMemTags")]
    public partial class OrgMemMemTag : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _OrgId;

        private int _PeopleId;

        private int _MemberTagId;

        private int? _Number;

        private bool? _IsLeader;

        private EntityRef<MemberTag> _MemberTag;

        private EntityRef<OrganizationMember> _OrganizationMember;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnOrgIdChanging(int value);
        partial void OnOrgIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnMemberTagIdChanging(int value);
        partial void OnMemberTagIdChanged();

        partial void OnNumberChanging(int? value);
        partial void OnNumberChanged();

        partial void OnIsLeaderChanging(bool? value);
        partial void OnIsLeaderChanged();

        #endregion

        public OrgMemMemTag()
        {
            _MemberTag = default(EntityRef<MemberTag>);

            _OrganizationMember = default(EntityRef<OrganizationMember>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "OrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OrgId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int OrgId
        {
            get => _OrgId;

            set
            {
                if (_OrgId != value)
                {
                    if (_OrganizationMember.HasLoadedOrAssignedValue)
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

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    if (_OrganizationMember.HasLoadedOrAssignedValue)
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

        [Column(Name = "MemberTagId", UpdateCheck = UpdateCheck.Never, Storage = "_MemberTagId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int MemberTagId
        {
            get => _MemberTagId;

            set
            {
                if (_MemberTagId != value)
                {
                    if (_MemberTag.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnMemberTagIdChanging(value);
                    SendPropertyChanging();
                    _MemberTagId = value;
                    SendPropertyChanged("MemberTagId");
                    OnMemberTagIdChanged();
                }
            }
        }

        [Column(Name = "Number", UpdateCheck = UpdateCheck.Never, Storage = "_Number", DbType = "int")]
        public int? Number
        {
            get => _Number;

            set
            {
                if (_Number != value)
                {
                    OnNumberChanging(value);
                    SendPropertyChanging();
                    _Number = value;
                    SendPropertyChanged("Number");
                    OnNumberChanged();
                }
            }
        }

        [Column(Name = "IsLeader", UpdateCheck = UpdateCheck.Never, Storage = "_IsLeader", DbType = "bit")]
        public bool? IsLeader
        {
            get => _IsLeader;

            set
            {
                if (_IsLeader != value)
                {
                    OnIsLeaderChanging(value);
                    SendPropertyChanging();
                    _IsLeader = value;
                    SendPropertyChanged("IsLeader");
                    OnIsLeaderChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_OrgMemMemTags_MemberTags", Storage = "_MemberTag", ThisKey = "MemberTagId", IsForeignKey = true)]
        public MemberTag MemberTag
        {
            get => _MemberTag.Entity;

            set
            {
                MemberTag previousValue = _MemberTag.Entity;
                if (((previousValue != value)
                            || (_MemberTag.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _MemberTag.Entity = null;
                        previousValue.OrgMemMemTags.Remove(this);
                    }

                    _MemberTag.Entity = value;
                    if (value != null)
                    {
                        value.OrgMemMemTags.Add(this);

                        _MemberTagId = value.Id;

                    }

                    else
                    {
                        _MemberTagId = default(int);

                    }

                    SendPropertyChanged("MemberTag");
                }
            }
        }

        [Association(Name = "FK_OrgMemMemTags_OrganizationMembers", Storage = "_OrganizationMember", ThisKey = "OrgId,PeopleId", IsForeignKey = true)]
        public OrganizationMember OrganizationMember
        {
            get => _OrganizationMember.Entity;

            set
            {
                OrganizationMember previousValue = _OrganizationMember.Entity;
                if (((previousValue != value)
                            || (_OrganizationMember.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _OrganizationMember.Entity = null;
                        previousValue.OrgMemMemTags.Remove(this);
                    }

                    _OrganizationMember.Entity = value;
                    if (value != null)
                    {
                        value.OrgMemMemTags.Add(this);

                        _OrgId = value.OrganizationId;

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _OrgId = default(int);

                        _PeopleId = default(int);

                    }

                    SendPropertyChanged("OrganizationMember");
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
