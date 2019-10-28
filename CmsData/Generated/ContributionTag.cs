using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ContributionTag")]
    public partial class ContributionTag : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _ContributionId;

        private string _TagName;

        private int? _Priority;



        private EntityRef<Contribution> _Contribution;

        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnContributionIdChanging(int value);
        partial void OnContributionIdChanged();

        partial void OnTagNameChanging(string value);
        partial void OnTagNameChanged();

        partial void OnPriorityChanging(int? value);
        partial void OnPriorityChanged();

        #endregion
        public ContributionTag()
        {


            this._Contribution = default(EntityRef<Contribution>);

            OnCreated();
        }


        #region Columns

        [Column(Name = "ContributionId", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int ContributionId
        {
            get => this._ContributionId;

            set
            {
                if (this._ContributionId != value)
                {
                    if (this._Contribution.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    this.OnContributionIdChanging(value);
                    this.SendPropertyChanging();
                    this._ContributionId = value;
                    this.SendPropertyChanged("ContributionId");
                    this.OnContributionIdChanged();
                }
            }
        }


        [Column(Name = "TagName", UpdateCheck = UpdateCheck.Never, Storage = "_TagName", DbType = "varchar(50) NOT NULL", IsPrimaryKey = true)]
        public string TagName
        {
            get => this._TagName;

            set
            {
                if (this._TagName != value)
                {
                    this.OnTagNameChanging(value);
                    this.SendPropertyChanging();
                    this._TagName = value;
                    this.SendPropertyChanged("TagName");
                    this.OnTagNameChanged();
                }
            }
        }


        [Column(Name = "Priority", UpdateCheck = UpdateCheck.Never, Storage = "_Priority", DbType = "int")]
        public int? Priority
        {
            get => this._Priority;

            set
            {
                if (this._Priority != value)
                {
                    this.OnPriorityChanging(value);
                    this.SendPropertyChanging();
                    this._Priority = value;
                    this.SendPropertyChanged("Priority");
                    this.OnPriorityChanged();
                }
            }
        }


        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_ContributionTag_Contribution", Storage = "_Contribution", ThisKey = "ContributionId", IsForeignKey = true)]
        public Contribution Contribution
        {
            get => this._Contribution.Entity;

            set
            {
                Contribution previousValue = this._Contribution.Entity;
                if (((previousValue != value)
                            || (this._Contribution.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if (previousValue != null)
                    {
                        this._Contribution.Entity = null;
                        previousValue.ContributionTags.Remove(this);
                    }

                    this._Contribution.Entity = value;
                    if (value != null)
                    {
                        value.ContributionTags.Add(this);

                        this._ContributionId = value.ContributionId;
                    }

                    else
                    {

                        this._ContributionId = default(int);
                    }

                    this.SendPropertyChanged("Contribution");
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
    }
}

