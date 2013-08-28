using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
    public class EmailInfo
    {
        public EmailInfo(string address, bool send)
        {
            Address = address;
            Send = send;
        }
        public string Address { get; set; }
        public bool Send { get; set; }
    }
    public class CellPhoneInfo
    {
        public CellPhoneInfo(string number, bool active)
        {
            Number = number;
            ReceiveText = active;
        }
        public string Number { get; set; }
        public bool ReceiveText { get; set; }
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

        private CodeValueModel cv = new CodeValueModel();
        public int PeopleId { get; set; }
        public CmsData.Person person { get; set; }

        [UIHint("Text"), DisplayName("Title"), TrackChanges]
        public string TitleCode { get; set; }

        [UIHint("Text"), TrackChanges]
        public string FirstName { get; set; }

        [UIHint("Text"), DisplayName("Goes By"), TrackChanges]
        public string GoesBy { get; set; }

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
//        public int MemberStatusId { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Spouse { get; set; }

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
//        public static BasicPersonInfo GetBasicPersonInfo(int? id)
//        {
//            var p = DbUtil.Db.LoadPersonById(id.Value);
//
//            var pi = new BasicPersonInfo();
//            pi.CopyPropertiesFrom(p);
//            pi.Mobile = new CellPhoneInfo(p.CellPhone.FmtFone(), p.ReceiveSMS);
//            pi.person = p;
//            return pi;
//
//            //            var pi = new BasicPersonInfo
//            //            {
//            //                Age = p.Age.ToString(),
//            //                Birthday = p.DOB,
//            //                Campus = new CodeInfo(p.CampusId, "Campus"),
//            //                DeceasedDate = p.DeceasedDate,
//            //                DoNotCallFlag = p.DoNotCallFlag,
//            //                DoNotMailFlag = p.DoNotMailFlag,
//            //                DoNotVisitFlag = p.DoNotVisitFlag,
//            //                DoNotPublishPhones = p.DoNotPublishPhones ?? false,
//            //                PrimaryEmail = new EmailInfo(p.EmailAddress, p.SendEmailAddress1 ?? true),
//            //                AltEmail = new EmailInfo(p.EmailAddress2, p.SendEmailAddress2 ?? false),
//            //                Employer = p.EmployerOther,
//            //                FirstName = p.FirstName,
//            //                Created = p.CreatedDate,
//            //                Grade = p.Grade.ToString(),
//            //                JoinDate = p.JoinDate,
//            //                LastName = p.LastName,
//            //                AltName = p.AltName,
//            //                FormerName = p.MaidenName,
//            //                Gender = new CodeInfo(p.GenderId, "Gender"),
//            //                Marital = new CodeInfo(p.MaritalStatusId, "Marital"),
//            //                MemberStatus = new CodeInfo(p.MemberStatusId, "MemberStatus"),
//            //                FamilyPosition = new CodeInfo(p.PositionInFamilyId, "FamilyPosition"),
//            //                MiddleName = p.MiddleName,
//            //                GoesBy = p.NickName,
//            //                Occupation = p.OccupationOther,
//            //                PeopleId = p.PeopleId,
//            //                School = p.SchoolOther,
//            //                Spouse = p.SpouseName(DbUtil.Db),
//            //                Suffix = p.SuffixCode,
//            //                Title = new CodeInfo(p.TitleCode, "Title"),
//            //                WeddingDate = p.WeddingDate.FormatDate(),
//            //                Work = p.WorkPhone.FmtFone(),
//            //                ReceiveSMS = p.ReceiveSMS,
//            //            };
//        }

//            //                HomePhone = p.Family.HomePhone.FmtFone(),

            //            var psb = new StringBuilder();
            //            var fsb = new StringBuilder();
            //            p.UpdateValue(psb, "DOB", Birthday);
            //            p.UpdateValue(psb, "DeceasedDate", DeceasedDate);
            //            p.UpdateValue(psb, "DoNotCallFlag", DoNotCallFlag);
            //            p.UpdateValue(psb, "DoNotMailFlag", DoNotMailFlag);
            //            p.UpdateValue(psb, "DoNotVisitFlag", DoNotVisitFlag);
            //            p.UpdateValue(psb, "DoNotPublishPhones", DoNotPublishPhones);
            //            p.UpdateValue(psb, "EmailAddress", PrimaryEmail.Address);
            //            p.UpdateValue(psb, "EmailAddress2", AltEmail.Address);
            //            p.UpdateValue(psb, "SendEmailAddress1", PrimaryEmail.Send);
            //            p.UpdateValue(psb, "SendEmailAddress2", AltEmail.Send);
            //            p.UpdateValue(psb, "FirstName", FirstName);
            //            p.UpdateValue(psb, "LastName", LastName);
            //            p.UpdateValue(psb, "AltName", AltName);
            //            p.UpdateValue(psb, "GenderId", Gender.Value.ToInt2());
            //            p.UpdateValue(psb, "Grade", Grade.ToInt2());
            //            p.UpdateValue(psb, "CellPhone", Mobile.Number.GetDigits());
            //            p.UpdateValue(psb, "ReceiveSMS", Mobile.ReceiveText);
            //            p.UpdateValue(psb, "MaidenName", FormerName);
            //            p.UpdateValue(psb, "MaritalStatusId", Marital.Value.ToInt2());
            //            p.UpdateValue(psb, "MiddleName", MiddleName);
            //            p.UpdateValue(psb, "NickName", GoesBy);
            //            p.UpdateValue(psb, "OccupationOther", Occupation);
            //            p.UpdateValue(psb, "SchoolOther", School);
            //            p.UpdateValue(psb, "SuffixCode", Suffix);
            //            p.UpdateValue(psb, "EmployerOther", Employer);
            //            p.UpdateValue(psb, "TitleCode", Title.Value);
            //            p.UpdateValue(psb, "CampusId", campusid);
            //            p.UpdateValue(psb, "WeddingDate", WeddingDate.ToDate());
            //            p.UpdateValue(psb, "WorkPhone", Work.GetDigits());

            //            p.Family.UpdateValue(fsb, "HomePhone", HomePhone.GetDigits());
        public void UpdatePerson()
        {
            var campusid = Campus.Value.ToInt2();
            if (campusid == 0)
                campusid = null;
            var p = DbUtil.Db.LoadPersonById(PeopleId);
            var changes = this.CopyPropertiesTo(p);
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
