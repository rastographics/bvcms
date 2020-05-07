using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Giving.Models
{
    public class GivingPageViewModel
    {
        public int pageId { get; set; }
        public string pageName { get; set; }
        public string pageTitle { get; set; }
        public PageTypesClass[] pageType { get; set; }
        public bool enabled { get; set; }
        public FundsClass defaultFund { get; set; }
        public FundsClass[] availFundsArray { get; set; }
        public string disRedirect { get; set; }
        public ShellClass skinFile { get; set; }
        public string topText { get; set; }
        public string thankYouText { get; set; }
        public PeopleClass[] onlineNotifyPerson { get; set; }
        public ConfirmEmailClass confirmEmailPledge { get; set; }
        public ConfirmEmailClass confirmEmailOneTime { get; set; }
        public ConfirmEmailClass confirmEmailRecurring { get; set; }
        public int? campusId { get; set; }
        public EntryPointClass entryPoint { get; set; }
    }
}
