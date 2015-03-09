using System.Linq;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;
using CmsWeb.Models;
using CmsData.Codes;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix = "NotUsedYet")]
	public class OrgMembersDialogController : CmsStaffController
	{
		public ActionResult Index(int id, bool? inactives, bool? pendings, int? sg)
		{
			var m = new OrgMembersDialogModel
			{
				orgid = id,
				inactives = inactives ?? false,
				pendings = pendings ?? false,
				Pending = pendings ?? false,
				sg = sg,
			};
			return View(m);
		}
		[HttpPost]
		public ActionResult Filter(OrgMembersDialogModel m)
		{
			return View("Rows", m);
		}
//		[HttpPost]
//		public ActionResult Display(int id, int pid)
//		{
//			var om = DbUtil.Db.OrganizationMembers.Single(m => m.PeopleId == pid && m.OrganizationId == id);
//			return View(om);
//		}
		[HttpPost]
		public ActionResult Update(OrgMembersDialogModel m)
		{
			var Db = DbUtil.Db;
			var tag = Db.PopulateTempTag(m.List);
			var q = from om in m.OrgMembers()
					where Db.TagPeople.Any(tt => tt.Id == tag.Id && tt.PeopleId == om.PeopleId)
					select om;
			foreach (var om in q)
			{
				if (m.MemberType == MemberTypeCode.Drop)
					om.Drop(Db);
				else
				{
					if (m.MemberType > 0)
						om.MemberTypeId = m.MemberType;
					if (m.InactiveDate.HasValue)
						om.InactiveDate = m.InactiveDate;
					if (m.EnrollmentDate.HasValue)
						om.EnrollmentDate = m.EnrollmentDate;
					om.Pending = m.Pending;
					if (m.MemTypeOriginal)
					{
						var et = (from e in Db.EnrollmentTransactions
								  where e.PeopleId == om.PeopleId
								  where e.OrganizationId == m.orgid
								  orderby e.TransactionDate
								  select e).First();
						et.MemberTypeId = om.MemberTypeId;
					}
				}
				Db.SubmitChanges();
			}
			return View();
		}
		public string HelpLink(string page)
		{
			return Util.HelpLink("SearchAdd_{0}".Fmt(page));
		}
	}
}
