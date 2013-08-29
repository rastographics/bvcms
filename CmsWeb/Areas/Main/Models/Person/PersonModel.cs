using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using UtilityExtensions;
using System.Text.RegularExpressions;
using System.Text;

namespace CmsWeb.Models.PersonPage
{
    public class PersonModel
    {
        public PersonInfo displayperson;
        public PersonModel(int? id)
        {
            displayperson = PersonInfo.GetPersonInfo(id);
        }
        private Person _person;
        public Person Person
        {
            get
            {
                if (_person == null && displayperson != null)
                    _person = DbUtil.Db.LoadPersonById(displayperson.PeopleId);
                return _person;
            }
        }

        public string Name
        {
            get { return displayperson.Name; }
            set { displayperson.Name = value; }
        }
        public int? ckorg;
        public bool CanCheckIn
        {
            get
            {
                ckorg = (int?)HttpContext.Current.Session["CheckInOrgId"];
                return ckorg.HasValue;
            }
        }
        public string addrtab { get { return displayperson.PrimaryAddr.Name; } }

        public bool FieldEqual(Person p, string field, string value)
        {
            if (value is string)
                value = ((string)value).TrimEnd();
            if (!Util.HasProperty(p, field))
                return false;
            var o = Util.GetProperty(p, field);
            if (o is string)
                o = ((string)o).TrimEnd();
            var p2 = new Person();
	        Util.SetPropertyFromText(p2, field, value);
            var o2 = Util.GetProperty(p2, field);
            if (o == o2)
                return true;
            if (o.IsNull() && o2.IsNotNull())
                return false;
            return o.Equals(o2);
        }
        public bool FieldEqual(Family f, string field, string value)
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
        public bool FieldEqual(string pf, string field, string value)
        {
            switch (pf)
            {
                case "p":
					if (field == "Picture")
						return false;
                    return FieldEqual(this.Person, field, value);
                case "f":
                    return FieldEqual(this.Person.Family, field, value);
            }
            return false;
        }
        public string PersonOrFamily(string FieldSet)
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

        public class ChangeLogInfo
        {
            public string User { get; set; }
            public string FieldSet { get; set; }
            public DateTime? Time { get; set; }
            public string Field { get; set; }
            public string Before { get; set; }
            public string After { get; set; }
            public string pf { get; set; }
            public bool Reversable { get; set; }
        }
        public List<ChangeLogInfo> details(ChangeLog log, string name)
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
        public IEnumerable<ChangeLogInfo> GetChangeLogs()
        {
            var list = (from c in DbUtil.Db.ChangeLogs
                        let userp = DbUtil.Db.People.SingleOrDefault(u => u.PeopleId == c.UserPeopleId)
                        where c.PeopleId == Person.PeopleId || c.FamilyId == Person.FamilyId
                        where userp != null
                        select new { userp, c }).ToList();
            var q = from i in list
                    orderby i.c.Created descending
                    from d in details(i.c, i.userp.Name)
                    select d;
            return q;
        }
        public void Reverse(string field, string value, string pf)
        {
            switch(pf)
            {
                case "p":
                    Person.UpdateValueFromText(field, value);
                    Person.LogChanges(DbUtil.Db, Util.UserPeopleId.Value);
                    break;
                case "f":
                    Person.Family.UpdateValueFromText(field, value);
                    Person.Family.LogChanges(DbUtil.Db, Person.PeopleId, Util.UserPeopleId.Value);
                    break;
            }
            DbUtil.Db.SubmitChanges();
        }
    }
}
