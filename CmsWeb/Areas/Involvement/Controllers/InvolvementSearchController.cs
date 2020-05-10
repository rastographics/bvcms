using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Code;
using CmsWeb.Lifecycle;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [SessionExpire]
    [RouteArea("Involvement", AreaPrefix = "InvolvementSearch"), Route("{action=index}/{id?}")]
    public class InvolvementSearchController : CmsStaffController
    {
        private const string STR_InvolvementSearch = "InvolvementSearch";

        public InvolvementSearchController(IRequestManager requestManager) : base(requestManager)
        {
        }

        // GET: Involvement/InvolvementSearch
        public ActionResult Index()
        {
            return View();
        }
    }
}
