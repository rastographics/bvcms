using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Models;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles="ManageOrgMembers")]
    [RouteArea("Manage", AreaPrefix= "OrgMembers"), Route("{action=index}/{id?}")]
    public class OrgMembersController : CmsStaffController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var m = new OrgMembersModel();
            m.FetchSavedIds();
            return View(m);
        }

//        [HttpPost]
//        public ActionResult ProcessMove(OrgMembersModel model)
//        {
//		}
        [HttpPost]
        public ActionResult EmailNotices(OrgMembersModel m)
        {
            m.SendMovedNotices();
            return View("List", m);
        }
        public ActionResult GradeList(int id)
        {
            var m = new OrgMembersModel();
            UpdateModel(m);
            return m.ToExcel(id);
        }

        [HttpPost]
        public ActionResult List(OrgMembersModel m)
        {
            m.ValidateIds();
            DbUtil.Db.SetUserPreference("OrgMembersModelIds", $"{m.ProgId}.{m.SourceDivId}.{m.SourceId}");
            DbUtil.DbDispose();
            DbUtil.Db.SetNoLock();
            return View(m);
        }
        [HttpPost]
        public ActionResult ResetMoved(OrgMembersModel m)
        {
            m.ResetMoved();
            return View("List", m);
        }
    }
}
