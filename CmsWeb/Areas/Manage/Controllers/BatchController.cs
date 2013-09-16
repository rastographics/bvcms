using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Dapper;
using DocumentFormat.OpenXml.Drawing;
using UtilityExtensions;
using System.Text;
using CmsData;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using CmsWeb.Models;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using Alias = System.Threading.Tasks;

namespace CmsWeb.Areas.Manage.Controllers
{
    [ValidateInput(false)]
    public class BatchController : CmsStaffAsyncController
    {
        [Authorize(Roles = "Admin")]
        public ActionResult MoveAndDelete()
        {
            return View();
        }
        [HttpPost]
        [AsyncTimeout(600000)]
        public void MoveAndDeleteAsync(string text)
        {
            AsyncManager.OutstandingOperations.Increment();
            string host = Util.Host;
            ThreadPool.QueueUserWorkItem((e) =>
            {
                var sb = new StringBuilder();
                sb.Append("<h2>done</h2>\n<p><a href='/'>home</a></p>\n");
                using (var csv = new CsvReader(new StringReader(text), false, '\t'))
                {
                    while (csv.ReadNextRecord())
                    {
                        if (csv.FieldCount != 2)
                        {
                            sb.AppendFormat("expected two ids, row {0}<br/>\n", csv[0]);
                            continue;
                        }

                        var fromid = csv[0].ToInt();
                        var toid = csv[1].ToInt();
                        var Db = new CMSDataContext(Util.GetConnectionString(host));
                        var p = Db.LoadPersonById(fromid);

                        if (p == null)
                        {
                            sb.AppendFormat("fromid {0} not found<br/>\n", fromid);
                            Db.Dispose();
                            continue;
                        }
                        var tp = Db.LoadPersonById(toid);
                        if (tp == null)
                        {
                            sb.AppendFormat("toid {0} not found<br/>\n", toid);
                            Db.Dispose();
                            continue;
                        }
                        try
                        {
                            p.MovePersonStuff(Db, toid);
                            Db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            sb.AppendFormat("error on move ({0}, {1}): {2}<br/>\n", fromid, toid, ex.Message);
                            Db.Dispose();
                            continue;
                        }
                        try
                        {
                            Db.PurgePerson(fromid);
                            sb.AppendFormat("moved ({0}, {1}) successful<br/>\n", fromid, toid);
                        }
                        catch (Exception ex)
                        {
                            sb.AppendFormat("error on delete ({0}): {1}<br/>\n", fromid, ex.Message);
                        }
                        finally
                        {
                            Db.Dispose();
                        }
                    }
                }
                AsyncManager.Parameters["results"] = sb.ToString();
                AsyncManager.OutstandingOperations.Decrement();
            });
        }
        public ActionResult MoveAndDeleteCompleted(string results)
        {
            return Content(results);
        }
        public ActionResult Grade(string text)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                ViewData["text"] = "";
                return View();
            }
            var batch = from s in text.Split('\n')
                        where s.HasValue()
                        let a = s.SplitStr("\t", 3)
                        select new { pid = a[0].ToInt(), oid = a[1].ToInt(), grade = a[2].ToInt() };
            foreach (var i in batch)
            {
                var m = DbUtil.Db.OrganizationMembers.Single(om => om.OrganizationId == i.oid && om.PeopleId == i.pid);
                m.Grade = i.grade;
            }
            DbUtil.Db.SubmitChanges();

            return Content("done");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult RegistrationMail(string text)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                ViewData["text"] = "";
                return View();
            }
            var batch = from s in text.Split('\n')
                        where s.HasValue()
                        let a = s.SplitStr("\t", 2)
                        select new { pid = a[0].ToInt(), em = a[1] };
            foreach (var i in batch)
            {
                var m = DbUtil.Db.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == 88485 && om.PeopleId == i.pid);
                if (m == null)
                    continue;
                m.RegisterEmail = i.em;
            }
            DbUtil.Db.SubmitChanges();

            return Content("done");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateOrg(string text)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                ViewData["text"] = "";
                return View();
            }
            var lines = text.Split('\n');
            var names = lines[0].Split('\t');

            for (var i = 1; i < lines.Length; i++)
            {
                var a = lines[i].Split('\t');
                var oid = a[0].ToInt();
                var o = DbUtil.Db.LoadOrganizationById(oid);
                for (var c = 1; c < a.Length; c++)
                    switch (names[c].Trim())
                    {
                        case "Campus":
                            if (a[c].TrimEnd().AllDigits())
                            {
                                o.CampusId = a[c].ToInt();
                                if (o.CampusId == 0)
                                    o.CampusId = null;
                            }
                            break;
                        case "CanSelfCheckin":
                            o.CanSelfCheckin = a[c].ToBool2();
                            break;
                        case "RegStart":
                            o.RegStart = a[c].ToDate();
                            break;
                        case "RegEnd":
                            o.RegEnd = a[c].ToDate();
                            break;
                        case "Schedule":
                            if (a[c].HasValue() && a[c] != "None")
                            {
                                var sc = o.OrgSchedules.FirstOrDefault();
                                var scin = Organization.ParseSchedule(a[c].TrimEnd());
                                if (sc != null)
                                {
                                    sc.SchedDay = scin.SchedDay;
                                    sc.SchedTime = scin.SchedTime;
                                }
                                else
                                    o.OrgSchedules.Add(scin);
                            }
                            break;
                        case "BirthDayStart":
                            o.BirthDayStart = a[c].ToDate();
                            break;
                        case "BirthDayEnd":
                            o.BirthDayEnd = a[c].ToDate();
                            break;
                        case "EntryPoint":
                            if (a[c].TrimEnd().AllDigits())
                            {
                                var id = a[c].ToInt();
                                if (id > 0)
                                    o.EntryPointId = id;
                            }
                            break;
                        case "LeaderType":
                            if (a[c].TrimEnd().AllDigits())
                            {
                                var id = a[c].ToInt();
                                if (id > 0)
                                    o.LeaderMemberTypeId = id;
                            }
                            break;
                        case "SecurityType":
                            o.SecurityTypeId = string.Compare(a[c], "LeadersOnly", true) == 0 ? 2 : string.Compare(a[c], "UnShared", true) == 0 ? 3 : 0;
                            break;
                        case "FirstMeeting":
                            o.FirstMeetingDate = a[c].ToDate();
                            break;
                        case "Gender":
                            o.GenderId = a[c] == "Male" ? (int?)1 : a[c] == "Female" ? (int?)2 : null;
                            break;
                        case "GradeAgeStart":
                            o.GradeAgeStart = a[c].ToInt2();
                            break;
                        case "MainFellowshipOrg":
                            o.IsBibleFellowshipOrg = a[c].ToBool2();
                            break;
                        case "LastDayBeforeExtra":
                            o.LastDayBeforeExtra = a[c].ToDate();
                            break;
                        case "LastMeeting":
                            o.LastMeetingDate = a[c].ToDate();
                            break;
                        case "Limit":
                            o.Limit = a[c].ToInt2();
                            break;
                        case "Location":
                            o.Location = a[c];
                            break;
                        case "Name":
                            o.OrganizationName = a[c];
                            break;
                        case "NoSecurityLabel":
                            o.NoSecurityLabel = a[c].ToBool2();
                            break;
                        case "NumCheckInLabels":
                            o.NumCheckInLabels = a[c].ToInt2();
                            break;
                        case "NumWorkerCheckInLabels":
                            o.NumWorkerCheckInLabels = a[c].ToInt2();
                            break;
                        case "OnLineCatalogSort":
                            o.OnLineCatalogSort = a[c] == "0" ? (int?)null : a[c].ToInt2();
                            break;
                        case "OrganizationStatusId":
                            o.OrganizationStatusId = a[c].ToInt();
                            break;
                        case "PhoneNumber":
                            o.PhoneNumber = a[c];
                            break;
                        case "RollSheetVisitorWks":
                            o.RollSheetVisitorWks = a[c] == "0" ? (int?)null : a[c].ToInt2();
                            break;
                    }
                DbUtil.Db.SubmitChanges();
            }
            return Redirect("/");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult UpdatePeople0(string text)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                ViewData["text"] = "";
                return View();
            }
            var list = text.Split('\n').Select(li => li.Split('\t'));
            var list0 = list.First().ToList();
            var names = list0.ToDictionary(i => i.TrimEnd(),
                i => list0.FindIndex(s => s == i));

            if (names.ContainsKey("Campus"))
            {
                var campuslist = (from li in list.Skip(1)
                                  where li.Length == names.Count
                                  group li by li[names["Campus"]] into campus
                                  select campus.Key).ToList();
                var dbc = from c in campuslist
                          join cp in DbUtil.Db.Campus on c equals cp.Description into j
                          from cp in j.DefaultIfEmpty()
                          select new { cp, c };
                var clist = dbc.ToList();
                var maxcampusid = DbUtil.Db.Campus.Max(c => c.Id);
                foreach (var i in clist)
                    if (i.cp == null)
                    {
                        var cp = new Campu { Description = i.c, Id = ++maxcampusid };
                        DbUtil.Db.Campus.InsertOnSubmit(cp);
                    }
            }
            DbUtil.Db.SubmitChanges();
            var campuses = DbUtil.Db.Campus.ToDictionary(cp => cp.Description, cp => cp.Id);

            var q = from li in list.Skip(1)
                    where li.Length == names.Count
                    select li;

            foreach (var li in q)
            {
                DateTime? dob = null;
                if (names.ContainsKey("Birthday"))
                    dob = li[names["Birthday"]].ToDate();
                string email = "", cell = "", homephone = "";
                if (names.ContainsKey("CellPhone"))
                    cell = li[names["CellPhone"]].GetDigits();
                if (names.ContainsKey("Email"))
                    email = li[names["Email"]].Trim();
                if (names.ContainsKey("HomePhone"))
                    homephone = li[names["HomePhone"]].GetDigits();
                var first = li[names["First"]].Trim();
                var last = li[names["Last"]].Trim();
                var pid = DbUtil.Db.FindPerson3(first, last, dob, email, cell, homephone, null).FirstOrDefault();
                if (pid == null)
                    continue;
                var p = DbUtil.Db.LoadPersonById(pid.PeopleId.Value);

                foreach (var name in names)
                    switch (name.Key)
                    {
                        case "Title":
                            p.TitleCode = li[name.Value];
                            break;
                        case "Campus":
                            p.CampusId = campuses[li[name.Value]];
                            break;
                        case "Gender":
                            switch (li[name.Value])
                            {
                                case "Male":
                                    p.GenderId = 1;
                                    break;
                                case "Female":
                                    p.GenderId = 2;
                                    break;
                            }
                            break;
                        case "Married":
                            switch (li[name.Value])
                            {
                                case "Married":
                                    p.MaritalStatusId = 20;
                                    break;
                                case "Single":
                                    p.MaritalStatusId = 10;
                                    break;
                                default:
                                    p.MaritalStatusId = 0;
                                    break;
                            }
                            break;
                        case "Address":
                            p.AddressLineOne = li[name.Value];
                            break;
                        case "Address2":
                            p.AddressLineTwo = li[name.Value];
                            break;
                        case "City":
                            p.CityName = li[name.Value];
                            break;
                        case "State":
                            p.StateCode = li[name.Value];
                            break;
                        case "Zip":
                            p.ZipCode = li[name.Value];
                            break;
                        case "Position":
                            switch (li[name.Value])
                            {
                                case "Primary":
                                    p.PositionInFamilyId = 10;
                                    break;
                                case "Secondary":
                                    p.PositionInFamilyId = 20;
                                    break;
                                case "Child":
                                    p.PositionInFamilyId = 30;
                                    break;
                            }
                            break;
                        case "Birthday":
                            p.DOB = dob.FormatDate();
                            break;
                        case "CellPhone":
                            p.CellPhone = cell;
                            break;
                        case "HomePhone":
                            p.HomePhone = homephone;
                            break;
                        case "Email":
                            p.EmailAddress = email;
                            break;
                    }
                DbUtil.Db.SubmitChanges();
            }
            return Redirect("/");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateFields() // UpdateForATag
        {
            var m = new UpdateFieldsModel();
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                var success = (string)TempData["success"];
                if (success.HasValue())
                    ViewData["success"] = success;
                ViewData["text"] = "";
                return View(m);
            }
            UpdateModel(m);
            var a = m.Tag.SplitStr(":", 2);
            if (a.Length > 1)
                a[1] = a[1].TrimStart();
            IQueryable<Person> q = null;
            if (a[0] == "last query")
            {
                q = DbUtil.Db.PeopleQuery(Util.QueryBuilderScratchPadId);
            }
            else if (a[0] == "tag")
            {
                var b = a[1].SplitStr(":", 2);
                var tag = DbUtil.Db.TagById(b[0].ToInt());
                q = tag.People(DbUtil.Db);
            }
            else if (a[0] == "exval")
            {
                var b = a[1].SplitStr(":", 2);
                q = from e in DbUtil.Db.PeopleExtras
                    where e.Field == b[0]
                    where e.StrValue == b[1]
                    select e.Person;
            }
            foreach (var p in q)
            {
                switch (m.Field)
                {
                    case "Member Status":
                        p.MemberStatusId = m.NewValue.ToInt();
                        break;
                    case "Drop All Memberships":
                        p.DropMemberships(DbUtil.Db);
                        break;
                    case "Deceased Date":
                        p.DeceasedDate = m.NewValue.ToDate();
                        break;
                    case "New Member Class":
                        p.NewMemberClassStatusId = m.NewValue.ToInt();
                        break;
                    case "Drop Type":
                        p.DropCodeId = m.NewValue.ToInt();
                        break;
                    case "Baptism Status":
                        p.BaptismStatusId = m.NewValue.ToInt();
                        break;
                    case "Baptism Type":
                        p.BaptismTypeId = m.NewValue.ToInt();
                        break;
                    case "Campus":
                        p.CampusId = m.NewValue.ToInt();
                        break;
                    case "Marital Status":
                        p.MaritalStatusId = m.NewValue.ToInt();
                        break;
                    case "Family Position":
                        p.PositionInFamilyId = m.NewValue.ToInt();
                        break;
                    case "Entry Point":
                        p.EntryPointId = m.NewValue.ToInt();
                        break;
                    case "Gender":
                        p.GenderId = m.NewValue.ToInt();
                        break;
                    case "Occupation":
                        p.OccupationOther = m.NewValue;
                        break;
                    case "School":
                        p.SchoolOther = m.NewValue;
                        break;
                    case "Employer":
                        p.EmployerOther = m.NewValue;
                        break;
                    case "Grade":
                        if (m.NewValue == "+1")
                            p.Grade = p.Grade + 1;
                        else
                            p.Grade = m.NewValue.ToInt();
                        break;
                    case "Statement Options":
                        p.ContributionOptionsId = m.NewValue.ToInt();
                        break;
                    case "Envelope Options":
                        p.EnvelopeOptionsId = m.NewValue.ToInt();
                        break;
                    case "Approval Codes":
                        {
                            var i = p.Volunteers.SingleOrDefault();
                            if (i == null)
                            {
                                i = new Volunteer { PeopleId = p.PeopleId };
                                DbUtil.Db.Volunteers.InsertOnSubmit(i);
                                DbUtil.Db.SubmitChanges();
                            }

                            var code = m.NewValue.ToInt();
                            var app = (from v in DbUtil.Db.VoluteerApprovalIds
                                       where v.ApprovalId == Math.Abs(code)
                                       where v.PeopleId == p.PeopleId
                                       select v).SingleOrDefault();
                            if (code < 0)
                            {
                                if (app != null)
                                    DbUtil.Db.VoluteerApprovalIds.DeleteOnSubmit(app);
                            }
                            else if (code > 0)
                            {
                                if (app == null)
                                    p.VoluteerApprovalIds.Add(new VoluteerApprovalId { ApprovalId = code, });
                            }
                            else if (code == 0)
                            {
                                DbUtil.Db.VoluteerApprovalIds.DeleteAllOnSubmit(p.VoluteerApprovalIds);
                            }
                        }
                        break;
                    case "Background Check Date":
                        {
                            var i = p.Volunteers.SingleOrDefault();
                            if (i == null)
                            {
                                i = new Volunteer { PeopleId = p.PeopleId };
                                DbUtil.Db.Volunteers.InsertOnSubmit(i);
                                DbUtil.Db.SubmitChanges();
                            }
                            i.ProcessedDate = m.NewValue.ToDate();
                        }
                        break;
                    case "Title":
                        p.TitleCode = m.NewValue;
                        break;
                    case "ReceiveSMS":
                        p.ReceiveSMS = m.NewValue.ToBool();
                        break;
                }
                DbUtil.Db.SubmitChanges();
            }
            TempData["success"] = m.Field + " Updated";
            return RedirectToAction("UpdateFields");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult UpdatePeople()
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                ViewData["text"] = "";
                return View();
            }
            var file = Request.Files[0];
            if (file.ContentLength == 0)
                return Content("empty file");
            var path = Server.MapPath("/Upload/" + Guid.NewGuid().ToCode() + ".xls");
            file.SaveAs(path);
            try
            {
                UpdatePeopleModel.UpdatePeople(path, Util.Host, Util.UserPeopleId.Value);
            }
            finally
            {
                System.IO.File.Delete(path);
            }
            return Content("<div>done <a href='/'>go home</a><div>");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult LookupDataPage()
        {
            return View(new UpdateFieldsModel().TitleItems());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UpdateStatusFlags()
        {
            var qbits = DbUtil.Db.StatusFlags().ToList();
            foreach (var a in qbits)
            {
                var t = DbUtil.Db.FetchOrCreateSystemTag(a[0]);
                DbUtil.Db.TagAll2(DbUtil.Db.PeopleQuery(a[0] + ":" + a[1]), t);
            }
            return View();
        }
        [AsyncTimeout(600000)]
        [Authorize(Roles = "Admin")]
        public void UpdateQueryStatsAsync()
        {
            AsyncManager.OutstandingOperations.Increment();
            string host = Util.Host;
            var uid = Util.UserId;
            ThreadPool.QueueUserWorkItem((e) =>
            {
                Util.SessionId = Guid.NewGuid().ToString();
                var Db = new CMSDataContext(Util.GetConnectionString(host));
                var d = DateTime.Today.Subtract(DateTime.Parse("1/1/1900")).Days;
                var list = Db.QueryStats.Where(ss => ss.RunId == d);
                foreach (var a in Db.QueryStatClauses())
                {
                    var st = list.SingleOrDefault(ss => ss.StatId == a[0]);
                    if (st == null)
                    {
                        st = new QueryStat { StatId = a[0], Description = a[1], RunId = d, Runtime = DateTime.Now };
                        Db.QueryStats.InsertOnSubmit(st);
                    }
                    st.Count = Db.PeopleQuery(a[0] + ":" + a[1]).Count();
                    Db.SubmitChanges();
                }
                AsyncManager.OutstandingOperations.Decrement();
            });
        }
        public ActionResult UpdateQueryStatsCompleted()
        {
            return Redirect("/");
        }

        public class FindInfo
        {
            public int? PeopleId { get; set; }
            public int Found { get; set; }
            public string First { get; set; }
            public string Last { get; set; }
            public string Email { get; set; }
            public string CellPhone { get; set; }
            public string HomePhone { get; set; }
            public DateTime? Birthday { get; set; }
        }
        [HttpGet]
        [Authorize(Roles = "Edit")]
        public ActionResult FindTagPeople()
        {
            return View("FindTagPeople0");
        }
        [HttpPost]
        string FindColumn(Dictionary<string, int> names, string[] a, string col)
        {
            if (names.ContainsKey(col))
                return a[names[col]];
            return null;
        }
        string FindColumnDigits(Dictionary<string, int> names, string[] a, string col)
        {
            var s = FindColumn(names, a, col);
            if (s.HasValue())
                return s.GetDigits();
            return s;
        }
        DateTime? FindColumnDate(Dictionary<string, int> names, string[] a, string col)
        {
            var s = FindColumn(names, a, col);
            DateTime dt;
            if (names.ContainsKey(col))
                if (DateTime.TryParse(a[names[col]], out dt))
                    return dt;
            return null;
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult FindTagPeople(string text, string tagname)
        {
            if (!tagname.HasValue())
                return Content("no tag");
            var csv = new CsvReader(new StringReader(text), false, '\t').ToList();

            if (!csv.Any())
                return Content("no data");
            var line0 = csv.First().ToList();
            var names = line0.ToDictionary(i => i.TrimEnd(),
                i => line0.FindIndex(s => s == i));
            var ActiveNames = new List<string>
            {
                "First",
                "Last",
                "Birthday",
                "Email",
                "CellPhone",
                "HomePhone",
            };
            var hasvalidcolumn = false;
            foreach (var name in names.Keys)
                if (ActiveNames.Contains(name))
                {
                    hasvalidcolumn = true;
                    break;
                }
            if (!hasvalidcolumn)
                return Content("no valid column");


            var list = new List<FindInfo>();
            foreach (var a in csv.Skip(1))
            {
                var row = new FindInfo();
                row.First = FindColumn(names, a, "First");
                row.Last = FindColumn(names, a, "Last");
                row.Birthday = FindColumnDate(names, a, "Birthday");
                row.Email = FindColumn(names, a, "Email");
                row.CellPhone = FindColumnDigits(names, a, "CellPhone");
                row.HomePhone = FindColumnDigits(names, a, "HomePhone");

                var pids = DbUtil.Db.FindPerson3(row.First, row.Last, row.Birthday, row.Email, row.CellPhone, row.HomePhone, null);
                row.Found = pids.Count();
                if (row.Found == 1)
                    row.PeopleId = pids.Single().PeopleId.Value;
                list.Add(row);
            }
            var q = from pi in list
                    where pi.PeopleId.HasValue
                    select pi.PeopleId;
            foreach (var pid in q.Distinct())
                Person.Tag(DbUtil.Db, pid.Value, tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            DbUtil.Db.SubmitChanges();

            return View(list);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize(Roles = "Edit")]
        public ActionResult TagPeopleIds()
        {
            return View();
        }
        public ActionResult TagUploadPeopleIds(string name, string text, bool newtag)
        {
            var q = from line in text.Split('\n')
                    select line.ToInt();
            if (newtag)
            {
                var tag = DbUtil.Db.FetchTag(name, Util.UserPeopleId, (int)DbUtil.TagTypeId_Personal);
                if (tag != null)
                    DbUtil.Db.ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
            }
            foreach (var pid in q)
            {
                Person.Tag(DbUtil.Db, pid, name, DbUtil.Db.CurrentUser.PeopleId, (int)DbUtil.TagTypeId_Personal);
                DbUtil.Db.SubmitChanges();
            }
            return Redirect("/Tags?tag=" + name);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult ExtraValuesFromPeopleIds()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ExtraValuesFromPeopleIds(string text, string field)
        {
            var csv = new CsvReader(new StringReader(text), false, '\t').ToList();
            foreach (var a in csv)
            {
                var p = DbUtil.Db.LoadPersonById(a[0].ToInt());
                p.AddEditExtraValue(field, a[1]);
                DbUtil.Db.SubmitChanges();
            }
            return Redirect("/Reports/ExtraValues");
        }
        [HttpGet]
        public ActionResult Test()
        {
            ViewData["guid"] = Guid.NewGuid().ToString();
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Finance")]
        public ActionResult DoGiving()
        {
            ManagedGiving.DoAllGiving(DbUtil.Db);
            return Content("done");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult SQLView(string id)
        {
            try
            {
                var cmd = new SqlCommand("select * from guest." + id.Replace(" ", ""));
                cmd.Connection = new SqlConnection(Util.ConnectionString);
                cmd.Connection.Open();
                var rdr = cmd.ExecuteReader();
                return View(rdr);
            }
            catch (Exception)
            {
                return Content("cannot find view guest." + id);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult RunScript(string id)
        {
            try
            {
                var script = DbUtil.Db.Content(id);
                var pe = new PythonEvents(DbUtil.Db, id, script.Body);
                pe.instance.Run();
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
            return Content("done");
        }
        [HttpGet]
        [Authorize(Roles = "Developer")]
        public ActionResult ConvertQueries()
        {
            List<int> list;
            var cs = DbUtil.Db.Connection.ConnectionString;
            using (var cn2 = new SqlConnection(cs))
            {
                cn2.Open();
                cn2.Execute("Truncate table dbo.Query");
                var q = cn2.Query<int>("select QueryId from QueryBuilderClauses where GroupId is null and Description is not null and savedby is not null");
                list = q.ToList();
            }
            foreach (var qid in list)
            {
                var db = new CMSDataContext(cs);
                var c = db.LoadQueryById(qid);
                var xml = c.ToXml("conv", qid);
                string name = null;
                if (c.Description != Util.ScratchPad)
                    name = c.Description.Truncate(50);
                var nc = Condition.Import(xml, name);
                xml = nc.ToXml();
                if (!nc.Conditions.Any())
                    continue;
                var q = new Query()
                {
                    QueryId = nc.Id,
                    Name = name ?? Util.ScratchPad2,
                    Text = xml,
                    Owner = c.SavedBy,
                    Created = c.CreatedOn,
                    LastRun = c.CreatedOn,
                    Ispublic = c.IsPublic,
                    RunCount = 0
                };
                db.Queries.InsertOnSubmit(q);
                db.SubmitChanges();
                db.Dispose();
            }
            return Redirect("/SavedQuery2");
        }

    }
}
