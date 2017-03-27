using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using CmsData;
using CmsData.API;
using CmsData.View;
using OfficeOpenXml;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class UploadExcelIpsModel
    {
        private readonly CMSDataContext Db2;
        private readonly string host;
        private readonly bool noupdate;
        private readonly int PeopleId;
        private Dictionary<string, int> Campuses;
        private List<ChangeDetail> fsb;
        private Dictionary<string, int> names;
        private List<ChangeDetail> psb;

        public bool InsertPeopleSpreadsheet => true;

        public UploadExcelIpsModel(string host, int peopleId, bool noupdate, bool testing = false)
        {
            Db2 = DbUtil.Create(host);
            PeopleId = peopleId;
            this.noupdate = noupdate;
            this.testing = testing;
            this.host = host;
        }

        private void UpdateField(Family f, string prop, dynamic a)
        {
            var v = a as object;
            if(v != null)
                f.UpdateValue(fsb, prop, v);
        }

        private void UpdateField(Person p, string prop, dynamic a)
        {
            var v = a as object;
            if(v != null)
                p.UpdateValue(psb, prop, v);
        }

        private string GetDigits(dynamic v)
        {
            var s = v as string;
            return s.HasValue() ? s.GetDigits() : null;
        }

        private string GetString(dynamic v)
        {
            var o = v as object;
            return o?.ToString();
        }

        private int Gender(dynamic v)
        {
            var s = v as string;
            s = s.trim()?.ToLower();
            switch (s)
            {
                case "male":
                case "m":
                    return 1;
                case "female":
                case "f":
                    return 2;
            }
            return 0;
        }

        private int Marital(dynamic v)
        {
            var s = v as string;
            s = s.trim()?.ToLower();
            switch (s)
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
            return 0;
        }

        private string Title(dynamic v)
        {
            var s = v as string;
            s = s.trim()?.ToLower();
            return !s.HasValue() ? s : s.Truncate(10).TrimEnd();
        }

        private int Position(dynamic v)
        {
            var s = v as string;
            s = s.trim()?.ToLower();
            switch (s)
            {
                case "primary":
                    return 10;
                case "secondary":
                    return 20;
                case "child":
                    return 30;
            }
            return 10;
        }

        private int? Campus(dynamic v)
        {
            var s = v as string;
            if (s == null)
                return null;
            s = s.trim().ToLower();
            return Campuses[s];
        }

        private List<string> standardnames = new List<string>
        {
            "familyid", "title", "first", "last", "goesby", "altname",
            "gender", "marital", "maidenName", "address", "address2",
            "city", "state", "zip", "position", "birthday", "deceased",
            "cellphone", "homephone", "workphone", "email", "email2",
            "suffix", "middle", "joindate", "dropdate", "baptismdate", "weddingdate",
            "memberstatus", "employer", "occupation", "CreatedDate", "BackgroundCheck",
            "individualid", "campus"
        };

        private readonly List<string> standardrecregnames = new List<string>
        {
            "Mother", "Father", "EmContact", "EmPhone", "Allergies",
            "Grade", "School", "Doctor", "DocPhone", "Insurance", "Policy",
        };

        readonly Dictionary<string, int> orgs = new Dictionary<string, int>();
        readonly Dictionary<string, int> membertypes = new Dictionary<string, int>();
        private List<string> extravaluenames;
        private List<string> recregnames;
        public bool testing;

        public bool DoUpload(ExcelPackage pkg)
        {
            var peopleWs = pkg.Workbook.Worksheets["Personal Data"];
            var giftsWs = pkg.Workbook.Worksheets["Gift Data"];
            var pledgesWs = pkg.Workbook.Worksheets["Pledges"];

            var rt = Db2.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();
            FetchData(peopleWs);
            names = peopleWs.GetHeaderColumns();

            extravaluenames = (from name in names
                               where !standardnames.Contains(name.Key, StringComparer.OrdinalIgnoreCase)
                               where !standardrecregnames.Contains(name.Key)
                               select name.Key).ToList();
            recregnames = (from name in names
                           where standardrecregnames.Contains(name.Key)
                           select name.Key).ToList();

            var db = DbUtil.Create(host);
            if (names.ContainsKey("Campus"))
            {
                var campuslist = (from li in datalist
                                  group li by ((string)li.Campus)
                                  into campus
                                  where campus.Key.HasValue()
                                  select campus.Key).ToList();
                var dbc = from c in campuslist
                          join cp in db.Campus on c equals cp.Description into j
                          from cp in j.DefaultIfEmpty()
                          select new { cp, c };
                var clist = dbc.ToList();
                if (clist.Count > 0)
                {
                    var maxcampusid = 0;
                    if (db.Campus.Any())
                        maxcampusid = db.Campus.Max(c => c.Id);
                    foreach (var i in clist)
                        if (i.cp == null)
                        {
                            var cp = new Campu { Description = i.c, Id = ++maxcampusid };
                            if (!testing)
                                db.Campus.InsertOnSubmit(cp);
                        }
                }
            }
            var now = DateTime.Now;
            if (!testing)
                db.SubmitChanges();
            Campuses = db.Campus.ToDictionary(cp => cp.Description, cp => cp.Id);

            var q = (from li in datalist
                     group li by li.FamilyId
                     into fam
                     select fam).ToList();
            rt.Count = q.Sum(ff => ff.Count());
            Db2.SubmitChanges();

            foreach (var fam in q)
            {
                Family f = null;
                var prevpid = 0;

                FindPerson3 pid;
                Person p = null;
                foreach (var a in fam)
                {
                    if (!testing)
                    {
                        db.SubmitChanges();
                        db.Dispose();
                        db = DbUtil.Create(host);
                    }

                    var potentialdup = false;
                    var first = a.First as string;
                    var last = a.Last as string;
                    DateTime dt;
                    DateTime? dob = null;
                    if (DateTime.TryParse((string)a.Birthday, out dt))
                    {
                        dob = dt;
                        if (dob.Value < SqlDateTime.MinValue)
                            dob = null;
                    }
                    var email = ((string)a.Email).trim();
                    var cell = ((string)a.CellPhone).GetDigits();
                    var homephone = ((string)a.HomePhone).GetDigits();
                    pid = db.FindPerson3(first, last, dob, email, cell, homephone, null).FirstOrDefault();

                    if (noupdate && pid != null)
                    {
                        if (!testing)
                        {
                            var pd = db.LoadPersonById(pid.PeopleId.Value);
                            pd.AddEditExtraBool("FoundDup", true);
                        }
                        potentialdup = true;
                        pid = null;
                    }
                    if (pid != null) // found
                    {
                        p = db.LoadPersonById(pid.PeopleId.Value);
                        prevpid = p.PeopleId;
                        psb = new List<ChangeDetail>();
                        fsb = new List<ChangeDetail>();

                        UpdateField(p, "TitleCode", a.Title);
                        UpdateField(p, "FirstName", a.First);
                        UpdateField(p, "NickName", a.Goesby);
                        UpdateField(p, "LastName", a.Last);
                        UpdateField(p, "EmailAddress", a.Email);
                        UpdateField(p, "EmailAddress2", a.Email2);
                        UpdateField(p, "DOB", a.Birthday);
                        UpdateField(p, "AltName", a.AltName);
                        UpdateField(p, "SuffixCode", a.Suffix);
                        UpdateField(p, "MiddleName", a.Middle);

                        UpdateField(p, "CellPhone", GetDigits(a.CellPhone));
                        UpdateField(p, "WorkPhone", GetDigits(a.WorkPhone));
                        UpdateField(p, "GenderId", Gender(a.Gender));
                        UpdateField(p, "MaritalStatusId", Marital(a.Marital));
                        UpdateField(p, "PositionInFamilyId", Position(a.Position));
                        if (!testing)
                            UpdateField(p, "CampusId", Campus(a.Campus));

                        UpdateField(p.Family, "AddressLineOne", a.Address);
                        UpdateField(p.Family, "AddressLineTwo", a.Address2);
                        UpdateField(p.Family, "CityName", a.City);
                        UpdateField(p.Family, "StateCode", a.State);
                        UpdateField(p.Family, "ZipCode", GetString(a.Zip));

                        if (names.ContainsKey("memberstatus"))
                            UpdateMemberStatus(db, p, a);

                        if (!testing)
                        {
                            p.LogChanges(db, psb, PeopleId);
                            p.Family.LogChanges(db, fsb, p.PeopleId, PeopleId);
                            db.SubmitChanges();
                            p.AddEditExtraBool("InsertPeopleUpdated", true);
                        }
                    }
                    else // new person
                    {
                        if (!testing)
                            if (prevpid > 0)
                                f = db.LoadFamilyByPersonId(prevpid);

                        if (f == null)
                        {
                            f = new Family();
                            f.AddressLineOne = a.Address;
                            f.AddressLineTwo = a.Address2;
                            f.CityName = a.City;
                            f.StateCode = a.State;
                            f.ZipCode = GetString(a.Zip);
                            f.HomePhone = GetDigits(a.HomePhone);
                            db.Families.InsertOnSubmit(f);
                            if (!testing)
                                db.SubmitChanges();
                        }

                        p = Person.Add(db, false, f, 10, null,
                            a.First,
                            a.GoesBy,
                            a.Last,
                            dob.FormatDate(),
                            0, 0, 0, null, testing);
                        prevpid = p.PeopleId;
                        p.FixTitle();

                        p.AltName = a.AltName;
                        p.SuffixCode = a.Suffix;
                        p.MiddleName = a.Middle;
                        p.MaidenName = a.MaidenName;
                        p.EmployerOther = a.Employer;
                        p.OccupationOther = a.Occupation;
                        p.CellPhone = cell;
                        p.WorkPhone = ((string)a.WorkPhone).GetDigits();
                        p.EmailAddress = email;
                        p.EmailAddress2 = a.Email2;
                        p.GenderId = Gender(a.Gender);
                        p.MaritalStatusId = Marital(a.Marital);
                        p.WeddingDate = ((string)a.WeddingDate).ToDate();
                        p.JoinDate = ((string)a.JoinDate).ToDate();
                        p.DropDate = ((string)a.DropDate).ToDate();
                        p.BaptismDate = ((string)a.BaptismDate).ToDate();
                        p.PositionInFamilyId = Position(a.Position);
                        p.TitleCode = Title(a.Title);
                        p.AddEditExtraInt("HouseholdId", (int)a.FamilyId);
                        p.AddEditExtraInt("IndividualId", (int) a.IndividualId);

                        SetMemberStatus(db, p, a.MemberStatus);
                        if (!testing)
                        {
                            p.CampusId = Campus(a.Campus);
                            p.AddEditExtraBool("InsertPeopleAdded", true);
                            if (potentialdup)
                                p.AddEditExtraBool("FoundDup", true);
                            db.SubmitChanges();
                        }
                    }

                    if (recregnames.Any())
                        SetRecRegs(p, a);

                    if (extravaluenames.Any())
                        ProcessExtraValues(db, p, a);

                    rt.Processed++;
                    Db2.SubmitChanges();
                }
                if (!testing)
                    db.SubmitChanges();
            }
            rt.Completed = DateTime.Now;
            Db2.SubmitChanges();
            return true;
        }

        private void ProcessExtraValues(CMSDataContext db, Person p, dynamic a)
        {
            if (!extravaluenames.Any())
                return;

            foreach (var name in extravaluenames)
            {
                var v = a.GetValue(name);
                var vs = v as string;
                string type = null;
                if (!evtypes.TryGetValue(name, out type))
                {
                    p.AddEditExtraCode(name, Util.trim(a[name]));
                    return;
                }
                switch (type)
                {
                    case "txt":
                        p.AddEditExtraText(name, a[name]);
                        break;
                    case "org":

                        if (testing)
                            continue;
                        var d = a[name].Trim().Trim();
                        if (!d.HasValue())
                            continue;
                        if (d == "TRUE")
                            d = "Member";
                        var oid = 0;
                        if (orgs.ContainsKey(name))
                            oid = orgs[name];
                        else
                        {
                            var prog = Organization.FetchOrCreateProgram(db, "InsertPeople");
                            var div = Organization.FetchOrCreateDivision(db, prog, "InsertPeople");
                            var o = Organization.FetchOrCreateOrganization(db, div, name.SplitUpperCaseToString());
                            oid = o.OrganizationId;
                            orgs.Add(name, oid);
                        }
                        var mtid = 0;
                        if (membertypes.ContainsKey(d))
                            mtid = membertypes[d];
                        else
                        {
                            var mt = Organization.FetchOrCreateMemberType(db, d);
                            mtid = mt.Id;
                            membertypes.Add(d, mtid);
                        }
                        OrganizationMember.InsertOrgMembers(db, oid, p.PeopleId, mtid, DateTime.Today, null, false);
                        break;
                    case "dt":
                        if (vs != null)
                        {
                            DateTime dt;
                            if (Util.DateValid(vs, out dt))
                                p.AddEditExtraDate(name, dt);
                        }
                        else if (v is DateTime)
                            p.AddEditExtraDate(name, v);
                        break;
                    case "int":
                        if (v != null && v is int)
                            p.AddEditExtraInt(name, v);
                        break;
                    case "bit":
                        if (v != null && v is int)
                            p.AddEditExtraBool(name, v == 1);
                        break;
                }
            }
        }

        private void SetRecRegs(Person p, string[] a)
        {
            var nq = (from name in names.Keys
                      where standardrecregnames.Contains(name, StringComparer.OrdinalIgnoreCase)
                      select name).ToList();
            foreach (var name in nq)
            {
                var rr = p.SetRecReg();
                var value = a[names[name]].Trim();
                switch (name)
                {
                    case "Mother":
                        rr.Mname = value;
                        break;
                    case "Father":
                        rr.Fname = value;
                        break;
                    case "EmContact":
                        rr.Emcontact = value;
                        break;
                    case "EmPhone":
                        rr.Emphone = value;
                        break;
                    case "Grade":
                        p.Grade = value.ToInt2();
                        break;
                    case "School":
                        p.SchoolOther = value;
                        break;
                    case "Doctor":
                        rr.Doctor = value;
                        break;
                    case "DocPhone":
                        rr.Docphone = value;
                        break;
                    case "Insurance":
                        rr.Insurance = value;
                        break;
                    case "Policy":
                        rr.Policy = value;
                        break;
                    case "Allergies":
                        rr.MedicalDescription = value;
                        break;
                }
            }
        }

        private void SetMemberStatus(CMSDataContext db, Person p, dynamic v)
        {
            var s = v as string;
            if (!s.HasValue())
                return;
            var qms = from mm in db.MemberStatuses
                      where mm.Description == s
                      select mm;
            var m = qms.SingleOrDefault();
            if (m == null)
            {
                var nx = db.MemberStatuses.Max(mm => mm.Id) + 1;
                m = new MemberStatus { Id = nx, Description = s, Code = nx.ToString() };
                db.MemberStatuses.InsertOnSubmit(m);
            }
            p.MemberStatusId = m.Id;
        }

        private void UpdateMemberStatus(CMSDataContext db, Person p, dynamic a)
        {
            var ms = a as string;
            if (ms == null)
                return;
            var qms = from mm in db.MemberStatuses
                      where mm.Description == ms
                      select mm;
            var m = qms.SingleOrDefault();
            if (m == null)
            {
                var nx = db.MemberStatuses.Max(mm => mm.Id) + 1;
                m = new MemberStatus { Id = nx, Description = ms, Code = nx.ToString() };
                db.MemberStatuses.InsertOnSubmit(m);
            }
            p.UpdateValue("MemberStatusId", m.Id);
        }

        private List<dynamic> datalist;
        private Dictionary<string, string> evtypes;
        public void FetchData(ExcelWorksheet ws)
        {
            FetchHeaderColumns(ws);
            var r = 2;
            datalist = new List<dynamic>();
            while (r <= ws.Dimension.End.Row)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in names)
                    dict[kv.Key] = ws.Cells[r, kv.Value].Value;
                datalist.Add(new DynamicData(dict));
                r++;
            }
        }
        public void FetchHeaderColumns(ExcelWorksheet sheet)
        {
            names = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            evtypes = new Dictionary<string, string>();
            var n = 0;
            foreach (var c in sheet.Cells[1, 1, 1, sheet.Dimension.End.Column])
            {
                n++;
                if (c.Text.HasValue())
                {
                    var b = c.Text.Split('.');
                    if (b.Length > 1)
                        evtypes[b[0]] = b[1]; 
                    names.Add(c.Text, n);
                }
            }
        }
    }
}