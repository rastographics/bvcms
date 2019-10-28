using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.DivOrg")]
    public partial class DivOrg : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _DivId;

        private int _OrgId;

        private int? _Id;

        private EntityRef<Division> _Division;

        private EntityRef<Organization> _Organization;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnDivIdChanging(int value);
        partial void OnDivIdChanged();

        partial void OnOrgIdChanging(int value);
        partial void OnOrgIdChanged();

        partial void OnIdChanging(int? value);
        partial void OnIdChanged();

        #endregion

        public DivOrg()
        {
            _Division = default(EntityRef<Division>);

            _Organization = default(EntityRef<Organization>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "DivId", UpdateCheck = UpdateCheck.Never, Storage = "_DivId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int DivId
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

        [Column(Name = "OrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OrgId", DbType = "int NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int")]
        public int? Id
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

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_DivOrg_Division", Storage = "_Division", ThisKey = "DivId", IsForeignKey = true)]
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
                        previousValue.DivOrgs.Remove(this);
                    }

                    _Division.Entity = value;
                    if (value != null)
                    {
                        value.DivOrgs.Add(this);

                        _DivId = value.Id;

                    }

                    else
                    {
                        _DivId = default(int);

                    }

                    SendPropertyChanged("Division");
                }
            }
        }

        [Association(Name = "FK_DivOrg_Organizations", Storage = "_Organization", ThisKey = "OrgId", IsForeignKey = true)]
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
                        previousValue.DivOrgs.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.DivOrgs.Add(this);

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
