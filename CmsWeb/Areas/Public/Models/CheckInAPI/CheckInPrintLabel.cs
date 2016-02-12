using CmsData;
using System;
using System.Linq;
using System.Xml;

namespace CmsWeb.Areas.Public.Models.CheckInAPI
{
    public class CheckInPrintLabel
    {
        public int peopleID;
        public int orgID;

        public DateTime? hour;

        public void writeToXML(XmlWriter writer, string securityCode)
        {
            Person person = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == peopleID);
            Organization org = DbUtil.Db.Organizations.SingleOrDefault(o => o.OrganizationId == orgID);
            OrganizationMember orgMember = DbUtil.Db.OrganizationMembers.SingleOrDefault(om => om.PeopleId == person.PeopleId && om.OrganizationId == org.OrganizationId);

            int labelCount = org.NumCheckInLabels ?? 0;

            string orgName = org.OrganizationName;
            string location = org.Location;

            bool requiresecuritylabel = (orgMember != null && (orgMember.MemberTypeId == 220 || orgMember.MemberTypeId == 230)) && (person.Age.Value < 18) && org.NoSecurityLabel.Value;

            string first = person.PreferredName;
            string last = person.LastName;

            DateTime? dob = person.BirthDate;

            bool transport = person.OkTransport ?? false;
            bool custody = person.CustodyIssue ?? false;
            bool member = orgMember != null && orgMember.MemberTypeId != 230 && orgMember.MemberTypeId != 310 && orgMember.MemberTypeId != 311;

            string allergies = person.GetRecReg().MedicalDescription;

            var parents = "";

            if (person.PositionInFamilyId == 30)
            {
                if (person.Family.HeadOfHouseholdId != null)
                {
                    parents = person.Family.HeadOfHousehold.FirstName;

                    if (person.Family.HeadOfHouseholdSpouseId != null)
                    {
                        parents += " & " + person.Family.HeadOfHouseholdSpouse.FirstName;
                    }
                }
                else if (person.Family.HeadOfHouseholdSpouseId != null)
                {
                    parents = person.Family.HeadOfHouseholdSpouse.FirstName;
                }
            }

            writer.WriteStartElement("LabelInfo");

            writer.WriteElementString("n", labelCount.ToString());
            writer.WriteElementString("dob", dob != null ? dob.Value.ToString("s") : "");
            writer.WriteElementString("location", location);
            writer.WriteElementString("allergies", allergies);
            writer.WriteElementString("org", orgName);
            writer.WriteElementString("hour", hour != null ? hour.Value.ToString("s") : "");
            writer.WriteElementString("pid", person.PeopleId.ToString());
            writer.WriteElementString("mv", member ? "M" : "G");
            writer.WriteElementString("first", first);
            writer.WriteElementString("last", last);
            writer.WriteElementString("transport", transport.ToString().ToLower());
            writer.WriteElementString("custody", custody.ToString().ToLower());
            writer.WriteElementString("requiressecuritylabel", requiresecuritylabel.ToString().ToLower());
            writer.WriteElementString("securitycode", securityCode);
            writer.WriteElementString("parents", parents);

            writer.WriteEndElement();
        }
    }
}