using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Models;
using UtilityExtensions;
using System.Collections.Generic;
using CmsData.Codes;
using CmsWeb.Models.OrganizationPage;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
	public partial class OnlineRegController
	{

		public ActionResult ManageSubscriptions(string id)
		{
			if (!id.HasValue())
				return Content("bad link");
			ManageSubsModel m;
			var td = TempData["ms"];
			if (td != null)
				m = new ManageSubsModel(td.ToInt(), id.ToInt());
			else
			{
				var guid = id.ToGuid();
				if (guid == null)
					return Content("invalid link");
				var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
				if (ot == null)
					return Content("invalid link");
				if (ot.Used)
					return Content("link used");
				if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
					return Content("link expired");
				var a = ot.Querystring.Split(',');
				m = new ManageSubsModel(a[1].ToInt(), a[0].ToInt());
				id = a[0];
				ot.Used = true;
				DbUtil.Db.SubmitChanges();
			}
			SetHeaders(id.ToInt());
			DbUtil.LogActivity("Manage Subs: {0} ({1})".Fmt(m.Description(), m.person.Name));
			return View(m);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ConfirmSubscriptions(ManageSubsModel m)
		{
			m.UpdateSubscriptions();
			var Staff = DbUtil.Db.StaffPeopleForOrg(m.masterorgid.Value);

		    var msg = DbUtil.Db.ContentHtml("ConfirmSubscriptions", Resource1.ConfirmSubscriptions);
		    msg = msg.Replace("{org}", m.Description()).Replace("{details}", m.Summary);
			DbUtil.Db.Email(Staff.First().FromEmail, m.person, "Subscription Confirmation", msg);

			DbUtil.Db.Email(m.person.FromEmail, Staff, "Subscriptions managed", 
            @"{0} managed subscriptions to {1}<br/>{2}".Fmt(m.person.Name, m.Description(), m.Summary));

			SetHeaders(m.masterorgid.Value);
			return View(m);
		}
	}
}
