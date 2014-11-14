using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
    public class TestMobileController : Controller
    {
        // GET: Public/TestMobile
        public ActionResult GetOneTimeRegisterLink(int orgId, int peopleId)
        {
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = "{0},{1},0".Fmt(orgId, peopleId),
                Expires = DateTime.Now.AddMinutes(10),
            };
            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("APIPerson GetOneTimeRegisterLink {0}, {1}".Fmt(orgId, peopleId));
            return Content(Util.CmsHost2 + "OnlineReg/RegisterLink/" + ot.Id.ToCode() + "?source=ios");
        }
    }
}