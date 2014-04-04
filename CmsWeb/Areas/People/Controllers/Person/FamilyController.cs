using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost, Route("Person2/FamilyMembers/{id}")]
        public ActionResult FamilyMembers(int id)
        {
            var m = new FamilyModel(id);
            return View("Family/Members", m);
        }
        [HttpPost, Route("Person2/RelatedFamilies/{id}")]
        public ActionResult RelatedFamilies(int id)
        {
            var m = new FamilyModel(id);
            return View("Family/Related", m);
        }
        [HttpPost, Route("Person2/UpdateRelation/{id}/{id1}/{id2}")]
        public ActionResult UpdateRelation(int id, int id1, int id2, string value)
        {
            var r = DbUtil.Db.RelatedFamilies.SingleOrDefault(rr => rr.FamilyId == id1 && rr.RelatedFamilyId == id2);
            r.FamilyRelationshipDesc = value;
            DbUtil.Db.SubmitChanges();
            var m = new FamilyModel(id);
            return View("Family/Related", m);
        }
        [HttpPost, Route("Person2/DeleteRelation/{id}/{id1}/{id2}")]
        public ActionResult DeleteRelation(int id, int id1, int id2)
        {
            var r = DbUtil.Db.RelatedFamilies.SingleOrDefault(rf => rf.FamilyId == id1 && rf.RelatedFamilyId == id2);
            DbUtil.Db.RelatedFamilies.DeleteOnSubmit(r);
            DbUtil.Db.SubmitChanges();
            var m = new FamilyModel(id);
            return View("Family/Related", m);
        }
        [HttpPost, Route("Person2/RelatedFamilyEdit/{id}/{id1}/{id2}")]
        public ActionResult RelatedFamilyEdit(int id, int id1, int id2)
        {
            var r = DbUtil.Db.RelatedFamilies.SingleOrDefault(rf => rf.FamilyId == id1 && rf.RelatedFamilyId == id2);
            ViewBag.Id = id;
            return View("Family/RelatedEdit", r);
        }
        [HttpGet, Route("Person2/FamilyQuery/{id}")]
        public ActionResult FamilyQuery(int id)
        {
            var c = DbUtil.Db.ScratchPadCondition();
            c.Reset(DbUtil.Db);
            c.AddNewClause(QueryType.FamilyId, CompareType.Equal, id);
            c.Save(DbUtil.Db);
            return Redirect("/Query/" + c.Id);
        }
        [HttpGet, Route("Person2/RelatedFamilyQuery/{id}")]
        public ActionResult RelatedFamilyQuery(int id)
        {
            var c = DbUtil.Db.ScratchPadCondition();
            c.Reset(DbUtil.Db);
            c.AddNewClause(QueryType.RelatedFamilyMembers, CompareType.Equal, id);
            c.Save(DbUtil.Db);
            return Redirect("/Query/" + c.Id);
        }
        [HttpPost, Route("Person2/FamilyPictureDialog/{id:int}")]
        public ActionResult FamilyPictureDialog(int id)
        {
            var m = new PersonModel(id);
            return View("Family/PictureDialog", m);
        }
        [HttpPost, Route("Person2/UploadFamilyPicture/{id:int}")]
        public ActionResult UploadFamilyPicture(int id, HttpPostedFileBase picture)
        {
            if (picture == null) 
                return Redirect("/Person2/" + id);
            var family = DbUtil.Db.Families.SingleOrDefault(ff => ff.People.Any(mm => mm.PeopleId == id));
            if (family == null)
                return Content("family not found");
            DbUtil.LogActivity("Uploading Picture for {0}".Fmt(family.FamilyName(DbUtil.Db)));
            family.UploadPicture(DbUtil.Db, picture.InputStream, id);
            return Redirect("/Person2/" + id);
        }
        [HttpPost, Route("Person2/DeleteFamilyPicture/{id:int}")]
        public ActionResult DeleteFamilyPicture(int id)
        {
            var family = DbUtil.Db.LoadFamilyByPersonId(id);
            family.DeletePicture(DbUtil.Db);
            return Redirect("/Person2/" + id);
        }
    }
}
