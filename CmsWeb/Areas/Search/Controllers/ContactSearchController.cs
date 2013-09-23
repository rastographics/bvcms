using System;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Code;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaUrl = "ContactSearch2")]
    public class ContactSearchController : CmsStaffController
    {
        private const string STR_ContactSearch = "ContactSearch2";

        [GET("ContactSearch2")]
        public ActionResult Index()
        {
            Response.NoCache();
            var m = new ContactSearchModel();
            m.Pager.Set("/ContactSearch2/Results");

            var os = Session[STR_ContactSearch] as ContactSearchInfo;
            if (os != null)
                m.CopyPropertiesFrom(os);
            return View(m);
        }
        [POST("ContactSearch2/Results/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Results(int? page, int? size, string sort, string dir, ContactSearchModel m)
        {
            m.Pager.Set("/ContactSearch2/Results", page, size, sort, dir);
            SaveToSession(m);
            return View(m);
        }
        [POST("ContactSearch2/Clear")]
        public ActionResult Clear()
        {
            var m = new ContactSearchModel();
            Session.Remove(STR_ContactSearch);
            return Redirect("/ContactSearch2");
        }
        private void SaveToSession(ContactSearchModel m)
        {
            var os = new ContactSearchInfo();
            m.CopyPropertiesTo(os);
            Session[STR_ContactSearch] = os;
        }
        [POST("ContactSearch2/ConvertToQuery")]
        public ActionResult ConvertToQuery(ContactSearchModel m)
        {
            var c = DbUtil.Db.ScratchPadCondition();
            c.Reset(DbUtil.Db);
            var clause = c.AddNewClause(QueryType.MadeContactTypeAsOf, CompareType.Equal, "1,T");
            clause.Program = m.SearchParameters.Ministry.Value.ToInt();
            clause.StartDate = m.SearchParameters.StartDate ?? DateTime.Parse("1/1/2000");
            clause.EndDate = m.SearchParameters.EndDate ?? DateTime.Today;
            var cvc = new CodeValueModel();
            var q = from v in cvc.ContactTypeList()
                    where v.Id == m.SearchParameters.ContactType.Value.ToInt()
                    select v.IdCode;
            var idvalue = q.Single();
            clause.CodeIdValue = idvalue;
            c.Save(DbUtil.Db);
            return Redirect("/Query/{0}".Fmt(c.Id));
        }
        [POST("ContactSearch2/ContactTypeQuery/{id:int}")]
        public ActionResult ContactTypeQuery(int id)
        {
            var c = DbUtil.Db.ScratchPadCondition();
            c.Reset(DbUtil.Db);
            var comp = CompareType.Equal;
            var clause = c.AddNewClause(QueryType.RecentContactType, comp, "1,T");
            clause.Days = 10000;
            var cvc = new CodeValueModel();
            var q = from v in cvc.ContactTypeList()
                    where v.Id == id
                    select v.IdCode;
            clause.CodeIdValue = q.Single();
            c.Save(DbUtil.Db);
            return Redirect("/Query/{0}".Fmt(c.Id));
        }
        [POST("ContactSearch2/ContactorSummary")]
        public ActionResult ContactorSummary(ContactSearchModel m)
        {
            int ministryid = m.SearchParameters.Ministry.Value.ToInt();
            var q = from c in DbUtil.Db.Contactors
                    where c.contact.ContactDate >= m.SearchParameters.StartDate
                    where c.contact.ContactDate <= m.SearchParameters.EndDate
                    where ministryid == 0 || ministryid == c.contact.MinistryId
                    group c by new
                    {
                        c.PeopleId,
                        c.person.Name,
                        c.contact.ContactType.Description,
                        c.contact.MinistryId,
                        c.contact.Ministry.MinistryName
                    } into g
                    where g.Key.MinistryId != null
                    orderby g.Key.MinistryId
                    select new
                    {
                        g.Key.PeopleId,
                        g.Key.Name,
                        g.Key.Description,
                        g.Key.MinistryName,
                        cnt = g.Count()
                    };
            return new DataGridResult(q);
        }
        [POST("ContactSearch2/ContactSummary")]
        public ActionResult ContactSummary(ContactSearchModel m)
        {
            var q = DbUtil.Db.ContactSummary(
                m.SearchParameters.StartDate, 
                m.SearchParameters.EndDate, 
                m.SearchParameters.Ministry.Value.ToInt(), 
                m.SearchParameters.ContactType.Value.ToInt(), 
                m.SearchParameters.ContactReason.Value.ToInt());
            var q2 = from i in q
                     select new
                         {
                             Count = i.Count ?? 0,
                             i.ContactType,
                             i.ReasonType,
                             i.Ministry,
                             HasComments = i.Comments,
                             HasDate = i.ContactDate,
                             HasContactor = i.Contactor,
                         };
            return new DataGridResult(q2);
        }

        [POST("ContactSearch2/ContactTypeTotals")]
        public ActionResult ContactTypeTotals(ContactSearchModel m)
        {
            var q = from c in DbUtil.Db.ContactTypeTotals(m.SearchParameters.StartDate, m.SearchParameters.EndDate, m.SearchParameters.Ministry.Value.ToInt())
                    orderby c.Count descending
                    select c;
            ViewBag.candelete = User.IsInRole("Developer") 
                && !m.SearchParameters.StartDate.HasValue 
                && !m.SearchParameters.EndDate.HasValue 
                && m.SearchParameters.Ministry.Value.ToInt() == 0;
            return View(q);
        }

        [Authorize(Roles = "Developer")]
        public ActionResult DeleteContactsForType(int id)
        {
            DbUtil.Db.ExecuteCommand("DELETE dbo.Contactees FROM dbo.Contactees ce JOIN dbo.Contact c ON ce.ContactId = c.ContactId WHERE c.ContactTypeId = {0}", id);
            DbUtil.Db.ExecuteCommand("DELETE dbo.Contactors FROM dbo.Contactors co JOIN dbo.Contact c ON co.ContactId = c.ContactId WHERE c.ContactTypeId = {0}", id);
            DbUtil.Db.ExecuteCommand("DELETE dbo.Contact WHERE ContactTypeId = {0}", id);
            return Redirect("/ContactSearch/ContactTypeTotals");
        }
    }
}
