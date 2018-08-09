using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	public class Label
	{
		private readonly Dictionary<int, LabelFormat> formats;
		public readonly List<LabelEntry> entries = new List<LabelEntry>();

		public bool hasMoreEntries;

		public Label( SqlConnection db, int size, Type type, Attendance attendance, List<AttendanceGroup> groups )
		{
			formats = LabelFormat.forSize( db, size );
			hasMoreEntries = process( type, attendance, groups );
		}

		public Label( SqlConnection db, int size, Type type, Attendance attendance, AttendanceGroup group )
		{
			List<AttendanceGroup> groups = new List<AttendanceGroup> {group};

			formats = LabelFormat.forSize( db, size );
			hasMoreEntries = process( type, attendance, groups );
		}

		private bool process( Type type, Attendance attendance, List<AttendanceGroup> groups )
		{
			LabelFormat format = formats[(int) type];

			// TODO: Use group count to figure out how many labels to print by getting the max repeats from the label format

			foreach( LabelFormatEntry formatEntry in format.entries ) {
				if( formatEntry.repeat > 1 && groups != null && groups.Count > 0 ) {
					for( int iX = 0; iX < formatEntry.repeat; iX++ ) {
						if( groups.Count <= iX ) break;

						LabelEntry entry = new LabelEntry( formatEntry, attendance, groups[iX], iX );

						entries.Add( entry );
					}
				} else {
					if( groups == null || groups.Count == 0 ) {
						entries.Add( new LabelEntry( formatEntry, attendance ) );
					} else {
						entries.Add( new LabelEntry( formatEntry, attendance, groups[0] ) );
					}
				}
			}

			return false;
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