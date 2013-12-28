using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [POST("Person2/ProfileHeader/{id}")]
        public ActionResult ProfileHeader(int id)
        {
            var m = new PersonModel(id);
            return View("Personal/Header", m);
        }

        [POST("Person2/PersonalDisplay/{id}")]
        public ActionResult PersonalDisplay(int id)
        {
            InitExportToolbar(id);
            var m = new BasicPersonInfo(id);
            return View("Personal/Display", m);
        }
        [POST("Person2/PersonalEdit/{id}")]
        public ActionResult PersonalEdit(int id)
        {
            var m = new BasicPersonInfo(id);
            return View("Personal/Edit", m);
        }
        [POST("Person2/PersonalUpdate/{id}")]
        public ActionResult Personalpdate(int id, BasicPersonInfo m)
        {
            m.UpdatePerson();
            DbUtil.LogActivity("Update Basic Info for: {0}".Fmt(m.person.Name));
            InitExportToolbar(id);
            return View("Personal/Display", m);
        }

        [POST("Person2/PictureDialog/{id:int}")]
        public ActionResult PictureDialog(int id)
        {
            var m = new PersonModel(id);
            return View("Personal/PictureDialog", m);
        }
        [POST("Person2/UploadPicture/{id:int}")]
        public ActionResult UploadPicture(int id, HttpPostedFileBase picture)
        {
            if (picture == null) 
                return Redirect("/Person2/" + id);
            var person = DbUtil.Db.LoadPersonById(id);
            DbUtil.LogActivity("Uploading Picture for {0}".Fmt(person.Name));
            person.UploadPicture(DbUtil.Db, picture.InputStream);
            return Redirect("/Person2/" + id);
        }
        [POST("Person2/DeletePicture/{id:int}")]
        public ActionResult DeletePicture(int id)
        {
            var person = DbUtil.Db.LoadPersonById(id);
            person.DeletePicture(DbUtil.Db);
            return Redirect("/Person2/" + id);
        }
        [POST("Person2/PostData")]
        public ActionResult PostData(int pk, string name, string value)
        {
            var p = DbUtil.Db.LoadPersonById(pk);
            switch (name)
            {
                case "position":
                    p.UpdatePosition(DbUtil.Db, value.ToInt());
                    break;
                case "campus":
                    p.UpdateCampus(DbUtil.Db, value.ToInt());
                    break;
            }
            return new EmptyResult();
        }
    }
}
