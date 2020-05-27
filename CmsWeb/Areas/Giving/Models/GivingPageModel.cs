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
                                GivingPageId = gp.GivingPageId,
                                PageName = gp.PageName,
                                PageUrl = gp.PageUrl,
                                EditUrl = "/Giving/" + gp.GivingPageId,
                                Enabled = gp.Enabled,
                                SkinFile = new ContentFile
                                {
                                    Id = gp.SkinFile.Id,
                                    Title = gp.SkinFile.Title
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
                                EntryPointId = gp.EntryPointId,
                                TopText = gp.TopText,
                                ThankYouText = gp.ThankYouText,
                                OnlineNotifyPerson = (from np in db.People
                                                      where onp.Contains(np.PeopleId)
                                                      select new NotifyPerson
                                                      {
                                                          PeopleId = np.PeopleId,
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
                                    Title = gp.ConfirmationEmailPledge.Title,
                                },
                                ConfirmEmailOneTime = new ContentFile
                                {
                                    Id = gp.ConfirmationEmailOneTime.Id,
                                    Title = gp.ConfirmationEmailOneTime.Title,
                                },
                                ConfirmEmailRecurring = new ContentFile
                                {
                                    Id = gp.ConfirmationEmailRecurring.Id,
                                    Title = gp.ConfirmationEmailRecurring.Title,
                                }
                            });
            if (id != 0)
            {
                pages = pages.Where(p => p.GivingPageId == id);
            }
            return pages.ToList();
        }

        public NotifyPerson[] GetSelectedOnlineNotifyPerson(string onlineNotifyPerson)
        {
            if (onlineNotifyPerson != null)
            {
                var OnlineNotifyPersonList = onlineNotifyPerson.Split(',').Select(int.Parse).ToList();
                var peopleClass = new NotifyPerson[OnlineNotifyPersonList.Count];
                var k = 0;
                foreach (var item in OnlineNotifyPersonList)
                {
                    var person = (from p in CurrentDatabase.People where p.PeopleId == item select new { p.PeopleId, p.Name }).FirstOrDefault();
                    var notifyPerson = new NotifyPerson()
                    {
                        PeopleId = person.PeopleId,
                        Name = person.Name
                    };
                    peopleClass[k] = notifyPerson;
                    k++;
                }
                return peopleClass;
            }
            else
            {
                return null;
            }
        }

        public ContentFile GetSelectedSkinFile(int? id)
        {
            var tempSkinFile = (from c in CurrentDatabase.Contents
                                where c.TypeID == ContentTypeCode.TypeHtml
                                where c.ContentKeyWords.Any(vv => vv.Word == "Shell")
                                where c.Id == id
                                select new { c.Id, c.Title }).FirstOrDefault();
            var shellClass = new ContentFile();
            if (tempSkinFile != null)
            {
                shellClass = new ContentFile()
                {
                    Id = tempSkinFile.Id,
                    Title = tempSkinFile.Title
                };
            }
            else
            {
                shellClass = null;
            }
            return shellClass;
        }

        public List<GivingPageItem> AddNewGivingPage(GivingPageViewModel viewModel)
        {
            var newGivingPage = new GivingPage()
            {
                PageName = viewModel.pageName,
                PageUrl = viewModel.pageUrl,
                Enabled = viewModel.enabled,
                DisabledRedirect = viewModel.disabledRedirect
            };
            newGivingPage.PageType = viewModel.pageType;
            newGivingPage.FundId = viewModel.defaultFund?.Id;
            newGivingPage.EntryPointId = viewModel.entryPointId;
            newGivingPage.SkinFileId = viewModel.skinFile?.Id;
            CurrentDatabase.GivingPages.InsertOnSubmit(newGivingPage);
            CurrentDatabase.SubmitChanges();

            if (viewModel.availableFunds != null)
            {
                foreach (var item in viewModel.availableFunds)
                {
                    var newGivingPageFund = new GivingPageFund()
                    {
                        GivingPageId = newGivingPage.GivingPageId,
                        FundId = item.Id
                    };
                    CurrentDatabase.GivingPageFunds.InsertOnSubmit(newGivingPageFund);
                }
                CurrentDatabase.SubmitChanges();
            }

            var newGivingPageList = new List<GivingPageItem>();
            var givingPageItem = new GivingPageItem()
            {
                GivingPageId = newGivingPage.GivingPageId,
                PageUrl = newGivingPage.PageUrl,
                PageName = newGivingPage.PageName,
                Enabled = newGivingPage.Enabled,
                SkinFile = viewModel.skinFile,
                DefaultFund = viewModel.defaultFund,
                PageType = viewModel.pageType,
                EntryPointId = viewModel.entryPointId
            };

            newGivingPageList.Add(givingPageItem);

            return newGivingPageList;
        }
        
        public List<GivingPageItem> UpdateGivingPage(GivingPageViewModel viewModel, GivingPage givingPage)
        {
            givingPage.PageName = viewModel.pageName;
            givingPage.PageUrl = viewModel.pageUrl;
            givingPage.PageType = viewModel.pageType;
            givingPage.Enabled = viewModel.enabled;
            givingPage.FundId = viewModel.defaultFund?.Id;
            givingPage.DisabledRedirect = viewModel.disabledRedirect;
            givingPage.SkinFileId = viewModel.skinFile?.Id;
            givingPage.TopText = viewModel.topText;
            givingPage.ThankYouText = viewModel.thankYouText;
            givingPage.ConfirmationEmailPledgeId = viewModel.confirmEmailPledge?.Id;
            givingPage.ConfirmationEmailOneTimeId = viewModel.confirmEmailOneTime?.Id;
            givingPage.ConfirmationEmailRecurringId = viewModel.confirmEmailRecurring?.Id;
            givingPage.CampusId = viewModel.campusId;
            givingPage.EntryPointId = viewModel.entryPointId;

            // update other funds
            CurrentDatabase.GivingPageFunds.DeleteAllOnSubmit(givingPage.GivingPageFunds);
            CurrentDatabase.SubmitChanges();
            if (viewModel.availableFunds != null)
            {
                foreach (var item in viewModel.availableFunds)
                {
                    var newGivingPageFund = new GivingPageFund()
                    {
                        GivingPageId = viewModel.pageId,
                        FundId = item.Id
                    };
                    CurrentDatabase.GivingPageFunds.InsertOnSubmit(newGivingPageFund);
                }
            }
            
            var onlineNotifyPersonString = "";
            if (viewModel.onlineNotifyPerson != null)
            {
                foreach (var item in viewModel.onlineNotifyPerson)
                {
                    onlineNotifyPersonString += item.PeopleId + ",";
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
                GivingPageId = viewModel.pageId,
                PageName = viewModel.pageName,
                PageUrl = viewModel.pageUrl,
                Enabled = viewModel.enabled,
                SkinFile = viewModel.skinFile,
                PageType = viewModel.pageType,
                DefaultFund = viewModel.defaultFund,
                DisabledRedirect = viewModel.disabledRedirect,
                EntryPointId = viewModel.entryPointId,
                TopText = viewModel.topText,
                ThankYouText = viewModel.thankYouText,
                OnlineNotifyPerson = viewModel.onlineNotifyPerson,
                ConfirmEmailPledge = viewModel.confirmEmailPledge,
                ConfirmEmailOneTime = viewModel.confirmEmailOneTime,
                ConfirmEmailRecurring = viewModel.confirmEmailRecurring
            };
            returningGivingPageList.Add(givingPageItem);
            return returningGivingPageList;
        }
    }

    public class GivingPageItem
    {
        public int GivingPageId { get; set; }
        public string PageName { get; set; }
        public string PageUrl { get; set; }
        public string EditUrl { get; set; }
        public bool Enabled { get; set; }
        public ContentFile SkinFile { get; set; }
        public int PageType { get; set; }
        public string DisabledRedirect { get; set; }
        public string TopText { get; set; }
        public string ThankYouText { get; set; }
        public NotifyPerson[] OnlineNotifyPerson { get; set; }
        public ContentFile ConfirmEmailPledge { get; set; }
        public ContentFile ConfirmEmailOneTime { get; set; }
        public ContentFile ConfirmEmailRecurring { get; set; }
        public int? CurrentIndex { get; set; }
        public int? EntryPointId { get; set; }
        public FundsClass DefaultFund { get; set; }
        public FundsClass[] AvailableFunds { get; set; }
    }

    public class FundsClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class NotifyPerson
    {
        public int PeopleId { get; set; }
        public string Name { get; set; }
    }

    public class ContentFile
    {
        public int? Id { get; set; }
        public string Title { get; set; }
    }
}
