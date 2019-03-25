using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;
using System.Xml.XPath;
using CmsData;
using MoreLinq;

namespace CmsWeb.Areas.Figures.Models
{
    public class OrgModel
    {
        public Int32 Id { get; set; }
        public String OrgName { get; set; }
        public OrgModel() { }
    }

    public class DivModel
    {
        public Int32 Id { get; set; }
        public String DivName { get; set; }
        public List<OrgModel> OrgList = new List<OrgModel>();
        public DivModel() { }
    }

    public class ProgModel
    {
        public Int32 Id { get; set; }
        public String ProgName { get; set; }
        public List<DivModel> DivList = new List<DivModel>();
        public ProgModel() { }
    }

    public class FundModel
    {
        public Int32 Id { get; set; }
        public String FundName { get; set; }
        public FundModel() { }
    }

}
