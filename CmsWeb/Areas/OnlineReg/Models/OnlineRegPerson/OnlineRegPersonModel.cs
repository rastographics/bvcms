using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Data.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData;
using CmsData.API;
using UtilityExtensions;
using System.Runtime.Serialization;
using System.Web.Mvc;
using CmsData.Codes;

namespace CmsWeb.Models
{
    [Serializable]
    public partial class OnlineRegPersonModel : IXmlSerializable
    {
        public int? orgid { get; set; }
        public int? masterorgid { get; set; }
        public int? divid { get; set; }
        public int? classid { get; set; }
        public int? PeopleId { get; set; }
        public bool? Found { get; set; }
        public bool IsNew { get; set; }
        public bool OtherOK { get; set; }
        public bool LoggedIn { get; set; }
        public bool IsValidForExisting { get; set; }
        public bool ShowAddress { get; set; }
        public bool CreatingAccount { get; set; }
        public string first { get; set; }
        public string middle { get; set; }
        public string last { get; set; }
        public string suffix { get; set; }
        //public string dob { get; set; }
        internal int? bmon, byear, bday;

        public string dob
        {
            get
            {
                return Util.FormatBirthday(byear, bmon, bday);
            }
            set
            {
                bday = null;
                bmon = null;
                byear = null;
                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                {
                    bday = dt.Day;
                    bmon = dt.Month;
                    if (Regex.IsMatch(value, @"\d+/\d+/\d+"))
                        byear = dt.Year;
                }
                else
                {
                    int n;
                    if (int.TryParse(value, out n))
                        if (n >= 1 && n <= 12)
                            bmon = n;
                        else
                            byear = n;
                }
            }
        }

        public bool DateValid()
        {
            return bmon.HasValue && byear.HasValue && bday.HasValue;
        }

        public string phone { get; set; }
        public string homephone { get; set; }
        public string address { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public int? gender { get; set; }
        public int? married { get; set; }
        public bool IsFilled { get; set; }
        public string shirtsize { get; set; }
        public string emcontact { get; set; }
        public string emphone { get; set; }
        public string insurance { get; set; }
        public string policy { get; set; }
        public string doctor { get; set; }
        public string docphone { get; set; }
        public string medical { get; set; }
        public string mname { get; set; }
        public string fname { get; set; }
        public bool memberus { get; set; }
        public bool otherchurch { get; set; }
        public bool? coaching { get; set; }
        public bool? sms { get; set; }
        public bool? tylenol { get; set; }
        public bool? advil { get; set; }
        public bool? maalox { get; set; }
        public bool? robitussin { get; set; }
        public bool? paydeposit { get; set; }
        public string request { get; set; }
        public string grade { get; set; }
        public int? ntickets { get; set; }
        public int? whatfamily { get; set; }
        public string gradeoption { get; set; }
        public bool IsFamily { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? Suggestedfee { get; set; }

        public Dictionary<int, decimal?> FundItem { get; set; }
        public List<Dictionary<string, string>> ExtraQuestion { get; set; }
        public Dictionary<string, bool?> YesNoQuestion { get; set; }
        public List<string> option { get; set; }
        public List<string> Checkbox { get; set; }
        public Dictionary<string, int?> MenuItem { get; set; }

        public void ReadXml(XmlReader reader)
        {
            var s = reader.ReadOuterXml();
            var x = XDocument.Parse(s);
            if (x.Root == null) return;

            var eqset = 0;
            foreach (var e in x.Root.Elements())
            {
                var name = e.Name.ToString();
                switch (name)
                {
                    case "FundItem":
                        if (FundItem == null)
                            FundItem = new Dictionary<int, decimal?>();
                        var fu = e.Attribute("fund");
                        if (fu != null)
                            FundItem.Add(fu.Value.ToInt(), e.Value.ToDecimal());
                        break;
                    case "ExtraQuestion":
                        if (ExtraQuestion == null)
                            ExtraQuestion = new List<Dictionary<string, string>>();
                        var eqsetattr = e.Attribute("set");
                        if(eqsetattr != null)
                            eqset = eqsetattr.Value.ToInt();
                        if (ExtraQuestion.Count == eqset)
                            ExtraQuestion.Add(new Dictionary<string, string>());
                        var eq = e.Attribute("question");
                        if (eq != null)
                            ExtraQuestion[eqset].Add(eq.Value, e.Value);
                        break;
                    case "YesNoQuestion":
                        if (YesNoQuestion == null)
                            YesNoQuestion = new Dictionary<string, bool?>();
                        var ynq = e.Attribute("question");
                        if (ynq != null)
                            YesNoQuestion.Add(ynq.Value, e.Value.ToBool());
                        break;
                    case "option":
                        if (option == null)
                            option = new List<string>();
                        option.Add(e.Value);
                        break;
                    case "Checkbox":
                        if (Checkbox == null)
                            Checkbox = new List<string>();
                        Checkbox.Add(e.Value);
                        break;
                    case "MenuItem":
                        if (MenuItem == null)
                            MenuItem = new Dictionary<string, int?>();
                        var aname = e.Attribute("name");
                        var number = e.Attribute("number");
                        if (aname != null && number != null)
                            MenuItem.Add(aname.Value, number.Value.ToInt());
                        break;
                    default:
                        Util.SetPropertyFromText(this, name, e.Value);
                        break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            var optionsAdded = false;
            var checkoxesAdded = false;
            var menuitemsAdded = false;
            var w = new APIWriter(writer);
            foreach (PropertyInfo pi in typeof(OnlineRegPersonModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(vv => vv.CanRead && vv.CanWrite))
            {
                switch (pi.Name)
                {
                    case "FundItem":
                        if (FundItem != null && FundItem.Count > 0)
                            foreach (var f in FundItem.Where(ff => ff.Value > 0))
                            {
                                w.Start("FundItem");
                                w.Attr("fund", f.Key);
                                w.AddText(f.Value.Value.ToString());
                                w.End();
                            }
                        break;
                    case "ExtraQuestion":
                        if(ExtraQuestion != null)
                            for (var i = 0; i < ExtraQuestion.Count; i++)
                                if (ExtraQuestion[i] != null && ExtraQuestion[i].Count > 0)
                                    foreach (var q in ExtraQuestion[i])
                                    {
                                        w.Start("ExtraQuestion");
                                        w.Attr("set", i);
                                        w.Attr("question", q.Key);
                                        w.AddText(q.Value);
                                        w.End();
                                    }
                        break;
                    case "YesNoQuestion":
                        if (YesNoQuestion != null && YesNoQuestion.Count > 0)
                            foreach (var q in YesNoQuestion)
                            {
                                w.Start("YesNoQuestion");
                                w.Attr("question", q.Key);
                                w.AddText(q.Value.ToString());
                                w.End();
                            }
                        break;
                    case "option":
                        if (option != null && option.Count > 0 && !optionsAdded)
                            foreach(var o in option)
                                w.Add("option", o);
                        optionsAdded = true;
                        break;
                    case "Checkbox":
                        if (Checkbox != null && Checkbox.Count > 0 && !checkoxesAdded)
                            foreach (var c in Checkbox)
                                w.Add("Checkbox", c);
                        checkoxesAdded = true;
                        break;
                    case "MenuItem":
                        if (MenuItem != null && !menuitemsAdded)
                            foreach (var kv in MenuItem)
                            {
                                w.Start("MenuItem");
                                w.Attr("name", kv.Key);
                                w.Attr("number", kv.Value);
                                w.End();
                            }
                        menuitemsAdded = true;
                        break;
                    default:
                        w.Add(pi.Name, pi.GetValue(this, null));
                        break;
                }
            }
        }

        public OnlineRegPersonModel()
        {
            YesNoQuestion = new Dictionary<string, bool?>();
            FundItem = new Dictionary<int, decimal?>();
            Parent = HttpContext.Current.Items["OnlineRegModel"] as OnlineRegModel;
        }
        private void AfterSettingConstructor()
        {
            if (_setting == null)
                return;
            var ndd = setting.AskItems.Count(aa => aa.Type == "AskDropdown");
            if (ndd > 0 && option == null)
                option = new string[ndd].ToList();

            var neqsets = setting.AskItems.Count(aa => aa.Type == "AskExtraQuestions");
            if (neqsets > 0 && ExtraQuestion == null)
            {
                ExtraQuestion = new List<Dictionary<string, string>>();
                for(var i = 0; i < neqsets; i++)
                    ExtraQuestion.Add(new Dictionary<string, string>());
            }
            var nmi = setting.AskItems.Count(aa => aa.Type == "AskMenu");
            if (nmi > 0 && MenuItem == null)
                MenuItem = new Dictionary<string, int?>();

            var ncb = setting.AskItems.Count(aa => aa.Type == "AskCheckboxes");
            if (ncb > 0 && Checkbox == null)
                Checkbox = new List<string>();

            if (!Suggestedfee.HasValue && setting.AskVisible("AskSuggestedFee"))
                Suggestedfee = setting.Fee;
        }

        public OnlineRegModel Parent;

        public int? index;
        public int Index()
        {
            if (!index.HasValue)
                index = Parent.List.IndexOf(this);
            if (index == -1)
                index = 0;
            return index.Value;
        }

        public bool LastItem()
        {
            return Index() == Parent.List.Count - 1;
        }

        public bool SawExistingAccount;
        public bool CannotCreateAccount;
        public bool CreatedAccount;


        public string email { get; set; }
        public string fromemail
        {
            get { return first + " " + last + " <" + email + ">"; }
        }

        public int? MenuItemValue(string s)
        {
            if (MenuItem.ContainsKey(s))
                return MenuItem[s];
            return null;
        }

        public decimal? FundItemValue(int n)
        {
            if (FundItem.ContainsKey(n))
                return FundItem[n];
            return null;
        }

        private DateTime _Birthday;
        public DateTime? birthday
        {
            get
            {
                if (_Birthday == DateTime.MinValue)
                    Util.BirthDateValid(dob, out _Birthday);
                return _Birthday == DateTime.MinValue ? (DateTime?)null : _Birthday;
            }
        }
        public string NotFoundText;
        private int count;

        private Person _Person;

        public Person person
        {
            get
            {
                if (_Person == null)
                    if (PeopleId.HasValue)
                    {
                        _Person = DbUtil.Db.LoadPersonById(PeopleId.Value);
                        count = 1;
                    }
                    else
                    {
                        //_Person = SearchPeopleModel.FindPerson(first, last, birthday, email, phone, out count);

                        var list = DbUtil.Db.FindPerson(first, last, birthday, email, phone.GetDigits()).ToList();
                        count = list.Count;
                        if (count == 1)
                            _Person = DbUtil.Db.LoadPersonById(list[0].PeopleId.Value);
                        if (_Person != null)
                            PeopleId = _Person.PeopleId;
                    }
                return _Person;
            }
        }
        public void AddPerson(Person p, int entrypoint)
        {
            Family f;
            if (p == null)
                f = new Family
                {
                    AddressLineOne = address,
                    AddressLineTwo = address2,
                    CityName = city,
                    StateCode = state,
                    ZipCode = zip,
                    CountryName = country,
                    HomePhone = homephone,
                };
            else
                f = p.Family;

            _Person = Person.Add(f, PositionInFamily.Child,
                null, first.Trim(), null, last.Trim(), dob, married == 20, gender ?? 0,
                    OriginCode.Enrollment, entrypoint);
            person.EmailAddress = email.Trim();
            person.SendEmailAddress1 = true;
            person.SuffixCode = suffix;
            person.MiddleName = middle;
            person.CampusId = DbUtil.Db.Setting("DefaultCampusId", "").ToInt2();
            if (person.Age >= 18)
                person.PositionInFamilyId = PositionInFamily.PrimaryAdult;
            person.CellPhone = phone.GetDigits();

            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("OnlineReg AddPerson {0}".Fmt(person.PeopleId));
            DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, person);
            PeopleId = person.PeopleId;
        }
        public bool IsCreateAccount()
        {
            if (org != null)
                return org.RegistrationTypeId == RegistrationTypeCode.CreateAccount;
            return false;
        }
        public XmlSchema GetSchema()
        {
            throw new System.NotImplementedException("The method or operation is not implemented.");
        }

    }
}
