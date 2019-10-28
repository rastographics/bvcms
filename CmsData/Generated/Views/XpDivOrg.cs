using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpDivOrg")]
    public partial class XpDivOrg
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _DivId;

        private int _OrgId;

        public XpDivOrg()
        {
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

        [Column(Name = "OrgId", Storage = "_OrgId", DbType = "int NOT NULL")]
        public int OrgId
        {
            get => _OrgId;

            set
            {
                if (_OrgId != value)
                {
                    _OrgId = value;
                }
            }
        }
    }
}
