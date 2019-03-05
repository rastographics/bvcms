using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData.View
{
    [Table(Name = "MyGatewaySettings")]
    public partial class MyGatewaySettings
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _GatewaySettingId;

        private string _ProcessName;

        private int _ProcessId;

        private string _GatewayName;

        private int _GatewayId;

        public MyGatewaySettings()
        {

        }

        [Column(Name = "GatewaySettingId", Storage = "_GatewaySettingId", DbType = "int")]
        public int GatewaySettingId
        {
            get
            {
                return this._GatewaySettingId;
            }

            set
            {
                if (this._GatewaySettingId != value)
                    this._GatewaySettingId = value;
            }
        }

        [Column(Name = "ProcessName", Storage = "_ProcessName", DbType = "nvarchar")]
        public string ProcessName
        {
            get
            {
                return this._ProcessName;
            }

            set
            {
                if (this._ProcessName != value)
                    this._ProcessName = value;
            }
        }

        [Column(Name = "ProcessId", Storage = "_ProcessId", DbType = "int")]
        public int ProcessId
        {
            get
            {
                return this._ProcessId;
            }

            set
            {
                if (this._ProcessId != value)
                    this._ProcessId = value;
            }
        }

        [Column(Name = "GatewayName", Storage = "_GatewayName", DbType = "nvarchar")]
        public string GatewayName
        {
            get
            {
                return this._GatewayName;
            }

            set
            {
                if (this._GatewayName != value)
                    this._GatewayName = value;
            }
        }

        [Column(Name = "GatewayId", Storage = "_GatewayId", DbType = "int")]
        public int GatewayId
        {
            get
            {
                return this._GatewayId;
            }

            set
            {
                if (this._GatewayId != value)
                    this._GatewayId = value;
            }
        }
    }
}
