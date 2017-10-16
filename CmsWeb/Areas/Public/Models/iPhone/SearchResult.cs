using System;
using System.Collections.Generic;
using System.Xml;
using System.Web.Mvc;
using System.Xml.Linq;
using UtilityExtensions;
using System.Linq;
using CmsData;

namespace CmsWeb.Models.iPhone
{
    public class SearchResult : ActionResult
    {
        private readonly List<PeopleInfo> items;
        private readonly int count;
        public SearchResult(IQueryable<Person> items)
        {
            this.items = PeopleList(items).ToList();
            count = this.items.Count;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings();
            settings.Encoding = new System.Text.UTF8Encoding(false);
            settings.Indent = true;

            using (var w = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
            {
                w.WriteStartElement("SearchResults");
                w.WriteAttributeString("count", count.ToString());

                foreach (var p in items)
                {
                    w.WriteStartElement("Person");
                    w.WriteAttributeString("peopleid", p.PeopleId.ToString());
                    w.WriteAttributeString("name", p.Name);
                    w.WriteAttributeString("address", p.Address);
                    w.WriteAttributeString("citystatezip", p.CityStateZip);
                    w.WriteAttributeString("zip", p.Zip);
                    w.WriteAttributeString("homephone", p.HomePhone);
                    w.WriteAttributeString("age", Person.AgeDisplay(p.Age, p.PeopleId).ToString());
                    w.WriteEndElement();
                }
                w.WriteEndElement();
            }
        }
        public static IEnumerable<PeopleInfo> PeopleList(IQueryable<Person> query)
        {
            var q = from p in query
                    select new PeopleInfo
                    {
                        PeopleId = p.PeopleId,
                        Name = p.Name,
                        First = p.FirstName,
                        Last = p.LastName,
                        Address = p.PrimaryAddress,
                        CityStateZip = p.PrimaryCity + ", " + p.PrimaryState + " " + p.PrimaryZip.Substring(0, 5),
                        Zip = p.PrimaryZip.Substring(0, 5),
                        Age = p.Age,
                        BirthYear = p.BirthYear,
                        BirthMon = p.BirthMonth,
                        BirthDay = p.BirthDay,
                        HomePhone = p.HomePhone,
                        CellPhone = p.CellPhone,
                        WorkPhone = p.WorkPhone,
                        MemberStatus = p.MemberStatus.Description,
                        Email = p.EmailAddress,
                        HasPicture = p.PictureId != null,
                    };
            return q;
        }
    }
}
