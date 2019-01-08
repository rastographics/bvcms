using CmsData;
using CmsData.API;
using CmsData.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class FamilyResult : ActionResult
    {
        private readonly int fid, campus, thisday, page;
        private readonly bool waslocked;

        public FamilyResult(int fid, int campus, int thisday, int page, bool waslocked)
        {
            this.fid = fid;
            this.campus = campus;
            this.thisday = thisday;
            this.page = page;
            this.waslocked = waslocked;
            if (fid > 0)
            {
                var db = DbUtil.Create(Util.Host);
                var lockf = DbUtil.Db.FamilyCheckinLocks.SingleOrDefault(f => f.FamilyId == fid);
                if (lockf == null)
                {
                    lockf = new FamilyCheckinLock { FamilyId = fid, Created = DateTime.Now };
                    DbUtil.Db.FamilyCheckinLocks.InsertOnSubmit(lockf);
                }
                lockf.Locked = true;
                if (!waslocked)
                {
                    lockf.Created = DateTime.Now;
                }

                DbUtil.Db.SubmitChanges();
            }
        }


        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings() { Encoding = new System.Text.UTF8Encoding(false), Indent = true };
            using (var w = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
            {
                var x = new APIWriter(w);
                x.NoDefaults = true;
                x.Start("Attendees");
                var m = new CheckInModel();
                List<CheckinFamilyMember> q;
                if (CheckInModel.UseOldCheckin())
                {
                    q = m.FamilyMembersOld(fid, campus, thisday);
                }
                else
                {
                    q = m.FamilyMembers(fid, campus, thisday);
                }

                x.Attr("familyid", fid);
                x.Attr("waslocked", waslocked);

                var count = q.Count();

                if (page > 0)
                {
                    const int INT_PageSize = 10;
                    var startrow = (page - 1) * INT_PageSize;
                    if (count > startrow + INT_PageSize)
                    {
                        x.Attr("next", (page + 1));
                    }
                    else
                    {
                        x.Attr("next", "");
                    }

                    if (page > 1)
                    {
                        x.Attr("prev", (page - 1));
                    }
                    else
                    {
                        x.Attr("prev", "");
                    }

                    q = q.Skip(startrow).Take(INT_PageSize).ToList();
                }
                x.Attr("maxlabels", DbUtil.Db.Setting("MaxLabels", "6"));

                // TODO: Consider the option to numbers only. Per Braden Kok @ Granite Springs
                var code = DbUtil.Db.NextSecurityCode().Select(c => c.Code).Single();
                x.Attr("securitycode", code);

                var accommodateCheckInBug = DbUtil.Db.Setting("AccommodateCheckinBug", "false").ToBool();

                foreach (var c in q)
                {
                    var parents = "";

                    if (c.Position == 30)
                    {
                        var child = (from e in DbUtil.Db.People
                                     where e.PeopleId == c.Id
                                     select e).SingleOrDefault();

                        if (child.Family.HeadOfHouseholdId != null)
                        {
                            parents = child.Family.HeadOfHousehold.FirstName;

                            if (child.Family.HeadOfHouseholdSpouseId != null)
                            {
                                parents += " & " + child.Family.HeadOfHouseholdSpouse.FirstName;
                            }
                        }
                        else if (child.Family.HeadOfHouseholdSpouseId != null)
                        {
                            parents = child.Family.HeadOfHouseholdSpouse.FirstName;
                        }
                    }

                    double hoursBeforeClassStarts = 0;
                    if (c.Hour.HasValue)
                    {
                        var midnight = c.Hour.Value.Date;
                        var now = midnight.Add(Util.Now.TimeOfDay);
                        hoursBeforeClassStarts = c.Hour.Value.Subtract(now).TotalHours;
                        // TZOffset will be positive to the east, negative to the west
                        // but we are trying to get to central time so we subtract
                        // if tzoffset is +1 then we need to -1 to go to CentralTime.
                        hoursBeforeClassStarts -= DbUtil.Db.Setting("TZOffset", "0").ToInt();
                        // now we need to make sure we are within 24 hours (ignore the date change)
                        hoursBeforeClassStarts %= 24;
                    }
                    string hour = Util.IsCultureUS() ? c.Hour.FormatDateTm() : c.Hour.FormatDateTmUS();

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
                    x.Attr("leadtime", hoursBeforeClassStarts.ToString());
                    x.Attr("age", Person.AgeDisplay(c.Age, c.Id).ToString());
                    x.Attr("numlabels", c.NumLabels.ToString());
                    x.Attr("checkedin", c.CheckedIn.ToString());
                    x.Attr("custody", c.Custody.ToString());
                    x.Attr("transport", c.Transport.ToString());
                    x.Attr("hour", hour);
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

                    x.Attr("parents", parents);

                    x.End();
                }
                x.End();
            }
        }
    }
}
