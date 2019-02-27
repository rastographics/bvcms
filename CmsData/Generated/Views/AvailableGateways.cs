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
    [Table(Name = "AvailableGateways")]
    public partial class AvailableGateways
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _ProcessId;

        private string _ProcessName;

        public AvailableGateways()
        {

        }

        [Column(Name = "ProcessId", Storage = "ProcessId", DbType = "int")]
        public int ProcessId
        {
            get
            {
                return this.ProcessId;
            }

            set
            {
                if (this._ProcessId != value)
                    this._ProcessId = value;
            }
        }

        [Column(Name = "ProcessName", Storage = "_ProcessName", DbType = "nvarchar")]
        public string ProcessName
        {
            get
            {
                return this.ProcessName;
            }

            set
            {
                if (this._ProcessName != value)
                    this._ProcessName = value;
            }
        }
    }
}
