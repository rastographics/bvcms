using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.CheckIn.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("CheckIn", AreaPrefix = "WebCheckinLabels"), Route("{action}")]
    public class WebCheckinLabelsController : CMSBaseController
    {
        public WebCheckinLabelsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/WebCheckinLabels")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save(List<int> active)
        {
            try
            {
                var inactiveLabels = CurrentDatabase.CheckInLabels.Where(l => !active.Contains(l.Id)).ToList();
                foreach (var label in inactiveLabels)
                {
                    label.Active = false;
                }
                var activeLabels = CurrentDatabase.CheckInLabels.Where(l => active.Contains(l.Id)).ToList();
                foreach (var label in activeLabels)
                {
                    label.Active = true;
                }
                CurrentDatabase.SubmitChanges();
            } catch (Exception e)
            {
                return Json(new
                {
                    Status = "error",
                    Message = e.Message
                });
            }
            return Json(new
            {
                Status = "success"
            });
        }
    }
}
