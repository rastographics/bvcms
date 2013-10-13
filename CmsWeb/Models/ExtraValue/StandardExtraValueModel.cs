using System.ComponentModel;
using System.Linq;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class StandardExtraValueModel
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

        public StandardExtraValueModel(int id, string table, string location)
        {
            ExtraValueType = new CodeInfo("2", "ExtraValueType");
            Id = id;
            ExtraValueTable = table;
            ExtraValueLocation = location;
        }
        public StandardExtraValueModel() { }

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
            var c = DbUtil.Content("StandardExtraValues.xml");
            var Fields = Util.DeSerialize<Fields>(c.Body);
            var existing = Fields.FieldList.SingleOrDefault(ff => ff.name == ExtraValueName);
            if (existing != null)
                return "field already exists";

            // Check for conflicts in AdHoc fields here
            // It is OK if an AdHoc field already exists which is the same type as this one.

            var f = new Field
            {
                name = ExtraValueName,
                table = ExtraValueTable.ToString(),
                type = ExtraValueType.Value,
                location = ExtraValueLocation,
                Codes = ExtraValueCodes.SplitLines().Select(ss => BitPrefix + ss).ToList(),
                VisibilityRoles = VisibilityRoles
            };
            Fields.FieldList.Add(f);
            var newxml = Util.Serialize(Fields);
            DbUtil.SetStandardExtraValues(newxml);
            DbUtil.Db.SubmitChanges();
            return null;
        }
    }
}