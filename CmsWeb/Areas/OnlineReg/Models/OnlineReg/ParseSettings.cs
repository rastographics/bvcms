using CmsData;
using CmsData.Registration;
using System;
using System.Collections.Generic;
using System.Web;
using UtilityExtensions;
using System.Linq;
using CmsWeb.Models;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        public void ParseSettings(CMSDataContext db)
        {
            var list = new Dictionary<int, Settings>();

            if (masterorgid.HasValue)
            {
                foreach (var o in UserSelectClasses(masterorg))
                {
                    list[o.OrganizationId] = db.CreateRegistrationSettings(o.OrganizationId);
                }

                list[masterorg.OrganizationId] = db.CreateRegistrationSettings(masterorg.OrganizationId);
            }
            else if (_orgid == null)
            {
                return;
            }
            else if (org != null)
            {
                list[_orgid.Value] = db.CreateRegistrationSettings(_orgid.Value);
            }

            HttpContextFactory.Current.Items["RegSettings"] = list;

            if (org == null || !org.AddToSmallGroupScript.HasValue())
            {
                return;
            }

            var script = db.Content(org.AddToSmallGroupScript);
            if (script == null || !script.Body.HasValue())
            {
                return;
            }

            Log("Script:" + org.AddToSmallGroupScript);
            try
            {
                var pe = new PythonModel(CurrentDatabase.Host, "RegisterEvent", script.Body);
                HttpContextFactory.Current.Items["PythonEvents"] = pe;
            }
            catch (Exception ex)
            {
                Log("PythonError");
                org.AddToExtraText("Python.errors", ex.Message);
                throw;
            }
        }

        public void ParseMasterSettings()
        {
            var list = new Dictionary<int, Settings>();
            var db = CMSDataContext.Create(HttpContextFactory.Current);

            if (masterorgid.HasValue)
            {
                foreach (var o in UserSelectClasses(masterorg))
                {
                    list[o.OrganizationId] = db.CreateRegistrationSettings(o.OrganizationId);
                }

                list[masterorg.OrganizationId] = db.CreateRegistrationSettings(masterorg.OrganizationId);
            }
            else if (_orgid == null)
            {
                return;
            }
            else if (org != null)
            {
                int MasterOrId = db.Organizations.Where(x => x.OrgPickList.Contains(_orgid.ToString())).Select(y => y.OrganizationId).First();
                list[MasterOrId] = db.CreateRegistrationSettings(MasterOrId);
            }

            HttpContextFactory.Current.Items["RegMasterSettings"] = list;
        }
    }
}
