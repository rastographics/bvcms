using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Web;
using UtilityExtensions;
using System.IO;
using System.Net.Mail;
using System.Web.Configuration;
using System.Data.SqlClient;
using CmsData.Codes;

namespace CmsData
{
	public partial class Attend
	{
		public static int[] VisitAttendTypes = new int[] 
        { 
            AttendTypeCode.VisitingMember, 
            AttendTypeCode.RecentVisitor, 
            AttendTypeCode.NewVisitor 
        };
		public static string RecordAttendance(int PeopleId, int MeetingId, bool attended)
		{
			return RecordAttendance(DbUtil.Db, PeopleId, MeetingId, attended);
		}
		public static string RecordAttendance(CMSDataContext Db, int PeopleId, int MeetingId, bool attended)
		{
			int ntries = 6;
			while (true)
			{
				try
				{
        			return Db.RecordAttendance(MeetingId, PeopleId, attended);
				}
				catch (SqlException ex)
				{
					if (ex.Number == 1205)
						if (--ntries > 0)
						{
							System.Threading.Thread.Sleep(500);
							continue;
						}
					throw;
				}
			}
		}
		public static int? MarkRegistered(CMSDataContext Db, int OrgId, int PeopleId, DateTime MeetingDate, int? CommitId, bool AvoidRegrets = false)
		{
			if (CommitId == null)
			{
				var m = Db.Meetings.SingleOrDefault(mm => mm.OrganizationId == OrgId && mm.MeetingDate == MeetingDate);
				if (m == null)
					return null;
			}
			var mid = Db.CreateMeeting(OrgId, MeetingDate);
			MarkRegistered(Db, PeopleId, mid, CommitId, AvoidRegrets);
		    return mid;
		}
		public static void MarkRegistered(CMSDataContext Db, int PeopleId, int MeetingId, int? CommitId, bool AvoidRegrets = false)
		{
			var i = (from m in Db.Meetings
					 where m.MeetingId == MeetingId
					 let om = Db.OrganizationMembers.SingleOrDefault(mm => mm.OrganizationId == m.OrganizationId && mm.PeopleId == PeopleId)
					 let a = Db.Attends.SingleOrDefault(aa => aa.PeopleId == PeopleId && aa.MeetingId == MeetingId)
					 select new
					 {
						 a,
						 m.OrganizationId,
						 MeetingDate = m.MeetingDate.Value,
						 MemberTypeId = om == null ? CmsData.Codes.MemberTypeCode.Visitor : om.MemberTypeId,
					 }).Single();
			if (i.a == null)
			{
				var a = new Attend
				{
					OrganizationId = i.OrganizationId,
					PeopleId = PeopleId,
					MeetingDate = i.MeetingDate,
					AttendanceFlag = false,
					CreatedDate = Util.Now,
					CreatedBy = Util.UserId1,
					AttendanceTypeId = null,
					BFCAttendance = null,
					OtherAttends = 0,
					MemberTypeId = i.MemberTypeId,
					OtherOrgId = 0,
					Commitment = CommitId,
					MeetingId = MeetingId
				};
				Db.Attends.InsertOnSubmit(a);
			}
			else if (AvoidRegrets == false || i.a.Commitment == 1 || i.a.Commitment == null)
				i.a.Commitment = CommitId;
			Db.SubmitChanges();
		}
		public static void RecordAttend(CMSDataContext Db, int PeopleId, int OrgId, bool Present, DateTime dt)
		{
			var q = from o in Db.Organizations
					where o.OrganizationId == OrgId
					let p = Db.People.Single(pp => pp.PeopleId == PeopleId)
					select new
					{
						o.Location,
						OrgEntryPoint = o.EntryPointId,
						p.EntryPointId,
					};
			var info = q.Single();
			if (info.EntryPointId == null)
			{
				var p = Db.LoadPersonById(PeopleId);
			    if (info.OrgEntryPoint > 0)
			    {
			        p.EntryPointId = info.OrgEntryPoint;
			        Db.SubmitChanges();
			    }
			}
			int ntries = 6;
			while (true)
			{
				try
				{
        		    Db.RecordAttendance(OrgId, PeopleId, dt, Present, info.Location);
				    return;
				}
				catch (SqlException ex)
				{
					if (ex.Number == 1205)
						if (--ntries > 0)
						{
							System.Threading.Thread.Sleep(500);
							continue;
						}
					throw;
				}
			}
		}
		public static int AddAttend(CMSDataContext Db, int PeopleId, int OrgId, bool Present, DateTime dt)
		{
			var meeting = Meeting.FetchOrCreateMeeting(Db, OrgId, dt, noautoabsents: true);
			var a = new Attend
			{
				AttendanceFlag = Present,
				AttendanceTypeId = AttendTypeCode.Member,
				MemberTypeId = Codes.MemberTypeCode.Member,
				MeetingId = meeting.MeetingId,
				MeetingDate = dt,
				PeopleId = PeopleId,
				OrganizationId = OrgId,
				CreatedDate = DateTime.Now,
			};
			Db.Attends.InsertOnSubmit(a);
			return meeting.MeetingId;
		}
        public static void AddAttend(CMSDataContext Db, int PeopleId, int OrgId, bool Present, int MeetingId, DateTime dt)
		{
			var a = new Attend
			{
				AttendanceFlag = Present,
				AttendanceTypeId = AttendTypeCode.Member,
				MemberTypeId = MemberTypeCode.Member,
				MeetingId = MeetingId,
				MeetingDate = dt,
				PeopleId = PeopleId,
				OrganizationId = OrgId,
				CreatedDate = DateTime.Now,
                SubGroupName = "",
                Pager = ""
			};
			Db.Attends.InsertOnSubmit(a);
		}
		public static Attend AddAttend(CMSDataContext Db, int PeopleId, int OrgId, bool Present, int attendtype, int membertype, DateTime dt)
		{
			var meeting = Meeting.FetchOrCreateMeeting(Db, OrgId, dt, noautoabsents: true);
			var a = new Attend
			{
				AttendanceFlag = Present,
				AttendanceTypeId = attendtype,
				MemberTypeId = membertype,
				MeetingId = meeting.MeetingId,
				MeetingDate = dt,
				PeopleId = PeopleId,
				OrganizationId = OrgId,
				CreatedDate = DateTime.Now,
			};
			Db.Attends.InsertOnSubmit(a);
            return a;
		}
	}
}
