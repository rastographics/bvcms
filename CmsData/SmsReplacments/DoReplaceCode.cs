using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class TextReplacements
    {
        private string DoReplaceCode(string code, SMSItem item)
        {
            switch (code.ToLower())
            {
                case "{address}":
                    return person.PrimaryAddress;

                case "{address2}":
                    if (person.PrimaryAddress2.HasValue())
                    {
                        return "<br>" + person.PrimaryAddress2;
                    }

                    return "";

                case "{birthdate}":
                    return Person.FormatBirthday(person.BirthYr, person.BirthMonth, person.BirthDay, "not available", item?.PeopleID);

                case "{campus}":
                    return person.CampusId != null ? person.Campu.Description : $"No {Util2.CampusLabel} Specified";

                case "{cellphone}":
                    return person.CellPhone.HasValue() ? person.CellPhone.FmtFone() : "no cellphone on record";

                case "{city}":
                    return person.PrimaryCity;

                case "{csz}":
                    return Util.FormatCSZ(person.PrimaryCity, person.PrimaryState, person.PrimaryZip);

                case "{country}":
                    return person.PrimaryCountry;

                case "{church}":
                    return db.Setting("NameOfChurch", "No NameOfChurch in Settings");

                case "{churchphone}":
                    return db.Setting("ChurchPhone", "No ChurchPhone in Settings");

                case "{cmshost}":
                    return db.ServerLink();

                case "{dob}":
                    return person.DOB;

                case "{employer}":
                    return person.EmployerOther;

                case "{estatement}":
                    if (person.ElectronicStatement == true)
                    {
                        return "Online Electronic Statement Only";
                    }

                    return "Printed Statement in Addition to Online Option";

                case "{first}":
                    if (person != null)
                    {
                        return person.PreferredName.Contains("?") || person.PreferredName.Contains("unknown", true) ? "" : person.PreferredName;
                    }

                    break;
                case "{firstorjoint}":
                    return FirstOrJoint();

                case "{homephone}":
                    return person.HomePhone.HasValue() ? person.HomePhone.FmtFone() : "no homephone on record";

                case "{last}":
                    if (person != null)
                    {
                        return person.LastName;
                    }

                    break;

                case "{name}":
                    return person.Name.Contains("?") || person.Name.Contains("unknown", true) ? "" : person.Name;

                case "{occupation}":
                    return person.OccupationOther;

                case "{peopleid}":
                    return person.PeopleId.ToString();

                case "{state}":
                    return person.PrimaryState;

                case "{email}":
                    return person.EmailAddress;

                case "{today}":
                    return Util.Today.ToShortDateString();

                case "{title}":
                    if (person.TitleCode.HasValue())
                    {
                        return person.TitleCode;
                    }

                    return person.ComputeTitle();

                default:
                    if (UnlayerLinkRe.IsMatch(code))
                    {
                        return UnlayerLinkReplacement(code, item);
                    }
                    break;
            }
            return code; // nothing matched
        }

        private string FirstOrJoint()
        {
            var spousename = person.SpouseId.HasValue
                ? db.People.Where(p => p.PeopleId == person.SpouseId).Select(p => p.PreferredName).SingleOrDefault()
                : null;
            if (person.ContributionOptionsId == Codes.StatementOptionCode.Joint && spousename.HasValue())
            {
                return $"{person.PreferredName} & {spousename}";
            }

            return person.PreferredName;
        }
    }
}
