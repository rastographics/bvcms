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
        private SettingsRegistrationModel settingsRegistrationModel;
        public SettingsRegistrationModel SettingsRegistrationModel
        {
            get
            {
                if(settingsRegistrationModel == null && Id.HasValue)
                    settingsRegistrationModel = new SettingsRegistrationModel(Id.Value);
                return settingsRegistrationModel;
            }
        }

        private void populate(int id)
        {
            Id = id;
            DbUtil.Db.SetCurrentOrgId(id);
            if (Org == null)
                return;
            OrgMain = new OrgMain(Org);
            GroupSelect = GroupSelectCode.Member;
            IsVolunteerLeader = OrganizationMember.VolunteerLeaderInOrg(DbUtil.Db, Id);
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
        public Settings RegSettings => _RegSettings ?? (_RegSettings = DbUtil.Db.CreateRegistrationSettings(Org.OrganizationId));


        internal bool? _showCommunityGroupTab;
        public bool ShowCommunityGroupTab()
        {
            // We request this twice per Org Index.cshtml. Let's not do the DB calls twice.
            if (_showCommunityGroupTab.HasValue) return _showCommunityGroupTab.Value;

            _showCommunityGroupTab = DbUtil.Db.Setting("ShowCommunityGroupTab", "false").ToLower() == "false";

            return _showCommunityGroupTab.Value;
        }
    }
}
