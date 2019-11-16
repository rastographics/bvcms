using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.RegistrationData")]
    public partial class RegistrationDatum : INotifyPropertyChanging, INotifyPropertyChanged
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

        private EntitySet<OrganizationMember> _OrganizationMembers;

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

        public RegistrationDatum()
        {
            _OrganizationMembers = new EntitySet<OrganizationMember>(new Action<OrganizationMember>(attach_OrganizationMembers), new Action<OrganizationMember>(detach_OrganizationMembers));

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

        [Column(Name = "Data", UpdateCheck = UpdateCheck.Never, Storage = "_Data", DbType = "xml")]
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

        [Association(Name = "FK_OrganizationMembers_RegistrationData", Storage = "_OrganizationMembers", OtherKey = "RegistrationDataId")]
        public EntitySet<OrganizationMember> OrganizationMembers
           {
               get => _OrganizationMembers;

            set => _OrganizationMembers.Assign(value);

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

        private void attach_OrganizationMembers(OrganizationMember entity)
        {
            SendPropertyChanging();
            entity.RegistrationDatum = this;
        }

        private void detach_OrganizationMembers(OrganizationMember entity)
        {
            SendPropertyChanging();
            entity.RegistrationDatum = null;
        }
    }
}
