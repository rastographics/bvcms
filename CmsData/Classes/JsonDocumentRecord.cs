using Dapper;

namespace CmsData.Classes
{
    public class JsonDocumentRecord
    {
        public static void AddUpdate(CMSDataContext db, string json,
                string section, object pk1, object pk2 = null, object pk3 = null, object pk4 = null)
        {
            var id1 = pk1.ToString();
            var id2 = pk2?.ToString() ?? "";
            var id3 = pk3?.ToString() ?? "";
            var id4 = pk4?.ToString() ?? "";
            db.Connection.Execute("custom.AddUpdateJsonRecord",
                new { json, section, id1, id2, id3, id4 },
                commandType: System.Data.CommandType.StoredProcedure);
        }
        public static string Fetch(CMSDataContext db, string section, object id1, object id2 = null, object id3 = null, object id4 = null)
        {
            id2 = id2?.ToString() ?? "";
            id3 = id3?.ToString() ?? "";
            id4 = id4?.ToString() ?? "";
            return db.Connection.QueryFirstOrDefault<string>(
                @"select Json from custom.JsonDocumentRecords
                  where Section = @section and Id1 = @id1 and Id2 = @id2 and Id3 = @id3 and Id4 = @id4",
                new { section, id1, id2, id3, id4 });
        }
    }
}
