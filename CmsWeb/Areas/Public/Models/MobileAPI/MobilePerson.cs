using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.MobileAPI
{
	// <?xml version="1.0" encoding="utf-8"?>
	// <People count="1">
	// <Person peopleid="149" name="Steven Yarbrough" first="Steven" last="Yarbrough" address="4074 Fir Hill Dr E"
	// citystatezip="Lakeland, TN 38002" zip="38002" age="36" birthdate="9/27/1975"
	// homephone="" cellphone="  901-481-3443" workphone="" memberstatus="Just Added" email="steven@bvcms.com" haspicture="0" />
	// </People>

	// Test Class to compare JSON results to current API
	// IMPORTANT: Any class that will be converted to JSON needs default values or get/set methods or [DataMember] tags on each field
	public class MobilePerson
	{
		public int id = 0;

		public string first { get; set; }
		public string last { get; set; }

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
		public int deceased { get; set; }

		public MobilePerson populate(CmsData.Person p)
		{
			addresses = new Dictionary<string, MobilePersonAddress>();
			emailPhone = new List<MobileContact>();
			family = new Dictionary<string, MobileFamilyMember>();
			relatives = new Dictionary<string, string>();

			id = p.PeopleId;

			first = p.PreferredName ?? "";
			last = p.LastName ?? "";

			if (p.AddressTypeId == 10)
				primaryAddress = "Family";
			else
				primaryAddress = "Personal";

			var familyAddr = new MobilePersonAddress();
			familyAddr.address1 = p.Family.AddressLineOne ?? "";
			familyAddr.address2 = p.Family.AddressLineTwo ?? "";
			familyAddr.city = p.Family.CityName ?? "";
			familyAddr.state = p.Family.StateCode ?? "";
			familyAddr.zip = p.Family.ZipCode.FmtZip() ?? "";

			addresses.Add("Family", familyAddr);

			if (p.AddressLineOne != null && p.AddressLineOne.Length > 0)
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
			age = p.Age ?? 0;
			birthday = p.DOB.Length > 0 ? p.DOB : "No Birthday Set";

			if (p.CellPhone != null && p.CellPhone.Length > 0)
				emailPhone.Add(new MobileContact(1, "Cell", p.CellPhone.FmtFone()));

			if (p.HomePhone != null && p.HomePhone.Length > 0)
				emailPhone.Add(new MobileContact(1, "Home", p.HomePhone.FmtFone()));

			if (p.WorkPhone != null && p.WorkPhone.Length > 0)
				emailPhone.Add(new MobileContact(1, "Work", p.WorkPhone.FmtFone()));

			if (p.EmailAddress != null && p.EmailAddress.Length > 0)
				emailPhone.Add(new MobileContact(2, "EMail1", p.EmailAddress));

			if (p.EmailAddress2 != null && p.EmailAddress2.Length > 0)
				emailPhone.Add(new MobileContact(2, "EMail2", p.EmailAddress2));

			status = p.MemberStatusId;
			statusText = p.MemberStatus.Description;

			deceased = ((p.IsDeceased ?? false) ? 1 : 0);

			foreach (var m in p.Family.People.Where(mm => mm.PeopleId != p.PeopleId))
			{
				var familyMember = new MobileFamilyMember();
				familyMember.id = m.PeopleId.ToString();
				familyMember.name = m.Name;
				familyMember.age = m.Age.ToString();
				familyMember.gender = m.Gender.Description;
				familyMember.position = m.FamilyPosition.Description;

				family.Add(m.PeopleId.ToString(), familyMember);
			}

			var q = from re in DbUtil.Db.RelatedFamilies
					  where re.FamilyId == p.FamilyId || re.RelatedFamilyId == p.FamilyId
					  let rf = re.RelatedFamilyId == p.FamilyId ? re.RelatedFamily1 : re.RelatedFamily2
					  select new { hohid = rf.HeadOfHouseholdId, description = re.FamilyRelationshipDesc };

			foreach (var rf in q)
			{
				relatives.Add(rf.hohid.ToString(), rf.description);
			}

			picture = "";

			if (p.Picture != null)
			{
				var image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == p.Picture.SmallId);

				if (image != null)
				{
					picture = Convert.ToBase64String(image.Bits);
				}
			}

			return this;
		}
	}
}