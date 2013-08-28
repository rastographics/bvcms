using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using CmsWeb.Code;
using UtilityExtensions;
using System.Text;

namespace CmsWeb.Models.PersonPage
{
    public class BasicPersonInfo
    {
        private CodeValueModel cv = new CodeValueModel();
        public int PeopleId { get; set; }
        public Person person { get; set; }
        public string NickName { get; set; }
        public string Title { get; set; }
        public string First { get; set; }
        public string Middle { get; set; }
        public string Last { get; set; }
        public string AltName { get; set; }
        public string Suffix { get; set; }
        public string Maiden { get; set; }
        public int GenderId { get; set; }
        public DateTime? Created { get; set; }

        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string WorkPhone { get; set; }
        public string EmailAddress { get; set; }
        public bool SendEmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
        public bool SendEmailAddress2 { get; set; }
        public string School { get; set; }
        public string Grade { get; set; }
        public string Employer { get; set; }
        public string Occupation { get; set; }
        public bool DoNotCallFlag { get; set; }
        public bool DoNotVisitFlag { get; set; }
        public bool DoNotMailFlag { get; set; }
        public bool ReceiveSMS { get; set; }

        public int? CampusId { get; set; }
        public int MemberStatusId { get; set; }
        public DateTime? JoinDate { get; set; }
        public int MaritalStatusId { get; set; }
        public string Spouse { get; set; }
        public DateTime? WeddingDate { get; set; }
        public string Birthday { get; set; }
        public DateTime? DeceasedDate { get; set; }
        public string Age { get; set; }
        public bool DoNotPublishPhones { get; set; }

        public string Campus
        {
            get { return cv.AllCampuses0().ItemValue(CampusId ?? 0); }

        }
        public string Gender
        {
            get { return cv.GenderCodes().ItemValue(GenderId); }
            
        }
        public string MaritalStatus
        {
            get { return cv.MaritalStatusCodes().ItemValue(MaritalStatusId); }
            
        }
        public string MemberStatus
        {
            get { return cv.MemberStatusCodes().ItemValue(MemberStatusId); }
            
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

        public static BasicPersonInfo GetBasicPersonInfo(int? id)
        {
            var p = DbUtil.Db.LoadPersonById(id.Value);
            var pi = new BasicPersonInfo
            {
                Age = p.Age.ToString(),
                Birthday = p.DOB,
                CampusId = p.CampusId ?? 0,
                CellPhone = p.CellPhone.FmtFone(),
                DeceasedDate = p.DeceasedDate,
                DoNotCallFlag = p.DoNotCallFlag,
                DoNotMailFlag = p.DoNotMailFlag,
                DoNotVisitFlag = p.DoNotVisitFlag,
                DoNotPublishPhones = p.DoNotPublishPhones ?? false,
                EmailAddress = p.EmailAddress,
                SendEmailAddress1 = p.SendEmailAddress1 ?? true,
                EmailAddress2 = p.EmailAddress2,
                SendEmailAddress2 = p.SendEmailAddress2 ?? false,
                Employer = p.EmployerOther,
                First = p.FirstName,
                GenderId = p.GenderId,
                Created = p.CreatedDate,
                Grade = p.Grade.ToString(),
                HomePhone = p.Family.HomePhone.FmtFone(),
                JoinDate = p.JoinDate,
                Last = p.LastName,
                AltName = p.AltName,
                Maiden = p.MaidenName,
                MaritalStatusId = p.MaritalStatusId,
                MemberStatusId = p.MemberStatusId,
                Middle = p.MiddleName,
                NickName = p.NickName,
                Occupation = p.OccupationOther,
                PeopleId = p.PeopleId,
                School = p.SchoolOther,
                Spouse = p.SpouseName(DbUtil.Db),
                Suffix = p.SuffixCode,
                Title = p.TitleCode,
                WeddingDate = p.WeddingDate,
                WorkPhone = p.WorkPhone.FmtFone(),
                ReceiveSMS = p.ReceiveSMS,
            };
            pi.person = p;
            return pi;
        }

        public void UpdatePerson()
        {
            if (CampusId == 0)
                CampusId = null;
            var p = DbUtil.Db.LoadPersonById(PeopleId);
            var psb = new StringBuilder();
            var fsb = new StringBuilder();
            p.UpdateValue(psb, "DOB", Birthday);
            p.UpdateValue(psb, "CampusId", CampusId);
            p.UpdateValue(psb, "DeceasedDate", DeceasedDate);
            p.UpdateValue(psb, "DoNotCallFlag", DoNotCallFlag);
            p.UpdateValue(psb, "DoNotMailFlag", DoNotMailFlag);
            p.UpdateValue(psb, "DoNotVisitFlag", DoNotVisitFlag);
            p.UpdateValue(psb, "DoNotPublishPhones", DoNotPublishPhones);
            p.UpdateValue(psb, "EmailAddress", EmailAddress);
            p.UpdateValue(psb, "EmailAddress2", EmailAddress2);
            p.UpdateValue(psb, "SendEmailAddress1", SendEmailAddress1);
            p.UpdateValue(psb, "SendEmailAddress2", SendEmailAddress2);
            p.UpdateValue(psb, "ReceiveSMS", ReceiveSMS);
            p.UpdateValue(psb, "FirstName", First);
            p.UpdateValue(psb, "LastName", Last);
            p.UpdateValue(psb, "AltName", AltName);
            p.UpdateValue(psb, "GenderId", GenderId);
            p.UpdateValue(psb, "Grade", Grade.ToInt2());
            p.UpdateValue(psb, "CellPhone", CellPhone.GetDigits());
            p.Family.UpdateValue(fsb, "HomePhone", HomePhone.GetDigits());
            p.UpdateValue(psb, "MaidenName", Maiden);
            p.UpdateValue(psb, "MaritalStatusId", MaritalStatusId);
            p.UpdateValue(psb, "MiddleName", Middle);
            p.UpdateValue(psb, "NickName", NickName);
            p.UpdateValue(psb, "OccupationOther", Occupation);
            p.UpdateValue(psb, "SchoolOther", School);
            p.UpdateValue(psb, "SuffixCode", Suffix);
            p.UpdateValue(psb, "EmployerOther", Employer);
            p.UpdateValue(psb, "TitleCode", Title);
            p.UpdateValue(psb, "WeddingDate", WeddingDate);
            p.UpdateValue(psb, "WorkPhone", WorkPhone.GetDigits());
            if (p.DeceasedDateChanged)
            {
                var ret = p.MemberProfileAutomation(DbUtil.Db);
                if (ret != "ok")
                    Elmah.ErrorSignal.FromCurrentContext().Raise(
                        new Exception(ret + " for PeopleId:" + p.PeopleId));
            }
                    

            p.LogChanges(DbUtil.Db, psb.ToString(), Util.UserPeopleId.Value);
            p.Family.LogChanges(DbUtil.Db, fsb.ToString(), p.PeopleId, Util.UserPeopleId.Value);

            DbUtil.Db.SubmitChanges();
            
            if (!HttpContext.Current.User.IsInRole("Access"))
                if (psb.Length > 0 || fsb.Length > 0)
                {
                    DbUtil.Db.EmailRedacted(p.FromEmail, DbUtil.Db.GetNewPeopleManagers(),
                        "Basic Person Info Changed on " + Util.Host,
                        "{0} changed the following information for {1} ({2}):<br />\n<table>{3}{4}</table>"
                        .Fmt(Util.UserName, First + " " + Last, PeopleId, psb.ToString(),fsb.ToString()));
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
