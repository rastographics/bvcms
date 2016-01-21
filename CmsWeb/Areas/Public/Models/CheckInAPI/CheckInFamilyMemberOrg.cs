using System;

namespace CmsWeb.CheckInAPI
{
    public class CheckInFamilyMemberOrg
    {
        public int orgID = 0;
        public int personID = 0;

        public string orgName = "";
        public string orgLeader = "";

        public DateTime orgHour;

        public bool orgMember = false;
        public bool checkedIn = false;

        public int labels = 0;

        public CheckInFamilyMemberOrg(CmsData.View.CheckinFamilyMember member)
        {
            personID = member.Id ?? 0;

            orgID = member.OrgId.Value;
            orgName = member.OrgName;
            orgLeader = member.Leader;
            orgMember = member.MemberVisitor == "M";

            checkedIn = member.CheckedIn.Value;
            labels = member.NumLabels.Value;

            if (member.Hour.HasValue)
                orgHour = member.Hour.Value;
        }
    }
}