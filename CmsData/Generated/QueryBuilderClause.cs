using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.QueryBuilderClauses")]
    public partial class QueryBuilderClause : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _QueryId;

        private int _ClauseOrder;

        private int? _GroupId;

        private string _Field;

        private string _Comparison;

        private string _TextValue;

        private DateTime? _DateValue;

        private string _CodeIdValue;

        private DateTime? _StartDate;

        private DateTime? _EndDate;

        private int _Program;

        private int _Division;

        private int _Organization;

        private int _Days;

        private string _SavedBy;

        private string _Description;

        private bool _IsPublic;

        private DateTime _CreatedOn;

        private string _Quarters;

        private string _SavedQueryIdDesc;

        private string _Tags;

        private int _Schedule;

        private int? _Age;

        private int? _Campus;

        private int? _OrgType;

        private EntitySet<QueryBuilderClause> _Clauses;

        private EntityRef<QueryBuilderClause> _Parent;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnQueryIdChanging(int value);
        partial void OnQueryIdChanged();

        partial void OnClauseOrderChanging(int value);
        partial void OnClauseOrderChanged();

        partial void OnGroupIdChanging(int? value);
        partial void OnGroupIdChanged();

        partial void OnFieldChanging(string value);
        partial void OnFieldChanged();

        partial void OnComparisonChanging(string value);
        partial void OnComparisonChanged();

        partial void OnTextValueChanging(string value);
        partial void OnTextValueChanged();

        partial void OnDateValueChanging(DateTime? value);
        partial void OnDateValueChanged();

        partial void OnCodeIdValueChanging(string value);
        partial void OnCodeIdValueChanged();

        partial void OnStartDateChanging(DateTime? value);
        partial void OnStartDateChanged();

        partial void OnEndDateChanging(DateTime? value);
        partial void OnEndDateChanged();

        partial void OnProgramChanging(int value);
        partial void OnProgramChanged();

        partial void OnDivisionChanging(int value);
        partial void OnDivisionChanged();

        partial void OnOrganizationChanging(int value);
        partial void OnOrganizationChanged();

        partial void OnDaysChanging(int value);
        partial void OnDaysChanged();

        partial void OnSavedByChanging(string value);
        partial void OnSavedByChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnIsPublicChanging(bool value);
        partial void OnIsPublicChanged();

        partial void OnCreatedOnChanging(DateTime value);
        partial void OnCreatedOnChanged();

        partial void OnQuartersChanging(string value);
        partial void OnQuartersChanged();

        partial void OnSavedQueryIdDescChanging(string value);
        partial void OnSavedQueryIdDescChanged();

        partial void OnTagsChanging(string value);
        partial void OnTagsChanged();

        partial void OnScheduleChanging(int value);
        partial void OnScheduleChanged();

        partial void OnAgeChanging(int? value);
        partial void OnAgeChanged();

        partial void OnCampusChanging(int? value);
        partial void OnCampusChanged();

        partial void OnOrgTypeChanging(int? value);
        partial void OnOrgTypeChanged();

        #endregion

        public QueryBuilderClause()
        {
            _Clauses = new EntitySet<QueryBuilderClause>(new Action<QueryBuilderClause>(attach_Clauses), new Action<QueryBuilderClause>(detach_Clauses));

            _Parent = default(EntityRef<QueryBuilderClause>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "QueryId", UpdateCheck = UpdateCheck.Never, Storage = "_QueryId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int QueryId
        {
            get => _QueryId;

            set
            {
                if (_QueryId != value)
                {
                    OnQueryIdChanging(value);
                    SendPropertyChanging();
                    _QueryId = value;
                    SendPropertyChanged("QueryId");
                    OnQueryIdChanged();
                }
            }
        }

        [Column(Name = "ClauseOrder", UpdateCheck = UpdateCheck.Never, Storage = "_ClauseOrder", DbType = "int NOT NULL")]
        public int ClauseOrder
        {
            get => _ClauseOrder;

            set
            {
                if (_ClauseOrder != value)
                {
                    OnClauseOrderChanging(value);
                    SendPropertyChanging();
                    _ClauseOrder = value;
                    SendPropertyChanged("ClauseOrder");
                    OnClauseOrderChanged();
                }
            }
        }

        [Column(Name = "GroupId", UpdateCheck = UpdateCheck.Never, Storage = "_GroupId", DbType = "int")]
        public int? GroupId
        {
            get => _GroupId;

            set
            {
                if (_GroupId != value)
                {
                    if (_Parent.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnGroupIdChanging(value);
                    SendPropertyChanging();
                    _GroupId = value;
                    SendPropertyChanged("GroupId");
                    OnGroupIdChanged();
                }
            }
        }

        [Column(Name = "Field", UpdateCheck = UpdateCheck.Never, Storage = "_Field", DbType = "nvarchar(32)")]
        public string Field
        {
            get => _Field;

            set
            {
                if (_Field != value)
                {
                    OnFieldChanging(value);
                    SendPropertyChanging();
                    _Field = value;
                    SendPropertyChanged("Field");
                    OnFieldChanged();
                }
            }
        }

        [Column(Name = "Comparison", UpdateCheck = UpdateCheck.Never, Storage = "_Comparison", DbType = "nvarchar(20)")]
        public string Comparison
        {
            get => _Comparison;

            set
            {
                if (_Comparison != value)
                {
                    OnComparisonChanging(value);
                    SendPropertyChanging();
                    _Comparison = value;
                    SendPropertyChanged("Comparison");
                    OnComparisonChanged();
                }
            }
        }

        [Column(Name = "TextValue", UpdateCheck = UpdateCheck.Never, Storage = "_TextValue", DbType = "nvarchar(100)")]
        public string TextValue
        {
            get => _TextValue;

            set
            {
                if (_TextValue != value)
                {
                    OnTextValueChanging(value);
                    SendPropertyChanging();
                    _TextValue = value;
                    SendPropertyChanged("TextValue");
                    OnTextValueChanged();
                }
            }
        }

        [Column(Name = "DateValue", UpdateCheck = UpdateCheck.Never, Storage = "_DateValue", DbType = "datetime")]
        public DateTime? DateValue
        {
            get => _DateValue;

            set
            {
                if (_DateValue != value)
                {
                    OnDateValueChanging(value);
                    SendPropertyChanging();
                    _DateValue = value;
                    SendPropertyChanged("DateValue");
                    OnDateValueChanged();
                }
            }
        }

        [Column(Name = "CodeIdValue", UpdateCheck = UpdateCheck.Never, Storage = "_CodeIdValue", DbType = "nvarchar(3000)")]
        public string CodeIdValue
        {
            get => _CodeIdValue;

            set
            {
                if (_CodeIdValue != value)
                {
                    OnCodeIdValueChanging(value);
                    SendPropertyChanging();
                    _CodeIdValue = value;
                    SendPropertyChanged("CodeIdValue");
                    OnCodeIdValueChanged();
                }
            }
        }

        [Column(Name = "StartDate", UpdateCheck = UpdateCheck.Never, Storage = "_StartDate", DbType = "datetime")]
        public DateTime? StartDate
        {
            get => _StartDate;

            set
            {
                if (_StartDate != value)
                {
                    OnStartDateChanging(value);
                    SendPropertyChanging();
                    _StartDate = value;
                    SendPropertyChanged("StartDate");
                    OnStartDateChanged();
                }
            }
        }

        [Column(Name = "EndDate", UpdateCheck = UpdateCheck.Never, Storage = "_EndDate", DbType = "datetime")]
        public DateTime? EndDate
        {
            get => _EndDate;

            set
            {
                if (_EndDate != value)
                {
                    OnEndDateChanging(value);
                    SendPropertyChanging();
                    _EndDate = value;
                    SendPropertyChanged("EndDate");
                    OnEndDateChanged();
                }
            }
        }

        [Column(Name = "Program", UpdateCheck = UpdateCheck.Never, Storage = "_Program", DbType = "int NOT NULL")]
        public int Program
        {
            get => _Program;

            set
            {
                if (_Program != value)
                {
                    OnProgramChanging(value);
                    SendPropertyChanging();
                    _Program = value;
                    SendPropertyChanged("Program");
                    OnProgramChanged();
                }
            }
        }

        [Column(Name = "Division", UpdateCheck = UpdateCheck.Never, Storage = "_Division", DbType = "int NOT NULL")]
        public int Division
        {
            get => _Division;

            set
            {
                if (_Division != value)
                {
                    OnDivisionChanging(value);
                    SendPropertyChanging();
                    _Division = value;
                    SendPropertyChanged("Division");
                    OnDivisionChanged();
                }
            }
        }

        [Column(Name = "Organization", UpdateCheck = UpdateCheck.Never, Storage = "_Organization", DbType = "int NOT NULL")]
        public int Organization
        {
            get => _Organization;

            set
            {
                if (_Organization != value)
                {
                    OnOrganizationChanging(value);
                    SendPropertyChanging();
                    _Organization = value;
                    SendPropertyChanged("Organization");
                    OnOrganizationChanged();
                }
            }
        }

        [Column(Name = "Days", UpdateCheck = UpdateCheck.Never, Storage = "_Days", DbType = "int NOT NULL")]
        public int Days
        {
            get => _Days;

            set
            {
                if (_Days != value)
                {
                    OnDaysChanging(value);
                    SendPropertyChanging();
                    _Days = value;
                    SendPropertyChanged("Days");
                    OnDaysChanged();
                }
            }
        }

        [Column(Name = "SavedBy", UpdateCheck = UpdateCheck.Never, Storage = "_SavedBy", DbType = "nvarchar(50)")]
        public string SavedBy
        {
            get => _SavedBy;

            set
            {
                if (_SavedBy != value)
                {
                    OnSavedByChanging(value);
                    SendPropertyChanging();
                    _SavedBy = value;
                    SendPropertyChanged("SavedBy");
                    OnSavedByChanged();
                }
            }
        }

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(150)")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    OnDescriptionChanging(value);
                    SendPropertyChanging();
                    _Description = value;
                    SendPropertyChanged("Description");
                    OnDescriptionChanged();
                }
            }
        }

        [Column(Name = "IsPublic", UpdateCheck = UpdateCheck.Never, Storage = "_IsPublic", DbType = "bit NOT NULL")]
        public bool IsPublic
        {
            get => _IsPublic;

            set
            {
                if (_IsPublic != value)
                {
                    OnIsPublicChanging(value);
                    SendPropertyChanging();
                    _IsPublic = value;
                    SendPropertyChanged("IsPublic");
                    OnIsPublicChanged();
                }
            }
        }

        [Column(Name = "CreatedOn", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedOn", DbType = "datetime NOT NULL")]
        public DateTime CreatedOn
        {
            get => _CreatedOn;

            set
            {
                if (_CreatedOn != value)
                {
                    OnCreatedOnChanging(value);
                    SendPropertyChanging();
                    _CreatedOn = value;
                    SendPropertyChanged("CreatedOn");
                    OnCreatedOnChanged();
                }
            }
        }

        [Column(Name = "Quarters", UpdateCheck = UpdateCheck.Never, Storage = "_Quarters", DbType = "nvarchar(50)")]
        public string Quarters
        {
            get => _Quarters;

            set
            {
                if (_Quarters != value)
                {
                    OnQuartersChanging(value);
                    SendPropertyChanging();
                    _Quarters = value;
                    SendPropertyChanged("Quarters");
                    OnQuartersChanged();
                }
            }
        }

        [Column(Name = "SavedQueryIdDesc", UpdateCheck = UpdateCheck.Never, Storage = "_SavedQueryIdDesc", DbType = "nvarchar(100)")]
        public string SavedQueryIdDesc
        {
            get => _SavedQueryIdDesc;

            set
            {
                if (_SavedQueryIdDesc != value)
                {
                    OnSavedQueryIdDescChanging(value);
                    SendPropertyChanging();
                    _SavedQueryIdDesc = value;
                    SendPropertyChanged("SavedQueryIdDesc");
                    OnSavedQueryIdDescChanged();
                }
            }
        }

        [Column(Name = "Tags", UpdateCheck = UpdateCheck.Never, Storage = "_Tags", DbType = "nvarchar(500)")]
        public string Tags
        {
            get => _Tags;

            set
            {
                if (_Tags != value)
                {
                    OnTagsChanging(value);
                    SendPropertyChanging();
                    _Tags = value;
                    SendPropertyChanged("Tags");
                    OnTagsChanged();
                }
            }
        }

        [Column(Name = "Schedule", UpdateCheck = UpdateCheck.Never, Storage = "_Schedule", DbType = "int NOT NULL")]
        public int Schedule
        {
            get => _Schedule;

            set
            {
                if (_Schedule != value)
                {
                    OnScheduleChanging(value);
                    SendPropertyChanging();
                    _Schedule = value;
                    SendPropertyChanged("Schedule");
                    OnScheduleChanged();
                }
            }
        }

        [Column(Name = "Age", UpdateCheck = UpdateCheck.Never, Storage = "_Age", DbType = "int")]
        public int? Age
        {
            get => _Age;

            set
            {
                if (_Age != value)
                {
                    OnAgeChanging(value);
                    SendPropertyChanging();
                    _Age = value;
                    SendPropertyChanged("Age");
                    OnAgeChanged();
                }
            }
        }

        [Column(Name = "Campus", UpdateCheck = UpdateCheck.Never, Storage = "_Campus", DbType = "int")]
        public int? Campus
        {
            get => _Campus;

            set
            {
                if (_Campus != value)
                {
                    OnCampusChanging(value);
                    SendPropertyChanging();
                    _Campus = value;
                    SendPropertyChanged("Campus");
                    OnCampusChanged();
                }
            }
        }

        [Column(Name = "OrgType", UpdateCheck = UpdateCheck.Never, Storage = "_OrgType", DbType = "int")]
        public int? OrgType
        {
            get => _OrgType;

            set
            {
                if (_OrgType != value)
                {
                    OnOrgTypeChanging(value);
                    SendPropertyChanging();
                    _OrgType = value;
                    SendPropertyChanged("OrgType");
                    OnOrgTypeChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "Clauses__Parent", Storage = "_Clauses", OtherKey = "GroupId")]
        public EntitySet<QueryBuilderClause> Clauses
           {
               get => _Clauses;

            set => _Clauses.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "Clauses__Parent", Storage = "_Parent", ThisKey = "GroupId", IsForeignKey = true)]
        public QueryBuilderClause Parent
        {
            get => _Parent.Entity;

            set
            {
                QueryBuilderClause previousValue = _Parent.Entity;
                if (((previousValue != value)
                            || (_Parent.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Parent.Entity = null;
                        previousValue.Clauses.Remove(this);
                    }

                    _Parent.Entity = value;
                    if (value != null)
                    {
                        value.Clauses.Add(this);

                        _GroupId = value.QueryId;

                    }

                    else
                    {
                        _GroupId = default(int?);

                    }

                    SendPropertyChanged("Parent");
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

        private void attach_Clauses(QueryBuilderClause entity)
        {
            SendPropertyChanging();
            entity.Parent = this;
        }

        private void detach_Clauses(QueryBuilderClause entity)
        {
            SendPropertyChanging();
            entity.Parent = null;
        }
    }
}
