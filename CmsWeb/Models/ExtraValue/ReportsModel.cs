using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class ReportsModel
    {
        public static IEnumerable<ExtraInfo> CodeSummary()
        {
            var NameTypes = Views.GetViewableNameTypes("People", nocache: true);
            var standardtypes = new CodeValueModel().ExtraValueTypeCodes();
            var adhoctypes = new CodeValueModel().AdhocExtraValueTypeCodes();

            var qcodevalues = (from e in DbUtil.Db.PeopleExtras
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
                         let typedisplay = sv == null 
                                ? adhoctypes.Single(ee => ee.Code == type).Value 
                                : standardtypes.Single(ee => ee.Code == type).Value 
                         select new ExtraInfo
                         {
                             Field = i.key.Field,
                             Value = i.key.val,
                             Type = i.key.Type,
                             TypeDisplay = typedisplay,
                             Standard = sv != null,
                             Count = i.count,
                         };

            var qdatavalues = (from e in DbUtil.Db.PeopleExtras
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
                                ? adhoctypes.Single(ee => ee.Code == type)
                                : standardtypes.Single(ee => ee.Code == type)
                          select new ExtraInfo
                          {
                              Field = i.key.Field,
                              Value = "(multiple)",
                              Type = i.key.Type,
                              TypeDisplay = typedisplay.Value,
                              Standard = sv != null,
                              Count = i.count,
                          };

            return qcodes.Union(qdatums).OrderBy(ee => ee.Field);
        }

        public static Condition QueryCodesCondition(string field, string value)
        {
            var c = DbUtil.Db.ScratchPadCondition();
            c.Reset(DbUtil.Db);
            c.AddNewClause(QueryType.PeopleExtra, CompareType.Equal, "{0}:{1}".Fmt(field, value));
            c.Save(DbUtil.Db);
            return c;
        }
        public static Condition QueryDataCondition(string field, string type)
        {
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset(DbUtil.Db);
            Condition c = null;

            switch (type.ToLower())
            {
                case "text":
                    c = cc.AddNewClause(QueryType.PeopleExtraData, CompareType.NotEqual, "");
                    c.Quarters = field;
                    break;
                case "date":
                    c = cc.AddNewClause(QueryType.PeopleExtraDate, CompareType.NotEqual, null);
                    c.Quarters = field;
                    break;
                case "int":
                    c = cc.AddNewClause(QueryType.PeopleExtraInt, CompareType.NotEqual, "");
                    c.Quarters = field;
                    break;
                case "?":
                    cc.AddNewClause(QueryType.HasPeopleExtraField, CompareType.Equal, field);
                    break;
            }
            cc.Save(DbUtil.Db);
            return cc;
        }
        public static SqlDataReader GridReader(Guid id, string sort)
        {
            var roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);
            var values = from value in Views.GetStandardExtraValues("People")
                         where value.VisibilityRoles != null && (value.VisibilityRoles.Split(',').All(rr => !roles.Contains(rr)))
                         select value.Name;
            var nodisplaycols = string.Join("|", values);

            var tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_ExtraValues);
            var cmd = new SqlCommand("dbo.ExtraValues @p1, @p2, @p3");
            cmd.Parameters.AddWithValue("@p1", tag.Id);
            cmd.Parameters.AddWithValue("@p2", sort ?? "");
            cmd.Parameters.AddWithValue("@p3", nodisplaycols);
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();
            var rdr = cmd.ExecuteReader();
            return rdr;
        }
        public static SqlDataReader Grid2Reader(Guid id, string sort)
        {
            var roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);
            var values = from value in Views.GetStandardExtraValues("People")
                         where value.VisibilityRoles != null && (value.VisibilityRoles.Split(',').All(rr => !roles.Contains(rr)))
                         select value.Name;
            var nodisplaycols = string.Join("|", values);

            var tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_ExtraValues);
            var cmd = new SqlCommand("dbo.ExtraValues @p1, @p2, @p3");
            cmd.Parameters.AddWithValue("@p1", tag.Id);
            cmd.Parameters.AddWithValue("@p2", sort ?? "");
            cmd.Parameters.AddWithValue("@p3", nodisplaycols);
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();
            var rdr = cmd.ExecuteReader();
            return rdr;
        }
        public static string DeleteAll(string field, string type, string value)
        {
            var ev = DbUtil.Db.PeopleExtras.FirstOrDefault(ee => ee.Field == field);
            if (ev == null)
                return "error: no field";
            switch (type.ToLower())
            {
                case "code":
                    DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and StrValue = {1}", field, value);
                    break;
                case "bit":
                    DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and BitValue = {1}", field, value);
                    break;
                case "int":
                    DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and IntValue is not null", field);
                    break;
                case "date":
                    DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and DateValue is not null", field);
                    break;
                case "text":
                    DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and Data is not null", field);
                    break;
                case "?":
                    DbUtil.Db.ExecuteCommand(
                        "delete PeopleExtra where field = {0} and data is null and datevalue is null and intvalue is null",
                        field);
                    break;
            }
            return "done";
        }
        public static void RenameAll(string field, string newname)
        {
            DbUtil.Db.ExecuteCommand("update PeopleExtra set field = {0} where field = {1}", newname, field);
        }
    }
}