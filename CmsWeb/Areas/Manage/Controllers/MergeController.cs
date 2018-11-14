using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System.Web.Mvc;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Manager, Admin, Manager2")]
    [RouteArea("Manage", AreaPrefix = "Merge"), Route("{action}")]
    public class MergeController : CmsStaffController
    {
        public MergeController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Merge/{peopleid1:int}/{peopleid2:int}")]
        public ActionResult Index(int peopleId1, int peopleId2)
        {
            var m = new MergeModel(peopleId1, peopleId2) { DeleteDuplicate = true };
            if (m.pi.Count != 3)
            {
                if (m.pi.Count == 2)
                {
                    if (m.pi[0].PeopleId != peopleId1)
                    {
                        return Content($"peopleid {peopleId1} not found");
                    }
                    else
                    {
                        return Content($"peopleid {peopleId2} not found");
                    }
                }
                else if (m.pi.Count == 1)
                {
                    return Content("neither peopleid found");
                }
            }

            return View(m);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MergeFields(MergeModel m)
        {
            m.LogMerge("Merge Fields");
            m.Update();
            return Redirect("/Person2/" + m.pi[1].PeopleId);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MergeFieldsAndMoveRelated(MergeModel m)
        {
            m.LogMerge("Merge and move related");
            m.Update();
            m.Move();
            return Redirect("/Person2/" + m.pi[1].PeopleId);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ToggleNotDuplicate(MergeModel m)
        {
            m.LogMerge("Toggle Not Duplicate");
            m.ToggleNotDuplicate();
            return Redirect($"/Merge/{m.pi[0].PeopleId}/{m.pi[1].PeopleId}");
        }
    }
}
