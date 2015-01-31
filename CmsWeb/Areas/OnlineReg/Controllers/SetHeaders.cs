using CmsData;
using CmsData.Registration;
using UtilityExtensions;
using System.Text.RegularExpressions;
using CmsWeb.Models;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        private void SetHeaders(OnlineRegModel m2)
        {
            ViewBag.Url = m2.URL;
            Session["gobackurl"] = m2.URL;
            if(m2.UseBootstrap)
                SetHeaders2(m2.Orgid ?? m2.masterorgid ?? 0);
            else
                SetHeaders(m2.Orgid ?? m2.masterorgid ?? 0);
        }
        private void SetHeaders2(int id)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            var shell = "";
            if ((settings == null || !settings.ContainsKey(id)) && org != null)
            {
                var setting = OnlineRegModel.ParseSetting(org.RegSetting, id);
                shell = DbUtil.Content(setting.ShellBs, null);
            }
            if (!shell.HasValue() && settings != null && settings.ContainsKey(id))
                shell = DbUtil.Content(settings[id].ShellBs, null);
            if (!shell.HasValue())
            {
                shell = DbUtil.Content("ShellDefaultBs", "");
                if(!shell.HasValue())
                    shell = DbUtil.Content("DefaultShellBs", "");
            }


            if (shell.HasValue())
            {
                var re = new Regex(@"(.*<!--FORM START-->\s*).*(<!--FORM END-->.*)", RegexOptions.Singleline);
                var t = re.Match(shell).Groups[1].Value.Replace("<!--FORM CSS-->", ViewExtensions2.Bootstrap3Css());
                ViewBag.hasshell = true;
                ViewBag.top = t;
                var b = re.Match(shell).Groups[2].Value;
                ViewBag.bottom = b;
            }
            else
                ViewBag.hasshell = false;
        }
        private void SetHeaders(int id)
        {
            Settings setting = null;
            var org = DbUtil.Db.LoadOrganizationById(id);
            if (org != null && (org.UseBootstrap ?? true))
            {
                SetHeaders2(id);
                return;
            }
            var shell = "";
            if ((settings == null || !settings.ContainsKey(id)) && org != null)
            {
                setting = OnlineRegModel.ParseSetting(org.RegSetting, id);
                shell = DbUtil.Content(setting.Shell, null);
            }
            if (!shell.HasValue() && settings != null && settings.ContainsKey(id))
            {
                shell = DbUtil.Content(settings[id].Shell, null);
            }
            if (!shell.HasValue())
                shell = DbUtil.Content("ShellDiv-" + id, DbUtil.Content("ShellDefault", ""));

            var s = shell;
            if (s.HasValue())
            {
                var re = new Regex(@"(.*<!--FORM START-->\s*).*(<!--FORM END-->.*)", RegexOptions.Singleline);
                var t = re.Match(s).Groups[1].Value.Replace("<!--FORM CSS-->", 
                ViewExtensions2.jQueryUICss() +
                "\r\n<link href=\"/Content/styles/onlinereg.css?v=8\" rel=\"stylesheet\" type=\"text/css\" />\r\n"); 
                ViewBag.hasshell = true;
                ViewBag.top = t;
                var b = re.Match(s).Groups[2].Value;
                ViewBag.bottom = b;
            }
            else
            {
                ViewBag.hasshell = false;
                ViewBag.header = DbUtil.Content("OnlineRegHeader-" + id,
                    DbUtil.Content("OnlineRegHeader", ""));
                ViewBag.top = DbUtil.Content("OnlineRegTop-" + id,
                    DbUtil.Content("OnlineRegTop", ""));
                ViewBag.bottom = DbUtil.Content("OnlineRegBottom-" + id,
                    DbUtil.Content("OnlineRegBottom", ""));
            }
        }
        
    }
}