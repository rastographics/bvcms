using CmsData;
using CmsWeb.Areas.Org.Models;

namespace CmsWeb.Models
{
    public class OrgMoveInfo
    {
        public string OrgName { get; set; }
        public int FromOrgId { get; set; }
        public int PeopleId { get; set; }
        public int ToOrgId { get; set; }
        public string Program { get; set; }
        public string Division { get; set; }
        public OrgSchedule orgSchedule { get; set; }
        public string Tip
        {
            get
            {
                if (orgSchedule == null)
                    return $"{Program} &bull; {Division}";
                var si = new ScheduleInfo(orgSchedule);
                return $"{Program} &bull; {Division} &bull; {si.SchedDay}, {si.Time}";
            }
        }
    }
}
