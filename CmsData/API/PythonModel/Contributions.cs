using System;
using System.Linq;
using System.Text;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public BundleHeader GetBundleHeader(DateTime date, DateTime now, int? btid = null)
        {
            return Contribution.GetBundleHeader(db, date, now, btid);
        }

        public void FinishBundle(BundleHeader bh)
        {
            Contribution.FinishBundle(db, bh);
        }
        public BundleDetail AddContributionDetail(DateTime date, int fundid,
            string amount, string checkno, string routing, string account)
        {
            return Contribution.AddContributionDetail(db, date, fundid, amount, checkno, routing, account);
        }
        public static BundleDetail NewBundleDetail(CMSDataContext db, DateTime date, int fundid, string amount)
        {
            var bd = new BundleDetail
            {
                CreatedBy = Util.UserId,
                CreatedDate = DateTime.Now
            };
            bd.Contribution = new Contribution
            {
                CreatedBy = Util.UserId,
                CreatedDate = DateTime.Now,
                ContributionDate = date,
                FundId = fundid,
                ContributionStatusId = 0,
                ContributionTypeId = ContributionTypeCode.CheckCash,
                ContributionAmount = amount.GetAmount()
            };
            return bd;
        }
        public static int FirstFundId(CMSDataContext db)
        {
            var firstfund = (from f in db.ContributionFunds
                             where f.FundStatusId == 1
                             orderby f.FundId
                             select f.FundId).First();
            return firstfund;
        }
        public void DeleteContribution(int cid)
        {
            var bd = db.BundleDetails.SingleOrDefault(d => d.ContributionId == cid);
            if (bd != null)
            {
                var c = bd.Contribution;
                db.BundleDetails.DeleteOnSubmit(bd);
                db.Contributions.DeleteOnSubmit(c);
                db.SubmitChanges();
            }
        }
        public ContributionFund FetchOrCreateFund(string description)
        {
            return db.FetchOrCreateFund(description);
        }

        public void MoveFundIdToExistingFundId(int fromid, int toid, string name = null)
        {
            var oldfund = DbUtil.Db.ContributionFunds.Single(ff => ff.FundId == fromid);
            var tofund = DbUtil.Db.ContributionFunds.Single(ff => ff.FundId == toid);

            var sql = $"update dbo.contribution set fundid = {toid} where fundid = {fromid}";
            DbUtil.Db.ExecuteCommand(sql);
        }
        public void MoveFundIdToNewFundId(int fromid, int toid, string name = null)
        {
            var oldfund = DbUtil.Db.ContributionFunds.Single(ff => ff.FundId == fromid);
            var newfund = DbUtil.Db.ContributionFunds.SingleOrDefault(ff => ff.FundId == toid);
            if(newfund != null)
                throw new Exception("Fund must not exist for MoveFundIdToNewFundId");

            DbUtil.Db.FetchOrCreateFund(toid, name ?? oldfund.FundDescription);
            var sql = $"update dbo.contribution set fundid = {toid} where fundid = {fromid}";
            DbUtil.Db.ExecuteCommand(sql);
        }
    }
}
