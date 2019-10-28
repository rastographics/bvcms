using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.GatewayServiceType")]
    public partial class GatewayServiceType : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _GatewayServiceTypeId;
        private string _GatewayServiceTypeName;
        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnGatewayServiceTypeIdChanging(int value);
        partial void OnGatewayServiceTypeIdChanged();

        partial void OnGatewayServiceTypeNameChanging(string value);
        partial void OnGatewayServiceTypeNameChanged();
        #endregion

        public GatewayServiceType()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "GatewayServiceTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayServiceTypeId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int PeopleId
        {
            get => _GatewayServiceTypeId;

            set
            {
                if (_GatewayServiceTypeId != value)
                {
                    OnGatewayServiceTypeIdChanging(value);
                    SendPropertyChanging();
                    _GatewayServiceTypeId = value;
                    SendPropertyChanged("GatewayServiceTypeId");
                    OnGatewayServiceTypeIdChanged();
                }
            }
        }

        [Column(Name = "GatewayServiceTypeName", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayServiceTypeName", DbType = "nvarchar")]
        public string GatewayServiceTypeName
        {
            get => _GatewayServiceTypeName;

            set
            {
                if (_GatewayServiceTypeName != value)
                {
                    OnGatewayServiceTypeNameChanging(value);
                    SendPropertyChanging();
                    _GatewayServiceTypeName = value;
                    SendPropertyChanged("GatewayServiceTypeName");
                    OnGatewayServiceTypeNameChanged();
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
