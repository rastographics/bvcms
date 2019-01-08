using CmsData;
using CmsData.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class ClassesResult : ActionResult
    {
        private readonly DateTime? bd;
        private readonly int campusid;
        private readonly int familyid;
        private readonly bool kioskmode;
        private readonly int thisday;
        private int? grade;
        private int? peopleid;

        public ClassesResult(int id, int thisday, int campusid, bool noagecheck, bool kioskmode)
        {
            this.thisday = thisday;
            this.campusid = campusid;
            this.kioskmode = kioskmode;
            var i = (from p in DbUtil.Db.People
                     where p.PeopleId == id
                     select new
                     {
                         p.FamilyId,
                         p.BirthDate,
                         p.Grade
                     }).SingleOrDefault();
            if (i == null)
            {
                return;
            }

            familyid = i.FamilyId;
            peopleid = id;
            bd = i.BirthDate;
            grade = i.Grade;

            this.noagecheck = noagecheck;
        }

        public bool noagecheck { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (!peopleid.HasValue)
            {
                return;
            }

            context.HttpContext.Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            using (var w = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
            {
                w.WriteStartElement("Results");
                w.WriteAttributeString("pid", peopleid.ToString());
                w.WriteAttributeString("fid", familyid.ToString());
                IEnumerable<OrgHourInfo> q;
                if (kioskmode)
                {
                    q = from o in DbUtil.Db.Organizations //todo: change this to work with multiple schedules
                        let bdaystart = o.BirthDayStart ?? DateTime.MaxValue
                        let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                        let tm = sc != null ? (sc.SchedTime ?? DateTime.Today) : DateTime.Today
                        //let meetingHours = DbUtil.Db.GetTodaysMeetingHours(o.OrganizationId, (int)DateTime.Now.DayOfWeek)
                        where bd == null || bd <= o.BirthDayEnd || o.BirthDayEnd == null || noagecheck
                        where bd == null || bd >= o.BirthDayStart || o.BirthDayStart == null || noagecheck
                        where o.AllowKioskRegister == true
                        where (o.ClassFilled ?? false) == false
                        where (o.CampusId == null && o.AllowNonCampusCheckIn == true)
                              || o.CampusId == campusid || campusid == 0
                        where o.OrganizationStatusId == OrgStatusCode.Active
                        orderby bdaystart, o.OrganizationName
                        //from meeting in meetingHours
                        select new OrgHourInfo
                        {
                            o = o,
                            Hour = DateTime.Today + tm.TimeOfDay
                        };
                }
                else
                {
                    q = from o in DbUtil.Db.Organizations
                        let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                        let meetingHours = DbUtil.Db.GetTodaysMeetingHours(o.OrganizationId, thisday)
                        let bdaystart = o.BirthDayStart ?? DateTime.MaxValue
                        where (o.SuspendCheckin ?? false) == false || noagecheck
                        where bd == null || bd <= o.BirthDayEnd || o.BirthDayEnd == null || noagecheck
                        where bd == null || bd >= o.BirthDayStart || o.BirthDayStart == null || noagecheck
                        where o.CanSelfCheckin == true
                        where (o.ClassFilled ?? false) == false
                        where (o.CampusId == null && o.AllowNonCampusCheckIn == true)
                              || o.CampusId == campusid || campusid == 0
                        where o.OrganizationStatusId == OrgStatusCode.Active
                        orderby sc.SchedTime.Value.TimeOfDay, bdaystart, o.OrganizationName
                        from meeting in meetingHours
                        select new OrgHourInfo { o = o, Hour = meeting.Hour.Value };
                }

                var q2 = from i in q
                         select new
                         {
                             i.o.LeaderName,
                             i.o.OrganizationId,
                             i.o.Location,
                             i.o.BirthDayStart,
                             i.o.BirthDayEnd,
                             i.Hour,
                             i.o.OrganizationName,
                             i.o.NumCheckInLabels,
                             i.o.Limit,
                             MemberCount = i.o.OrganizationMembers.Count(om =>
                                 om.MemberTypeId == MemberTypeCode.Member
                                 && om.Pending != true
                                 )
                         };

                foreach (var o in q2)
                {
                    double leadtime = 0;

                    if (o.Hour.HasValue)
                    {
                        var theirTime = DateTime.Now.AddHours(DbUtil.Db.Setting("TZOffset", "0").ToInt());

                        if (DateTime.Now.DayOfWeek.ToInt() != thisday)
                        {
                            int dayDiff = thisday - DateTime.Now.DayOfWeek.ToInt();

                            if (dayDiff < 0)
                            {
                                theirTime = theirTime.AddDays(7 + dayDiff);
                            }
                            else
                            {
                                theirTime = theirTime.AddDays(dayDiff);
                            }
                        }

                        leadtime = o.Hour.Value.Subtract(theirTime).TotalHours;

                        //var midnight = o.Hour.Value.Date;
                        //var now = midnight.Add(Util.Now.TimeOfDay);
                        //leadtime = o.Hour.Value.Subtract(now).TotalHours;
                        //leadtime -= DbUtil.Db.Setting("TZOffset", "0").ToInt(); // positive to the east, negative to the west
                    }

                    w.WriteStartElement("class");
                    w.WriteAttributeString("orgid", o.OrganizationId.ToString());
                    var loc = o.Location;
                    if (loc.HasValue())
                    {
                        loc = ", " + loc;
                    }

                    var leader = o.LeaderName;
                    if (leader.HasValue())
                    {
                        leader = ":" + leader;
                    }

                    var bdays = $" [{o.BirthDayStart:d}-{o.BirthDayEnd:d}]";
                    if (bdays == " [-]")
                    {
                        bdays = null;
                    }

                    if (kioskmode)
                    {
                        w.WriteAttributeString("display", $"{o.OrganizationName}{leader}{loc}{bdays} ({((o.Limit ?? 0) == 0 ? "" : o.Limit + "max, ")}{o.MemberCount})");
                    }
                    else
                    {
                        w.WriteAttributeString("display", $"{o.Hour:t} {o.OrganizationName}{leader}{loc}{bdays}");
                    }

                    w.WriteAttributeString("nlabels", o.NumCheckInLabels.ToString());
                    w.WriteAttributeString("hour", Util.IsCultureUS() ? o.Hour.FormatDateTm() : o.Hour.FormatDateTmUS());
                    w.WriteAttributeString("leadtime", leadtime.ToString());
                    w.WriteEndElement();
                }
                w.WriteEndElement();
            }
        }

        private class OrgHourInfo
        {
            public Organization o { get; set; }
            public DateTime? Hour { get; set; }
        }
    }
}
