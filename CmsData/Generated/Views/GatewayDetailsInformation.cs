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
    [Table(Name = "GatewayDetailsInformation")]
    public partial class GatewayDetailsInformation
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _GatewayId;

        private string _GatewayName;

        private int _GatewayDetailId;

        private string _GatewayDetailName;

        private string _GatewayDetailValue;

        private bool _IsDefault;

        public GatewayDetailsInformation()
        {

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

        [Column(Name = "GatewayDetailId", Storage = "_GatewayDetailId", DbType = "int")]
        public int GatewayDetailId
        {
            get
            {
                return this._GatewayDetailId;
            }

            set
            {
                if (this._GatewayDetailId != value)
                    this._GatewayDetailId = value;
            }
        }

        [Column(Name = "GatewayDetailName", Storage = "_GatewayDetailName", DbType = "nvarchar")]
        public string GatewayDetailName
        {
            get
            {
                return this._GatewayDetailName;
            }

            set
            {
                if (this._GatewayDetailName != value)
                    this._GatewayDetailName = value;
            }
        }

        [Column(Name = "GatewayDetailValue", Storage = "_GatewayDetailValue", DbType = "nvarchar")]
        public string GatewayDetailValue
        {
            get
            {
                return this._GatewayDetailValue;
            }

            set
            {
                if (this._GatewayDetailValue != value)
                    this._GatewayDetailValue = value;
            }
        }

        [Column(Name = "IsDefault", Storage = "_IsDefault", DbType = "bit")]
        public bool IsDefault
        {
            get
            {
                return this._IsDefault;
            }

            set
            {
                if (this._IsDefault != value)
                    this._IsDefault = value;
            }
        }
    }
}
