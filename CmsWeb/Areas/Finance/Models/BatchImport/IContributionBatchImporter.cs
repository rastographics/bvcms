using CmsWeb.Lifecycle;
using System;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal interface IContributionBatchImporter
    {
        int? RunImport(string text, DateTime date, int? fundid, bool fromFile);
    }

    public abstract class ContributionBatchImporterBase : IContributionBatchImporter
    {
        protected IRequestManager RequestManager { get; }

        public ContributionBatchImporterBase(IRequestManager requestManager)
        {
            RequestManager = requestManager;
        }

        public abstract int? RunImport(string text, DateTime date, int? fundid, bool fromFile);
    }
}
