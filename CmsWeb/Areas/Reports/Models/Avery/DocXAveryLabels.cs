using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;
using Xceed.Words.NET;

namespace CmsWeb.Models
{
    public class DocXAveryLabels : ActionResult
    {
        public int Skip = 0;
        public string Format;
        public bool? Titles;
        public bool? UseMailFlags;
        public bool? SortZip;
        public string Sort => (SortZip ?? false) ? "Zip" : "Name";
        public bool? UsePhone { get; set; }

        private readonly Guid id;
        public DocXAveryLabels(Guid id)
        {
            this.id = id;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var ctl = new MailingController { UseTitles = Titles ?? false, UseMailFlags = UseMailFlags ?? false };
            var response = context.HttpContext.Response;
            IEnumerable<MailingController.MailingInfo> q = null;
            switch (Format)
            {
                case "Individual":
                    q = ctl.FetchIndividualList(Sort, id);
                    break;
                case "GroupAddress":
                    q = ctl.GroupByAddress(Sort, id);
                    break;
                case "Family":
                case "FamilyMembers":
                    q = ctl.FetchFamilyList(Sort, id);
                    break;
                case "ParentsOf":
                    q = ctl.FetchParentsOfList(Sort, id);
                    break;
                case "CouplesEither":
                    q = ctl.FetchCouplesEitherList(Sort, id);
                    break;
                case "CouplesBoth":
                    q = ctl.FetchCouplesBothList(Sort, id);
                    break;
                default:
                    response.Write("unknown format");
                    return;
            }

            var bytes = AveryLabelTemplate() ?? Resource1.DocxAveryLabel;
            var ms = new MemoryStream(bytes);
            var docx = DocX.Load(ms);
            var row = 0;
            var col = 0;
            DocX finaldoc = null;
            DocX currpage = null;
            foreach (var p in q)
            {
                if(currpage == null || (row == 0 && col == 0))
                    currpage = docx.Copy();

                if (Skip > 0)
                {
                    row = Skip/3;
                    col = Skip%3;
                    if (col > 0)
                        col *= 2;
                    Skip = 0;
                }
                var cell = currpage.Tables[0].Rows[row].Cells[col];
                var pg = cell.Paragraphs[0];

                if (Format == "GroupAddress")
                    pg.InsertText(p.LabelName + " " + p.LastName);
                else if ((Format == "CouplesEither" || Format == "CouplesBoth") && p.CoupleName.HasValue())
                    pg.InsertText(p.CoupleName);
                else
                    pg.InsertText(p.LabelName);

                if (p.MailingAddress.HasValue())
                    pg.InsertText($"\n{p.MailingAddress.Trim()}");
                else
                {
                    pg.InsertText($"\n{p.Address}");
                    if (p.Address2.HasValue())
                        pg.InsertText($"\n{p.Address2}");
                    pg.InsertText($"\n{p.CSZ}");
                }
                if (UsePhone == true)
                {
                    var phone = Util.PickFirst(p.CellPhone.FmtFone("C "), p.HomePhone.FmtFone("H "));
                    pg.InsertText($"\n{phone}");
                }

                col+=2;
                if (col != 6)
                    continue;
                col = 0;
                row++;
                if (row != 10)
                    continue;
                row = 0;
                if (finaldoc == null)
                    finaldoc = currpage;
                else
                    finaldoc.InsertDocument(currpage);
                currpage = null;
            }
            if(finaldoc == null && currpage == null)
            {
                response.Write("no data found");
                return;
            }
            if (finaldoc == null)
                finaldoc = currpage;
            else if(currpage != null)
                finaldoc.InsertDocument(currpage);

            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            response.AddHeader("content-disposition", $"attachment;filename=AveryLabels-{DateTime.Now.ToSortableDateTime()}.docx");

            ms = new MemoryStream();
            finaldoc.SaveAs(ms);
            ms.WriteTo(response.OutputStream);
            response.End();
        }

        private static byte[] AveryLabelTemplate()
        {
            var loc = DbUtil.Db.Setting("AveryLabelTemplate", "");
            if (loc.HasValue())
            {
                var wc = new WebClient();
                return wc.DownloadData(loc);
            }
            return null;
        }
    }
}
