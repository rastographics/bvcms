using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Xml;
using System.Web.Mvc;
using System.Xml.Linq;
using CmsData.API;
using CmsData.View;
using Thinktecture.IdentityModel.Extensions;
using UtilityExtensions;
using System.Linq;
using CmsData;

namespace CmsWeb.Models
{
    public class FamilyResult : ActionResult
    {
        readonly int fid;
        readonly int campus;
        readonly int thisday;
        readonly int page;
        readonly bool waslocked;
        private int timeZoneOffset;
        private DateTime dateTimeNow;
        private bool useOldLeadtimeMethod;

        public FamilyResult(int fid, int campus, int thisday, int page, bool waslocked)
        {
            this.fid = fid;
            this.campus = campus;
            this.thisday = thisday;
            this.page = page;
            this.waslocked = waslocked;
            timeZoneOffset = DbUtil.Db.Setting("TZOffset", "0").ToInt();
            dateTimeNow = DateTime.Now;
            useOldLeadtimeMethod = DbUtil.Db.Setting("UseOldLeadtimeMethod", "false").ToBool(); 
            if (fid > 0)
            {
                var db = new CMSDataContext(Util.ConnectionString);
                var lockf = db.FamilyCheckinLocks.SingleOrDefault(f => f.FamilyId == fid);
                if (lockf == null)
                {
                    lockf = new FamilyCheckinLock { FamilyId = fid, Created = DateTime.Now };
                    db.FamilyCheckinLocks.InsertOnSubmit(lockf);
                }
                lockf.Locked = true;
                if (!waslocked)
                    lockf.Created = DateTime.Now;
                db.SubmitChanges();
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings() { Encoding = new System.Text.UTF8Encoding(false), Indent = true };
            using (var w = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
            {
                var x = new APIWriter(w) {NoDefaults = true};
                x.Start("Attendees");
                var m = new CheckInModel();
                List<CheckinFamilyMember> q;
                if (CheckInModel.UseOldCheckin())
                    q = m.FamilyMembersOld(fid, campus, thisday);
                else
                    q = m.FamilyMembers(fid, campus, thisday);

                x.Attr("familyid", fid);
                x.Attr("waslocked", waslocked);

                var count = q.Count();

                if (page > 0)
                {
                    const int INT_PageSize = 10;
                    var startrow = (page - 1) * INT_PageSize;
                    if (count > startrow + INT_PageSize)
                        x.Attr("next", (page + 1));
                    else
                        x.Attr("next", "");
                    if (page > 1)
                        x.Attr("prev", (page - 1));
                    else
                        x.Attr("prev", "");
                    q = q.Skip(startrow).Take(INT_PageSize).ToList();
                }
                x.Attr("maxlabels", DbUtil.Db.Setting("MaxLabels", "6"));
                var code = DbUtil.Db.NextSecurityCode(DateTime.Today).Select(c => c.Code).Single();
                x.Attr("securitycode", code);

                var accommodateCheckInBug = DbUtil.Db.Setting("AccommodateCheckinBug", "false").ToBool();

                foreach (var c in q)
                {
                    var leadtime = LeadTime(c.Hour);

                    //---------------------------------------------------------------
                    //Remove this code once satisfied the new way works
                    if (useOldLeadtimeMethod)
                        if (c.Hour.HasValue)
                        {
                            var midnight = c.Hour.Value.Date;
                            var now = midnight.Add(Util.Now.TimeOfDay);
                            leadtime = c.Hour.Value.Subtract(now).TotalHours;
                            leadtime -= DbUtil.Db.Setting("TZOffset", "0").ToInt();
                        }
                    //---------------------------------------------------------------

                    x.Start("attendee");
                    x.Attr("id", c.Id.ToString());
                    x.Attr("mv", c.MemberVisitor);
                    x.Attr("name", c.DisplayName);
                    x.Attr("preferredname", c.PreferredName);
                    x.Attr("first", accommodateCheckInBug ? c.PreferredName : c.First);
                    x.Attr("last", c.Last);
                    x.Attr("org", c.DisplayClass);
                    x.Attr("orgname", c.OrgName);
                    x.Attr("leader", c.Leader);
                    x.Attr("orgid", c.OrgId.ToString());
                    x.Attr("loc", c.Location);
                    x.Attr("gender", c.Genderid);
                    x.Attr("leadtime", leadtime.ToString());
                    x.Attr("age", c.Age.ToString());
                    x.Attr("numlabels", c.NumLabels.ToString());
                    x.Attr("checkedin", c.CheckedIn.ToString());
                    x.Attr("custody", c.Custody.ToString());
                    x.Attr("transport", c.Transport.ToString());
                    x.Attr("hour", c.Hour.FormatDateTm());
                    x.Attr("requiressecuritylabel", c.RequiresSecurityLabel.ToString());
                    x.Attr("church", c.Church);

                    x.Attr("email", c.Email);
                    x.Attr("dob", c.dob);
                    x.Attr("goesby", c.Goesby);
                    x.Attr("addr", c.Addr);
                    x.Attr("zip", c.Zip);
                    x.Attr("home", c.Home);
                    x.Attr("cell", c.Cell);
                    x.Attr("marital", c.Marital.ToString());
                    x.Attr("allergies", c.Allergies);
                    x.Attr("grade", c.Grade.ToString());
                    x.Attr("parent", c.Parent);
                    x.Attr("emfriend", c.Emfriend);
                    x.Attr("emphone", c.Emphone);
                    x.Attr("activeother", c.Activeother.ToString());
                    x.Attr("haspicture", c.HasPicture.ToString());

                    x.End();
                }
                x.End();
            }
        }
        public double LeadTime(DateTime? MeetingTime)
        {
            if (!MeetingTime.HasValue)
                return 0;
            var MeetingTimeOnServer = MeetingTime.Value.Date.AddHours(-timeZoneOffset).Add(MeetingTime.Value.TimeOfDay);
            var CurrentTimeOnServer = MeetingTimeOnServer.Date.Add(dateTimeNow.TimeOfDay);
            var leadtime = MeetingTimeOnServer.Subtract(CurrentTimeOnServer).TotalHours;
            //new { timeZoneOffset, dateTimeNow, MeetingTime, MeetingTimeOnServer, CurrentTimeOnServer, leadtime }.Dump();
            return leadtime;
        }
    }
}