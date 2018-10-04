using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	public class Label
	{
		public readonly List<LabelEntry> entries = new List<LabelEntry>();

		public static List<Label> generate( Dictionary<int, LabelFormat> formats, Type type, Attendance attendance, List<AttendanceGroup> groups )
		{
			List<Label> labels = new List<Label>();
			LabelFormat format = formats[(int) type];

			List<AttendanceGroup> groupsCopy = new List<AttendanceGroup>( groups );

			if( format.canRepeat ) {
				do {
					labels.Add( getLabel( format, attendance, nextGroups( groupsCopy, format.maxRepeat() ) ) );
				} while( groupsCopy.Any() );
			} else {
				labels.Add( getLabel( format, attendance, nextGroups( groupsCopy, 1 ) ) );
			}

			return labels;
		}

		private static List<AttendanceGroup> nextGroups( List<AttendanceGroup> groups, int count )
		{
			int adjustedCount = count <= groups.Count ? count : groups.Count;

			List<AttendanceGroup> groupsCopy = new List<AttendanceGroup>( groups.GetRange( 0, adjustedCount ) );

			groups.RemoveRange( 0, adjustedCount );

			return groupsCopy;
		}

		private static Label getLabel( LabelFormat format, Attendance attendance, List<AttendanceGroup> groups )
		{
			Label label = new Label();

			foreach( LabelFormatEntry formatEntry in format.entries ) {
				if( formatEntry.repeat > 1 ) {
					for( int iX = 0; iX < formatEntry.repeat; iX++ ) {
						if( groups.Count <= iX ) break;

						LabelEntry entry = new LabelEntry( formatEntry, attendance, groups[iX], iX );

						label.entries.Add( entry );
					}
				} else {
					label.entries.Add( new LabelEntry( formatEntry, attendance, groups[0] ) );
				}
			}

			return label;
		}

		public enum Type
		{
			NONE = 0,
			MAIN = 1,
			LOCATION = 2,
			SECURITY = 3,
			GUEST = 4,
			EXTRA = 5,
			NAME_TAG = 6
		}
	}
}