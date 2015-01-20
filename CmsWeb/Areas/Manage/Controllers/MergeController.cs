using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Manager, Admin, Manager2")]
    [RouteArea("Manage", AreaPrefix= "Merge"), Route("{action}")]
    public class MergeController : CmsStaffController
    {
        [Route("~/Merge/{peopleid1:int}/{peopleid2:int}")]
       public ActionResult Index(int PeopleId1, int PeopleId2)
        {
            var m = new MergeModel(PeopleId1, PeopleId2);
            if (m.pi.Count != 3)
                if (m.pi.Count == 2)
                    if (m.pi[0].PeopleId != PeopleId1)
                        return Content("peopleid {0} not found".Fmt(PeopleId1));
                    else
                        return Content("peopleid {0} not found".Fmt(PeopleId2));
                else if (m.pi.Count == 1)
                    return Content("neither peopleid found");
            return View(m);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Run(string submit, bool? Delete, MergeModel m)
        {
            var mh = new MergeHistory
            {
                FromId = m.pi[0].PeopleId,
                ToId = m.pi[1].PeopleId,
                FromName = m.pi[0].person.Name,
                ToName = m.pi[1].person.Name,
                WhoId = Util.UserPeopleId,
                WhoName = Util.UserFullName,
                Action = submit + (Delete == true ? " + Delete" : ""),
                Dt = DateTime.Now,
            };
            DbUtil.Db.MergeHistories.InsertOnSubmit(mh);
            if (submit.StartsWith("Merge Fields"))
			{
				DbUtil.LogActivity("Merging Fields from {0} to {1}".Fmt(m.pi[0].PeopleId, m.pi[1].PeopleId));
				m.Update();
			}
        	if (submit == "Merge Fields and Move Related Records")
            {
				DbUtil.LogActivity("Moving records from {0} to {1}".Fmt(m.pi[0].PeopleId, m.pi[1].PeopleId));
                m.Move();
				if (Delete == true)
				{
					DbUtil.LogActivity("Deleting Record during Merge {0} to {1}".Fmt(m.pi[0].PeopleId, m.pi[1].PeopleId));
					m.Delete();
				}
            }
            if (submit == "Toggle Not Duplicate")
            {
                if (m.pi[0].notdup || m.pi[1].notdup)
                {
                    var dups = DbUtil.Db.PeopleExtras.Where(ee => ee.Field == "notdup" && (ee.PeopleId == m.pi[0].PeopleId || ee.PeopleId == m.pi[1].PeopleId));
                    DbUtil.Db.PeopleExtras.DeleteAllOnSubmit(dups);
                }
                else
                {
                    m.pi[0].person.AddEditExtraInt("notdup", m.pi[1].PeopleId);
                    m.pi[1].person.AddEditExtraInt("notdup", m.pi[0].PeopleId);
                }
                DbUtil.Db.SubmitChanges();
                return Redirect("/Merge/{0}/{1}".Fmt(m.pi[0].PeopleId,m.pi[1].PeopleId));
            }
            return Redirect("/Person2/" + m.pi[1].PeopleId);
        }
    }
}
