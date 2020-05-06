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

        //public HashSet<GivingPageItem> GetGivingPageHashSet()
        //{
        //    var outputHashSet = new HashSet<GivingPageItem>();
        //    var response = from gp in CurrentDatabase.GivingPages
        //                   select new
        //                   {
        //                       GivingPageId = gp.GivingPageId,
        //                       Enabled = gp.Enabled,
        //                       PageName = gp.PageName,
        //                       Skin = gp.SkinFile,
        //                       PageType = gp.PageType,
        //                       DefaultFund = gp.ContributionFund.FundName
        //                   };
        //    var myHashSet = response.ToHashSet();
        //    foreach (var item in myHashSet)
        //    {
        //        var givingPage = new GivingPageItem
        //        {
        //            GivingPageId = item.GivingPageId,
        //            Enabled = item.Enabled,
        //            PageName = item.PageName,
        //            SkinFile = item.Skin,
        //            PageType = item.PageType.ToString(),
        //            DefaultFund = item.DefaultFund
        //        };
        //        outputHashSet.Add(givingPage);
        //    }
        //    return outputHashSet;
        //}

        public List<GivingPageItem> GetGivingPageList()
        {
            var outputList = new List<GivingPageItem>();
            var response = (from gp in CurrentDatabase.GivingPages
                            select new
                            {
                                GivingPageId = gp.GivingPageId,
                                PageName = gp.PageName,
                                PageTitle = gp.PageTitle,
                                Enabled = gp.Enabled,
                                SkinFile = gp.SkinFile,
                                PageType = gp.PageType,
                                DefaultFundId = gp.ContributionFund.FundId,
                                DisabledRedirect = gp.DisabledRedirect,
                                EntryPointId = gp.EntryPointId,
                                TopText = gp.TopText,
                                ThankYouText = gp.ThankYouText,
                                ConfirmEmailPledge = gp.ConfirmationEmailPledge,
                                ConfirmEmailOneTime = gp.ConfirmationEmailOneTime,
                                ConfirmEmailRecurring = gp.ConfirmationEmailRecurring
                            }).ToList();
            for (var i = 0; i < response.Count(); i++)
            {
                var givingPage = new GivingPageItem
                {
                    GivingPageId = response[i].GivingPageId,
                    PageName = response[i].PageName,
                    PageTitle = response[i].PageTitle,
                    Enabled = response[i].Enabled,
                    SkinFile = response[i].SkinFile,
                    //PageType = response[i].PageType.ToString(),
                    //DefaultFund = (from d in CurrentDatabase.ContributionFunds where d.FundId == response[i].DefaultFundId select d).ToArray(),
                    AvailableFunds = (from gpf in CurrentDatabase.GivingPageFunds where gpf.GivingPageId == response[i].GivingPageId select gpf).ToArray(),
                    DisabledRedirect = response[i].DisabledRedirect,
                    EntryPoint = response[i].EntryPointId.ToString(),
                    TopText = response[i].TopText,
                    ThankYouText = response[i].ThankYouText,
                    OnlineNotifyPerson = null,
                    ConfirmEmailPledge = response[i].ConfirmEmailPledge,
                    ConfirmEmailOneTime = response[i].ConfirmEmailOneTime,
                    ConfirmEmailRecurring = response[i].ConfirmEmailRecurring
                };
                var tempDefaultFund = (from d in CurrentDatabase.ContributionFunds where d.FundId == response[i].DefaultFundId select d).FirstOrDefault();
                givingPage.DefaultFund = new FundsClass()
                {
                    FundId = tempDefaultFund.FundId,
                    FundName = tempDefaultFund.FundName
                };
                var newPageType = new PageTypesClass();
                switch (response[i].PageType)
                {
                    case 1:
                        newPageType.id = 1;
                        newPageType.pageTypeName = "Pledge";
                        break;
                    case 2:
                        newPageType.id = 2;
                        newPageType.pageTypeName = "One Time";
                        break;
                    case 3:
                        newPageType.id = 3;
                        newPageType.pageTypeName = "Recurring";
                        break;
                    default:
                        break;
                }
                givingPage.PageType = newPageType;

                outputList.Add(givingPage);
            }
            return outputList;
        }
    }
}

public class GivingPageItem
{
    public int GivingPageId { get; set; }
    public string PageName { get; set; }
    public string PageTitle { get; set; }
    public bool Enabled { get; set; }
    public string SkinFile { get; set; }
    public PageTypesClass PageType { get; set; }
    public FundsClass DefaultFund { get; set; }
    public string DefaultFundId { get; set; }
    public GivingPageFund[] AvailableFunds { get; set; }
    public string DisabledRedirect { get; set; }
    public string EntryPoint { get; set; }
    public string TopText { get; set; }
    public string ThankYouText { get; set; }
    public string OnlineNotifyPerson { get; set; }
    public string ConfirmEmailPledge { get; set; }
    public string ConfirmEmailOneTime { get; set; }
    public string ConfirmEmailRecurring { get; set; }
}

public class PageTypesClass
{
    public int id { get; set; }
    public string pageTypeName { get; set; }
}
public class FundsClass
{
    public int FundId { get; set; }
    public string FundName { get; set; }
}
