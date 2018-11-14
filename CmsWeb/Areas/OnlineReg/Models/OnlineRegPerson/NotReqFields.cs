using CmsData;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        public bool RequiredAddr()
        {
            return org != null
                       ? setting.NotReqAddr == false
                       : settings == null || !settings.Values.Any(o => o.NotReqAddr);
        }
        public bool RequiredCampus()
        {
            return org != null
                       ? setting.NotReqCampus == false
                       : settings == null || !settings.Values.Any(o => o.NotReqCampus);
        }
        public bool ShowCampusOnRegistration => DbUtil.Db.Setting("ShowCampusOnRegistration", "false").ToBool();

        public bool StillNeedBirthday()
        {
            if (RequiredDOB())
            {
                if (!DateOfBirth.HasValue() || byear == null)
                {
                    return true;
                }
            }

            if (NoAppropriateOrgFound())
            {
                return true;
            }

            return false;
        }
        public bool ShowOptionalBirthday()
        {
            return !DateOfBirth.HasValue();
        }

        public bool BirthYearRequired()
        {
            if (ComputesOrganizationByAge())
            {
                return true;
            }

            return org != null
                       ? setting.NoReqBirthYear == false :
                       settings == null || !settings.Values.Any(i => i.NoReqBirthYear);
        }
        public bool RequiredDOB()
        {
            if (ComputesOrganizationByAge())
            {
                return true;
            }

            return org != null
                       ? setting.NotReqDOB == false :
                       settings == null || !settings.Values.Any(i => i.NotReqDOB);
        }

        public bool RequiredZip()
        {
            var req = org != null
                       ? setting.NotReqZip == false
                       : settings == null || !settings.Values.Any(o => o.NotReqZip);
            return req;
        }

        public bool RequiredMarital()
        {
            return org != null
                       ? setting.NotReqMarital == false
                       : settings == null || !settings.Values.Any(o => o.NotReqMarital);
        }

        public bool RequiredGender()
        {
            return org != null
                       ? setting.NotReqGender == false
                       : settings == null || !settings.Values.Any(o => o.NotReqGender);
        }

        public bool RequiredPhone()
        {
            return org != null
                       ? setting.NotReqPhone == false
                       : settings == null || !settings.Values.Any(o => o.NotReqPhone);
        }
    }
}
