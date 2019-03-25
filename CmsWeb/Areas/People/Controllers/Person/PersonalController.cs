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
        public ActionResult ProfileHeader(int id)
        {
            var m = new PersonModel(id);
            return View("Personal/Header", m);
        }

        [HttpPost]
        public ActionResult PersonalDisplay(int id)
        {
            InitExportToolbar(id);
            var m = new BasicPersonInfo(id);
            return View("Personal/Display", m);
        }

        [HttpPost]
        public ActionResult PersonalEdit(int id)
        {
            var m = new BasicPersonInfo(id);
            return View("Personal/Edit", m);
        }

        [HttpPost]
        public ActionResult PersonalUpdate(int id, BasicPersonInfo m)
        {
            if (!ModelState.IsValid)
            {
                return View("Personal/Edit", m);
            }

            m.UpdatePerson();
            DbUtil.LogPersonActivity($"Update Basic Info for: {m.person.Name}", m.Id, m.person.Name);
            InitExportToolbar(id);
            return View("Personal/Display", m);
        }

        [HttpPost]
        public ActionResult PictureDialog(int id)
        {
            var m = new PersonModel(id);
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
            person.UploadPicture(CurrentDatabase, picture.InputStream);
            return Redirect("/Person2/" + id);
        }

        [HttpPost]
        public ActionResult DeletePicture(int id)
        {
            var person = CurrentDatabase.LoadPersonById(id);
            person.DeletePicture(CurrentDatabase);
            return Redirect("/Person2/" + id);
        }

        [HttpPost]
        public ActionResult RefreshThumbnail(int id)
        {
            var person = CurrentDatabase.LoadPersonById(id);
            person.DeleteThumbnail(CurrentDatabase);
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
            if (Util.UserPeopleId == id)
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
