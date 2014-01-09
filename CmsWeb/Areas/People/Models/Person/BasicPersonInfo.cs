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

namespace CmsWeb.Areas.People.Models
{
    public class EmailInfo : IModelViewModelObject
    {
        [StringLength(20)]
        public string Address { get; set; }
        public bool Send { get; set; }

        public string CopyToModel(PropertyInfo vm, object model, PropertyInfo[] modelProps, bool track)
        {
            var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
            var ckpi = modelProps.Single(mm => mm.Name == ckf);
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
        [StringLength(20)]
        public string Number { get; set; }
        public bool ReceiveText { get; set; }

        public string CopyToModel(PropertyInfo vm, object model, PropertyInfo[] modelProps, bool track)
        {
            var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
            var ckpi = modelProps.Single(mm => mm.Name == ckf);
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

    [TrackChanges]
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

        [NoUpdate]
        public int PeopleId { get; set; }
        public Person person { get; set; }

        [DisplayName("Title"), StringLength(10)]
        public string TitleCode { get; set; }

        [TrackChanges, StringLength(25)]
        public string FirstName { get; set; }

        [DisplayName("Goes By"), StringLength(15)]
        public string NickName { get; set; }

        [StringLength(15)]
        public string MiddleName { get; set; }

        [StringLength(100), Required]
        public string LastName { get; set; }

        [StringLength(100)]
        public string AltName { get; set; }

        [DisplayName("Former Name"), StringLength(20)]
        public string MaidenName { get; set; }

        [DisplayName("Suffix"), StringLength(10)]
        public string SuffixCode { get; set; }

        public CodeInfo Gender { get; set; }

        public CodeInfo PositionInFamily { get; set; }

        public CodeInfo Campus { get; set; }

        [DisplayName("Birthday")]
        public string DOB { get; set; }

        public CodeInfo MaritalStatus { get; set; }

        public DateTime? WeddingDate { get; set; }

        [DisplayName("Occupation"), NoTrack, StringLength(60)]
        public string OccupationOther { get; set; }

        [DisplayName("Employer"), NoTrack, StringLength(60)]
        public string EmployerOther { get; set; }

        [DisplayName("School"), NoTrack, StringLength(100)]
        public string SchoolOther { get; set; }

        public int? Grade { get; set; }

        [DisplayName("Primary Email"), FieldInfo(CheckboxField = "SendEmailAddress1")]
        public EmailInfo EmailAddress { get; set; }

        [DisplayName("Alt Email"), FieldInfo(CheckboxField = "SendEmailAddress2")]
        public EmailInfo EmailAddress2 { get; set; }

        [PhoneNumber, StringLength(20)]
        public string HomePhone { get; set; }

        [PhoneNumber, FieldInfo(CheckboxField = "ReceiveSMS")]
        public CellPhoneInfo CellPhone { get; set; }

        [PhoneNumber, StringLength(20)]
        public string WorkPhone { get; set; }

        public DateTime? DeceasedDate { get; set; }

        public bool DoNotCallFlag { get; set; }

        public bool DoNotVisitFlag { get; set; }

        public bool DoNotMailFlag { get; set; }

        public bool DoNotPublishPhones { get; set; }

        [NoUpdate]
        public DateTime? CreatedDate { get; set; }
        [NoUpdate]
        public DateTime? JoinDate { get; set; }
        [NoUpdate]
        public string Spouse { get; set; }

        [NoUpdate]
        public string Age { get; set; }

        [NoUpdate]
        public CodeInfo MemberStatus { get; set; }

        [NoTrack]
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
        {
            Id = id;
            if (person == null)
                return;
            person.SendEmailAddress1 = person.SendEmailAddress1 ?? true; // set sendemailaddress1 to either true or false, null is not allowed, default is true
            this.CopyPropertiesFrom(person);
        }
        public void UpdatePerson()
        {
            var p = DbUtil.Db.LoadPersonById(PeopleId);

            if (Grade == null && p.Grade == 0) // special case to fix bug
                p.Grade = null;

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
    }
}
