using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models.Person
{
    public class ChangesModel : PagedTableModel<ChangeLog, ChangeLogInfo>
    {
        public readonly CmsData.Person person;
        public ChangesModel(int id)
            : base("Time", "desc")
        {
            person = DbUtil.Db.LoadPersonById(id);
        }
        public void Reverse(string field, string value, string pf)
        {
            switch (pf)
            {
                case "p":
                    person.UpdateValueFromText(field, value);
                    person.LogChanges(DbUtil.Db, Util.UserPeopleId.Value);
                    break;
                case "f":
                    person.Family.UpdateValueFromText(field, value);
                    person.Family.LogChanges(DbUtil.Db, person.PeopleId, Util.UserPeopleId.Value);
                    break;
            }
            DbUtil.Db.SubmitChanges();
        }

        override public IQueryable<ChangeLog> DefineModelList()
        {
            return from c in DbUtil.Db.ChangeLogs
                   where c.PeopleId == person.PeopleId || c.FamilyId == person.FamilyId
                   select c;
        }
        private IEnumerable<ChangeLogInfo> Details(ChangeLog log, string name)
        {
            var list = new List<ChangeLogInfo>();
            var re = new Regex("<tr><td>(?<field>[^<]+)</td><td>(?<before>[^<]*)</td><td>(?<after>[^<]*)</td></tr>", RegexOptions.Singleline);
            Match matchResult = re.Match(log.Data);
            var FieldSet = log.Field;
            var pf = PersonOrFamily(FieldSet);
            DateTime? Time = log.Created;
            while (matchResult.Success)
            {
                var After = matchResult.Groups["after"].Value;
                var Field = matchResult.Groups["field"].Value;
                var c = new ChangeLogInfo
                {
                    User = name,
                    FieldSet = FieldSet,
                    Time = Time,
                    Field = Field,
                    Before = matchResult.Groups["before"].Value,
                    After = After,
                    pf = pf,
                    Reversable = FieldEqual(pf, Field, After)
                };
                list.Add(c);
                FieldSet = "";
                name = "";
                Time = null;
                matchResult = matchResult.NextMatch();
            }
            return list;
        }
        override public IQueryable<ChangeLog> DefineModelSort(IQueryable<ChangeLog> q)
        {
            switch (Pager.SortExpression)
            {
                case "Time":
                    return q.OrderBy(a => a.Created);
                case "Time desc":
                    return q.OrderByDescending(a => a.Created);
            }
            return null;
        }

        public override IEnumerable<ChangeLogInfo> DefineViewList(IQueryable<ChangeLog> q)
        {
            return from c in q
                   let userp = DbUtil.Db.People.SingleOrDefault(u => u.PeopleId == c.UserPeopleId)
                   from d in Details(c, userp.Name)
                   select d;
        }

        private bool FieldEqual(CmsData.Person p, string field, string value)
        {
            if (value is string)
                value = ((string)value).TrimEnd();
            if (!Util.HasProperty(p, field))
                return false;
            var o = Util.GetProperty(p, field);
            if (o is string)
                o = ((string)o).TrimEnd();
            var p2 = new CmsData.Person();
            Util.SetPropertyFromText(p2, field, value);
            var o2 = Util.GetProperty(p2, field);
            if (o == o2)
                return true;
            if (o.IsNull() && o2.IsNotNull())
                return false;
            return o.Equals(o2);
        }
        private bool FieldEqual(Family f, string field, string value)
        {
            if (value is string)
                value = ((string)value).TrimEnd();
            if (!Util.HasProperty(f, field))
                return false;
            var o = Util.GetProperty(f, field);
            if (o is string)
                o = ((string)o).TrimEnd();
            var f2 = new Family();
            Util.SetPropertyFromText(f2, field, value);
            var o2 = Util.GetProperty(f2, field);
            if (o == o2)
                return true;
            if (o.IsNull() && o2.IsNotNull())
                return false;
            return o.Equals(o2);
        }
        private bool FieldEqual(string pf, string field, string value)
        {
            switch (pf)
            {
                case "p":
                    if (field == "Picture")
                        return false;
                    return FieldEqual(person, field, value);
                case "f":
                    return FieldEqual(person.Family, field, value);
            }
            return false;
        }
        private string PersonOrFamily(string FieldSet)
        {
            switch (FieldSet)
            {
                case "HomePhone":
                case "Basic Info":
                case "PersonalAddr":
                    return "p";
                case "Family":
                case "FamilyAddr":
                    return "f";
            }
            return "n";
        }
    }
}
