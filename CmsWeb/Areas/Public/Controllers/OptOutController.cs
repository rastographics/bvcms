using CmsData;
using CmsWeb.Lifecycle;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
    public class OptOutController : CmsController
    {
        public OptOutController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult UnSubscribe(string optout, string cancel)
        {
            var id = Request["id"];
            var enc = Request["enc"];
            ViewData["id"] = Request["id"];
            ViewData["enc"] = Request["enc"];
            if (!id.HasValue() && !enc.HasValue())
            {
                return Content("unable to identify request");
            }

            string s = null;
            try
            {
                if (id.HasValue())
                {
                    s = Util.Decrypt(id);
                }
                else if (enc.HasValue())
                {
                    s = Util.DecryptFromUrl(enc);
                }
            }
            catch (Exception)
            {
                return Content("unable to identify request");
            }
            if (!(s.HasValue() && s.Contains("|")))
            {
                return Content("Unable to identify request");
            }

            var a = s.SplitStr("|");
            ViewData["fromemail"] = a[1];
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                var p = CurrentDatabase.LoadPersonById(a[0].ToInt());
                if (p == null)
                {
                    return Content("Person not found in database");
                }

                ViewData["toemail"] = p.EmailAddress;
                return View();
            }
            if (optout.HasValue() && optout.StartsWith("Yes"))
            {
                var oo = CurrentDatabase.EmailOptOuts.SingleOrDefault(eo => eo.FromEmail == a[1] && eo.ToPeopleId == a[0].ToInt());
                if (oo == null)
                {
                    oo = new CmsData.EmailOptOut
                    {
                        ToPeopleId = a[0].ToInt(),
                        FromEmail = a[1],
                        DateX = DateTime.Now
                    };
                    CurrentDatabase.EmailOptOuts.InsertOnSubmit(oo);
                    CurrentDatabase.SubmitChanges();
                }
                return View("Confirm");
            }
            return View("Cancel");
        }
    }
}
