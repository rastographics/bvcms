using CmsData;
using CmsData.Registration;
using System;
using System.Collections.Generic;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        public void ParseSettings()
        {
            var list = new Dictionary<int, Settings>();
            if (masterorgid.HasValue)
            {
                foreach (var o in UserSelectClasses(masterorg))
                {
                    list[o.OrganizationId] = DbUtil.Db.CreateRegistrationSettings(o.OrganizationId);
                }

                list[masterorg.OrganizationId] = DbUtil.Db.CreateRegistrationSettings(masterorg.OrganizationId);
            }
            else if (_orgid == null)
            {
                return;
            }
            else if (org != null)
            {
                list[_orgid.Value] = DbUtil.Db.CreateRegistrationSettings(_orgid.Value);
            }

            HttpContextFactory.Current.Items["RegSettings"] = list;

            if (org == null || !org.AddToSmallGroupScript.HasValue())
            {
                return;
            }

            var script = DbUtil.Db.Content(org.AddToSmallGroupScript);
            if (script == null || !script.Body.HasValue())
            {
                return;
            }

            Log("Script:" + org.AddToSmallGroupScript);
            try
            {
                var pe = new PythonModel(Util.Host, "RegisterEvent", script.Body);
                HttpContextFactory.Current.Items["PythonEvents"] = pe;
            }
            catch (Exception ex)
            {
                Log("PythonError");
                org.AddToExtraText("Python.errors", ex.Message);
                throw;
            }
        }
    }
}
