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
			Type enumType = (Type) type;

			switch( enumType ) {
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

				case Type.FAMILY_PRIMARY: {
					value = "10"; // Damily Address Type

					updateInteger( person, personChangeList, "AddressTypeId" );
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

				case Type.PERSONAL_PRIMARY: {
					value = "30"; // Personal Address Type

					updateInteger( person, personChangeList, "AddressTypeId" );
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

				case Type.PRIMARY_EMAIL_ACTIVE: {
					updateBoolean( person, personChangeList, "SendEmailAddress1" );

					break;
				}

				case Type.ALT_EMAIL: {
					updateString( person, personChangeList, "EmailAddress2" );

					break;
				}

				case Type.ALT_EMAIL_ACTIVE: {
					updateBoolean( person, personChangeList, "SendEmailAddress2" );

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

				case Type.ELECTRONIC_STATEMENTS: {
					updateBoolean( person, personChangeList, "ElectronicStatement" );

					if( person.SpouseId.HasValue ) {
						Person spouse = DbUtil.Db.People.FirstOrDefault( p => p.PeopleId == person.SpouseId );
						updateInteger( spouse, personChangeList, "ElectronicStatement" );
					}

					break;
				}

				case Type.STATEMENT_TYPE: {
					updateInteger( person, personChangeList, "ContributionOptionsId" );

					if( person.SpouseId.HasValue ) {
						Person spouse = DbUtil.Db.People.FirstOrDefault( p => p.PeopleId == person.SpouseId );
						updateInteger( spouse, personChangeList, "ContributionOptionsId" );
					}

					break;
				}

				case Type.ENVELOPE_OPTIONS: {
					updateInteger( person, personChangeList, "EnvelopeOptionsId" );

					if( person.SpouseId.HasValue ) {
						Person spouse = DbUtil.Db.People.FirstOrDefault( p => p.PeopleId == person.SpouseId );
						updateInteger( spouse, personChangeList, "EnvelopeOptionsId" );
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

		private enum Type
		{
			TITLE = 1,
			FIRST,
			MIDDLE,
			LAST,
			SUFFIX,

			GOESBY,
			ALT_NAME,
			FORMER,

			GENDER,
			MARITAL_STATUS,

			BIRTH_DATE,
			WEDDING_DATE,
			DECEASED_DATE,

			FAMILY_PRIMARY,
			FAMILY_ADDRESS1,
			FAMILY_ADDRESS2,
			FAMILY_CITY,
			FAMILY_STATE,
			FAMILY_ZIP,
			FAMILY_COUNTRY,

			PERSONAL_PRIMARY,
			PERSONAL_ADDRESS1,
			PERSONAL_ADDRESS2,
			PERSONAL_CITY,
			PERSONAL_STATE,
			PERSONAL_ZIP,
			PERSONAL_COUNTRY,

			PRIMARY_EMAIL,
			PRIMARY_EMAIL_ACTIVE,
			ALT_EMAIL,
			ALT_EMAIL_ACTIVE,
			HOME_PHONE,
			WORK_PHONE,
			MOBILE_PHONE,

			DO_NOT_CALL,
			DO_NOT_MAIL,
			DO_NOT_VISIT,
			DO_NOT_PUBLISH_PHONE,

			FATHER_NAME,
			MOTHER_NAME,
			SHIRT_SIZE,

			EMERGENCY_NAME,
			EMERGENCY_PHONE,

			INTEREST_COACHING,

			CUSTODY_ISSUE,
			TRANSPORT,
			MEMBER_HERE,
			ACTIVE_ANOHTER_CHURCH,

			DOCTOR_NAME,
			DOCTOR_PHONE,
			INSURANCE_NAME,
			INSURANCE_POLICY_NUMBER,
			ALLERGIES,

			CAN_GIVE_TYLENOL,
			CAN_GIVE_ADVIL,
			CAN_GIVE_ROBITUSSIN,
			CAN_GIVE_MAALOX,

			EMPLOYER,
			OCCUPATION,
			SCHOOL,
			GRADE,

			ELECTRONIC_STATEMENTS,
			STATEMENT_TYPE,
			ENVELOPE_OPTIONS
		}
	}
}