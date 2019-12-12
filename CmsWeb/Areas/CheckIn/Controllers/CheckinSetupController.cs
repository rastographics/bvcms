using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ImageData;
using System;
using System.Web;
using Newtonsoft.Json;
using CmsWeb.Common;
using System.Text.RegularExpressions;

namespace CmsWeb.Areas.CheckIn.Controllers
{
    [Authorize(Roles = "Checkin")]
    [RouteArea("CheckIn", AreaPrefix = "CheckinSetup"), Route("{action}")]
    public class CheckinSetupController : CmsStaffController
    {
        private MapperConfiguration _config;

        public CheckinSetupController(IRequestManager requestManager) : base(requestManager)
        {
            _config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Campu, CampusModel>();
                cfg.CreateMap<CheckinProfile, CheckinProfilesModel>();
                cfg.CreateMap<CheckinProfileSetting, CheckinProfileSettingsModel>();
            });
        }

        [Route("~/CheckinSetup")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("~/CheckinSetup/GetCheckinProfiles")]
        public JsonResult GetCheckinProfiles()
        {
            if (CurrentDatabase.CheckinProfiles.Count() == 0)
            {
                CheckinProfilesModel.CreateDefault(CurrentDatabase);
            }
            var CheckinProfiles = CurrentDatabase.CheckinProfiles.ProjectTo<CheckinProfilesModel>(_config).ToList();
            foreach (var item in CheckinProfiles)
            {
                item.CheckinProfileSettings = CurrentDatabase.CheckinProfileSettings
                    .ProjectTo<CheckinProfileSettingsModel>(_config)
                    .FirstOrDefault(c => c.CheckinProfileId == item.CheckinProfileId);
            }

            return Json(CheckinProfiles, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/CheckinSetup/CreateCheckinSettings")]
        public JsonResult CreateCheckinSettings()
        {
            var CheckinProfileSettings = new CheckinProfileSettingsModel();

            return Json(CheckinProfileSettings, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/CheckinSetup/CreateCheckinProfile")]
        public JsonResult CreateCheckinProfile()
        {
            var CheckinProfile = new CheckinProfilesModel()
            {
                CheckinProfileSettings = new CheckinProfileSettingsModel()
            };

            return Json(CheckinProfile, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/CheckinSetup/GetCampuses")]
        public JsonResult GetCampuses()
        {
            var Campuses = CurrentDatabase.Campus.ProjectTo<CampusModel>(_config);

            return Json(Campuses, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/CheckinSetup/InsertCheckinProfile")]
        public HttpStatusCodeResult InsertCheckinProfile()
        {
            var file = Request.Files.Count == 0 ? null : Request.Files[0];
            var json = JsonConvert.DeserializeObject<CheckinProfilesModel>(Request["jsonD"]);
            CheckinProfile checkinProfile = MapCheckinProfile(json);
            CheckinProfileSetting checkinProfileSettings = MapCheckinProfileSettings(json.CheckinProfileId, json.CheckinProfileSettings, file);

            if (json.CheckinProfileId == 0)
            {
                CurrentDatabase.CheckinProfiles.InsertOnSubmit(checkinProfile);
                CurrentDatabase.SubmitChanges();
                checkinProfileSettings.CheckinProfileId = checkinProfile.CheckinProfileId;
                CurrentDatabase.CheckinProfileSettings.InsertOnSubmit(checkinProfileSettings);
            }

            CurrentDatabase.SubmitChanges();

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [HttpDelete]
        [Route("~/CheckinSetup/DeleteProfile/{ProfileId}")]
        public HttpStatusCodeResult DeleteProfile(int ProfileId)
        {
            var checkinProfile = CurrentDatabase.CheckinProfiles.FirstOrDefault(c => c.CheckinProfileId == ProfileId);
            if (checkinProfile != null)
            {
                var checkinProfileSettings = CurrentDatabase.CheckinProfileSettings.FirstOrDefault(c => c.CheckinProfileId == ProfileId);
                Image.Delete(CurrentImageDatabase, checkinProfileSettings.BackgroundImage);
                CurrentDatabase.CheckinProfileSettings.DeleteOnSubmit(checkinProfileSettings);
                CurrentDatabase.CheckinProfiles.DeleteOnSubmit(checkinProfile);
                CurrentDatabase.SubmitChanges();
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private CheckinProfile MapCheckinProfile(CheckinProfilesModel json)
        {
            CheckinProfile checkinProfile;
            if (json.CheckinProfileId == 0)
            {
                checkinProfile = new CheckinProfile();
            }
            else
            {
                checkinProfile = CurrentDatabase.CheckinProfiles.FirstOrDefault(c => c.CheckinProfileId == json.CheckinProfileId);
            }

            checkinProfile.Name = json.Name;
            return checkinProfile;
        }

        private CheckinProfileSetting MapCheckinProfileSettings(int checkinProfileId, CheckinProfileSettingsModel jsonSettings, HttpPostedFileBase file)
        {
            CheckinProfileSetting checkinProfileSettings;
            if (checkinProfileId == 0)
            {
                checkinProfileSettings = new CheckinProfileSetting();
            }
            else
            {
                checkinProfileSettings = CurrentDatabase.CheckinProfileSettings.FirstOrDefault(c => c.CheckinProfileId == checkinProfileId);
            }

            checkinProfileSettings.CampusId = jsonSettings.CampusId == -1 ? null : jsonSettings.CampusId;
            checkinProfileSettings.Testing = jsonSettings.Testing;
            checkinProfileSettings.TestDay = jsonSettings.TestDay;
            checkinProfileSettings.AdminPIN = PinIsValid(jsonSettings.AdminPIN) ? jsonSettings.AdminPIN : "00000";
            checkinProfileSettings.PINTimeout = jsonSettings.PINTimeout;
            checkinProfileSettings.DisableJoin = jsonSettings.DisableJoin;
            checkinProfileSettings.DisableTimer = jsonSettings.DisableTimer;
            checkinProfileSettings.CutoffAge = jsonSettings.CutoffAge;
            checkinProfileSettings.Logout = PinIsValid(jsonSettings.Logout) ? jsonSettings.Logout : "00000";
            checkinProfileSettings.Guest = jsonSettings.Guest;
            checkinProfileSettings.Location = jsonSettings.Location;
            checkinProfileSettings.SecurityType = jsonSettings.SecurityType;
            checkinProfileSettings.ShowCheckinConfirmation = jsonSettings.ShowCheckinConfirmation;

            if (file != null)
            {
                var DefaultHost = CurrentDatabase.Setting("DefaultHost", "");
                checkinProfileSettings.BackgroundImage = StoreBGImage(file, checkinProfileSettings.BackgroundImage);
                checkinProfileSettings.BackgroundImageName = file.FileName;
                checkinProfileSettings.BackgroundImageURL = $"/BackgroundImage/{checkinProfileSettings.BackgroundImage}?{DateTime.Now.ToString("yyMMddhhmmss")}";
            }

            return checkinProfileSettings;
        }

        private bool PinIsValid(string logout)
        {
            return logout.All(Char.IsDigit);         
        }

        private int StoreBGImage(HttpPostedFileBase file, int? imageId)
        {
            var stream = file.InputStream;
            var bits = new byte[stream.Length];
            stream.Read(bits, 0, bits.Length);
            if (imageId == null)
            {
                return Image.NewImageFromBits(bits, CurrentImageDatabase, true).Id;
            }
            return CurrentImageDatabase.UpdateImageFromBits(imageId.Value, bits).Id;
        }
    }
}
