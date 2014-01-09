using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;
using System.Text;
using CmsWeb.Models.PersonPage;
using CmsWeb.Models;
using System.Diagnostics;
using System.Web.Routing;
using System.Threading;

namespace CmsWeb.Areas.Main.Controllers
{
    [ValidateInput(false)]
    public class FamilyController : CmsStaffController
    {
        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
                return Content("no id");
            var m = new FamilyModel { familyid = id.Value };
			if (m.Family == null)
				return Content("no family");
            return View(m);
        }
        public ActionResult QuerySearch(int? id)
        {
            if (!id.HasValue)
                return Content("no id");
            var qb = DbUtil.Db.ScratchPadCondition();
            qb.Reset(DbUtil.Db);
            var comp = CompareType.Equal;
            var clause = qb.AddNewClause(QueryType.FamilyId, comp, id);
            qb.Save(DbUtil.Db);
            return Redirect("/QueryBuilder2/Main/{0}".Fmt(qb.Id));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult EditPosition(string id, string value)
        {
            var pid = id.Split('.')[1].ToInt();
            var p = DbUtil.Db.LoadPersonById(pid);
            p.PositionInFamilyId = value.ToInt();
            DbUtil.Db.SubmitChanges();
            return Content(p.FamilyPosition.Description);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult EditRelation(string id, string value)
        {
            var a = id.Split('.');
            var r = DbUtil.Db.RelatedFamilies.SingleOrDefault(m => m.FamilyId == a[1].ToInt() && m.RelatedFamilyId == a[2].ToInt());
            if (a[0] == "e")
                r.FamilyRelationshipDesc = value;
            DbUtil.Db.SubmitChanges();
            var c = new ContentResult();
            c.Content = value;
            return c;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult DeleteRelation(string id)
        {
            var a = id.Split('.');
            var r = DbUtil.Db.RelatedFamilies.SingleOrDefault(m => m.FamilyId == a[1].ToInt() && m.RelatedFamilyId == a[2].ToInt());
            DbUtil.Db.RelatedFamilies.DeleteOnSubmit(r);
            DbUtil.Db.SubmitChanges();
            return Content(a[3]);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult Split(int id)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            var f = new Family
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                AddressLineOne = p.PrimaryAddress,
                AddressLineTwo = p.PrimaryAddress2,
                CityName = p.PrimaryCity,
                StateCode = p.PrimaryState,
                ZipCode = p.PrimaryZip,
                HomePhone = p.Family.HomePhone
            };
            f.People.Add(p);
            DbUtil.Db.Families.InsertOnSubmit(f);
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Splitting Family for {0}".Fmt(p.Name));
            if (p == null)
                return Content("/");
            return Content("/Person/Index/" + id);
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PositionCodes()
        {
            var q = from c in DbUtil.Db.FamilyPositions
                    select new
                    {
                        Code = c.Id.ToString(),
                        Value = c.Description,
                    };
            return Json(q.ToDictionary(k => k.Code, v => v.Value));
        }

		[HttpPost]
		public ContentResult DeleteExtra(int id, string field)
		{
			var e = DbUtil.Db.FamilyExtras.First(ee => ee.FamilyId == id && ee.Field == field);
			DbUtil.Db.FamilyExtras.DeleteOnSubmit(e);
			DbUtil.Db.SubmitChanges();
			return Content("done");
		}
		[HttpPost]
		public ContentResult EditExtra(string id, string value)
		{
			var a = id.SplitStr("-", 2);
			var b = a[1].SplitStr(".", 2);
			var f = DbUtil.Db.Families.Single(ff => ff.FamilyId == b[1].ToInt());
			switch (a[0])
			{
				case "s":
					f.AddEditExtraValue(b[0], value);
					break;
				case "t":
					f.AddEditExtraData(b[0], value);
					break;
				case "d":
					{
						DateTime dt;
						if (DateTime.TryParse(value, out dt))
						{
							f.AddEditExtraDate(b[0], dt);
							value = dt.ToShortDateString();
						}
						else
							value = "";
					}
					break;
				case "i":
					f.AddEditExtraInt(b[0], value.ToInt());
					break;
				case "b":
					if (value == "True")
						f.AddEditExtraBool(b[0], true);
					else
						f.RemoveExtraValue(DbUtil.Db, b[0]);
					break;
				case "m":
				{
					if (value == null)
						value = Request.Form["value[]"];
					var cc = Code.FamilyExtraValues.ExtraValueBits(b[0], b[1].ToInt());
					var aa = value.Split(',');
					foreach (var c in cc)
					{
						if (aa.Contains(c.Key)) // checked now
							if (!c.Value) // was not checked before
								f.AddEditExtraBool(c.Key, true);
						if (!aa.Contains(c.Key)) // not checked now
							if (c.Value) // was checked before
								f.RemoveExtraValue(DbUtil.Db, c.Key);
					}
					DbUtil.Db.SubmitChanges();
					break;
				}
			}
			DbUtil.Db.SubmitChanges();
			if (value == "null")
				return Content(null);
			return Content(value);
		}
		[HttpPost]
		public JsonResult ExtraValues(string id)
		{
			var a = id.SplitStr("-", 2);
			var b = a[1].SplitStr(".", 2);
			var c = Code.FamilyExtraValues.Codes(b[0]);
			var j = Json(c);
			return j;
		}
		[HttpPost]
		public JsonResult ExtraValues2(string id)
		{
			var a = id.SplitStr("-", 2);
			var b = a[1].SplitStr(".", 2);
			var c = Code.FamilyExtraValues.ExtraValueBits(b[0], b[1].ToInt());
			var j = Json(c);
			return j;
		}
		[HttpPost]
		public ActionResult NewExtraValue(int id, string field, string type, string value)
		{
			var v = new FamilyExtra { FamilyId = id, Field = field };
			DbUtil.Db.FamilyExtras.InsertOnSubmit(v);
			switch (type)
			{
				case "string":
					v.StrValue = value;
					break;
				case "text":
					v.Data = value;
					break;
				case "date":
					var dt = DateTime.MinValue;
					DateTime.TryParse(value, out dt);
					v.DateValue = dt;
					break;
				case "int":
					v.IntValue = value.ToInt();
					break;
			}
			try
			{
				DbUtil.Db.SubmitChanges();
			}
			catch (Exception ex)
			{
				return Content("error: " + ex.Message);
			}
			return Content("ok");
		}
		[HttpPost]
		public ActionResult ExtrasGrid(int id)
		{
			var f = DbUtil.Db.Families.Single(ff => ff.FamilyId == id);
			return View(f);
		}

    }
}
