using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CmsData.Classes.DataMapper;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	public class FamilyMember : DataMapper
	{
		public int id = 0;
		public int age = 0;
		public DateTime birthday = DateTime.MinValue;
		public int position = 0;
		public int genderID = 0;

		public string name = "";
		public string altName = "";
		public string email = "";
		public string mobile = "";
		public string picture = "";

		public int pictureX = 0;
		public int pictureY = 0;

		public List<Group> groups = new List<Group>();

		public static List<FamilyMember> forFamilyID( SqlConnection db, int familyID, int campus, DateTime date )
		{
			List<FamilyMember> members = new List<FamilyMember>();
			DataTable table = new DataTable();

			string qMembers = @"SELECT *
										FROM (SELECT
													person.PeopleId AS id,
													person.Name AS name,
													ISNULL( person.Age, 0 ) AS age,
													person.BDate AS birthday,
													gender.Id AS genderID,
													EmailAddress AS email,
													CellPhone AS mobile
												FROM dbo.People AS person
													LEFT JOIN lookup.Gender AS gender ON person.GenderId = gender.Id
												WHERE person.FamilyId = @familyID
												UNION
												SELECT
													person.PeopleId AS id,
													person.Name AS name,
													ISNULL( person.Age, 0 ) AS age,
													person.BDate AS birthday,
													gender.Id AS genderID,
													EmailAddress AS email,
													CellPhone AS mobile
												FROM dbo.PeopleExtra AS extra
													INNER JOIN People AS person on person.PeopleId = extra.PeopleId
													LEFT JOIN lookup.Gender AS gender ON person.GenderId = gender.Id
												WHERE extra.Field = 'Parent'
														AND extra.IntValue IN (SELECT person.PeopleId
																					  FROM dbo.People AS person
																					  WHERE person.FamilyId = @familyID)
											  ) AS familyMembers
										ORDER BY familyMembers.Age DESC, familyMembers.genderID";

			using( SqlCommand cmd = new SqlCommand( qMembers, db ) ) {
				SqlParameter parameter = new SqlParameter( "familyID", familyID );

				cmd.Parameters.Add( parameter );

				SqlDataAdapter adapter = new SqlDataAdapter( cmd );
				adapter.Fill( table );
			}

			foreach( DataRow row in table.Rows ) {
				FamilyMember member = new FamilyMember();
				member.populate( row );
				member.loadPicture();
				member.loadGroups( db, campus, date );

				members.Add( member );
			}

			return members;
		}

		private void loadGroups( SqlConnection db, int campus, DateTime date )
		{
			groups.AddRange( Group.forPersonID( db, id, campus, date ) );
		}

		private void loadPicture()
		{
			CmsData.Person person = CmsData.DbUtil.Db.People.SingleOrDefault( p => p.PeopleId == id );

			if( person == null || person.Picture == null ) return;

			ImageData.Image image = ImageData.DbUtil.Db.Images.SingleOrDefault( i => i.Id == person.Picture.SmallId );

			if( image != null ) {
				picture = Convert.ToBase64String( image.Bits );
			}
		}
	}
}