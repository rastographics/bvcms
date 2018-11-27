using CmsData;
using CmsData.View;
using CmsWeb.Models;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class ChangesModel : PagedTableModel<ChangeLogDetail, ChangeLogInfo>
    {
        public int PeopleId
        {
            get { return peopleid; }
            set
            {
                peopleid = value;
                Person = DbUtil.Db.LoadPersonById(peopleid);
            }
        }
        private int peopleid;
        public Person Person;

        public ChangesModel()
            : base("Time", "desc", true)
        { }

        public void Reverse(string field, string value, string pf)
        {
            switch (pf)
            {
                case "p":
                    Person.UpdateValueFromText(field, value);
                    Person.LogChanges(DbUtil.Db, Util.UserPeopleId ?? 0);
                    break;
                case "f":
                    Person.Family.UpdateValueFromText(field, value);
                    Person.Family.LogChanges(DbUtil.Db, PeopleId, Util.UserPeopleId ?? 0);
                    break;
            }
            DbUtil.Db.SubmitChanges();
        }

        public override IQueryable<ChangeLogDetail> DefineModelList()
        {
            return from c in DbUtil.Db.ViewChangeLogDetails
                   where c.PeopleId == PeopleId || c.FamilyId == Person.FamilyId
                   select c;
        }
        public override IQueryable<ChangeLogDetail> DefineModelSort(IQueryable<ChangeLogDetail> q)
        {
            switch (SortExpression)
            {
                case "Time":
                    return q.OrderBy(a => a.Created);
                case "Time desc":
                    return q.OrderByDescending(a => a.Created);
            }
            return q;
        }

        public override IEnumerable<ChangeLogInfo> DefineViewList(IQueryable<ChangeLogDetail> q)
        {
            var q1 = (from c in q
                      let userp = DbUtil.Db.People.SingleOrDefault(u => u.PeopleId == c.UserPeopleId)
                      select new ChangeLogInfo()
                      {
                          User = userp.Name,
                          FieldSet = c.Section,
                          Time = c.Created,
                          Field = c.Field,
                          Before = c.Before,
                          After = c.After,
                      }).ToList();
            foreach (var i in q1)
            {
                i.pf = PersonOrFamily(i.FieldSet);
                i.Reversable = FieldEqual(i.pf, i.Field, i.After);
            }
            return q1;
        }

        private bool FieldEqual(CmsData.Person p, string field, string value)
        {
            if (value is string)
            {
                value = ((string)value).TrimEnd();
            }

            if (!Util.HasProperty(p, field))
            {
                return false;
            }

            var o = Util.GetProperty(p, field);
            if (o is string)
            {
                o = ((string)o).TrimEnd();
            }

            var p2 = new CmsData.Person();
            Util.SetPropertyFromText(p2, field, value);
            var o2 = Util.GetProperty(p2, field);
            if (o == o2)
            {
                return true;
            }

            if (o.IsNull() && o2.IsNotNull())
            {
                return false;
            }

            return o.Equals(o2);
        }
        private bool FieldEqual(Family f, string field, string value)
        {
            if (value is string)
            {
                value = ((string)value).TrimEnd();
            }

            if (!Util.HasProperty(f, field))
            {
                return false;
            }

            var o = Util.GetProperty(f, field);
            if (o is string)
            {
                o = ((string)o).TrimEnd();
            }

            var f2 = new Family();
            Util.SetPropertyFromText(f2, field, value);
            var o2 = Util.GetProperty(f2, field);
            if (o == o2)
            {
                return true;
            }

            if (o.IsNull() && o2.IsNotNull())
            {
                return false;
            }

            return o.Equals(o2);
        }
        private bool FieldEqual(string pf, string field, string value)
        {
            switch (pf)
            {
                case "p":
                    if (field == "Picture")
                    {
                        return false;
                    }

                    return FieldEqual(Person, field, value);
                case "f":
                    return FieldEqual(Person.Family, field, value);
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
