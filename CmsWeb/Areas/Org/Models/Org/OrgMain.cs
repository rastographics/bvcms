using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgMain
    {
        public Organization Org;
        public int Id
        {
            get { return Org != null ? Org.OrganizationId : 0; }
            set
            {
                if (Org == null)
                    Org = DbUtil.Db.LoadOrganizationById(value);
            }
        }

        public OrgMain(Organization org)
        {
            Org = org;
            this.CopyPropertiesFrom(Org);
        }

        public OrgMain()
        {

        }
        public int? LeaderId { get { return Org.LeaderId; } }
        public string LeaderName { get { return Org.LeaderName; } }

        public string OrganizationName { get; set; }
        [SettingDisplayName("Campus")]
        public CodeInfo Campus { get; set; }
        public CodeInfo LeaderMemberType { get; set; }
        public CodeInfo OrganizationType { get; set; }
        public CodeInfo SecurityType { get; set; }
        public CodeInfo OrganizationStatus { get; set; }
        [DisplayName("Main Fellowship")]
        public bool IsBibleFellowshipOrg { get; set; }

        private string _schedule;
        public string Schedule
        {
            get
            {
                if (_schedule.HasValue()) 
                    return _schedule;
                var sch = (from sc in DbUtil.Db.OrgSchedules
                    where sc.OrganizationId == Id
                    orderby sc.Id
                    select sc).FirstOrDefault();
                return _schedule = sch == null ? "None" : (new ScheduleInfo(sch)).Display;
            }
        }

        public void Update()
        {
            this.CopyPropertiesTo(Org);
            DbUtil.Db.SubmitChanges();
        }
    }
}