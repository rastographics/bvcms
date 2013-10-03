using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Configuration;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [POST("Person2/Optouts/{id:int}")]
        public ActionResult Optouts(int id)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            return View("Emails/Optouts", p);
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
            return View("Emails/Optouts", p);
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
            return View("Emails/Optouts", p);
		}

        [POST("Person2/FailedEmails/{id:int}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult FailedEmails(int id, int? page, int? size, string sort, string dir)
        {
            var m = new FailedMailModel(id);
            m.Pager.Set("/Person2/FailedEmails/" + id, page, size, sort, dir);
            return View("Emails/Failed", m);
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
        [POST("Person2/ReceivedEmails/{id:int}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult ReceivedEmails(int id, int? page, int? size, string sort, string dir)
        {
            var m = new EmailReceivedModel(id);
            m.Pager.Set("/Person2/ReceivedEmails/" + id, page, size, sort, dir);
            return View("Emails/Emails", m);
        }
        [POST("Person2/SentEmails/{id:int}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult SentEmails(int id, int? page, int? size, string sort, string dir)
        {
            var m = new EmailSentModel(id);
            m.Pager.Set("/Person2/SentEmails/" + id, page, size, sort, dir);
            return View("Emails/Emails", m);
        }
        [POST("Person2/TransactionalEmails/{id:int}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult TransactionalEmails(int id, int? page, int? size, string sort, string dir)
        {
            var m = new EmailTransactionalModel(id);
            m.Pager.Set("/Person2/TransactionalEmails/" + id, page, size, sort, dir);
            return View("Emails/Emails", m);
        }
        [GET("Person2/ViewEmail/{emailid:int}")]
		public ActionResult ViewEmail(int emailid)
        {
            var q = from e in DbUtil.Db.EmailQueues
                where e.Id == emailid
                let isAdmin = User.IsInRole("Admin")
    			let isPublic = (e.PublicX ?? false)
                let p = DbUtil.Db.People.Single(pp => pp.PeopleId == Util.UserPeopleId)
                let isSender = e.QueuedBy == Util.UserPeopleId
                               || (e.FromAddr == p.EmailAddress && p.EmailAddress.Length > 0)
                               || (e.FromAddr == p.EmailAddress2 && p.EmailAddress2.Length > 0)
                let isReceiver = e.EmailQueueTos.Any(ee => ee.PeopleId == Util.UserPeopleId)
                select new {e, isAdmin, isSender, isPublic };

            var i = q.SingleOrDefault();
			if (i == null)
				return Content("email document not found");

			var em = new EmailQueue
			{
				Subject = i.e.Subject,
				Body = i.e.Body.Replace("{track}", "", ignoreCase: true).Replace("{first}", "", ignoreCase: true)
			};
			return View("Emails/View", em);
		}
    }
}
