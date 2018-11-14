using CmsData;
using System;
using System.Linq;
using System.Xml;

namespace CmsWeb.CheckInAPI
{
    public class CheckInPrintLabel
    {
        public int peopleID;
        public int orgID;

        public DateTime? hour;

        public void writeToXML(XmlWriter writer, string securityCode)
        {
            int labelCount = 1;
            bool requiresecuritylabel = false;
            bool member = false;

            DateTime dob;

            Person person = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == peopleID);
            Organization org = DbUtil.Db.Organizations.SingleOrDefault(o => o.OrganizationId == orgID);

            OrganizationMember orgMember = DbUtil.Db.OrganizationMembers.SingleOrDefault(om => om.PeopleId == person.PeopleId && om.OrganizationId == org.OrganizationId);

            if (orgMember != null)
            {
                // Members
                member = orgMember.MemberTypeId != 310 && orgMember.MemberTypeId != 311;

                if ((orgMember.MemberTypeId == 220 || orgMember.MemberTypeId == 230) && !(org.NoSecurityLabel ?? false))
                {
                    if (person?.Age != null)
                    {
                        if (person.Age.Value < 18)
                        {
                            requiresecuritylabel = true;
                        }
                        else
                        {
                            requiresecuritylabel = false;
                        }
                    }
                    else
                    {
                        if (person?.FamilyPosition.Id == 30)
                        {
                            requiresecuritylabel = true;
                        }
                        else
                        {
                            requiresecuritylabel = false;
                        }
                    }
                }

                if (orgMember.MemberTypeId == 220 || orgMember.MemberTypeId == 230)
                {
                    labelCount = org?.NumCheckInLabels ?? 1;
                }
                else
                {
                    labelCount = org?.NumWorkerCheckInLabels ?? 0;
                }
            }
            else
            {
                Attend attend = DbUtil.Db.Attends.SingleOrDefault(om => om.PeopleId == person.PeopleId && om.OrganizationId == org.OrganizationId && om.MeetingDate == hour);

                if (attend != null)
                {
                    labelCount = org.NumCheckInLabels ?? 1;

                    if (!(org.NoSecurityLabel ?? false))
                    {
                        requiresecuritylabel = true;
                    }
                }
                else
                {
                    return;
                }
            }

            string orgName = org.OrganizationName;
            string location = org.Location;

            if (person.BirthDate == null)
            {
                if (person.FamilyPosition.Id == 30)
                {
                    dob = DateTime.Now;
                }
                else
                {
                    dob = DateTime.MinValue;
                }
            }
            else
            {
                dob = person.BirthDate.Value;
            }

            string first = person.PreferredName;
            string last = person.LastName;

            bool transport = person.OkTransport ?? false;
            bool custody = person.CustodyIssue ?? false;

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
            writer.WriteElementString("dob", dob.ToString("s"));
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
