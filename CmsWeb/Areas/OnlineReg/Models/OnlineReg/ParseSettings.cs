using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;
using CmsData.Registration;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        public void ParseSettings()
        {
            var list = new Dictionary<int, Settings>();
            if (masterorgid.HasValue)
            {
                var q = from o in UserSelectClasses(masterorg)
                        select new { o.OrganizationId, o.RegSetting };
                foreach (var i in q)
                    list[i.OrganizationId] = new Settings(i.RegSetting, DbUtil.Db, i.OrganizationId);
                list[masterorg.OrganizationId] = new Settings(masterorg.RegSetting, DbUtil.Db, masterorg.OrganizationId);
            }
            else if (_orgid == null)
                return;
            else if (org != null)
                list[_orgid.Value] = new Settings(org.RegSetting, DbUtil.Db, _orgid.Value);
            HttpContext.Current.Items["RegSettings"] = list;

            if (org == null || !org.AddToSmallGroupScript.HasValue()) 
                return;

            var script = DbUtil.Db.Content(org.AddToSmallGroupScript);
            if (script == null || !script.Body.HasValue()) 
                return;

            Log("Script:" + org.AddToSmallGroupScript);
            try
            {
                var pe = new PythonEvents(Util.Host, "RegisterEvent", script.Body);
                HttpContext.Current.Items["PythonEvents"] = pe;
            }
            catch (Exception ex)
            {
                Log("PythonError");
                org.AddToExtraData("Python.errors", ex.Message);
                throw;
            }
        }

        public static Settings ParseSetting(string regSetting, int orgId)
        {
            return new Settings(regSetting, DbUtil.Db, orgId);
        }
    }
}
