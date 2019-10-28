using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.GatewayServiceType")]
    public partial class GatewayServiceType : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._GatewayServiceTypeId;

            set
            {
                if (this._GatewayServiceTypeId != value)
                {
                    this.OnGatewayServiceTypeIdChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayServiceTypeId = value;
                    this.SendPropertyChanged("GatewayServiceTypeId");
                    this.OnGatewayServiceTypeIdChanged();
                }

            }

        }

        [Column(Name = "GatewayServiceTypeName", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayServiceTypeName", DbType = "nvarchar")]
        public string GatewayServiceTypeName
        {
            get => this._GatewayServiceTypeName;

            set
            {
                if (this._GatewayServiceTypeName != value)
                {
                    this.OnGatewayServiceTypeNameChanging(value);
                    this.SendPropertyChanging();
                    this._GatewayServiceTypeName = value;
                    this.SendPropertyChanged("GatewayServiceTypeName");
                    this.OnGatewayServiceTypeNameChanged();
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
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
