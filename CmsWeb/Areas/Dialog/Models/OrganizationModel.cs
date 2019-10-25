using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class NewOrganizationModel : IDbBinder
    {
        public Organization org { get; set; }
        public int? OrganizationId { get; set; }
        public bool copysettings { get; set; }
        public bool copyregistration { get; set; }
        public bool DisplayCopySettings { get; set; }
        public CMSDataContext CurrentDatabase { get; set; }

        public NewOrganizationModel(CMSDataContext db, int? id, bool displayCopySettings = false)
        {
            CurrentDatabase = db;
            DisplayCopySettings = displayCopySettings;
            if (!id.HasValue)
            {
                id = CurrentDatabase.Setting("DefaultOrgId", "0").ToInt();
            }

            org = CurrentDatabase.LoadOrganizationById(id);
            if (org == null)
            {
                org = CurrentDatabase.Organizations.First();
            }

            OrganizationId = org.OrganizationId;
        }

        public NewOrganizationModel()
        {
        }

        public NewOrganizationModel(CMSDataContext db)
        {
            CurrentDatabase = db;
        }

        private CodeValueModel cv = new CodeValueModel();

        public IEnumerable<SelectListItem> CampusList()
        {
            return CodeValueModel.ConvertToSelect(cv.AllCampuses0(), "Id");
        }
        public IEnumerable<SelectListItem> OrgStatusList()
        {
            return CodeValueModel.ConvertToSelect(cv.OrganizationStatusCodes(), "Id");
        }
        public IEnumerable<SelectListItem> LeaderTypeList()
        {
            var items = CodeValueModel.MemberTypeCodes0().Select(c => new CodeValueItem { Code = c.Code, Id = c.Id, Value = c.Value });
            return CodeValueModel.ConvertToSelect(items, "Id");
        }
        public IEnumerable<SelectListItem> EntryPointList()
        {
            return CodeValueModel.ConvertToSelect(cv.EntryPoints(), "Id");
        }
        public IEnumerable<SelectListItem> GenderList()
        {
            return CodeValueModel.ConvertToSelect(cv.GenderCodes(), "Id");
        }
        public IEnumerable<SelectListItem> SecurityTypeList()
        {
            return CodeValueModel.ConvertToSelect(cv.SecurityTypeCodes(), "Id");
        }
    }
}
