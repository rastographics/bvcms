using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using CmsData.Codes;
using UtilityExtensions;
using IronPython.Hosting;
using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.RegularExpressions;
using CmsData.API;
using Dapper;
using Microsoft.Scripting.Hosting;
using HandlebarsDotNet;

namespace CmsData
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PythonEvents : IPythonApi
    {
        private CMSDataContext db;

        private Dictionary<string, object> dictionary { get; set; }
        public dynamic instance { get; set; }

        private void ResetDb()
        {
            if (db == null)
                return;
            var dbname = db.Host;
            db.Dispose();
            db = DbUtil.Create(dbname);
        }

        /// <summary>
        /// This constructor creates an instance of the class named classname, and is called with pe.instance.Run().
        /// It supports the old style of MorningBatch and RegisterEvent.
        /// </summary>
        public PythonEvents(string dbname, string classname, string script)
            : this(dbname)
        {
            var engine = Python.CreateEngine();
            var sc = engine.CreateScriptSourceFromString(script);

            var code = sc.Compile();
            var scope = engine.CreateScope();
            scope.SetVariable("model", this);
            code.Execute(scope);
            db.SubmitChanges();

            dynamic Event = scope.GetVariable(classname);

            instance = Event();
        }

        public PythonEvents(string dbname)
        {
            dictionary = new Dictionary<string, object>();
            Data = new DynamicData(dictionary);
            db = DbUtil.Create(dbname);
        }
        public PythonEvents(string dbname, Dictionary<string, object> dict)
        {
            dictionary = dict;
            Data = new DynamicData(dictionary);
            db = DbUtil.Create(dbname);
        }

        // set this in the python code for javascript on the output page
        public string Script { get; set; }

        // set this in the python code for output page
        public string Header { get; set; }

        // this is set automatically from the print statements for the output page
        public string Output { get; set; }

        public string Form { get; set; }

        public string HttpMethod { get; set; }

        public string UserName => Util.UserName;

        public string RunScript(string script)
        {
            try
            {
                Output = ExecutePython(script, this);
            }
            catch (Exception ex)
            {
                Output = ex.Message;
            }
            return Output;
        }

        public static string RunScript(string dbname, string script)
        {
            return ExecutePython(script, new PythonEvents(dbname));
        }
        public static string RunScript(string dbname, string script, DateTime time)
        {
            var pe = new PythonEvents(dbname) { ScheduledTime = time.ToString("HHmm") };
            return ExecutePython(script, pe);
        }

        public string CallScript(string scriptname)
        {
            var script = db.ContentOfTypePythonScript(scriptname);
            return ExecutePython(script, new PythonEvents(db.Host, dictionary));
        }

        public void CreateTask(int forPeopleId, Person p, string description)
        {
            var t = p.AddTaskAbout(db, forPeopleId, description);
            db.SubmitChanges();
            db.Email(db.Setting("AdminMail", "support@touchpointsoftware.com"), db.LoadPersonById(forPeopleId),
                "TASK: " + description,
                Task.TaskLink(db, description, t.Id) + "<br/>" + p.Name);
        }

        public void JoinOrg(int orgId, Person p)
        {
            OrganizationMember.InsertOrgMembers(db, orgId, p.PeopleId, 220, DateTime.Now, null, false);
        }

        public void UpdateField(Person p, string field, object value)
        {
            p.UpdateValue(field, value);
        }

        public void EmailReminders(object orgId)
        {
            var oid = orgId.ToInt();
            var org = db.LoadOrganizationById(oid);
            var m = new API.APIOrganization(db);
            Util.IsInRoleEmailTest = TestEmail;
            if (org.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes)
                m.SendVolunteerReminders(oid, false);
            else
                m.SendEventReminders(oid);
        }

        public int DayOfWeek => DateTime.Today.DayOfWeek.ToInt();
        public string ScheduledTime { get; private set; }
        public DateTime DateTime => DateTime.Now;
        public bool DictionaryIsNotAvailable => false;
        public dynamic Data { get; }

        public string Dictionary(string s)
        {
            if (dictionary != null && dictionary.ContainsKey(s))
                return dictionary[s].ToString();
            return "";
        }

        public bool DataHas(string key)
        {
            return dictionary.ContainsKey(key);
        }
        public void DictionaryAdd(string key, string value)
        {
            dictionary.Add(key, value);
        }

        public DateTime DateAddDays(object dt, int days)
        {
            var dt2 = dt.ToDate();
            if (dt2 == null)
                throw new Exception("bad date: " + dt);
            return dt2.Value.AddDays(days);
        }

        public int WeekNumber(object dt)
        {
            var dt2 = dt.ToDate();
            if (dt2 == null)
                throw new Exception("bad date: " + dt);
            return dt2.Value.GetWeekNumber();
        }

        public DateTime SundayForDate(object dt)
        {
            var dt2 = dt.ToDate();
            if (dt2 == null)
                throw new Exception("bad date: " + dt);
            return dt2.Value.Sunday();
        }

        public DateTime SundayForWeek(int year, int week)
        {
            return Util.SundayForWeek(year, week);
        }

        public DateTime MostRecentAttendedSunday(int progid)
        {
            var q = from m in db.Meetings
                    where m.MeetingDate.Value.Date.DayOfWeek == 0
                    where m.MaxCount > 0
                    where
                        progid == 0 || m.Organization.DivOrgs.Any(dd => dd.Division.ProgDivs.Any(pp => pp.ProgId == progid))
                    where m.MeetingDate < Util.Now
                    orderby m.MeetingDate descending
                    select m.MeetingDate.Value.Date;
            var dt = q.FirstOrDefault();
            if (dt == DateTime.MinValue) //Sunday Date equal/before today
                dt = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            return dt;
        }

        public bool FromMorningBatch { get; set; }
        public bool TestEmail { get; set; }
        public bool Transactional { get; set; }
        public int CurrentOrgId { get; set; }

        public bool SmtpDebug
        {
            set { Util.SmtpDebug = value; }
        }

        public void Email2(Guid qid, int queuedBy, string fromAddr, string fromName, string subject, string body)
        {
            var q = db.PeopleQuery(qid);
            Email2(q, queuedBy, fromAddr, fromName, subject, body);
        }

        private void Email2(IQueryable<Person> q, int queuedBy, string fromAddr, string fromName, string subject,
            string body)
        {
            //db.Log($"Email2 {subject}");
            var from = new MailAddress(fromAddr, fromName);
            q = from p in q
                where p.EmailAddress != null
                where p.EmailAddress != ""
                where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                select p;
            var tag = db.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);

            Util.IsInRoleEmailTest = TestEmail;
            var queueremail = db.People.Where(pp => pp.PeopleId == queuedBy).Select(pp => pp.EmailAddress).Single();
            Util.UserEmail = queueremail;
            db.SetCurrentOrgId(CurrentOrgId);

            var emailqueue = db.CreateQueue(queuedBy, from, subject, body, null, tag.Id, false);
            emailqueue.Transactional = Transactional;
            db.SendPeopleEmail(emailqueue.Id);
            //db.Log($"Email2 (queued) {subject}");
        }

        public void EmailContent2(Guid qid, int queuedBy, string fromAddr, string fromName, string contentName)
        {
            var c = db.ContentOfTypeHtml(contentName);
            if (c == null)
                return;
            Email2(qid, queuedBy, fromAddr, fromName, c.Title, c.Body);
        }

        public void EmailContent2(Guid qid, int queuedBy, string fromAddr, string fromName, string subject,
            string contentName)
        {
            var c = db.ContentOfTypeHtml(contentName);
            if (c == null)
                return;
            Email2(qid, queuedBy, fromAddr, fromAddr, subject, c.Body);
        }

        public void Email(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string body)
        {
            var q = db.PeopleQuery2(savedQuery);
            if (q == null)
                return;
            Email2(q, queuedBy, fromAddr, fromName, subject, body);
        }

        public void EmailContent(object savedQuery, int queuedBy, string fromAddr, string fromName, string contentName)
        {
            var c = db.ContentOfTypeHtml(contentName);
            if (c == null)
                return;
            EmailContent(savedQuery, queuedBy, fromAddr, fromName, c.Title, contentName);
        }

        public void EmailContent(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string contentName)
        {
            EmailContent2(savedQuery, queuedBy, fromAddr, fromName, subject, contentName);
        }
        public void EmailContent2(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject,
            string contentName)
        {
            var c = db.ContentOfTypeHtml(contentName);
            if (c == null)
                return;
            var q = db.PeopleQuery2(savedQuery);
            Email2(q, queuedBy, fromAddr, fromName, subject, c.Body);
        }

        public Guid OrgMembersQuery(int progid, int divid, int orgid, string memberTypes)
        {
            var c = db.ScratchPadCondition();
            c.Reset();
            var mtlist = memberTypes.Split(',');
            var mts = string.Join(";", from mt in db.MemberTypes
                                       where mtlist.Contains(mt.Description)
                                       select $"{mt.Id},{mt.Code}");
            var clause = c.AddNewClause(QueryType.MemberTypeCodes, CompareType.OneOf, mts);
            clause.Program = progid.ToString();
            clause.Division = divid.ToString();
            clause.Organization = orgid.ToString();
            c.Save(db);
            return c.Id;
        }

        public List<int> OrganizationIds(int progid, int divid)
        {
            var q = from o in db.Organizations
                    where progid == 0 || o.DivOrgs.Any(dd => dd.Division.ProgDivs.Any(pp => pp.ProgId == progid))
                    where divid == 0 || o.DivOrgs.Select(dd => dd.DivId).Contains(divid)
                    select o.OrganizationId;
            return q.ToList();
        }

        public List<int> PeopleIds(object savedQuery)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            return list;
        }

        public void AddExtraValueCode(object savedQuery, string name, string text)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraValue(db, pid, name, text);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueText(object savedQuery, string name, string text)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraData(db, pid, name, text);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueDate(object savedQuery, string name, object dt)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            var dt2 = dt.ToDate();
            foreach (var pid in list)
            {
                Person.AddEditExtraDate(db, pid, name, dt2);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueInt(object savedQuery, string name, int n)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraInt(db, pid, name, n);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueBool(object savedQuery, string name, bool b)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraBool(db, pid, name, b);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void UpdateNamedField(object savedQuery, string field, object value)
        {
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue(field, value);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateCampus(object savedQuery, string campus)
        {
            var id = db.FetchOrCreateCampusId(campus);
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("CampusId", id);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateMemberStatus(object savedQuery, string status)
        {
            var id = Person.FetchOrCreateMemberStatus(db, status);
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("MemberStatusId", id);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateNewMemberClassStatus(object savedQuery, string status)
        {
            var id = Person.FetchOrCreateNewMemberClassStatus(db, status);
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("NewMemberClassStatusId", id);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateNewMemberClassDate(object savedQuery, object dt)
        {
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("NewMemberClassDate", dt);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateNewMemberClassDateIfNullForLastAttended(object savedQuery, object orgId)
        {
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                // skip any who already have their new member class date set
                if (p.NewMemberClassDate.HasValue)
                    continue;

                // get the most recent attend date
                var lastAttend = p.Attends
                    .Where(x => x.OrganizationId == orgId.ToInt() && x.AttendanceFlag)
                    .OrderByDescending(x => x.MeetingDate)
                    .FirstOrDefault();

                if (lastAttend != null)
                {
                    p.UpdateValue("NewMemberClassDate", lastAttend.MeetingDate);
                    p.LogChanges(db);
                    db.SubmitChanges();
                }
            }
        }

        public void AddMembersToOrg(object savedQuery, object orgId)
        {
            var q = db.PeopleQuery2(savedQuery);
            var dt = DateTime.Now;
            foreach (var p in q)
            {
                OrganizationMember.InsertOrgMembers(db, orgId.ToInt(), p.PeopleId, MemberTypeCode.Member, dt, null, false);
                db.SubmitChanges();
            }
        }

        public bool InOrg(object pid, object orgId)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            return om != null;
        }
        public void AddMemberToOrg(object pid, object orgId)
        {
            AddMembersToOrg("peopleid=" + pid.ToInt(), orgId.ToInt());
        }
        public bool InSubGroup(object pid, object orgId, string group)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            if (om == null)
                return false;

            return om.IsInGroup(group);
        }
        public void AddSubGroup(object pid, object orgId, string group)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            if (om == null)
                throw new Exception($"no orgmember {pid}:");
            om.AddToGroup(db, group);
        }
        public void RemoveSubGroup(object pid, object orgId, string group)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            if (om == null)
                throw new Exception($"no orgmember {pid}:");
            om.RemoveFromGroup(db, group);
        }

        public DateTime ParseDate(string dt)
        {
            var d = dt.ToDate();
            return d ?? DateTime.MinValue;
        }

        public string ContentForDate(string contentName, object date)
        {
            var dtwanted = date.ToDate();
            if (!dtwanted.HasValue)
                return "no date";
            dtwanted = dtwanted.Value.Date;
            var c = db.ContentOfTypeHtml(contentName);
            var a = Regex.Split(c.Body, @"<h1>\s*(?<dt>\d{1,2}(?:/|-)\d{1,2}(?:/|-)\d{2,4})=+\s*</h1>", RegexOptions.ExplicitCapture);
            var i = 0;
            for (; i < a.Length; i++)
            {
                if (a[i].Length < 6 || a[i].Length > 10)
                    continue;
                var dt = a[i].ToDate();
                if (dt.HasValue && dt == dtwanted)
                    return a[i + 1];
            }
            return "cannot find email content";
        }

        public string HtmlContent(string name)
        {
            var c = db.ContentOfTypeHtml(name);
            return c.Body;
        }
        public string Content(string name)
        {
            var c = db.Content(name);
            return c.Body;
        }
        public string Replace(string text, string pattern, string replacement)
        {
            return Regex.Replace(text, pattern, replacement);
        }

        public int ExtraValueInt(object pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid.ToInt(), name);
            if (ev != null)
                return ev.IntValue ?? 0;
            return 0;
        }

        public string ExtraValueText(object pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid.ToInt(), name);
            if (ev != null)
                return ev.Data ?? "";
            return "";
        }

        public DateTime ExtraValueDate(object pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid.ToInt(), name);
            if (ev != null)
                return ev.DateValue ?? DateTime.MinValue;
            return DateTime.MinValue;
        }

        public string ExtraValue(object pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid.ToInt(), name);
            if (ev != null)
                return ev.StrValue ?? "";
            return "";
        }

        public string ExtraValueCode(object pid, string name)
        {
            return ExtraValue(pid, name);
        }

        public bool ExtraValueBit(object pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid.ToInt(), name);
            if (ev != null)
                return ev.BitValue ?? false;
            return false;
        }

        public APIPerson.Person GetPerson(object pid)
        {
            var api = new APIPerson(db);
            var p = api.GetPersonData(pid.ToInt());
            return p;
        }
        public APIPerson.Person GetSpouse(object pid)
        {
            var p1 = db.LoadPersonById(pid.ToInt());
            if (p1 == null)
                return null;
            var api = new APIPerson(db);
            var p = api.GetPersonData(p1.SpouseId ?? 0);
            return p;
        }
        public APIOrganization.Organization GetOrganization(object orgId)
        {
            var api = new APIOrganization(db);
            return api.GetOrganization(orgId.ToInt());
        }

        /// <summary>
        /// EmailReport is designed to be very similar to EmailContent,
        /// except that the body of the email is generated by a python script
        /// instead of being pulled from an static file.
        /// The code for the Python Engine was copied from the VitalStats function in QueryFunctions.cs
        /// </summary>
        public void EmailReport(object savedquery, int queuedBy, string fromaddr, string fromname, string subject, string report)
        {
            var from = new MailAddress(fromaddr, fromname);
            var q = db.PeopleQuery2(savedquery);

            q = from p in q
                where p.EmailAddress != null
                where p.EmailAddress != ""
                where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                select p;
            var tag = db.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);

            var script = db.ContentOfTypePythonScript(report);
            if (script == null)
                return;

            var emailbody = RunScript(script);
            var emailqueue = db.CreateQueue(queuedBy, from, subject, emailbody, null, tag.Id, false);

            emailqueue.Transactional = Transactional;
            Util.IsInRoleEmailTest = TestEmail;

            db.SendPeopleEmail(emailqueue.Id);
        }


        /// <summary>
        /// Overloaded version of EmailReport adds variables in the function call for QueryName and QueryDescription. The original version of EmailReport
        /// required you to embed the query name in the Python Script.  This version of the function permits you to have a generic Python script
        /// and then call it multiple times with a different query and description each time.
        /// </summary>
        public void EmailReport(string savedquery, int queuedBy, string fromaddr, string fromname, string subject, string report, string queryname, string querydescription)
        {
            Data.QueryName = queryname;
            Data.QueryDescription = querydescription;
            EmailReport(savedquery, queuedBy, fromaddr, fromname, subject, report);
        }

        public string FmtPhone(string s, string prefix)
        {
            return s.FmtFone(prefix);
        }

        public string UploadExcelFromSqlToFtp(string sqlscript, string username, string password, string targetpath, string filename)
        {
            var script = db.Content(sqlscript, "");
            if (!script.HasValue())
                throw new Exception("no sql script found");
            var bytes = db.Connection.ExecuteReader(sqlscript).ToExcelBytes(filename);
            var url = Path.Combine(targetpath, filename);
            using (var webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential(username, password);
                webClient.UploadData(url, bytes);
            }
            return url;
        }
        public void UploadExcelFromSqlToDropBox(string savedQuery, string sqlscript, string targetpath, string filename)
        {
            var accesstoken = DbUtil.Db.Setting("DropBoxAccessToken", ConfigurationManager.AppSettings["DropBoxAccessToken"]);
            var script = db.Content(sqlscript, "");
            if (!script.HasValue())
                throw new Exception("no sql script found");

            var p = new DynamicParameters();
            foreach (var kv in dictionary)
                p.Add("@" + kv.Key, kv.Value);
            if (script.Contains("@qtagid"))
            {
                int? qtagid = null;
                if (savedQuery.HasValue())
                {
                    var q = db.PeopleQuery2(savedQuery);
                    var tag = db.PopulateSpecialTag(q, DbUtil.TagTypeId_Query);
                    qtagid = tag.Id;
                }
                p.Add("@qtagid", qtagid);
            }
            var bytes = db.Connection.ExecuteReader(script, p).ToExcelBytes(filename);

            var wc = new WebClient();
            wc.Headers.Add($"Authorization: Bearer {accesstoken}");
            wc.Headers.Add("Content-Type: application/octet-stream");
            wc.Headers.Add($@"Dropbox-API-Arg: {{""path"":""{targetpath}/{filename}"",""mode"":""overwrite""}}");
            wc.UploadData("https://content.dropboxapi.com/2-beta-2/files/upload", bytes);
        }
        public void UploadExcelFromSqlToDropBox(string sqlscript, string targetpath, string filename)
        {
            UploadExcelFromSqlToDropBox(null, sqlscript, targetpath, filename);
        }
        public string UploadExcelFromSqlToRackspace(string sqlscript, string filename)
        {
            return "";
        }
        private static string ExecutePython(string scriptContent, PythonEvents model)
        {
            var engine = Python.CreateEngine();

            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            {
                engine.Runtime.IO.SetOutput(ms, sw);
                engine.Runtime.IO.SetErrorOutput(ms, sw);

                try
                {
                    var sc = engine.CreateScriptSourceFromString(scriptContent);
                    var code = sc.Compile();

                    var scope = engine.CreateScope();
                    scope.SetVariable("model", model);

                    var qf = new QueryFunctions(model.db, model.dictionary);
                    scope.SetVariable("q", qf);
                    code.Execute(scope);

                    model.db.SubmitChanges();

                    ms.Position = 0;

                    using (var sr = new StreamReader(ms))
                    {
                        var s = sr.ReadToEnd();
                        return s;
                    }
                }
                catch (Exception ex)
                {
                    var err = engine.GetService<ExceptionOperations>().FormatException(ex);
                    throw new Exception(err);
                }
            }
        }

        public string RenderTemplate(string source)
        {
            return RenderTemplate(source, Data);
        }
        public string RenderTemplate(string source, object data)
        {
            RegisterHelpers(db);
            var template = Handlebars.Compile(source);
            var result = template(data);
            return result;
        }
        public static void RegisterHelpers(CMSDataContext db)
        {
            Handlebars.RegisterHelper("BottomBorder", (writer, context, args) => { writer.Write(CssStyle.BottomBorder); });
            Handlebars.RegisterHelper("AlignTop", (writer, context, args) => { writer.Write(CssStyle.AlignTop); });
            Handlebars.RegisterHelper("AlignRight", (writer, context, args) => { writer.Write(CssStyle.AlignRight); });
            Handlebars.RegisterHelper("DataLabelStyle", (writer, context, args) => { writer.Write(CssStyle.DataLabelStyle); });
            Handlebars.RegisterHelper("LabelStyle", (writer, context, args) => { writer.Write(CssStyle.LabelStyle); });
            Handlebars.RegisterHelper("DataStyle", (writer, context, args) => { writer.Write(CssStyle.DataStyle); });
            Handlebars.RegisterHelper("ServerLink", (writer, context, args) => { writer.Write(db.ServerLink().TrimEnd('/')); });
            Handlebars.RegisterHelper("FmtDate", (writer, context, args) => { writer.Write(args[0].ToDate().FormatDate()); });
            Handlebars.RegisterHelper("IfEqual", (writer, options, context, args) =>
            {
                if (args[0] == args[1])
                    options.Template(writer, (object)context);
                else
                    options.Inverse(writer, (object)context);
            });
            Handlebars.RegisterHelper("GetToken", (writer, context, args) =>
            {
                var s = args[0].ToString();
                var n = args[1].ToInt();
                var a = s.SplitStr(" ", 2);
                writer.Write(a[n]);
            });
        }
    }
}
