using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2.Caching
{
	public class AttendanceCacheSet
	{
		public CMSDataContext dataContext;

		public Dictionary<int, LabelFormat> formats;

		public int securityLabels;
		public string securityCode = "";

		public bool guestLabels;
		public bool locationLabels;

		public int nameTagAge;

		private readonly Dictionary<int, CmsData.Family> families = new Dictionary<int, CmsData.Family>();
		private readonly Dictionary<int, CmsData.Person> people = new Dictionary<int, CmsData.Person>();
		
		private readonly Dictionary<int, Organization> organizations = new Dictionary<int, Organization>();
		private readonly Dictionary<string, OrganizationMember> organizationMembers = new Dictionary<string, OrganizationMember>();
		private readonly Dictionary<string, Meeting> meetings = new Dictionary<string, Meeting>();

		public CmsData.Family getFamily( int familyID )
		{
			if( !families.ContainsKey( familyID ) ) {
				families[familyID] = dataContext.Families.FirstOrDefault( f => f.FamilyId == familyID );
			}

			return families[familyID];
		}
		
		public CmsData.Person getPerson( int personID )
		{
			if( !people.ContainsKey( personID ) ) {
				people[personID] = dataContext.People.FirstOrDefault( p => p.PeopleId == personID );
			}

			return people[personID];
		}

		public Organization getOrganization( int orgID )
		{
			if( !organizations.ContainsKey( orgID ) ) {
				organizations[orgID] = dataContext.Organizations.FirstOrDefault( o => o.OrganizationId == orgID );
			}

			return organizations[orgID];
		}

		public OrganizationMember getOrganizationMember( int organizationID, int peopleID )
		{
			string key = $"{organizationID}-{peopleID}";

			if( !organizationMembers.ContainsKey( key ) ) {
				organizationMembers[key] = dataContext.OrganizationMembers.FirstOrDefault( om => om.OrganizationId == organizationID && om.PeopleId == peopleID );
			}

			return organizationMembers[key];
		}

		public Meeting getMeeting( int groupID, DateTime dateTime )
		{
			string key = $"{groupID}-{dateTime}";

			if( !meetings.ContainsKey( key ) ) {
				int meetingID = dataContext.CreateMeeting( groupID, dateTime );

				meetings[key] = dataContext.Meetings.SingleOrDefault( m => m.MeetingId == meetingID );
			}

			return meetings[key];
		}
	}
}