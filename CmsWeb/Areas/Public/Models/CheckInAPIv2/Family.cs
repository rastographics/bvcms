using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CmsData.Classes.DataMapper;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	public class Family : DataMapper
	{
		public int id = 0;
		public string name = "";

		public string picture = "";

		public bool locked = false;

		public readonly List<FamilyMember> members = new List<FamilyMember>();

		public static List<Family> forSearch( SqlConnection db, string search, int campus, DateTime date )
		{
			List<Family> families = new List<Family>();
			DataTable table = new DataTable();

			string qFamilies;
			bool isNumeric = Regex.IsMatch( search, @"^\d+$" );

			if( isNumeric ) {
				qFamilies = @"SELECT TOP 50
										family.FamilyId AS id,
										MAX( head.Name ) AS name,
										CAST( CASE WHEN MAX( lock.FamilyId ) IS NOT NULL THEN 1 ELSE 0 END AS bit ) AS locked
									FROM dbo.Families family
										LEFT JOIN dbo.People AS members
											ON family.FamilyId = members.FamilyId
										LEFT JOIN dbo.People AS head
											ON family.HeadOfHouseholdId = head.PeopleId
												AND head.DeceasedDate IS NULL
										LEFT JOIN dbo.PeopleExtra AS extras
											ON members.PeopleId = extras.PeopleId
												AND extras.Field = 'PIN'
										LEFT JOIN dbo.FamilyCheckinLock AS lock
											ON family.FamilyId = lock.FamilyId
												AND DATEDIFF( s, lock.Created, GETDATE( ) ) < 60
												AND Locked = 1
									WHERE REPLACE( family.HomePhone, '-', '' ) LIKE @search
											OR REPLACE( members.CellPhone, '-', '' ) LIKE @search
											OR REPLACE( members.WorkPhone, '-', '' ) LIKE @search
											OR REPLACE( extras.Data, '-', '' ) LIKE @search
									GROUP BY family.FamilyId
									ORDER BY name";

				using( SqlCommand cmd = new SqlCommand( qFamilies, db ) ) {
					SqlParameter parameter = new SqlParameter( "search", $"%{search}" );

					cmd.Parameters.Add( parameter );

					SqlDataAdapter adapter = new SqlDataAdapter( cmd );
					adapter.Fill( table );
				}
			} else {
				string first = "";
				string last = "";
				string[] parts = search.Split( ' ' );

				if( parts.Length == 1 ) {
					last = parts[0];
				} else if( parts.Length > 1 ) {
					first = parts[0];
					last = parts[1];
				}

				qFamilies = @"SELECT TOP 50
										family.FamilyId AS id,
										MAX( head.Name ) AS name,
										CAST( CASE WHEN MAX( lock.FamilyId ) IS NOT NULL THEN 1 ELSE 0 END AS BIT ) AS locked
									FROM People AS person
										LEFT JOIN dbo.Families AS family
											ON family.FamilyId = person.FamilyId
										LEFT JOIN dbo.People AS head
											ON family.HeadOfHouseholdId = head.PeopleId
												AND head.DeceasedDate IS NULL
										LEFT JOIN dbo.FamilyCheckinLock AS lock
											ON family.FamilyId = lock.FamilyId
												AND DATEDIFF( S, lock.Created, GETDATE( ) ) < 60
												AND Locked = 1
									WHERE (person.LastName LIKE @last OR person.MaidenName LIKE @last OR @last = '')
										AND (person.FirstName LIKE @first OR person.NickName LIKE @first OR person.MiddleName LIKE @first OR @first = '')
									GROUP BY family.FamilyId
									ORDER BY name";

				using( SqlCommand cmd = new SqlCommand( qFamilies, db ) ) {
					SqlParameter firstParameter = new SqlParameter( "first", $"{first}%" );
					SqlParameter lastParameter = new SqlParameter( "last", $"{last}%" );

					cmd.Parameters.Add( firstParameter );
					cmd.Parameters.Add( lastParameter );

					SqlDataAdapter adapter = new SqlDataAdapter( cmd );
					adapter.Fill( table );
				}
			}

			foreach( DataRow row in table.Rows ) {
				Family family = new Family();
				family.populate( row );
				family.loadPicture();
				family.loadMembers( db, campus, date );

				families.Add( family );
			}

			return families;
		}

		private void loadMembers( SqlConnection db, int campus, DateTime date )
		{
			members.AddRange( FamilyMember.forFamilyID( db, id, campus, date ) );
		}

		private void loadPicture()
		{
			CmsData.Family family = CmsData.DbUtil.Db.Families.SingleOrDefault( f => f.FamilyId == id );

			if( family == null || family.Picture == null ) return;

			ImageData.Image image = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == family.Picture.SmallId );

			if( image != null ) {
				picture = Convert.ToBase64String( image.Bits );
			}
		}
	}
}