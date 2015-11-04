using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;
using HtmlAgilityPack;

namespace CmsWeb.MobileAPI
{
    public class MobileRegistrationCategory
    {
        public bool Current { get; set; }
        public string Title { get; set; }
        public List<MobileRegistration> Registrations { get; set; }
    }
    public class MobileRegistration
    {
        public int OrgId { get; set; }
        public string Name { get; set; }
        public string MoreInfo { get; set; }
        public string Category { get; set; }
        public bool UseRegisterLink2 { get; set; }
        public DateTime? RegStart { get; set; }
        public DateTime? RegEnd { get; set; }
        public string PublicSortOrder { get; set; }

        private string description;

        public string Description
        {
            get { return description; }
            set
            {
                if (!value.HasValue())
                {
                    description = null;
                    MoreInfo = null;
                    return;
                }
                var doc = new HtmlDocument();
                doc.LoadHtml(value);
                var linkList = doc.DocumentNode.SelectNodes("//a[@href]");
                if (linkList == null)
                {
                    description = doc.DocumentNode.OuterHtml;
                    return;
                }
                MoreInfo = linkList[0].Attributes["href"].Value;
                foreach (var link in linkList)
                    link.Remove();
                description = doc.DocumentNode.OuterHtml;
            }
        }
    }
}