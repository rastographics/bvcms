using CmsData;
using CmsData.Classes.DataMapper;
using ImageData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

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

        public string memberList = "";

		public static Family forID( CMSDataContext dataContext, CMSImageDataContext imageContext, int familyID, int campus, DateTime date, bool returnPictureUrls = false)
		{
			Family family = new Family();
			DataTable table = new DataTable();

			string qFamily = @"SELECT family.FamilyId AS id, MAX( head.Name ) AS name
										FROM dbo.Families family
											LEFT JOIN dbo.People AS head ON family.HeadOfHouseholdId = head.PeopleId AND head.DeceasedDate IS NULL
										WHERE family.familyId = @familyID
										GROUP BY family.FamilyId";

			using( SqlCommand cmd = new SqlCommand( qFamily, dataContext.ReadonlyConnection() as SqlConnection ) ) {
				SqlParameter parameter = new SqlParameter( "familyID", familyID );

				cmd.Parameters.Add( parameter );

				SqlDataAdapter adapter = new SqlDataAdapter( cmd );
				adapter.Fill( table );
			}

			if( table.Rows.Count == 1 ) {
				family.populate( table.Rows[0] );
				family.loadMembers( dataContext, imageContext, campus, date, returnPictureUrls );
			}

			return family;
		}

        public static List<Family> forAttendanceBundle(CMSDataContext cmsdb, CMSImageDataContext cmsidb, AttendanceBundle bundle, int campus, DateTime date, bool returnPictureUrls)
        {
            List<Family> families = new List<Family>();
            Family family = new Family();

            var PeopleIds = bundle.attendances.Select(a => a.peopleID).ToList();
            int FamilyId = cmsdb.People.Where(p => PeopleIds.Contains(p.PeopleId)).FirstOrDefault().FamilyId;

            family.id = FamilyId;
            family.loadMembers(cmsdb, cmsidb, campus, date, returnPictureUrls);
            families.Add(family);

            return families;
        }

        public static List<Family> forSearch( CMSDataContext cmsdb, CMSImageDataContext cmsidb, string search, int campus, DateTime date, bool returnPictureUrls )
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

                using (SqlCommand cmd = new SqlCommand(qFamilies, cmsdb.ReadonlyConnection() as SqlConnection))
                {
                    SqlParameter parameter = new SqlParameter("search", $"%{search}");

                    cmd.Parameters.Add(parameter);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);
                }
            } else {
                Util.NameSplit(search, out string first, out string last);

                qFamilies = GetQuerySearch(first, last);

                using (SqlCommand cmd = new SqlCommand(qFamilies, cmsdb.ReadonlyConnection() as SqlConnection))
                {
                    SqlParameter firstParameter = new SqlParameter("first", $"{first}%");
                    SqlParameter lastParameter = new SqlParameter("last", $"{last}%");
                    SqlParameter searchParameter = new SqlParameter("search", $"{search}%");

                    cmd.Parameters.Add(firstParameter);
                    cmd.Parameters.Add(lastParameter);
                    cmd.Parameters.Add(searchParameter);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);
                }
            }

			foreach( DataRow row in table.Rows ) {
				Family family = new Family();
				family.populate( row );
				if( !returnPictureUrls ) {
					family.loadPicture( cmsdb, cmsidb );
				}

				family.loadMembers( cmsdb, cmsidb, campus, date, returnPictureUrls );

				families.Add( family );
			}

			return families;
		}

        private static string GetQuerySearch(string first, string last)
        {
            if (string.IsNullOrEmpty(first) && string.IsNullOrEmpty(last))
            {
                return @"SELECT TOP 50
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
                        WHERE (person.LastName LIKE @search OR person.MaidenName LIKE @search)
                            OR (person.FirstName LIKE @search OR person.NickName LIKE @search OR person.MiddleName LIKE @search)
                        GROUP BY family.FamilyId
                        ORDER BY name";
            }
            else
            {
                return @"SELECT TOP 50
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
            }            
        }

        private void loadMembers( CMSDataContext cmsdb, CMSImageDataContext cmsidb, int campus, DateTime date, bool returnPictureUrls )
		{
			members.AddRange( FamilyMember.forFamilyID( cmsdb, cmsidb, id, campus, date, returnPictureUrls ) );
            memberList = string.Join(", ", members.Where(m => m.name != name).Select(x => x.firstName));
		}

		private void loadPicture( CMSDataContext cmsdb, CMSImageDataContext cmsidb )
		{
			CmsData.Family family = cmsdb.Families.SingleOrDefault( f => f.FamilyId == id );
			int? ImageId;
			if( family == null || family.Picture == null ) {
				ImageId = CmsData.Picture.SmallMissingGenericId;
			} else {
				ImageId = family.Picture.SmallId;
			}

			Image image = cmsidb.Images.SingleOrDefault( i => i.Id == ImageId );

			if( image != null ) {
				picture = Convert.ToBase64String( image.Bits );
			}
		}
	}
}
