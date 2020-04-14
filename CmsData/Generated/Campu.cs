using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.Campus")]
    public partial class Campu : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Code;

        private string _Description;

        private bool? _Hardwired;

        private EntitySet<Contribution> _Contributions;

        private EntitySet<Organization> _Organizations;

        private EntitySet<GivingPage> _GivingPages;

        private EntitySet<Person> _People;

        private EntitySet<Resource> _Resources;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnCodeChanging(string value);
        partial void OnCodeChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnHardwiredChanging(bool? value);
        partial void OnHardwiredChanged();

        #endregion

        public Campu()
        {
            _Contributions = new EntitySet<Contribution>(new Action<Contribution>(attach_Contributions), new Action<Contribution>(detach_Contributions));

            _Organizations = new EntitySet<Organization>(new Action<Organization>(attach_Organizations), new Action<Organization>(detach_Organizations));

            _GivingPages = new EntitySet<GivingPage>(new Action<GivingPage>(attach_GivingPages), new Action<GivingPage>(detach_GivingPages));

            _People = new EntitySet<Person>(new Action<Person>(attach_People), new Action<Person>(detach_People));

            _Resources = new EntitySet<Resource>(new Action<Resource>(attach_Resources), new Action<Resource>(detach_Resources));

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "Code", UpdateCheck = UpdateCheck.Never, Storage = "_Code", DbType = "nvarchar(20)")]
        public string Code
        {
            get => _Code;

            set
            {
                if (_Code != value)
                {
                    OnCodeChanging(value);
                    SendPropertyChanging();
                    _Code = value;
                    SendPropertyChanged("Code");
                    OnCodeChanged();
                }
            }
        }

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(100)")]
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

        [Column(Name = "Hardwired", UpdateCheck = UpdateCheck.Never, Storage = "_Hardwired", DbType = "bit")]
        public bool? Hardwired
        {
            get => _Hardwired;

            set
            {
                if (_Hardwired != value)
                {
                    OnHardwiredChanging(value);
                    SendPropertyChanging();
                    _Hardwired = value;
                    SendPropertyChanged("Hardwired");
                    OnHardwiredChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables
        [Association(Name = "FK_Contribution_Campus", Storage = "_Contributions", OtherKey = "CampusId")]
        public EntitySet<Contribution> Contributions
        {
            get { return _Contributions; }

            set { _Contributions.Assign(value); }
        }

        [Association(Name = "FK_Organizations_Campus", Storage = "_Organizations", OtherKey = "CampusId")]
        public EntitySet<Organization> Organizations
        {
            get => _Organizations;

            set => _Organizations.Assign(value);

        }

        [Association(Name = "FK_People_Campus", Storage = "_People", OtherKey = "CampusId")]
        public EntitySet<Person> People
        {
            get => _People;

            set => _People.Assign(value);

        }

        [Association(Name = "FK_Resource_Campus", Storage = "_Resources", OtherKey = "CampusId")]
        public EntitySet<Resource> Resources
        {
            get => _Resources;

            set => _Resources.Assign(value);

        }

        [Association(Name = "FK_GivingPages_Campus", Storage = "_GivingPages", OtherKey = "CampusId")]
        public EntitySet<GivingPage> GivingPages
        {
            get => _GivingPages;

            set => _GivingPages.Assign(value);
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
            entity.Campu = this;
        }
        private void detach_Contributions(Contribution entity)
        {
            SendPropertyChanging();
            entity.Campu = null;
        }

        private void attach_Organizations(Organization entity)
        {
            SendPropertyChanging();
            entity.Campu = this;
        }

        private void detach_Organizations(Organization entity)
        {
            SendPropertyChanging();
            entity.Campu = null;
        }

        private void attach_GivingPages(GivingPage entity)
        {
            SendPropertyChanging();
            entity.Campu = this;
        }

        private void detach_GivingPages(GivingPage entity)
        {
            SendPropertyChanging();
            entity.Campu = null;
        }

        private void attach_People(Person entity)
        {
            SendPropertyChanging();
            entity.Campu = this;
        }

        private void detach_People(Person entity)
        {
            SendPropertyChanging();
            entity.Campu = null;
        }

        private void attach_Resources(Resource entity)
        {
            SendPropertyChanging();
            entity.Campu = this;
        }

        private void detach_Resources(Resource entity)
        {
            SendPropertyChanging();
            entity.Campu = null;
        }
    }
}
