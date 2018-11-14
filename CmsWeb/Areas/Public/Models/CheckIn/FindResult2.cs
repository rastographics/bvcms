using CmsData;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class FindResult2 : ActionResult
    {
        private int fid;
        private readonly string building;
        private readonly string querybit;

        public FindResult2(int fid, string building, string querybit)
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
                var q =
                    from p in DbUtil.Db.People
                    let notes = p.PeopleExtras.SingleOrDefault(ee => ee.Field == building + "-notes").Data
                    let accessflag = p.PeopleExtras.SingleOrDefault(ea => ea.Field == building + "-access").StrValue
                    where p.FamilyId == fid
                    where p.DeceasedDate == null
                    orderby p.PositionInFamilyId, p.PositionInFamilyId == 10 ? p.Gender.Code : "U", p.Age
                    select new
                    {
                        Id = p.PeopleId,
                        Position = p.PositionInFamilyId,
                        First = p.FirstName,
                        Last = p.LastName,
                        dob = p.DOB,
                        Age = p.Age ?? 0,
                        Gender = p.Gender.Code,

                        goesby = p.NickName,
                        email = p.EmailAddress,
                        addr = p.Family.AddressLineOne,
                        zip = p.Family.ZipCode,
                        home = p.Family.HomePhone,
                        cell = p.CellPhone,
                        marital = p.MaritalStatusId,
                        gender = p.GenderId,
                        grade = p.Grade,
                        HasPicture = p.PictureId != null,
                        MemberStatus = p.MemberStatus.Code,
                        MemberStatusId = p.MemberStatus.Id.ToString(),
                        notes,
                        access = accessflag == "false" ? "restricted" : p.Tags.Any(t => t.Tag.Name == querybit).ToString().ToLower()
                    };

                w.WriteAttributeString("familyid", fid.ToString());

                foreach (var c in q)
                {
                    w.WriteStartElement("member");
                    w.WriteAttributeString("id", c.Id.ToString());
                    w.WriteAttributeString("first", c.First);
                    w.WriteAttributeString("last", c.Last);
                    w.WriteAttributeString("gender", c.gender.ToString());
                    w.WriteAttributeString("age", Person.AgeDisplay(c.Age, c.Id).ToString());

                    w.WriteAttributeString("email", c.email);
                    w.WriteAttributeString("dob", c.dob);
                    w.WriteAttributeString("goesby", c.goesby);
                    w.WriteAttributeString("addr", c.addr);
                    w.WriteAttributeString("zip", c.zip);
                    w.WriteAttributeString("home", c.home);
                    w.WriteAttributeString("cell", c.cell);
                    w.WriteAttributeString("marital", c.marital.ToString());
                    w.WriteAttributeString("grade", c.grade.ToString());
                    w.WriteAttributeString("haspicture", c.HasPicture.ToString());
                    w.WriteAttributeString("memberstatus", c.MemberStatus);
                    w.WriteAttributeString("memberstatusid", c.MemberStatusId);
                    w.WriteAttributeString("access", c.access);

                    int visits = (from e in DbUtil.Db.CheckInTimes
                                  where e.PeopleId == c.Id
                                  where e.Location == building
                                  where e.AccessTypeID == 2
                                  select e).Count();

                    w.WriteAttributeString("visitcount", visits.ToString());

                    if (c.notes.HasValue())
                    {
                        w.WriteString(c.notes);
                    }

                    w.WriteEndElement();
                }
                w.WriteEndElement();
            }
        }
    }
}
