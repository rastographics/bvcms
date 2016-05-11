using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Resource
{
    public class Resource
    {
        public int ResourceId { get; set; }
        public ResourceType Type { get; set; }
        public string Name { get; set; }
        public int? OrgId { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }

    public enum ResourceType
    {
        Pdf,
        Image,
        Spreadsheet
    }
}
