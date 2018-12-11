using CmsData;
using CmsData.Codes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class UploadExcelIpsModel : UploadPeopleModel
    {
        internal bool AlphaNumericIds;
        internal bool IgnoreMissingGifts;
        private readonly StringBuilder _orphanedGifts = new StringBuilder();
        private readonly StringBuilder _orphanedPledges = new StringBuilder();
        private Dictionary<int, int> _numericPeopleIds;
        private Dictionary<string, int> _alphanumericPeopleIds;

        public UploadExcelIpsModel()
        {
        }

        public UploadExcelIpsModel(CMSDataContext db, string host, int peopleId, bool noupdate, bool ignoremissinggifts, bool testing = false)
            : base(db, host, peopleId, noupdate, testing)
        {
            PeopleSheetName = "Personal Data";
            IgnoreMissingGifts = ignoremissinggifts;
        }

        public override bool DoUpload(ExcelPackage pkg)
        {
            var rt = ProgressDbContext.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();

            var ws = pkg.Workbook.Worksheets[PeopleSheetName];
            FetchData(pkg.Workbook.Worksheets[PeopleSheetName]);
            const string sheet = "Personal Data";
            CheckColumn("IndividualId", sheet);
            CheckColumn("FamilyId", sheet);
            CheckColumn("First", sheet);
            CheckColumn("Last", sheet);

            var sid = ((object)Datalist[0].IndividualId).ToString();

            if (sid.ToCharArray().Any(char.IsLetter))
            {
                AlphaNumericIds = true;
            }

            if (AlphaNumericIds)
            {
                _alphanumericPeopleIds = JobDbContext.PeopleExtras
                    .Where(vv => vv.Field == "IndividualId" && vv.Data.Length > 0)
                    .ToDictionary(vv => vv.Data, vv => vv.PeopleId);
            }
            else
            {
                _numericPeopleIds = JobDbContext.PeopleExtras
                    .Where(vv => vv.Field == "IndividualId" && vv.IntValue != null)
                    .ToDictionary(vv => vv.IntValue ?? 0, vv => vv.PeopleId);
            }

            UploadPeople(rt, ws);
            TryDeleteAllContributions();
            UploadPledges(rt, pkg);
            UploadGifts(rt, pkg);

            rt.Completed = DateTime.Now;
            ProgressDbContext.SubmitChanges();

            return true;
        }

        internal override void StoreIds(Person p, dynamic a)
        {
            if (AlphaNumericIds)
            {
                p.AddEditExtraText("IndividualId", (string)a.IndividualId);

                if (!Testing)
                {
                    _alphanumericPeopleIds.Add(a.IndividualId, p.PeopleId);
                }
            }
            else
            {
                p.AddEditExtraInt("IndividualId", (int)a.IndividualId);

                if (!Testing)
                {
                    _numericPeopleIds.Add((int)a.IndividualId, p.PeopleId);
                }
            }
        }

        private void UploadPledges(UploadPeopleRun rt, ExcelPackage pkg)
        {
            var data = FetchPledgeData(pkg.Workbook.Worksheets["Pledges"]).ToList();
            rt.Count = data.Count;
            rt.Description = $"Uploading Pledges {(Testing ? "in testing mode" : "for real")}";
            rt.Processed = 0;
            ProgressDbContext.SubmitChanges();

            var weeks = (from g in data
                         group g by g.Date.Sunday()
                into weeklypledges
                         select weeklypledges).ToList();
            BundleHeader bh = null;

            foreach (var week in weeks)
            {
                FinishBundle(JobDbContext, bh);

                bh = new BundleHeader
                {
                    BundleHeaderTypeId = BundleTypeCode.Pledge,
                    BundleStatusId = BundleStatusCode.Closed,
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Today,
                    ContributionDate = week.Key
                };
                foreach (var pledge in week)
                {
                    var pid = GetPeopleId(pledge);
                    var f = new ContributionFund { FundId = 0 };
                    if (!Testing)
                    {
                        if (!pid.HasValue)
                        {
                            if (IgnoreMissingGifts)
                            {
                                _orphanedPledges.Append($"{pledge.IndividualId} {pledge.Date:d} {pledge.Amount:C}\n");
                                continue;
                            }

                            throw new Exception($"peopleid not found from individualid {pledge.IndividualId}");
                        }

                        f = ProgressDbContext.FetchOrCreateFund(pledge.FundId,
                            pledge.FundName ?? pledge.FundDescription);
                        f.FundPledgeFlag = true;
                    }

                    var bd = new BundleDetail
                    {
                        CreatedBy = Util.UserId,
                        CreatedDate = DateTime.Now,
                        Contribution = new Contribution
                        {
                            CreatedBy = Util.UserId,
                            CreatedDate = DateTime.Now,
                            ContributionDate = pledge.Date,
                            FundId = f.FundId,
                            ContributionStatusId = 0,
                            ContributionTypeId = ContributionTypeCode.Pledge,
                            ContributionAmount = pledge.Amount,
                            PeopleId = pid
                        }
                    };
                    bh.BundleDetails.Add(bd);

                    // save orphaned pledges
                    if (!Testing)
                    {
                        var currentOrphans = JobDbContext.Content("OrphanedPledges", "---", ContentTypeCode.TypeText);
                        currentOrphans.Body = _orphanedPledges.ToString();
                        JobDbContext.SubmitChanges();
                    }

                    rt.Processed++;
                    ProgressDbContext.SubmitChanges();
                }
            }

            FinishBundle(JobDbContext, bh);
        }

        private void UploadGifts(UploadPeopleRun rt, ExcelPackage pkg)
        {
            var data = FetchContributionData(pkg.Workbook.Worksheets["Gift Data"]).ToList();
            rt.Count = data.Count;
            rt.Description = $"Uploading Gifts {(Testing ? "in testing mode" : "for real")}";
            rt.Processed = 0;
            ProgressDbContext.SubmitChanges();

            var weeks = (from g in data
                         group g by g.Date.Sunday()
                into weeklygifts
                         select weeklygifts).ToList();
            BundleHeader bh = null;

            foreach (var week in weeks)
            {
                FinishBundle(JobDbContext, bh);

                bh = new BundleHeader
                {
                    BundleHeaderTypeId = BundleTypeCode.ChecksAndCash,
                    BundleStatusId = BundleStatusCode.Closed,
                    CreatedBy = Util.UserId,
                    CreatedDate = DateTime.Today,
                    ContributionDate = week.Key
                };
                foreach (var gift in week)
                {
                    var pid = GetPeopleId(gift);
                    if (!Testing)
                    {
                        if (!pid.HasValue)
                        {
                            if (IgnoreMissingGifts)
                            {
                                _orphanedGifts.Append($"{gift.IndividualId} {gift.Date:d} {gift.Amount:C}\n");

                                continue;
                            }

                            throw new Exception($"peopleid not found from individualid {gift.IndividualId}");
                        }
                    }

                    if (!Testing)
                    {
                        var f = ProgressDbContext.FetchOrCreateFund(gift.FundId, gift.FundName ?? gift.FundDescription);
                        if (gift.FundId == 0)
                        {
                            gift.FundId = f.FundId;
                        }
                    }

                    var bd = new BundleDetail
                    {
                        CreatedBy = Util.UserId,
                        CreatedDate = DateTime.Now,
                        Contribution = new Contribution
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
                        }
                    };
                    bh.BundleDetails.Add(bd);

                    // save orphaned gifts
                    if (!Testing)
                    {
                        var currentOrphans = JobDbContext.Content("OrphanedGifts", "---", ContentTypeCode.TypeText);
                        currentOrphans.Body = _orphanedGifts.ToString();
                        JobDbContext.SubmitChanges();
                    }

                    rt.Processed++;
                    ProgressDbContext.SubmitChanges();
                }
            }

            FinishBundle(JobDbContext, bh);
        }

        private void FinishBundle(CMSDataContext db, BundleHeader bh)
        {
            if (Testing)
            {
                return;
            }

            if (bh == null)
            {
                return;
            }

            bh.TotalChecks = bh.BundleDetails.Sum(d => d.Contribution.ContributionAmount);
            bh.TotalCash = 0;
            bh.TotalEnvelopes = 0;
            db.BundleHeaders.InsertOnSubmit(bh);
            db.SubmitChanges();
        }

        public IEnumerable<PledgeGift> FetchContributionData(ExcelWorksheet ws)
        {
            FetchHeaderColumns(ws);
            const string sheet = "Gift Data";
            CheckColumn("IndividualId", sheet);
            CheckColumn("Amount", sheet);
            CheckColumn("Date", sheet);
            CheckColumn("FundId", sheet);
            CheckColumn("FundName", sheet);
            CheckColumn("FundDescription", sheet);
            var r = 2;
            while (r <= ws.Dimension.End.Row)
            {
                var row = new PledgeGift
                {
                    IndividualId = ws.Cells[r, Names["IndividualId"]].Value,
                    Amount = GetDecimal(ws.Cells[r, Names["Amount"]].Value),
                    Date = GetDate(ws.Cells[r, Names["Date"]].Value) ?? DateTime.MinValue,
                    FundId = GetInt(ws.Cells[r, Names["FundId"]].Value) ?? 0,
                    FundDescription = GetString(ws.Cells[r, Names["FundDescription"]].Value),
                    FundName = GetString(ws.Cells[r, Names["FundName"]].Value)
                };
                if (Names.ContainsKey("CheckNo"))
                {
                    row.CheckNo = GetString(ws.Cells[r, Names["CheckNo"]].Value);
                }

                r++;
                yield return row;
            }
        }

        public IEnumerable<PledgeGift> FetchPledgeData(ExcelWorksheet ws)
        {
            FetchHeaderColumns(ws);
            const string sheet = "Pledges";
            CheckColumn("IndividualId", sheet);
            CheckColumn("PledgeAmount", sheet);
            CheckColumn("PledgeDate", sheet);
            CheckColumn("FundId", sheet);
            CheckColumn("FundName", sheet);
            CheckColumn("FundDescription", sheet);

            var r = 2;
            while (r <= ws.Dimension.End.Row)
            {
                var row = new PledgeGift
                {
                    IndividualId = ws.Cells[r, Names["IndividualId"]].Value,
                    Amount = GetDecimal(ws.Cells[r, Names["PledgeAmount"]].Value),
                    Date = GetDate(ws.Cells[r, Names["PledgeDate"]].Value) ?? DateTime.MinValue,
                    FundId = GetInt(ws.Cells[r, Names["FundId"]].Value) ?? 0,
                    FundName = GetString(ws.Cells[r, Names["FundName"]].Value),
                    FundDescription = GetString(ws.Cells[r, Names["FundDescription"]].Value)
                };
                r++;
                yield return row;
            }
        }

        private void TryDeleteAllContributions()
        {
            if (Testing)
            {
                return;
            }

            //var db = DbUtil.Create(Host);
            if (!JobDbContext.Setting("UploadExcelIpsDeleteGifts"))
            {
                return;
            }

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
            JobDbContext.ExecuteCommand(deletesql);
        }

        internal override int? GetPeopleId(dynamic a)
        {
            if (a.IndividualId == null)
            {
                return null;
            }

            if (AlphaNumericIds)
            {
                var id = (string)a.IndividualId;
                if (_alphanumericPeopleIds.ContainsKey(id))
                {
                    return _alphanumericPeopleIds[id];
                }
            }
            else
            {
                var id = (int)a.IndividualId;
                if (_numericPeopleIds.ContainsKey(id))
                {
                    return _numericPeopleIds[id];
                }
            }

            return null;
        }
    }

    public class PledgeGift
    {
        public object IndividualId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string FundName { get; set; }
        public int FundId { get; set; }
        public string FundDescription { get; set; }
        public string CheckNo { get; set; }
    }
}
