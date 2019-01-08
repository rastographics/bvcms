using CmsData;
using CmsData.ExtraValue;
using System;
using System.Data.SqlClient;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class ReportsModel
    {
        public ReportsModel() { }
        public static Condition QueryCodesCondition(string field, string value)
        {
            var c = DbUtil.Db.ScratchPadCondition();
            c.Reset();
            c.AddNewClause(QueryType.PeopleExtra, CompareType.Equal, $"{field}:{value}");
            c.Save(DbUtil.Db);
            return c;
        }

        public static Condition FamilyQueryCodesCondition(string field, string value)
        {
            var c = DbUtil.Db.ScratchPadCondition();
            c.Reset();
            c.AddNewClause(QueryType.FamilyExtra, CompareType.Equal, $"{field}:{value}");
            c.Save(DbUtil.Db);
            return c;
        }

        public static Condition QueryDataCondition(string field, string type)
        {
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset();
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

        public static Condition FamilyQueryDataCondition(string field, string type)
        {
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset();
            Condition c = null;

            switch (type.ToLower())
            {
                case "text":
                    c = cc.AddNewClause(QueryType.FamilyExtraData, CompareType.NotEqual, "");
                    c.Quarters = field;
                    break;
                case "date":
                    c = cc.AddNewClause(QueryType.FamilyExtraDate, CompareType.NotEqual, null);
                    c.Quarters = field;
                    break;
                case "int":
                    c = cc.AddNewClause(QueryType.FamilyExtraInt, CompareType.NotEqual, "");
                    c.Quarters = field;
                    break;
                case "?":
                    cc.AddNewClause(QueryType.HasFamilyExtraField, CompareType.Equal, field);
                    break;
            }
            cc.Save(DbUtil.Db);
            return cc;
        }

        public static SqlDataReader GridReader(Guid id, string sort)
        {
            var roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);
            var values = from value in Views.GetStandardExtraValues(DbUtil.Db, "People")
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
    }
}
