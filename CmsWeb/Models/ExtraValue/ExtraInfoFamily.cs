using CmsData;
using CmsData.ExtraValue;
using CmsWeb.Code;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class ExtraInfoFamily : ExtraInfo
    {
        public override string QueryUrl
        {
            get
            {
                if (Type == "Bit" || Type == "Code")
                {
                    return $"/ExtraValue/FamilyQueryCodes?field={HttpUtility.UrlEncode(Field)}&value={HttpUtility.UrlEncode(Value)}";
                }

                return $"/ExtraValue/FamilyQueryData?field={HttpUtility.UrlEncode(Field)}&type={Type}";
            }
        }

        public override string RenameAllUrl => $"/ExtraValue/RenameAll/Family?field={HttpUtility.UrlEncode(Field)}";

        public override string DeleteAllUrl => $"/ExtraValue/DeleteAll/Family/{Type}?field={HttpUtility.UrlEncode(Field)}&value={HttpUtility.UrlEncode(Value)}";

        public override string ConvertToStandardUrl => $"/ExtraValue/ConvertToStandard/Family?name={HttpUtility.UrlEncode(Field)}";

        public override IEnumerable<ExtraInfo> CodeSummary()
        {
            var NameTypes = Views.GetViewableNameTypes(DbUtil.Db, "Family", nocache: true);
            var standardtypes = new CodeValueModel().ExtraValueTypeCodes();
            var adhoctypes = new CodeValueModel().AdhocExtraValueTypeCodes();

            var qcodevalues = (from e in DbUtil.Db.FamilyExtras
                               where e.Type == "Bit" || e.Type == "Code"
                               group e by new
                               {
                                   e.Field,
                                   val = e.StrValue ?? (e.BitValue == true ? "1" : "0"),
                                   e.Type,
                               } into g
                               select new { key = g.Key, count = g.Count() }).ToList();

            var qcodes = from i in qcodevalues
                         join sv in NameTypes on i.key.Field equals sv.Name into j
                         from sv in j.DefaultIfEmpty()
                         let type = sv == null ? i.key.Type : sv.Type
                         let typobj = sv == null
                                ? adhoctypes.SingleOrDefault(ee => ee.Code == type)
                                : standardtypes.SingleOrDefault(ee => ee.Code == type)
                         let typedisplay = typobj == null ? "unknown" : typobj.Value
                         select new ExtraInfoFamily
                         {
                             Field = i.key.Field,
                             Value = i.key.val,
                             Type = i.key.Type,
                             TypeDisplay = typedisplay,
                             Standard = sv != null,
                             Count = i.count,
                             CanView = sv == null || sv.CanView
                         };

            var qdatavalues = (from e in DbUtil.Db.FamilyExtras
                               where !(e.Type == "Bit" || e.Type == "Code")
                               where e.Type != "CodeText"
                               group e by new
                               {
                                   e.Field,
                                   e.Type,
                               } into g
                               select new { key = g.Key, count = g.Count() }).ToList();

            var qdatums = from i in qdatavalues
                          join sv in NameTypes on i.key.Field equals sv.Name into j
                          from sv in j.DefaultIfEmpty()
                          let type = sv == null ? i.key.Type : sv.Type
                          let typedisplay = sv == null
                                ? adhoctypes.SingleOrDefault(ee => ee.Code == type)
                                : standardtypes.SingleOrDefault(ee => ee.Code == type)
                          select new ExtraInfoFamily
                          {
                              Field = i.key.Field,
                              Value = "(multiple)",
                              Type = i.key.Type,
                              TypeDisplay = typedisplay == null ? (type ?? "unknown") : typedisplay.Value,
                              Standard = sv != null,
                              Count = i.count,
                              CanView = sv == null || sv.CanView
                          };

            return qcodes.Union(qdatums).OrderBy(ee => ee.Field);
        }
        public override string DeleteAll(string type, string field, string value)
        {
            var ev = DbUtil.Db.FamilyExtras.FirstOrDefault(ee => ee.Field == field);
            if (ev == null)
            {
                return "error: no field";
            }

            switch (type.ToLower())
            {
                case "code":
                    DbUtil.Db.ExecuteCommand("delete FamilyExtra where field = {0} and StrValue = {1}", field, value);
                    break;
                case "bit":
                    DbUtil.Db.ExecuteCommand("delete FamilyExtra where field = {0} and BitValue = {1}", field, value);
                    break;
                case "int":
                    DbUtil.Db.ExecuteCommand("delete FamilyExtra where field = {0} and IntValue is not null", field);
                    break;
                case "date":
                    DbUtil.Db.ExecuteCommand("delete FamilyExtra where field = {0} and DateValue is not null", field);
                    break;
                case "text":
                    DbUtil.Db.ExecuteCommand("delete FamilyExtra where field = {0} and Data is not null", field);
                    break;
                case "?":
                    DbUtil.Db.ExecuteCommand(
                        "delete FamilyExtra where field = {0} and data is null and datevalue is null and intvalue is null",
                        field);
                    break;
            }
            DbUtil.LogActivity($"EVFamily DeleteAll {field} {value}");
            return "done";
        }

        public override void RenameAll(string field, string newname)
        {
            DbUtil.Db.ExecuteCommand("update FamilyExtra set field = {0} where field = {1}", newname, field);
            DbUtil.LogActivity($"EVFamily RenameAll {field}>{newname}");
        }
    }
}
