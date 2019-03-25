using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CmsData.API;
using CmsData.Codes;
using Dapper;
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
        public int FirstFundId()
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
        public int ResolveFundId(string fundName)
        {
            var fund = ResolveFund(fundName);
            return fund.IsNotNull() ? fund.FundId : FirstFundId();
        }
        private ContributionFund ResolveFund(string name)
        {
            // take a pushpay fund and find or create a touchpoint fund
            IQueryable<ContributionFund> funds = db.ContributionFunds.AsQueryable();

            var result = from f in funds
                         where f.FundName == name
                         where f.FundStatusId > 0
                         orderby f.FundStatusId
                         orderby f.FundId descending
                         select f;
            if (result.Any())
            {
                int id = result.Select(f => f.FundId).First();
                return db.ContributionFunds.SingleOrDefault(f => f.FundId == id);
            }
            else
            {
                return null;
                
            }
        }
        public void MoveFundIdToExistingFundId(int fromid, int toid, string name = null)
        {
            var oldfund = db.ContributionFunds.Single(ff => ff.FundId == fromid);
            var tofund = db.ContributionFunds.Single(ff => ff.FundId == toid);

            var sql = $"update dbo.contribution set fundid = {toid} where fundid = {fromid}";
            db.ExecuteCommand(sql);
        }
        public void MoveFundIdToNewFundId(int fromid, int toid, string name = null)
        {
            var oldfund = db.ContributionFunds.Single(ff => ff.FundId == fromid);
            var newfund = db.ContributionFunds.SingleOrDefault(ff => ff.FundId == toid);
            if(newfund != null)
                throw new Exception("Fund must not exist for MoveFundIdToNewFundId");

            db.FetchOrCreateFund(toid, name ?? oldfund.FundDescription);
            var sql = $"update dbo.contribution set fundid = {toid} where fundid = {fromid}";
            db.ExecuteCommand(sql);
        }
        /// <summary>
        /// ContributionTags table is used to cache sets of contributions so that dashboard reports can run very quickly.
        /// This function will create a tag of the contributions matching the json critera.
        /// See the SQL function Contributions2SearchIds for what values can be used in the JSON
        /// 
        /// This will first delete the ContributionTags of given name so that it is replaced with new ids
        /// you can add "AddToTag": 1 to the json if you want to preserve existing Tags of this name.
        /// </summary>
        public string CreateContributionTag(string name, DynamicData dd)
        {
            var p = new DynamicParameters();
            p.Add("@tagname", name);
            var json = JsonSerialize(dd);
            p.Add("@json", json);
            db.Connection.Execute("dbo.TagContributions", p, commandType: CommandType.StoredProcedure);
            return FormatJson(dd);
        }
        /// <summary>
        /// This works for the same purpose as CretaeContributionTag above
        /// but uses SQL to generate a list of ids instead of Contributions2SearchIds.
        /// They both use the json derived from DynamicData dd
        /// 
        /// This way a contributor developer can modify the search algorithm 
        /// without needing to modify the Contribution2SearchIds function 
        /// not having direct write access to the database.
        /// </summary>
        public string CreateContributionTagFromSql(string name, DynamicData dd, string sql)
        {
            var args = new DynamicParameters();
            var json = JsonSerialize(dd);
            args.Add("@json", json);

            var list = QueryContributionIds(sql, args);
            var csv = string.Join(",", list);
            args.Add("@ids", csv);
            args.Add("@tagname", name);

            db.Connection.Execute("dbo.TagContributionsFromIds", args, commandType: CommandType.StoredProcedure);
            return $@"{name} = {FormatJson(dd)}";
        }
        /// <summary>
        /// It is good practice to give a set of tags for a particular report a common prefix like a namespace.
        /// This way you can remove old tags in the Python script before createing new ones.
        /// </summary>
        public void DeleteContributionTags(string namelike)
        {
            db.Connection.Execute("DELETE dbo.ContributionTag WHERE TagName LIKE @namelike", new {namelike});
        }
        public IEnumerable<int> QueryContributionIds(string sql, object declarations)
        {
            var cn = db.ReadonlyConnection();
            var parameters = new DynamicParameters();
            if (declarations != null)
                parameters.AddDynamicParams(declarations);
            return cn.Query<int>(sql, parameters, commandTimeout: 600);
        }

    }
}
