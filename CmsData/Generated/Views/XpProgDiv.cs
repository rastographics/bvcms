using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpProgDiv")]
    public partial class XpProgDiv
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _ProgId;

        private int _DivId;

        public XpProgDiv()
        {
        }

        [Column(Name = "ProgId", Storage = "_ProgId", DbType = "int NOT NULL")]
        public int ProgId
        {
            get => _ProgId;

            set
            {
                if (_ProgId != value)
                {
                    _ProgId = value;
                }
            }
        }

        [Column(Name = "DivId", Storage = "_DivId", DbType = "int NOT NULL")]
        public int DivId
        {
            get => _DivId;

            set
            {
                if (_DivId != value)
                {
                    _DivId = value;
                }
            }
        }
    }
}
