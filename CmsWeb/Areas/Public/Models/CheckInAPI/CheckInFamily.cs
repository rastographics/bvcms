using System.Collections.Generic;

namespace CmsWeb.CheckInAPI
{
    public class CheckInFamily
    {
        public int id = 0;
        public string name = "";

        public List<CheckInFamilyMember> members = new List<CheckInFamilyMember>();

        public CheckInFamily(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public void addMember(CmsData.View.CheckinFamilyMember newMember)
        {
            if (members.Count == 0)
            {
                members.Add(new CheckInFamilyMember(newMember));
            }
            else
            {
                foreach (var member in members)
                {
                    if (member.id == newMember.Id)
                    {
                        member.addOrg(newMember);
                        return;
                    }
                }

                members.Add(new CheckInFamilyMember(newMember));
            }
        }
    }
}