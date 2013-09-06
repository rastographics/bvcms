using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;
using System.Net.Mail;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Manager, Admin, Manager2")]
    public class MergeController : CmsStaffController
    {
       public ActionResult Index(int? PeopleId1, int? PeopleId2)
        {
            if (!PeopleId1.HasValue || !PeopleId2.HasValue)
                return Content("need PeopleId1 and PeopleId2");
            var m = new MergeModel(PeopleId1.Value, PeopleId2.Value);
            if (m.pi.Count != 3)
                if (m.pi.Count == 2)
                    if (m.pi[0].PeopleId != PeopleId1.Value)
                        return Content("peopleid {0} not found".Fmt(PeopleId1.Value));
                    else
                        return Content("peopleid {0} not found".Fmt(PeopleId2.Value));
                else if (m.pi.Count == 1)
                    return Content("neither peopleid found");
            return View(m);
        }
        [HttpPost]
        public ActionResult Run(string submit, bool? Delete, MergeModel m)
        {
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
                return Redirect("/Merge/Index?PeopleId1={0}&PeopleId2={1}".Fmt(m.pi[0].PeopleId,m.pi[1].PeopleId));
            }
            return Redirect("/Person/Index/" + m.pi[1].PeopleId);
        }
    }
}
