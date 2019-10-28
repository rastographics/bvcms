using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ProgDiv")]
    public partial class ProgDiv : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ProgId;

        private int _DivId;

        private EntityRef<Division> _Division;

        private EntityRef<Program> _Program;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnProgIdChanging(int value);
        partial void OnProgIdChanged();

        partial void OnDivIdChanging(int value);
        partial void OnDivIdChanged();

        #endregion

        public ProgDiv()
        {
            _Division = default(EntityRef<Division>);

            _Program = default(EntityRef<Program>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ProgId", UpdateCheck = UpdateCheck.Never, Storage = "_ProgId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int ProgId
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

        [Column(Name = "DivId", UpdateCheck = UpdateCheck.Never, Storage = "_DivId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int DivId
        {
            get => _DivId;

            set
            {
                if (_DivId != value)
                {
                    if (_Division.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnDivIdChanging(value);
                    SendPropertyChanging();
                    _DivId = value;
                    SendPropertyChanged("DivId");
                    OnDivIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_ProgDiv_Division", Storage = "_Division", ThisKey = "DivId", IsForeignKey = true)]
        public Division Division
        {
            get => _Division.Entity;

            set
            {
                Division previousValue = _Division.Entity;
                if (((previousValue != value)
                            || (_Division.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Division.Entity = null;
                        previousValue.ProgDivs.Remove(this);
                    }

                    _Division.Entity = value;
                    if (value != null)
                    {
                        value.ProgDivs.Add(this);

                        _DivId = value.Id;

                    }

                    else
                    {
                        _DivId = default(int);

                    }

                    SendPropertyChanged("Division");
                }
            }
        }

        [Association(Name = "FK_ProgDiv_Program", Storage = "_Program", ThisKey = "ProgId", IsForeignKey = true)]
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
                        previousValue.ProgDivs.Remove(this);
                    }

                    _Program.Entity = value;
                    if (value != null)
                    {
                        value.ProgDivs.Add(this);

                        _ProgId = value.Id;

                    }

                    else
                    {
                        _ProgId = default(int);

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
    }
}
