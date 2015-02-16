using CmsData;
using UtilityExtensions;

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
                    return "{0} &bull; {1}".Fmt(Program, Division);
                var si = new ScheduleInfo(orgSchedule);
                return "{0} &bull; {1} &bull; {2}, {3}".Fmt(Program, Division, si.SchedDay, si.Time);
            }
        }
    }
}