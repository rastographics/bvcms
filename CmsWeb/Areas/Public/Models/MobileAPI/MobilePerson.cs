using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CmsData;
using ImageData;
using UtilityExtensions;

// ReSharper disable CheckNamespace
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global
namespace CmsWeb.MobileAPI
{
	public class MobilePerson
	{
		public int id;
		public int familyID;

		public string first = "";
		public string last = "";
		public string alt = "";
		public string suffix = "";

		public string gender = "";
		public int age;
		public string birthday = "";

		public string primaryAddress = "";

		public Dictionary<string, MobilePersonAddress> addresses = new Dictionary<string, MobilePersonAddress>();
		public List<MobileContact> emailPhone = new List<MobileContact>();
		public Dictionary<string, MobileFamilyMember> family = new Dictionary<string, MobileFamilyMember>();
		public Dictionary<string, string> relatives = new Dictionary<string, string>();

		public int status;
		public string statusText = "";

		public string picture = "";
		public int pictureX;
		public int pictureY;

		public int deceased;

		public MobilePerson populate( Person p, CMSDataContext cmsdb, CMSImageDataContext cmsidb, bool hideAgeYear = false, int hideAgeValue = 0 )
		{
			id = p.PeopleId;
			familyID = p.FamilyId;

			first = p.PreferredName ?? "";
			last = p.LastName ?? "";
			alt = p.AltName ?? "";
			suffix = p.SuffixCode;

			if( p.AddressTypeId == 10 ) {
				primaryAddress = "Family";
			} else {
				primaryAddress = "Personal";
			}

			MobilePersonAddress familyAddr = new MobilePersonAddress {
				address1 = p.Family.AddressLineOne ?? "",
				address2 = p.Family.AddressLineTwo ?? "",
				city = p.Family.CityName ?? "",
				state = p.Family.StateCode ?? "",
				zip = p.Family.ZipCode.FmtZip() ?? ""
			};

			addresses.Add( "Family", familyAddr );

			if( !string.IsNullOrEmpty( p.AddressLineOne ) ) {
				MobilePersonAddress personalAddr = new MobilePersonAddress {
					address1 = p.AddressLineOne ?? "",
					address2 = p.AddressLineTwo ?? "",
					city = p.CityName ?? "",
					state = p.StateCode ?? "",
					zip = p.ZipCode.FmtZip() ?? ""
				};

				addresses.Add( "Personal", personalAddr );
			}

			gender = p.Gender.Description;
			age = Person.AgeDisplay( p.Age, p.PeopleId ) ?? 0;

			if( hideAgeYear && age > hideAgeValue ) {
				if( p.BirthMonth > 0 && p.BirthDay > 0 ) {
					string month = DateTimeFormatInfo.CurrentInfo?.GetAbbreviatedMonthName( p.BirthMonth ?? 0 ) ?? "";

					birthday = $"{month} {p.BirthDay}";
				} else {
					birthday = "No Birthday Set";
				}
			} else {
				birthday = p.DOB.Length > 0 ? p.DOB : "No Birthday Set";
			}

			// Clear age because of settings (NoBirthYearRole and NoBirthYearOverAge)
			if( hideAgeYear && age > hideAgeValue ) {
				age = 0;
			}

			if( !string.IsNullOrEmpty( p.CellPhone ) ) {
				emailPhone.Add( new MobileContact( 1, "Cell", p.CellPhone.FmtFone() ) );
			}

			if( !string.IsNullOrEmpty( p.HomePhone ) ) {
				emailPhone.Add( new MobileContact( 1, "Home", p.HomePhone.FmtFone() ) );
			}

			if( !string.IsNullOrEmpty( p.WorkPhone ) ) {
				emailPhone.Add( new MobileContact( 1, "Work", p.WorkPhone.FmtFone() ) );
			}

			if( !string.IsNullOrEmpty( p.EmailAddress ) ) {
				emailPhone.Add( new MobileContact( 2, "EMail1", p.EmailAddress ) );
			}

			if( !string.IsNullOrEmpty( p.EmailAddress2 ) ) {
				emailPhone.Add( new MobileContact( 2, "EMail2", p.EmailAddress2 ) );
			}

			status = p.MemberStatusId;
			statusText = p.MemberStatus.Description;

			deceased = (p.IsDeceased ?? false) ? 1 : 0;

			foreach( Person m in p.Family.People.Where( mm => mm.PeopleId != p.PeopleId ) ) {
				MobileFamilyMember familyMember = new MobileFamilyMember {
					id = m.PeopleId.ToString(),
					name = m.Name,
					age = Person.AgeDisplay( m.Age, m.PeopleId ).ToString(),
					gender = m.Gender.Description,
					position = m.FamilyPosition.Description,
					deceased = m.Deceased
				};

				family.Add( m.PeopleId.ToString(), familyMember );
			}

			var q = from re in cmsdb.RelatedFamilies
						where re.FamilyId == p.FamilyId || re.RelatedFamilyId == p.FamilyId
						let rf = re.RelatedFamilyId == p.FamilyId ? re.RelatedFamily1 : re.RelatedFamily2
						select new {
							hohid = rf.HeadOfHouseholdId,
							description = re.FamilyRelationshipDesc
						};

			foreach( var rf in q ) {
				if( !relatives.ContainsKey( rf.hohid.ToString() ) ) {
					relatives.Add( rf.hohid.ToString(), rf.description );
				}
			}

			picture = "";

			if( p.Picture != null ) {
				Image image = cmsidb.Images.SingleOrDefault( i => i.Id == p.Picture.SmallId );

				if( image != null ) {
					picture = Convert.ToBase64String( image.Bits );
					pictureX = p.Picture.X ?? 0;
					pictureY = p.Picture.Y ?? 0;
				}
			}

			return this;
		}
	}
}