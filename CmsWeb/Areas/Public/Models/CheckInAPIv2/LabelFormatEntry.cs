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
	[SuppressMessage( "ReSharper", "UnassignedField.Global" )]
	public class LabelFormatEntry : DataMapper
	{
		public int id;

		public int labelID;
		public int typeID;

		public int repeat;
		public decimal offset;

		public string font = "";
		public int fontSize;

		public int fieldID = 0;
		public string fieldFormat = "";

		public decimal startX;
		public decimal startY;

		public int alignX;
		public int alignY;

		public decimal endX;
		public decimal endY;

		public decimal width;
		public decimal height;

        public bool invert;
        public int order;

        public string orgEV;
        public string personFlag;


        public static List<LabelFormatEntry> forLabelID( SqlConnection db, int id )
		{
			List<LabelFormatEntry> entries = new List<LabelFormatEntry>();
			DataTable table = new DataTable();

			const string qLabels = @"SELECT *
												FROM CheckInLabelEntry
												WHERE labelID = @labelID
                                                ORDER by [order]";

			using( SqlCommand cmd = new SqlCommand( qLabels, db ) ) {
				SqlParameter labelParameter = new SqlParameter( "labelID", id );

				cmd.Parameters.Add( labelParameter );

				SqlDataAdapter adapter = new SqlDataAdapter( cmd );
				adapter.Fill( table );
			}

			foreach( DataRow row in table.Rows ) {
				LabelFormatEntry entry = new LabelFormatEntry();
				entry.populate( row );

				entries.Add( entry );
			}

			return entries;
		}
	}
}
