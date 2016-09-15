using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using UtilityExtensions;
using CmsWeb.Areas.Reports.Models;
using System.Collections;

namespace CmsWeb.Areas.Org.Models
{
    public class MeetingModel
    {
        public Meeting meeting;
        public Organization org;

        public bool showall { get; set; }
        public bool currmembers { get; set; }
        public bool showregister { get; set; }
        public bool showregistered { get; set; }
        public bool sortbyname { get; set; }
        public bool showlarge { get; set; }
        public bool CommitsOnly { get; set; }

        public MeetingModel(int id)
        {
            var i = (from m in DbUtil.Db.Meetings
                     where m.MeetingId == id
                     select new
                                {
                                    org = m.Organization,
                                    m,
                                }).SingleOrDefault();
            if (i == null)
                return;
            meeting = i.m;
            org = i.org;
        }

        public string RegularMeetingHeadCountSetting()
        {
            return DbUtil.Db.Setting("RegularMeetingHeadCount", "enable");
        }

        public List<RollsheetModel.AttendInfo> Attends(bool sorted = false, string highlight = null)
        {
            if (!meeting.MeetingDate.HasValue)
                throw new Exception("Meeting should have a date");
            var rm = highlight == null 
                ? RollsheetModel.RollList(meeting.MeetingId, meeting.OrganizationId, meeting.MeetingDate.Value, sorted, currmembers, CommitsOnly)
                : RollsheetModel.RollListHighlight(meeting.MeetingId, meeting.OrganizationId, meeting.MeetingDate.Value, sorted, currmembers, highlight, CommitsOnly);
            return rm;
        }
        public IEnumerable<RollsheetModel.AttendInfo> VisitAttends(bool sorted = false)
        {
            var q = RollsheetModel.RollList(meeting.MeetingId, meeting.OrganizationId, meeting.MeetingDate.Value, sorted);
            return q.Where(vv => !vv.Member && vv.Attended);
        }
        public string AttendCreditType()
        {
            if (meeting.AttendCredit == null)
                return "Every Meeting";
            return meeting.AttendCredit.Description;
        }
        public bool HasRegistered()
        {
            return meeting.Attends.Any(aa => aa.Commitment != null);
        }
        public static IEnumerable AttendCommitments()
        {
            var q = CmsData.Codes.AttendCommitmentCode.GetCodePairs();
            return q.ToDictionary(k => k.Key.ToString(), v => v.Value);
        }
        public class NamesInfo
        {
            public string Name { get; set; }
            public string Addr { get; set; }
            public int Pid { get; set; }
        }
        public static IEnumerable<NamesInfo> Names(string text, int limit)
        {
            string First, Last;
            var qp = DbUtil.Db.People.AsQueryable();
			if (Util2.OrgLeadersOnly)
				qp = DbUtil.Db.OrgLeadersOnlyTag2().People(DbUtil.Db);
            qp = from p in qp
                 where p.DeceasedDate == null
                 select p;

			Util.NameSplit(text, out First, out Last);

			var hasfirst = First.HasValue();
            if (text.AllDigits())
            {
                string phone = null;
                if (text.HasValue() && text.AllDigits() && text.Length == 7)
                    phone = text;
                if (phone.HasValue())
                {
                    var id = Last.ToInt();
                    qp = from p in qp
                         where
                             p.PeopleId == id
                             || p.CellPhone.Contains(phone)
                             || p.Family.HomePhone.Contains(phone)
                             || p.WorkPhone.Contains(phone)
                         orderby p.Name2
                         select p;
                }
                else
                {
                    var id = Last.ToInt();
                    qp = from p in qp
                         where p.PeopleId == id
                         orderby p.Name2
                         select p;
                }
            }
            else
            {
                var id = Last.ToInt();
                qp = from p in qp
                     where
                         (
                             (p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last)
                              || p.LastName.StartsWith(text) || p.MaidenName.StartsWith(text))
                             &&
                             (!hasfirst || p.FirstName.StartsWith(First) || p.NickName.StartsWith(First) ||
                              p.MiddleName.StartsWith(First)
                              || p.LastName.StartsWith(text) || p.MaidenName.StartsWith(text))
                         )
                         || p.PeopleId == id
                     orderby p.Name2
                     select p;
            }

            var r = from p in qp
                    let age = p.Age.HasValue ? " (" + p.Age + ")" : ""
                    orderby p.Name2
                    select new NamesInfo()
                               {
                                   Pid = p.PeopleId,
                                   Name = p.Name2 + age,
                                   Addr = p.PrimaryAddress ?? "",
                               };
            return r.Take(limit);
        }
    }
}