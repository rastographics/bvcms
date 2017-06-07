using System.Collections.Generic;
using System.Linq;
using CmsData.Codes;

namespace CmsData.Email.ReplacementCodes
{
    public class OrgInfo
    {
        private readonly CMSDataContext db;
        private class NameCount
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Count { get; set; }
        }

        public OrgInfo(CMSDataContext db)
        {
            this.db = db;
        }

        public string Name(int? id) => GetOrgInfo(id).Name;
        public string Count(int? id) => GetOrgInfo(id).Count;
        public string Id(int? id) => GetOrgInfo(id).Id.ToString();

        private readonly Dictionary<int, NameCount> orginfos = new Dictionary<int, NameCount>();

        private NameCount GetOrgInfo(int? orgid)
        {
            NameCount oi = null;
            var oid = orgid ?? Util2.CurrentOrgId;

            if (oid.HasValue)
            {
                if (!orginfos.ContainsKey(oid.Value))
                {
                    var q = from i in db.Organizations
                        where i.OrganizationId == oid
                        select new NameCount()
                        {
                            Name = i.OrganizationName,
                            Id = i.OrganizationId,
                            Count = i.OrganizationMembers.Count(mm => 
                                new []{MemberTypeCode.Prospect, MemberTypeCode.InActive}
                                .Contains(mm.MemberTypeId) == false).ToString()
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
