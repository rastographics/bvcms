using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	public class AttendanceGroup
	{
		public int groupID = 0;
		public int subgroupID = 0;
		public string subgroupName = "";
		public DateTime datetime = DateTime.Now;
		public bool present = false;
		public bool join = false;

		public CmsData.Organization org;
		public CmsData.OrganizationMember orgMember;
		public CmsData.Meeting meeting;

		public void load( int peopleID )
		{
			if( !present || groupID == 0 ) return;

			org = CmsData.DbUtil.Db.Organizations.SingleOrDefault( o => o.OrganizationId == groupID );
			orgMember = CmsData.DbUtil.Db.OrganizationMembers.FirstOrDefault( om => om.OrganizationId == org.OrganizationId && om.PeopleId == peopleID );

			int meetingID = CmsData.DbUtil.Db.CreateMeeting( groupID, datetime );
			meeting = CmsData.DbUtil.Db.Meetings.SingleOrDefault( m => m.MeetingId == meetingID );
		}

		public bool isGroupMember()
		{
			return orgMember != null;
		}
	}
}