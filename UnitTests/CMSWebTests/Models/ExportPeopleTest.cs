using CmsData;
using CmsData.Codes;
using CmsWeb.Models;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UtilityExtensions;
using Xunit;
using System.Collections.Generic;

namespace CMSWebTests.Models
{
    [Collection(Collections.Database)]
    public class ExportPeopleTests
    {
        [Theory]        
        [InlineData(0, 0, false, true, true, null, null)]
        [InlineData(0, 0, false, true, false, null, null)]
        [InlineData(0, 0, false, false, true, null, null)]
        [InlineData(0, 0, false, false, false, null, null)]
        [InlineData(0, 0, false, null, true, null, null)]
        [InlineData(0, 0, false, null, false, null, null)]        
        public void DonorDetails_Should_Not_Bring_Reversed_or_Returned_contributions(
            int fundid, int campusid, bool pledges, bool? nontaxdeductible, bool includeUnclosed, int? tagid, string fundids)
        {
            using (var db = CMSDataContext.Create(Util.Host))
            {   
                var bundleList = CreateTestContributionSet(db, Util.Now.Date);

                var _exportPeople = new ExportPeople();
                DateTime exportStartDt = new DateTime(Util.Now.Date.Year, 1, 1);
                DateTime exportEndDt = new DateTime(Util.Now.Date.Year, 12, 31);
                DataTable tableResult = _exportPeople.DonorDetails(exportStartDt, exportEndDt, fundid, campusid, pledges, nontaxdeductible, includeUnclosed, tagid, fundids);

                var dbContributionsQry = db.Contributions
                    .Where(x => !ContributionTypeCode.ReturnedReversedTypes.Contains(x.ContributionTypeId) && !ContributionTypeCode.Pledge.Equals(x.ContributionTypeId))
                    .Where(x => ContributionStatusCode.Recorded.Equals(x.ContributionStatusId))
                    .Where(x => x.ContributionDate >= exportStartDt && x.ContributionDate < exportEndDt)
                    .Select(x => x);                

                dbContributionsQry = nontaxdeductible.HasValue
                    ? (nontaxdeductible is true)
                        ? dbContributionsQry = dbContributionsQry.Where(x => ContributionTypeCode.NonTaxDed.Equals(x.ContributionTypeId)).Select(x => x)
                        : dbContributionsQry = dbContributionsQry.Where(x => !ContributionTypeCode.NonTaxDed.Equals(x.ContributionTypeId)).Select(x => x)
                    : dbContributionsQry;
                
                if (includeUnclosed is false)
                {
                    dbContributionsQry = from c in dbContributionsQry
                                         join bd in db.BundleDetails on c.ContributionId equals bd.ContributionId
                                         join bh in db.BundleHeaders on bd.BundleHeaderId equals bh.BundleHeaderId
                                         where bh.BundleStatusId == 0
                                         select c;
                }

                var rc = tableResult.AsEnumerable().Where(row => ContributionTypeCode.ReturnedReversedTypes.Contains(row.Field<int>("ContributionTypeId")));
                var tableResultTotals = tableResult.AsEnumerable().Sum(row => row.Field<decimal>("Amount"));
                var totalContributions = dbContributionsQry.Sum(x => x.ContributionAmount) ?? 0;

                totalContributions.ToDouble().ShouldBe(tableResultTotals.ToDouble());
                rc.Count().ShouldBeLessThan(1);

                foreach(var b in bundleList)
                {
                    MockContributions.DeleteAllFromBundle(db, b);
                }
            }
        }

        [Theory]
        [InlineData( 0, false, true, true, null, null)]
        [InlineData( 0, false, true, false, null, null)]
        [InlineData( 0, false, false, true, null, null)]
        [InlineData( 0, false, false, false, null, null)]
        [InlineData( 0, false, null, true, null, null)]
        [InlineData( 0, false, null, false, null, null)]
        public void ExcelDonorTotals_Should_Not_Bring_Reversed_or_Returned_contributions(int campusid, bool pledges, bool? nontaxdeductible, bool includeUnclosed, int? tagid, string fundids)
        {
            using (var db = CMSDataContext.Create(Util.Host))
            {
                var bundleList = CreateTestContributionSet(db, Util.Now.Date);
                var _exportPeople = new ExportPeople();
                DateTime exportStartDt = new DateTime(Util.Now.Date.Year, 1, 1);
                DateTime exportEndDt = new DateTime(Util.Now.Date.Year, 12, 31);
                DataTable tableResult = _exportPeople.ExcelDonorTotals(exportStartDt,exportEndDt,campusid,pledges,nontaxdeductible,includeUnclosed, tagid, fundids);
                var dbContributionsQry = db.Contributions
                    .Where(x => !ContributionTypeCode.ReturnedReversedTypes.Contains(x.ContributionTypeId) && !ContributionTypeCode.Pledge.Equals(x.ContributionTypeId))
                    .Where(x => ContributionStatusCode.Recorded.Equals(x.ContributionStatusId))
                    .Where(x => x.ContributionDate >= exportStartDt && x.ContributionDate < exportEndDt)
                    .Select(x => x);

                dbContributionsQry = nontaxdeductible.HasValue
                    ? (nontaxdeductible is true)
                        ? dbContributionsQry = dbContributionsQry.Where(x => ContributionTypeCode.NonTaxDed.Equals(x.ContributionTypeId)).Select(x => x)
                        : dbContributionsQry = dbContributionsQry.Where(x => !ContributionTypeCode.NonTaxDed.Equals(x.ContributionTypeId)).Select(x => x)
                    : dbContributionsQry;

                if (includeUnclosed is false)
                {
                    dbContributionsQry = from c in dbContributionsQry
                                         join bd in db.BundleDetails on c.ContributionId equals bd.ContributionId
                                         join bh in db.BundleHeaders on bd.BundleHeaderId equals bh.BundleHeaderId
                                         where bh.BundleStatusId == 0
                                         select c;
                }

                var rc = tableResult.AsEnumerable().Where(row => ContributionTypeCode.ReturnedReversedTypes.Contains(row.Field<int>("ContributionTypeId")));
                var tableResultTotals = tableResult.AsEnumerable().Sum(row => row.Field<decimal>("Amount"));
                var totalContributions = dbContributionsQry.Sum(x => x.ContributionAmount) ?? 0;

                totalContributions.ToDouble().ShouldBe(tableResultTotals.ToDouble());
                rc.Count().ShouldBeLessThan(1);

                foreach (var b in bundleList)
                {
                    MockContributions.DeleteAllFromBundle(db, b);
                }
            }
        }

        [Theory]
        [InlineData(0, 0, false, true, true, null, null)]
        [InlineData(0, 0, false, true, false, null, null)]
        [InlineData(0, 0, false, false, true, null, null)]
        [InlineData(0, 0, false, false, false, null, null)]
        [InlineData(0, 0, false, null, true, null, null)]
        [InlineData(0, 0, false, null, false, null, null)]
        public void ExcelDonorFundTotals_Should_Not_Bring_Reversed_or_Returned_contributions(int fundid, int campusid, bool pledges, bool? nontaxdeductible, bool includeUnclosed, int? tagid, string fundids)
        {
            using (var db = CMSDataContext.Create(Util.Host))
            {
                var bundleList = CreateTestContributionSet(db, Util.Now.Date);
                var _exportPeople = new ExportPeople();
                DateTime exportStartDt = new DateTime(Util.Now.Date.Year, 1, 1);
                DateTime exportEndDt = new DateTime(Util.Now.Date.Year, 12, 31);
                DataTable tableResult = _exportPeople.ExcelDonorFundTotals(exportStartDt, exportEndDt, fundid, campusid, pledges, nontaxdeductible, includeUnclosed, tagid, fundids);
                var dbContributionsQry = db.Contributions
                    .Where(x => !ContributionTypeCode.ReturnedReversedTypes.Contains(x.ContributionTypeId) && !ContributionTypeCode.Pledge.Equals(x.ContributionTypeId))
                    .Where(x => ContributionStatusCode.Recorded.Equals(x.ContributionStatusId))
                    .Where(x => x.ContributionDate >= exportStartDt && x.ContributionDate < exportEndDt)
                    .Select(x => x);

                dbContributionsQry = nontaxdeductible.HasValue
                    ? (nontaxdeductible is true)
                        ? dbContributionsQry = dbContributionsQry.Where(x => ContributionTypeCode.NonTaxDed.Equals(x.ContributionTypeId)).Select(x => x)
                        : dbContributionsQry = dbContributionsQry.Where(x => !ContributionTypeCode.NonTaxDed.Equals(x.ContributionTypeId)).Select(x => x)
                    : dbContributionsQry;

                if (includeUnclosed is false)
                {
                    dbContributionsQry = from c in dbContributionsQry
                                         join bd in db.BundleDetails on c.ContributionId equals bd.ContributionId
                                         join bh in db.BundleHeaders on bd.BundleHeaderId equals bh.BundleHeaderId
                                         where bh.BundleStatusId == 0
                                         select c;
                }

                var rc = tableResult.AsEnumerable().Where(row => ContributionTypeCode.ReturnedReversedTypes.Contains(row.Field<int>("ContributionTypeId")));
                var tableResultTotals = tableResult.AsEnumerable().Sum(row => row.Field<decimal>("Amount"));
                var totalContributions = dbContributionsQry.Sum(x => x.ContributionAmount) ?? 0;

                totalContributions.ToDouble().ShouldBe(tableResultTotals.ToDouble());
                rc.Count().ShouldBeLessThan(1);

                foreach (var b in bundleList)
                {
                    MockContributions.DeleteAllFromBundle(db, b);
                }
            }
        }

        private List<BundleHeader> CreateTestContributionSet(CMSDataContext db, DateTime dt)
        {
            List<BundleHeader> bundleList = new List<BundleHeader>();
            var b1 = MockContributions.CreateSaveBundle(db);
            bundleList.Add(b1);
            var c1 = MockContributions.CreateSaveContribution(db, b1, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.CheckCash);
            var c2 = MockContributions.CreateSaveContribution(db, b1, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.GiftInKind);
            var c3 = MockContributions.CreateSaveContribution(db, b1, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.NonTaxDed);
            var c4 = MockContributions.CreateSaveContribution(db, b1, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.Online);
            var c5 = MockContributions.CreateSaveContribution(db, b1, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.Pledge);
            var c6 = MockContributions.CreateSaveContribution(db, b1, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.ReturnedCheck);
            var c7 = MockContributions.CreateSaveContribution(db, b1, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.Reversed);
            b1.BundleStatusId = BundleStatusCode.Closed;
            db.SubmitChanges();

            var b2 = MockContributions.CreateSaveBundle(db);
            bundleList.Add(b2);
            var b2c1 = MockContributions.CreateSaveContribution(db, b2, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.CheckCash);
            var b2c2 = MockContributions.CreateSaveContribution(db, b2, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.GiftInKind);
            var b2c3 = MockContributions.CreateSaveContribution(db, b2, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.NonTaxDed);
            var b2c4 = MockContributions.CreateSaveContribution(db, b2, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.Online);
            var b2c5 = MockContributions.CreateSaveContribution(db, b2, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.Pledge);
            var b2c6 = MockContributions.CreateSaveContribution(db, b2, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.ReturnedCheck);
            var b2c7 = MockContributions.CreateSaveContribution(db, b2, dt, 500, peopleId: 1, contributionType: ContributionTypeCode.Reversed);
            return bundleList;
        }
    }
}
