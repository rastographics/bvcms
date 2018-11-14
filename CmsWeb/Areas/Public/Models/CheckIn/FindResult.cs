using CmsData;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class FindResult : ActionResult
    {
        private int fid;
        private readonly string building;
        private readonly string querybit;

        public FindResult(int fid, string building, string querybit)
        {
            this.fid = fid;
            this.building = building;
            this.querybit = querybit;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings();
            settings.Encoding = new System.Text.UTF8Encoding(false);
            settings.Indent = true;

            using (var w = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
            {
                w.WriteStartElement("Family");

                var people = from p in DbUtil.Db.People
                             where p.FamilyId == fid
                             where p.DeceasedDate == null
                             orderby p.PositionInFamilyId, p.PositionInFamilyId == 10 ? p.Gender.Code : "U", p.Age
                             select p;

                w.WriteAttributeString("familyid", fid.ToString());

                foreach (var c in people)
                {
                    var allowAccess = "false";
                    var accessExtraValue = c.PeopleExtras.SingleOrDefault(ea => ea.Field == building + "-access");
                    var accessFlag = accessExtraValue != null ? accessExtraValue.StrValue : "";

                    if (accessFlag == "false")
                    {
                        allowAccess = "restricted";
                    }
                    else if (c.Tags.Any(t => t.Tag.Name == querybit) == false)
                    {
                        allowAccess = "false";
                    }
                    else
                    {
                        var lastCheckInTime = c.CheckInTimes.OrderByDescending(la => la.CheckInTimeX).FirstOrDefault();

                        if (lastCheckInTime != null && lastCheckInTime.CheckInTimeX.Value.AddHours(5) > DateTime.Now)
                        {
                            allowAccess = "timer";
                        }
                        else
                        {
                            allowAccess = "true";
                        }
                    }

                    w.WriteStartElement("member");
                    w.WriteAttributeString("id", c.PeopleId.ToString());
                    w.WriteAttributeString("first", c.FirstName);
                    w.WriteAttributeString("last", c.LastName);
                    w.WriteAttributeString("gender", c.Gender.ToString());
                    w.WriteAttributeString("age", Person.AgeDisplay(c.Age, c.PeopleId).ToString());

                    w.WriteAttributeString("email", c.EmailAddress);
                    w.WriteAttributeString("dob", c.DOB);
                    w.WriteAttributeString("goesby", c.NickName);
                    w.WriteAttributeString("addr", c.Family.AddressLineOne);
                    w.WriteAttributeString("zip", c.Family.ZipCode);
                    w.WriteAttributeString("home", c.Family.HomePhone);
                    w.WriteAttributeString("cell", c.CellPhone);
                    w.WriteAttributeString("marital", c.MaritalStatusId.ToString());
                    w.WriteAttributeString("grade", c.Grade.ToString());
                    w.WriteAttributeString("haspicture", (c.PictureId != null).ToString());
                    w.WriteAttributeString("memberstatus", c.MemberStatus.Code);
                    w.WriteAttributeString("memberstatusid", c.MemberStatus.Id.ToString());
                    w.WriteAttributeString("access", allowAccess);

                    int visits = (from e in DbUtil.Db.CheckInTimes
                                  where e.PeopleId == c.PeopleId
                                  where e.Location == building
                                  where e.AccessTypeID == 2
                                  select e).Count();

                    w.WriteAttributeString("visitcount", visits.ToString());

                    var notes = c.PeopleExtras.SingleOrDefault(ee => ee.Field == building + "-notes");

                    if (notes != null && notes.Data.HasValue())
                    {
                        w.WriteString(notes.Data);
                    }

                    w.WriteEndElement();
                }
                w.WriteEndElement();
            }
        }
    }
}
