using System.Collections.Generic;
using System.Linq;
using CmsData;
using UtilityExtensions;
using System.Text.RegularExpressions;
using CmsData.Registration;
using CmsWeb.Areas.OnlineReg.Models;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        private const string ManagedGivingShellSettingKey = "UX-ManagedGivingShell";

        private Dictionary<int, Settings> _settings;
        public Dictionary<int, Settings> settings
        {
            get
            {
                if (_settings == null)
                    _settings = HttpContext.Items["RegSettings"] as Dictionary<int, Settings>;
                return _settings;
            }
        }

        public void SetHeaders(OnlineRegModel m2)
        {
            Session["gobackurl"] = m2.URL;
            ViewBag.Title = m2.Header;
            SetHeaders2(m2.Orgid ?? m2.masterorgid ?? 0);
        }
        private void SetHeaders2(int id)
        {
            var org = CurrentDatabase.LoadOrganizationById(id);
            var shell = SetAlternativeManagedGivingShell();
            
            if (!shell.HasValue() && (settings == null || !settings.ContainsKey(id)) && org != null)
            {
                var setting = CurrentDatabase.CreateRegistrationSettings(id);
                shell = CurrentDatabase.ContentOfTypeHtml(setting.ShellBs)?.Body;
            }
            if (!shell.HasValue() && settings != null && settings.ContainsKey(id))
                shell = CurrentDatabase.ContentOfTypeHtml(settings[id].ShellBs)?.Body;
            if (!shell.HasValue())
            {
                shell = CurrentDatabase.ContentOfTypeHtml("ShellDefaultBs")?.Body;
                if(!shell.HasValue())
                    shell = CurrentDatabase.ContentOfTypeHtml("DefaultShellBs")?.Body;
            }


            if (shell != null && shell.HasValue())
            {
                shell = shell.Replace("{title}", ViewBag.Title);
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
            var org = CurrentDatabase.LoadOrganizationById(id);
            if (org != null)
            {
                SetHeaders2(id);
                return;
            }

            var shell = SetAlternativeManagedGivingShell();
            if (!shell.HasValue() && (settings == null || !settings.ContainsKey(id)))
            {
                setting = CurrentDatabase.CreateRegistrationSettings(id);
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

        private string SetAlternativeManagedGivingShell()
        {
            var shell = string.Empty;
            var managedGivingShellSettingKey = ManagedGivingShellSettingKey;
            var campus = Session["Campus"]?.ToString(); // campus is only set for managed giving flow.
            if (!string.IsNullOrWhiteSpace(campus))
            {
                managedGivingShellSettingKey = $"{managedGivingShellSettingKey}-{campus.ToUpper()}";
            }
            var alternateShellSetting = CurrentDatabase.Settings.SingleOrDefault(x => x.Id == managedGivingShellSettingKey);
            if (alternateShellSetting != null)
            {
                var alternateShell = CurrentDatabase.Contents.SingleOrDefault(x => x.Name == alternateShellSetting.SettingX);
                if (alternateShell != null)
                {
                    shell = alternateShell.Body;
                }
            }

            return shell;
        }
    }
}
