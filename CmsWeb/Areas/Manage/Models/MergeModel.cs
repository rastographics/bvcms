using CmsData;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class MergeModel
    {
        public bool DeleteDuplicate { get; set; }
        public List<BasicInfo> pi { get; set; }

        public int UseTitleCode { get; set; }
        public int UseFirstName { get; set; }
        public int UseLastName { get; set; }
        public int UseNickName { get; set; }
        public int UseAltName { get; set; }
        public int UseCampusId { get; set; }
        public int UseGenderId { get; set; }
        public int UseMaritalStatusId { get; set; }
        public int UseDOB { get; set; }
        public int UseCellPhone { get; set; }
        public int UseEmailAddress { get; set; }
        public int UseEmailAddress2 { get; set; }
        public int UseSuffixCode { get; set; }
        public int UseMiddleName { get; set; }
        public int UseHomePhone { get; set; }
        public int UseWorkPhone { get; set; }
        public int UseAddressLineOne { get; set; }
        public int UseAddressLineTwo { get; set; }
        public int UseCityName { get; set; }
        public int UseStateCode { get; set; }
        public int UseZipCode { get; set; }
        public int UseCountry { get; set; }
        public int UseMaiden { get; set; }

        public class BasicInfo
        {
            private CodeValueModel cvc = new CodeValueModel();

            public int PeopleId { get; set; }
            public Person person
            {
                get { return DbUtil.Db.LoadPersonById(PeopleId); }
            }
            public string TitleCode { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string NickName { get; set; }
            public string AltName { get; set; }
            public string Maiden { get; set; }
            public int? CampusId { get; set; }
            public string Campus
            {
                get { return cvc.AllCampuses0().Single(gg => gg.Id == (CampusId ?? 0)).Value; }
                set { CampusId = cvc.AllCampuses0().Single(gg => gg.Value == value).Id; }
            }
            public int GenderId { get; set; }
            public string Gender
            {
                get { return cvc.GenderCodes().Single(gg => gg.Id == GenderId).Value; }
                set { GenderId = cvc.GenderCodes().Single(gg => gg.Value == value).Id; }
            }
            public int MaritalStatusId { get; set; }
            public string MaritalStatus
            {
                get { return cvc.MaritalStatusCodes().Single(gg => gg.Id == MaritalStatusId).Value; }
                set { MaritalStatusId = cvc.MaritalStatusCodes().Single(gg => gg.Value == value).Id; }
            }
            public string DOB { get; set; }
            public string CellPhone { get; set; }
            public string EmailAddress { get; set; }
            public string EmailAddress2 { get; set; }
            public string SuffixCode { get; set; }
            public string MiddleName { get; set; }
            public string HomePhone { get; set; }
            public string WorkPhone { get; set; }
            public string AddressLineOne { get; set; }
            public string AddressLineTwo { get; set; }
            public string CityName { get; set; }
            public string StateCode { get; set; }
            public string ZipCode { get; set; }
            public string Country { get; set; }
            public DateTime Created { get; set; }
            public int FamilyId { get; set; }
            public bool notdup { get; set; }
            public string MemberStatus { get; set; }
            public bool hasotherfamily { get; set; }
            public bool hasrelations { get; set; }
            public bool hasinvolvements { get; set; }
            public bool hasinprogressreg { get; set; }
        }


        public MergeModel(int pid1, int pid2)
        {
            //var db = Db;
            var q = from p in DbUtil.Db.People
                    where p.PeopleId == pid1 || p.PeopleId == pid2
                    orderby p.PeopleId == pid1 ? 1 : 2
                    let notdup = p.PeopleExtras.Any(ee => ee.Field == "notdup" && (ee.IntValue == pid1 || ee.IntValue == pid2))
                    let orelations = p.Family.RelatedFamilies1.Count() + p.Family.RelatedFamilies2.Count()
                    let oinvolvements = p.OrganizationMembers.Count()
                    let ofamily = p.Family.People.Count()
                    select new BasicInfo
                    {
                        PeopleId = p.PeopleId,
                        TitleCode = p.TitleCode,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        NickName = p.NickName,
                        AltName = p.AltName,
                        Maiden = p.MaidenName,
                        GenderId = p.GenderId,
                        CampusId = p.CampusId,
                        MaritalStatusId = p.MaritalStatusId,
                        DOB = p.DOB,
                        CellPhone = p.CellPhone,
                        HomePhone = p.Family.HomePhone,
                        WorkPhone = p.WorkPhone,
                        EmailAddress = p.EmailAddress,
                        EmailAddress2 = p.EmailAddress2,
                        SuffixCode = p.SuffixCode,
                        MiddleName = p.MiddleName,
                        AddressLineOne = p.Family.AddressLineOne,
                        AddressLineTwo = p.Family.AddressLineTwo,
                        CityName = p.Family.CityName,
                        StateCode = p.Family.StateCode,
                        ZipCode = p.Family.ZipCode,
                        Country = p.Family.CountryName,
                        Created = p.CreatedDate ?? DateTime.Parse("1/1/1980"),
                        FamilyId = p.FamilyId,
                        notdup = notdup,
                        hasinvolvements = oinvolvements > 0,
                        hasotherfamily = ofamily > 1,
                        hasrelations = orelations > 0,
                        MemberStatus = p.MemberStatus.Description,
                        hasinprogressreg = DbUtil.Db.ViewInProgressRegistrations.Any(vv => vv.PeopleId == p.PeopleId)
                    };
            pi = q.ToList();
            pi.Add(new BasicInfo());

            UseTitleCode = 1;
            UseFirstName = 1;
            UseLastName = 1;
            UseNickName = 1;
            UseAltName = 1;
            UseMaiden = 1;
            UseGenderId = 1;
            UseCampusId = 1;
            UseMaritalStatusId = 1;
            UseDOB = 1;
            UseCellPhone = 1;
            UseWorkPhone = 1;
            UseEmailAddress = 1;
            UseEmailAddress2 = 1;
            UseSuffixCode = 1;
            UseMiddleName = 1;
            UseHomePhone = 1;
            UseAddressLineOne = 1;
            UseAddressLineTwo = 1;
            UseCityName = 1;
            UseStateCode = 1;
            UseZipCode = 1;
            UseCountry = 1;
        }
        public MergeModel()
        {

        }
        public void Delete()
        {
            var p = DbUtil.Db.LoadPersonById(pi[0].PeopleId);
            DbUtil.Db.PurgePerson(p.PeopleId);
        }
        public void Update()
        {
            if (!Util.UserPeopleId.HasValue)
            {
                return;
            }

            var target = DbUtil.Db.LoadPersonById(pi[1].PeopleId);
            var psb = new List<ChangeDetail>();

            target.UpdateValue(psb, "TitleCode", pi[UseTitleCode].TitleCode);
            target.UpdateValue(psb, "FirstName", pi[UseFirstName].FirstName ?? "?");
            target.UpdateValue(psb, "LastName", pi[UseLastName].LastName ?? "?");
            target.UpdateValue(psb, "NickName", pi[UseNickName].NickName);
            target.UpdateValue(psb, "AltName", pi[UseAltName].AltName);
            target.UpdateValue(psb, "MaidenName", pi[UseMaiden].Maiden);
            target.UpdateValue(psb, "GenderId", pi[UseGenderId].GenderId);
            target.UpdateValue(psb, "CampusId", (pi[UseCampusId].CampusId ?? 0) == 0 ? (int?)null : (int?)pi[UseCampusId].CampusId);
            target.UpdateValue(psb, "MaritalStatusId", pi[UseMaritalStatusId].MaritalStatusId);
            target.UpdateValue(psb, "DOB", pi[UseDOB].DOB);
            target.UpdateValue(psb, "CellPhone", pi[UseCellPhone].CellPhone.GetDigits());
            target.UpdateValue(psb, "WorkPhone", pi[UseWorkPhone].WorkPhone.GetDigits());
            target.UpdateValue(psb, "EmailAddress", pi[UseEmailAddress].EmailAddress);
            target.UpdateValue(psb, "EmailAddress2", pi[UseEmailAddress2].EmailAddress2);
            if (!target.SendEmailAddress2.HasValue && pi[UseEmailAddress2].EmailAddress2.HasValue())
            {
                target.UpdateValue(psb, "SendEmailAddress2", true);
            }

            target.UpdateValue(psb, "SuffixCode", pi[UseSuffixCode].SuffixCode);
            target.UpdateValue(psb, "MiddleName", pi[UseMiddleName].MiddleName);

            var fsb = new List<ChangeDetail>();

            target.Family.UpdateValue(fsb, "HomePhone", pi[UseHomePhone].HomePhone.GetDigits());
            target.Family.UpdateValue(fsb, "AddressLineOne", pi[UseAddressLineOne].AddressLineOne);
            target.Family.UpdateValue(fsb, "AddressLineTwo", pi[UseAddressLineTwo].AddressLineTwo);
            target.Family.UpdateValue(fsb, "CityName", pi[UseCityName].CityName);
            target.Family.UpdateValue(fsb, "StateCode", pi[UseStateCode].StateCode);
            target.Family.UpdateValue(fsb, "ZipCode", pi[UseZipCode].ZipCode);
            target.Family.UpdateValue(fsb, "CountryName", pi[UseCountry].Country);

            target.LogChanges(DbUtil.Db, psb);
            target.Family.LogChanges(DbUtil.Db, fsb, target.PeopleId, Util.UserPeopleId.Value);
            DbUtil.Db.SubmitChanges();
        }
        public void Move()
        {
            var from = DbUtil.Db.LoadPersonById(pi[0].PeopleId);
            from.MovePersonStuff(DbUtil.Db, pi[1].PeopleId);
            if (DeleteDuplicate == true)
            {
                Delete();
            }
        }
        public SelectList GenderList()
        {
            var cv = new CodeValueModel();
            return new SelectList(cv.GenderCodes(), "Id", "Value");
        }
        public SelectList MaritalStatusList()
        {
            var cv = new CodeValueModel();
            return new SelectList(cv.MaritalStatusCodes(), "Id", "Value");
        }
        public SelectList CampusList()
        {
            var cv = new CodeValueModel();
            return new SelectList(cv.AllCampuses0(), "Id", "Value");
        }
        public void LogMerge(string action)
        {
            var mh = new MergeHistory
            {
                FromId = pi[0].PeopleId,
                ToId = pi[1].PeopleId,
                FromName = pi[0].person.Name,
                ToName = pi[1].person.Name,
                WhoId = Util.UserPeopleId,
                WhoName = Util.UserFullName,
                Action = $"{action}{(DeleteDuplicate ? " + Delete" : "")}",
                Dt = DateTime.Now
            };
            DbUtil.Db.MergeHistories.InsertOnSubmit(mh);
            DbUtil.LogActivity($"{action} from {pi[0].PeopleId} to {pi[1].PeopleId}");
            if (DeleteDuplicate == true)
            {
                DbUtil.LogActivity($"Deleting Record during Merge {pi[0].PeopleId} to {pi[1].PeopleId}");
            }
        }

        public void ToggleNotDuplicate()
        {
            if (IsDuplicate())
            {
                var dups = from ee in DbUtil.Db.PeopleExtras
                           where ee.Field == "notdup"
                           where ee.PeopleId == pi[0].PeopleId || ee.PeopleId == pi[1].PeopleId
                           select ee;
                DbUtil.Db.PeopleExtras.DeleteAllOnSubmit(dups);
            }
            else
            {
                pi[0].person.AddEditExtraInt("notdup", pi[1].PeopleId);
                pi[1].person.AddEditExtraInt("notdup", pi[0].PeopleId);
            }
            DbUtil.Db.SubmitChanges();
        }

        public bool IsDuplicate()
        {
            return pi[0].notdup || pi[1].notdup;
        }
    }
}
