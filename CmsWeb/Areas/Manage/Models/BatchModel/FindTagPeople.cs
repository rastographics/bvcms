using CmsData;
using CmsWeb.Code;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Models.BatchModel
{
    public partial class BatchModel
    {
        public static List<FindInfo> FindTagPeople(string text, string tagname)
        {
            if (!tagname.HasValue())
            {
                throw new UserInputException("No Tag");
            }

            if (!text.HasValue())
            {
                throw new UserInputException("No Data");
            }

            var csv = new CsvReader(new StringReader(text));
            csv.Configuration.Delimiter = "\t";
            csv.Read();
            csv.ReadHeader();

            var cols = csv.Context.HeaderRecord;
            if (!cols.Contains("First") || !cols.Contains("Last"))
            {
                throw new UserInputException("Both First and Last are required");
            }

            if (!cols.Any(name => new[] { "Birthday", "Email", "Phone", "Phone2", "Phone3" }.Contains(name)))
            {
                throw new UserInputException("One of Birthday, Email, Phone, Phone2 or Phone3 is required");
            }

            var list = new List<FindInfo>();

            while (csv.Read())
            {
                var row = new FindInfo();
                foreach (var c in cols)
                {
                    switch (c)
                    {
                        case "First":
                            row.First = csv["First"];
                            break;
                        case "Last":
                            row.Last = csv["Last"];
                            break;
                        case "Birthday":
                            row.Birthday = csv["Birthday"].ToDate();
                            break;
                        case "Email":
                            row.Email = csv["Email"];
                            break;
                        case "Phone":
                            row.Phone = csv["Phone"].GetDigits();
                            break;
                        case "Phone2":
                            row.Phone2 = csv["Phone2"].GetDigits();
                            break;
                        case "Phone3":
                            row.Phone3 = csv["Phone3"].GetDigits();
                            break;
                    }
                };

                var pids = DbUtil.Db.FindPerson3(row.First, row.Last, row.Birthday, row.Email,
                    row.Phone, row.Phone2, row.Phone3).ToList();
                row.Found = pids.Count;
                if (pids.Count == 1)
                {
                    row.PeopleId = pids[0].PeopleId;
                }

                list.Add(row);
            }
            var q = from pi in list
                    where pi.PeopleId.HasValue
                    select pi.PeopleId;
            foreach (var pid in q.Distinct())
            {
                Person.Tag(DbUtil.Db, pid ?? 0, tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            }

            DbUtil.Db.SubmitChanges();
            return list;
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
