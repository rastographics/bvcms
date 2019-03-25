using CmsData;
using System;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.CheckInAPI
{
    public class CheckInOrganization
    {
        public int peopleID = 0;

        public int id;
        public string name;
        public string leader;
        public DateTime? hour;

        public int leadTime = 0;

        public string location;

        public DateTime? birthdayStart;
        public DateTime? birthdayEnd;

        public bool member = false;
        public bool checkedIn = false;

        public bool allowOverlap = false;

        public CheckInOrganization()
        {
        }

        public CheckInOrganization(CmsData.View.CheckinFamilyMember familyMember, int day, int tzOffset)
        {
            peopleID = familyMember.Id ?? 0;

            id = familyMember.OrgId.Value;
            name = familyMember.OrgName;
            leader = familyMember.Leader;
            member = familyMember.MemberVisitor == "M";

            checkedIn = familyMember.CheckedIn.Value;
            //labels = familyMember.NumLabels.Value;

            Organization orgInfo = DbUtil.Db.Organizations.SingleOrDefault(a => a.OrganizationId == familyMember.OrgId);

            if (orgInfo != null)
            {
                allowOverlap = orgInfo.AllowAttendOverlap;
            }

            if (familyMember.Hour.HasValue)
            {
                hour = familyMember.Hour.Value;

                var theirTime = DateTime.Now.AddHours(tzOffset);

                if (DateTime.Now.DayOfWeek.ToInt() != day)
                {
                    int dayDiff = day - DateTime.Now.DayOfWeek.ToInt();

                    if (dayDiff < 0)
                    {
                        theirTime = theirTime.AddDays(7 + dayDiff);
                    }
                    else
                    {
                        theirTime = theirTime.AddDays(dayDiff);
                    }
                }

                leadTime = (int)familyMember.Hour.Value.Subtract(theirTime).TotalMinutes;
            }
        }

        public void adjustLeadTime(int day, int tzOffset)
        {
            if (hour.HasValue)
            {
                var theirTime = DateTime.Now.AddHours(tzOffset);

                if (DateTime.Now.DayOfWeek.ToInt() != day)
                {
                    int dayDiff = day - DateTime.Now.DayOfWeek.ToInt();

                    if (dayDiff < 0)
                    {
                        theirTime = theirTime.AddDays(7 + dayDiff);
                    }
                    else
                    {
                        theirTime = theirTime.AddDays(dayDiff);
                    }
                }

                leadTime = (int)hour.Value.Subtract(theirTime).TotalMinutes;
            }
        }
    }
}
