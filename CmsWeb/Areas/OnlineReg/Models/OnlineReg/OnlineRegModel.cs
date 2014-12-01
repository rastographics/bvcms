using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Data.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData;
using CmsData.API;
using CmsData.Registration;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Models
{
    [Serializable]
    public partial class OnlineRegModel : IXmlSerializable
    {
        private const string MOBILE_APP_RETURN_URL = "bvcmsapp://";

        public bool InMobileAppMode
        {
            get { return (bool) HttpContext.Current.Session["source"]; }
            set { HttpContext.Current.Session["source"] = value; }
        }

        public string MobileAppReturnUrl
        {
            get { return MOBILE_APP_RETURN_URL; }
        }

        public bool? testing { get; set; }
        public string URL { get; set; }
        private int? _masterorgid;

        public bool DisplaySpecialFunds
        {
            get { return UseBootstrap && (OnlineGiving() || ManageGiving()); }
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

        private int? orgid;
        public int? Orgid
        {
            get { return orgid; }
            set
            {
                orgid = value;
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
                _Transaction = null;
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

        private string _Registertag;
        public string registertag
        {
            get { return _Registertag; }
            set { _Registertag = value; }
        }

        private List<OnlineRegPersonModel> list = new List<OnlineRegPersonModel>();

        public List<OnlineRegPersonModel> List
        {
            get { return list; }
            set { list = value; }
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
                            list.Add(Util.DeSerialize<OnlineRegPersonModel>(ee.ToString()));
                        break;
                    case "History":
                        foreach (var ee in e.Elements())
                            history.Add(ee.Value);
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
            foreach (var pi in typeof (OnlineRegModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                          .Where(vv => vv.CanRead && vv.CanWrite))
            {
                switch (pi.Name)
                {
                    case "List":
                        w.Start("List");
                        foreach (var i in list)
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
                        select new {o.OrganizationId, o.RegSetting};
                foreach (var i in q)
                    list[i.OrganizationId] = new Settings(i.RegSetting, DbUtil.Db, i.OrganizationId);
                list[masterorg.OrganizationId] = new Settings(masterorg.RegSetting, DbUtil.Db, masterorg.OrganizationId);
            }
            else if (orgid == null)
                return;
            else if (org != null)
                list[orgid.Value] = new Settings(org.RegSetting, DbUtil.Db, orgid.Value);
//            if (HttpContext.Current.Items.Contains("RegSettings"))
//                return;
            HttpContext.Current.Items["RegSettings"] = list;

            if (org != null && org.AddToSmallGroupScript.HasValue())
            {
                var script = DbUtil.Db.Content(org.AddToSmallGroupScript);
                if (script != null && script.Body.HasValue())
                {
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
            }
        }

        public static Settings ParseSetting(string RegSetting, int OrgId)
        {
            return new Settings(RegSetting, DbUtil.Db, OrgId);
        }

        private Organization _masterorg;

        public Organization masterorg
        {
            get
            {
                if (_masterorg != null)
                    return _masterorg;
                if (masterorgid.HasValue)
                    _masterorg = DbUtil.Db.LoadOrganizationById(masterorgid.Value);
                return _masterorg;
            }
        }

        public void CheckMasterOrg()
        {
            if (org != null && masterorgid == null &&
                (org.RegistrationTypeId == RegistrationTypeCode.UserSelectsOrganization2
                 || org.RegistrationTypeId == RegistrationTypeCode.ComputeOrganizationByAge2
                 || org.RegistrationTypeId == RegistrationTypeCode.ManageSubscriptions2))
            {
                _masterorg = org;
                masterorgid = Orgid;
                orgid = null;
                _org = null;
            }
        }

        private CmsData.Organization _org;

        public CmsData.Organization org
        {
            get
            {
                if (_org == null && Orgid.HasValue)
                    if (Orgid == Util.CreateAccountCode)
                        _org = CreateAccountOrg();
                    else
                        _org = DbUtil.Db.LoadOrganizationById(Orgid.Value);
                return _org;
            }
        }

        private Transaction _Transaction;

        public Transaction Transaction
        {
            get
            {
                if (_Transaction == null && TranId.HasValue)
                    _Transaction = DbUtil.Db.Transactions.SingleOrDefault(tt => tt.Id == TranId);
                return _Transaction;
            }
        }

        private Person _User;

        public Person user
        {
            get
            {
                if (_User == null && UserPeopleId.HasValue)
                    _User = DbUtil.Db.LoadPersonById(UserPeopleId.Value);
                return _User;
            }
        }

        private CmsData.Meeting _meeting;

        public CmsData.Meeting meeting()
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
                            EmailAddress = "david@bvcms.com",
                            Phone = "",
#endif
                        }
                };
        }

        private List<string> history = new List<string>();

        public List<string> History
        {
            get { return history; }
            set { history = value; }
        }

        public void HistoryAdd(string s)
        {
            History.Add("{0} {1:g}".Fmt(s, DateTime.Now));
        }
    }
}
