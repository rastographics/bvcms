using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.RelatedFamilies")]
    public partial class RelatedFamily : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _FamilyId;

        private int _RelatedFamilyId;

        private int _CreatedBy;

        private DateTime _CreatedDate;

        private string _FamilyRelationshipDesc;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private EntityRef<Family> _RelatedFamily1;

        private EntityRef<Family> _RelatedFamily2;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnFamilyIdChanging(int value);
        partial void OnFamilyIdChanged();

        partial void OnRelatedFamilyIdChanging(int value);
        partial void OnRelatedFamilyIdChanged();

        partial void OnCreatedByChanging(int value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        partial void OnFamilyRelationshipDescChanging(string value);
        partial void OnFamilyRelationshipDescChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        #endregion

        public RelatedFamily()
        {
            _RelatedFamily1 = default(EntityRef<Family>);

            _RelatedFamily2 = default(EntityRef<Family>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "FamilyId", UpdateCheck = UpdateCheck.Never, Storage = "_FamilyId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    if (_RelatedFamily1.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnFamilyIdChanging(value);
                    SendPropertyChanging();
                    _FamilyId = value;
                    SendPropertyChanged("FamilyId");
                    OnFamilyIdChanged();
                }
            }
        }

        [Column(Name = "RelatedFamilyId", UpdateCheck = UpdateCheck.Never, Storage = "_RelatedFamilyId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int RelatedFamilyId
        {
            get => _RelatedFamilyId;

            set
            {
                if (_RelatedFamilyId != value)
                {
                    if (_RelatedFamily2.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnRelatedFamilyIdChanging(value);
                    SendPropertyChanging();
                    _RelatedFamilyId = value;
                    SendPropertyChanged("RelatedFamilyId");
                    OnRelatedFamilyIdChanged();
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

        [Column(Name = "FamilyRelationshipDesc", UpdateCheck = UpdateCheck.Never, Storage = "_FamilyRelationshipDesc", DbType = "nvarchar(256) NOT NULL")]
        public string FamilyRelationshipDesc
        {
            get => _FamilyRelationshipDesc;

            set
            {
                if (_FamilyRelationshipDesc != value)
                {
                    OnFamilyRelationshipDescChanging(value);
                    SendPropertyChanging();
                    _FamilyRelationshipDesc = value;
                    SendPropertyChanged("FamilyRelationshipDesc");
                    OnFamilyRelationshipDescChanged();
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

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "RelatedFamilies1__RelatedFamily1", Storage = "_RelatedFamily1", ThisKey = "FamilyId", IsForeignKey = true)]
        public Family RelatedFamily1
        {
            get => _RelatedFamily1.Entity;

            set
            {
                Family previousValue = _RelatedFamily1.Entity;
                if (((previousValue != value)
                            || (_RelatedFamily1.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _RelatedFamily1.Entity = null;
                        previousValue.RelatedFamilies1.Remove(this);
                    }

                    _RelatedFamily1.Entity = value;
                    if (value != null)
                    {
                        value.RelatedFamilies1.Add(this);

                        _FamilyId = value.FamilyId;

                    }

                    else
                    {
                        _FamilyId = default(int);

                    }

                    SendPropertyChanged("RelatedFamily1");
                }
            }
        }

        [Association(Name = "RelatedFamilies2__RelatedFamily2", Storage = "_RelatedFamily2", ThisKey = "RelatedFamilyId", IsForeignKey = true)]
        public Family RelatedFamily2
        {
            get => _RelatedFamily2.Entity;

            set
            {
                Family previousValue = _RelatedFamily2.Entity;
                if (((previousValue != value)
                            || (_RelatedFamily2.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _RelatedFamily2.Entity = null;
                        previousValue.RelatedFamilies2.Remove(this);
                    }

                    _RelatedFamily2.Entity = value;
                    if (value != null)
                    {
                        value.RelatedFamilies2.Add(this);

                        _RelatedFamilyId = value.FamilyId;

                    }

                    else
                    {
                        _RelatedFamilyId = default(int);

                    }

                    SendPropertyChanged("RelatedFamily2");
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
