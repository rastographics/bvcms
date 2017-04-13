using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsData.Codes;
using OfficeOpenXml;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class UploadExcelIpsModel : UploadPeopleModel
    {
        private Dictionary<int, int> peopleids;
        public UploadExcelIpsModel(string host, int peopleId, bool noupdate, bool testing = false)
            : base(host, peopleId, noupdate, testing)
        {
            PeopleSheetName = "Personal Data";
        }
        public override bool DoUpload(ExcelPackage pkg)
        {
            var rt = Db2.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();

            peopleids = Db2.PeopleExtras.Where(vv => vv.Field == "IndividualId" && vv.IntValue != null)
                .ToDictionary(vv => vv.IntValue ?? 0, vv => vv.PeopleId);

            UploadPeople(rt, pkg);

            TryDeleteAllContributions();
            UploadPledges(rt, pkg);
            UploadGifts(rt, pkg);

            rt.Completed = DateTime.Now;
            Db2.SubmitChanges();
            return true;
        }
        internal override void StoreIds(Person p, dynamic a)
        {
            p.AddEditExtraInt("HouseholdId", (int)a.FamilyId);
            p.AddEditExtraInt("IndividualId", (int)a.IndividualId);
            if(!Testing)
                peopleids.Add((int)a.IndividualId, p.PeopleId);
        }

        private void UploadPledges(UploadPeopleRun rt, ExcelPackage pkg)
        {
            var db = DbUtil.Create(Host);
            var data = FetchPledgeData(pkg.Workbook.Worksheets["Pledges"]).ToList();
            rt.Count = data.Count;
            rt.Description = $"Uploading Pledges {(Testing ? "in testing mode" : "for real")}";
            rt.Processed = 0;
            Db2.SubmitChanges();

            var weeks = (from g in data
                         group g by g.Date.Sunday() into weeklypledges
                         select weeklypledges).ToList();
            BundleHeader bh = null;
            foreach (var week in weeks)
            {
                FinishBundle(db, bh);
                if (!Testing)
                {
                    db.Dispose();
                    db = DbUtil.Create(Host);
                }
                bh = new BundleHeader
                {
                    BundleHeaderTypeId = BundleTypeCode.Pledge,
                    BundleStatusId = BundleStatusCode.Closed,
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Today,
                    ContributionDate = week.Key,
                };
                foreach (var pledge in week)
                {
                    var pid = GetPeopleId(pledge);
                    if (!Testing)
                    {
                        if (!pid.HasValue)
                            throw new Exception($"peopleid not found from individualid {pledge.IndividualId}");
                        var f = db.FetchOrCreateFund(pledge.FundId, pledge.FundName ?? pledge.FundDescription);
                        f.FundPledgeFlag = true;
                    }
                    var bd = new BundleDetail();
                    bd.CreatedBy = Util.UserId;
                    bd.CreatedDate = DateTime.Now;
                    bd.Contribution = new Contribution
                    {
                        CreatedBy = Util.UserId,
                        CreatedDate = DateTime.Now,
                        ContributionDate = pledge.Date,
                        FundId = pledge.FundId,
                        ContributionStatusId = 0,
                        ContributionTypeId = ContributionTypeCode.Pledge,
                        ContributionAmount = pledge.Amount,
                        PeopleId = pid
                    };
                    bh.BundleDetails.Add(bd);
                    rt.Processed++;
                    Db2.SubmitChanges();
                }
            }
            FinishBundle(db, bh);
            if (!Testing)
                db.Dispose();
        }
        private void UploadGifts(UploadPeopleRun rt, ExcelPackage pkg)
        {
            var db = DbUtil.Create(Host);
            var data = FetchContributionData(pkg.Workbook.Worksheets["Gift Data"]).ToList();
            rt.Count = data.Count;
            rt.Description = $"Uploading Gifts {(Testing ? "in testing mode" : "for real")}";
            rt.Processed = 0;
            Db2.SubmitChanges();

            var weeks = (from g in data
                         group g by g.Date.Sunday() into weeklygifts
                         select weeklygifts).ToList();
            BundleHeader bh = null;
            foreach (var week in weeks)
            {
                FinishBundle(db, bh);
                if (!Testing)
                {
                    db.Dispose();
                    db = DbUtil.Create(Host);
                }
                bh = new BundleHeader
                {
                    BundleHeaderTypeId = BundleTypeCode.ChecksAndCash,
                    BundleStatusId = BundleStatusCode.Closed,
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Today,
                    ContributionDate = week.Key,
                };
                foreach (var gift in week)
                {
                    var pid = GetPeopleId(gift);
                    if (!Testing)
                        if (!pid.HasValue)
                            throw new Exception($"peopleid not found from individualid {gift.IndividualId}");
                    if (!Testing)
                        db.FetchOrCreateFund(gift.FundId, gift.FundName ?? gift.FundDescription);
                    var bd = new BundleDetail();
                    bd.CreatedBy = Util.UserId;
                    bd.CreatedDate = DateTime.Now;
                    bd.Contribution = new Contribution
                    {
                        CreatedBy = Util.UserId,
                        CreatedDate = DateTime.Now,
                        ContributionDate = gift.Date,
                        FundId = gift.FundId,
                        ContributionStatusId = 0,
                        ContributionTypeId = ContributionTypeCode.CheckCash,
                        ContributionAmount = gift.Amount,
                        CheckNo = gift.CheckNo,
                        PeopleId = pid
                    };
                    bh.BundleDetails.Add(bd);
                    rt.Processed++;
                    Db2.SubmitChanges();
                }
            }
            FinishBundle(db, bh);
            if (!Testing)
                db.Dispose();
        }
        private void FinishBundle(CMSDataContext db, BundleHeader bh)
        {
            if (!Testing)
            {
                if (bh != null)
                {
                    bh.TotalChecks = bh.BundleDetails.Sum(d => d.Contribution.ContributionAmount);
                    bh.TotalCash = 0;
                    bh.TotalEnvelopes = 0;
                    db.BundleHeaders.InsertOnSubmit(bh);
                    db.SubmitChanges();
                }
            }
        }
        public IEnumerable<PledgeGift> FetchContributionData(ExcelWorksheet ws)
        {
            FetchHeaderColumns(ws);
            var r = 2;
            while (r <= ws.Dimension.End.Row)
            {
                var row = new PledgeGift()
                {
                    IndividualId = GetInt(ws.Cells[r, Names["IndividualId"]].Value),
                    Amount = GetDecimal(ws.Cells[r, Names["Amount"]].Value),
                    Date = GetDate(ws.Cells[r, Names["Date"]].Value) ?? DateTime.MinValue,
                    FundId = GetInt(ws.Cells[r, Names["FundId"]].Value),
                    FundDescription = GetString(ws.Cells[r, Names["FundDescription"]].Value),
                    FundName = GetString(ws.Cells[r, Names["FundName"]].Value),
                };
                if (Names.ContainsKey("CheckNo"))
                    row.CheckNo = GetString(ws.Cells[r, Names["CheckNo"]].Value);
                r++;
                yield return row;
            }
        }
        public IEnumerable<PledgeGift> FetchPledgeData(ExcelWorksheet ws)
        {
            FetchHeaderColumns(ws);
            var r = 2;
            while (r <= ws.Dimension.End.Row)
            {
                var row = new PledgeGift
                {
                    IndividualId = GetInt(ws.Cells[r, Names["IndividualId"]].Value),
                    Amount = GetDecimal(ws.Cells[r, Names["PledgeAmount"]].Value),
                    Date = GetDate(ws.Cells[r, Names["PledgeDate"]].Value) ?? DateTime.MinValue,
                    FundId = GetInt(ws.Cells[r, Names["FundId"]].Value),
                    FundName = GetString(ws.Cells[r, Names["FundName"]].Value),
                    FundDescription = GetString(ws.Cells[r, Names["FundDescription"]].Value),
                };
                r++;
                yield return row;
            }
        }
        private void TryDeleteAllContributions()
        {
            if (Testing)
                return;
            var db = DbUtil.Create(Host);
            if (!db.Setting("UploadExcelIpsDeleteGifts"))
                return;

            var deletesql = @"
DELETE dbo.BundleDetail
FROM dbo.BundleDetail d
JOIN dbo.Contribution c ON d.ContributionId = c.ContributionId
DELETE dbo.Contribution
DELETE dbo.BundleHeader
DBCC CHECKIDENT ('[Contribution]', RESEED, 0)
DBCC CHECKIDENT ('[BundleHeader]', RESEED, 0)
DBCC CHECKIDENT ('[BundleDetail]', RESEED, 0)
";
            db.ExecuteCommand(deletesql);
        }
        internal override int? GetPeopleId(dynamic a)
        {
            if (a.IndividualId == null)
                return null;
            var id = (int)a.IndividualId;
            if (peopleids.ContainsKey(id))
                return peopleids[id];
            return null;
        }
    }
    public class PledgeGift
    {
        public int IndividualId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string FundName { get; set; }
        public int FundId { get; set; }
        public string FundDescription { get; set; }
        public string CheckNo { get; set; }
    }
}