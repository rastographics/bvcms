using CmsData;
using CsvHelper;
using System.IO;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Models.BatchModel
{
    public partial class BatchModel
    {
        public BatchModel() { }
        public static void UpdateOrgs(string text)
        {
            text = text.Trim();
            var csv = new CsvReader(new StringReader(text));
            csv.Configuration.Delimiter = "\t";

            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var oid = csv.GetField<int>(0);
                var o = DbUtil.Db.LoadOrganizationById(oid);
                for (var c = 1; c < csv.Context.HeaderRecord.Length; c++)
                {
                    var val = csv.GetField<string>(c);
                    var name = csv.Context.HeaderRecord[c];
                    switch (name)
                    {
                        case "Campus":
                            if (val.AllDigits())
                            {
                                o.CampusId = val.ToInt();
                                if (o.CampusId == 0)
                                {
                                    o.CampusId = null;
                                }
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
                                {
                                    o.OrgSchedules.Add(scin);
                                }
                            }
                            if (val.Equal("None"))
                            {
                                DbUtil.Db.OrgSchedules.DeleteAllOnSubmit(o.OrgSchedules);
                            }

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
                                {
                                    o.EntryPointId = id;
                                }
                            }
                            break;
                        case "LeaderType":
                            if (val.AllDigits())
                            {
                                var id = val.ToInt();
                                if (id > 0)
                                {
                                    o.LeaderMemberTypeId = id;
                                }
                            }
                            break;
                        case "FirstMeeting":
                            o.FirstMeetingDate = val.ToDate();
                            break;
                        case "Gender":
                            o.GenderId = val.Equal("Male") ? 1 : val.Equal("Female") ? (int?)2 : null;
                            break;
                        case "Grade":
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
                        case "LimitToRole":
                            if (val.Equal("none"))
                            {
                                o.LimitToRole = null;
                            }
                            else if (o.LimitToRole.HasValue())
                            {
                                o.LimitToRole = val;
                            }

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
                        case "SubGroups":
                            if (val == "(clear)")
                            {
                                DbUtil.Db.MemberTags.DeleteAllOnSubmit(o.MemberTags);
                            }
                            else
                            {
                                foreach (var sg in val.Split(','))
                                {
                                    o.AddMemberTag(sg);
                                }
                            }

                            break;

                        default:
                            if (name.EndsWith(".ev"))
                            {
                                if (val.HasValue())
                                {
                                    var a = name.Substring(0, name.Length - 3);
                                    o.AddEditExtraText(a, val);
                                }
                            }

                            break;
                    }
                    DbUtil.Db.SubmitChanges();
                }
            }
        }
    }
}
