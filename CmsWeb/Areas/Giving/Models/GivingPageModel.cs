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
                                DefaultFundId = gp.FundId,
                                DisabledRedirect = gp.DisabledRedirect,
                                EntryPointId = gp.EntryPointId,
                                TopText = gp.TopText,
                                ThankYouText = gp.ThankYouText,
                                OnlineNotifyPerson = gp.OnlineNotifyPerson,
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
                    DisabledRedirect = response[i].DisabledRedirect,
                    TopText = response[i].TopText,
                    ThankYouText = response[i].ThankYouText,
                };
                if (response[i].DefaultFundId != null)
                {
                    var tempDefaultFund = (from d in CurrentDatabase.ContributionFunds where d.FundId == response[i].DefaultFundId select d).FirstOrDefault();
                    if (tempDefaultFund != null)
                    {
                        givingPage.DefaultFund = new FundsClass()
                        {
                            FundId = tempDefaultFund.FundId,
                            FundName = tempDefaultFund.FundName
                        };
                    }
                }

                var newPageType = new List<PageTypesClass>();
                var temp = new PageTypesClass();
                switch (response[i].PageType)
                {
                    case 1:
                        temp.id = 1;
                        temp.pageTypeName = "Pledge";
                        newPageType.Add(temp);
                        break;
                    case 3:
                        temp.id = 2;
                        temp.pageTypeName = "One Time";
                        newPageType.Add(temp);
                        break;
                    case 7:
                        temp.id = 3;
                        temp.pageTypeName = "Recurring";
                        newPageType.Add(temp);
                        break;
                    default:
                        break;
                }
                givingPage.PageType = newPageType.ToArray();
                if(newPageType.Count() > 0)
                {
                    givingPage.PageTypeString = newPageType[0].pageTypeName;
                }
                //var newPageType = new PageTypesClass();
                //switch (response[i].PageType)
                //{
                //    case 1:
                //        newPageType.id = 1;
                //        newPageType.pageTypeName = "Pledge";
                //        break;
                //    case 2:
                //        newPageType.id = 2;
                //        newPageType.pageTypeName = "One Time";
                //        break;
                //    case 3:
                //        newPageType.id = 3;
                //        newPageType.pageTypeName = "Recurring";
                //        break;
                //    default:
                //        break;
                //}
                //givingPage.PageType = newPageType;


                var tempAvailableFunds = (from gpf in CurrentDatabase.GivingPageFunds where gpf.GivingPageId == response[i].GivingPageId select new { gpf.GivingPageFundId, gpf.FundId, gpf.GivingPageId }).ToList();
                var j = 0;
                var tempAvailFundsList = new List<FundsClass>();
                foreach(var item in tempAvailableFunds)
                {
                    var tempContributionFund = (from cf in CurrentDatabase.ContributionFunds
                                       where cf.FundId == item.FundId
                                       select cf).FirstOrDefault();
                    var tempFundsClass = new FundsClass()
                    {
                        FundId = item.FundId,
                        FundName = tempContributionFund.FundName
                    };
                    tempAvailFundsList.Add(tempFundsClass);
                }
                givingPage.AvailableFunds = new FundsClass[tempAvailFundsList.Count()];
                foreach (var item in tempAvailFundsList)
                {
                    givingPage.AvailableFunds[j] = item;
                    j++;
                }
                var tempEntryPoint = (from ep in CurrentDatabase.EntryPoints where ep.Id == response[i].EntryPointId select new { ep.Id, ep.Description }).FirstOrDefault();
                if(tempEntryPoint != null)
                {
                    givingPage.EntryPoint = new EntryPointClass()
                    {
                        Id = (int)response[i].EntryPointId,
                        Description = tempEntryPoint.Description
                    };
                }

                if(response[i].OnlineNotifyPerson != null)
                {
                    var OnlineNotifyPersonList = response[i].OnlineNotifyPerson.Split(',').Select(int.Parse).ToList();
                    givingPage.OnlineNotifyPerson = new PeopleClass[OnlineNotifyPersonList.Count];
                    var k = 0;
                    foreach (var item in OnlineNotifyPersonList)
                    {
                        var person = (from p in CurrentDatabase.People where p.PeopleId == item select new { p.PeopleId, p.Name }).FirstOrDefault();
                        var notifyPerson = new PeopleClass()
                        {
                            PeopleId = person.PeopleId,
                            Name = person.Name
                        };
                        givingPage.OnlineNotifyPerson[k] = notifyPerson;
                        k++;
                    }
                }

                if(response[i].ConfirmEmailPledge != null)
                {
                    var tempConfirmEmailPledge = (from c in CurrentDatabase.Contents where c.Id == response[i].ConfirmEmailPledge select new { c.Id, c.Title }).FirstOrDefault();
                    if (tempConfirmEmailPledge != null)
                    {
                        givingPage.ConfirmEmailPledge = new ConfirmEmailClass()
                        {
                            Id = tempConfirmEmailPledge.Id,
                            Title = tempConfirmEmailPledge.Title
                        };
                    }
                }
                if (response[i].ConfirmEmailOneTime != null)
                {
                    var tempConfirmEmailOneTime = (from c in CurrentDatabase.Contents where c.Id == response[i].ConfirmEmailOneTime select new { c.Id, c.Title }).FirstOrDefault();
                    if (tempConfirmEmailOneTime != null)
                    {
                        givingPage.ConfirmEmailOneTime = new ConfirmEmailClass()
                        {
                            Id = tempConfirmEmailOneTime.Id,
                            Title = tempConfirmEmailOneTime.Title
                        };
                    }
                }
                if (response[i].ConfirmEmailRecurring != null)
                {
                    var tempConfirmEmailRecurring = (from c in CurrentDatabase.Contents where c.Id == response[i].ConfirmEmailRecurring select new { c.Id, c.Title }).FirstOrDefault();
                    if (tempConfirmEmailRecurring != null)
                    {
                        givingPage.ConfirmEmailRecurring = new ConfirmEmailClass()
                        {
                            Id = tempConfirmEmailRecurring.Id,
                            Title = tempConfirmEmailRecurring.Title
                        };
                    }
                }

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
    public PageTypesClass[] PageType { get; set; }
    public string PageTypeString { get; set; }
    public FundsClass DefaultFund { get; set; }
    public string DefaultFundId { get; set; }
    public FundsClass[] AvailableFunds { get; set; }
    public string DisabledRedirect { get; set; }
    public EntryPointClass EntryPoint { get; set; }
    public string TopText { get; set; }
    public string ThankYouText { get; set; }
    public PeopleClass[] OnlineNotifyPerson { get; set; }
    public ConfirmEmailClass ConfirmEmailPledge { get; set; }
    public ConfirmEmailClass ConfirmEmailOneTime { get; set; }
    public ConfirmEmailClass ConfirmEmailRecurring { get; set; }
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
public class EntryPointClass
{
    public int Id { get; set; }
    public string Description { get; set; }
}
public class PeopleClass
{
    public int PeopleId { get; set; }
    public string Name { get; set; }
}
public class ConfirmEmailClass
{
    public int Id { get; set; }
    public string Title { get; set; }
}
