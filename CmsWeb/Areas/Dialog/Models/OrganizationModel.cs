using CmsData;
using CmsWeb.Code;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class NewOrganizationModel
    {
        public Organization org { get; set; }
        public int? OrganizationId { get; set; }
        public bool copysettings { get; set; }
        public bool copyregistration { get; set; }
        public bool DisplayCopySettings { get; set; }
        public NewOrganizationModel(int? id, bool displayCopySettings = false)
        {
            DisplayCopySettings = displayCopySettings;
            if (!id.HasValue)
            {
                id = DbUtil.Db.Setting("DefaultOrgId", "0").ToInt();
            }

            org = DbUtil.Db.LoadOrganizationById(id);
            if (org == null)
            {
                org = DbUtil.Db.Organizations.First();
            }

            OrganizationId = org.OrganizationId;
        }

        public NewOrganizationModel()
        {
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
