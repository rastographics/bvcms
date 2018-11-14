using CmsData;
using CmsData.Codes;
using CsvHelper;
using Dapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class StonyRunFriendsImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            using (var csv = new CsvReader(new StringReader(text)))
            {
                return Import(csv, date);
            }
        }

        private static int? Import(CsvReader csv, DateTime date)
        {
            var bundleHeader = BatchImportContributions.GetBundleHeader(date, DateTime.Now);

            var list = new List<DepositRecord>();
            var n = 0;
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var r = new DepositRecord() { Row = ++n, Valid = true };
                r.Date = csv["Date"].ToDate();
                r.PeopleId = csv["PeopleId"].ToInt();
                r.CheckNo = csv["Check#"];
                var typ = csv["Type"];
                r.FundId = csv["Fund"].ToInt2();
                r.Amount = csv["Amount"];
                r.Description = csv["Notes"];
                if (!r.Date.HasValue)
                {
                    r.AddError("Missing Date");
                }

                if (!r.FundId.HasValue)
                {
                    r.AddError("Missing Fund");
                }

                if (r.PeopleId == 0)
                {
                    r.AddError("Missing PeopleId");
                }

                if (0 == DbUtil.Db.Connection.ExecuteScalar<int>(
                        "SELECT IIF(EXISTS(SELECT NULL FROM dbo.People WHERE PeopleId = @pid), 1, 0)", new { pid = r.PeopleId }))
                {
                    r.AddError("Cannot Find Person");
                }

                if (0 == DbUtil.Db.Connection.ExecuteScalar<int>(
                        "SELECT IIF(EXISTS(SELECT NULL FROM dbo.ContributionFund WHERE FundId = @fid), 1, 0)", new { fid = r.FundId }))
                {
                    r.AddError("Cannot Find Fund");
                }

                switch (typ)
                {
                    case "PL":
                        r.TypeId = ContributionTypeCode.Pledge;
                        break;
                    case "NT":
                        r.TypeId = ContributionTypeCode.NonTaxDed;
                        break;
                    case "GK":
                        r.TypeId = ContributionTypeCode.GiftInKind;
                        break;
                    case "SK":
                        r.TypeId = ContributionTypeCode.Stock;
                        break;
                    case "CN":
                        r.TypeId = ContributionTypeCode.CheckCash;
                        break;
                    default:
                        r.AddError("missing/unknown type");
                        break;
                }
                list.Add(r);
            }
            if (list.Any(vv => vv.Valid == false))
            {
                throw new Exception("The following errors were found<br>\n" +
                    string.Join("<br>\n", list.Where(vv => vv.Valid == false).Select(vv => vv.RowError()).ToArray()));
            }

            foreach (var r in list)
            {
                if (!r.Date.HasValue || !r.FundId.HasValue)
                {
                    continue;
                }

                var bd = BatchImportContributions.AddContributionDetail(r.Date.Value, r.FundId.Value, r.Amount, r.PeopleId);
                var c = bd.Contribution;
                c.ContributionDesc = r.Description;
                c.CheckNo = r.CheckNo;
                bundleHeader.BundleDetails.Add(bd);
            }
            BatchImportContributions.FinishBundle(bundleHeader);
            return bundleHeader.BundleHeaderId;
        }
    }
}
