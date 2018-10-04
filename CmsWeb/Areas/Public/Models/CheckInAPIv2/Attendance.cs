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
		public const int SECURITY_LABELS_NONE = 0;
		public const int SECURITY_LABELS_PER_MEETING = 1;
		public const int SECURITY_LABELS_PER_CHILD = 2;
		public const int SECURITY_LABELS_PER_FAMILY = 3;

		public int peopleID;

		// Label options
		public string labelSecurityCode;

		public CmsData.Person person;

		public CmsData.Person head;
		public CmsData.Person spouse;

		public List<AttendanceGroup> groups = new List<AttendanceGroup>();

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

		public List<Label> getSecurityLabel( Dictionary<int, LabelFormat> formats )
		{
			return Label.generate( formats, Label.Type.SECURITY, this, groups );
		}

		public List<Label> getLabels( Dictionary<int, LabelFormat> formats, int securityLabels, bool guestLabels, bool locationLabels, int nameTagAge )
		{
			List<Label.Type> labelTypes = new List<Label.Type>();
			List<Label> labels = new List<Label>();

			// Create label type list
			if( groups.Count > 0 && hasAttends() ) {
				// TODO: Client size option for age cutoff for Name Tag.  Server will override and will send it to the client and disable field
				if( (person.Age ?? 0) < nameTagAge ) {
					labelTypes.Add( Label.Type.MAIN );

					foreach( AttendanceGroup group in groups ) {
						if( group.org != null && group.org.NumCheckInLabels > 1 && group.present ) {
							for( int iX = 0; iX < group.org.NumCheckInLabels - 1; iX++ ) {
								labelTypes.Add( Label.Type.EXTRA );
							}
						}
					}

					if( hasVisits() ) {
						if( guestLabels ) {
							foreach( AttendanceGroup _ in getVisitGroups() ) {
								labelTypes.Add( Label.Type.GUEST );
							}
						}

						if( locationLabels ) labelTypes.Add( Label.Type.LOCATION );
					}

					switch( securityLabels ) {
						// case SECURITY_LABELS_NONE: 0 = No Security Labels
						// case SECURITY_LABELS_PER_FAMILY: Security Label Per Family (Handled outside this routine)

						case SECURITY_LABELS_PER_MEETING: {
							for( int iX = 0; iX < groups.Count; iX++ ) {
								labelTypes.Add( Label.Type.SECURITY );
							}

							break;
						}

						case SECURITY_LABELS_PER_CHILD: {
							labelTypes.Add( Label.Type.SECURITY );

							break;
						}
					}
				} else {
					labelTypes.Add( Label.Type.NAME_TAG );
				}
			}

			// Generate labels from type list
			foreach( Label.Type labelType in labelTypes ) {
				if( labelType == Label.Type.GUEST ) {
					labels.AddRange( Label.generate( formats, labelType, this, getVisitGroups().ToList() ) );
				} else {
					labels.AddRange( Label.generate( formats, labelType, this, groups ) );
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