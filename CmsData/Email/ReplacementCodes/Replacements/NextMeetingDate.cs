using System;
using System.Linq;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private string NextMeetingDate(int? orgid, int pid)
        {
            if (!orgid.HasValue)
                return null;

            var mt = (from aa in db.Attends
                where aa.OrganizationId == orgid
                where aa.PeopleId == pid
                where AttendCommitmentCode.committed.Contains(aa.Commitment ?? 0)
                where aa.MeetingDate > Util.Now
                orderby aa.MeetingDate
                select aa.MeetingDate).FirstOrDefault();
            return mt == DateTime.MinValue ? "none" : mt.ToString("g");
        }

        private string NextMeetingDate0(int? orgid)
        {
            if (!orgid.HasValue)
                return null;

            var mt = (from mm in db.Meetings
                         where mm.OrganizationId == orgid
                         where mm.MeetingDate > Util.Now
                         orderby mm.MeetingDate
                         select mm.MeetingDate).FirstOrDefault() ?? DateTime.MinValue;
            return mt == DateTime.MinValue ? "none" : mt.ToString("g");
        }
    }
}
