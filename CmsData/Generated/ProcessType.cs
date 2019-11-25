using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.ProcessType")]
    public partial class ProcessType : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ProcessTypeId;
        private string _ProcessTypeName;
        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnProcessTypeIdChanging(int value);
        partial void OnProcessTypeIdChanged();

        partial void OnProcessTypeNameChanging(string value);
        partial void OnProcessTypeNameChanged();
        #endregion

        public ProcessType()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "ProcessTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_ProcessTypeId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int PeopleId
        {
            get => _ProcessTypeId;

            set
            {
                if (_ProcessTypeId != value)
                {
                    OnProcessTypeIdChanging(value);
                    SendPropertyChanging();
                    _ProcessTypeId = value;
                    SendPropertyChanged("ProcessTypeId");
                    OnProcessTypeIdChanged();
                }
            }
        }

        [Column(Name = "ProcessTypeName", UpdateCheck = UpdateCheck.Never, Storage = "_ProcessTypeName", DbType = "nvarchar")]
        public string ProcessTypeName
        {
            get => _ProcessTypeName;

            set
            {
                if (_ProcessTypeName != value)
                {
                    OnProcessTypeNameChanging(value);
                    SendPropertyChanging();
                    _ProcessTypeName = value;
                    SendPropertyChanged("ProcessTypeName");
                    OnProcessTypeNameChanged();
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
