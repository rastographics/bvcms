using System;
using System.Collections.Generic;
using System.Linq;
using CmsData.Codes;
using System.Text;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class Organization : ITableWithExtraValues
    {
        public static string FormatOrgName(string name, string leader, string loc)
        {
            if (loc.HasValue())
                loc = ", " + loc;
            if (leader.HasValue())
                leader = ":" + leader;
            return $"{name}{leader}{loc}";
        }

        public string FullName => FormatOrgName(OrganizationName, LeaderName, Location);

        public string FullName2 => DivisionName + ", " + FormatOrgName(OrganizationName, LeaderName, Location);

        public string Title => Util.PickFirst(RegistrationTitle, OrganizationName);

        private string tagString;
        public string TagString()
        {
            if (tagString == null)
            {
                var sb = new StringBuilder();
                var q = from d in DivOrgs
                        orderby d.Division.Name
                        select d.Division.Name;
                foreach (var name in q)
                    sb.Append(name + ",");
                if (sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);
                tagString = sb.ToString();
            }
            return tagString;
        }
        public void SetTagString(CMSDataContext db, string value)
        {
            if (!value.HasValue())
            {
                db.DivOrgs.DeleteAllOnSubmit(DivOrgs);
                return;
            }
            var a = value.Split(',');
            var qdelete = from d in DivOrgs
                          where !a.Contains(d.Division.Name)
                          select d;
            db.DivOrgs.DeleteAllOnSubmit(qdelete);

            var q = from s in a
                    join d2 in DivOrgs on s equals d2.Division.Name into g
                    from d in g.DefaultIfEmpty()
                    where d == null
                    select s;

            foreach (var s in q)
            {
                var div = db.Divisions.FirstOrDefault(d => d.Name == s);
                if (div == null)
                {
                    div = new Division { Name = s };
                    string misctags = db.Setting("MiscTagsString", "Misc Tags");
                    var prog = db.Programs.SingleOrDefault(p => p.Name == misctags);
                    if (prog == null)
                    {
                        prog = new Program { Name = misctags };
                        db.Programs.InsertOnSubmit(prog);
                    }
                    div.Program = prog;
                }
                DivOrgs.Add(new DivOrg { Division = div });
            }
            tagString = value;
        }
        public bool ToggleTag(CMSDataContext db, int divid)
        {
            var divorg = DivOrgs.SingleOrDefault(d => d.DivId == divid);
            if (divorg == null)
            {
                DivOrgs.Add(new DivOrg { DivId = divid });
                return true;
            }
            if (DivOrgs.Count == 1)
                return true;
            DivOrgs.Remove(divorg);
            db.DivOrgs.DeleteOnSubmit(divorg);
            return false;
        }
        public void AddToDiv(CMSDataContext db, int divid)
        {
            var divorg = DivOrgs.SingleOrDefault(d => d.DivId == divid);
            if (divorg == null)
                DivOrgs.Add(new DivOrg { DivId = divid });
        }

        public string PurgeOrg(CMSDataContext db)
        {
            try
            {
                db.PurgeOrganization(OrganizationId);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return null;
        }
        public void CopySettings(CMSDataContext db, int fromid)
        {
            var frorg = db.LoadOrganizationById(fromid);

            //only Copy settings
            NotifyIds = frorg.NotifyIds;
            FirstMeetingDate = frorg.FirstMeetingDate;
            LastDayBeforeExtra = frorg.LastDayBeforeExtra;
            LastMeetingDate = frorg.LastMeetingDate;
            Limit = frorg.Limit;
            RegistrationTypeId = frorg.RegistrationTypeId;
            RegStart = frorg.RegStart;
            RegEnd = frorg.RegEnd;
            IsMissionTrip = frorg.IsMissionTrip;
            AddToSmallGroupScript = frorg.AddToSmallGroupScript;

            RegSetting = frorg.RegSetting;
            RegSettingXml = frorg.RegSettingXml;

            CopySettings2(frorg, this);
            db.SubmitChanges();
        }
        public static void CopySettings2(Organization frorg, Organization toorg)
        {
            toorg.AllowNonCampusCheckIn = frorg.AllowNonCampusCheckIn;
            toorg.AllowAttendOverlap = frorg.AllowAttendOverlap;
            toorg.CanSelfCheckin = frorg.CanSelfCheckin;
            toorg.NumWorkerCheckInLabels = frorg.NumWorkerCheckInLabels;
            toorg.NoSecurityLabel = frorg.NoSecurityLabel;
            toorg.NumCheckInLabels = frorg.NumCheckInLabels;
            toorg.PhoneNumber = frorg.PhoneNumber;
            toorg.EntryPointId = frorg.EntryPointId;
            toorg.RollSheetVisitorWks = frorg.RollSheetVisitorWks;
            toorg.GradeAgeStart = frorg.GradeAgeStart;
            toorg.DivisionId = frorg.DivisionId;
        }
        public Organization CloneOrg(CMSDataContext db, int? divisionId)
        {
            var neworg = new Organization
            {
                SecurityTypeId = SecurityTypeId,
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                LeaderMemberTypeId = LeaderMemberTypeId,
                OrganizationName = OrganizationName + " (copy)",
                OrganizationStatusId = OrganizationStatusId,
                CampusId = CampusId,
                IsBibleFellowshipOrg = IsBibleFellowshipOrg,
            };
            db.Organizations.InsertOnSubmit(neworg);
            foreach (var div in DivOrgs)
                neworg.DivOrgs.Add(new DivOrg { Organization = neworg, DivId = div.DivId });
            foreach (var sc in OrgSchedules)
                neworg.OrgSchedules.Add(new OrgSchedule
                {
                    OrganizationId = OrganizationId,
                    AttendCreditId = sc.AttendCreditId,
                    SchedDay = sc.SchedDay,
                    SchedTime = sc.SchedTime,
                    Id = sc.Id
                });

            CopySettings2(this, neworg);
            db.SubmitChanges();
            return neworg;
        }
        public Organization CloneOrg(CMSDataContext db)
        {
            return CloneOrg(db, DivisionId);
        }
        public static DateTime? GetDateFromScheduleId(int id)
        {
            int dw = id / 10000 - 1;
            id %= 10000;
            if (dw == 10) // any day
                dw = DateTime.Today.DayOfWeek.ToInt();
            if (dw == 0)
                dw = 7;
            int hour = id / 100;
            int min = id % 100;
            if (hour > 0)
                return new DateTime(1900, 1, dw, hour, min, 0);
            return null;
        }
        public string DivisionName
        {
            get
            {
                return Division != null ?
                    (Division.Program != null ? Division.Program.Name : "no program")
                        + ":" + Division.Name :
                       "<span style='color:red'>need a main division</span>";
            }
        }
        public static OrgSchedule ParseSchedule(string s)
        {
            var m = Regex.Match(s, @"\A(?<dow>.*)\s(?<time>\d{1,2}:\d{2}\s(A|P)M)", RegexOptions.IgnoreCase);
            var dow = m.Groups["dow"].Value;
            var time = m.Groups["time"].Value;
            return ParseSchedule(dow, time);
        }
        public static OrgSchedule ParseSchedule(string dow, string time, int i = 1)
        {
            var d = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "sun", 0 },
                { "mon", 1 },
                { "tue", 2 },
                { "wed", 3 },
                { "thu", 4 },
                { "fri", 5 },
                { "sat", 6 },
                { "any", 10 },
            };

            if (!dow.HasValue())
                dow = "sun";
            dow = dow.Truncate(3);
            var re = new Regex(@"(?<h>\d+)(?::?)(?<s>\d{2})?\s*(?<a>(?:A|P)M)?", RegexOptions.IgnoreCase);
            var m = re.Match(time);
            var h = m.Groups["h"].Value;
            var s = Util.PickFirst(m.Groups["s"].Value, "00");
            var a = Util.PickFirst(m.Groups["a"].Value, "AM");
            var ts = $"{h}:{s} {a}";
            var t = DateTime.Parse(ts);
            var mt = Util.Now.Sunday().AddDays(d[dow]).Add(t.TimeOfDay);
            //SELECT @id = (ISNULL(@day, DATEPART(dw, @time)-1) + 1) * 10000 + DATEPART(hour, @time) * 100 + DATEPART(mi, @time)
            var schedid = (d[dow] + 1) * 10000 + t.TimeOfDay.Hours * 100 + t.TimeOfDay.Minutes;
            var sc = new OrgSchedule
            {
                Id = i,
                SchedDay = d[dow],
                SchedTime = mt,
                AttendCreditId = 1,
                ScheduleId = schedid
            };
            return sc;
        }
        public static OrganizationType FetchOrCreateType(CMSDataContext db, string type)
        {
            var t = db.OrganizationTypes.SingleOrDefault(pp => pp.Description == type);
            if (t == null)
            {
                var max = 10;
                if (db.OrganizationTypes.Any())
                    max = db.OrganizationTypes.Max(mm => mm.Id) + 10;
                t = new OrganizationType { Description = type, Code = type.Substring(0, 3), Id = max };
                db.OrganizationTypes.InsertOnSubmit(t);
                db.SubmitChanges();
            }
            return t;
        }
        public static Program FetchOrCreateProgram(CMSDataContext db, string program)
        {
            var p = db.Programs.SingleOrDefault(pp => pp.Name == program);
            if (p == null)
            {
                p = new Program { Name = program };
                db.Programs.InsertOnSubmit(p);
                db.SubmitChanges();
            }
            return p;
        }
        public static Division FetchOrCreateDivision(CMSDataContext db, Program program, string division)
        {
            var d = db.Divisions.SingleOrDefault(pp => pp.Name == division && pp.ProgDivs.Any(pd => pd.ProgId == program.Id));
            if (d == null)
            {
                d = new Division { Name = division, Program = program };
                var progdiv = new ProgDiv { Division = d, Program = program };
                db.ProgDivs.InsertOnSubmit(progdiv);
                db.SubmitChanges();
            }
            else
            {
                var pd = db.ProgDivs.SingleOrDefault(dd => dd.ProgId == program.Id && dd.DivId == d.Id);
                if (pd == null)
                    program.Divisions.Add(d);
                db.SubmitChanges();
            }
            return d;
        }
        public static MemberType FetchOrCreateMemberType(CMSDataContext db, string type)
        {
            var mt = db.MemberTypes.SingleOrDefault(pp => pp.Description == type);
            if (mt == null)
            {
                var max = db.MemberTypes.Max(mm => mm.Id) + 10;
                if (max < 1000)
                    max = 1010;
                mt = new MemberType { Id = max, Description = type, Code = type.Truncate(20), AttendanceTypeId = AttendTypeCode.Member };
                db.MemberTypes.InsertOnSubmit(mt);
                db.SubmitChanges();
            }
            return mt;
        }
        public static Organization FetchOrCreateOrganization(CMSDataContext db, int divid, string organization)
        {
            var o = db.LoadOrganizationByName(organization, divid);
            if (o == null)
                return CreateOrganization(db, divid, organization);
            return o;
        }
        public static Organization FetchOrCreateOrganization(CMSDataContext db, Division division, string organization)
        {
            var o = db.LoadOrganizationByName(organization, division.Id);
            if (o == null)
                return CreateOrganization(db, division, organization);
            return o;
        }
        //public static Organization FetchOrCreateOrganization(CMSDataContext Db, Division division, string organization, string description)
        //{
        //    var o = Db.Organizations.SingleOrDefault(oo => oo.Description == description);
        //    if (o == null)
        //    {
        //        o = CreateOrganization(Db, division, organization);
        //        o.Description = description;
        //    }
        //    return o;
        //}
        public static Organization CreateOrganization(CMSDataContext db, Division division, string organization)
        {
            var o = new Organization
            {
                OrganizationName = organization,
                SecurityTypeId = 0,
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                OrganizationStatusId = 30,
            };
            division.Organizations.Add(o);
            db.DivOrgs.InsertOnSubmit(new DivOrg { Division = division, Organization = o });
            db.SubmitChanges();
            return o;
        }
        public static Organization CreateOrganization(CMSDataContext db, int divid, string organization)
        {
            var o = new Organization
            {
                OrganizationName = organization.Truncate(100),
                SecurityTypeId = 0,
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                OrganizationStatusId = 30,
                DivisionId = divid,
            };
            db.DivOrgs.InsertOnSubmit(new DivOrg { DivId = divid, Organization = o });
            db.SubmitChanges();
            return o;
        }

        public static void AddAsPreviousMember(CMSDataContext db, int oid, int pid, string orgname, int mbrid, DateTime joindt, DateTime dropdt, int userid)
        {
            db.EnrollmentTransactions.InsertOnSubmit(
                new EnrollmentTransaction
                {
                    OrganizationId = oid,
                    PeopleId = pid,
                    OrganizationName = orgname,
                    MemberTypeId = mbrid,
                    TransactionDate = joindt,
                    TransactionTypeId = 1,
                    CreatedBy = userid,
                    CreatedDate = Util.Now,
                });
            db.EnrollmentTransactions.InsertOnSubmit(
                new EnrollmentTransaction
                {
                    OrganizationId = oid,
                    PeopleId = pid,
                    OrganizationName = orgname,
                    MemberTypeId = mbrid,
                    TransactionDate = dropdt,
                    TransactionTypeId = 5,
                    CreatedBy = userid,
                    CreatedDate = Util.Now,
                });
            db.SubmitChanges();
        }

        public OrganizationExtra GetExtraValue(string field)
        {
            field = field.Trim();
            var ev = OrganizationExtras.AsEnumerable().FirstOrDefault(ee => ee.Field == field);
            if (ev == null)
            {
                ev = new OrganizationExtra()
                {
                    OrganizationId = OrganizationId,
                    Field = field,

                };
                OrganizationExtras.Add(ev);
            }
            return ev;
        }
        public static OrganizationExtra GetExtraValue(CMSDataContext db, int id, string field)
        {
            field = field.Trim();
            var q = from v in db.OrganizationExtras
                    where v.Field == field
                    where v.OrganizationId == id
                    select v;
            var ev = q.SingleOrDefault();
            if (ev == null)
            {
                ev = new OrganizationExtra
                {
                    OrganizationId = id,
                    Field = field,
                    TransactionTime = DateTime.Now
                };
                db.OrganizationExtras.InsertOnSubmit(ev);
            }
            return ev;
        }

        public void AddEditExtra(CMSDataContext db, string field, string value, bool multiline = false)
        {
            var oev = db.OrganizationExtras.SingleOrDefault(oe => oe.OrganizationId == OrganizationId && oe.Field == field);
            if (oev == null)
            {
                oev = new OrganizationExtra
                {
                    OrganizationId = OrganizationId,
                    Field = field,
                };
                db.OrganizationExtras.InsertOnSubmit(oev);
            }
            oev.Data = value;
            oev.DataType = multiline ? "text" : null;
        }
        public void AddToExtraText(string field, string value)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.DataType = "text";
            if (ev.Data.HasValue())
                ev.Data = value + "\n" + ev.Data;
            else
                ev.Data = value;
        }

        public static string GetExtra(CMSDataContext db, int? id, string field)
        {
            var oev = db.OrganizationExtras.SingleOrDefault(oe => oe.OrganizationId == id && oe.Field == field);
            if (oev == null)
                return "";
            if (oev.StrValue.HasValue())
                return oev.StrValue;
            if (oev.Data.HasValue())
                return oev.Data;
            if (oev.DateValue.HasValue)
                return oev.DateValue.FormatDate();
            if (oev.IntValue.HasValue)
                return oev.IntValue.ToString();
            return oev.BitValue.ToString();
        }
        public string GetExtra(CMSDataContext db, string field)
        {
            return GetExtra(db, OrganizationId, field);
        }

        public void AddEditExtraCode(string field, string value, string location = null)
        {
            if (!field.HasValue())
                return;
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.StrValue = value;
            ev.TransactionTime = DateTime.Now;
        }

        public void AddEditExtraText(string field, string value, DateTime? dt = null)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.Data = value;
            ev.TransactionTime = dt ?? DateTime.Now;
        }

        public void AddEditExtraDate(string field, DateTime? value)
        {
            if (!value.HasValue)
                return;
            var ev = GetExtraValue(field);
            ev.DateValue = value;
            ev.TransactionTime = DateTime.Now;
        }

        public void AddEditExtraInt(string field, int value)
        {
            var ev = GetExtraValue(field);
            ev.IntValue = value;
            ev.TransactionTime = DateTime.Now;
        }

        public void AddEditExtraBool(string field, bool tf, string name = null, string location = null)
        {
            if (!field.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.BitValue = tf;
            ev.TransactionTime = DateTime.Now;
        }

        public void AddEditExtraValue(string field, string code, DateTime? date, string text, bool? bit, int? intn, DateTime? dt = null)
        {
            var ev = GetExtraValue(field);
            ev.StrValue = code;
            ev.Data = text;
            ev.DateValue = date;
            ev.IntValue = intn;
            ev.BitValue = bit;
            ev.UseAllValues = true;
            ev.TransactionTime = dt ?? DateTime.Now;
        }

        public void RemoveExtraValue(CMSDataContext db, string field)
        {
            var ev = OrganizationExtras.AsEnumerable().FirstOrDefault(ee => string.Compare(ee.Field, field, ignoreCase: true) == 0);
            if (ev != null)
            {
                db.OrganizationExtras.DeleteOnSubmit(ev);
                ev.TransactionTime = DateTime.Now;
            }
        }

        public void LogExtraValue(string op, string field)
        {
            DbUtil.LogActivity($"EVOrg {op}:{field}", orgid: OrganizationId);
        }

        public static bool CheckExtraValueIntegrity(CMSDataContext db, string type, string newfield)
        {
            return !db.OrganizationExtras.Any(ee => ee.Field == newfield && ee.Type != type);
        }

        public static void AddEditExtraValue(CMSDataContext db, int id, string field, string value)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(db, id, field);
            ev.StrValue = value;
            ev.TransactionTime = DateTime.Now;
        }
        public static void AddEditExtraData(CMSDataContext db, int id, string field, string value)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(db, id, field);
            ev.Data = value;
            ev.TransactionTime = DateTime.Now;
        }
        public static void AddEditExtraDate(CMSDataContext db, int id, string field, DateTime? value)
        {
            if (!value.HasValue)
                return;
            var ev = GetExtraValue(db, id, field);
            ev.DateValue = value;
            ev.TransactionTime = DateTime.Now;
        }
        public static void AddEditExtraInt(CMSDataContext db, int id, string field, int? value)
        {
            if (!value.HasValue)
                return;
            var ev = GetExtraValue(db, id, field);
            ev.IntValue = value;
            ev.TransactionTime = DateTime.Now;
        }
        public static void AddEditExtraBool(CMSDataContext db, int id, string field, bool? value)
        {
            if (!value.HasValue)
                return;
            var ev = GetExtraValue(db, id, field);
            ev.BitValue = value;
            ev.TransactionTime = DateTime.Now;
        }

        private int? regLimitCount;
        public int RegLimitCount(CMSDataContext db)
        {
            if (!regLimitCount.HasValue)
                regLimitCount = db.OrganizationMemberCount2(OrganizationId) ?? 0;
            return regLimitCount.Value;
        }
        public IEnumerable<OrganizationExtra> GetOrganizationExtras()
        {
            return OrganizationExtras.OrderBy(pp => pp.Field);
        }

        public void UpdateRegSetting(Registration.Settings os)
        {
            RegSettingXml = Util.Serialize(os);
        }
        public static void AddMemberTag(CMSDataContext db, int orgId, string name)
        {
            if (!name.HasValue())
                return;
            var name2 = name.Trim().Truncate(200);
            var mt = db.MemberTags.SingleOrDefault(t => t.Name == name2 && t.OrgId == orgId);
            if (mt == null)
            {
                mt = new MemberTag { Name = name2, OrgId = orgId };
                db.MemberTags.InsertOnSubmit(mt);
                db.SubmitChanges();
            }
            db.SubmitChanges();
        }

        public bool IsOnePageOnlineGiving(CMSDataContext db)
        {
            if (RegistrationTypeId != RegistrationTypeCode.OnlineGiving)
                return false;
            var settings = Registration.Settings.CreateSettings(RegSettingXml, db, this);
            return !settings.AskDonation;
        }

        public void AddMemberTag(string sg)
        {
            sg = sg.Trim();
            if (MemberTags.Any(vv => vv.Name == sg))
                return;
            MemberTags.Add(new MemberTag() { Name = sg });
        }
    }
}
