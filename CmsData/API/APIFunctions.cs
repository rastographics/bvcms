using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using CmsData.Codes;
using Dapper;
using IronPython.Hosting;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsData.API
{
    public partial class APIFunctions
    {
        private CMSDataContext Db;

        public APIFunctions(CMSDataContext Db)
        {
            this.Db = Db;
        }
        public static string TestAPI(string init, string script, Dictionary<string, string> namevalues)
        {
            var shell = @"
from System.Net import WebClient, NetworkCredential
from System.Collections.Specialized import NameValueCollection
from System.Text import Encoding
from System import Convert

{0}
class TestAPI(object):
	def Run(self):
{1}";
            try
            {
                var sb = new StringBuilder();
                var ss = (script ?? "return").Replace("\r\n", "\n");
                foreach (var s in ss.Split('\n'))
                    sb.AppendFormat("\t\t{0}\n", s);
                var engine = Python.CreateEngine();
                script = string.Format(shell, init, sb);
                var sc = engine.CreateScriptSourceFromString(script);
                var code = sc.Compile();
                var scope = engine.CreateScope();
                foreach (var i in namevalues)
                    scope.SetVariable(i.Key, i.Value);
                code.Execute(scope);
                dynamic TestAPI = scope.GetVariable("TestAPI");
                dynamic m = TestAPI();
                var ret = m.Run();
                return HttpUtility.HtmlEncode(ret);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string Login(Person p)
        {
            var script = Db.Content("API-LoginInfo");
            if (script == null)
            {
                script = new Content();
                script.Body = @"
from System import *
from System.Text import *

class LoginInfo(object):

	def Run(self, m, w, p):
		w.Start('Login')
		w.Attr('PeopleId', p.PeopleId)
		w.Attr('PreferredName', p.PreferredName)
		w.Attr('Last', p.LastName)
		w.Attr('EmailAddress', p.EmailAddress)
		w.Attr('IsMember', p.MemberStatusId == 10)
		for b in m.StatusFlags(p.PeopleId):
			w.Add('StatusFlag', b);
		w.End()
		return w.ToString()
";
            }
            var engine = Python.CreateEngine();
            var sc = engine.CreateScriptSourceFromString(script.Body);
            try
            {
                var code = sc.Compile();
                var scope = engine.CreateScope();
                code.Execute(scope);

                dynamic LoginInfo = scope.GetVariable("LoginInfo");
                dynamic m = LoginInfo();
                var api = new APIFunctions(Db);
                var w = new APIWriter();
                return m.Run(api, w, p);
            }
            catch (Exception ex)
            {
                return $"<login error=\"API-LoginInfo script error: {ex.Message}\" />";
            }
        }
        public int AttendCount(int orgid, int PeopleId)
        {
            var q = from a in Db.Attends
                    where a.OrganizationId == orgid
                    where a.PeopleId == PeopleId
                    where a.AttendanceFlag == true
                    select a;
            return q.Count();
        }
        public List<string> QueryBits(int PeopleId)
        {
            var q1 = (from f in Db.StatusFlags()
                      select f).ToList();
            var q2 = (from t in Db.TagPeople
                      where t.PeopleId == PeopleId
                      where t.Tag.TypeId == DbUtil.TagTypeId_StatusFlags
                      select t.Tag.Name).ToList();
            var q = from t in q2
                    join f in q1 on t equals f[0]
                    select f[1];
            var list = q.ToList();
            return list;
        }
        public List<string> StatusFlags(int PeopleId)
        {
            var q1 = (from f in Db.StatusFlags()
                      select f).ToList();
            var q2 = (from t in Db.TagPeople
                      where t.PeopleId == PeopleId
                      where t.Tag.TypeId == DbUtil.TagTypeId_StatusFlags
                      select t.Tag.Name).ToList();
            var q = from t in q2
                    join f in q1 on t equals f[0]
                    select f[1];
            var list = q.ToList();
            return list;
        }
        public DateTime LastAttendDate(int orgid, int PeopleId)
        {
            var q = from a in Db.Attends
                    where a.OrganizationId == orgid
                    where a.PeopleId == PeopleId
                    where a.AttendanceFlag == true
                    orderby a.MeetingDate descending
                    select a.MeetingDate;
            return q.FirstOrDefault();
        }
        public int IsMemberOf(int orgid, int PeopleId)
        {
            var q = from a in Db.OrganizationMembers
                    where a.OrganizationId == orgid
                    where a.PeopleId == PeopleId
                    where a.MemberTypeId != MemberTypeCode.InActive
                    select a;
            return q.Count();
        }
        public void DeleteExtraValue(int peopleid, string field)
        {
            var q = from v in Db.PeopleExtras
                    where v.Field == field
                    where v.PeopleId == peopleid
                    select v;
            Db.PeopleExtras.DeleteAllOnSubmit(q);
            Db.SubmitChanges();
        }
        public string ExtraValues(int peopleid, string fields)
        {
            try
            {
                var a = (fields ?? "").Split(',');
                var nofields = !fields.HasValue();
                var q = from v in Db.PeopleExtras
                        where nofields || a.Contains(v.Field)
                        where v.PeopleId == peopleid
                        select v;
                var w = new APIWriter();
                w.Start("ExtraValues");
                w.Attr("Id", peopleid);
                foreach (var v in q)
                    w.Add(v.Field, v.StrValue ?? v.Data ?? v.DateValue.FormatDate() ?? v.IntValue.ToString());
                w.End();
                return w.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string AddEditExtraValue(int peopleid, string field, string value, string type = "data")
        {
            try
            {
                var q = from v in Db.PeopleExtras
                        where v.Field == field
                        where v.PeopleId == peopleid
                        select v;
                var ev = q.SingleOrDefault();
                if (ev == null)
                {
                    ev = new PeopleExtra
                    {
                        PeopleId = peopleid,
                        Field = field,
                        TransactionTime = DateTime.Now
                    };
                    Db.PeopleExtras.InsertOnSubmit(ev);
                }
                else
                {
					// prepare for new data type
					ev.Data = null;
					ev.IntValue = null;
					ev.DateValue = null;
					ev.StrValue = null;
                }
                switch (type)
                {
                    case "code":
                        ev.StrValue = value;
                        break;
                    case "data":
                        ev.Data = value;
                        break;
                    case "date":
                        ev.DateValue = value.ToDate();
                        break;
                    case "int":
                        ev.IntValue = value.ToInt2();
                        break;
                }
                Db.SubmitChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string FamilyMembers(int familyid)
        {
            try
            {
                var q = from p in Db.People
                        where p.FamilyId == familyid
                        select p;
                var w = new APIWriter();
                w.Start("Family");
                w.Attr("Id", familyid);
                foreach (var m in q)
                {
                    w.Start("Member");
                    w.Add("peopleid", m.PeopleId);
                    w.Add("first", m.FirstName);
                    w.Add("last", m.LastName);
                    w.Add("goesby", m.NickName);
                    w.Add("birthday", m.BDate);
                    w.Add("position", m.PositionInFamilyId);
                    w.Add("marital", m.MaritalStatusId);
                    w.Add("suffix", m.SuffixCode);
                    w.Add("title", m.TitleCode);
                    w.End();
                }
                w.End();
                return w.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [Serializable()]
        public class AccessUsers
        {
            [XmlElement("Person")]
            public AccessUserInfo[] People { get; set; }
        }
        [Serializable()]
        public class AccessUserInfo
        {
            [XmlAttribute("peopleid")]
            public int peopleid { get; set; }
            public string first { get; set; }
            public string goesby { get; set; }
            public string last { get; set; }
            public string cphone { get; set; }
            public string hphone { get; set; }
            public string wphone { get; set; }
            public string dob { get; set; }
            public int? bmon { get; set; }
            public int? bday { get; set; }
            public int? byear { get; set; }
            public int? married { get; set; }
            public int? gender { get; set; }
            public string company { get; set; }
            public string email { get; set; }
            public string email2 { get; set; }
            public string username { get; set; }
            public string lastactive { get; set; }
            public string roles { get; set; }
			public List<string> StatusFlags { get; set; }
        }
        public IEnumerable<AccessUserInfo> AccessUsersData(bool includeNoAccess = false)
        {
            var q = from u in Db.Users
                    where includeNoAccess || u.UserRoles.Any(rr => rr.Role.RoleName == "Access")
                    where u.EmailAddress.Length > 0
                    select new
                    {
                        u.PeopleId,
                        roles = u.UserRoles.Select(uu => uu.Role.RoleName),
                        lastactive = Db.LastActive(u.UserId),
                        first = u.Person.FirstName,
                        goesby = u.Person.NickName,
                        last = u.Person.LastName,
                        married = u.Person.MaritalStatusId,
                        gender = u.Person.GenderId,
                        cphone = u.Person.CellPhone,
                        hphone = u.Person.HomePhone,
                        wphone = u.Person.WorkPhone,
                        dob = u.Person.DOB,
                        bday = u.Person.BirthDay,
                        bmon = u.Person.BirthMonth,
                        byear = u.Person.BirthYr,
                        company = u.Person.EmployerOther,
                        email = u.EmailAddress,
                        email2 = u.Person.EmailAddress2,
                        username = u.Username,
                    };
            var list = q.ToList();

            var q2 = from i in list
                     group i by i.PeopleId into g
                     let i1 = g.OrderByDescending(i => i.lastactive).First()
                     select new AccessUserInfo
                     {
                         peopleid = i1.PeopleId.Value,
                         first = i1.first,
                         goesby = i1.goesby,
                         last = i1.last,
                         married = i1.married,
                         gender = i1.gender,
                         cphone = i1.cphone.GetDigits(),
                         hphone = i1.hphone.GetDigits(),
                         wphone = i1.wphone,
                         dob = i1.dob,
                         bday = i1.bday,
                         bmon = i1.bmon,
                         byear = i1.byear,
                         company = i1.company,
                         email = i1.email,
                         email2 = i1.email2,
                         username = i1.username,
                         lastactive = i1.lastactive.ToString2("s"),
                         roles = string.Join(",", i1.roles),
#if DEBUG
#else
						 StatusFlags = (from qb in StatusFlags(i1.PeopleId.Value) select qb).ToList()
#endif
                     };
            return q2;
        }
        public string AccessUsersXml(bool includeNoAccess = false)
        {
            var xs = new XmlSerializer(typeof(AccessUsers));
            var sw = new StringWriter();
            var a = new AccessUsers { People = AccessUsersData(includeNoAccess).ToArray() };
            xs.Serialize(sw, a);
            return sw.ToString();
        }
        public string SqlScriptXml(string sqlscript, string p1 = null, Dictionary<string, string> d = null)
        {
            var rd = GetReader(sqlscript, p1, d);
            return WriteXmlFromReader(rd);
        }
        public string SqlScriptJson(string sqlscript, string p1 = null, Dictionary<string, string> d = null)
        {
            var rd = GetReader(sqlscript, p1, d);
            return WriteJsonFromReader(rd);
        }
        private IDataReader GetReader(string sqlscript, string p1, Dictionary<string, string> d)
        {
            var script = Db.Content(sqlscript, "");
            if (!script.HasValue())
                throw new Exception("no sql script found");

            var p = new DynamicParameters();
            p.Add("@p1", p1 ?? "");
            if (d != null)
                foreach (var kv in d)
                    p.Add("@" + kv.Key, kv.Value);

            var rd = Db.Connection.ExecuteReader(script, p);
            return rd;
        }
        private static string WriteXmlFromReader(IDataReader rd)
        {
            var w = new APIWriter();
            w.Start("SqlScriptXml");
            while (rd.Read())
            {
                w.Start("Row");
                for (var i = 0; i < rd.FieldCount; i++)
                {
                    var name = rd.GetName(i);
                    var value = rd.GetValue(i);
                    w.Add(name, value);
                }
                w.End();
            }
            w.End();
            return w.ToString();
        }
        private string WriteJsonFromReader(IDataReader rd)
        {
            var sb = new StringBuilder();
            var w = new JsonTextWriter(new StringWriter(sb));
            w.Formatting = Formatting.Indented;
            w.WriteStartArray();
            while (rd.Read())
            {
                w.WriteStartObject();
                for (var i = 0; i < rd.FieldCount; i++)
                {
                    var name = rd.GetName(i);
                    var value = rd.GetValue(i);
                    w.WritePropertyName(name);
                    w.WriteValue(value);
                }
                w.WriteEndObject();
            }
            w.WriteEndArray();
            return sb.ToString();
        }
    }
}
