using CmsData;
using CmsWeb.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Giving.Models
{
    public class GivingPageModel : IDbBinder
    {
        public GivingPageModel(){}
        public GivingPageModel(CMSDataContext db)
        {
            CurrentDatabase = db;
        }
        private CMSDataContext _db;
        public CMSDataContext CurrentDatabase
        {
            get => _db;
            set
            {
                _db = value;
            }
        }

        public HashSet<GivingPageItem> GetGivingPageHashSet()
        {
            var outputHashSet = new HashSet<GivingPageItem>();
            var response = from gp in CurrentDatabase.GivingPages
                           select new
                           {
                               GivingPageId = gp.GivingPageId,
                               Enabled = gp.Enabled,
                               PageName = gp.PageName,
                               Skin = gp.SkinFile,
                               PageType = gp.PageType,
                               DefaultFund = gp.ContributionFund.FundName
                           };
            var myHashSet = response.ToHashSet();
            foreach (var item in myHashSet)
            {
                var givingPage = new GivingPageItem
                {
                    GivingPageId = item.GivingPageId,
                    Enabled = item.Enabled,
                    PageName = item.PageName,
                    Skin = item.Skin,
                    PageType = item.PageType.ToString(),
                    DefaultFund = item.DefaultFund
                };
                outputHashSet.Add(givingPage);
            }
            return outputHashSet;
        }
    }
}

public class GivingPageItem
{
    public int GivingPageId { get; set; }
    public bool Enabled { get; set; }
    public string PageName { get; set; }
    public string Skin { get; set; }
    public string PageType { get; set; }
    public string DefaultFund { get; set; }
}

public class PageTypesClass
{
    public int id { get; set; }
    public string pageTypeName { get; set; }
}
