using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.MobileAPI
{
    public class MobilePerson
    {
        public int id = 0;
        public int familyID = 0;

        public string first { get; set; }
        public string last { get; set; }
        public string alt { get; set; }
        public string suffix { get; set; }

        public string gender { get; set; }
        public int age { get; set; }
        public string birthday { get; set; }

        public string primaryAddress { get; set; }

        public Dictionary<string, MobilePersonAddress> addresses { get; set; }
        public List<MobileContact> emailPhone { get; set; }
        public Dictionary<string, MobileFamilyMember> family { get; set; }
        public Dictionary<string, string> relatives { get; set; }

        public int status { get; set; }
        public string statusText { get; set; }

        public string picture { get; set; }
        public int pictureX = 0;
        public int pictureY = 0;

        public int deceased { get; set; }

        public MobilePerson populate(CmsData.Person p)
        {
            addresses = new Dictionary<string, MobilePersonAddress>();
            emailPhone = new List<MobileContact>();
            family = new Dictionary<string, MobileFamilyMember>();
            relatives = new Dictionary<string, string>();

            id = p.PeopleId;
            familyID = p.FamilyId;

            first = p.PreferredName ?? "";
            last = p.LastName ?? "";
            alt = p.AltName ?? "";
            suffix = p.SuffixCode;

            if (p.AddressTypeId == 10)
            {
                primaryAddress = "Family";
            }
            else
            {
                primaryAddress = "Personal";
            }

            var familyAddr = new MobilePersonAddress();
            familyAddr.address1 = p.Family.AddressLineOne ?? "";
            familyAddr.address2 = p.Family.AddressLineTwo ?? "";
            familyAddr.city = p.Family.CityName ?? "";
            familyAddr.state = p.Family.StateCode ?? "";
            familyAddr.zip = p.Family.ZipCode.FmtZip() ?? "";

            addresses.Add("Family", familyAddr);

            if (!string.IsNullOrEmpty(p.AddressLineOne))
            {
                var personalAddr = new MobilePersonAddress();
                personalAddr.address1 = p.AddressLineOne ?? "";
                personalAddr.address2 = p.AddressLineTwo ?? "";
                personalAddr.city = p.CityName ?? "";
                personalAddr.state = p.StateCode ?? "";
                personalAddr.zip = p.ZipCode.FmtZip() ?? "";

                addresses.Add("Personal", personalAddr);
            }

            gender = p.Gender.Description;
            age = Person.AgeDisplay(p.Age, p.PeopleId) ?? 0;
            birthday = p.DOB.Length > 0 ? p.DOB : "No Birthday Set";

            if (!string.IsNullOrEmpty(p.CellPhone))
            {
                emailPhone.Add(new MobileContact(1, "Cell", p.CellPhone.FmtFone()));
            }

            if (!string.IsNullOrEmpty(p.HomePhone))
            {
                emailPhone.Add(new MobileContact(1, "Home", p.HomePhone.FmtFone()));
            }

            if (!string.IsNullOrEmpty(p.WorkPhone))
            {
                emailPhone.Add(new MobileContact(1, "Work", p.WorkPhone.FmtFone()));
            }

            if (!string.IsNullOrEmpty(p.EmailAddress))
            {
                emailPhone.Add(new MobileContact(2, "EMail1", p.EmailAddress));
            }

            if (!string.IsNullOrEmpty(p.EmailAddress2))
            {
                emailPhone.Add(new MobileContact(2, "EMail2", p.EmailAddress2));
            }

            status = p.MemberStatusId;
            statusText = p.MemberStatus.Description;

            deceased = ((p.IsDeceased ?? false) ? 1 : 0);

            foreach (var m in p.Family.People.Where(mm => mm.PeopleId != p.PeopleId))
            {
                var familyMember = new MobileFamilyMember();
                familyMember.id = m.PeopleId.ToString();
                familyMember.name = m.Name;
                familyMember.age = Person.AgeDisplay(m.Age, m.PeopleId).ToString();
                familyMember.gender = m.Gender.Description;
                familyMember.position = m.FamilyPosition.Description;
                familyMember.deceased = m.Deceased;

                family.Add(m.PeopleId.ToString(), familyMember);
            }

            var q = from re in DbUtil.Db.RelatedFamilies
                    where re.FamilyId == p.FamilyId || re.RelatedFamilyId == p.FamilyId
                    let rf = re.RelatedFamilyId == p.FamilyId ? re.RelatedFamily1 : re.RelatedFamily2
                    select new { hohid = rf.HeadOfHouseholdId, description = re.FamilyRelationshipDesc };

            foreach (var rf in q)
            {
                if (!relatives.ContainsKey(rf.hohid.ToString()))
                {
                    relatives.Add(rf.hohid.ToString(), rf.description);
                }
            }

            picture = "";

            if (p.Picture != null)
            {
                var image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == p.Picture.SmallId);

                if (image != null)
                {
                    picture = Convert.ToBase64String(image.Bits);
                    pictureX = p.Picture.X ?? 0;
                    pictureY = p.Picture.Y ?? 0;
                }
            }

            return this;
        }
    }
}
