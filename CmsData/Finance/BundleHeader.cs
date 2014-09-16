using System;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Net.Mail;
using UtilityExtensions;

namespace CmsData
{
    public partial class BundleHeader
    {
        partial void OnContributionDateChanged()
        {
            ContributionDateChanged = true;
        }
        public bool ContributionDateChanged { get; private set; }

        partial void OnBundleStatusIdChanged()
        {
            BundleStatusIdChanged = true;
        }
        public bool BundleStatusIdChanged { get; private set; }

        partial void OnFundIdChanged()
        {
            FundIdChanged = true;
        }
        public bool FundIdChanged { get; private set; }

        public static int FetchOrCreateBundleType(CMSDataContext Db, string type)
        {
            var bt = Db.BundleHeaderTypes.SingleOrDefault(m => m.Description == type);
            if (bt == null)
            {
                var max = Db.BundleHeaderTypes.Max(mm => mm.Id) + 1;
                bt = new BundleHeaderType { Id = max, Code = "M" + max, Description = type };
                Db.BundleHeaderTypes.InsertOnSubmit(bt);
                Db.SubmitChanges();
            }
            return bt.Id;
        }
    }
}