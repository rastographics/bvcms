using CmsData;
using CmsWeb.Areas.People.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost]
        public ActionResult FamilyMembers(int id)
        {
            var m = new FamilyModel(id);
            return View("Family/Members", m);
        }

        [HttpPost]
        public ActionResult RelatedFamilies(int id)
        {
            var m = new FamilyModel(id);
            return View("Family/Related", m);
        }

        [HttpPost, Route("UpdateRelation/{id}/{id1}/{id2}")]
        public ActionResult UpdateRelation(int id, int id1, int id2, string value)
        {
            var r = CurrentDatabase.RelatedFamilies.SingleOrDefault(rr => rr.FamilyId == id1 && rr.RelatedFamilyId == id2);
            r.FamilyRelationshipDesc = value.Truncate(256);
            CurrentDatabase.SubmitChanges();
            var m = new FamilyModel(id);
            return View("Family/Related", m);
        }

        [HttpPost, Route("DeleteRelation/{id}/{id1}/{id2}")]
        public ActionResult DeleteRelation(int id, int id1, int id2)
        {
            var r = CurrentDatabase.RelatedFamilies.SingleOrDefault(rf => rf.FamilyId == id1 && rf.RelatedFamilyId == id2);
            CurrentDatabase.RelatedFamilies.DeleteOnSubmit(r);
            CurrentDatabase.SubmitChanges();
            var m = new FamilyModel(id);
            return View("Family/Related", m);
        }

        [HttpPost, Route("RelatedFamilyEdit/{id}/{id1}/{id2}")]
        public ActionResult RelatedFamilyEdit(int id, int id1, int id2)
        {
            var r = CurrentDatabase.RelatedFamilies.SingleOrDefault(rf => rf.FamilyId == id1 && rf.RelatedFamilyId == id2);
            ViewBag.Id = id;
            return View("Family/RelatedEdit", r);
        }

        [HttpGet]
        public ActionResult FamilyQuery(int id)
        {
            var c = CurrentDatabase.ScratchPadCondition();
            c.Reset();
            c.AddNewClause(QueryType.FamilyId, CompareType.Equal, id);
            c.Save(CurrentDatabase);
            return Redirect("/Query/" + c.Id);
        }

        [HttpGet]
        public ActionResult RelatedFamilyQuery(int id)
        {
            var c = CurrentDatabase.ScratchPadCondition();
            c.Reset();
            c.AddNewClause(QueryType.RelatedFamilyMembers, CompareType.Equal, id);
            c.Save(CurrentDatabase);
            return Redirect("/Query/" + c.Id);
        }

        [HttpPost]
        public ActionResult FamilyPictureDialog(int id)
        {
            var currentPerson = new PersonModel(id);
            return View("Family/PictureDialog", currentPerson);
        }

        [HttpPost]
        public ActionResult UploadFamilyPicture(int id, HttpPostedFileBase picture)
        {
            if (picture == null)
            {
                return Redirect("/Person2/" + id);
            }

            var family = CurrentDatabase.Families.SingleOrDefault(ff => ff.People.Any(mm => mm.PeopleId == id));
            if (family == null)
            {
                return Content("family not found");
            }

            DbUtil.LogActivity($"Uploading Picture for {family.FamilyName(CurrentDatabase)}");
            family.UploadPicture(CurrentDatabase, picture.InputStream, id);
            return Redirect("/Person2/" + id);
        }

        [HttpPost]
        public ActionResult DeleteFamilyPicture(int id)
        {
            var family = CurrentDatabase.LoadFamilyByPersonId(id);
            family.DeletePicture(CurrentDatabase);
            return Redirect("/Person2/" + id);
        }
    }
}
