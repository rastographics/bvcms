using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;
using CmsWeb.Models;
using System.IO;
using CmsData.API;

namespace CmsWeb.Areas.Public.Controllers
{
    public class APIPersonController : CmsController
    {
        [HttpPost]
        public ActionResult Login(string user, string password)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("<Login error=\"{0}\" />".Fmt(ret.Substring(1)));

			var validationStatus = AccountModel.AuthenticateLogon(user, password, Request.Url.OriginalString);
			if (!validationStatus.IsValid)
                return Content("<Login error=\"{0} not valid\">{1}</Login>".Fmt(user ?? "(null)", validationStatus.ErrorMessage));

            var api = new APIFunctions(DbUtil.Db);
            return Content(api.Login(validationStatus.User.Person), "text/xml");
        }

        [HttpGet]
        public ActionResult LoginInfo(int? id)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("<LoginInfo error=\"{0}\" />".Fmt(ret.Substring(1)));
			if (!id.HasValue)
				return Content("<LoginInfo error=\"Missing id\" />");
            var p = DbUtil.Db.People.Single(pp => pp.PeopleId == id);
            var api = new APIFunctions(DbUtil.Db);
            return Content(api.Login(p), "text/xml");
        }
        [HttpPost]
        public ActionResult GetOneTimeLoginLink(string url, string user)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content(url);
			DbUtil.LogActivity("APIPerson GetOneTimeLoginLink {0}, {1}".Fmt(url, user));
            return Content(GetOTLoginLink(url, user));
        }
        public static string GetOTLoginLink(string url, string user)
        {
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = user,
                Expires = DateTime.Now.AddHours(24)
            };
            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();
            return "{0}Logon?ReturnUrl={1}&otltoken={2}".Fmt(
                Util.CmsHost2, 
                HttpUtility.UrlEncode(url), 
                ot.Id.ToCode());
        }
        [HttpPost]
        public ActionResult GetOneTimeRegisterLink(int OrgId, int PeopleId)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("/");
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = "{0},{1},0".Fmt(OrgId, PeopleId),
                Expires = DateTime.Now.AddMinutes(10),
            };
            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();
			DbUtil.LogActivity("APIPerson GetOneTimeRegisterLink {0}, {1}".Fmt(OrgId, PeopleId));
            return Content(Util.CmsHost2 + "OnlineReg/RegisterLink/" + ot.Id.ToCode());
        }
        [HttpGet]
        public ActionResult ExtraValues(int? id, string fields)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("<ExtraValues error=\"{0}\" />".Fmt(ret.Substring(1)));
			if (!id.HasValue)
				return Content("<ExtraValues error=\"Missing id\" />");

			DbUtil.LogActivity("APIPerson ExtraValues {0}, {1}".Fmt(id, fields));
            return Content(new APIFunctions(DbUtil.Db).ExtraValues(id.Value, fields), "text/xml");
        }
        [HttpPost]
        public ActionResult AddEditExtraValue(int peopleid, string field, string value, string type = "data")
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content(ret.Substring(1));
			DbUtil.LogActivity("APIPerson AddExtraValue {0}, {1}".Fmt(peopleid, field));
            return Content(new APIFunctions(DbUtil.Db).AddEditExtraValue(peopleid, field, value, type));
        }
        [HttpPost]
        public ActionResult DeleteExtraValue(int peopleid, string field)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content(ret.Substring(1));
			DbUtil.LogActivity("APIPerson DeleteExtraValue {0}, {1}".Fmt(peopleid, field));
            new APIFunctions(DbUtil.Db).DeleteExtraValue(peopleid, field);
            return Content("ok");
        }
        [HttpPost]
        public ActionResult ChangePassword(string username, string current, string password)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content(ret.Substring(1));
			DbUtil.LogActivity("APIPerson ChangePassword " + username);
            try
            {
                var ok = MembershipService.ChangePassword(username, current, password);
                if (ok)
                    return Content("ok");
                else
                    return Content("<ChangePassword error=\"invalid password\" />");
            }
            catch (Exception ex)
            {
                return Content("<ChangePassword error=\"{0}\" />".Fmt(ex.Message));
            }
        }
        [HttpPost]
        public ActionResult SetPassword(string username, string password)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content(ret.Substring(1));
            var mu = CMSMembershipProvider.provider.GetUser(username, false);
            mu.UnlockUser();
			DbUtil.LogActivity("APIPerson SetPassword " + username);
            try
            {
                if (mu.ChangePassword(mu.ResetPassword(), password))
                    return Content("ok");
                else
                    return Content("<ChangePassword error=\"invalid password\" />");
            }
            catch (Exception ex)
            {
                return Content("<ChangePassword error=\"{0}\" />".Fmt(ex.Message));
            }
        }
        [HttpGet]
        public ActionResult FamilyMembers(int? id)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("<FamilyMembers error=\"{0}\" />".Fmt(ret.Substring(1)));
			if (!id.HasValue)
				return Content("<FamilyMembers error=\"Missing id\" />");
			DbUtil.LogActivity("APIPerson FamilyMembers " + id);
            return Content(new APIFunctions(DbUtil.Db).FamilyMembers(id.Value), "text/xml");
        }
        [HttpGet]
        public ActionResult AccessUsers()
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("<AccessUsers error=\"{0}\" />".Fmt(ret.Substring(1)));
			DbUtil.LogActivity("APIPerson AccessUsers");
            return Content(new APIFunctions(DbUtil.Db).AccessUsersXml(), "text/xml");
        }
        [HttpGet]
        public ActionResult AllUsers()
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("<AccessUsers error=\"{0}\" />".Fmt(ret.Substring(1)));
			DbUtil.LogActivity("APIPerson AccessUsers");
            return Content(new APIFunctions(DbUtil.Db).AccessUsersXml(includeNoAccess:true), "text/xml");
        }
        [HttpGet]
        public ActionResult GetPeople(int? peopleid, int? famid, string first, string last)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("<Person error=\"{0}\" />".Fmt(ret.Substring(1)));
			DbUtil.LogActivity("APIPerson GetPeople");
            return Content(new APIPerson(DbUtil.Db).GetPeopleXml(peopleid, famid, first, last), "text/xml");
        }
        [HttpGet]
        public ActionResult GetPerson(int? id)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("<Person error=\"{0}\" />".Fmt(ret.Substring(1)));
			if (!id.HasValue)
				return Content("<Person error=\"Missing id\" />");
			DbUtil.LogActivity("APIPerson GetPerson " + id);
            return Content(new APIPerson(DbUtil.Db).GetPersonXml(id.Value), "text/xml");
        }
        [HttpPost]
        public ActionResult UpdatePerson()
        {
            var reader = new StreamReader(Request.InputStream);
            string xml = reader.ReadToEnd();
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("<Person error=\"{0}\" />".Fmt(ret.Substring(1)));
			DbUtil.LogActivity("APIPerson Update");
            return Content(new APIPerson(DbUtil.Db).UpdatePersonXml(xml), "text/xml");
        }
        [HttpGet, Route("Portrait/{id:int?}/{w:int?}/{h:int?}")]
        public ActionResult Portrait(int? id, int? w, int? h)
        {
            return new PictureResult(id ?? 0, w, h, portrait: true);
        }
        [HttpGet, Route("FamilyPortrait/{id:int?}/{w:int?}/{h:int?}")]
        public ActionResult FamilyPortrait(int? id, int? w, int? h)
        {
            return new PictureResult(id ?? 0, w, h, portrait: false);
        }
    }
}