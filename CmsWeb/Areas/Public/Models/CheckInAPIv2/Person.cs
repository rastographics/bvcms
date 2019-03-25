using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using iTextSharp.text.pdf;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "ConvertToConstant.Global" )]
	[SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Global" )]
	[SuppressMessage( "ReSharper", "UnassignedField.Global" )]
	[SuppressMessage( "ReSharper", "ClassNeverInstantiated.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	public class Person
	{
		public bool edit = false;

		public int campus = 0;
		public int id = 0;
		public int familyID = 0;

		public string firstName = "";
		public string goesBy = "";
		public string altName = "";
		public string lastName = "";

		public int genderID = 0;

		public DateTime? birthday;

		public string primaryEmail = "";
		public string homePhone = "";
		public string workPhone = "";
		public string mobilePhone = "";

		public int maritalStatusID = 0;

		public string address = "";
		public string address2 = "";
		public string city = "";
		public string state = "";
		public string zipCode = "";

		public string country = "";

		public string church = "";

		public string allergies = "";

		public string emergencyName = "";
		public string emergencyPhone = "";

		public void clean()
		{
			firstName = firstName.Trim();
			goesBy = goesBy.Trim();
			lastName = lastName.Trim();

			mobilePhone = mobilePhone.GetDigits();
			homePhone = homePhone.GetDigits();
			primaryEmail = primaryEmail.Trim();

			address = address.Trim();
			address2 = address2.Trim();
			city = city.Trim();
			state = state.Trim();
			zipCode = zipCode.Trim();

			allergies = allergies.Trim();

			emergencyName = emergencyName.Trim();
			emergencyPhone = emergencyPhone.Trim();
		}

		public int getAge()
		{
			if( birthday == null ) return -1;

			DateTime today = DateTime.Now;

			int age = today.Year - birthday.Value.Year;

			if( today.Month < birthday.Value.Month || (today.Month == birthday.Value.Month && today.Day < birthday.Value.Day) )
				age--;

			return age;
		}

		public void populate( CmsData.Person p )
		{
			id = p.PeopleId;
			familyID = p.FamilyId;

			firstName = p.FirstName ?? "";
			lastName = p.LastName ?? "";

			goesBy = p.NickName;
			altName = p.AltName;

			genderID = p.GenderId;
			maritalStatusID = p.MaritalStatus.Id;

			birthday = p.BirthDate;

			primaryEmail = p.EmailAddress;
			homePhone = p.HomePhone.FmtFone();
			workPhone = p.WorkPhone.FmtFone();
			mobilePhone = p.CellPhone.FmtFone();

			address = p.Family.AddressLineOne ?? "";
			address2 = p.Family.AddressLineTwo ?? "";
			city = p.Family.CityName ?? "";
			state = p.Family.StateCode ?? "";
			zipCode = p.Family.ZipCode.FmtZip() ?? "";

			country = p.PrimaryCountry;

			church = p.OtherPreviousChurch;

			allergies = p.SetRecReg().MedicalDescription;

			emergencyName = p.SetRecReg().Emcontact;
			emergencyPhone = p.SetRecReg().Emphone;
		}

		public void fillFamily( CmsData.Family family )
		{
			family.HomePhone = homePhone;
			family.AddressLineOne = address;
			family.AddressLineTwo = address2;
			family.CityName = city;
			family.StateCode = state;
			family.ZipCode = zipCode;
			family.CountryName = country;
		}

		public void fillPerson( CmsData.Person person )
		{
			person.FirstName = firstName;
			person.LastName = lastName;
			person.NickName = goesBy;
			person.AltName = altName;

			if( birthday != null ) {
				person.BirthDay = birthday.Value.Day;
				person.BirthMonth = birthday.Value.Month;
				person.BirthYear = birthday.Value.Year;
			}

			person.GenderId = genderID;
			person.MaritalStatusId = maritalStatusID;

			person.FixTitle();

			person.EmailAddress = primaryEmail;
			person.HomePhone = homePhone;
			person.WorkPhone = workPhone;
			person.CellPhone = mobilePhone;

			person.SetRecReg().MedicalDescription = allergies;

			person.SetRecReg().Emcontact = emergencyName;
			person.SetRecReg().Emphone = emergencyPhone.Truncate( 50 );

			person.SetRecReg().ActiveInAnotherChurch = !string.IsNullOrEmpty( church );
			person.OtherPreviousChurch = church;
		}

		public int computePositionInFamily( SqlConnection db )
		{
			int age = getAge();
			bool married = maritalStatusID == CmsData.Codes.MaritalStatusCode.Married;

			// New Family
			if( familyID == 0 ) return age == -1 || age >= 18 ? CmsData.Codes.PositionInFamily.PrimaryAdult : CmsData.Codes.PositionInFamily.Child;

			// Existing Family
			DataTable table = new DataTable();

			const string qFamilies = @"SELECT SUM(PrimaryAdult) AS countPrimary,
														SUM(SecondaryAdult) AS countSecondary,
														SUM(Child) AS countChild,
														SUM(Married) AS countMarried,
														SUM(Single) AS countSingle
												FROM dbo.People
														  INNER JOIN lookup.FamilyPosition ON lookup.FamilyPosition.Id = dbo.People.PositionInFamilyId
														  INNER JOIN lookup.MaritalStatus ON lookup.MaritalStatus.Id = dbo.People.MaritalStatusId
												WHERE FamilyId = @familyID";

			using( SqlCommand cmd = new SqlCommand( qFamilies, db ) ) {
				SqlParameter parameter = new SqlParameter( "familyID", familyID );

				cmd.Parameters.Add( parameter );

				SqlDataAdapter adapter = new SqlDataAdapter( cmd );
				adapter.Fill( table );
			}

			int countPrimary = (int) table.Rows[0]["countPrimary"];
			int countMarried = (int) table.Rows[0]["countMarried"];

			switch( countPrimary ) {
				case 2: // Two Primary Adults
					switch( married ) {
						case true: // Married
							return age == -1 || age >= 18 ? CmsData.Codes.PositionInFamily.SecondaryAdult : CmsData.Codes.PositionInFamily.Child;

						case false: // Not Married
							return age == -1 || age < 18 ? CmsData.Codes.PositionInFamily.Child : CmsData.Codes.PositionInFamily.SecondaryAdult;
					}

					break;

				case 1: // One Primary Adults
					switch( countMarried > 0 ) {
						case true: // One Primary Married
							switch( married ) {
								case true: // Married
									return age == -1 || age >= 18 ? CmsData.Codes.PositionInFamily.PrimaryAdult : CmsData.Codes.PositionInFamily.Child;

								case false: // Not Married
									return age == -1 || age < 18 ? CmsData.Codes.PositionInFamily.Child : CmsData.Codes.PositionInFamily.SecondaryAdult;
							}

							break;

						case false: // One Primary Single
							switch( married ) {
								case true: // Married
									return age == -1 || age >= 18 ? CmsData.Codes.PositionInFamily.PrimaryAdult : CmsData.Codes.PositionInFamily.Child;

								case false: // Not Married
									return age == -1 || age < 18 ? CmsData.Codes.PositionInFamily.Child : CmsData.Codes.PositionInFamily.SecondaryAdult;
							}

							break;
					}

					break;
			}

			return age == -1 || age >= 18 ? CmsData.Codes.PositionInFamily.PrimaryAdult : CmsData.Codes.PositionInFamily.Child;
		}
	}
}