using System;
using System.Collections.Generic;
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
        public bool? testing { get; set; }
        public string URL { get; set; }
        private int? _masterorgid;

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

        private int? _Orgid;

        public int? orgid
        {
            get { return _Orgid; }
            set
            {
                _Orgid = value;
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
        public string username { get; set; }
        public bool nologin { get; set; }
        public decimal? donation { get; set; }
        public int? donor { get; set; }
        public int? UserPeopleId { get; set; }
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
            foreach (
                PropertyInfo pi in typeof (OnlineRegModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                          .Where(vv => vv.CanRead && vv.CanWrite))
            {
                Debug.WriteLine(pi.Name);
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
            else if (org == null)
                return;
            else
                list[_Orgid.Value] = new Settings(org.RegSetting, DbUtil.Db, _Orgid.Value);
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
                        var pe = new PythonEvents(DbUtil.Db, "RegisterEvent", script.Body);
                        HttpContext.Current.Items.Add("PythonEvents", pe);
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
                masterorgid = orgid;
                _Orgid = null;
                _org = null;
            }
        }

        private CmsData.Organization _org;

        public CmsData.Organization org
        {
            get
            {
                if (_org == null && orgid.HasValue)
                    if (orgid == Util.CreateAccountCode)
                        _org = CreateAccountOrg();
                    else
                        _org = DbUtil.Db.LoadOrganizationById(orgid.Value);
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
                        where m.Organization.OrganizationId == orgid
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
                            orgid = orgid,
                            masterorgid = masterorgid,
                            LoggedIn = false,
#if DEBUG
                            first = "David",
                            last = "Roll",
                            dob = "5/30/52",
                            email = "david@bvcms.com",
                            phone = "",
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
    }
}
