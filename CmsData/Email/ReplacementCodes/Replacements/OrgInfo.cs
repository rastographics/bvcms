using System.Collections.Generic;
using System.Linq;

namespace CmsData.Email.ReplacementCodes
{
    public class OrgInfo
    {
        private readonly CMSDataContext db;
        private class NameCount
        {
            public string Name { get; set; }
            public string Count { get; set; }
        }

        public OrgInfo(CMSDataContext db)
        {
            this.db = db;
        }

        public string Name(int? id) => GetOrgInfo(id).Name;
        public string Count(int? id) => GetOrgInfo(id).Count;

        private readonly Dictionary<int, NameCount> orginfos = new Dictionary<int, NameCount>();

        private NameCount GetOrgInfo(int? orgid)
        {
            NameCount oi = null;
            var oid = orgid ?? db.CurrentOrgId;

            if (oid.HasValue)
            {
                if (!orginfos.ContainsKey(oid.Value))
                {
                    var q = from i in db.Organizations
                        where i.OrganizationId == oid
                        select new NameCount()
                        {
                            Name = i.OrganizationName,
                            Count = i.OrganizationMembers.Count().ToString()
                        };
                    oi = q.SingleOrDefault();
                    orginfos.Add(oid.Value, oi);
                }
                else
                    oi = orginfos[oid.Value];
            }
            return oi ?? new NameCount();
        }
    }
}
