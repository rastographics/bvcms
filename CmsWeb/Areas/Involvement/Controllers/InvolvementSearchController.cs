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

        [Route("~/InvolvementSearch/{progid:int?}/{div:int?}")]
        public ActionResult Index(int? div, int? progid, int? onlinereg, string name)
        {
            Response.NoCache();
            var m = new OrgSearchModel(CurrentDatabase);
            m.StatusId = OrgStatusCode.Active;

            if (name.HasValue())
            {
                m.Name = name;
                m.StatusId = null;
            }
            if (onlinereg.HasValue)
            {
                m.OnlineReg = onlinereg;
            }

            if (div.HasValue)
            {
                m.DivisionId = div;
                if (progid.HasValue)
                {
                    m.ProgramId = progid;
                }
                else
                {
                    m.ProgramId = m.Division().ProgId;
                }

                m.TagProgramId = m.ProgramId;
                m.TagDiv = div;
            }
            else if (progid.HasValue)
            {
                m.ProgramId = progid;
                m.TagProgramId = m.ProgramId;
            }
            else if (Util.OrgSearch.IsNotNull())
            {
                // TODO: get this working...
                //var search = JsonConvert.DeserializeObject<OrgSearchInfo>(Util.OrgSearch);
                //search.CopyPropertiesTo(m);
            }

            return View(m);
        }
    }
}
