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
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            _Contribution = default(EntityRef<Contribution>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ContributionId", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int ContributionId
        {
            get => _ContributionId;

            set
            {
                if (_ContributionId != value)
                {
                    if (_Contribution.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnContributionIdChanging(value);
                    SendPropertyChanging();
                    _ContributionId = value;
                    SendPropertyChanged("ContributionId");
                    OnContributionIdChanged();
                }
            }
        }

        [Column(Name = "TagName", UpdateCheck = UpdateCheck.Never, Storage = "_TagName", DbType = "varchar(50) NOT NULL", IsPrimaryKey = true)]
        public string TagName
        {
            get => _TagName;

            set
            {
                if (_TagName != value)
                {
                    OnTagNameChanging(value);
                    SendPropertyChanging();
                    _TagName = value;
                    SendPropertyChanged("TagName");
                    OnTagNameChanged();
                }
            }
        }

        [Column(Name = "Priority", UpdateCheck = UpdateCheck.Never, Storage = "_Priority", DbType = "int")]
        public int? Priority
        {
            get => _Priority;

            set
            {
                if (_Priority != value)
                {
                    OnPriorityChanging(value);
                    SendPropertyChanging();
                    _Priority = value;
                    SendPropertyChanged("Priority");
                    OnPriorityChanged();
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
            get => _Contribution.Entity;

            set
            {
                Contribution previousValue = _Contribution.Entity;
                if (((previousValue != value)
                            || (_Contribution.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Contribution.Entity = null;
                        previousValue.ContributionTags.Remove(this);
                    }

                    _Contribution.Entity = value;
                    if (value != null)
                    {
                        value.ContributionTags.Add(this);

                        _ContributionId = value.ContributionId;
                    }

                    else
                    {
                        _ContributionId = default(int);
                    }

                    SendPropertyChanged("Contribution");
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
