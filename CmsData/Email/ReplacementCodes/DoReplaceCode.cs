using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private string DoReplaceCode(string code, EmailQueueTo emailqueueto = null)
        {
            if (code.StartsWith("<style"))
            {
                return code;
            }

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

                case "{amtdue}":
                    return pi?.AmountDue.ToString2("c") ?? code;

                case "{amtpaid}":
                    return pi?.AmountPaid.ToString2("c") ?? code;

                case "{missiontripamtdue}":
                    return (pi?.Amount ?? 0 - pi?.AmountPaidWithSupporters ?? 0).ToString("c") ?? code;

                case "{amount}":
                    if (pi != null)
                    {
                        return pi.Amount.ToString2("c");
                    }

                    break;

                case "{barcode}":
                    return $"<img src='{db.ServerLink("/Track/Barcode/" + person.PeopleId)}' />";

                case "{birthdate}":
                    return Person.FormatBirthday(person.BirthYr, person.BirthMonth, person.BirthDay, "not available", emailqueueto?.PeopleId);

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

                case "{createaccount}":
                    if (emailqueueto != null)
                    {
                        return CreateUserLinkReplacement(emailqueueto);
                    }

                    break;

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

                case "{emailhref}":
                    if (emailqueueto != null)
                    {
                        return db.ServerLink("/EmailView/" + emailqueueto.Id);
                    }

                    break;

                case "{first}":
                    if (person != null)
                    {
                        return person.PreferredName.Contains("?") || person.PreferredName.Contains("unknown", true) ? "" : person.PreferredName;
                    }

                    break;
                case "{firstorjoint}":
                    return FirstOrJoint();
                case "{fromemail}":
                    return from.Address;

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

                case "{nextmeetingtime}":
                    if (emailqueueto != null)
                    {
                        return NextMeetingDate(emailqueueto.OrgId, emailqueueto.PeopleId) ?? code;
                    }

                    break;
                case "{nextmeetingtime0}":
                    if (emailqueueto != null)
                    {
                        return NextMeetingDate0(emailqueueto.OrgId) ?? code;
                    }

                    break;

                case "{occupation}":
                    return person.OccupationOther;

                case "{orgname}":
                case "{org}":
                    return OrgInfos.Name(emailqueueto?.OrgId);
                case "{orgid}":
                    return OrgInfos.Name(emailqueueto?.OrgId);

                case "{orgmembercount}":
                    return OrgInfos.Count(emailqueueto?.OrgId);

                case "{paylink}":
                    if (pi != null && pi.PayLink.HasValue())
                    {
                        return $"<a href=\"{pi.PayLink}\">Click this link to make a payment and view your balance.</a>";
                    }

                    break;

                case "{peopleid}":
                    return person.PeopleId.ToString();

                case "{receivesms}":
                    return person.ReceiveSMS ? "Yes" : "No";

                case "{salutation}":
                    if (emailqueueto != null)
                    {
                        return db.GoerSupporters.Where(ee => ee.Id == emailqueueto.GoerSupportId)
                                .Select(ee => ee.Salutation).SingleOrDefault();
                    }

                    break;

                case "{state}":
                    return person.PrimaryState;

                case "{statementtype}":
                    return StatementTypeReplacement();

                case "{email}":
                case "{toemail}":
                    if (ListAddresses?.Count > 0)
                    {
                        return ListAddresses[0].Address;
                    }

                    return person.EmailAddress;

                case "{today}":
                    return Util.Today.ToShortDateString();

                case "{title}":
                    if (person.TitleCode.HasValue())
                    {
                        return person.TitleCode;
                    }

                    return person.ComputeTitle();

                case "{track}":
                    if (emailqueueto != null)
                    {
                        return emailqueueto.Guid.HasValue ?
                            $"<img src=\"{db.ServerLink("/Track/Key/" + emailqueueto.Guid.Value.GuidToQuerystring())}\" />"
                            : "";
                    }

                    break;

                case "{unsubscribe}":
                    if (emailqueueto != null)
                    {
                        return UnsubscribeReplacement(emailqueueto);
                    }

                    break;

                default:
                    var eq = emailqueueto ?? new EmailQueueTo()
                    {
                        PeopleId = person.PeopleId,
                        OrgId = db.CurrentSessionOrgId
                    };

                    if (AddSmallGroupRe.IsMatch(code))
                    {
                        return AddSmallGroupReplacement(code, eq);
                    }

                    if (PledgeRe.IsMatch(code))
                    {
                        return PledgeReplacement(code, eq);
                    }

                    if (PledgeFundRe.IsMatch(code))
                    {
                        return PledgeFundReplacement(code);
                    }

                    if (SettingRe.IsMatch(code))
                    {
                        return SettingReplacement(code);
                    }

                    if (SettingUrlRe.IsMatch(code))
                    {
                        return SettingUrlReplacement(db, code);
                    }

                    if (PythonDataRe.IsMatch(code))
                    {
                        return PythonDataReplacement(code);
                    }

                    if (ExtraValueRe.IsMatch(code))
                    {
                        return ExtraValueReplacement(code, eq);
                    }

                    if (FirstOrSubstituteRe.IsMatch(code))
                    {
                        return FirstOrSubstituteReplacement(code);
                    }

                    if (SubGroupsRe.IsMatch(code))
                    {
                        return SubGroupsReplacement(code, eq);
                    }

                    if (OrgExtraRe.IsMatch(code))
                    {
                        return OrgExtraReplacement(code, eq);
                    }

                    if (SmallGroupRe.IsMatch(code))
                    {
                        return SmallGroupReplacement(code, eq);
                    }

                    if (OrgMemberRe.IsMatch(code))
                    {
                        return OrgMemberReplacement(code, eq);
                    }

                    if (OrgBarCodeRe.IsMatch(code))
                    {
                        return OrgBarCodeReplacement(code, eq);
                    }

                    if (RegTextRe.IsMatch(code))
                    {
                        return RegTextReplacement(code, eq);
                    }

                    if (DateFormattedRe.IsMatch(code))
                    {
                        return DateFormattedReplacement(code);
                    }

                    if (UnlayerLinkRe.IsMatch(code))
                    {
                        return UnlayerLinkReplacement(code, eq);
                    }

                    if (RegisterLinkRe.IsMatch(code))
                    {
                        return RegisterLinkReplacement(code, eq);
                    }

                    if (RegisterLinkHrefRe.IsMatch(code))
                    {
                        return RegisterLinkHrefReplacement(code, eq);
                    }

                    if (RegisterTagRe.IsMatch(code))
                    {
                        return RegisterTagReplacement(code, eq);
                    }

                    if (RsvpLinkRe.IsMatch(code))
                    {
                        return RsvpLinkReplacement(code, eq);
                    }

                    if (SendLinkRe.IsMatch(code))
                    {
                        return SendLinkReplacement(code, eq);
                    }

                    if (SupportLinkRe.IsMatch(code))
                    {
                        return SupportLinkReplacement(code, eq);
                    }

                    if (MasterLinkRe.IsMatch(code))
                    {
                        return MasterLinkReplacement(code, eq);
                    }

                    if (VolReqLinkRe.IsMatch(code))
                    {
                        return VolReqLinkReplacement(code, eq);
                    }

                    if (VolSubLinkRe.IsMatch(code))
                    {
                        return VolSubLinkReplacement(code, eq);
                    }

                    if (VoteLinkRe.IsMatch(code))
                    {
                        return VoteLinkReplacement(code, eq);
                    }

                    if (DropFromOrgTagRe.IsMatch(code))
                    {
                        return DropFromOrgTagReplacement(code, eq);
                    }

                    if (SqlLookupRe.IsMatch(code))
                    {
                        return SqlLookupReplacement(code, eq);
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
