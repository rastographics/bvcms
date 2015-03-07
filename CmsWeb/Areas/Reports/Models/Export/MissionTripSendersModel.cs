using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Areas.Org2.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace CmsWeb.Models
{
    public class MissionTripSendersModel
    {
        public class MissionTripSendInfo
        {
            public int OrgId { get; set; }
            public string Trip { get; set; }
            public int SenderId { get; set; }
            public string Sender { get; set; }
            public int? GoerId { get; set; }
            public string Goer { get; set; }
            public DateTime? DateGiven { get; set; }
            public decimal? Amt { get; set; }
        }
        public static EpplusResult List(int id)
        {
            return Result(SenderGifts(id.ToString()));
        }
        public static EpplusResult List(OrgSearchModel m)
        {
            var orgids = string.Join(",", m.FetchOrgs().Select(mm => mm.OrganizationId));
            return Result(SenderGifts(orgids));
        }

        public static IEnumerable<MissionTripSendInfo> SenderGifts(string orgids)
        {
            var q = from sa in DbUtil.Db.GoerSenderAmounts
                join o in DbUtil.Db.Organizations on sa.OrgId equals o.OrganizationId
                join i in DbUtil.Db.SplitInts(orgids) on o.OrganizationId equals i.ValueX
                join s in DbUtil.Db.People on sa.SupporterId equals s.PeopleId
                join g in DbUtil.Db.People on sa.GoerId equals g.PeopleId
                select new MissionTripSendInfo
                {
                    OrgId = o.OrganizationId,
                    Trip = o.OrganizationName,
                    SenderId = sa.SupporterId,
                    Sender = s.Name2,
                    GoerId = sa.GoerId,
                    Goer = g.Name2,
                    DateGiven = sa.Created,
                    Amt = sa.Amount
                };
            return q;
        }

        private static EpplusResult Result(IEnumerable<MissionTripSendInfo> q)
        {
            var list = q.ToList();
            var count = list.Count;
            var cols = typeof(MissionTripSendInfo).GetProperties();
            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");
            if (count == 0)
            {
                ws.Cells["A1"].Value = "nothing to report";
                return new EpplusResult(ep, "MissionTripSenders.xlsx");
            }
            ws.Cells["A2"].LoadFromCollection(list);
            var range = ws.Cells[1, 1, count + 1, cols.Length];
            var table = ws.Tables.Add(range, "Trips");
            table.TableStyle = TableStyles.Light9;
            table.ShowFilter = false;
            for (var i = 0; i < cols.Length; i++)
            {
                var col = i + 1;
                var name = cols[i].Name;
                table.Columns[i].Name = name;
                var colrange = ws.Cells[1, col, count + 2, col];
                if (name == "DateGiven")
                {
                    colrange.Style.Numberformat.Format = "mm-dd-yy";
                    colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Column(col).Width = 12;
                }
                else if (name == "Amt")
                {
                    colrange.Style.Numberformat.Format = "#,##0.00";
                    colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Column(col).Width = 8;
                }
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return new EpplusResult(ep, "MissionTripSenders.xlsx");
        }
    }
}