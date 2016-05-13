using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Resource
{
    public class Resource
    {
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? OrgId { get; set; }
        public int? CampusId { get; set; }
        public string MemberTypes { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public List<Attachment> Attachments { get; set; }
    }
}



