using System.Linq;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models.Person;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [POST("Person2/FamilyMembers/{id}")]
        public ActionResult FamilyMembers(int id)
        {
            var m = new FamilyModel(id);
            return View("Family/Members", m);
        }
        [POST("Person2/RelatedFamilies/{id}")]
        public ActionResult RelatedFamilies(int id)
        {
            var m = new FamilyModel(id);
            return View("Family/Related", m);
        }
        [POST("Person2/UpdateRelation/{id}/{id1}/{id2}")]
        public ActionResult UpdateRelation(int id, int id1, int id2, string value)
        {
            var r = DbUtil.Db.RelatedFamilies.SingleOrDefault(rr => rr.FamilyId == id1 && rr.RelatedFamilyId == id2);
            r.FamilyRelationshipDesc = value;
            DbUtil.Db.SubmitChanges();
            var m = new FamilyModel(id);
            return View("Family/Related", m);
        }
        [POST("Person2/DeleteRelation/{id}/{id1}/{id2}")]
        public ActionResult DeleteRelation(int id, int id1, int id2)
        {
            var r = DbUtil.Db.RelatedFamilies.SingleOrDefault(rf => rf.FamilyId == id1 && rf.RelatedFamilyId == id2);
            DbUtil.Db.RelatedFamilies.DeleteOnSubmit(r);
            DbUtil.Db.SubmitChanges();
            var m = new FamilyModel(id);
            return View("Family/Related", m);
        }
        [POST("Person2/RelatedFamilyEdit/{id}/{id1}/{id2}")]
        public ActionResult RelatedFamilyEdit(int id, int id1, int id2)
        {
            var r = DbUtil.Db.RelatedFamilies.SingleOrDefault(rf => rf.FamilyId == id1 && rf.RelatedFamilyId == id2);
            ViewBag.Id = id;
            return View("Family/RelatedEdit", r);
        }
    }
}
