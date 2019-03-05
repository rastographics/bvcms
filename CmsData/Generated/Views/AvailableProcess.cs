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
    [Table(Name = "AvailableProcess")]
    public partial class AvailableProcess
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _ProcessId;

        private string _ProcessName;

        public AvailableProcess()
        {

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
    }
}
