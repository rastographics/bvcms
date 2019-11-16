using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgMember")]
    public partial class OrgMember
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _PeopleId;

        public OrgMember()
        {
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    _PeopleId = value;
                }
            }
        }
    }
}
