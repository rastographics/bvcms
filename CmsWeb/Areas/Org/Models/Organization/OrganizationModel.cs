using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using CmsData.Registration;
using CmsData.View;
using CmsWeb.Code;
using UtilityExtensions;
using System.Text.RegularExpressions;
using CmsData.Codes;

namespace CmsWeb.Areas.Org.Models
{
    public class OrganizationModel : OrgPeopleModel
    {

        public bool IsVolunteerLeader { get; set; }

        public OrganizationModel()
        {
        }
        public OrganizationModel(int id)
        {
            populate(id);
        }
        public OrgMain OrgMain { get; set; }

        private SettingsGeneralModel settingsGeneralModel;
        public SettingsGeneralModel SettingsGeneralModel
        {
            get
            {
                if(settingsGeneralModel == null && Id.HasValue)
                    settingsGeneralModel = new SettingsGeneralModel(Id.Value);
                return settingsGeneralModel;
            }
        }

        private void populate(int id)
        {
            Id = id;
            DbUtil.Db.CurrentOrg.Id = id;
            OrgMain = new OrgMain(Org);
            GroupSelect = GroupSelectCode.Member;
            IsVolunteerLeader = VolunteerLeaderInOrg(Id);
        }
        public static bool VolunteerLeaderInOrg(int? orgid)
        {
            if (orgid == null)
                return false;
            var o = DbUtil.Db.LoadOrganizationById(orgid);
            if (o == null || o.RegistrationTypeId != RegistrationTypeCode.ChooseVolunteerTimes)
                return false;
            if (HttpContext.Current.User.IsInRole("Admin") ||
                HttpContext.Current.User.IsInRole("ManageVolunteers"))
                return true;
            var leaderorgs = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
            if (leaderorgs == null)
                return false;
            return leaderorgs.Contains(orgid.Value);
        }

        private CodeValueModel cv = new CodeValueModel();

        public IEnumerable<SelectListItem> Groups()
        {
            var q = from g in DbUtil.Db.MemberTags
                    where g.OrgId == Id
                    orderby g.Name
                    select new SelectListItem
                    {
                        Text = g.Name,
                        Value = g.Id.ToString()
                    };
            return q;
        }
        public static IEnumerable<SelectListItem> Tags()
        {
            var cv = new CodeValueModel();
            var tg = CodeValueModel.ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Id").ToList();
            if (HttpContext.Current.User.IsInRole("Edit"))
                tg.Insert(0, new SelectListItem { Value = "-1", Text = "(last query)" });
            tg.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return tg;
        }
        public static IEnumerable<SearchDivision> Divisions(int? id)
        {
            var q = from d in DbUtil.Db.SearchDivisions(id, null)
                where d.IsChecked == true                
                orderby d.IsMain descending, d.IsChecked descending, d.Program, d.Division
                select d;
            return q;
        }

//        public IEnumerable<SelectListItem> CampusList()
//        {
//            return CodeValueModel.ConvertToSelect(cv.AllCampuses0(), "Id");
//        }
//        public IEnumerable<SelectListItem> EntryPointList()
//        {
//            return CodeValueModel.ConvertToSelect(cv.EntryPoints(), "Id");
//        }
//        public IEnumerable<SelectListItem> GenderList()
//        {
//            return CodeValueModel.ConvertToSelect(cv.GenderCodes(), "Id");
//        }
        public static string SpaceCamelCase(string s)
        {
            return Regex.Replace(s, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }
        private Settings _RegSettings;
        public Settings RegSettings
        {
            get
            {
                if (_RegSettings == null)
                {
                    _RegSettings = new Settings(Org.RegSetting, DbUtil.Db, Org.OrganizationId);
                    _RegSettings.org = Org;
                }
                return _RegSettings;
            }
        }
    }
}
