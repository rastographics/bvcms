using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	[SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Global" )]
	public class Attendance
	{
		public int peopleID;

		// Label options
		public string labelSecurityCode;
		public int labelSize = 200;

		public int nameTagAge = 13;
		public bool securityLabels = false;
		public bool guestLabels = true;
		public bool locationLabels = true;

		public CmsData.Person person;

		public CmsData.Person head;
		public CmsData.Person spouse;

		public List<AttendanceGroup> groups = new List<AttendanceGroup>();

		public static List<Attendance> dummyData()
		{
			List<Attendance> attendances = new List<Attendance>();

			Attendance attendance1 = new Attendance
			{
				peopleID = 1061061,
				labelSize = 200,
				groups =
				{
					new AttendanceGroup
					{
						groupID = 86788,
						datetime = new DateTime( 2018, 06, 17, 9, 20, 0 ),
						present = true
					},
					new AttendanceGroup
					{
						groupID = 89731,
						datetime = new DateTime( 2018, 06, 17, 18, 0, 0 ),
						present = true
					}
				}
			};

			Attendance attendance2 = new Attendance
			{
				peopleID = 1090115,
				labelSize = 200,
				groups =
				{
					new AttendanceGroup
					{
						groupID = 86788,
						datetime = new DateTime( 2018, 06, 17, 9, 20, 0 ),
						present = true
					},
				}
			};

			attendances.Add( attendance1 );
			attendances.Add( attendance2 );

			return attendances;
		}

		public void load()
		{
			person = CmsData.DbUtil.Db.People.FirstOrDefault( p => p.PeopleId == peopleID );
			if( person == null ) return;

			head = person.Family.HeadOfHousehold;
			spouse = person.Family.HeadOfHouseholdSpouse;

			foreach( AttendanceGroup group in groups ) {
				group.load( peopleID );
			}
		}

		public Label getSecurityLabel( SqlConnection db )
		{
			return new Label( db, labelSize, Label.Type.SECURITY, this, groups[0] );
		}

		public List<Label> getLabels( SqlConnection db )
		{
			List<Label> labels = new List<Label>();

			if( groups.Count > 0 && hasAttends() ) {
				// TODO: Client size option for age cutoff for Name Tag.  Server will override and will send it to the client and disable field
				if( (person.Age ?? 0) < nameTagAge ) {
					labels.Add( new Label( db, labelSize, Label.Type.MAIN, this, groups ) );

					foreach( AttendanceGroup group in groups ) {
						if( group.org.NumCheckInLabels > 1 && group.present ) {
							for( int iX = 0; iX < group.org.NumCheckInLabels - 1; iX++ ) {
								labels.Add( new Label( db, labelSize, Label.Type.EXTRA, this, group ) );
							}
						}
					}

					if( hasVisits() ) {
						if( guestLabels ) {
							foreach( AttendanceGroup group in getVisitGroups() ) {
								labels.Add( new Label( db, labelSize, Label.Type.GUEST, this, group ) );
							}
						}

						if( locationLabels ) labels.Add( new Label( db, labelSize, Label.Type.LOCATION, this, getVisitGroups().ToList() ) );
					}

					if( securityLabels ) {
						for( int iX = 0; iX < groups.Count; iX++ ) {
							labels.Add( new Label( db, labelSize, Label.Type.SECURITY, this, groups[0] ) );
						}
					}
				} else {
					labels.Add( new Label( db, labelSize, Label.Type.NAME_TAG, this, groups[0] ) );
				}
			}

			return labels;
		}

		private bool hasVisits()
		{
			foreach( AttendanceGroup group in groups ) {
				if( !group.isGroupMember() ) return true;
			}

			return false;
		}

		private bool hasAttends()
		{
			foreach( AttendanceGroup group in groups ) {
				if( group.present ) return true;
			}

			return false;
		}

		private IEnumerable<AttendanceGroup> getVisitGroups()
		{
			foreach( AttendanceGroup group in groups ) {
				if( !group.isGroupMember() ) yield return group;
			}
		}
	}
}