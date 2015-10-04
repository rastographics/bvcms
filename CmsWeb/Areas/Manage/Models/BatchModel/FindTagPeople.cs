using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmsData;
using CmsWeb.Code;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Models.BatchModel
{
    public partial class BatchModel
    {
        public static List<FindInfo> FindTagPeople(string text, string tagname)
        {
            if (!tagname.HasValue())
                throw new UserInputException("No Tag");

            var csv = new CsvReader(new StringReader(text), true, '\t');
            if (!csv.Any())
                throw new UserInputException("No Data");

            var cols = csv.GetFieldHeaders();
            if (!cols.Contains("First") || !cols.Contains("Last"))
                throw new UserInputException("Both First and Last are required");

            if (!cols.Any(name => new[] { "Birthday", "Email", "Phone", "Phone2", "Phone3" }.Contains(name)))
                throw new UserInputException("One of Birthday, Email, Phone, Phone2 or Phone3 is required");
            
            var list = new List<FindInfo>();

            while (csv.ReadNextRecord())
            {
                var row = new FindInfo
                {
                    First = FindColumn(csv, "First"),
                    Last = FindColumn(csv, "Last"),
                    Birthday = FindColumnDate(csv, "Birthday"),
                    Email = FindColumn(csv, "Email"),
                    Phone = FindColumnDigits(csv, "Phone"),
                    Phone2 = FindColumnDigits(csv, "Phone2"),
                    Phone3 = FindColumnDigits(csv, "Phone3")
                };

                var pids = DbUtil.Db.FindPerson3(row.First, row.Last, row.Birthday, row.Email, 
                    row.Phone, row.Phone2, row.Phone3).ToList();
                row.Found = pids.Count;
                if(pids.Count == 1)
                    row.PeopleId = pids[0].PeopleId;
                list.Add(row);
            }
            var q = from pi in list
                where pi.PeopleId.HasValue
                select pi.PeopleId;
            foreach (var pid in q.Distinct())
                Person.Tag(DbUtil.Db, pid ?? 0, tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            DbUtil.Db.SubmitChanges();
            return list;
        }
        private static string FindColumn(CsvReader csv, string col)
        {
            var i = csv.GetFieldIndex(col);
            if (i >= 0)
                return csv[i];
            return null;
        }
        private static string FindColumnDigits(CsvReader csv, string col)
        {
            var s = FindColumn(csv, col);
            if (s.HasValue())
                return s.GetDigits();
            return s;
        }
        private static DateTime? FindColumnDate(CsvReader csv, string col)
        {
            var s = FindColumn(csv, col);
            DateTime dt;
            if (s != null)
                if (DateTime.TryParse(s, out dt))
                    return dt;
            return null;
        }
        public class FindInfo
        {
            public int? PeopleId { get; set; }
            public int Found { get; set; }
            public string First { get; set; }
            public string Last { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Phone2 { get; set; }
            public string Phone3 { get; set; }
            public DateTime? Birthday { get; set; }
        }
    }
}