using System.Collections.Generic;
using System.Linq;

namespace CmsData.Email.ReplacementCodes
{
    public class OrgInfos
    {
        private readonly CMSDataContext db;
        private class OrgInfo
        {
            public string Name { get; set; }
            public string Count { get; set; }
        }

        public OrgInfos(CMSDataContext db)
        {
            this.db = db;
        }

        public string Name(int? id)
        {
            return GetOrgInfo(id).Name;
        }
        public string Count(int? id)
        {
            return GetOrgInfo(id).Count;
        }

        private readonly Dictionary<int, OrgInfo> orginfos = new Dictionary<int, OrgInfo>();

        private OrgInfo GetOrgInfo(int? orgid)
        {
            OrgInfo oi = null;
            var oid = orgid ?? db.CurrentOrgId;

            if (oid.HasValue)
            {
                if (!orginfos.ContainsKey(oid.Value))
                {
                    var q = from i in db.Organizations
                        where i.OrganizationId == oid
                        select new OrgInfo()
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
            return oi ?? new OrgInfo();
        }
    }
}
