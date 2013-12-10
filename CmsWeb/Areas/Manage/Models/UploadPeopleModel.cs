using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using CmsData;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class UploadPeopleModel
    {
        private Dictionary<string, int> names;
        private StringBuilder psb;
        private StringBuilder fsb;
        private CMSDataContext Db;
        private CMSDataContext Db2;
        private int PeopleId;

        public UploadPeopleModel(CMSDataContext Db, int PeopleId, string connectionstring)
        {
            this.Db = Db;
            Db2 = new CMSDataContext(connectionstring);
            this.PeopleId = PeopleId;
        }

        private void UpdateField(Family f, string[] a, string prop, string s)
        {
            if (names.ContainsKey(s))
                if (a[names[s]].HasValue())
                    f.UpdateValue(fsb, prop, a[names[s]]);
        }

        private void UpdateField(Person p, string[] a, string prop, string s)
        {
            if (names.ContainsKey(s))
                if (a[names[s]].HasValue())
                    p.UpdateValue(psb, prop, a[names[s]]);
        }

        private void UpdateField(Person p, string[] a, string prop, string s, object value)
        {
            if (names.ContainsKey(s))
                if (value != null)
                    p.UpdateValue(psb, prop, value);
        }

        private void SetField(Family f, string[] a, string prop, string s)
        {
            if (names.ContainsKey(s))
                if (a[names[s]].HasValue())
                    Util.SetProperty(f, prop, a[names[s]]);
        }

        private void SetField(Person p, string[] a, string prop, string s)
        {
            if (names.ContainsKey(s))
                if (a[names[s]].HasValue())
                    Util.SetProperty(p, prop, a[names[s]]);
        }

        private void SetField(Person p, string[] a, string prop, string s, object value)
        {
            if (names.ContainsKey(s))
                if (value != null)
                    Util.SetProperty(p, prop, value);
        }
        private void SetField(Family f, string[] a, string prop, string s, object value)
        {
            if (names.ContainsKey(s))
                if (value != null)
                    Util.SetProperty(f, prop, value);
        }

        private string GetDigits(string[] a, string s)
        {
            if (names.ContainsKey(s))
                if (a[names[s]].HasValue())
                    return a[names[s]].GetDigits();
            return "";
        }

        private DateTime? GetDate(Person p, string[] a, string s)
        {
            if (names.ContainsKey(s))
                if (a[names[s]].HasValue())
                {
                    DateTime dt;
                    if (DateTime.TryParse(a[names[s]], out dt))
                    {
                        if (dt.Year < 1800)
                            throw new Exception("error on {0} {1}: [{2}]({3})".Fmt(p.FirstName, p.LastName, names[s], a[names[s]]));
                        return dt;
                    }
                }
            return null;
        }

        private int Gender(string[] a)
        {
            if (names.ContainsKey("gender"))
                if (a[names["gender"]].HasValue())
                {
                    var v = a[names["gender"]].ToLower().TrimEnd();
                    switch (v)
                    {
                        case "male":
                        case "m":
                            return 1;
                        case "female":
                        case "f":
                            return 2;
                    }
                }
            return 0;
        }

        private int Marital(string[] a)
        {
            if (names.ContainsKey("marital"))
                if (a[names["marital"]].HasValue())
                {
                    var v = a[names["marital"]].ToLower().TrimEnd();
                    switch (v)
                    {
                        case "married":
                        case "m":
                            return 20;
                        case "single":
                        case "s":
                            return 10;
                        case "widowed":
                        case "w":
                            return 50;
                        case "divorced":
                        case "d":
                            return 40;
                        case "separated":
                            return 30;
                    }
                }
            return 0;
        }

        private string Title(string[] a)
        {
            if (names.ContainsKey("title"))
                if (a[names["title"]].HasValue())
                    return a[names["title"]].Truncate(10).TrimEnd();
            return null;
        }

        private int Position(string[] a)
        {
            if (names.ContainsKey("position"))
                if (a[names["position"]].HasValue())
                {
                    var v = a[names["position"]].ToLower().TrimEnd();
                    switch (v)
                    {
                        case "primary":
                            return 10;
                        case "secondary":
                            return 20;
                        case "child":
                            return 30;
                    }
                }
            return 10;
        }
        private int? Campus(string[] a)
        {
            if (names.ContainsKey("campus"))
                if (a[names["campus"]].HasValue())
                    return Campuses[a[names["campus"]]];
            return null;
        }

        private Dictionary<string, int> Campuses;
        public bool DoUpload(string text, bool testing = false)
        {
            var rt = Db2.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();
            var csv = new CsvReader(new StringReader(text), false, '\t');
            var list = csv.ToList();

            var list0 = list.First().Select(kk => kk.ToLower()).ToList();
            names = list0.ToDictionary(i => i.TrimEnd(),
                                       i => list0.FindIndex(s => s == i));

            if (names.ContainsKey("campus"))
            {
                var campuslist = (from li in list.Skip(1)
                                  where li.Length == names.Count
                                  group li by li[names["campus"]]
                                      into campus
                                      where campus.Key.HasValue()
                                      select campus.Key).ToList();
                var dbc = from c in campuslist
                          join cp in Db.Campus on c equals cp.Description into j
                          from cp in j.DefaultIfEmpty()
                          select new { cp, c };
                var clist = dbc.ToList();
                if (clist.Count > 0)
                {
                    var maxcampusid = Db.Campus.Max(c => c.Id);
                    foreach (var i in clist)
                        if (i.cp == null)
                        {
                            var cp = new Campu { Description = i.c, Id = ++maxcampusid };
                            if (!testing)
                                Db.Campus.InsertOnSubmit(cp);
                        }
                }
            }
            var now = DateTime.Now;
            if (!testing)
                Db.SubmitChanges();
            Campuses = Db.Campus.ToDictionary(cp => cp.Description, cp => cp.Id);

            var q = (from li in list.Skip(1)
                     where li.Length == names.Count
                     group li by li[names["familyid"]]
                         into fam
                         select fam).ToList();
            rt.Count = q.Sum(ff => ff.Count());
            Db2.SubmitChanges();

            var standardnames = new List<string>
        	{
        		"familyid", "title", "first", "last", "goesby", "altname", "gender", "marital", "maidenName",
        		"address", "address2", "city", "state", "zip", "position", "birthday", "cellphone", "homephone",
        		"workphone", "email", "email2", "suffix", "middle", "joindate", "dropdate", "baptismdate", "weddingdate",
        		"memberstatus", "employer", "occupation",
        	};

            foreach (var fam in q)
            {
                Family f = null;

                foreach (var a in fam)
                {
                    var first = a[names["first"]];
                    var last = a[names["last"]];
                    DateTime dt;
                    DateTime? dob = null;
                    if (names.ContainsKey("birthday"))
                        if (DateTime.TryParse(a[names["birthday"]], out dt))
                        {
                            dob = dt;
                            if (dob.Value < SqlDateTime.MinValue)
                                dob = null;
                        }
                    string email = null;
                    string cell = null;
                    string homephone = null;
                    if (names.ContainsKey("email"))
                        email = a[names["email"]].Trim();
                    if (names.ContainsKey("cellphone"))
                        cell = a[names["cellphone"]].GetDigits();
                    if (names.ContainsKey("homephone"))
                        homephone = a[names["homephone"]].GetDigits();
                    Person p = null;
                    var pid = Db.FindPerson3(first, last, dob, email, cell, homephone, null).FirstOrDefault();

                    if (pid != null) // found
                    {
                        p = Db.LoadPersonById(pid.PeopleId.Value);
                        psb = new StringBuilder();
                        fsb = new StringBuilder();

                        UpdateField(p, a, "TitleCode", "title");
                        UpdateField(p, a, "FirstName", "first");
                        UpdateField(p, a, "NickName", "goesby");
                        UpdateField(p, a, "LastName", "last");
                        UpdateField(p, a, "EmailAddress", "email");
                        UpdateField(p, a, "EmailAddress2", "email2");
                        UpdateField(p, a, "DOB", "birthday");
                        UpdateField(p, a, "AltName", "altname");
                        UpdateField(p, a, "SuffixCode", "suffix");
                        UpdateField(p, a, "MiddleName", "middle");

                        UpdateField(p, a, "CellPhone", "cellphone", GetDigits(a, "cellphone"));
                        UpdateField(p, a, "WorkPhone", "workphone", GetDigits(a, "workphone"));
                        UpdateField(p, a, "GenderId", "gender", Gender(a));
                        UpdateField(p, a, "MaritalStatusId", "marital", Marital(a));
                        UpdateField(p, a, "PositionInFamilyId", "position", Position(a));
                        UpdateField(p, a, "CampusId", "campus", Campus(a));

                        UpdateField(p.Family, a, "AddressLineOne", "address");
                        UpdateField(p.Family, a, "AddressLineTwo", "address2");
                        UpdateField(p.Family, a, "CityName", "city");
                        UpdateField(p.Family, a, "StateCode", "state");
                        UpdateField(p.Family, a, "ZipCode", "zip");

                        if (!testing)
                        {
                            p.LogChanges(Db, psb.ToString(), PeopleId);
                            p.Family.LogChanges(Db, fsb.ToString(), p.PeopleId, PeopleId);
                            Db.SubmitChanges();
                            Person.Tag(Db, p.PeopleId, "InsertPeopleUpdated", Util.UserPeopleId, DbUtil.TagTypeId_Personal);
                        }
                    }
                    else // new person
                    {
                        try
                        {

                            if (f == null || !a[names["familyid"]].HasValue())
                            {
                                f = new Family();
                                SetField(f, a, "AddressLineOne", "address");
                                SetField(f, a, "AddressLineTwo", "address2");
                                SetField(f, a, "CityName", "city");
                                SetField(f, a, "StateCode", "state");
                                SetField(f, a, "ZipCode", "zip");
                                SetField(f, a, "HomePhone", "homephone", GetDigits(a, "homephone"));
                                Db.Families.InsertOnSubmit(f);
                                if (!testing)
                                    Db.SubmitChanges();
                            }

                            string goesby = null;
                            if (names.ContainsKey("goesby"))
                                goesby = a[names["goesby"]];
                            p = Person.Add(Db, false, f, 10, null,
                                           a[names["first"]],
                                           goesby,
                                           a[names["last"]],
                                           dob.FormatDate(),
                                           0, 0, 0, null, testing);
                            p.FixTitle();

                            SetField(p, a, "AltName", "altname");
                            SetField(p, a, "SuffixCode", "suffix");
                            SetField(p, a, "MiddleName", "middle");
                            SetField(p, a, "MaidenName", "maidenname");
                            SetField(p, a, "EmployerOther", "employer");
                            SetField(p, a, "OccupationOther", "occupation");
                            SetField(p, a, "CellPhone", "cellphone", GetDigits(a, "cellphone"));
                            SetField(p, a, "WorkPhone", "workphone", GetDigits(a, "workphone"));
                            SetField(p, a, "EmailAddress", "email");
                            SetField(p, a, "EmailAddress2", "email2");
                            SetField(p, a, "GenderId", "gender", Gender(a));
                            SetField(p, a, "MaritalStatusId", "marital", Marital(a));
                            SetField(p, a, "WeddingDate", "weddingdate", GetDate(p, a, "weddingdate"));
                            SetField(p, a, "JoinDate", "joindate", GetDate(p, a, "joindate"));
                            SetField(p, a, "DropDate", "dropdate", GetDate(p, a, "dropdate"));
                            SetField(p, a, "BaptismDate", "baptismdate", GetDate(p, a, "baptismdate"));
                            SetField(p, a, "PositionInFamilyId", "position", Position(a));
                            SetField(p, a, "TitleCode", "title", Title(a));
                            SetField(p, a, "CampusId", "campus", Campus(a));

                            if (names.ContainsKey("memberstatus"))
                            {
                                var ms = a[names["memberstatus"]];
                                var qms = from mm in Db.MemberStatuses
                                          where mm.Description == ms
                                          select mm;
                                var m = qms.SingleOrDefault();
                                if (m == null)
                                {
                                    var nx = Db.MemberStatuses.Max(mm => mm.Id) + 1;
                                    m = new MemberStatus { Id = nx, Description = ms, Code = nx.ToString() };
                                    Db.MemberStatuses.InsertOnSubmit(m);
                                }
                                p.MemberStatusId = m.Id;
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                    try
                    {
                        var nq = from name in names.Keys
                                 where !standardnames.Contains(name)
                                 select name;
                        foreach (var name in nq)
                        {
                            var b = name.Split('.');
                            if (name.EndsWith(".txt"))
                                p.AddEditExtraData(b[0], a[names[name]].Trim());
                            else if (name.EndsWith(".dt"))
                            {
                                var d = a[names[name]].Trim().ToDate();
                                if (d.HasValue)
                                    p.AddEditExtraDate(b[0], d.Value);
                            }
                            else if (name.EndsWith(".int"))
                                p.AddEditExtraInt(b[0], a[names[name]].Trim().ToInt());
                            else if (name.EndsWith(".bit"))
                            {
                                var v = a[names[name]];
                                if (v.HasValue())
                                    p.AddEditExtraBool(b[0], v.ToInt() == 1);
                            }
                            else
                                p.AddEditExtraValue(name, a[names[name]].Trim());
                        }
                        rt.Processed++;
                        Db2.SubmitChanges();
                        if (!testing)
                        {
                            Person.Tag(Db, p.PeopleId, "InsertPeopleAdded", Util.UserPeopleId, DbUtil.TagTypeId_Personal);
                            Db.SubmitChanges();
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                if (!testing)
                    Db.SubmitChanges();
            }
            rt.Completed = DateTime.Now;
            Db2.SubmitChanges();
            return true;
        }
    }
}