using System;
using UtilityExtensions;

namespace CmsWeb.CheckInAPI
{
    public class CheckInFamilyMemberOrg
    {
        public int orgID = 0;
        public int personID = 0;

        public string orgName = "";
        public string orgLeader = "";

        public DateTime orgHour;
        public int orgLeadTime = 0;

        public bool orgMember = false;
        public bool checkedIn = false;

        //public int labels = 0;

        public CheckInFamilyMemberOrg(CmsData.View.CheckinFamilyMember member, int day, int tzOffset)
        {
            personID = member.Id ?? 0;

            orgID = member.OrgId.Value;
            orgName = member.OrgName;
            orgLeader = member.Leader;
            orgMember = member.MemberVisitor == "M";

            checkedIn = member.CheckedIn.Value;
            //labels = member.NumLabels.Value;

            if (member.Hour.HasValue)
            {
                orgHour = member.Hour.Value;

                var theirTime = DateTime.Now.AddHours(tzOffset);

                if (DateTime.Now.DayOfWeek.ToInt() != day)
                {
                    int dayDiff = day - DateTime.Now.DayOfWeek.ToInt();

                    if (dayDiff < 0)
                        theirTime = theirTime.AddDays(7 + dayDiff);
                    else
                        theirTime = theirTime.AddDays(dayDiff);
                }

                orgLeadTime = (int)member.Hour.Value.Subtract(theirTime).TotalMinutes;
            }
        }
    }
}