using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CmsData;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Models
{
    public class BatchModel
    {
        public static void UpdateOrgs(string text)
        {
            var csv = new CsvReader(new StringReader(text), true, '\t');
            var cols = csv.GetFieldHeaders();

            while (csv.ReadNextRecord())
            {
                var oid = csv[0].ToInt();
                var o = DbUtil.Db.LoadOrganizationById(oid);
                for (var c = 1; c < csv.FieldCount; c++)
                {
                    var val = csv[c].Trim();
                    var name = cols[c].Trim();
                    switch (name)
                    {
                        case "Campus":
                            if (val.AllDigits())
                            {
                                o.CampusId = val.ToInt();
                                if (o.CampusId == 0)
                                    o.CampusId = null;
                            }
                            break;
                        case "CanSelfCheckin":
                            o.CanSelfCheckin = val.ToBool2();
                            break;
                        case "RegStart":
                            o.RegStart = val.ToDate();
                            break;
                        case "RegEnd":
                            o.RegEnd = val.ToDate();
                            break;
                        case "Schedule":
                            if (val.HasValue() && val.NotEqual("None"))
                            {
                                var sc = o.OrgSchedules.FirstOrDefault();
                                var scin = Organization.ParseSchedule(val);
                                if (sc != null)
                                {
                                    sc.SchedDay = scin.SchedDay;
                                    sc.SchedTime = scin.SchedTime;
                                }
                                else
                                    o.OrgSchedules.Add(scin);
                            }
                            if (val.Equal("None"))
                                DbUtil.Db.OrgSchedules.DeleteAllOnSubmit(o.OrgSchedules);
                            break;
                        case "BirthDayStart":
                            o.BirthDayStart = val.ToDate();
                            break;
                        case "BirthDayEnd":
                            o.BirthDayEnd = val.ToDate();
                            break;
                        case "EntryPoint":
                            if (val.AllDigits())
                            {
                                var id = val.ToInt();
                                if (id > 0)
                                    o.EntryPointId = id;
                            }
                            break;
                        case "LeaderType":
                            if (val.AllDigits())
                            {
                                var id = val.ToInt();
                                if (id > 0)
                                    o.LeaderMemberTypeId = id;
                            }
                            break;
                        case "SecurityType":
                            o.SecurityTypeId = val.Equal("LeadersOnly") ? 2 : val.Equal("UnShared") ? 3 : 0;
                            break;
                        case "FirstMeeting":
                            o.FirstMeetingDate = val.ToDate();
                            break;
                        case "Gender":
                            o.GenderId = val.Equal("Male") ? 1 : val.Equal("Female") ? (int?) 2 : null;
                            break;
                        case "GradeAgeStart":
                            o.GradeAgeStart = val.ToInt2();
                            break;
                        case "MainFellowshipOrg":
                            o.IsBibleFellowshipOrg = val.ToBool2();
                            break;
                        case "LastDayBeforeExtra":
                            o.LastDayBeforeExtra = val.ToDate();
                            break;
                        case "LastMeeting":
                            o.LastMeetingDate = val.ToDate();
                            break;
                        case "Limit":
                            o.Limit = val.ToInt2();
                            break;
                        case "Location":
                            o.Location = val;
                            break;
                        case "AppCategory":
                            o.AppCategory = val;
                            break;
                        case "PublicSortOrder":
                            o.PublicSortOrder = val;
                            break;
                        case "UseRegisterLink2":
                            o.UseRegisterLink2 = val.ToBool2();
                            break;
                        case "Name":
                            o.OrganizationName = val;
                            break;
                        case "NoSecurityLabel":
                            o.NoSecurityLabel = val.ToBool2();
                            break;
                        case "NumCheckInLabels":
                            o.NumCheckInLabels = val.ToInt2();
                            break;
                        case "NumWorkerCheckInLabels":
                            o.NumWorkerCheckInLabels = val.ToInt2();
                            break;
                        case "OnLineCatalogSort":
                            o.OnLineCatalogSort = val == "0" ? null : val.ToInt2();
                            break;
                        case "OrganizationStatusId":
                            o.OrganizationStatusId = val.ToInt();
                            break;
                        case "PhoneNumber":
                            o.PhoneNumber = val;
                            break;
                        case "Description":
                            o.Description = val;
                            break;
                        case "RollSheetVisitorWks":
                            o.RollSheetVisitorWks = val == "0" ? null : val.ToInt2();
                            break;

                        default:
                            if (name.EndsWith(".ev"))
                                if (val.HasValue())
                                {
                                    var a = name.Substring(0, name.Length - 3);
                                    o.AddEditExtraData(a, val);
                                }
                            break;
                    }
                    DbUtil.Db.SubmitChanges();
                }
            }
        }
        public static List<FindInfo> FindTagPeople(string text, string tagname, ref string error)
        {
            var csv = new CsvReader(new StringReader(text), false, '\t').ToList();
            if (!csv.Any())
            {
                error = "No Data";
                return null;
            }
            var line0 = csv.First().Select(vv => vv.TrimEnd()).ToList();
            if (!line0.Contains("First") || !line0.Contains("Last"))
            {
                error = "Both First and Last are required";
                return null;
            }
            if (!line0.Any(name => new[] { "Birthday", "Email", "CellPhone", "HomePhone" }.Contains(name)))
            {
                error = "One of Birthday, Email, CellPhone or HomePhone is required";
                return null;
            }
            var list = new List<FindInfo>();

            var names = line0.ToDictionary(i => i.TrimEnd(), i => line0.FindIndex(s => s == i));
            foreach (var a in csv.Skip(1))
            {
                var row = new FindInfo
                {
                    First = FindColumn(names, a, "First"),
                    Last = FindColumn(names, a, "Last"),
                    Birthday = FindColumnDate(names, a, "Birthday"),
                    Email = FindColumn(names, a, "Email"),
                    CellPhone = FindColumnDigits(names, a, "CellPhone"),
                    HomePhone = FindColumnDigits(names, a, "HomePhone")
                };

                var pids = DbUtil.Db.FindPerson3(row.First, row.Last, row.Birthday, row.Email, row.CellPhone, row.HomePhone, null).ToList();
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
        private static string FindColumn(Dictionary<string, int> names, string[] a, string col)
        {
            if (names.ContainsKey(col))
                return a[names[col]];
            return null;
        }
        private static string FindColumnDigits(Dictionary<string, int> names, string[] a, string col)
        {
            var s = FindColumn(names, a, col);
            if (s.HasValue())
                return s.GetDigits();
            return s;
        }
        private static DateTime? FindColumnDate(Dictionary<string, int> names, string[] a, string col)
        {
            var s = FindColumn(names, a, col);
            DateTime dt;
            if (names.ContainsKey(col))
                if (DateTime.TryParse(a[names[col]], out dt))
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
            public string CellPhone { get; set; }
            public string HomePhone { get; set; }
            public DateTime? Birthday { get; set; }
        }
    }
}