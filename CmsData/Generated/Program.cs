using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Program")]
    public partial class Program : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Name;

        private string _RptGroup;

        private decimal? _StartHoursOffset;

        private decimal? _EndHoursOffset;

        private EntitySet<Division> _Divisions;

        private EntitySet<ProgDiv> _ProgDivs;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnRptGroupChanging(string value);
        partial void OnRptGroupChanged();

        partial void OnStartHoursOffsetChanging(decimal? value);
        partial void OnStartHoursOffsetChanged();

        partial void OnEndHoursOffsetChanging(decimal? value);
        partial void OnEndHoursOffsetChanged();

        #endregion

        public Program()
        {
            _Divisions = new EntitySet<Division>(new Action<Division>(attach_Divisions), new Action<Division>(detach_Divisions));

            _ProgDivs = new EntitySet<ProgDiv>(new Action<ProgDiv>(attach_ProgDivs), new Action<ProgDiv>(detach_ProgDivs));

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

        [Column(Name = "RptGroup", UpdateCheck = UpdateCheck.Never, Storage = "_RptGroup", DbType = "nvarchar(200)")]
        public string RptGroup
        {
            get => _RptGroup;

            set
            {
                if (_RptGroup != value)
                {
                    OnRptGroupChanging(value);
                    SendPropertyChanging();
                    _RptGroup = value;
                    SendPropertyChanged("RptGroup");
                    OnRptGroupChanged();
                }
            }
        }

        [Column(Name = "StartHoursOffset", UpdateCheck = UpdateCheck.Never, Storage = "_StartHoursOffset", DbType = "real")]
        public decimal? StartHoursOffset
        {
            get => _StartHoursOffset;

            set
            {
                if (_StartHoursOffset != value)
                {
                    OnStartHoursOffsetChanging(value);
                    SendPropertyChanging();
                    _StartHoursOffset = value;
                    SendPropertyChanged("StartHoursOffset");
                    OnStartHoursOffsetChanged();
                }
            }
        }

        [Column(Name = "EndHoursOffset", UpdateCheck = UpdateCheck.Never, Storage = "_EndHoursOffset", DbType = "real")]
        public decimal? EndHoursOffset
        {
            get => _EndHoursOffset;

            set
            {
                if (_EndHoursOffset != value)
                {
                    OnEndHoursOffsetChanging(value);
                    SendPropertyChanging();
                    _EndHoursOffset = value;
                    SendPropertyChanged("EndHoursOffset");
                    OnEndHoursOffsetChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_Division_Program", Storage = "_Divisions", OtherKey = "ProgId")]
        public EntitySet<Division> Divisions
           {
               get => _Divisions;

            set => _Divisions.Assign(value);

           }

        [Association(Name = "FK_ProgDiv_Program", Storage = "_ProgDivs", OtherKey = "ProgId")]
        public EntitySet<ProgDiv> ProgDivs
           {
               get => _ProgDivs;

            set => _ProgDivs.Assign(value);

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

        private void attach_Divisions(Division entity)
        {
            SendPropertyChanging();
            entity.Program = this;
        }

        private void detach_Divisions(Division entity)
        {
            SendPropertyChanging();
            entity.Program = null;
        }

        private void attach_ProgDivs(ProgDiv entity)
        {
            SendPropertyChanging();
            entity.Program = this;
        }

        private void detach_ProgDivs(ProgDiv entity)
        {
            SendPropertyChanging();
            entity.Program = null;
        }
    }
}
