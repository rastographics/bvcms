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
	[SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Global" )]
	[SuppressMessage( "ReSharper", "ConvertToConstant.Global" )]
	public class LabelFormat : DataMapper
	{
		public int id = 0;
		public int typeID = 0;

		public string name = "";

		public int minimum = 0;
		public int maximum = 0;

		public bool canRepeat = false;

		public List<LabelFormatEntry> entries = new List<LabelFormatEntry>();

		public static Dictionary<int, LabelFormat> forSize( SqlConnection db, int size )
		{
			Dictionary<int, LabelFormat> formats = new Dictionary<int, LabelFormat>();
			DataTable table = new DataTable();

			const string qFormats = @"SELECT
													label.id AS id,
													label.name AS name,
													label.typeID AS typeID,
													type.name AS type,
													minimum,
													maximum,
													type.canRepeat AS canRepeat
												FROM CheckInLabel AS label
													LEFT JOIN CheckInLabelType AS type ON type.id = label.typeID
												WHERE @size BETWEEN minimum AND maximum";

			using( SqlCommand cmd = new SqlCommand( qFormats, db ) ) {
				SqlParameter sizeParameter = new SqlParameter( "size", size );

				cmd.Parameters.Add( sizeParameter );

				SqlDataAdapter adapter = new SqlDataAdapter( cmd );
				adapter.Fill( table );
			}

			foreach( DataRow row in table.Rows ) {
				LabelFormat format = new LabelFormat();
				format.populate( row );
				format.loadEntries( db );

				formats.Add( format.typeID, format );
			}

			return formats;
		}

		public void loadEntries( SqlConnection db )
		{
			entries.Clear();
			entries.AddRange( LabelFormatEntry.forLabelID( db, id ) );
		}

		public int maxRepeat()
		{
			int count = 0;

			foreach( LabelFormatEntry entry in entries ) {
				if( entry.repeat > count ) count = entry.repeat;
			}

			return count;
		}
	}
}