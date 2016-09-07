using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
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
        public int PeopleId { get; set; }
        public string Family { get; set; }
        public int FamilyId { get; set; }
        public string FullName { get; set; }
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
        public bool PhonesOk { get; set; }
        public Picture Picture { get; set; }
        public bool HasAccess { get; set; }
        public int GenderId { get; set; }
        public string SmallPic => GenderId == 1 ? Picture.SmallMaleUrl : GenderId == 2 ? Picture.SmallFemaleUrl : Picture.SmallUrl;
        public string MediumPic => GenderId == 1 ? Picture.MediumMaleUrl : GenderId == 2 ? Picture.MediumFemaleUrl : Picture.MediumUrl;
        public string PicPos => Picture.PortraitBgPos;
    }
}