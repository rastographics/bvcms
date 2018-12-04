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
	public class Subgroup : DataMapper
	{
		public int id;
		public string name = "";

		public int open = 0;
		public int capacity = 0;
		public int count = 0;

		public DateTime? previous = DateTime.MinValue;

		public Subgroup() { }

		public Subgroup( int id, string name )
		{
			this.id = id;
			this.name = name;
		}

		public static List<Subgroup> forGroupID( SqlConnection db, int groupID, int peopleID, int scheduleID, DateTime meetingDate )
		{
			List<Subgroup> subGroups = new List<Subgroup>();
			DataTable table = new DataTable();

			const string qSubGroups = @"SELECT
													MAX(Id) AS id,
													MAX(Name) AS name,
													MAX(CAST(CheckInOpen AS TINYINT)) AS 'open',
													MAX(CheckInCapacity) AS capacity,
													MAX(lastAttend.MeetingDate) AS previous,
													MAX(attendCount.Count) AS count
												FROM dbo.MemberTags AS tags
													LEFT JOIN (SELECT *
																	FROM dbo.Attend
																	WHERE OrganizationId = @groupID
																		AND PeopleId = @peopleID
																		AND AttendanceFlag = 1) AS lastAttend ON lastAttend.SubGroupID = tags.Id
													LEFT JOIN (SELECT COUNT(*) AS Count, MAX(SubGroupID) AS SubGroupID
																  FROM dbo.Attend
																  WHERE OrganizationId = @groupID
																		AND AttendanceFlag = 1
																		AND MeetingDate = @meetingDate
																	GROUP BY SubGroupID) AS attendCount ON attendCount.SubGroupID = tags.Id
												WHERE OrgId = @groupID
													AND ScheduleId = @scheduleID
													AND CheckIn = 1
												GROUP BY Id
												ORDER BY name";

			using( SqlCommand cmd = new SqlCommand( qSubGroups, db ) ) {
				SqlParameter groupParameter = new SqlParameter( "groupID", groupID );
				SqlParameter peopleParameter = new SqlParameter( "peopleID", peopleID );
				SqlParameter scheduleParameter = new SqlParameter( "scheduleID", scheduleID );
				SqlParameter dateParameter = new SqlParameter( "meetingDate", meetingDate );

				cmd.Parameters.Add( groupParameter );
				cmd.Parameters.Add( peopleParameter );
				cmd.Parameters.Add( scheduleParameter );
				cmd.Parameters.Add( dateParameter );

				SqlDataAdapter adapter = new SqlDataAdapter( cmd );
				adapter.Fill( table );
			}

			foreach( DataRow row in table.Rows ) {
				Subgroup subGroup = new Subgroup();
				subGroup.populate( row );

				subGroups.Add( subGroup );
			}

			return subGroups;
		}
	}
}