using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Classes.ProtectMyMinistry;
using CmsWeb.Areas.Main.Models.Other;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaPrefix = "Volunteering"), Route("{action}/{id?}")]
    public class VolunteeringController : Controller
    {
        [Route("~/Volunteering/{id:int}")]
        public ActionResult Index(int id)
        {
            var vol = new VolunteerModel(id);
            return View(vol);
        }

        [HttpPost]
        public ActionResult Display(int id)
        {
            var vol = new VolunteerModel(id);
            return View(vol);
        }

        [HttpPost]
        public ActionResult Edit(int id)
        {
            var vol = new VolunteerModel(id);
            return View(vol);
        }

        [HttpPost]
        public ActionResult Update(int id, DateTime? processDate, int statusId, string comments, List<int> approvals, DateTime? mvrDate, int mvrStatusId)
        {
            var m = new VolunteerModel(id);
            m.Update(processDate, statusId, comments, approvals, mvrDate, mvrStatusId);

            m = new VolunteerModel(id);
            return View("Display", m);
        }

        [HttpPost]
        public ActionResult Upload(int id, HttpPostedFileBase file)
        {
            if (file == null)
                return Content("no file");

            var vol = new VolunteerModel(id);
            var name = System.IO.Path.GetFileName(file.FileName);

            var f = new VolunteerForm
                        {
                            UploaderId = Util.UserId1,
                            PeopleId = vol.Volunteer.PeopleId,
                            Name = name.Truncate(100),
                            AppDate = Util.Now,
                        };

            var bits = new byte[file.ContentLength];
            file.InputStream.Read(bits, 0, bits.Length);

            var mimetype = file.ContentType.ToLower();

            switch (mimetype)
            {
                case "image/jpeg":
                case "image/pjpeg":
                case "image/gif":
                case "image/png":
                    {
                        f.IsDocument = false;

                        try
                        {
                            f.SmallId = ImageData.Image.NewImageFromBits(bits, 165, 220).Id;
                            f.MediumId = ImageData.Image.NewImageFromBits(bits, 675, 900).Id;
                            f.LargeId = ImageData.Image.NewImageFromBits(bits).Id;
                        }
                        catch
                        {
                            return View("Index", vol);
                        }

                        break;
                    }

                case "text/plain":
                case "application/pdf":
                case "application/msword":
                case "application/vnd.ms-excel":
                    {
                        f.MediumId = ImageData.Image.NewImageFromBits(bits, mimetype).Id;
                        f.SmallId = f.MediumId;
                        f.LargeId = f.MediumId;
                        f.IsDocument = true;
                        break;
                    }

                default: return View("Index", vol);
            }

            DbUtil.Db.VolunteerForms.InsertOnSubmit(f);
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity($"Uploading VolunteerApp for {vol.Volunteer.Person.Name}");

            return Redirect($"/Volunteering/{vol.Volunteer.PeopleId}#tab_documents");
        }

        public ActionResult Delete(int id, int peopleId)
        {
            var form = DbUtil.Db.VolunteerForms.Single(f => f.Id == id);

            ImageData.Image.DeleteOnSubmit(form.SmallId);
            ImageData.Image.DeleteOnSubmit(form.MediumId);
            ImageData.Image.DeleteOnSubmit(form.LargeId);

            DbUtil.Db.VolunteerForms.DeleteOnSubmit(form);
            DbUtil.Db.SubmitChanges();

            return Redirect($"/Volunteering/{peopleId}#tab_documents");
        }

        public ActionResult CreateCheck(int id, string code, int type, int label = 0)
        {
            var tabName = type == 1 ? "tab_backgroundChecks" : "tab_creditChecks";
            ProtectMyMinistryHelper.Create(id, code, type, label);
            return Redirect($"/Volunteering/{id}#{tabName}");
        }

        public ActionResult EditCheck(int id, int type, int label = 0)
        {
            var tabName = type == 1 ? "tab_backgroundChecks" : "tab_creditChecks";
            var bc = (from e in DbUtil.Db.BackgroundChecks
                      where e.Id == id
                      select e).Single();

            bc.ReportLabelID = label;
            DbUtil.Db.SubmitChanges();
            return Redirect($"/Volunteering/{bc.PeopleID}#{tabName}");
        }

        [HttpPost]
        public ActionResult DeleteCheck(int id)
        {
            var bc = (from e in DbUtil.Db.BackgroundChecks
                      where e.Id == id
                      select e).Single();

            DbUtil.Db.BackgroundChecks.DeleteOnSubmit(bc);
            DbUtil.Db.SubmitChanges();

            return new EmptyResult();
        }

        public ActionResult SubmitCheck(int id, int type, int iPeopleID, string sSSN, string sDLN, string sUser = "", string sPassword = "", int iStateID = 0, string sPlusCounty = "", string sPlusState = "")
        {
            var tabName = type == 1 ? "tab_backgroundChecks" : "tab_creditChecks";
            var responseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + ProtectMyMinistryHelper.PMM_Append;

            var p = (from e in DbUtil.Db.People
                     where e.PeopleId == iPeopleID
                     select e).Single();

            // Check for Existing SSN
            if (sSSN != null && sSSN.Length > 1)
            {
                if (sSSN.Substring(0, 1) == "X")
                {
                    sSSN = Util.Decrypt(p.Ssn, "People");
                }
                else
                {
                    sSSN = sSSN.Replace("-", "").Replace(" ", ""); ;
                    p.Ssn = Util.Encrypt(sSSN, "People");
                }
            }
            else
            {
                sSSN = Util.Decrypt(p.Ssn, "People");
            }

            // Check for Existing DLN and DL State
            if (sDLN != null && sDLN.Length > 1)
            {
                if (sDLN.Substring(0, 1) == "X")
                {
                    sDLN = Util.Decrypt(p.Dln, "People");
                    iStateID = p.DLStateID ?? 0;
                }
                else
                {
                    p.Dln = Util.Encrypt(sDLN, "People");
                    p.DLStateID = iStateID;
                }
            }

            DbUtil.Db.SubmitChanges();

            ProtectMyMinistryHelper.Submit(id, sSSN, sDLN, responseUrl, iStateID, sUser, sPassword, sPlusCounty, sPlusState);

            var bc = (from e in DbUtil.Db.BackgroundChecks
                      where e.Id == id
                      select e).Single();

            if (bc != null && (bc.ServiceCode == "Combo" || bc.ServiceCode == "ComboPC" || bc.ServiceCode == "ComboPS"))
            {
                var vol = DbUtil.Db.Volunteers.SingleOrDefault(e => e.PeopleId == iPeopleID);
                vol.ProcessedDate = DateTime.Now;
                DbUtil.Db.SubmitChanges();
            }

            return Redirect($"/Volunteering/{iPeopleID}#{tabName}");
        }

        public ActionResult DialogSubmit(int id, int type)
        {
            var bc = (from e in DbUtil.Db.BackgroundChecks
                      where e.Id == id
                      select e).Single();

            ViewBag.dialogType = type;

            return View(bc);
        }

        public ActionResult DialogEdit(int id, int type)
        {
            var bc = (from e in DbUtil.Db.BackgroundChecks
                      where e.Id == id
                      select e).Single();

            ViewBag.dialogType = type;

            return View(bc);
        }

        public ActionResult DialogType(int id, int type)
        {
            var p = (from e in DbUtil.Db.People
                     where e.PeopleId == id
                     select e).Single();

            ViewBag.dialogType = type;

            return View(p);
        }

        [HttpPost]
        public ContentResult EditForm(int pk, string value)
        {
            var f = DbUtil.Db.VolunteerForms.Single(m => m.Id == pk);
            f.Name = value.Truncate(100);
            DbUtil.Db.SubmitChanges();
            var c = new ContentResult();
            c.Content = value;
            return c;
        }
    }
}
