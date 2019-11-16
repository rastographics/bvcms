using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Division")]
    public partial class Division : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Name;

        private int? _ProgId;

        private int? _SortOrder;

        private string _EmailMessage;

        private string _EmailSubject;

        private string _Instructions;

        private string _Terms;

        private int? _ReportLine;

        private bool? _NoDisplayZero;

        private EntitySet<Coupon> _Coupons;

        private EntitySet<DivOrg> _DivOrgs;

        private EntitySet<Organization> _Organizations;

        private EntitySet<ProgDiv> _ProgDivs;

        private EntitySet<Resource> _Resources;

        private EntitySet<Promotion> _FromPromotions;

        private EntitySet<Promotion> _ToPromotions;

        private EntityRef<Program> _Program;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnProgIdChanging(int? value);
        partial void OnProgIdChanged();

        partial void OnSortOrderChanging(int? value);
        partial void OnSortOrderChanged();

        partial void OnEmailMessageChanging(string value);
        partial void OnEmailMessageChanged();

        partial void OnEmailSubjectChanging(string value);
        partial void OnEmailSubjectChanged();

        partial void OnInstructionsChanging(string value);
        partial void OnInstructionsChanged();

        partial void OnTermsChanging(string value);
        partial void OnTermsChanged();

        partial void OnReportLineChanging(int? value);
        partial void OnReportLineChanged();

        partial void OnNoDisplayZeroChanging(bool? value);
        partial void OnNoDisplayZeroChanged();

        #endregion

        public Division()
        {
            _Coupons = new EntitySet<Coupon>(new Action<Coupon>(attach_Coupons), new Action<Coupon>(detach_Coupons));

            _DivOrgs = new EntitySet<DivOrg>(new Action<DivOrg>(attach_DivOrgs), new Action<DivOrg>(detach_DivOrgs));

            _Organizations = new EntitySet<Organization>(new Action<Organization>(attach_Organizations), new Action<Organization>(detach_Organizations));

            _ProgDivs = new EntitySet<ProgDiv>(new Action<ProgDiv>(attach_ProgDivs), new Action<ProgDiv>(detach_ProgDivs));

            _Resources = new EntitySet<Resource>(new Action<Resource>(attach_Resources), new Action<Resource>(detach_Resources));

            _FromPromotions = new EntitySet<Promotion>(new Action<Promotion>(attach_FromPromotions), new Action<Promotion>(detach_FromPromotions));

            _ToPromotions = new EntitySet<Promotion>(new Action<Promotion>(attach_ToPromotions), new Action<Promotion>(detach_ToPromotions));

            _Program = default(EntityRef<Program>);

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

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(50)")]
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

        [Column(Name = "ProgId", UpdateCheck = UpdateCheck.Never, Storage = "_ProgId", DbType = "int")]
        [IsForeignKey]
        public int? ProgId
        {
            get => _ProgId;

            set
            {
                if (_ProgId != value)
                {
                    if (_Program.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnProgIdChanging(value);
                    SendPropertyChanging();
                    _ProgId = value;
                    SendPropertyChanged("ProgId");
                    OnProgIdChanged();
                }
            }
        }

        [Column(Name = "SortOrder", UpdateCheck = UpdateCheck.Never, Storage = "_SortOrder", DbType = "int")]
        public int? SortOrder
        {
            get => _SortOrder;

            set
            {
                if (_SortOrder != value)
                {
                    OnSortOrderChanging(value);
                    SendPropertyChanging();
                    _SortOrder = value;
                    SendPropertyChanged("SortOrder");
                    OnSortOrderChanged();
                }
            }
        }

        [Column(Name = "EmailMessage", UpdateCheck = UpdateCheck.Never, Storage = "_EmailMessage", DbType = "nvarchar")]
        public string EmailMessage
        {
            get => _EmailMessage;

            set
            {
                if (_EmailMessage != value)
                {
                    OnEmailMessageChanging(value);
                    SendPropertyChanging();
                    _EmailMessage = value;
                    SendPropertyChanged("EmailMessage");
                    OnEmailMessageChanged();
                }
            }
        }

        [Column(Name = "EmailSubject", UpdateCheck = UpdateCheck.Never, Storage = "_EmailSubject", DbType = "nvarchar(100)")]
        public string EmailSubject
        {
            get => _EmailSubject;

            set
            {
                if (_EmailSubject != value)
                {
                    OnEmailSubjectChanging(value);
                    SendPropertyChanging();
                    _EmailSubject = value;
                    SendPropertyChanged("EmailSubject");
                    OnEmailSubjectChanged();
                }
            }
        }

        [Column(Name = "Instructions", UpdateCheck = UpdateCheck.Never, Storage = "_Instructions", DbType = "nvarchar")]
        public string Instructions
        {
            get => _Instructions;

            set
            {
                if (_Instructions != value)
                {
                    OnInstructionsChanging(value);
                    SendPropertyChanging();
                    _Instructions = value;
                    SendPropertyChanged("Instructions");
                    OnInstructionsChanged();
                }
            }
        }

        [Column(Name = "Terms", UpdateCheck = UpdateCheck.Never, Storage = "_Terms", DbType = "nvarchar")]
        public string Terms
        {
            get => _Terms;

            set
            {
                if (_Terms != value)
                {
                    OnTermsChanging(value);
                    SendPropertyChanging();
                    _Terms = value;
                    SendPropertyChanged("Terms");
                    OnTermsChanged();
                }
            }
        }

        [Column(Name = "ReportLine", UpdateCheck = UpdateCheck.Never, Storage = "_ReportLine", DbType = "int")]
        public int? ReportLine
        {
            get => _ReportLine;

            set
            {
                if (_ReportLine != value)
                {
                    OnReportLineChanging(value);
                    SendPropertyChanging();
                    _ReportLine = value;
                    SendPropertyChanged("ReportLine");
                    OnReportLineChanged();
                }
            }
        }

        [Column(Name = "NoDisplayZero", UpdateCheck = UpdateCheck.Never, Storage = "_NoDisplayZero", DbType = "bit")]
        public bool? NoDisplayZero
        {
            get => _NoDisplayZero;

            set
            {
                if (_NoDisplayZero != value)
                {
                    OnNoDisplayZeroChanging(value);
                    SendPropertyChanging();
                    _NoDisplayZero = value;
                    SendPropertyChanged("NoDisplayZero");
                    OnNoDisplayZeroChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_Coupons_Division", Storage = "_Coupons", OtherKey = "DivId")]
        public EntitySet<Coupon> Coupons
           {
               get => _Coupons;

            set => _Coupons.Assign(value);

           }

        [Association(Name = "FK_DivOrg_Division", Storage = "_DivOrgs", OtherKey = "DivId")]
        public EntitySet<DivOrg> DivOrgs
           {
               get => _DivOrgs;

            set => _DivOrgs.Assign(value);

           }

        [Association(Name = "FK_Organizations_Division", Storage = "_Organizations", OtherKey = "DivisionId")]
        public EntitySet<Organization> Organizations
           {
               get => _Organizations;

            set => _Organizations.Assign(value);

           }

        [Association(Name = "FK_ProgDiv_Division", Storage = "_ProgDivs", OtherKey = "DivId")]
        public EntitySet<ProgDiv> ProgDivs
           {
               get => _ProgDivs;

            set => _ProgDivs.Assign(value);

           }

        [Association(Name = "FK_Resource_Division", Storage = "_Resources", OtherKey = "DivisionId")]
        public EntitySet<Resource> Resources
           {
               get => _Resources;

            set => _Resources.Assign(value);

           }

        [Association(Name = "FromPromotions__FromDivision", Storage = "_FromPromotions", OtherKey = "FromDivId")]
        public EntitySet<Promotion> FromPromotions
           {
               get => _FromPromotions;

            set => _FromPromotions.Assign(value);

           }

        [Association(Name = "ToPromotions__ToDivision", Storage = "_ToPromotions", OtherKey = "ToDivId")]
        public EntitySet<Promotion> ToPromotions
           {
               get => _ToPromotions;

            set => _ToPromotions.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Division_Program", Storage = "_Program", ThisKey = "ProgId", IsForeignKey = true)]
        public Program Program
        {
            get => _Program.Entity;

            set
            {
                Program previousValue = _Program.Entity;
                if (((previousValue != value)
                            || (_Program.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Program.Entity = null;
                        previousValue.Divisions.Remove(this);
                    }

                    _Program.Entity = value;
                    if (value != null)
                    {
                        value.Divisions.Add(this);

                        _ProgId = value.Id;

                    }

                    else
                    {
                        _ProgId = default(int?);

                    }

                    SendPropertyChanged("Program");
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

        private void attach_Coupons(Coupon entity)
        {
            SendPropertyChanging();
            entity.Division = this;
        }

        private void detach_Coupons(Coupon entity)
        {
            SendPropertyChanging();
            entity.Division = null;
        }

        private void attach_DivOrgs(DivOrg entity)
        {
            SendPropertyChanging();
            entity.Division = this;
        }

        private void detach_DivOrgs(DivOrg entity)
        {
            SendPropertyChanging();
            entity.Division = null;
        }

        private void attach_Organizations(Organization entity)
        {
            SendPropertyChanging();
            entity.Division = this;
        }

        private void detach_Organizations(Organization entity)
        {
            SendPropertyChanging();
            entity.Division = null;
        }

        private void attach_ProgDivs(ProgDiv entity)
        {
            SendPropertyChanging();
            entity.Division = this;
        }

        private void detach_ProgDivs(ProgDiv entity)
        {
            SendPropertyChanging();
            entity.Division = null;
        }

        private void attach_Resources(Resource entity)
        {
            SendPropertyChanging();
            entity.Division = this;
        }

        private void detach_Resources(Resource entity)
        {
            SendPropertyChanging();
            entity.Division = null;
        }

        private void attach_FromPromotions(Promotion entity)
        {
            SendPropertyChanging();
            entity.FromDivision = this;
        }

        private void detach_FromPromotions(Promotion entity)
        {
            SendPropertyChanging();
            entity.FromDivision = null;
        }

        private void attach_ToPromotions(Promotion entity)
        {
            SendPropertyChanging();
            entity.ToDivision = this;
        }

        private void detach_ToPromotions(Promotion entity)
        {
            SendPropertyChanging();
            entity.ToDivision = null;
        }
    }
}
