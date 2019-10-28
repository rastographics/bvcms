using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ExtraData")]
    public partial class ExtraDatum : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Data;

        private DateTime? _Stamp;

        private bool? _Completed;

        private int? _OrganizationId;

        private int? _UserPeopleId;

        private bool? _Abandoned;

        private EntitySet<Contribution> _Contributions;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnDataChanging(string value);
        partial void OnDataChanged();

        partial void OnStampChanging(DateTime? value);
        partial void OnStampChanged();

        partial void OnCompletedChanging(bool? value);
        partial void OnCompletedChanged();

        partial void OnOrganizationIdChanging(int? value);
        partial void OnOrganizationIdChanged();

        partial void OnUserPeopleIdChanging(int? value);
        partial void OnUserPeopleIdChanged();

        partial void OnAbandonedChanging(bool? value);
        partial void OnAbandonedChanged();

        #endregion

        public ExtraDatum()
        {
            _Contributions = new EntitySet<Contribution>(new Action<Contribution>(attach_Contributions), new Action<Contribution>(detach_Contributions));

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

        [Column(Name = "Data", UpdateCheck = UpdateCheck.Never, Storage = "_Data", DbType = "nvarchar")]
        public string Data
        {
            get => _Data;

            set
            {
                if (_Data != value)
                {
                    OnDataChanging(value);
                    SendPropertyChanging();
                    _Data = value;
                    SendPropertyChanged("Data");
                    OnDataChanged();
                }
            }
        }

        [Column(Name = "Stamp", UpdateCheck = UpdateCheck.Never, Storage = "_Stamp", DbType = "datetime")]
        public DateTime? Stamp
        {
            get => _Stamp;

            set
            {
                if (_Stamp != value)
                {
                    OnStampChanging(value);
                    SendPropertyChanging();
                    _Stamp = value;
                    SendPropertyChanged("Stamp");
                    OnStampChanged();
                }
            }
        }

        [Column(Name = "completed", UpdateCheck = UpdateCheck.Never, Storage = "_Completed", DbType = "bit")]
        public bool? Completed
        {
            get => _Completed;

            set
            {
                if (_Completed != value)
                {
                    OnCompletedChanging(value);
                    SendPropertyChanging();
                    _Completed = value;
                    SendPropertyChanged("Completed");
                    OnCompletedChanged();
                }
            }
        }

        [Column(Name = "OrganizationId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationId", DbType = "int")]
        public int? OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    OnOrganizationIdChanging(value);
                    SendPropertyChanging();
                    _OrganizationId = value;
                    SendPropertyChanged("OrganizationId");
                    OnOrganizationIdChanged();
                }
            }
        }

        [Column(Name = "UserPeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_UserPeopleId", DbType = "int")]
        public int? UserPeopleId
        {
            get => _UserPeopleId;

            set
            {
                if (_UserPeopleId != value)
                {
                    OnUserPeopleIdChanging(value);
                    SendPropertyChanging();
                    _UserPeopleId = value;
                    SendPropertyChanged("UserPeopleId");
                    OnUserPeopleIdChanged();
                }
            }
        }

        [Column(Name = "abandoned", UpdateCheck = UpdateCheck.Never, Storage = "_Abandoned", DbType = "bit")]
        public bool? Abandoned
        {
            get => _Abandoned;

            set
            {
                if (_Abandoned != value)
                {
                    OnAbandonedChanging(value);
                    SendPropertyChanging();
                    _Abandoned = value;
                    SendPropertyChanged("Abandoned");
                    OnAbandonedChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_Contribution_ExtraData", Storage = "_Contributions", OtherKey = "ExtraDataId")]
        public EntitySet<Contribution> Contributions
           {
               get => _Contributions;

            set => _Contributions.Assign(value);

           }

        #endregion

        #region Foreign Keys

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

        private void attach_Contributions(Contribution entity)
        {
            SendPropertyChanging();
            entity.ExtraDatum = this;
        }

        private void detach_Contributions(Contribution entity)
        {
            SendPropertyChanging();
            entity.ExtraDatum = null;
        }
    }
}
