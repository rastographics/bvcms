using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;

namespace CmsWeb.MobileAPI
{
	public class MobilePostEditField
	{
		public int type = 0;
		public string value = "";

		public bool? getBoolValue( bool? defaultValue )
		{
			int tempInt;

			if( int.TryParse( value, out tempInt ) ) {
				return tempInt > 0;
			}

			return defaultValue;
		}

		public void updatePerson( Person person, List<ChangeDetail> personChangeList, List<ChangeDetail> familyChangeList )
		{
			switch( type ) {
				case Type.TITLE: {
					updateString( person, personChangeList, "TitleCode" );
					break;
				}

				case Type.FIRST: {
					updateString( person, personChangeList, "FirstName" );
					break;
				}

				case Type.MIDDLE: {
					updateString( person, personChangeList, "MiddleName" );
					break;
				}

				case Type.LAST: {
					updateString( person, personChangeList, "LastName" );
					break;
				}

				case Type.SUFFIX: {
					updateString( person, personChangeList, "SuffixCode" );
					break;
				}

				case Type.GOESBY: {
					updateString( person, personChangeList, "NickName" );
					break;
				}

				case Type.ALT_NAME: {
					updateString( person, personChangeList, "AltName" );
					break;
				}

				case Type.FORMER: {
					updateString( person, personChangeList, "MaidenName" );
					break;
				}

				case Type.GENDER: {
					updateInteger( person, personChangeList, "GenderId" );
					break;
				}

				case Type.MARITAL_STATUS: {
					updateInteger( person, personChangeList, "MaritalStatusId" );
					break;
				}

				case Type.BIRTH_DATE: {
					updateString( person, personChangeList, "DOB" );
					break;
				}

				case Type.WEDDING_DATE: {
					updateDate( person, personChangeList, "WeddingDate" );
					break;
				}

				case Type.DECEASED_DATE: {
					// updateDate( person, personChangeList, "DeceasedDate" );
					break;
				}

				case Type.FAMILY_ADDRESS1: {
					updateFamilyString( person.Family, familyChangeList, "AddressLineOne" );
					break;
				}

				case Type.FAMILY_ADDRESS2: {
					updateFamilyString( person.Family, familyChangeList, "AddressLineTwo" );
					break;
				}

				case Type.FAMILY_CITY: {
					updateFamilyString( person.Family, familyChangeList, "CityName" );
					break;
				}

				case Type.FAMILY_STATE: {
					updateFamilyString( person.Family, familyChangeList, "StateCode" );
					break;
				}

				case Type.FAMILY_ZIP: {
					updateFamilyString( person.Family, familyChangeList, "ZipCode" );
					break;
				}

				case Type.FAMILY_COUNTRY: {
					updateFamilyString( person.Family, familyChangeList, "CountryName" );
					break;
				}

				case Type.PERSONAL_ADDRESS1: {
					updateString( person, personChangeList, "AddressLineOne" );
					break;
				}

				case Type.PERSONAL_ADDRESS2: {
					updateString( person, personChangeList, "AddressLineTwo" );
					break;
				}

				case Type.PERSONAL_CITY: {
					updateString( person, personChangeList, "CityName" );
					break;
				}

				case Type.PERSONAL_STATE: {
					updateString( person, personChangeList, "StateCode" );
					break;
				}

				case Type.PERSONAL_ZIP: {
					updateString( person, personChangeList, "ZipCode" );
					break;
				}

				case Type.PERSONAL_COUNTRY: {
					updateString( person, personChangeList, "CountryName" );
					break;
				}

				case Type.PRIMARY_EMAIL: {
					updateString( person, personChangeList, "EmailAddress" );

					break;
				}

				case Type.ALT_EMAIL: {
					updateString( person, personChangeList, "EmailAddress2" );

					break;
				}

				case Type.HOME_PHONE: {
					updateString( person, personChangeList, "HomePhone" );

					break;
				}

				case Type.WORK_PHONE: {
					updateString( person, personChangeList, "WorkPhone" );

					break;
				}

				case Type.MOBILE_PHONE: {
					updateString( person, personChangeList, "CellPhone" );
					break;
				}

				case Type.DO_NOT_CALL: {
					updateBoolean( person, personChangeList, "DoNotCallFlag" );
					break;
				}

				case Type.DO_NOT_MAIL: {
					updateBoolean( person, personChangeList, "DoNotMailFlag" );
					break;
				}

				case Type.DO_NOT_VISIT: {
					updateBoolean( person, personChangeList, "DoNotVisitFlag" );
					break;
				}

				case Type.DO_NOT_PUBLISH_PHONE: {
					updateBoolean( person, personChangeList, "DoNotPublishPhones" );
					break;
				}

				case Type.FATHER_NAME: {
					person.GetRecReg().Fname = value;
					break;
				}

				case Type.MOTHER_NAME: {
					person.GetRecReg().Mname = value;
					break;
				}

				case Type.SHIRT_SIZE: {
					person.GetRecReg().ShirtSize = value;
					break;
				}

				case Type.EMERGENCY_NAME: {
					person.GetRecReg().Emcontact = value;
					break;
				}

				case Type.EMERGENCY_PHONE: {
					person.GetRecReg().Emphone = value;
					break;
				}

				case Type.INTEREST_COACHING: {
					person.GetRecReg().Coaching = getBoolValue( person.GetRecReg().Coaching );
					break;
				}

				case Type.DOCTOR_NAME: {
					person.GetRecReg().Doctor = value;
					break;
				}

				case Type.DOCTOR_PHONE: {
					person.GetRecReg().Docphone = value;
					break;
				}

				case Type.INSURANCE_NAME: {
					person.GetRecReg().Insurance = value;
					break;
				}

				case Type.INSURANCE_POLICY_NUMBER: {
					person.GetRecReg().Policy = value;
					break;
				}

				case Type.ALLERGIES: {
					person.GetRecReg().MedicalDescription = value;
					break;
				}

				case Type.CAN_GIVE_TYLENOL: {
					person.GetRecReg().Tylenol = getBoolValue( person.GetRecReg().Tylenol );
					break;
				}

				case Type.CAN_GIVE_ADVIL: {
					person.GetRecReg().Advil = getBoolValue( person.GetRecReg().Advil );
					break;
				}

				case Type.CAN_GIVE_ROBITUSSIN: {
					person.GetRecReg().Robitussin = getBoolValue( person.GetRecReg().Robitussin );
					break;
				}

				case Type.CAN_GIVE_MAALOX: {
					person.GetRecReg().Maalox = getBoolValue( person.GetRecReg().Maalox );
					break;
				}

				case Type.EMPLOYER: {
					updateString( person, personChangeList, "EmployerOther" );
					break;
				}

				case Type.OCCUPATION: {
					updateString( person, personChangeList, "OccupationOther" );
					break;
				}

				case Type.SCHOOL: {
					updateString( person, personChangeList, "SchoolOther" );
					break;
				}

				case Type.GRADE: {
					updateString( person, personChangeList, "Grade" );
					break;
				}

				case Type.ELECTRONIC_STATEMENTS:
					{
						updateBoolean(person, personChangeList, "ElectronicStatement");

						if (person.SpouseId.HasValue)
						{
							Person spouse = DbUtil.Db.People.FirstOrDefault(p => p.PeopleId == person.SpouseId);
							updateInteger(spouse, personChangeList, "ElectronicStatement");
						}

						break;
					}

				case Type.STATEMENT_TYPE:
					{
						updateInteger(person, personChangeList, "ContributionOptionsId");

						if (person.SpouseId.HasValue)
						{
							Person spouse = DbUtil.Db.People.FirstOrDefault(p => p.PeopleId == person.SpouseId);
							updateInteger(spouse, personChangeList, "ContributionOptionsId");
						}

						break;
					}

				case Type.ENVELOPE_OPTIONS:
					{
						updateInteger(person, personChangeList, "EnvelopeOptionsId");

						if (person.SpouseId.HasValue)
						{
							Person spouse = DbUtil.Db.People.FirstOrDefault(p => p.PeopleId == person.SpouseId);
							updateInteger(spouse, personChangeList, "EnvelopeOptionsId");
						}

						break;
					}

				default: {
					break;
				}
			}
		}

		private void updateFamilyString( Family family, List<ChangeDetail> changeList, string field )
		{
			family.UpdateValue( changeList, field, value );
		}

		private void updateString( Person person, List<ChangeDetail> changeList, string field )
		{
			person.UpdateValue( changeList, field, value );
		}

		private void updateInteger( Person person, List<ChangeDetail> changeList, string field )
		{
			int tempInt;

			if( int.TryParse( value, out tempInt ) ) {
				person.UpdateValue( changeList, field, tempInt );
			}
		}

		private void updateBoolean( Person person, List<ChangeDetail> changeList, string field )
		{
			int tempInt;

			if( int.TryParse( value, out tempInt ) ) {
				person.UpdateValue( changeList, field, tempInt > 0 );
			}
		}

		private void updateDate( Person person, List<ChangeDetail> changeList, string field )
		{
			DateTime tempDate;

			if( DateTime.TryParse( value, out tempDate ) ) {
				person.UpdateValue( changeList, field, tempDate );
			}
		}

		private abstract class Type
		{
			public const int TITLE = 1;
			public const int FIRST = 2;
			public const int MIDDLE = 3;
			public const int LAST = 4;
			public const int SUFFIX = 5;

			public const int GOESBY = 6;
			public const int ALT_NAME = 7;
			public const int FORMER = 8;

			public const int GENDER = 9;
			public const int MARITAL_STATUS = 10;

			public const int BIRTH_DATE = 11;
			public const int WEDDING_DATE = 12;
			public const int DECEASED_DATE = 13;

			public const int FAMILY_ADDRESS1 = 14;
			public const int FAMILY_ADDRESS2 = 15;
			public const int FAMILY_CITY = 16;
			public const int FAMILY_STATE = 17;
			public const int FAMILY_ZIP = 18;
			public const int FAMILY_COUNTRY = 19;

			public const int PERSONAL_ADDRESS1 = 20;
			public const int PERSONAL_ADDRESS2 = 21;
			public const int PERSONAL_CITY = 22;
			public const int PERSONAL_STATE = 23;
			public const int PERSONAL_ZIP = 24;
			public const int PERSONAL_COUNTRY = 25;

			public const int PRIMARY_EMAIL = 26;
			public const int ALT_EMAIL = 27;
			public const int HOME_PHONE = 28;
			public const int WORK_PHONE = 29;
			public const int MOBILE_PHONE = 30;

			public const int DO_NOT_CALL = 31;
			public const int DO_NOT_MAIL = 32;
			public const int DO_NOT_VISIT = 33;
			public const int DO_NOT_PUBLISH_PHONE = 34;

			public const int FATHER_NAME = 35;
			public const int MOTHER_NAME = 36;
			public const int SHIRT_SIZE = 37;

			public const int EMERGENCY_NAME = 38;
			public const int EMERGENCY_PHONE = 39;

			public const int INTEREST_COACHING = 40;

			public const int CUSTODY_ISSUE = 41;
			public const int TRANSPORT = 42;
			public const int MEMBER_HERE = 43;
			public const int ACTIVE_ANOHTER_CHURCH = 44;

			public const int DOCTOR_NAME = 45;
			public const int DOCTOR_PHONE = 46;
			public const int INSURANCE_NAME = 47;
			public const int INSURANCE_POLICY_NUMBER = 48;
			public const int ALLERGIES = 49;

			public const int CAN_GIVE_TYLENOL = 50;
			public const int CAN_GIVE_ADVIL = 51;
			public const int CAN_GIVE_ROBITUSSIN = 52;
			public const int CAN_GIVE_MAALOX = 53;

			public const int EMPLOYER = 54;
			public const int OCCUPATION = 55;
			public const int SCHOOL = 56;
			public const int GRADE = 57;

			public const int ELECTRONIC_STATEMENTS = 58;
			public const int STATEMENT_TYPE = 59;
			public const int ENVELOPE_OPTIONS = 60;
		}
	}
}