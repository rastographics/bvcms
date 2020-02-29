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

		public static List<Subgroup> forGroupID( SqlConnection db, int groupID, int peopleID )
		{
			List<Subgroup> subGroups = new List<Subgroup>();
			DataTable table = new DataTable();

			const string qSubGroups = @"SELECT
                                                MAX(Id) AS id,
												MAX(Name) AS name
												FROM MemberTags AS tags
												join OrgMemMemTags as mt on tags.Id = mt.MemberTagId
												WHERE mt.OrgId = @groupID
												AND mt.PeopleId = @peopleID
												GROUP BY Id
												ORDER BY name";

			using( SqlCommand cmd = new SqlCommand( qSubGroups, db ) ) {
				SqlParameter groupParameter = new SqlParameter( "groupID", groupID );
				SqlParameter peopleParameter = new SqlParameter( "peopleID", peopleID );

				cmd.Parameters.Add( groupParameter );
				cmd.Parameters.Add( peopleParameter );

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
