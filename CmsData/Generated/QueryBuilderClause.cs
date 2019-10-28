using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.QueryBuilderClauses")]
    public partial class QueryBuilderClause : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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

            this._Clauses = new EntitySet<QueryBuilderClause>(new Action<QueryBuilderClause>(this.attach_Clauses), new Action<QueryBuilderClause>(this.detach_Clauses));


            this._Parent = default(EntityRef<QueryBuilderClause>);

            OnCreated();
        }


        #region Columns

        [Column(Name = "QueryId", UpdateCheck = UpdateCheck.Never, Storage = "_QueryId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int QueryId
        {
            get => this._QueryId;

            set
            {
                if (this._QueryId != value)
                {

                    this.OnQueryIdChanging(value);
                    this.SendPropertyChanging();
                    this._QueryId = value;
                    this.SendPropertyChanged("QueryId");
                    this.OnQueryIdChanged();
                }

            }

        }


        [Column(Name = "ClauseOrder", UpdateCheck = UpdateCheck.Never, Storage = "_ClauseOrder", DbType = "int NOT NULL")]
        public int ClauseOrder
        {
            get => this._ClauseOrder;

            set
            {
                if (this._ClauseOrder != value)
                {

                    this.OnClauseOrderChanging(value);
                    this.SendPropertyChanging();
                    this._ClauseOrder = value;
                    this.SendPropertyChanged("ClauseOrder");
                    this.OnClauseOrderChanged();
                }

            }

        }


        [Column(Name = "GroupId", UpdateCheck = UpdateCheck.Never, Storage = "_GroupId", DbType = "int")]
        public int? GroupId
        {
            get => this._GroupId;

            set
            {
                if (this._GroupId != value)
                {

                    if (this._Parent.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    this.OnGroupIdChanging(value);
                    this.SendPropertyChanging();
                    this._GroupId = value;
                    this.SendPropertyChanged("GroupId");
                    this.OnGroupIdChanged();
                }

            }

        }


        [Column(Name = "Field", UpdateCheck = UpdateCheck.Never, Storage = "_Field", DbType = "nvarchar(32)")]
        public string Field
        {
            get => this._Field;

            set
            {
                if (this._Field != value)
                {

                    this.OnFieldChanging(value);
                    this.SendPropertyChanging();
                    this._Field = value;
                    this.SendPropertyChanged("Field");
                    this.OnFieldChanged();
                }

            }

        }


        [Column(Name = "Comparison", UpdateCheck = UpdateCheck.Never, Storage = "_Comparison", DbType = "nvarchar(20)")]
        public string Comparison
        {
            get => this._Comparison;

            set
            {
                if (this._Comparison != value)
                {

                    this.OnComparisonChanging(value);
                    this.SendPropertyChanging();
                    this._Comparison = value;
                    this.SendPropertyChanged("Comparison");
                    this.OnComparisonChanged();
                }

            }

        }


        [Column(Name = "TextValue", UpdateCheck = UpdateCheck.Never, Storage = "_TextValue", DbType = "nvarchar(100)")]
        public string TextValue
        {
            get => this._TextValue;

            set
            {
                if (this._TextValue != value)
                {

                    this.OnTextValueChanging(value);
                    this.SendPropertyChanging();
                    this._TextValue = value;
                    this.SendPropertyChanged("TextValue");
                    this.OnTextValueChanged();
                }

            }

        }


        [Column(Name = "DateValue", UpdateCheck = UpdateCheck.Never, Storage = "_DateValue", DbType = "datetime")]
        public DateTime? DateValue
        {
            get => this._DateValue;

            set
            {
                if (this._DateValue != value)
                {

                    this.OnDateValueChanging(value);
                    this.SendPropertyChanging();
                    this._DateValue = value;
                    this.SendPropertyChanged("DateValue");
                    this.OnDateValueChanged();
                }

            }

        }


        [Column(Name = "CodeIdValue", UpdateCheck = UpdateCheck.Never, Storage = "_CodeIdValue", DbType = "nvarchar(3000)")]
        public string CodeIdValue
        {
            get => this._CodeIdValue;

            set
            {
                if (this._CodeIdValue != value)
                {

                    this.OnCodeIdValueChanging(value);
                    this.SendPropertyChanging();
                    this._CodeIdValue = value;
                    this.SendPropertyChanged("CodeIdValue");
                    this.OnCodeIdValueChanged();
                }

            }

        }


        [Column(Name = "StartDate", UpdateCheck = UpdateCheck.Never, Storage = "_StartDate", DbType = "datetime")]
        public DateTime? StartDate
        {
            get => this._StartDate;

            set
            {
                if (this._StartDate != value)
                {

                    this.OnStartDateChanging(value);
                    this.SendPropertyChanging();
                    this._StartDate = value;
                    this.SendPropertyChanged("StartDate");
                    this.OnStartDateChanged();
                }

            }

        }


        [Column(Name = "EndDate", UpdateCheck = UpdateCheck.Never, Storage = "_EndDate", DbType = "datetime")]
        public DateTime? EndDate
        {
            get => this._EndDate;

            set
            {
                if (this._EndDate != value)
                {

                    this.OnEndDateChanging(value);
                    this.SendPropertyChanging();
                    this._EndDate = value;
                    this.SendPropertyChanged("EndDate");
                    this.OnEndDateChanged();
                }

            }

        }


        [Column(Name = "Program", UpdateCheck = UpdateCheck.Never, Storage = "_Program", DbType = "int NOT NULL")]
        public int Program
        {
            get => this._Program;

            set
            {
                if (this._Program != value)
                {

                    this.OnProgramChanging(value);
                    this.SendPropertyChanging();
                    this._Program = value;
                    this.SendPropertyChanged("Program");
                    this.OnProgramChanged();
                }

            }

        }


        [Column(Name = "Division", UpdateCheck = UpdateCheck.Never, Storage = "_Division", DbType = "int NOT NULL")]
        public int Division
        {
            get => this._Division;

            set
            {
                if (this._Division != value)
                {

                    this.OnDivisionChanging(value);
                    this.SendPropertyChanging();
                    this._Division = value;
                    this.SendPropertyChanged("Division");
                    this.OnDivisionChanged();
                }

            }

        }


        [Column(Name = "Organization", UpdateCheck = UpdateCheck.Never, Storage = "_Organization", DbType = "int NOT NULL")]
        public int Organization
        {
            get => this._Organization;

            set
            {
                if (this._Organization != value)
                {

                    this.OnOrganizationChanging(value);
                    this.SendPropertyChanging();
                    this._Organization = value;
                    this.SendPropertyChanged("Organization");
                    this.OnOrganizationChanged();
                }

            }

        }


        [Column(Name = "Days", UpdateCheck = UpdateCheck.Never, Storage = "_Days", DbType = "int NOT NULL")]
        public int Days
        {
            get => this._Days;

            set
            {
                if (this._Days != value)
                {

                    this.OnDaysChanging(value);
                    this.SendPropertyChanging();
                    this._Days = value;
                    this.SendPropertyChanged("Days");
                    this.OnDaysChanged();
                }

            }

        }


        [Column(Name = "SavedBy", UpdateCheck = UpdateCheck.Never, Storage = "_SavedBy", DbType = "nvarchar(50)")]
        public string SavedBy
        {
            get => this._SavedBy;

            set
            {
                if (this._SavedBy != value)
                {

                    this.OnSavedByChanging(value);
                    this.SendPropertyChanging();
                    this._SavedBy = value;
                    this.SendPropertyChanged("SavedBy");
                    this.OnSavedByChanged();
                }

            }

        }


        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(150)")]
        public string Description
        {
            get => this._Description;

            set
            {
                if (this._Description != value)
                {

                    this.OnDescriptionChanging(value);
                    this.SendPropertyChanging();
                    this._Description = value;
                    this.SendPropertyChanged("Description");
                    this.OnDescriptionChanged();
                }

            }

        }


        [Column(Name = "IsPublic", UpdateCheck = UpdateCheck.Never, Storage = "_IsPublic", DbType = "bit NOT NULL")]
        public bool IsPublic
        {
            get => this._IsPublic;

            set
            {
                if (this._IsPublic != value)
                {

                    this.OnIsPublicChanging(value);
                    this.SendPropertyChanging();
                    this._IsPublic = value;
                    this.SendPropertyChanged("IsPublic");
                    this.OnIsPublicChanged();
                }

            }

        }


        [Column(Name = "CreatedOn", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedOn", DbType = "datetime NOT NULL")]
        public DateTime CreatedOn
        {
            get => this._CreatedOn;

            set
            {
                if (this._CreatedOn != value)
                {

                    this.OnCreatedOnChanging(value);
                    this.SendPropertyChanging();
                    this._CreatedOn = value;
                    this.SendPropertyChanged("CreatedOn");
                    this.OnCreatedOnChanged();
                }

            }

        }


        [Column(Name = "Quarters", UpdateCheck = UpdateCheck.Never, Storage = "_Quarters", DbType = "nvarchar(50)")]
        public string Quarters
        {
            get => this._Quarters;

            set
            {
                if (this._Quarters != value)
                {

                    this.OnQuartersChanging(value);
                    this.SendPropertyChanging();
                    this._Quarters = value;
                    this.SendPropertyChanged("Quarters");
                    this.OnQuartersChanged();
                }

            }

        }


        [Column(Name = "SavedQueryIdDesc", UpdateCheck = UpdateCheck.Never, Storage = "_SavedQueryIdDesc", DbType = "nvarchar(100)")]
        public string SavedQueryIdDesc
        {
            get => this._SavedQueryIdDesc;

            set
            {
                if (this._SavedQueryIdDesc != value)
                {

                    this.OnSavedQueryIdDescChanging(value);
                    this.SendPropertyChanging();
                    this._SavedQueryIdDesc = value;
                    this.SendPropertyChanged("SavedQueryIdDesc");
                    this.OnSavedQueryIdDescChanged();
                }

            }

        }


        [Column(Name = "Tags", UpdateCheck = UpdateCheck.Never, Storage = "_Tags", DbType = "nvarchar(500)")]
        public string Tags
        {
            get => this._Tags;

            set
            {
                if (this._Tags != value)
                {

                    this.OnTagsChanging(value);
                    this.SendPropertyChanging();
                    this._Tags = value;
                    this.SendPropertyChanged("Tags");
                    this.OnTagsChanged();
                }

            }

        }


        [Column(Name = "Schedule", UpdateCheck = UpdateCheck.Never, Storage = "_Schedule", DbType = "int NOT NULL")]
        public int Schedule
        {
            get => this._Schedule;

            set
            {
                if (this._Schedule != value)
                {

                    this.OnScheduleChanging(value);
                    this.SendPropertyChanging();
                    this._Schedule = value;
                    this.SendPropertyChanged("Schedule");
                    this.OnScheduleChanged();
                }

            }

        }


        [Column(Name = "Age", UpdateCheck = UpdateCheck.Never, Storage = "_Age", DbType = "int")]
        public int? Age
        {
            get => this._Age;

            set
            {
                if (this._Age != value)
                {

                    this.OnAgeChanging(value);
                    this.SendPropertyChanging();
                    this._Age = value;
                    this.SendPropertyChanged("Age");
                    this.OnAgeChanged();
                }

            }

        }


        [Column(Name = "Campus", UpdateCheck = UpdateCheck.Never, Storage = "_Campus", DbType = "int")]
        public int? Campus
        {
            get => this._Campus;

            set
            {
                if (this._Campus != value)
                {

                    this.OnCampusChanging(value);
                    this.SendPropertyChanging();
                    this._Campus = value;
                    this.SendPropertyChanged("Campus");
                    this.OnCampusChanged();
                }

            }

        }


        [Column(Name = "OrgType", UpdateCheck = UpdateCheck.Never, Storage = "_OrgType", DbType = "int")]
        public int? OrgType
        {
            get => this._OrgType;

            set
            {
                if (this._OrgType != value)
                {

                    this.OnOrgTypeChanging(value);
                    this.SendPropertyChanging();
                    this._OrgType = value;
                    this.SendPropertyChanged("OrgType");
                    this.OnOrgTypeChanged();
                }

            }

        }


        #endregion

        #region Foreign Key Tables

        [Association(Name = "Clauses__Parent", Storage = "_Clauses", OtherKey = "GroupId")]
        public EntitySet<QueryBuilderClause> Clauses
           {
               get => this._Clauses;

            set => this._Clauses.Assign(value);

           }


        #endregion

        #region Foreign Keys

        [Association(Name = "Clauses__Parent", Storage = "_Parent", ThisKey = "GroupId", IsForeignKey = true)]
        public QueryBuilderClause Parent
        {
            get => this._Parent.Entity;

            set
            {
                QueryBuilderClause previousValue = this._Parent.Entity;
                if (((previousValue != value)
                            || (this._Parent.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if (previousValue != null)
                    {
                        this._Parent.Entity = null;
                        previousValue.Clauses.Remove(this);
                    }

                    this._Parent.Entity = value;
                    if (value != null)
                    {
                        value.Clauses.Add(this);

                        this._GroupId = value.QueryId;

                    }

                    else
                    {

                        this._GroupId = default(int?);

                    }

                    this.SendPropertyChanged("Parent");
                }

            }

        }


        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private void attach_Clauses(QueryBuilderClause entity)
        {
            this.SendPropertyChanging();
            entity.Parent = this;
        }

        private void detach_Clauses(QueryBuilderClause entity)
        {
            this.SendPropertyChanging();
            entity.Parent = null;
        }


    }

}

