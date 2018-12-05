using CmsData;
using CmsData.View;
using CmsWeb.Areas.Search.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Linq;

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
            public string NoticeSent { get; set; }
        }
        public MissionTripSendersModel() { }
        public static EpplusResult List(int id)
        {
            return Result(SenderGifts(id.ToString()));
        }
        public static EpplusResult List(OrgSearchModel m)
        {
            var orgids = string.Join(",", m.FetchOrgs().Select(mm => mm.OrganizationId));
            return Result(SenderGifts(orgids));
        }

        public static IEnumerable<SenderGift> SenderGifts(string orgids)
        {
            var q = from sg in DbUtil.Db.SenderGifts(orgids)
                    select sg;
            return q;
        }

        private static EpplusResult Result(IEnumerable<SenderGift> q)
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
