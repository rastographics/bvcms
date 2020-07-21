using CmsData;
using CmsWeb.Areas.People.Models;
using CmsWeb.Code;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost]
        public ActionResult ProfileHeader(int id)
        {
            var m = new PersonModel(id, CurrentDatabase);
            return View("Personal/Header", m);
        }

        [HttpPost]
        [Route("PersonalDisplay/{id:int}/{isBusiness:bool}")]
        [Route("PersonalDisplay/{id:int}")]
        public ActionResult PersonalDisplay(int id, bool isBusiness)
        {            
            InitExportToolbar(id);
            var m = new BasicPersonInfo(id, isBusiness);
            return View("Personal/Display", m);
        }

        [HttpPost]
        [Route("PersonalEdit/{id:int}/{isBusiness:bool}")]
        [Route("PersonalEdit/{id:int}")]
        public ActionResult PersonalEdit(int id, bool isBusiness)
        {
            var m = new BasicPersonInfo(id, isBusiness);
            return View("Personal/Edit", m);
        }

        [HttpPost]
        [Route("PersonalUpdate/{id:int}/{isBusiness:bool}")]
        [Route("PersonalUpdate/{id:int}")]
        public ActionResult PersonalUpdate(int id, BasicPersonInfo m, bool isBusiness)
        {
            m.IsBusiness = isBusiness;
            if (isBusiness)
            {
                m.Gender = new CodeInfo(0, "Gender");
                m.MaritalStatus = new CodeInfo(0, "MaritalStatus");
            }   

            if (!ModelState.IsValid)
            {
                return View("Personal/Edit", m);
            }

            m.UpdatePerson(CurrentDatabase);
            DbUtil.LogPersonActivity($"Update Basic Info for: {m.person.Name}", m.Id, m.person.Name);
            InitExportToolbar(id);
            return View("Personal/Display", m);
        }

        [HttpPost]
        public ActionResult PictureDialog(int id)
        {
            var m = new PersonModel(id, CurrentDatabase);
            return View("Personal/PictureDialog", m);
        }

        [HttpPost]
        public ActionResult UploadPicture(int id, HttpPostedFileBase picture)
        {
            if (picture == null)
            {
                return Redirect("/Person2/" + id);
            }

            var person = CurrentDatabase.LoadPersonById(id);
            DbUtil.LogPersonActivity($"Uploading Picture for {person.Name}", id, person.Name);
            person.UploadPicture(CurrentDatabase, CurrentImageDatabase, picture.InputStream);
            return Redirect("/Person2/" + id);
        }

        [HttpPost]
        public ActionResult DeletePicture(int id)
        {
            var person = CurrentDatabase.LoadPersonById(id);
            person.DeletePicture(CurrentDatabase, CurrentImageDatabase);
            return Redirect("/Person2/" + id);
        }

        [HttpPost]
        public ActionResult RefreshThumbnail(int id)
        {
            var person = CurrentDatabase.LoadPersonById(id);
            person.DeleteThumbnail(CurrentDatabase, CurrentImageDatabase);
            return Redirect("/Person2/" + id);
        }

        [HttpPost]
        public ActionResult UpdateCropPosition(int id, int pictureId, int xPos, int yPos)
        {
            var picture = CurrentDatabase.Pictures.Single(pp => pp.PictureId == pictureId);
            picture.X = xPos;
            picture.Y = yPos;
            CurrentDatabase.SubmitChanges();

            // if we are updating the current user update their thumbnail pic position in session also.
            if (CurrentDatabase.UserPeopleId == id)
            {
                Util.UserThumbPictureBgPosition = $"{xPos}% {yPos}%";
            }
            return Redirect("/Person2/" + id);
        }

        [HttpPost]
        public ActionResult PostData(int pk, string name, string value)
        {
            var p = CurrentDatabase.LoadPersonById(pk);
            switch (name)
            {
                case "position":
                    p.UpdatePosition(CurrentDatabase, value.ToInt());
                    break;
                case "campus":
                    p.UpdateCampus(CurrentDatabase, value.ToInt());
                    break;
            }
            return new EmptyResult();
        }
    }
}
