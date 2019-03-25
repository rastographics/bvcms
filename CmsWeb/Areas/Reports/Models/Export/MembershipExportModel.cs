using CmsData;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Linq;

namespace CmsWeb.Models
{
    public class MembershipExportModel
    {
        public class MembershipInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public string DecisionType { get; set; }
            public DateTime? DecisionDate { get; set; }
            public string BaptismType { get; set; }
            public string BaptismStatus { get; set; }
            public DateTime? BaptismDate { get; set; }
            public DateTime? BaptismSchedDate { get; set; }
            public string NewMemberClassStatus { get; set; }
            public DateTime? NewMemberClassDate { get; set; }
            public string MemberStatus { get; set; }
            public string JoinType { get; set; }
            public DateTime? JoinDate { get; set; }
            public string OtherPreviousChurch { get; set; }
            public string DropType { get; set; }
            public DateTime? DropDate { get; set; }
            public string OtherNewChurch { get; set; }
            public string LetterStatus { get; set; }
            public DateTime? LetterDateRequested { get; set; }
            public DateTime? LetterDateReceived { get; set; }
        }
        public MembershipExportModel() { }
        public static EpplusResult MembershipInfoList(Guid queryid)
        {
            var q = from p in DbUtil.Db.PeopleQuery(queryid)
                    select new MembershipInfo()
                    {
                        PeopleId = p.PeopleId,
                        Name = p.Name,
                        DecisionDate = p.DecisionDate,
                        DecisionType = p.DecisionType.Description,
                        BaptismType = p.BaptismType.Description,
                        BaptismStatus = p.BaptismStatus.Description,
                        BaptismDate = p.BaptismDate,
                        BaptismSchedDate = p.BaptismSchedDate,
                        NewMemberClassStatus = p.NewMemberClassStatus.Description,
                        NewMemberClassDate = p.NewMemberClassDate,
                        MemberStatus = p.MemberStatus.Description,
                        JoinType = p.JoinType.Description,
                        JoinDate = p.JoinDate,
                        OtherPreviousChurch = p.OtherPreviousChurch,
                        DropType = p.DropType.Description,
                        DropDate = p.DropDate,
                        OtherNewChurch = p.OtherNewChurch,
                        LetterStatus = p.MemberLetterStatus.Description,
                        LetterDateRequested = p.LetterDateRequested,
                        LetterDateReceived = p.LetterDateReceived,
                    };
            var cols = typeof(MembershipInfo).GetProperties();
            var count = q.Count();
            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A2"].LoadFromCollection(q);
            var range = ws.Cells[1, 1, count + 1, cols.Length];
            var table = ws.Tables.Add(range, "Members");
            table.TableStyle = TableStyles.Light9;
            table.ShowFilter = false;
            for (var i = 0; i < cols.Length; i++)
            {
                var col = i + 1;
                var name = cols[i].Name;
                table.Columns[i].Name = name;
                var colrange = ws.Cells[1, col, count + 2, col];

                if (name.Contains("Date"))
                {
                    colrange.Style.Numberformat.Format = "mm-dd-yy";
                    colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Column(col).Width = 12;
                }
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return new EpplusResult(ep, "MembershipInfo.xlsx");
        }
    }
}
