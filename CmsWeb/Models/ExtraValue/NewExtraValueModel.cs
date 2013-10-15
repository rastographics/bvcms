using System.ComponentModel;
using System.Linq;
using CmsData;
using CmsWeb.Code;
using DocumentFormat.OpenXml.Drawing;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class NewExtraValueModel
    {
        public int Id { get; set; }
        public string ExtraValueTable { get; set; }
        public string ExtraValueLocation { get; set; }

        [DisplayNameAttribute("Name")]
        public string ExtraValueName { get; set; }
        [DisplayNameAttribute("Type")]
        public CodeInfo ExtraValueType { get; set; }
        [DisplayNameAttribute("Checkboxes Prefix")]
        public string ExtraValueBitPrefix { get; set; }
        private string BitPrefix {
            get
            {
                if (ExtraValueBitPrefix.HasValue())
                    return ExtraValueBitPrefix + "-";
                return "";
            }
        }
        [DisplayNameAttribute("Checkboxes")]
        public string ExtraValueCodes { get; set; }
        public string VisibilityRoles { get; set; }

        public NewExtraValueModel(int id, string table, string location)
        {
            ExtraValueType = new CodeInfo("2", "ExtraValueType");
            Id = id;
            ExtraValueTable = table;
            ExtraValueLocation = location;
        }
        public NewExtraValueModel() { }

        public bool ExtraValueBitPrefixDisabled
        {
            get { return ExtraValueType.Value != "Bits"; }
        }
        public bool ExtraValueCodesDisabled
        {
            get { return new string[] {"Code", "Bits"}.Contains(ExtraValueType.Value); }
        }

        public string AddAsNew()
        {
            var fields = Views.GetStandardExtraValues(ExtraValueTable);
            var existing = fields.SingleOrDefault(ff => ff.Name == ExtraValueName);
            if (existing != null)
                return "field already exists";

            // Check for conflicts in AdHoc fields here
            // It is OK if an AdHoc field already exists which is the same type as this one.

            var v = new Value
            {
                Name = ExtraValueName,
                Type = ExtraValueType.Value,
                Codes = ExtraValueCodes.SplitLines().Select(ss => BitPrefix + ss).ToList(),
                VisibilityRoles = VisibilityRoles
            };
            var i = Views.GetViewsView(ExtraValueTable, ExtraValueLocation);
            i.view.Values.Add(v);
            i.views.Save();
            return null;
        }
    }
}