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
    public class SearchResult0 : ActionResult
    {
        private readonly List<PeopleInfo> items;
        private readonly int count;
        public SearchResult0(IQueryable<Person> items)
        {
            this.items = SearchResult.PeopleList(items).ToList();
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
                w.WriteStartElement("People");
                w.WriteAttributeString("count", count.ToString());

                foreach (var p in items)
                {
                    w.WriteStartElement("Person");
                    w.WriteAttributeString("peopleid", p.PeopleId.ToString());
                    w.WriteAttributeString("name", p.Name);
                    w.WriteAttributeString("first", p.First);
                    w.WriteAttributeString("last", p.Last);
                    w.WriteAttributeString("address", p.Address);
                    w.WriteAttributeString("citystatezip", p.CityStateZip);
                    w.WriteAttributeString("zip", p.Zip);
                    w.WriteAttributeString("age", Person.AgeDisplay(p.Age, p.PeopleId).ToString());
                    w.WriteAttributeString("birthdate", p.BirthDate);
                    w.WriteAttributeString("homephone", p.HomePhone);
                    w.WriteAttributeString("cellphone", p.CellPhone);
                    w.WriteAttributeString("workphone", p.WorkPhone);
                    w.WriteAttributeString("memberstatus", p.MemberStatus);
                    w.WriteAttributeString("email", p.Email);
                    w.WriteAttributeString("haspicture", p.HasPicture ? "1": "0");
                    w.WriteEndElement();
                }
                w.WriteEndElement();
            }
        }
    }
}
