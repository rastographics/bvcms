using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models.Person;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [POST("Person2/Changes/{id:int}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Changes(int id, int? page, int? size, string sort, string dir)
        {
            var m = new ChangesModel(id);
            m.Pager.Set("/Person2/Changes/" + id, page, size, sort, dir);
            return View("System/Changes", m);
        }
        [POST("Person2/Reverse/{id:int}")]
        public ActionResult Reverse(int id, string field, string value, string pf)
        {
            var m = new ChangesModel(id);
            m.Reverse(field, value, pf);
            return View("System/Changes", m);
        }
        [POST("Person2/Optouts/{id:int}")]
        public ActionResult Optouts(int id)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            return View("System/Optouts", p);
        }
        [POST("Person2/Duplicates/{id:int}")]
        public ActionResult Duplicates(int id)
        {
            var m = new DuplicatesModel(id);
            return View("System/Duplicates", m);
        }
        [POST("Person2/DeleteOptout/{id:int}")]
        public ActionResult DeleteOptout(int id, string email)
        {
            var oo = DbUtil.Db.EmailOptOuts.SingleOrDefault(o => o.FromEmail == email && o.ToPeopleId == id);
            if (oo == null)
                return Content("not found");
            DbUtil.Db.EmailOptOuts.DeleteOnSubmit(oo);
            DbUtil.Db.SubmitChanges();
            var p = DbUtil.Db.LoadPersonById(id);
            return View("System/Optouts", p);
        }
        [POST("Person2/AddOptout/{id:int}")]
		public ActionResult AddOptout(int id, string email)
		{
			var oo = DbUtil.Db.EmailOptOuts.SingleOrDefault(o => o.FromEmail == email && o.ToPeopleId == id);
		    if (oo == null)
		    {
                DbUtil.Db.EmailOptOuts.InsertOnSubmit(new EmailOptOut { FromEmail = email, ToPeopleId = id, DateX = DateTime.Now });
		        DbUtil.Db.SubmitChanges();
		    }
            var p = DbUtil.Db.LoadPersonById(id);
            return View("System/Optouts", p);
		}

        [POST("Person2/FailedEmails/{id:int}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult FailedEmails(int id, int? page, int? size, string sort, string dir)
        {
            var m = new FailedMailModel(id);
            m.Pager.Set("/Person2/FailedEmails/" + id, page, size, sort, dir);
            return View("System/FailedEmails", m);
        }
		[POST("Person2/EmailUnblock"), Authorize(Roles = "Admin")]
		public ActionResult Unblock(string email)
		{
			var deletebounce = ConfigurationManager.AppSettings["DeleteBounce"] + email;
			var wc = new WebClient();
			var ret = wc.DownloadString(deletebounce);
			return Content(ret);
		}
		[POST("Person2/EmailUnspam"), Authorize(Roles = "Developer")]
		public ActionResult Unspam(string email)
		{
			var deletespam = ConfigurationManager.AppSettings["DeleteSpamReport"] + email;
			var wc = new WebClient();
			var ret = wc.DownloadString(deletespam);
			return Content(ret);
		}
    }
}
