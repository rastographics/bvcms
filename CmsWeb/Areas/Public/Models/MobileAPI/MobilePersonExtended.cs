using CmsData;
using ImageData;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.MobileAPI
{
    public class MobilePersonExtended
    {
        public int id = 0;
        public int familyID = 0;

        public int campusID = 0;

        public string title = "";
        public string first = "";
        public string middle = "";
        public string last = "";
        public string suffix = "";

        public string goesBy = "";
        public string alt = "";
        public string former = "";

        public int genderID = 0;
        public string genderText = "";

        public int maritalStatusID = 0;
        public string maritalStatusText = "";

        public int hasBirthday = 0;
        public DateTime birthdayDate;

        public int age = -1;

        public int hasWeddingDate = 0;
        public DateTime weddingDate;

        public int isDeceased = 0;
        public DateTime deceasedDate;

        public int statusID = 0;
        public string statusText = "";

        public string primaryEmail = "";
        public int primaryEmailActive = 0;

        public string altEmail = "";
        public int altEmailActive = 0;

        public string homePhone = "";
        public string workPhone = "";
        public string mobilePhone = "";

        public int doNotCall = 0;
        public int doNotMail = 0;
        public int doNotVisit = 0;
        public int doNotPublishPhones = 0;

        public string occupation = "";
        public string employer = "";
        public string school = "";
        public string grade = "";

        public MobilePersonAddress familyAddress = new MobilePersonAddress();
        public MobilePersonAddress personalAddress = new MobilePersonAddress();

        public List<MobileFamilyMember> familyMembers = new List<MobileFamilyMember>();
        public Dictionary<int, string> relatedFamily = new Dictionary<int, string>();

        public string pictureData = "";

        public int pictureX = 0;
        public int pictureY = 0;

        public string regFatherName = "";
        public string regMotherName = "";
        public string regShirtSize = "";

        public string regEmergencyName = "";
        public string regEmergencyPhone = "";

        public int regCustodyIssue = 0;
        public int regTransport = 0;
        public int regCoaching = 0;
        public int regMemberHere = 0;
        public int regActiveAnotherChurch = 0;

        public string regDoctor = "";
        public string regDoctorPhone = "";
        public string regHealthInsurance = "";
        public string regPolicyNumber = "";
        public string regAllergies = "";

        public int regTylenol = 0;
        public int regAdvil = 0;
        public int regRobitussin = 0;
        public int regMaalox = 0;

        public int electronicStatement = 0;
        public int statementType = 0;
        public int envelopeOption = 0;

        public MobilePersonExtended populate(Person p, bool includeFamily)
        {
            id = p.PeopleId;
            familyID = p.FamilyId;

            campusID = p.CampusId ?? 0;

            title = p.TitleCode ?? "";
            first = p.FirstName ?? "";
            middle = p.MiddleName ?? "";
            last = p.LastName ?? "";
            suffix = p.SuffixCode ?? "";

            goesBy = p.NickName ?? "";
            alt = p.AltName ?? "";
            former = p.MaidenName ?? "";

            genderID = p.GenderId;
            genderText = p.Gender.Description ?? "";

            maritalStatusID = p.MaritalStatusId;
            maritalStatusText = p.MaritalStatus.Description ?? "";

            age = p.Age ?? -1;

            if (p.DOB.Length > 0)
            {
                hasBirthday = 1;
                birthdayDate = new DateTime(p.BirthYr ?? 1800, p.BirthMonth ?? 0, p.BirthDay ?? 0);
            }

            if (p.WeddingDate.HasValue)
            {
                hasWeddingDate = 1;
                weddingDate = p.WeddingDate.Value;
            }

            if (p.DeceasedDate.HasValue)
            {
                isDeceased = 1;
                deceasedDate = p.DeceasedDate.Value;
            }

            primaryEmail = p.EmailAddress ?? "";
            primaryEmailActive = p.SendEmailAddress1 ?? false ? 1 : 0;

            altEmail = p.EmailAddress2 ?? "";
            altEmailActive = p.SendEmailAddress2 ?? false ? 1 : 0;

            homePhone = p.HomePhone ?? "";
            workPhone = p.WorkPhone ?? "";
            mobilePhone = p.CellPhone ?? "";

            doNotCall = p.DoNotCallFlag ? 1 : 0;
            doNotMail = p.DoNotMailFlag ? 1 : 0;
            doNotVisit = p.DoNotVisitFlag ? 1 : 0;
            doNotPublishPhones = p.DoNotPublishPhones ?? false ? 1 : 0;

            familyAddress.primary = p.AddressTypeId == 10 ? 1 : 0;
            familyAddress.address1 = p.Family.AddressLineOne ?? "";
            familyAddress.address2 = p.Family.AddressLineTwo ?? "";
            familyAddress.city = p.Family.CityName ?? "";
            familyAddress.state = p.Family.StateCode ?? "";
            familyAddress.zip = p.Family.ZipCode.FmtZip() ?? "";
            familyAddress.country = p.Family.CountryName ?? "";

            personalAddress.primary = p.AddressTypeId == 30 ? 1 : 0;
            personalAddress.address1 = p.AddressLineOne ?? "";
            personalAddress.address2 = p.AddressLineTwo ?? "";
            personalAddress.city = p.CityName ?? "";
            personalAddress.state = p.StateCode ?? "";
            personalAddress.zip = p.ZipCode.FmtZip() ?? "";
            personalAddress.country = p.CountryName ?? "";

            statusID = p.MemberStatusId;
            statusText = p.MemberStatus.Description ?? "";

            occupation = p.OccupationOther ?? "";
            employer = p.EmployerOther ?? "";
            school = p.SchoolOther ?? "";
            grade = p.Grade?.ToString() ?? "";

            electronicStatement = p.ElectronicStatement ?? false ? 1 : 0;
            statementType = p.ContributionOptionsId ?? 0;
            envelopeOption = p.EnvelopeOptionsId ?? 0;

            if (includeFamily)
            {
                List<Person> family = (from m in p.Family.People
                                       where m.PeopleId != p.PeopleId
                                       orderby m.PositionInFamilyId, m.PositionInFamilyId == 10 ? m.GenderId : 10, m.Age descending
                                       select m).ToList();

                foreach (Person familyMember in family)
                {
                    familyMembers.Add(new MobileFamilyMember(familyMember));
                }
            }

            pictureData = "";

            if (p.Picture != null)
            {
                Image image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == p.Picture.SmallId);

                if (image != null)
                {
                    pictureData = Convert.ToBase64String(image.Bits);
                    pictureX = p.Picture.X ?? 0;
                    pictureY = p.Picture.Y ?? 0;
                }
            }

            RecReg registration = p.GetRecReg();

            if (registration != null)
            {
                regFatherName = registration.Fname ?? "";
                regMotherName = registration.Mname ?? "";

                regCustodyIssue = p.CustodyIssue ?? false ? 1 : 0;
                regTransport = p.OkTransport ?? false ? 1 : 0;
                regCoaching = registration.Coaching ?? false ? 1 : 0;
                regMemberHere = registration.Member ?? false ? 1 : 0;
                regActiveAnotherChurch = registration.ActiveInAnotherChurch ?? false ? 1 : 0;

                regEmergencyName = registration.Emcontact ?? "";
                regEmergencyPhone = registration.Emphone ?? "";

                regDoctor = registration.Doctor ?? "";
                regDoctorPhone = registration.Docphone ?? "";
                regHealthInsurance = registration.Insurance ?? "";
                regPolicyNumber = registration.Policy ?? "";
                regAllergies = registration.MedicalDescription ?? "";

                regTylenol = registration.Tylenol ?? false ? 1 : 0;
                regAdvil = registration.Advil ?? false ? 1 : 0;
                regRobitussin = registration.Robitussin ?? false ? 1 : 0;
                regMaalox = registration.Maalox ?? false ? 1 : 0;
            }

            return this;
        }
    }
}
