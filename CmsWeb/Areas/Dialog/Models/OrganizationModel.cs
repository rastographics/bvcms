using CmsData;
using CmsWeb.Code;
using CmsWeb.Constants;
using CmsWeb.Models;
using System;
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

        [Obsolete(Errors.ModelBindingConstructorError, false)]
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
        public IEnumerable<SelectListItem> RegistrationTypeList()
        {
            // return CodeValueModel.ConvertToSelect(cv.RegistrationTypes(), "Id");
            var regTypes = CodeValueModel.ConvertToSelect(cv.RegistrationTypes(), "Id");


            if (regTypes.Any(x => x.Text == "Ticketed Event"))
            {
                List<SelectListItem> ticketedTypes = new List<SelectListItem>();

                string indent = "";

                foreach (var item in regTypes)
                {
                    if (item.Text == "Ticketed Event")
                    {
                        indent = "\xA0 \xA0 \xA0";
                        ticketedTypes.Add(new SelectListItem { Text = "Ticketed Event", Value = item.Value });
                        ticketedTypes.Add(new SelectListItem { Text = "-----LEGACY-----", Value = "", Disabled = true });
                        continue;
                    }

                    ticketedTypes.Add(new SelectListItem { Text = indent + item.Text, Value = item.Value });
                }

                return ticketedTypes;
            }

            return regTypes;
        }
    }
}
