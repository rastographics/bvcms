using CmsData;
using CmsData.Codes;
using CmsWeb.Models;
using System.Collections.Generic;
using System.Linq;
using CmsData.Classes.Giving;
using CmsWeb.Constants;
using System;
using System.Linq.Dynamic;

namespace CmsWeb.Areas.Giving.Models
{
    public class GivingPageModel : IDbBinder
    {
        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public GivingPageModel() { }
        public GivingPageModel(CMSDataContext db)
        {
            CurrentDatabase = db;
        }

        public CMSDataContext CurrentDatabase { get; set; }

        public List<GivingPageItem> GetGivingPages(int id = 0)
        {
            var db = CurrentDatabase;
            var outputList = new List<GivingPageItem>();
            var pages = (from gp in db.GivingPages
                            let onp = db.SplitInts(gp.OnlineNotifyPerson).Select(i => i.ValueX)
                            select new GivingPageItem
                            {
                                PageId = gp.GivingPageId,
                                PageName = gp.PageName,
                                PageUrl = gp.PageUrl,
                                EditUrl = "/Giving/Manage/" + gp.GivingPageId,
                                Enabled = gp.Enabled,
                                DefaultPage = gp.DefaultPage,
                                MainCampusPageFlag = gp.MainCampusPageFlag,
                                SkinFile = new ContentFile
                                {
                                    Id = gp.SkinFile.Id,
                                    Name = gp.SkinFile.Title
                                },
                                PageType = gp.PageType,
                                DefaultFund = (from fund in db.ContributionFunds
                                               where fund.FundId == gp.FundId
                                               select new FundsClass
                                               {
                                                   Id = fund.FundId,
                                                   Name = fund.FundName
                                               }).SingleOrDefault(),
                                DisabledRedirect = gp.DisabledRedirect,
                                EntryPoint = (from point in db.EntryPoints
                                              where point.Id == gp.EntryPointId
                                              select new EntryPoint
                                              {
                                                  Id = point.Id,
                                                  Name = point.Description
                                              }).SingleOrDefault(),
                                TopText = gp.TopText,
                                ThankYouText = gp.ThankYouText,
                                OnlineNotifyPerson = (from np in db.People
                                                      where onp.Contains(np.PeopleId)
                                                      select new NotifyPerson
                                                      {
                                                          Id = np.PeopleId,
                                                          Name = np.Name
                                                      }).ToArray(),
                                AvailableFunds = (from fund in db.GivingPageFunds
                                                  where fund.GivingPageId == gp.GivingPageId
                                                  join f in db.ContributionFunds on fund.FundId equals f.FundId
                                                  select new FundsClass {
                                                      Id = fund.FundId,
                                                      Name = f.FundName
                                                    }).ToArray(),
                                ConfirmEmailPledge = new ContentFile
                                {
                                    Id = gp.ConfirmationEmailPledge.Id,
                                    Name = gp.ConfirmationEmailPledge.Title,
                                },
                                ConfirmEmailOneTime = new ContentFile
                                {
                                    Id = gp.ConfirmationEmailOneTime.Id,
                                    Name = gp.ConfirmationEmailOneTime.Title,
                                },
                                ConfirmEmailRecurring = new ContentFile
                                {
                                    Id = gp.ConfirmationEmailRecurring.Id,
                                    Name = gp.ConfirmationEmailRecurring.Title,
                                }
                            });
            if (id != 0)
            {
                pages = pages.Where(p => p.PageId == id);
            }
            return pages.ToList();
        }
        
        public GivingPageItem Create(GivingPageViewModel viewModel)
        {
            var givingPageList = (from gpList in CurrentDatabase.GivingPages select gpList).ToList();
            var defaultPage = false;
            if (givingPageList.Count == 0)
                defaultPage = true;
            if (viewModel.MainCampusPageFlag == null)
                viewModel.MainCampusPageFlag = false;
            var newGivingPage = new GivingPage()
            {
                PageName = viewModel.PageName,
                PageUrl = viewModel.PageUrl,
                DefaultPage = defaultPage
            };
            CurrentDatabase.GivingPages.InsertOnSubmit(newGivingPage);
            CurrentDatabase.SubmitChanges();
            return Fill(viewModel, newGivingPage);
        }
        
        public GivingPageItem Fill(GivingPageViewModel viewModel, GivingPage givingPage)
        {
            givingPage.PageName = viewModel.PageName;
            givingPage.PageType = viewModel.PageType;
            givingPage.Enabled = viewModel.Enabled;
            givingPage.FundId = viewModel.DefaultFund?.Id;
            givingPage.DisabledRedirect = viewModel.DisabledRedirect;
            givingPage.SkinFileId = viewModel.SkinFile?.Id;
            givingPage.TopText = viewModel.TopText;
            givingPage.ThankYouText = viewModel.ThankYouText;
            givingPage.ConfirmationEmailPledgeId = viewModel.ConfirmEmailPledge?.Id;
            givingPage.ConfirmationEmailOneTimeId = viewModel.ConfirmEmailOneTime?.Id;
            givingPage.ConfirmationEmailRecurringId = viewModel.ConfirmEmailRecurring?.Id;
            givingPage.CampusId = viewModel.CampusId;
            givingPage.MainCampusPageFlag = viewModel.MainCampusPageFlag;
            givingPage.EntryPointId = viewModel.EntryPoint?.Id;

            // update other funds
            CurrentDatabase.GivingPageFunds.DeleteAllOnSubmit(givingPage.GivingPageFunds);
            CurrentDatabase.SubmitChanges();
            if (viewModel.AvailableFunds != null)
            {
                foreach (var fund in viewModel.AvailableFunds)
                {
                    var newGivingPageFund = new GivingPageFund()
                    {
                        GivingPageId = givingPage.GivingPageId,
                        FundId = fund.Id
                    };
                    CurrentDatabase.GivingPageFunds.InsertOnSubmit(newGivingPageFund);
                }
            }
            
            var onlineNotifyPersonString = "";
            if (viewModel.OnlineNotifyPerson != null)
            {
                foreach (var item in viewModel.OnlineNotifyPerson)
                {
                    onlineNotifyPersonString += item.Id + ",";
                }
                givingPage.OnlineNotifyPerson = onlineNotifyPersonString.Remove(onlineNotifyPersonString.Length - 1, 1);
            }
            else
            {
                givingPage.OnlineNotifyPerson = null;
            }
            CurrentDatabase.SubmitChanges();

            var returningGivingPageList = new List<GivingPageItem>();
            var givingPageItem = new GivingPageItem()
            {
                PageId = givingPage.GivingPageId,
                PageName = viewModel.PageName,
                PageUrl = givingPage.PageUrl,
                Enabled = viewModel.Enabled,
                SkinFile = viewModel.SkinFile,
                PageType = viewModel.PageType,
                DefaultFund = viewModel.DefaultFund,
                DisabledRedirect = viewModel.DisabledRedirect,
                EntryPoint = viewModel.EntryPoint,
                TopText = viewModel.TopText,
                ThankYouText = viewModel.ThankYouText,
                OnlineNotifyPerson = viewModel.OnlineNotifyPerson,
                ConfirmEmailPledge = viewModel.ConfirmEmailPledge,
                ConfirmEmailOneTime = viewModel.ConfirmEmailOneTime,
                ConfirmEmailRecurring = viewModel.ConfirmEmailRecurring
            };
            return givingPageItem;
        }

        public GivingPageItem UpdateGivingDefaultPage(bool value, int PageId)
        {
            var givingPageItem = new GivingPageItem();
            var givingPage = (from gp in CurrentDatabase.GivingPages where gp.GivingPageId == PageId select gp).FirstOrDefault();
            givingPageItem.OldDefaultPageId = 0;
            if (value == true)
            {
                var oldDefaultPage = (from g in CurrentDatabase.GivingPages where g.DefaultPage == true select g).FirstOrDefault();
                if (oldDefaultPage != null)
                {
                    oldDefaultPage.DefaultPage = false;
                    givingPageItem.OldDefaultPageId = oldDefaultPage.GivingPageId;
                }
            }
            givingPage.DefaultPage = value;
            CurrentDatabase.SubmitChanges();
            givingPageItem.PageId = givingPage.GivingPageId;
            givingPageItem.PageName = givingPage.PageName;
            givingPageItem.DefaultPage = givingPage.DefaultPage;
            return givingPageItem;
        }

        public List<ContributionFund> GetFundsByGivingPageUrl(string givingPageTitle)
        {
            var givingPage = (from g in CurrentDatabase.GivingPages where g.PageUrl == givingPageTitle select g).FirstOrDefault();
            if (givingPage == null)
                return null;
            else
            {
                var fundsList = new List<ContributionFund>();
                foreach (var item in givingPage.GivingPageFunds)
                {
                    var fund = new ContributionFund()
                    {
                        FundId = item.ContributionFund.FundId,
                        FundName = item.ContributionFund.FundName
                    };
                    fundsList.Add(fund);
                }
                fundsList.OrderBy(f => f.FundName);
                return fundsList;
            }
        }
    }

    public class GivingPageItem
    {
        public int PageId { get; set; }
        public string PageName { get; set; }
        public string PageUrl { get; set; }
        public string EditUrl { get; set; }
        public bool Enabled { get; set; }
        public bool? DefaultPage { get; set; }
        public ContentFile SkinFile { get; set; }
        public int PageType { get; set; }
        public string DisabledRedirect { get; set; }
        public string TopText { get; set; }
        public string ThankYouText { get; set; }
        public int? CampusId { get; set; }
        public bool? MainCampusPageFlag { get; set; }
        public int? OldDefaultPageId { get; set; }
        public NotifyPerson[] OnlineNotifyPerson { get; set; }
        public ContentFile ConfirmEmailPledge { get; set; }
        public ContentFile ConfirmEmailOneTime { get; set; }
        public ContentFile ConfirmEmailRecurring { get; set; }
        public EntryPoint EntryPoint { get; set; }
        public FundsClass DefaultFund { get; set; }
        public FundsClass[] AvailableFunds { get; set; }
    }

    public class FundsClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EntryPoint
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class NotifyPerson
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ContentFile
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}
