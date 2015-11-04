using CmsData;
using CmsWeb.CheckInAPI;
using CmsWeb.MobileAPI;
using CmsWeb.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Public.Controllers
{
    public class CheckInAPIController : Controller
    {
        public ActionResult Exists()
        {
            return Content("1");
        }

        private static bool Auth()
        {
            return AccountModel.AuthenticateMobile("Checkin").IsValid;
        }

        public ActionResult Authenticate(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            var br = new BaseMessage();
            br.error = 0;
            br.data = JsonConvert.SerializeObject(new CheckInInformation(getSettings(), getCampuses(), getLabelFormats()));
            return br;
        }

        private List<CheckInSettingsEntry> getSettings()
        {
            return (from s in DbUtil.Db.CheckInSettings
                    select new CheckInSettingsEntry
                    {
                        name = s.Name,
                        settings = s.Settings
                    }).ToList();
        }

        private List<CheckInCampus> getCampuses()
        {
            return (from c in DbUtil.Db.Campus
                    where c.Organizations.Any(o => o.CanSelfCheckin == true)
                    orderby c.Id
                    select new CheckInCampus
                    {
                        id = c.Id,
                        name = c.Description
                    }).ToList();
        }

        private List<CheckInLabelFormat> getLabelFormats()
        {
            var labels = (from e in DbUtil.Db.LabelFormats
                          select new CheckInLabelFormat
                          {
                              name = e.Name,
                              size = e.Size,
                              format = e.Format,
                          }).ToList();

            foreach (var label in labels)
            {
                label.parse();
            }

            return labels;
        }

        public ActionResult Search() //string id, int campus, int day
        {
            //if (!Auth())
            //    return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            //DbUtil.LogActivity("Check-In Search: " + id);

            /* Replacement for CheckinMatch without the lock
             SELECT FamilyId, PeopleId, Name, GenderId
             FROM dbo.People
             WHERE FamilyId IN (SELECT FamilyId FROM dbo.Families WHERE HomePhoneLU LIKE '4813443%')
               OR FamilyId IN (SELECT FamilyId FROM dbo.People WHERE CellPhoneLU LIKE '4813443%' OR WorkPhoneLU LIKE '4813443%')
               AND (CampusId = 1 OR CampusId IS NULL)
             ORDER BY PositionInFamilyId, GenderId
            */

            //var matches = DbUtil.Db.CheckinMatch(id).ToList();

            //if (!matches.Any())
            //    return new FamilyResult(0, campus, day, 0, false); // not found
            //if (matches.Count() == 1)
            //    return new FamilyResult(matches.Single().Familyid.Value, campus, day, 0, matches[0].Locked ?? false);
            //return new MultipleResult(matches, 1);

            var results = (from p in DbUtil.Db.People
                           where (from f in DbUtil.Db.Families where f.HomePhoneLU.Contains("4813443") select f.FamilyId).Contains(p.FamilyId)
                             || (from o in DbUtil.Db.People where o.CellPhone.Contains("4813443") || o.WorkPhoneLU.Contains("4813443") select o.FamilyId).Contains(p.FamilyId)
                           where p.DeceasedDate == null
                           orderby p.Name
                           select new { p.FamilyId }).Distinct();

            var br = new BaseMessage();
            var families = new List<CheckInFamily>();

            CheckInFamily family;

            foreach (var item in results)
            {
                var members = (from a in DbUtil.Db.CheckinFamilyMembers(item.FamilyId, 1, 1).ToList()
                               orderby a.Position, a.Genderid, a.Age
                               select a).ToList();

                if (members.Count() > 0)
                {
                    family = new CheckInFamily();
                    family.id = item.FamilyId;
                    family.name = members[0].Name;

                    foreach (var member in members)
                    {
                        family.addMember(new CheckInPerson(member));
                    }

                    families.Add(family);
                }
            }

            br.error = 0;
            br.count = results.Count();
            br.data = JsonConvert.SerializeObject(families);
            return br;
        }

        [HttpPost]
        public ActionResult SaveSettings(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            CheckInSettingsEntry entry = JsonConvert.DeserializeObject<CheckInSettingsEntry>(dataIn.data);

            var setting = (from e in DbUtil.Db.CheckInSettings
                           where e.Name == entry.name
                           select e).SingleOrDefault();

            BaseMessage br = new BaseMessage();

            if (setting == null)
            {
                setting = new CheckInSetting();
                setting.Name = entry.name;
                setting.Settings = entry.settings;

                DbUtil.Db.CheckInSettings.InsertOnSubmit(setting);

                br.data = "Settings saved";
            }
            else
            {
                setting.Settings = entry.settings;

                br.data = "Settings updated";
            }

            DbUtil.Db.SubmitChanges();

            br.error = 0;
            br.id = setting.Id;
            br.count = 1;

            return br;
        }
    }
}