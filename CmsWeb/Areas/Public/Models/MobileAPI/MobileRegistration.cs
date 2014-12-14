using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;
using HtmlAgilityPack;

namespace CmsWeb.MobileAPI
{
    public class MobileRegistrationCategory
    {
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

        private string publicSortOrder;
        public string PublicSortOrder
        {
            get { return publicSortOrder; }
            set
            {
                publicSortOrder = value;
                var a = value.SplitStr(":", 2);
                Category = "";
                if (a.Length > 1)
                {
                    Category = a[0];
                    publicSortOrder = a[1];
                }
                else
                    publicSortOrder = value;
            }
        }

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
                description = HttpUtility.HtmlEncode(doc.DocumentNode.OuterHtml);
            }
        }
    }
}