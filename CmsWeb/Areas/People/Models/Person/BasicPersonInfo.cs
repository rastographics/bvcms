using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using System.Linq;
using CmsData;
using System.Web.Mvc;
using CmsWeb.Areas.Dialog.Controllers;
using CmsWeb.Code;
using CmsWeb.Models;
using UtilityExtensions;
using System.Text;

namespace CmsWeb.Areas.People.Models.Person
{
    public class EmailInfo : IModelViewModelObject
    {
//        public EmailInfo()
//        {
//            
//        }
//        public EmailInfo(string address, bool send)
//        {
//            Address = address;
//            Send = send;
//        }
        public string Address { get; set; }
        public bool Send { get; set; }

        public string CopyToModel(PropertyInfo vm, object model, PropertyInfo[] modelProps)
        {
            var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
            var ckpi = modelProps.Single(mm => mm.Name == ckf);
            var track = Attribute.IsDefined(vm, typeof(TrackChangesAttribute));
            if (track)
            {
                var changes = new StringBuilder();
                model.UpdateValue(changes, vm.Name, Address);
                model.UpdateValue(changes, ckf, Send);
                return changes.ToString();
            }
            var ci = modelProps.FirstOrDefault(ss => ss.Name == vm.Name);
            Debug.Assert(ci != null, "ci != null");
            ci.SetValue(model, Address, null);
            ckpi.SetValue(model, Send, null);
            return string.Empty;
        }

        public void CopyFromModel(PropertyInfo vm, object model, PropertyInfo[] modelProps)
        {
            var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
            var ckpi = modelProps.Single(ss => ss.Name == ckf);
            var ck = ckpi.GetValue(model, null) as bool?;
            var m = modelProps.FirstOrDefault(mm => mm.Name == vm.Name);
            Debug.Assert(m != null, "m != null");
            Address = ((string)m.GetValue(model, null));
            Send = ck ?? false;
        }
    }
    public class CellPhoneInfo : IModelViewModelObject
    {
//        public CellPhoneInfo(string number, bool active)
//        {
//            Number = number;
//            ReceiveText = active;
//        }
        public string Number { get; set; }
        public bool ReceiveText { get; set; }

        public string CopyToModel(PropertyInfo vm, object model, PropertyInfo[] modelProps)
        {
            var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
            var ckpi = modelProps.Single(mm => mm.Name == ckf);
            var track = Attribute.IsDefined(vm, typeof(TrackChangesAttribute));
            if (track)
            {
                var changes = new StringBuilder();
                model.UpdateValue(changes, vm.Name, Number.GetDigits());
                model.UpdateValue(changes, ckf, ReceiveText);
                return changes.ToString();
            }
            var ci = modelProps.FirstOrDefault(ss => ss.Name == vm.Name);
            Debug.Assert(ci != null, "ci != null");
            ci.SetValue(model, Number.GetDigits(), null);
            ckpi.SetValue(model, ReceiveText, null);
            return string.Empty;
        }

        public void CopyFromModel(PropertyInfo vm, object model, PropertyInfo[] modelProps)
        {
            var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
            var ckpi = modelProps.Single(ss => ss.Name == ckf);
            var ck = ckpi.GetValue(model, null) as bool?;
            var m = modelProps.FirstOrDefault(mm => mm.Name == vm.Name);
            Number = ((string)m.GetValue(model, null)).FmtFone();
            ReceiveText = ck ?? false;
        }
    }

    public class BasicPersonInfo
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                if (_id == 0)
                    return;
                person = DbUtil.Db.LoadPersonById(value);
            }
        }
        private RouteValueDictionary routeVals;
        public RouteValueDictionary RouteVals
        {
            get
            {
                if (routeVals == null)
                    routeVals = new RouteValueDictionary { { "Id", PeopleId }, { "Area", "People" } };
                return routeVals;
            }
        }

        public Dictionary<string, object> Classes(string classes)
        {
            var d = new Dictionary<string, object>();
            d.Add("class", classes);
            return d;
        }

        [NoUpdate]
        public int PeopleId { get; set; }
        public CmsData.Person person { get; set; }

        [UIHint("Text"), DisplayName("Title"), TrackChanges]
        public string TitleCode { get; set; }

        [UIHint("Text"), TrackChanges]
        public string FirstName { get; set; }

        [UIHint("Text"), DisplayName("Goes By"), TrackChanges]
        public string NickName { get; set; }

        [UIHint("Text"), TrackChanges]
        public string MiddleName { get; set; }

        [UIHint("Text"), TrackChanges]
        public string LastName { get; set; }

        [UIHint("Text"), TrackChanges]
        public string AltName { get; set; }

        [UIHint("Text"), DisplayName("Former Name"), TrackChanges]
        public string MaidenName { get; set; }

        [UIHint("Text"), DisplayName("Suffix"), TrackChanges]
        public string SuffixCode { get; set; }

        [TrackChanges]
        public CodeInfo Gender { get; set; }

        [UIHint("InlineCode"), TrackChanges]
        public CodeInfo PositionInFamily { get; set; }

        [UIHint("InlineCode"), TrackChanges]
        public CodeInfo Campus { get; set; }

        [UIHint("Date"), DisplayName("Birthday"), TrackChanges]
        public string DOB { get; set; }

        [TrackChanges]
        public CodeInfo MaritalStatus { get; set; }

        [UIHint("Date"), TrackChanges]
        public string WeddingDate { get; set; }

        [UIHint("Text"), DisplayName("Occupation")]
        public string OccupationOther { get; set; }

        [UIHint("Text"), DisplayName("Employer")]
        public string EmployerOther { get; set; }

        [UIHint("Text"), DisplayName("School")]
        public string SchoolOther { get; set; }

        [UIHint("Text"), TrackChanges]
        public string Grade { get; set; }

        [UIHint("Email"), DisplayName("Primary Email"), TrackChanges,
            FieldInfo(CheckboxField = "SendEmailAddress1")]
        public EmailInfo EmailAddress { get; set; }

        [UIHint("Email"), DisplayName("Alt Email"), TrackChanges,
            FieldInfo(CheckboxField = "SendEmailAddress2")]
        public EmailInfo EmailAddress2 { get; set; }

        [UIHint("Text"), TrackChanges, Phone]
        public string HomePhone { get; set; }

        [UIHint("CellPhone"), TrackChanges, Phone,
            FieldInfo(CheckboxField = "ReceiveSMS")]
        public CellPhoneInfo CellPhone { get; set; }

        [UIHint("Text"), TrackChanges, Phone]
        public string WorkPhone { get; set; }

        [UIHint("Date"), TrackChanges]
        public DateTime? DeceasedDate { get; set; }

        [TrackChanges]
        public bool DoNotCallFlag { get; set; }

        [TrackChanges]
        public bool DoNotVisitFlag { get; set; }

        [TrackChanges]
        public bool DoNotMailFlag { get; set; }

        [TrackChanges]
        public bool DoNotPublishPhones { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Spouse { get; set; }

        [NoUpdate]
        public string Age { get; set; }

        public CodeInfo MemberStatus { get; set; }

        public string DoNotContact
        {
            get
            {
                var list = new List<string>();
                if (DoNotCallFlag)
                    list.Add("by Phone");
                if (DoNotMailFlag)
                    list.Add("by Mail");
                if (DoNotVisitFlag)
                    list.Add("in Person");
                var s = string.Join(", ", list);
                if (!s.HasValue())
                    s = "N/A";
                return s;
            }
        }
        public string DoNotCall
        {
            get { return DoNotCallFlag ? "Do Not Call" : ""; }
        }
        public string DoNotVisit
        {
            get { return DoNotVisitFlag ? "Do Not Visit" : ""; }
        }
        public string DoNotMail
        {
            get { return DoNotMailFlag ? "Do Not Mail" : ""; }
        }
        public bool DoNotContactAny
        {
            get { return DoNotCallFlag || DoNotCallFlag || DoNotVisitFlag; }
        }

        public BasicPersonInfo()
        {

        }

        public BasicPersonInfo(int id)
            : this()
        {
            Id = id;
            if (person == null)
                return;
            this.CopyPropertiesFrom(person);
        }
        public void UpdatePerson()
        {
            var p = DbUtil.Db.LoadPersonById(PeopleId);
            var changes = this.CopyPropertiesTo(p);
            p.LogChanges(DbUtil.Db, changes);
            if (p.DeceasedDateChanged)
            {
                var ret = p.MemberProfileAutomation(DbUtil.Db);
                if (ret != "ok")
                    Elmah.ErrorSignal.FromCurrentContext().Raise(
                        new Exception(ret + " for PeopleId:" + p.PeopleId));
            }

            DbUtil.Db.SubmitChanges();

            if (!HttpContext.Current.User.IsInRole("Access"))
                if (!string.IsNullOrEmpty(changes))
                {
                    DbUtil.Db.EmailRedacted(p.FromEmail, DbUtil.Db.GetNewPeopleManagers(),
                        "Basic Person Info Changed on " + Util.Host,
                        "{0} changed the following information for {1} ({2}):<br />\n<table>{3}</table>"
                        .Fmt(Util.UserName, FirstName + " " + LastName, PeopleId, changes));
                }
        }
        public static IEnumerable<SelectListItem> GenderCodes()
        {
            var cv = new CodeValueModel();
            return CodeValueModel.ConvertToSelect(cv.GenderCodes(), "Id");
        }
        public static IEnumerable<SelectListItem> Campuses()
        {
            var cv = new CodeValueModel();
            return CodeValueModel.ConvertToSelect(cv.AllCampuses0(), "Id");
        }
        public static IEnumerable<SelectListItem> MemberStatuses()
        {
            var cv = new CodeValueModel();
            return CodeValueModel.ConvertToSelect(cv.MemberStatusCodes(), "Id");
        }
        public static IEnumerable<SelectListItem> MaritalStatuses()
        {
            var cv = new CodeValueModel();
            return CodeValueModel.ConvertToSelect(cv.MaritalStatusCodes(), "Id");
        }
    }
}
