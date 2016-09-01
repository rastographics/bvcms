using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.Models;
using UtilityExtensions;
using HandlebarsDotNet;

namespace CmsWeb.Areas.Search.Models
{
    public class DirectoryInfo
    {
        public string Family { get; set; }
        public int FamilyId { get; set; }
        public string Name { get; set; }
        public string Suffix { get; set; }
        public string Birthday { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string CityStateZip { get; set; }
        public string Home { get; set; }
        public string Cell { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public bool? DoNotPublishPhones { get; set; }
    }
}