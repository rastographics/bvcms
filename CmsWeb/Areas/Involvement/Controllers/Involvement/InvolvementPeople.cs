using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Involvement.Models;
using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Involvement.Controllers
{
    public partial class InvolvementController : CmsStaffController
    {
        [HttpPost]
        public ActionResult People(InvolvementPeopleModel m)
        {
            //if (m.FilterIndividuals)
            //{
            //    if (m.NameFilter.HasValue())
            //    {
            //        m.FilterIndividuals = false;
            //    }
            //    else if (CurrentDatabase.OrgFilterCheckedCount(m.QueryId) == 0)
            //    {
            //        m.FilterIndividuals = false;
            //    }
            //}

            //ViewBag.OrgMemberContext = true;
            //ViewBag.orgname = Util.ActiveOrganization;
            return PartialView(m);
        }
    }
}
