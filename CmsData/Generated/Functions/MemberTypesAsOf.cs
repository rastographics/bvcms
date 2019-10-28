using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "MemberTypesAsOf")]
    public partial class MemberTypesAsOf
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _MemberTypeId;

        public MemberTypesAsOf()
        {
        }

        [Column(Name = "member_type_id", Storage = "_MemberTypeId", DbType = "int NOT NULL")]
        public int MemberTypeId
        {
            get => _MemberTypeId;

            set
            {
                if (_MemberTypeId != value)
                {
                    _MemberTypeId = value;
                }
            }
        }
    }
}
