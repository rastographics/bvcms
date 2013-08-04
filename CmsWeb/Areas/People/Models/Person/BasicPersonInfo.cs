using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Routing;
using System.Linq;
using CmsData;
using System.Web.Mvc;
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

        public CodeInfo Title { get; set; }

        [UIHint("Text")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [UIHint("Text")]
        [DisplayName("Goes By")]
        public string GoesBy { get; set; }

        [UIHint("Text")]
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        [UIHint("Text")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [UIHint("Text")]
        [DisplayName("Alt Name")]
        public string AltName { get; set; }

        [UIHint("Text")]
        [DisplayName("Former Name")]
        public string FormerName { get; set; }

        [UIHint("Text")]
        public string Suffix { get; set; }

        public CodeInfo Gender { get; set; }

        [UIHint("InlineCode")]
        [DisplayName("Family Position")]
        public CodeInfo FamilyPosition { get; set; }

        [UIHint("InlineCode")]
        [DisplayName("Campus")]
        public CodeInfo Campus { get; set; }

        [UIHint("Date")]
        public string Birthday { get; set; }

        public CodeInfo Marital { get; set; }

        [UIHint("Date")]
        [DisplayName("Wedding Date")]
        public string WeddingDate { get; set; }

        [UIHint("Text")]
        public string Occupation { get; set; }

        [UIHint("Text")]
        public string Employer { get; set; }

        [UIHint("Text")]
        public string School { get; set; }

        [UIHint("Text")]
        public string Grade { get; set; }

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
        [UIHint("Email")]
        [DisplayName("Primary Email")]
        public EmailInfo PrimaryEmail { get; set; }
        [UIHint("Email")]
        [DisplayName("Alt Email")]
        public EmailInfo AltEmail { get; set; }

        [UIHint("Text")]
        [DisplayName("Home Phone")]
        public string HomePhone { get; set; }

        [UIHint("CellPhone")]
        [DisplayName("Cell Phone")]
        public CellPhoneInfo Mobile { get; set; }

        [UIHint("Text")]
        public string Work { get; set; }

        public bool DoNotCallFlag { get; set; }
        public bool DoNotVisitFlag { get; set; }
        public bool DoNotMailFlag { get; set; }

        public DateTime? Created { get; set; }

        public bool ReceiveSMS { get; set; }
        public int MemberStatusId { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Spouse { get; set; }
        public DateTime? DeceasedDate { get; set; }
        public string Age { get; set; }

        public bool DoNotPublishPhones { get; set; }



//        public string Campus
//        {
//            get { return cv.AllCampuses0().ItemValue(CampusId ?? 0); }
//        }

        public CodeInfo MemberStatus { get; set; }
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

        public static BasicPersonInfo GetBasicPersonInfo(int? id)
        {
            var cv = new CodeValueModel();
            var p = DbUtil.Db.LoadPersonById(id.Value);
            var pi = new BasicPersonInfo
            {
                Age = p.Age.ToString(),
                Birthday = p.DOB,
                Campus = new CodeInfo(p.CampusId, "Campus"),
                Mobile = new CellPhoneInfo(p.CellPhone.FmtFone(), p.ReceiveSMS),
                DeceasedDate = p.DeceasedDate,
                DoNotCallFlag = p.DoNotCallFlag,
                DoNotMailFlag = p.DoNotMailFlag,
                DoNotVisitFlag = p.DoNotVisitFlag,
                DoNotPublishPhones = p.DoNotPublishPhones ?? false,
                PrimaryEmail = new EmailInfo(p.EmailAddress, p.SendEmailAddress1 ?? true),
                AltEmail = new EmailInfo(p.EmailAddress2, p.SendEmailAddress2 ?? false),
                Employer = p.EmployerOther,
                FirstName = p.FirstName,
                Created = p.CreatedDate,
                Grade = p.Grade.ToString(),
                HomePhone = p.Family.HomePhone.FmtFone(),
                JoinDate = p.JoinDate,
                LastName = p.LastName,
                AltName = p.AltName,
                FormerName = p.MaidenName,
                Gender = new CodeInfo(p.GenderId, "Gender"),
                Marital = new CodeInfo(p.MaritalStatusId, "Marital"),
                MemberStatus = new CodeInfo(p.MemberStatusId, "MemberStatus"),
                FamilyPosition = new CodeInfo(p.PositionInFamilyId, "FamilyPosition"),
                MiddleName = p.MiddleName,
                GoesBy = p.NickName,
                Occupation = p.OccupationOther,
                PeopleId = p.PeopleId,
                School = p.SchoolOther,
                Spouse = p.SpouseName(DbUtil.Db),
                Suffix = p.SuffixCode,
                Title = new CodeInfo(p.TitleCode, "Title"),
                WeddingDate = p.WeddingDate.FormatDate(),
                Work = p.WorkPhone.FmtFone(),
                ReceiveSMS = p.ReceiveSMS,
            };
            pi.person = p;
            return pi;
        }

        public void UpdatePerson()
        {
            var campusid = Campus.Value.ToInt2();
            if (campusid == 0)
                campusid = null;
            var p = DbUtil.Db.LoadPersonById(PeopleId);
            var psb = new StringBuilder();
            var fsb = new StringBuilder();
            p.UpdateValue(psb, "DOB", Birthday);
            p.UpdateValue(psb, "DeceasedDate", DeceasedDate);
            p.UpdateValue(psb, "DoNotCallFlag", DoNotCallFlag);
            p.UpdateValue(psb, "DoNotMailFlag", DoNotMailFlag);
            p.UpdateValue(psb, "DoNotVisitFlag", DoNotVisitFlag);
            p.UpdateValue(psb, "DoNotPublishPhones", DoNotPublishPhones);
            p.UpdateValue(psb, "EmailAddress", PrimaryEmail.Address);
            p.UpdateValue(psb, "EmailAddress2", AltEmail.Address);
            p.UpdateValue(psb, "SendEmailAddress1", PrimaryEmail.Send);
            p.UpdateValue(psb, "SendEmailAddress2", AltEmail.Send);
            p.UpdateValue(psb, "FirstName", FirstName);
            p.UpdateValue(psb, "LastName", LastName);
            p.UpdateValue(psb, "AltName", AltName);
            p.UpdateValue(psb, "GenderId", Gender.Value.ToInt2());
            p.UpdateValue(psb, "Grade", Grade.ToInt2());
            p.UpdateValue(psb, "CellPhone", Mobile.Number.GetDigits());
            p.UpdateValue(psb, "ReceiveSMS", Mobile.ReceiveText);
            p.Family.UpdateValue(fsb, "HomePhone", HomePhone.GetDigits());
            p.UpdateValue(psb, "MaidenName", FormerName);
            p.UpdateValue(psb, "MaritalStatusId", Marital.Value.ToInt2());
            p.UpdateValue(psb, "MiddleName", MiddleName);
            p.UpdateValue(psb, "NickName", GoesBy);
            p.UpdateValue(psb, "OccupationOther", Occupation);
            p.UpdateValue(psb, "SchoolOther", School);
            p.UpdateValue(psb, "SuffixCode", Suffix);
            p.UpdateValue(psb, "EmployerOther", Employer);
            p.UpdateValue(psb, "TitleCode", Title.Value);
            p.UpdateValue(psb, "CampusId", campusid);
            p.UpdateValue(psb, "WeddingDate", WeddingDate.ToDate());
            p.UpdateValue(psb, "WorkPhone", Work.GetDigits());
            if (p.DeceasedDateChanged)
            {
                var ret = p.MemberProfileAutomation(DbUtil.Db);
                if (ret != "ok")
                    Elmah.ErrorSignal.FromCurrentContext().Raise(
                        new Exception(ret + " for PeopleId:" + p.PeopleId));
            }


            p.LogChanges(DbUtil.Db, psb, Util.UserPeopleId.Value);
            p.Family.LogChanges(DbUtil.Db, fsb, p.PeopleId, Util.UserPeopleId.Value);

            DbUtil.Db.SubmitChanges();

            if (!HttpContext.Current.User.IsInRole("Access"))
                if (psb.Length > 0 || fsb.Length > 0)
                {
                    DbUtil.Db.EmailRedacted(p.FromEmail, DbUtil.Db.GetNewPeopleManagers(),
                        "Basic Person Info Changed on " + Util.Host,
                        "{0} changed the following information for {1} ({2}):<br />\n<table>{3}{4}</table>"
                        .Fmt(Util.UserName, FirstName + " " + LastName, PeopleId, psb.ToString(), fsb.ToString()));
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
