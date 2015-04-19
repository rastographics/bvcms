using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsData.Registration;
using CmsWeb.Controllers;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    [Serializable]
    public partial class OnlineRegModel : IXmlSerializable
    {

        public bool? testing { get; set; }
        public string FromMobile { get; set; }
        public string URL { get; set; }
        private int? _masterorgid;

        public bool DisplaySpecialFunds
        {
            get { return OnlineGiving() || ManageGiving(); }
        }

        public int? masterorgid
        {
            get { return _masterorgid; }
            set
            {
                _masterorgid = value;
                if (value > 0)
                    ParseSettings();
            }
        }

        private int? _orgid;
        public int? Orgid
        {
            get { return _orgid; }
            set
            {
                _orgid = value;
                if (value > 0)
                {
                    CheckMasterOrg();
                    ParseSettings();
                }
            }
        }

        private int? _tranId;
        public int? TranId
        {
            get { return _tranId; }
            set
            {
                _tranId = value;
                _transaction = null;
            }
        }

        public int? classid { get; set; }
        [DisplayName("Username")]
        public string username { get; set; }
        public bool nologin { get; set; }
        public decimal? donation { get; set; }
        public int? donor { get; set; }
        public int? UserPeopleId { get; set; }
        public bool prospect { get; set; }

        private string _registerTag;
        public string registertag
        {
            get { return _registerTag; }
            set { _registerTag = value; }
        }

        private List<OnlineRegPersonModel> _list = new List<OnlineRegPersonModel>();

        public List<OnlineRegPersonModel> List
        {
            get { return _list; }
            set { _list = value; }
        }

        [XmlIgnore]
        [DisplayName("Password")]
        public string password { get; set; }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            var s = reader.ReadOuterXml();
            var x = XDocument.Parse(s);
            if (x.Root == null) return;

            foreach (var e in x.Root.Elements())
            {
                var name = e.Name.ToString();
                switch (name)
                {
                    case "List":
                        foreach (var ee in e.Elements())
                            _list.Add(Util.DeSerialize<OnlineRegPersonModel>(ee.ToString()));
                        break;
                    case "History":
                        foreach (var ee in e.Elements())
                            _history.Add(ee.Value);
                        break;
                    default:
                        Util.SetPropertyFromText(this, name, e.Value);
                        break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            var w = new APIWriter(writer);
            writer.WriteComment(DateTime.Now.ToString());
            foreach (var pi in typeof(OnlineRegModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                          .Where(vv => vv.CanRead && vv.CanWrite))
            {
                switch (pi.Name)
                {
                    case "List":
                        w.Start("List");
                        foreach (var i in _list)
                            Util.Serialize(i, writer);
                        w.End();
                        break;
                    case "History":
                        w.Start("History");
                        foreach (var i in History)
                            w.Add("item", i);
                        w.End();
                        break;
                    case "password":
                        break;
                    case "testing":
                        if (testing == true)
                            w.Add(pi.Name, testing);
                        break;
                    case "FromMobile":
                        if (FromMobile.HasValue())
                            w.Add(pi.Name, FromMobile);
                        else if (MobileAppMenuController.Source.HasValue())
                            w.Add(pi.Name, MobileAppMenuController.Source);
                        break;
                    case "prospect":
                        if (prospect)
                            w.Add(pi.Name, prospect);
                        break;
                    default:
                        w.Add(pi.Name, pi.GetValue(this, null));
                        break;
                }
            }
        }

        public OnlineRegModel()
        {
            HttpContext.Current.Items["OnlineRegModel"] = this;
        }

        public OnlineRegModel(HttpRequestBase req, int? id, bool? testing, string email, bool? login, string source)
            : this()
        {
#if DEBUG
            DebugCleanUp();
#endif
            Orgid = id;
            if (req.Url != null)
                URL = req.Url.OriginalString;

            if (DbUtil.Db.Roles.Any(rr => rr.RoleName == "disabled"))
                throw new Exception("Site is disabled for maintenance, check back later");
            if (!id.HasValue)
                throw new Exception("no organization");

            MobileAppMenuController.Source = source;

            if (org == null && masterorg == null)
                throw new Exception("invalid registration");

            if (masterorg != null)
            {
                if (!UserSelectClasses(masterorg).Any())
                    throw new Exception("no classes available on this org");
            }
            else if (org != null)
            {
                if ((org.RegistrationTypeId ?? 0) == RegistrationTypeCode.None)
                    throw new Exception("no registration allowed on this org");
            }
            this.testing = testing == true || DbUtil.Db.Setting("OnlineRegTesting", Util.IsDebug() ? "true" : "false").ToBool();

            if (Util.ValidEmail(email) || login != true)
                nologin = true;

            if (nologin)
                CreateList();
            else
                List = new List<OnlineRegPersonModel>();

            if (Util.ValidEmail(email))
                List[0].EmailAddress = email;

            HistoryAdd("index");
        }

        public bool ShowFindInstructions;
        public bool ShowLoginInstructions;
        public bool ShowOtherInstructions;

        public int? GoerSupporterId { get; set; }
        public int? GoerId { get; set; }
        public bool SupportMissionTrip { get { return GoerSupporterId.HasValue || GoerId.HasValue; } }

        public Person GetGoer()
        {
            return DbUtil.Db.LoadPersonById(GoerId ?? 0);
        }

        public void ParseSettings()
        {
            //            if (HttpContext.Current.Items.Contains("RegSettings"))
            //                return;
            var list = new Dictionary<int, Settings>();
            if (masterorgid.HasValue)
            {
                var q = from o in UserSelectClasses(masterorg)
                        select new { o.OrganizationId, o.RegSetting };
                foreach (var i in q)
                    list[i.OrganizationId] = new Settings(i.RegSetting, DbUtil.Db, i.OrganizationId);
                list[masterorg.OrganizationId] = new Settings(masterorg.RegSetting, DbUtil.Db, masterorg.OrganizationId);
            }
            else if (_orgid == null)
                return;
            else if (org != null)
                list[_orgid.Value] = new Settings(org.RegSetting, DbUtil.Db, _orgid.Value);
            //            if (HttpContext.Current.Items.Contains("RegSettings"))
            //                return;
            HttpContext.Current.Items["RegSettings"] = list;

            if (org == null || !org.AddToSmallGroupScript.HasValue()) return;

            var script = DbUtil.Db.Content(org.AddToSmallGroupScript);
            if (script == null || !script.Body.HasValue()) return;

            try
            {
                var pe = new PythonEvents(Util.Host, "RegisterEvent", script.Body);
                HttpContext.Current.Items["PythonEvents"] = pe;
            }
            catch (Exception ex)
            {
                org.AddToExtraData("Python.errors", ex.Message);
                throw;
            }
        }

        public static Settings ParseSetting(string regSetting, int orgId)
        {
            return new Settings(regSetting, DbUtil.Db, orgId);
        }

        private Organization _masterOrg;

        public Organization masterorg
        {
            get
            {
                if (_masterOrg != null)
                    return _masterOrg;
                if (masterorgid.HasValue)
                    _masterOrg = DbUtil.Db.LoadOrganizationById(masterorgid.Value);
                return _masterOrg;
            }
        }

        public void CheckMasterOrg()
        {
            if (org != null && masterorgid == null &&
                (org.RegistrationTypeId == RegistrationTypeCode.UserSelectsOrganization2
                 || org.RegistrationTypeId == RegistrationTypeCode.ComputeOrganizationByAge2
                 || org.RegistrationTypeId == RegistrationTypeCode.ManageSubscriptions2))
            {
                _masterOrg = org;
                masterorgid = Orgid;
                _orgid = null;
                _org = null;
            }
        }

        private Organization _org;
        public Organization org
        {
            get
            {
                if (_org == null && Orgid.HasValue)
                {
                    _org = Orgid == Util.CreateAccountCode
                        ? CreateAccountOrg()
                        : DbUtil.Db.LoadOrganizationById(Orgid.Value);
                }
                return _org;
            }
        }

        private Transaction _transaction;
        public Transaction Transaction
        {
            get
            {
                if (_transaction == null && TranId.HasValue)
                    _transaction = DbUtil.Db.Transactions.SingleOrDefault(tt => tt.Id == TranId);
                return _transaction;
            }
        }

        private Person _user;
        public Person user
        {
            get
            {
                if (_user == null && UserPeopleId.HasValue)
                    _user = DbUtil.Db.LoadPersonById(UserPeopleId.Value);
                return _user;
            }
        }

        private Meeting _meeting;
        public Meeting meeting()
        {
            if (_meeting == null)
            {
                var q = from m in DbUtil.Db.Meetings
                        where m.Organization.OrganizationId == Orgid
                        where m.MeetingDate > Util.Now.AddHours(-12)
                        orderby m.MeetingDate
                        select m;
                _meeting = q.FirstOrDefault();
            }
            return _meeting;
        }

        public void CreateList()
        {
            List = new List<OnlineRegPersonModel>
                {
                    new OnlineRegPersonModel
                        {
                            orgid = Orgid,
                            masterorgid = masterorgid,
                            LoggedIn = false,
#if DEBUG
                            FirstName = "David",
                            LastName = "Carroll", // + DateTime.Now.Millisecond,
                            DateOfBirth = "5/30/52",
                            EmailAddress = "david@touchpointsoftware.com",
                            Phone = "",
#endif
                        }
                };
        }

        private List<string> _history = new List<string>();
        public List<string> History
        {
            get { return _history; }
            set { _history = value; }
        }

        public void HistoryAdd(string s)
        {
            History.Add("{0} {1:g} (c-ip={2})".Fmt(s, DateTime.Now, Util.GetIpAddress()));
        }
    }
}
