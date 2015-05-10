using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
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

        public bool RequiredDOB()
        {
            if (ComputesOrganizationByAge())
                return true;
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