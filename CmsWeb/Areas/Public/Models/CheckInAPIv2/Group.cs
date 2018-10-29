using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using CmsData.Classes.DataMapper;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	public class Group : DataMapper
	{
		public int id = 0;
		public string name = "";
		public string leaderName = "";
		public DateTime? date = DateTime.MinValue;
		public int scheduleID = 0;

		public DateTime? birthdayStart = DateTime.MinValue;
		public DateTime? birthdayEnd = DateTime.MinValue;

		public bool allowOverlap = false;

		public string location = "";

		public bool member = false;
		public string memberType = "";
		public bool leader = false;
		public bool guest = false;

		public bool checkedIn = false;

		public int roomID = 0;
		public string roomName = "";

		public int subGroupID = 0;
		public string subGroupName = "";

		public static List<Group> forPersonID( SqlConnection db, int personID, int campus, DateTime date )
		{
			List<Group> groups = new List<Group>();

			groups.AddRange( loadMemberGroups( db, personID, campus, date ) );
			groups.AddRange( loadVisitorGroups( db, personID, campus, date ) );

			return groups;
		}

		public static List<Group> forGroupFinder( SqlConnection db, DateTime? birthday, int campus, int day, int showAll )
		{
			List<Group> groups = new List<Group>();
			DataTable table = new DataTable();

			string qGroups = @"SELECT org.OrganizationId AS id,
											 org.OrganizationName AS name,
											 org.LeaderName AS leaderName,
											 schedule.Id AS scheduleID,
											 schedule.SchedTime AS date,
											 org.BirthDayStart,
											 org.BirthDayEnd,
											 org.Location,
											 org.AllowAttendOverlap
									FROM dbo.Organizations AS org
											  LEFT JOIN dbo.OrgSchedule AS schedule ON schedule.OrganizationId = org.OrganizationId
									WHERE schedule.SchedDay = @day
										AND (org.SuspendCheckin IS NULL OR org.SuspendCheckin = 0)
										AND org.CanSelfCheckin = 1
										AND (org.ClassFilled IS NULL OR org.ClassFilled = 0)
										AND ((org.CampusId IS NULL AND org.AllowNonCampusCheckIn = 1) OR org.CampusId = @campus OR @campus = 0)
										AND org.OrganizationStatusId = 30
										AND (@birthday IS NULL OR CAST(org.BirthDayStart AS DATE) <= @birthday OR org.BirthDayStart IS NULL)
										AND (@birthday IS NULL OR CAST(org.BirthDayEnd AS DATE) >= @birthday OR org.BirthDayEnd IS NULL)
									ORDER BY org.OrganizationName, schedule.SchedTime";

			using( SqlCommand cmd = new SqlCommand( qGroups, db ) ) {
				SqlParameter personParameter = new SqlParameter( "birthday", birthday );
				SqlParameter campusParameter = new SqlParameter( "campus", campus );
				SqlParameter dateParameter = new SqlParameter( "day", day );
				SqlParameter showAllParameter = new SqlParameter( "showAll", showAll );

				cmd.Parameters.Add( personParameter );
				cmd.Parameters.Add( campusParameter );
				cmd.Parameters.Add( dateParameter );
				cmd.Parameters.Add( showAllParameter );

				SqlDataAdapter adapter = new SqlDataAdapter( cmd );
				adapter.Fill( table );
			}

			foreach( DataRow row in table.Rows ) {
				Group group = new Group();
				group.populate( row );

				groups.Add( group );
			}

			return groups;
		}

		private static List<Group> loadMemberGroups( SqlConnection db, int peopleID, int campus, DateTime date )
		{
			List<Group> groups = new List<Group>();
			DataTable table = new DataTable();

			string qMembers = @"SELECT
											org.OrganizationId AS id,
											org.OrganizationName AS name,
											org.LeaderName AS leaderName,
											org.Location AS location,
											CAST( 1 AS BIT ) AS member,
											memberType.Description AS memberType,
											attendType.Worker AS leader,
											attendType.Guest AS guest,
											room.Id AS roomID,
											room.Name AS roomName,
											schedule.NextMeetingDate AS date,
											schedule.Id AS scheduleID,
											ISNULL( attend.AttendanceFlag, 0 ) AS checkedIn,
											attend.SubGroupID AS subGroupID,
											attend.SubGroupName AS subGroupName
										FROM dbo.OrganizationMembers AS member
											LEFT JOIN dbo.Organizations AS org ON org.OrganizationId = member.OrganizationId
											LEFT JOIN dbo.OrgSchedule AS schedule ON schedule.OrganizationId = org.OrganizationId
											LEFT JOIN (dbo.MemberTags AS room LEFT JOIN dbo.OrgMemMemTags AS tags ON room.id = tags.MemberTagId)
																	ON schedule.Id = room.ScheduleId AND room.CheckIn = 1 AND org.OrganizationId = tags.OrgId AND member.PeopleId = tags.PeopleId
											LEFT JOIN dbo.Meetings AS meeting ON meeting.MeetingDate = schedule.NextMeetingDate AND meeting.OrganizationId = org.OrganizationId
											LEFT JOIN dbo.Attend AS attend ON attend.MeetingId = meeting.MeetingId AND attend.PeopleId = member.PeopleId
											LEFT JOIN dbo.Setting AS setting on setting.Id = 'DisallowInactiveCheckin' AND Setting = 'true'
											LEFT JOIN lookup.MemberType AS memberType ON memberType.Id = member.MemberTypeId
											LEFT JOIN lookup.AttendType AS attendType ON attendType.Id = memberType.AttendanceTypeId
											LEFT JOIN lookup.OrganizationStatus AS orgStatus ON orgStatus.Id = org.OrganizationStatusId
										WHERE member.PeopleId = @peopleID
												AND org.CanSelfCheckin = 1
												AND CAST( schedule.NextMeetingDate AS DATE ) = CAST( @date AS DATE )
												AND member.Pending = 0
												AND memberType.Pending = 0 -- Used for Prospects
												-- Inactive is normally allowed unless DisallowInactiveCheckin setting is set to true
												AND (memberType.Inactive = 0 OR setting.Setting = 'true')
												AND orgStatus.Active = 1
												AND (org.CampusId = @campus OR @campus IS NULL OR @campus = 0 OR org.AllowNonCampusCheckIn = 1)
										ORDER BY schedule.NextMeetingDate, org.OrganizationName";

			using( SqlCommand cmd = new SqlCommand( qMembers, db ) ) {
				SqlParameter personParameter = new SqlParameter( "peopleID", peopleID );
				SqlParameter campusParameter = new SqlParameter( "campus", campus );
				SqlParameter dateParameter = new SqlParameter( "date", date );

				cmd.Parameters.Add( personParameter );
				cmd.Parameters.Add( campusParameter );
				cmd.Parameters.Add( dateParameter );

				SqlDataAdapter adapter = new SqlDataAdapter( cmd );
				adapter.Fill( table );
			}

			foreach( DataRow row in table.Rows ) {
				Group group = new Group();
				group.populate( row );

				groups.Add( group );
			}

			return groups;
		}

		public static List<Group> loadVisitorGroups( SqlConnection db, int peopleID, int campus, DateTime date )
		{
			List<Group> groups = new List<Group>();
			DataTable table = new DataTable();

			string qVisits = @"SELECT
										org.OrganizationId AS id,
										org.OrganizationName AS name,
										org.LeaderName AS leader,
										org.Location AS location,
										CAST( 0 AS BIT ) AS member,
										room.Id AS roomID,
										room.Name AS roomName,
										schedule.NextMeetingDate AS date,
										attend.AttendanceFlag AS checkedIn,
										attend.SubGroupID AS subGroupID,
										attend.SubGroupName AS subGroupName
									FROM (SELECT org.OrganizationId
											FROM Attend AS attend
												INNER JOIN Organizations AS org ON org.OrganizationId = attend.OrganizationId
												INNER JOIN lookup.OrganizationStatus AS orgStatus ON orgStatus.Id = org.OrganizationStatusId
												INNER JOIN lookup.AttendType AS attendType ON attendType.Id = attend.AttendanceTypeId
											WHERE attend.PeopleId = @peopleID
													AND org.CanSelfCheckin = 1
													AND (org.AllowNonCampusCheckIn = 1 OR org.CampusId = @campus OR @campus IS NULL OR @campus = 0)
													AND orgStatus.Active = 1
													AND attend.AttendanceFlag = 1
													AND (attend.MeetingDate >= org.FirstMeetingDate OR org.FirstMeetingDate IS NULL)
													AND attend.MeetingDate >= org.VisitorDate
													AND attendType.Guest = 1
											GROUP BY org.OrganizationId) AS visit
										INNER JOIN Organizations AS org ON org.OrganizationId = visit.OrganizationId
										LEFT JOIN (dbo.OrgMemMemTags AS tags INNER JOIN dbo.MemberTags AS room ON tags.MemberTagId = room.Id AND
																																room.CheckIn = 1) ON tags.PeopleId = @peopleID AND org.OrganizationId = tags.OrgId
										LEFT JOIN dbo.OrgSchedule AS schedule ON schedule.OrganizationId = org.OrganizationId
										LEFT JOIN (SELECT
															meeting.OrganizationId,
															attend.MeetingDate,
															attend.AttendanceFlag,
															attend.SubGroupID,
															attend.SubGroupName
													  FROM dbo.Attend AS attend
														  JOIN dbo.Meetings AS meeting ON meeting.MeetingId = attend.MeetingId
													  WHERE attend.PeopleId = @peopleID
															  AND attend.AttendanceFlag = 1) AS attend
											ON attend.OrganizationId = org.OrganizationId AND attend.MeetingDate = schedule.NextMeetingDate
									WHERE CAST( schedule.NextMeetingDate AS DATE ) = CAST( @date AS DATE )
									ORDER BY date, name";

			using( SqlCommand cmd = new SqlCommand( qVisits, db ) ) {
				SqlParameter personParameter = new SqlParameter( "peopleID", peopleID );
				SqlParameter campusParameter = new SqlParameter( "campus", campus );
				SqlParameter dateParameter = new SqlParameter( "date", date );

				cmd.Parameters.Add( personParameter );
				cmd.Parameters.Add( campusParameter );
				cmd.Parameters.Add( dateParameter );

				SqlDataAdapter adapter = new SqlDataAdapter( cmd );
				adapter.Fill( table );
			}

			foreach( DataRow row in table.Rows ) {
				Group group = new Group();
				group.populate( row );

				groups.Add( group );
			}

			return groups;
		}
	}
}