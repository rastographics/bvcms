using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CmsData;
using CmsWeb.Areas.Public.Models.CheckInAPIv2.Caching;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	[SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Global" )]
	[SuppressMessage( "ReSharper", "UnassignedField.Global" )]
	[SuppressMessage( "ReSharper", "ClassNeverInstantiated.Global" )]
	public class Attendance
	{
		public const int SECURITY_LABELS_NONE = 0;
		public const int SECURITY_LABELS_PER_MEETING = 1;
		public const int SECURITY_LABELS_PER_CHILD = 2;
		public const int SECURITY_LABELS_PER_FAMILY = 3;

		public int peopleID;

		public List<AttendanceGroup> groups = new List<AttendanceGroup>();

		public List<Label> getSecurityLabel( AttendanceCacheSet cacheSet )
		{
			return Label.generate( cacheSet, Label.Type.SECURITY, this, groups );
		}

		public List<Label> getLabels( AttendanceCacheSet cacheSet )
		{
			List<Label.Type> labelTypes = new List<Label.Type>();
			List<Label> labels = new List<Label>();
			CmsData.Person person = cacheSet.getPerson( peopleID );

			// Create label type list
			if( groups.Count > 0 && hasAttends() ) {
				// TODO: Client size option for age cutoff for Name Tag.  Server will override and will send it to the client and disable field
				if( (person.Age ?? 0) < cacheSet.nameTagAge ) {
					labelTypes.Add( Label.Type.MAIN );

					foreach( AttendanceGroup group in groups ) {
						Organization org = cacheSet.getOrganization( group.groupID );

						if( org != null && org.NumCheckInLabels > 1 && group.present ) {
							for( int iX = 0; iX < org.NumCheckInLabels - 1; iX++ ) {
								labelTypes.Add( Label.Type.EXTRA );
							}
						}
					}

					if( hasVisits( cacheSet ) ) {
						if( cacheSet.guestLabels ) {
							foreach( AttendanceGroup _ in getVisitGroups( cacheSet ) ) {
								labelTypes.Add( Label.Type.GUEST );
							}
						}

						if( cacheSet.locationLabels ) {
							labelTypes.Add( Label.Type.LOCATION );
						}
					}

					switch( cacheSet.securityLabels ) {
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
					labels.AddRange( Label.generate( cacheSet, labelType, this, getVisitGroups( cacheSet ).ToList() ) );
				} else {
					labels.AddRange( Label.generate( cacheSet, labelType, this, groups ) );
				}
			}

			return labels;
		}

		private bool hasVisits( AttendanceCacheSet cacheSet )
		{
			OrganizationMember member;

			foreach( AttendanceGroup group in groups ) {
				member = cacheSet.getOrganizationMember( group.groupID, peopleID );

				if( member != null ) {
					return true;
				}
			}

			return false;
		}

		private bool hasAttends()
		{
			foreach( AttendanceGroup group in groups ) {
				if( group.present ) {
					return true;
				}
			}

			return false;
		}

		private IEnumerable<AttendanceGroup> getVisitGroups( AttendanceCacheSet cacheSet )
		{
			OrganizationMember member;

			foreach( AttendanceGroup group in groups ) {
				member = cacheSet.getOrganizationMember( group.groupID, peopleID );

				if( member != null ) {
					yield return group;
				}
			}
		}
	}
}