using CmsData;
using CmsData.Codes;
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
    public class NewInvolvementModel : IDbBinder
    {
        public Involvement org { get; set; }
        public int? InvolvementId { get; set; }
        public bool copysettings { get; set; }
        public bool copyregistration { get; set; }
        public bool DisplayCopySettings { get; set; }
        public CMSDataContext CurrentDatabase { get; set; }

        public NewInvolvementModel(CMSDataContext db, int? id, bool displayCopySettings = false)
        {
            CurrentDatabase = db;
            DisplayCopySettings = displayCopySettings;
            if (!id.HasValue)
            {
                id = CurrentDatabase.Setting("DefaultOrgId", "0").ToInt();
            }

            org = CurrentDatabase.LoadInvolvementById(id);
            if (org == null)
            {
                org = CurrentDatabase.Involvements.First();
            }

            InvolvementId = org.InvolvementId;
        }

        [Obsolete(Errors.ModelBindingConstructorError, false)]
        public NewInvolvementModel()
        {
        }

        public NewInvolvementModel(CMSDataContext db)
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
            return CodeValueModel.ConvertToSelect(cv.InvolvementStatusCodes(), "Id");
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

        public static IEnumerable<SelectListItem> InvolvementTypeIds()
        {
            var list = new List<SelectListItem>();

            var codes = RegistrationTypeCode.GetCodePairs();
            list.AddRange(codes.Select(dd => new SelectListItem { Value = dd.Key.ToString(), Text = dd.Value }));

            return list;
        }
    }
}
