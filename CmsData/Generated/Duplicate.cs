using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Duplicate")]
    public partial class Duplicate : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id1;

        private int _Id2;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnId1Changing(int value);
        partial void OnId1Changed();

        partial void OnId2Changing(int value);
        partial void OnId2Changed();

        #endregion

        public Duplicate()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "id1", UpdateCheck = UpdateCheck.Never, Storage = "_Id1", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int Id1
        {
            get => _Id1;

            set
            {
                if (_Id1 != value)
                {
                    OnId1Changing(value);
                    SendPropertyChanging();
                    _Id1 = value;
                    SendPropertyChanged("Id1");
                    OnId1Changed();
                }
            }
        }

        [Column(Name = "id2", UpdateCheck = UpdateCheck.Never, Storage = "_Id2", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int Id2
        {
            get => _Id2;

            set
            {
                if (_Id2 != value)
                {
                    OnId2Changing(value);
                    SendPropertyChanging();
                    _Id2 = value;
                    SendPropertyChanged("Id2");
                    OnId2Changed();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

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
    }
}
