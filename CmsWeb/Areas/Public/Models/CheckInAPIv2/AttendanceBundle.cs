using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CmsData;
using CmsWeb.Areas.Public.Models.CheckInAPIv2.Caching;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	public class AttendanceBundle
	{
		public int labelSize = 0;

		public int securityLabels = 0;
		public bool guestLabels = true;
		public bool locationLabels = true;
		public int nameTagAge = 13;

		public List<Attendance> attendances = new List<Attendance>();

		public void recordAttendance( CMSDataContext dataContext )
		{
			foreach( Attendance attendance in attendances ) {
				foreach( AttendanceGroup group in attendance.groups ) {
					Attend.RecordAttend( dataContext, attendance.peopleID, group.groupID, group.present, group.datetime );

					Attend attend = dataContext.Attends.FirstOrDefault( a => a.PeopleId == attendance.peopleID && a.OrganizationId == group.groupID && a.MeetingDate == group.datetime );

					if( attend == null ) {
						continue;
					}

					if( group.present ) {
						attend.SubGroupID = group.subgroupID;
						attend.SubGroupName = group.subgroupName;
					} else {
						attend.SubGroupID = 0;
						attend.SubGroupName = "";
					}

					if( group.join ) {
						joinToOrg( dataContext, attendance.peopleID, group.groupID );
					}
				}

				dataContext.SubmitChanges();
			}
		}

		private static void joinToOrg( CMSDataContext dataContext, int peopleID, int orgID )
		{
			OrganizationMember om = dataContext.OrganizationMembers.SingleOrDefault( m => m.PeopleId == peopleID && m.OrganizationId == orgID );

			if( om == null ) {
				om = OrganizationMember.InsertOrgMembers( dataContext, orgID, peopleID, CmsData.Codes.MemberTypeCode.Member, DateTime.Today );

				DbUtil.LogActivity( $"Joined {om.PeopleId} to {om.Organization.OrganizationId} via Check-In desktop client", peopleid: om.PeopleId, orgid: om.OrganizationId );

				dataContext.SubmitChanges();

				// Check Entry Point and replace if Check-In
				CmsData.Person person = dataContext.People.FirstOrDefault( p => p.PeopleId == peopleID );

				if( person?.EntryPoint != null && person.EntryPoint.Code == "CHECKIN" ) {
					person.EntryPoint = om.Organization.EntryPoint;

					dataContext.SubmitChanges();
				}
			}
		}

		public List<Label> createLabelData( CMSDataContext dataContext )
		{
			List<Label> labels = new List<Label>();

			using( var db = new SqlConnection( Util.ConnectionString ) ) {
				AttendanceCacheSet cacheSet = new AttendanceCacheSet {
					dataContext = dataContext,
					formats = LabelFormat.forSize( db, labelSize ),
					securityLabels = securityLabels,
					securityCode = dataContext.NextSecurityCode().Select( c => c.Code ).Single().Trim(),
					guestLabels = guestLabels,
					locationLabels = locationLabels,
					nameTagAge = nameTagAge
				};

				foreach( Attendance attendance in attendances ) {
					labels.AddRange( attendance.getLabels( cacheSet ) );
				}

				if( labels.Count > 0 && attendances.Count > 0 && securityLabels == Attendance.SECURITY_LABELS_PER_FAMILY ) {
					labels.AddRange( attendances[0].getSecurityLabel( cacheSet ) );
				}
			}

			return labels;
		}
	}
}