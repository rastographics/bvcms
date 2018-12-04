using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.MobileAPI
{
    public class MobilePostEditField
    {
        public int type = 0;
        public string value = "";

        public bool? getBoolValue(bool? defaultValue)
        {
            int tempInt;

            if (int.TryParse(value, out tempInt))
            {
                return tempInt > 0;
            }

            return defaultValue;
        }

        public void updatePerson(Person person, List<ChangeDetail> personChangeList, List<ChangeDetail> familyChangeList)
        {
            Type enumType = (Type)type;

            switch (enumType)
            {
                case Type.TITLE:
                    {
                        updateString(person, personChangeList, "TitleCode");
                        break;
                    }

                case Type.FIRST:
                    {
                        updateString(person, personChangeList, "FirstName");
                        break;
                    }

                case Type.MIDDLE:
                    {
                        updateString(person, personChangeList, "MiddleName");
                        break;
                    }

                case Type.LAST:
                    {
                        updateString(person, personChangeList, "LastName");
                        break;
                    }

                case Type.SUFFIX:
                    {
                        updateString(person, personChangeList, "SuffixCode");
                        break;
                    }

                case Type.GOESBY:
                    {
                        updateString(person, personChangeList, "NickName");
                        break;
                    }

                case Type.ALT_NAME:
                    {
                        updateString(person, personChangeList, "AltName");
                        break;
                    }

                case Type.FORMER:
                    {
                        updateString(person, personChangeList, "MaidenName");
                        break;
                    }

                case Type.GENDER:
                    {
                        updateInteger(person, personChangeList, "GenderId");
                        break;
                    }

                case Type.MARITAL_STATUS:
                    {
                        updateInteger(person, personChangeList, "MaritalStatusId");
                        break;
                    }

                case Type.BIRTH_DATE:
                    {
                        updateString(person, personChangeList, "DOB");
                        break;
                    }

                case Type.WEDDING_DATE:
                    {
                        updateDate(person, personChangeList, "WeddingDate");
                        break;
                    }

                case Type.DECEASED_DATE:
                    {
                        // updateDate( person, personChangeList, "DeceasedDate" );
                        break;
                    }

                case Type.FAMILY_PRIMARY:
                    {
                        value = "10"; // Damily Address Type

                        updateInteger(person, personChangeList, "AddressTypeId");
                        break;
                    }

                case Type.FAMILY_ADDRESS1:
                    {
                        updateFamilyString(person.Family, familyChangeList, "AddressLineOne");
                        break;
                    }

                case Type.FAMILY_ADDRESS2:
                    {
                        updateFamilyString(person.Family, familyChangeList, "AddressLineTwo");
                        break;
                    }

                case Type.FAMILY_CITY:
                    {
                        updateFamilyString(person.Family, familyChangeList, "CityName");
                        break;
                    }

                case Type.FAMILY_STATE:
                    {
                        updateFamilyString(person.Family, familyChangeList, "StateCode");
                        break;
                    }

                case Type.FAMILY_ZIP:
                    {
                        updateFamilyString(person.Family, familyChangeList, "ZipCode");
                        break;
                    }

                case Type.FAMILY_COUNTRY:
                    {
                        updateFamilyString(person.Family, familyChangeList, "CountryName");
                        break;
                    }

                case Type.PERSONAL_PRIMARY:
                    {
                        value = "30"; // Personal Address Type

                        updateInteger(person, personChangeList, "AddressTypeId");
                        break;
                    }

                case Type.PERSONAL_ADDRESS1:
                    {
                        updateString(person, personChangeList, "AddressLineOne");
                        break;
                    }

                case Type.PERSONAL_ADDRESS2:
                    {
                        updateString(person, personChangeList, "AddressLineTwo");
                        break;
                    }

                case Type.PERSONAL_CITY:
                    {
                        updateString(person, personChangeList, "CityName");
                        break;
                    }

                case Type.PERSONAL_STATE:
                    {
                        updateString(person, personChangeList, "StateCode");
                        break;
                    }

                case Type.PERSONAL_ZIP:
                    {
                        updateString(person, personChangeList, "ZipCode");
                        break;
                    }

                case Type.PERSONAL_COUNTRY:
                    {
                        updateString(person, personChangeList, "CountryName");
                        break;
                    }

                case Type.PRIMARY_EMAIL:
                    {
                        updateString(person, personChangeList, "EmailAddress");

                        break;
                    }

                case Type.PRIMARY_EMAIL_ACTIVE:
                    {
                        updateBoolean(person, personChangeList, "SendEmailAddress1");

                        break;
                    }

                case Type.ALT_EMAIL:
                    {
                        updateString(person, personChangeList, "EmailAddress2");

                        break;
                    }

                case Type.ALT_EMAIL_ACTIVE:
                    {
                        updateBoolean(person, personChangeList, "SendEmailAddress2");

                        break;
                    }

                case Type.HOME_PHONE:
                    {
                        updateString(person, personChangeList, "HomePhone");

                        break;
                    }

                case Type.WORK_PHONE:
                    {
                        updateString(person, personChangeList, "WorkPhone");

                        break;
                    }

                case Type.MOBILE_PHONE:
                    {
                        updateString(person, personChangeList, "CellPhone");
                        break;
                    }

                case Type.DO_NOT_CALL:
                    {
                        updateBoolean(person, personChangeList, "DoNotCallFlag");
                        break;
                    }

                case Type.DO_NOT_MAIL:
                    {
                        updateBoolean(person, personChangeList, "DoNotMailFlag");
                        break;
                    }

                case Type.DO_NOT_VISIT:
                    {
                        updateBoolean(person, personChangeList, "DoNotVisitFlag");
                        break;
                    }

                case Type.DO_NOT_PUBLISH_PHONE:
                    {
                        updateBoolean(person, personChangeList, "DoNotPublishPhones");
                        break;
                    }

                case Type.FATHER_NAME:
                    {
                        person.GetRecReg().Fname = value;
                        break;
                    }

                case Type.MOTHER_NAME:
                    {
                        person.GetRecReg().Mname = value;
                        break;
                    }

                case Type.SHIRT_SIZE:
                    {
                        person.GetRecReg().ShirtSize = value;
                        break;
                    }

                case Type.EMERGENCY_NAME:
                    {
                        person.GetRecReg().Emcontact = value;
                        break;
                    }

                case Type.EMERGENCY_PHONE:
                    {
                        person.GetRecReg().Emphone = value;
                        break;
                    }

                case Type.INTEREST_COACHING:
                    {
                        person.GetRecReg().Coaching = getBoolValue(person.GetRecReg().Coaching);
                        break;
                    }

                case Type.DOCTOR_NAME:
                    {
                        person.GetRecReg().Doctor = value;
                        break;
                    }

                case Type.DOCTOR_PHONE:
                    {
                        person.GetRecReg().Docphone = value;
                        break;
                    }

                case Type.INSURANCE_NAME:
                    {
                        person.GetRecReg().Insurance = value;
                        break;
                    }

                case Type.INSURANCE_POLICY_NUMBER:
                    {
                        person.GetRecReg().Policy = value;
                        break;
                    }

                case Type.ALLERGIES:
                    {
                        person.GetRecReg().MedicalDescription = value;
                        break;
                    }

                case Type.CAN_GIVE_TYLENOL:
                    {
                        person.GetRecReg().Tylenol = getBoolValue(person.GetRecReg().Tylenol);
                        break;
                    }

                case Type.CAN_GIVE_ADVIL:
                    {
                        person.GetRecReg().Advil = getBoolValue(person.GetRecReg().Advil);
                        break;
                    }

                case Type.CAN_GIVE_ROBITUSSIN:
                    {
                        person.GetRecReg().Robitussin = getBoolValue(person.GetRecReg().Robitussin);
                        break;
                    }

                case Type.CAN_GIVE_MAALOX:
                    {
                        person.GetRecReg().Maalox = getBoolValue(person.GetRecReg().Maalox);
                        break;
                    }

                case Type.EMPLOYER:
                    {
                        updateString(person, personChangeList, "EmployerOther");
                        break;
                    }

                case Type.OCCUPATION:
                    {
                        updateString(person, personChangeList, "OccupationOther");
                        break;
                    }

                case Type.SCHOOL:
                    {
                        updateString(person, personChangeList, "SchoolOther");
                        break;
                    }

                case Type.GRADE:
                    {
                        updateString(person, personChangeList, "Grade");
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

                case Type.CAMPUS:
                    {
                        updateInteger(person, personChangeList, "CampusId");
                        break;
                    }

                case Type.CUSTODY_ISSUE:
                    {
                        // Disabled in app
                        break;
                    }

                case Type.TRANSPORT:
                    {
                        // Disabled in app
                        break;
                    }

                case Type.MEMBER_HERE:
                    {
                        // Disabled in app
                        break;
                    }

                case Type.ACTIVE_ANOHTER_CHURCH:
                    {
                        // Disabled in app
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        private void updateFamilyString(Family family, List<ChangeDetail> changeList, string field)
        {
            family.UpdateValue(changeList, field, value);
        }

        private void updateString(Person person, List<ChangeDetail> changeList, string field)
        {
            person.UpdateValue(changeList, field, value);
        }

        private void updateInteger(Person person, List<ChangeDetail> changeList, string field)
        {
            int tempInt;

            if (int.TryParse(value, out tempInt))
            {
                person.UpdateValue(changeList, field, tempInt);
            }
        }

        private void updateBoolean(Person person, List<ChangeDetail> changeList, string field)
        {
            int tempInt;

            if (int.TryParse(value, out tempInt))
            {
                person.UpdateValue(changeList, field, tempInt > 0);
            }
        }

        private void updateDate(Person person, List<ChangeDetail> changeList, string field)
        {
            DateTime tempDate;

            if (DateTime.TryParse(value, out tempDate))
            {
                person.UpdateValue(changeList, field, tempDate);
            }
        }

        private enum Type
        {
            TITLE = 1,
            FIRST, // 2
            MIDDLE, // 3
            LAST, // 4
            SUFFIX, // 5

            GOESBY, // 6
            ALT_NAME, // 7
            FORMER, // 8

            GENDER, // 9
            MARITAL_STATUS, // 10

            BIRTH_DATE, // 11
            WEDDING_DATE, // 12
            DECEASED_DATE, // 13

            FAMILY_PRIMARY, // 14
            FAMILY_ADDRESS1, // 15
            FAMILY_ADDRESS2, // 16
            FAMILY_CITY, // 17
            FAMILY_STATE, // 18
            FAMILY_ZIP, // 19
            FAMILY_COUNTRY, // 20

            PERSONAL_PRIMARY, // 21
            PERSONAL_ADDRESS1, // 22
            PERSONAL_ADDRESS2, // 23
            PERSONAL_CITY, // 24
            PERSONAL_STATE, // 25
            PERSONAL_ZIP, // 26
            PERSONAL_COUNTRY, // 27

            PRIMARY_EMAIL, // 28
            PRIMARY_EMAIL_ACTIVE, // 29
            ALT_EMAIL, // 30
            ALT_EMAIL_ACTIVE, // 31
            HOME_PHONE, // 32
            WORK_PHONE, // 33
            MOBILE_PHONE, // 34

            DO_NOT_CALL, // 35
            DO_NOT_MAIL, // 36
            DO_NOT_VISIT, // 37
            DO_NOT_PUBLISH_PHONE, // 38

            FATHER_NAME, // 39
            MOTHER_NAME, // 40
            SHIRT_SIZE, // 41

            EMERGENCY_NAME, // 42
            EMERGENCY_PHONE, // 43

            INTEREST_COACHING, // 44

            CUSTODY_ISSUE, // 45
            TRANSPORT, // 46
            MEMBER_HERE, // 47
            ACTIVE_ANOHTER_CHURCH, // 48

            DOCTOR_NAME, // 49
            DOCTOR_PHONE, // 50
            INSURANCE_NAME, // 51
            INSURANCE_POLICY_NUMBER, // 52
            ALLERGIES, // 53

            CAN_GIVE_TYLENOL, // 54
            CAN_GIVE_ADVIL, // 55
            CAN_GIVE_ROBITUSSIN, // 56
            CAN_GIVE_MAALOX, // 57

            EMPLOYER, // 58
            OCCUPATION, // 59
            SCHOOL, // 60
            GRADE, // 61

            ELECTRONIC_STATEMENTS, // 62
            STATEMENT_TYPE, // 63
            ENVELOPE_OPTIONS, // 64

            CAMPUS // 65
        }
    }
}
