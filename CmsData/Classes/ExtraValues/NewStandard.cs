using System.Linq;
using CmsData.Classes.ExtraValues;
using UtilityExtensions;

namespace CmsData.ExtraValue
{
    public class NewStandard
    {
        public string Name;
        public string Table;
        public string Type;
        public string VisibilityRoles;
        public string Checkboxes;
        public string Codes;
        public string BitPrefix;
        public string Location;

        public void Add(CMSDataContext db)
        {
            var fields = Views.GetStandardExtraValues(db, Table);
            const string defaultCodes = @"
Option 1
Option 2
";
            var codes = Type == "Bits"
                ? Checkboxes
                : Type == "Code"
                    ? Codes ?? defaultCodes
                    : null;
            var a = codes.SplitLines(noblanks: true).Select(ss => new Code { Text = BitPrefix + ss }).ToList();
            var v = new Value
            {
                Type = Type,
                Name = Name,
                VisibilityRoles = VisibilityRoles,
                Codes = a,
                //Link = HttpUtility.HtmlEncode(ExtraValueLink)
            };
            var i = Views.GetViewsView(db, Table, Location);
            i.view.Values.Add(v);
            i.views.Save(db);
        }
    }
}
