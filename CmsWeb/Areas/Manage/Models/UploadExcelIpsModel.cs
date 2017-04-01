using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsWeb.Areas.Finance.Models.BatchImport;
using OfficeOpenXml;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class UploadExcelIpsModel
    {
        public bool Testing;
        public bool InsertPeopleSpreadsheet => true;
        private readonly CMSDataContext db2;
        private readonly string host;
        private readonly bool noupdate;
        private readonly int peopleId;
        private Dictionary<string, int> campuses;
        private List<ChangeDetail> fsb;
        private Dictionary<string, int> names;
        private List<ChangeDetail> psb;
        private readonly Dictionary<string, int> orgs = new Dictionary<string, int>();
        private readonly Dictionary<string, int> membertypes = new Dictionary<string, int>();
        private List<string> extravaluenames;
        private List<string> recregnames;
        private List<dynamic> datalist;
        private Dictionary<string, string> evtypes;
        private Dictionary<int, int> peopleids;
        public UploadExcelIpsModel(string host, int peopleId, bool noupdate, bool testing = false)
        {
            db2 = DbUtil.Create(host);
            this.peopleId = peopleId;
            this.noupdate = noupdate;
            Testing = testing;
            this.host = host;
        }
        public bool DoUpload(ExcelPackage pkg)
        {
            var rt = db2.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();

            UploadPeople(rt, pkg);

            TryDeleteAllContributions();
            UploadPledges(rt, pkg);
            UploadGifts(rt, pkg);

            rt.Completed = DateTime.Now;
            db2.SubmitChanges();
            return true;
        }
        private void UploadPeople(UploadPeopleRun rt, ExcelPackage pkg)
        {
            var db = DbUtil.Create(host);
            peopleids = db.PeopleExtras.Where(vv => vv.Field == "IndividualId" && vv.IntValue != null)
                .ToDictionary(vv => vv.IntValue ?? 0, vv => vv.PeopleId);
            FetchData(pkg.Workbook.Worksheets["Personal Data"]);

            extravaluenames = (from name in names
                               where !standardnames.Contains(name.Key, StringComparer.OrdinalIgnoreCase)
                               where !standardrecregnames.Contains(name.Key)
                               select name.Key).ToList();
            recregnames = (from name in names
                           where standardrecregnames.Contains(name.Key)
                           select name.Key).ToList();

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
                            if (!Testing)
                                db.Campus.InsertOnSubmit(cp);
                        }
                }
            }
            if (!Testing)
                db.SubmitChanges();
            campuses = db.Campus.ToDictionary(cp => cp.Description, cp => cp.Id);

            var q = (from li in datalist
                     group li by li.FamilyId
                into fam
                     select fam).ToList();
            rt.Count = q.Sum(ff => ff.Count());
            rt.Description = $"Uploading People {(Testing ? "in testing mode" : "for real")}";
            db2.SubmitChanges();

            foreach (var fam in q)
            {
                var prevpid = 0;

                foreach (var a in fam)
                {
                    if (!Testing)
                    {
                        db.SubmitChanges();
                        db.Dispose();
                        db = DbUtil.Create(host);
                    }

                    Family f = null;
                    var potentialdup = false;
                    int? pid = FindRecord(db, a, ref potentialdup);
                    var p = pid.HasValue
                        ? UpdateRecord(db, pid.Value, a)
                        : NewRecord(db, ref f, a, prevpid, potentialdup);
                    prevpid = p.PeopleId;

                    if (recregnames.Any())
                        SetRecRegs(p, a);

                    if (extravaluenames.Any())
                        ProcessExtraValues(db, p, a);

                    rt.Processed++;
                    db2.SubmitChanges();
                }
                if (!Testing)
                    db.SubmitChanges();
            }
        }
        private Person UpdateRecord(CMSDataContext db, int pid, dynamic a)
        {
            var p = db.LoadPersonById(pid);
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
            if (!Testing)
                UpdateField(p, "CampusId", Campus(a.Campus));

            UpdateField(p.Family, "AddressLineOne", a.Address);
            UpdateField(p.Family, "AddressLineTwo", a.Address2);
            UpdateField(p.Family, "CityName", a.City);
            UpdateField(p.Family, "StateCode", a.State);
            UpdateField(p.Family, "ZipCode", GetString(a.Zip));

            UpdateMemberStatus(db, p, a.MemberStatus);

            if (!Testing)
            {
                p.LogChanges(db, psb, peopleId);
                p.Family.LogChanges(db, fsb, p.PeopleId, peopleId);
                db.SubmitChanges();
                p.AddEditExtraBool("InsertPeopleUpdated", true);
            }
            return p;
        }
        private Person NewRecord(CMSDataContext db, ref Family f, dynamic a, int prevpid, bool potentialdup)
        {
            if (!Testing)
                if (prevpid > 0)
                    f = db.LoadFamilyByPersonId(prevpid);

            if (f == null)
            {
                f = new Family
                {
                    AddressLineOne = a.Address,
                    AddressLineTwo = a.Address2,
                    CityName = a.City,
                    StateCode = a.State,
                    ZipCode = GetString(a.Zip),
                    HomePhone = GetDigits(a.HomePhone)
                };
                db.Families.InsertOnSubmit(f);
                if (!Testing)
                    db.SubmitChanges();
            }

            string dob = GetDate(a.Birthday)?.FormatDate();
            var p = Person.Add(db, false, f, 10, null,
                (string)a.First,
                (string)a.GoesBy,
                (string)a.Last,
                dob,
                0, 0, 0, null, Testing);
            p.FixTitle();

            p.AltName = a.AltName;
            p.SuffixCode = a.Suffix;
            p.MiddleName = a.Middle;
            p.MaidenName = a.MaidenName;
            p.EmployerOther = a.Employer;
            p.OccupationOther = a.Occupation;
            p.CellPhone = GetDigits(a.CellPhone);
            p.WorkPhone = GetDigits(a.WorkPhone);
            p.EmailAddress = GetStringTrimmed(a.Email);
            p.EmailAddress2 = GetStringTrimmed(a.Email2);
            p.GenderId = Gender(a.Gender);
            p.MaritalStatusId = Marital(a.Marital);
            p.WeddingDate = GetDate(a.WeddingDate);
            p.JoinDate = GetDate(a.JoinDate);
            p.DropDate = GetDate(a.DropDate);
            p.BaptismDate = GetDate(a.BaptismDate);
            p.PositionInFamilyId = Position(a.Position);
            p.TitleCode = Title(a.Title);
            p.AddEditExtraInt("HouseholdId", (int)a.FamilyId);
            p.AddEditExtraInt("IndividualId", (int)a.IndividualId);

            SetMemberStatus(db, p, a.MemberStatus);
            if (!Testing)
            {
                p.CampusId = Campus(a.Campus);
                p.AddEditExtraBool("InsertPeopleAdded", true);
                if (potentialdup)
                    p.AddEditExtraBool("FoundDup", true);
                db.SubmitChanges();
            }
            peopleids.Add((int)a.IndividualId, p.PeopleId);
            return p;
        }
        private void ProcessExtraValues(CMSDataContext db, Person p, dynamic a)
        {
            if (!extravaluenames.Any())
                return;

            foreach (var name in extravaluenames)
            {
                object o = a.GetValue(name);
                var vs = o as string;
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

                        if (Testing)
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
                            var org = Organization.FetchOrCreateOrganization(db, div, name.SplitUpperCaseToString());
                            oid = org.OrganizationId;
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
                        else if (o is DateTime)
                            p.AddEditExtraDate(name, (DateTime)o);
                        break;
                    case "int":
                        if (o is int)
                            p.AddEditExtraInt(name, (int)o);
                        break;
                    case "bit":
                        if (o is int)
                            p.AddEditExtraBool(name, (int)o == 1);
                        break;
                }
            }
        }
        private void SetRecRegs(Person p, dynamic a)
        {
            var nq = (from name in names.Keys
                      where standardrecregnames.Contains(name, StringComparer.OrdinalIgnoreCase)
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
        private void SetMemberStatus(CMSDataContext db, Person p, object o)
        {
            var s = o as string;
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
        private void UpdateMemberStatus(CMSDataContext db, Person p, object o)
        {
            var ms = o as string;
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
        private void UploadPledges(UploadPeopleRun rt, ExcelPackage pkg)
        {
            var db = DbUtil.Create(host);
            var data = FetchPledgeData(pkg.Workbook.Worksheets["Pledges"]).ToList();
            rt.Count = data.Count;
            rt.Description = $"Uploading Pledges {(Testing ? "in testing mode" : "for real")}";
            rt.Processed = 0;
            db2.SubmitChanges();

            var weeks = (from g in data
                         group g by g.Date.Sunday() into weeklypledges
                         select weeklypledges).ToList();
            BundleHeader bh = null;
            foreach (var week in weeks)
            {
                FinishBundle(db, bh);
                if (!Testing)
                {
                    db.Dispose();
                    db = DbUtil.Create(host);
                }
                bh = new BundleHeader
                {
                    BundleHeaderTypeId = BundleTypeCode.Pledge,
                    BundleStatusId = BundleStatusCode.Closed,
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Today,
                    ContributionDate = week.Key,
                };
                foreach (var pledge in week)
                {
                    var pid = GetPeopleId(pledge.IndividualId);
                    if (!pid.HasValue)
                        throw new Exception($"peopleid not found from individualid {pledge.IndividualId}");
                    if (!Testing)
                    {
                        var f = db.FetchOrCreateFund(pledge.FundId, pledge.FundName ?? pledge.FundDescription);
                        f.FundPledgeFlag = true;
                    }
                    var bd = new BundleDetail();
                    bd.CreatedBy = Util.UserId;
                    bd.CreatedDate = DateTime.Now;
                    bd.Contribution = new Contribution
                    {
                        CreatedBy = Util.UserId,
                        CreatedDate = DateTime.Now,
                        ContributionDate = pledge.Date,
                        FundId = pledge.FundId,
                        ContributionStatusId = 0,
                        ContributionTypeId = ContributionTypeCode.Pledge,
                        ContributionAmount = pledge.Amount,
                        PeopleId = pid
                    };
                    bh.BundleDetails.Add(bd);
                    rt.Processed++;
                    db2.SubmitChanges();
                }
            }
            FinishBundle(db, bh);
            if (!Testing)
                db.Dispose();
        }
        private void UploadGifts(UploadPeopleRun rt, ExcelPackage pkg)
        {
            var db = DbUtil.Create(host);
            var data = FetchContributionData(pkg.Workbook.Worksheets["Gift Data"]).ToList();
            rt.Count = data.Count;
            rt.Description = $"Uploading Gifts {(Testing ? "in testing mode" : "for real")}";
            rt.Processed = 0;
            db2.SubmitChanges();

            var weeks = (from g in data
                         group g by g.Date.Sunday() into weeklygifts
                         select weeklygifts).ToList();
            BundleHeader bh = null;
            foreach (var week in weeks)
            {
                FinishBundle(db, bh);
                if (!Testing)
                {
                    db.Dispose();
                    db = DbUtil.Create(host);
                }
                bh = new BundleHeader
                {
                    BundleHeaderTypeId = BundleTypeCode.ChecksAndCash,
                    BundleStatusId = BundleStatusCode.Closed,
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Today,
                    ContributionDate = week.Key,
                };
                foreach (var gift in week)
                {
                    var pid = GetPeopleId(gift.IndividualId);
                    if (!Testing)
                        if (!pid.HasValue)
                            throw new Exception($"peopleid not found from individualid {gift.IndividualId}");
                    if (!Testing)
                        db.FetchOrCreateFund(gift.FundId, gift.FundName ?? gift.FundDescription);
                    var bd = new BundleDetail();
                    bd.CreatedBy = Util.UserId;
                    bd.CreatedDate = DateTime.Now;
                    bd.Contribution = new Contribution
                    {
                        CreatedBy = Util.UserId,
                        CreatedDate = DateTime.Now,
                        ContributionDate = gift.Date,
                        FundId = gift.FundId,
                        ContributionStatusId = 0,
                        ContributionTypeId = ContributionTypeCode.CheckCash,
                        ContributionAmount = gift.Amount,
                        CheckNo = gift.CheckNo,
                        PeopleId = pid
                    };
                    bh.BundleDetails.Add(bd);
                    rt.Processed++;
                    db2.SubmitChanges();
                }
            }
            FinishBundle(db, bh);
            if (!Testing)
                db.Dispose();
        }
        private void FinishBundle(CMSDataContext db, BundleHeader bh)
        {
            if (!Testing)
            {
                if (bh != null)
                {
                    bh.TotalChecks = bh.BundleDetails.Sum(d => d.Contribution.ContributionAmount);
                    bh.TotalCash = 0;
                    bh.TotalEnvelopes = 0;
                    db.BundleHeaders.InsertOnSubmit(bh);
                    db.SubmitChanges();
                }
            }
        }
        public IEnumerable<PledgeGift> FetchContributionData(ExcelWorksheet ws)
        {
            FetchHeaderColumns(ws);
            var r = 2;
            while (r <= ws.Dimension.End.Row)
            {
                var row = new PledgeGift()
                {
                    IndividualId = GetInt(ws.Cells[r, names["IndividualId"]].Value),
                    Amount = GetDecimal(ws.Cells[r, names["Amount"]].Value),
                    Date = GetDate(ws.Cells[r, names["Date"]].Value) ?? DateTime.MinValue,
                    FundId = GetInt(ws.Cells[r, names["FundId"]].Value),
                    FundDescription = GetString(ws.Cells[r, names["FundDescription"]].Value),
                    FundName = GetString(ws.Cells[r, names["FundName"]].Value),
                };
                if (names.ContainsKey("CheckNo"))
                    row.CheckNo = GetString(ws.Cells[r, names["CheckNo"]].Value);
                r++;
                yield return row;
            }
        }
        public IEnumerable<PledgeGift> FetchPledgeData(ExcelWorksheet ws)
        {
            FetchHeaderColumns(ws);
            var r = 2;
            while (r <= ws.Dimension.End.Row)
            {
                var row = new PledgeGift
                {
                    IndividualId = GetInt(ws.Cells[r, names["IndividualId"]].Value),
                    Amount = GetDecimal(ws.Cells[r, names["PledgeAmount"]].Value),
                    Date = GetDate(ws.Cells[r, names["PledgeDate"]].Value) ?? DateTime.MinValue,
                    FundId = GetInt(ws.Cells[r, names["FundId"]].Value),
                    FundName = GetString(ws.Cells[r, names["FundName"]].Value),
                    FundDescription = GetString(ws.Cells[r, names["FundDescription"]].Value),
                };
                r++;
                yield return row;
            }
        }
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
        private void TryDeleteAllContributions()
        {
            if (Testing)
                return;
            var db = DbUtil.Create("host");
            if (!db.Setting("UploadExcelIpsDeleteGifts"))
                return;

            var deletesql = @"
DELETE dbo.BundleDetail
FROM dbo.BundleDetail d
JOIN dbo.Contribution c ON d.ContributionId = c.ContributionId
DELETE dbo.Contribution
DELETE dbo.BundleHeader
DBCC CHECKIDENT ('[Contribution]', RESEED, 0)
DBCC CHECKIDENT ('[BundleHeader]', RESEED, 0)
DBCC CHECKIDENT ('[BundleDetail]', RESEED, 0)
";
            db.ExecuteCommand(deletesql);
        }
        private int? GetPeopleId(int individualid)
        {
            if (peopleids.ContainsKey(individualid))
                return peopleids[individualid];
            return null;
        }
        private int? FindRecord(CMSDataContext db, dynamic a, ref bool potentialdup)
        {
            if (a.IndividualId != null)
            {
                var peopleid = GetPeopleId((int)a.IndividualId);
                return peopleid;
            }
            // Only use search if there is no IndividualId

            string first = a.First as string;
            string last = a.Last as string;
            DateTime? dt = GetDate(a.Birthday);
            string email = GetStringTrimmed(a.Email);
            string cell = GetDigits(a.CellPhone);
            string home = GetDigits(a.HomePhone);

            var pid = db.FindPerson3(first, last, dt, email, cell, home, null).FirstOrDefault();

            if (noupdate && pid?.PeopleId != null)
            {
                if (!Testing)
                {
                    var pd = db.LoadPersonById(pid.PeopleId.Value);
                    pd.AddEditExtraBool("FoundDup", true);
                }
                potentialdup = true;
                pid = null;
            }
            return pid?.PeopleId;
        }
        private void UpdateField(Family f, string prop, object o)
        {
            if (o != null)
                f.UpdateValue(fsb, prop, o);
        }
        private void UpdateField(Person p, string prop, object o)
        {
            if (o != null)
                p.UpdateValue(psb, prop, o);
        }
        private string GetDigits(object o)
        {
            var s = o as string;
            return s.HasValue() ? s.GetDigits() : null;
        }
        private string GetString(object o)
        {
            return o?.ToString();
        }
        private string GetStringTrimmed(object o)
        {
            string s = o?.ToString();
            return s.trim();
        }
        private int GetInt(object o)
        {
            return o.ToInt();
        }
        private decimal GetDecimal(object o)
        {
            return o.ToNullableDecimal() ?? 0m;
        }
        private DateTime? GetDate(object o)
        {
            var dt = o.ToDate();
            if (dt.HasValue)
                if (dt.Value < SqlDateTime.MinValue)
                    dt = null;
            return dt;
        }
        private int Gender(object o)
        {
            var s = o as string;
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
        private int Marital(object o)
        {
            var s = o as string;
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
        private string Title(object o)
        {
            var s = o as string;
            s = s.trim()?.ToLower();
            return !s.HasValue() ? s : s.Truncate(10).TrimEnd();
        }
        private int Position(object o)
        {
            var s = o as string;
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
        private int? Campus(object o)
        {
            var s = o as string;
            if (s == null)
                return null;
            s = s.trim().ToLower();
            return campuses[s];
        }
        private readonly List<string> standardnames = new List<string>
        {
            "familyid", "title", "first", "last", "goesby", "altname", "gender", "marital", "maidenName", "address", "address2",
            "city", "state", "zip", "position", "birthday", "deceased", "cellphone", "homephone", "workphone", "email", "email2",
            "suffix", "middle", "joindate", "dropdate", "baptismdate", "weddingdate", "memberstatus", "employer", "occupation",
            "CreatedDate", "BackgroundCheck", "individualid", "campus"
        };
        private readonly List<string> standardrecregnames = new List<string>
        {
            "Mother", "Father", "EmContact", "EmPhone", "Allergies", "Grade", "School", "Doctor", "DocPhone", "Insurance", "Policy",
        };
    }
    public class PledgeGift
    {
        public int IndividualId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string FundName { get; set; }
        public int FundId { get; set; }
        public string FundDescription { get; set; }
        public string CheckNo { get; set; }
    }
}