using CmsData;
using CmsData.API;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class UploadPeopleModel
    {
        internal readonly string Host;
        internal readonly CMSDataContext JobDbContext;
        internal readonly Dictionary<string, int> Membertypes = new Dictionary<string, int>();
        internal readonly bool Noupdate;
        internal readonly Dictionary<string, int> Orgs = new Dictionary<string, int>();
        internal readonly int PeopleId;
        internal readonly CMSDataContext ProgressDbContext;

        internal readonly List<string> Standardnames = new List<string>
        {
            "familyid", "title", "first", "last", "goesby", "altname", "gender", "marital", "maidenName", "address",
            "address2", "city", "state", "zip", "position", "birthday", "deceased", "cellphone", "homephone", "workphone", "email",
            "email2", "suffix", "middle", "joindate", "dropdate", "baptismdate", "weddingdate", "memberstatus", "employer",
            "occupation", "CreatedDate", "BackgroundCheck", "individualid", "campus"
        };

        internal readonly List<string> Standardrecregnames = new List<string>
        {
            "Mother", "Father", "EmContact", "EmPhone", "Allergies", "Grade", "School", "Doctor", "DocPhone",
            "Insurance", "Policy"
        };

        internal Dictionary<string, int> Campuses;
        internal List<dynamic> Datalist;
        internal Dictionary<string, string> Evtypes;
        internal List<string> Extravaluenames;
        internal List<ChangeDetail> Fsb;
        internal Dictionary<string, int> Names;
        internal List<ChangeDetail> Psb;
        internal List<string> Recregnames;
        public bool Testing;

        public UploadPeopleModel()
        {
        }

        public UploadPeopleModel(string host, int peopleId, bool noupdate, bool testing = false)
        {
            JobDbContext = DbUtil.Create(host);
            ProgressDbContext = DbUtil.Create(host);
            PeopleId = peopleId;
            Noupdate = noupdate;
            Testing = testing;
            Host = host;
            PeopleSheetName = "People";
        }

        public UploadPeopleModel(CMSDataContext existingProgressDbContext, string host, int peopleId, bool noupdate, bool testing = false)
        {
            JobDbContext = DbUtil.Create(host);
            ProgressDbContext = existingProgressDbContext;
            PeopleId = peopleId;
            Noupdate = noupdate;
            Testing = testing;
            Host = host;
            PeopleSheetName = "People";
        }

        public bool InsertPeopleSpreadsheet
        {
            get { return true; }
        }

        internal string PeopleSheetName { get; set; }

        public virtual bool DoUpload(ExcelPackage pkg)
        {
            var rt = ProgressDbContext.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();
            var ws = pkg.Workbook.Worksheets[PeopleSheetName];
            FetchData(pkg.Workbook.Worksheets[PeopleSheetName]);

            UploadPeople(rt, ws);

            rt.Completed = DateTime.Now;
            ProgressDbContext.SubmitChanges();

            return true;
        }

        internal void UploadPeople(UploadPeopleRun rt, ExcelWorksheet ws)
        {
            Extravaluenames = (from name in Names
                               where !Standardnames.Contains(name.Key, StringComparer.OrdinalIgnoreCase)
                               where !Standardrecregnames.Contains(name.Key)
                               select name.Key).ToList();

            Recregnames = (from name in Names
                           where Standardrecregnames.Contains(name.Key)
                           select name.Key).ToList();

            if (Names.ContainsKey("Campus"))
            {
                var campuslist = (from li in Datalist
                                  group li by (string)li.Campus
                    into campus
                                  where campus.Key.HasValue()
                                  select campus.Key).ToList();
                var dbc = from c in campuslist
                          join cp in JobDbContext.Campus on c equals cp.Description into j
                          from cp in j.DefaultIfEmpty()
                          select new { cp, c };
                var clist = dbc.ToList();
                if (clist.Count > 0)
                {
                    var maxcampusid = 0;
                    if (JobDbContext.Campus.Any())
                    {
                        maxcampusid = JobDbContext.Campus.Max(c => c.Id);
                    }

                    foreach (var i in clist)
                    {
                        if (i.cp != null)
                        {
                            continue;
                        }

                        var cp = new Campu { Description = i.c, Id = ++maxcampusid };
                        if (!Testing)
                        {
                            JobDbContext.Campus.InsertOnSubmit(cp);
                        }
                    }
                }
            }

            if (!Testing)
            {
                JobDbContext.SubmitChanges();
            }

            Campuses = JobDbContext.Campus.ToDictionary(cp => cp.Description, cp => cp.Id);

            var q = (from li in Datalist
                     group li by li.FamilyId
                into fam
                     select fam).ToList();
            rt.Count = q.Sum(ff => ff.Count());
            rt.Description = $"Uploading People {(Testing ? "in testing mode" : "for real")}";
            ProgressDbContext.SubmitChanges();

            foreach (var fam in q)
            {
                var prevpid = 0;

                foreach (var a in fam)
                {
                    if (!Testing)
                    {
                        JobDbContext.SubmitChanges();
                    }

                    Family f = null;
                    var potentialdup = false;
                    int? pid = FindRecord(JobDbContext, a, ref potentialdup);
                    if (pid == -1) // no data: no first or last name
                    {
                        continue;
                    }

                    var p = pid.HasValue
                        ? UpdateRecord(JobDbContext, pid.Value, a)
                        : NewRecord(JobDbContext, ref f, a, prevpid, potentialdup);
                    prevpid = p.PeopleId;

                    if (Recregnames.Any())
                    {
                        SetRecRegs(p, a);
                    }

                    if (Extravaluenames.Any())
                    {
                        ProcessExtraValues(JobDbContext, p, a);
                    }

                    rt.Processed++;
                    ProgressDbContext.SubmitChanges();
                }

                if (!Testing)
                {
                    JobDbContext.SubmitChanges();
                }
            }
        }

        internal Person UpdateRecord(CMSDataContext db, int pid, dynamic a)
        {
            var p = db.LoadPersonById(pid);
            Psb = new List<ChangeDetail>();
            Fsb = new List<ChangeDetail>();

            UpdateField(p, "TitleCode", a.Title);
            UpdateField(p, "FirstName", a.First);
            UpdateField(p, "NickName", a.Goesby);
            UpdateField(p, "LastName", a.Last);
            UpdateField(p, "EmailAddress", a.Email);
            UpdateField(p, "EmailAddress2", a.Email2);

            DateTime? dob = GetDate(a.Birthday);
            var dobstr = dob.FormatDate();
            UpdateField(p, "DOB", dobstr);
            UpdateField(p, "AltName", GetString(a.AltName));
            UpdateField(p, "SuffixCode", GetString(a.Suffix));
            UpdateField(p, "MiddleName", GetString(a.Middle));

            UpdateField(p, "CellPhone", GetDigits(a.CellPhone));
            UpdateField(p, "WorkPhone", GetDigits(a.WorkPhone));
            UpdateField(p, "GenderId", Gender(a.Gender));
            UpdateField(p, "MaritalStatusId", Marital(a.Marital));
            UpdateField(p, "PositionInFamilyId", Position(a.Position));
            if (!Testing)
            {
                UpdateField(p, "CampusId", Campus(a.Campus));
            }

            UpdateField(p.Family, "AddressLineOne", GetString(a.Address));
            UpdateField(p.Family, "AddressLineTwo", GetString(a.Address2));
            UpdateField(p.Family, "CityName", GetString(a.City));
            UpdateField(p.Family, "StateCode", GetString(a.State));
            UpdateField(p.Family, "ZipCode", GetString(a.Zip));

            UpdateMemberStatus(db, p, a.MemberStatus);

            if (Testing)
            {
                return p;
            }

            p.LogChanges(db, Psb, PeopleId);
            p.Family.LogChanges(db, Fsb, p.PeopleId, PeopleId);
            db.SubmitChanges();
            p.AddEditExtraBool("InsertPeopleUpdated", true);

            return p;
        }

        internal Person NewRecord(CMSDataContext db, ref Family f, dynamic a, int prevpid, bool potentialdup)
        {
            if (!Testing)
            {
                if (prevpid > 0)
                {
                    f = db.LoadFamilyByPersonId(prevpid);
                }
            }

            if (f == null)
            {
                f = new Family
                {
                    AddressLineOne = GetString(a.Address),
                    AddressLineTwo = GetString(a.Address2),
                    CityName = GetString(a.City),
                    StateCode = GetString(a.State),
                    ZipCode = GetString(a.Zip),
                    HomePhone = GetDigits(a.HomePhone)
                };
                db.Families.InsertOnSubmit(f);

                if (!Testing)
                {
                    db.SubmitChanges();
                }
            }

            DateTime? dob = GetDate(a.Birthday);
            var dobstr = dob.FormatDate();

            var p = Person.Add(db, false, f, 10, null,
                (string)a.First,
                (string)a.GoesBy,
                (string)a.Last,
                dobstr,
                0, 0, 0, null, Testing);
            p.FixTitle();

            p.AltName = GetString(a.AltName);
            p.SuffixCode = GetString(a.Suffix);
            p.MiddleName = GetString(a.Middle);
            p.MaidenName = GetString(a.MaidenName);
            p.EmployerOther = GetString(a.Employer);
            p.OccupationOther = GetString(a.Occupation);

            p.EmailAddress = GetStringTrimmed(a.Email);
            p.EmailAddress2 = GetStringTrimmed(a.Email2);

            p.CellPhone = GetDigits(a.CellPhone);
            p.WorkPhone = GetDigits(a.WorkPhone);

            p.TitleCode = Title(a.Title);
            p.GenderId = Gender(a.Gender);
            p.MaritalStatusId = Marital(a.Marital);
            p.PositionInFamilyId = Position(a.Position);
            SetMemberStatus(db, p, a.MemberStatus);

            p.WeddingDate = GetDate(a.WeddingDate);
            p.JoinDate = GetDate(a.JoinDate);
            p.DropDate = GetDate(a.DropDate);
            p.BaptismDate = GetDate(a.BaptismDate);

            StoreIds(p, a);

            if (Testing)
            {
                return p;
            }

            p.CampusId = Campus(a.Campus);
            p.AddEditExtraBool("InsertPeopleAdded", true);
            if (potentialdup)
            {
                p.AddEditExtraBool("FoundDup", true);
            }

            db.SubmitChanges();

            return p;
        }

        internal virtual void StoreIds(Person p, dynamic a)
        {
        }

        internal void ProcessExtraValues(CMSDataContext db, Person p, dynamic a)
        {
            if (!Extravaluenames.Any())
            {
                return;
            }

            foreach (var name in Extravaluenames)
            {
                object o = a.GetValue(name);
                var vs = o as string;
                string type = null;
                if (!Evtypes.TryGetValue(name, out type))
                {
                    p.AddEditExtraCode(name, ((string)a[name])?.Trim());
                    continue;
                }

                switch (type)
                {
                    case "fam":
                        p.Family.AddEditExtraCode(name, ((string)a[name])?.Trim());
                        break;
                    case "txt":
                        p.AddEditExtraText(name, $"{a[name]}".Trim());
                        break;
                    case "org":

                        if (Testing)
                        {
                            continue;
                        }

                        var d = ((string)a[name])?.Trim();
                        if (!d.HasValue())
                        {
                            continue;
                        }

                        if (d.Equal("true"))
                        {
                            d = "Member";
                        }

                        var oid = 0;
                        if (Orgs.ContainsKey(name))
                        {
                            oid = Orgs[name];
                        }
                        else
                        {
                            var prog = Organization.FetchOrCreateProgram(db, "InsertPeople");
                            var div = Organization.FetchOrCreateDivision(db, prog, "InsertPeople");
                            var org = Organization.FetchOrCreateOrganization(db, div, name.SplitUpperCaseToString());
                            oid = org.OrganizationId;
                            Orgs.Add(name, oid);
                        }

                        var mtid = 0;
                        if (Membertypes.ContainsKey(d))
                        {
                            mtid = Membertypes[d];
                        }
                        else
                        {
                            var mt = Organization.FetchOrCreateMemberType(db, d);
                            mtid = mt.Id;
                            Membertypes.Add(d, mtid);
                        }

                        OrganizationMember.InsertOrgMembers(db, oid, p.PeopleId, mtid, DateTime.Today, null, false);
                        break;
                    case "dt":
                        if (o is DateTime)
                        {
                            p.AddEditExtraDate(name, (DateTime)o);
                        }

                        var dt = o.ToDate();
                        if (dt == null)
                        {
                            continue;
                        }

                        p.AddEditExtraDate(name, dt);
                        break;
                    case "int":
                        var i = GetInt(o);
                        if (i.HasValue)
                        {
                            p.AddEditExtraInt(name, i.Value);
                        }

                        break;
                    case "bit":
                        p.AddEditExtraBool(name, IsTrue(o));
                        break;
                }
            }
        }

        private static bool IsTrue(object arg)
        {
            switch (arg)
            {
                case null:
                    return false;
                case int _ when (int)arg != 0:
                case double _ when arg.ToInt() != 0:
                    return true;
            }

            return arg.ToString().Equal("true") || arg.ToString().Equal("1");
        }

        internal void SetRecRegs(Person p, dynamic a)
        {
            var nq = (from name in Names.Keys
                      where Standardrecregnames.Contains(name, StringComparer.OrdinalIgnoreCase)
                      select name).ToList();
            foreach (var name in nq)
            {
                var rr = p.SetRecReg();
                string value = GetStringTrimmed(a.GetValue(name));
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

        internal void SetMemberStatus(CMSDataContext db, Person p, object o)
        {
            var s = o as string;
            if (!s.HasValue())
            {
                return;
            }

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

        internal void UpdateMemberStatus(CMSDataContext db, Person p, object o)
        {
            var ms = o as string;
            if (ms == null)
            {
                return;
            }

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

        public void FetchData(ExcelWorksheet ws)
        {
            FetchHeaderColumns(ws);

            var r = 2;
            Datalist = new List<dynamic>();
            while (r <= ws.Dimension.End.Row)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in Names)
                {
                    dict[kv.Key] = ws.Cells[r, kv.Value].Value;
                }

                Datalist.Add(new DynamicData(dict));
                r++;
            }
        }

        public void FetchHeaderColumns(ExcelWorksheet sheet)
        {
            Names = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            Evtypes = new Dictionary<string, string>();
            var n = 0;
            foreach (var c in sheet.Cells[1, 1, 1, sheet.Dimension.End.Column])
            {
                n++;
                if (!c.Text.HasValue())
                {
                    continue;
                }

                var colname = c.Text;
                if (colname.Equal("IndividualId") && colname != "IndividualId")
                {
                    throw new Exception($"Must Use IndividualId, not {colname}, case sensitive");
                }

                if (colname.Equal("FamilyId") && colname != "FamilyId")
                {
                    throw new Exception($"Must Use FamilyId, not {colname}, case sensitive");
                }

                var b = colname.SplitStr(".", 2);
                if (b.Length > 1)
                {
                    Evtypes[b[0]] = b[1];
                    colname = b[0];
                }

                Names.Add(colname, n);
            }
        }

        internal void CheckColumn(string name, string sheet)
        {
            if (!Names.ContainsKey(name))
            {
                throw new Exception($"Missing {name} column on {sheet} sheet");
            }
        }

        internal virtual int? GetPeopleId(dynamic a)
        {
            return null;
        }

        internal int? FindRecord(CMSDataContext db, dynamic a, ref bool potentialdup)
        {
            var id = (int?)GetPeopleId(a);
            if (id > 0)
            {
                return id; // existing person's PeopleId
            }

            var first = a.First as string;
            var last = a.Last as string;
            if (!first.HasValue() && !last.HasValue())
            {
                return -1;
            }

            DateTime? dt = GetDate(a.Birthday);
            string email = GetStringTrimmed(a.Email);
            string cell = GetDigits(a.CellPhone);
            string home = GetDigits(a.HomePhone);

            var pid = db.FindPerson3(first, last, dt, email, cell, home, null).FirstOrDefault();

            if (!Noupdate || pid?.PeopleId == null)
            {
                return pid?.PeopleId;
            }

            if (!Testing)
            {
                var pd = db.LoadPersonById(pid.PeopleId.Value);
                pd.AddEditExtraBool("FoundDup", true);
            }

            potentialdup = true;
            pid = null;

            return pid?.PeopleId;
        }

        internal void UpdateField(Family f, string prop, object o)
        {
            if (o != null)
            {
                f.UpdateValue(Fsb, prop, o);
            }
        }

        internal void UpdateField(Person p, string prop, object o)
        {
            if (o != null)
            {
                p.UpdateValue(Psb, prop, o);
            }
        }

        internal string GetDigits(object o)
        {
            var s = o as string;
            return s.HasValue() ? s.GetDigits() : null;
        }

        internal string GetString(object o)
        {
            return o?.ToString();
        }

        internal string GetStringTrimmed(object o)
        {
            return GetString(o)?.Trim();
        }

        internal int? GetInt(object o)
        {
            return o.ToInt2();
        }

        internal decimal GetDecimal(object o)
        {
            return o.ToNullableDecimal() ?? 0m;
        }

        internal DateTime? GetDate(object o)
        {
            //DateTime dt = DateTime.FromOADate(39938);
            var dt = o.ToDate();
            if (!dt.HasValue)
            {
                return dt;
            }

            if (dt.Value < SqlDateTime.MinValue)
            {
                dt = null;
            }

            return dt;
        }

        internal int Gender(object o)
        {
            var s = o as string;
            s = s?.Trim()?.ToLower();
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

        internal int Marital(object o)
        {
            return getMaritalId(o);
        }

        internal int getMaritalId(object o)
        {
            var s = o as string;
            s = s?.Trim()?.ToLower();
            int? a = (from m in JobDbContext.MaritalStatuses
                      where m.Description == s
                      select m.Id).SingleOrDefault();
            return a.Value;
        }

        internal string Title(object o)
        {
            var s = o as string;
            s = s?.Trim()?.ToLower();
            return !s.HasValue() ? s : s.Truncate(10).TrimEnd();
        }

        internal int Position(object o)
        {
            var s = o as string;
            s = s?.Trim()?.ToLower();
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

        internal int? Campus(object o)
        {
            var s = o as string;
            if (!s.HasValue())
            {
                return null;
            }

            s = s.Trim().ToLower();
            int i;
            if (!Campuses.TryGetValue(s, out i))
            {
                return null;
            }

            return i;
        }
    }
}
